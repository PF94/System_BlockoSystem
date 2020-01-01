//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 108 $
//#      $Date: 2009-09-05 11:39:30 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/RTBS_GUITransfer.cs $
//#
//#      $Id: RTBS_GUITransfer.cs 108 2009-09-05 10:39:30Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Content Download (RTBCD/CContentDownload)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBC_ContentDownload = 1;

//*********************************************************
//* Variable Declarations
//*********************************************************
$RTB::CContentDownload::Addons = 0;

//*********************************************************
//* Load TCP Object
//*********************************************************
%RTBCD_SB = RTB_createSwitchboard("CD","APIMS");
%RTBCD_SB.registerLine(1,0);
%RTBCD_SB.setLineProperty(1,"No-Retries",1);
$RTB::CContentDownload::Switchboard = %RTBCD_SB;

//*********************************************************
//* Load TCP Object
//*********************************************************
//- RTBMM_SendRequest (Compiles arguments into POST string for transfer)
function RTBCD_SendRequest(%cmd,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6,%arg7,%arg8,%arg9,%arg10)
{
   for(%i=1;%i<11;%i++)
   {
      %arg[%i] = strReplace(%arg[%i],"\n","<br>");
      %string = %string@strReplace(urlEnc(%arg[%i]),"\t","")@"\t";
   }
   RTB_SB_CD.placeCall(%line,%cmd,%string);
}

//*********************************************************
//* Connection Phases
//*********************************************************
//- clientCmdMissionPreparePhase1 (begins phase 1 of the mission preparation)
function clientCmdMissionPreparePhase1()
{
   if(!$RTB::Options::CD::DownloadContent)
   {
      echo("*** Prep-Phase 1: Download RTB Add-Ons - Skipped by preference");
      commandtoserver('MissionPreparePhase1Ack',1);
      return;
   }
   
   if($RTB::CContentDownload::Cache::Downloading)
   {
      RTBMM_TransferQueue.clear();
      RTBMM_FileGrabber_Init();
   }
      
   echo("*** Prep-Phase 1: Download RTB Add-Ons");
   deleteVariables("$RTB::CContentDownload::Cache::*");
   $RTB::CContentDownload::Addons = 0;
   
   LoadingProgressTxt.setText("GETTING SERVER RTB ADD-ONS");
   
   commandtoserver('MissionPreparePhase1Ack');
   
   $RTB::CContentDownload::Cache::Receiving = 1;
}

//*********************************************************
//* Add-on Download Functions
//*********************************************************
//- clientCmdRTB_receiveMap (receives the map from the server)
function clientCmdRTB_receiveMap(%id,%img)
{
   if(!$RTB::CContentDownload::Cache::Receiving)
      return;
      
   $RTB::CContentDownload::Addon[$RTB::CContentDownload::Addons] = %id;
   $RTB::CContentDownload::Addons++;
   
   $RTB::CContentDownload::Cache::Map = %id;
   $RTB::CContentDownload::Cache::MapImage = %img;
}

//- clientCmdRTB_receiveAddons (receives add-ons from the server)
function clientCmdRTB_receiveAddons(%addons)
{
   if(!$RTB::CContentDownload::Cache::Receiving)
      return;
      
   for(%i=0;%i<getWordCount(%addons);%i++)
   {
      %id = getWord(%addons,%i);
      $RTB::CContentDownload::Addon[$RTB::CContentDownload::Addons] = %id;
      $RTB::CContentDownload::Addons++;
   }
}

//- clientCmdRTB_receiveAddonsComplete (server's done sending add-ons, lets download them)
function clientCmdRTB_receiveAddonsComplete()
{
   if(!$RTB::CContentDownload::Cache::Receiving)
      return;
      
   $RTB::CContentDownload::Cache::Receiving = 0;
      
   %addons = RTBCD_getModList();
   for(%i=0;%i<$RTB::CContentDownload::Addons;%i++)
   {
      %add = 0;
      %id = $RTB::CContentDownload::Addon[%i];
      for(%k=0;%k<getFieldCount(%addons);%k++)
      {
         %addon = getField(%addons,%k);
         if(firstWord(%addon) $= %id)
         {
            %add = 1;
            %list = %list @ $RTB::CContentDownload::Addon[%i] @ "-"@ restWords(%addon) @ ".";
            break;
         }
      }
      
      if(!%add)
         %list = %list @ $RTB::CContentDownload::Addon[%i] @ "-0.";
   }
   
   if(%list $= "")
   {
      commandtoserver('MissionPreparePhase1End');
      return;
   }
   RTBCD_SendRequest("VERIFYADDONS",1,%list);
   LoadingProgressTxt.setText("VERIFYING RTB ADD-ONS");
}

//- RTBCD_onVerifyReply (reply back from rtb server)
%RTBCD_SB.registerResponseHandler("VERIFYADDONS","RTBCD_onVerifyReply");
%RTBCD_SB.registerFailHandler("VERIFYADDONS","RTBCD_onVerifyFail");
function RTBCD_onVerifyReply(%tcp,%line)
{
   %files = strReplace(%line,"."," ");
   if(getWordCount(%files) <= 0)
   {
      commandtoserver('MissionPreparePhase1End');
      return;
   }
   
   $RTB::CContentDownload::Addons = 0;
   for(%i=0;%i<getWordCount(%files);%i++)
   {
      %file = getWord(%files,%i);
      
      $RTB::CContentDownload::Addon[$RTB::CContentDownload::Addons] = %file;
      $RTB::CContentDownload::Addons++;
      
      RTBCD_downloadContent(%file);
   }
   $RTB::CContentDownload::Cache::Downloading = 1;
}

//- RTBCD_onVerifyFail (if the reply fails to be returned)
function RTBCD_onVerifyFail(%tcp,%line)
{
   commandtoserver('MissionPreparePhase1End');
}

//- RTBCD_downloadContent (downloads required content)
function RTBCD_downloadContent(%id)
{
   %queue = RTBMM_TransferQueue.addItem(%id,1);
}

//*********************************************************
//* Support Functions
//*********************************************************
function RTBCD_getModList()
{
   if($RTB::CContentDownload::Cache::ModArray !$= "")
      return $RTB::CContentDownload::Cache::ModArray;
   
   %filepath = findFirstFile("Add-Ons/*_*/server.cs");
   while(strlen(%filepath) > 0)
   {
      %zip = getSubStr(%filepath,8,strLen(%filepath)-strLen("/server.cs")-strLen("Add-Ons/"));

      %type = "";
      %id = "";
      %version = "";
      
      %fileObject = new FileObject();
      if(%fileObject.openForRead("Add-Ons/"@%zip@"/rtbInfo.txt"))
      {
         %type = "rtb";
         while(!%fileObject.isEOF())
         {
            %line = %fileObject.readLine();
            if(striPos(%line,"id:") !$= -1)
            {
               %id = getWord(%line,1);
            }
            else if(striPos(%line,"version:") !$= -1)
            {
               %version = getWord(%line,1);
            }
            else if(striPos(%line,"name:") !$= -1)
            {
               %type = "rtb2";
            }
         }
         %fileObject.close();
      }
      %fileObject.delete();
      
      if(%type $= "rtb")
      {
         %mod[%id] = 1;
         %modArray = %modArray@%id@" "@%version@"\t";
      }

      %filepath = findNextFile("Add-Ons/*_*/server.cs");
   }
   
   %filepath = findFirstFile("Add-Ons/*_*/rtbInfo.txt");
   while(strlen(%filepath) > 0)
   {
      %zip = getSubStr(%filepath,8,strLen(%filepath)-strLen("/rtbInfo.txt")-strLen("Add-Ons/"));

      %type = "";
      %id = "";
      %version = "";
      
      %fileObject = new FileObject();
      if(%fileObject.openForRead("Add-Ons/"@%zip@"/rtbInfo.txt"))
      {
         %type = "rtb";
         while(!%fileObject.isEOF())
         {
            %line = %fileObject.readLine();
            if(striPos(%line,"id:") !$= -1)
            {
               %id = getWord(%line,1);
            }
            else if(striPos(%line,"version:") !$= -1)
            {
               %version = getWord(%line,1);
            }
            else if(striPos(%line,"name:") !$= -1)
            {
               %type = "rtb2";
            }
         }
         %fileObject.close();
      }
      %fileObject.delete();
      
      if(%type $= "rtb" && !%mod[%id])
      {
         %mod[%id] = 1;
         %modArray = %modArray@%id@" "@%version@"\t";
      }

      %filepath = findNextFile("Add-Ons/*_*/rtbInfo.txt");
   }
   
   %filepath = findFirstFile("Add-Ons/*_*/rtbContent.txt");
   while(strlen(%filepath) > 0)
   {
      %zip = getSubStr(%filepath,8,strLen(%filepath)-strLen("/rtbContent.txt")-strLen("Add-Ons/"));

      %id = "";
      %version = "";
         
      %fileObject = new FileObject();
      if(%fileObject.openForRead("Add-Ons/"@%zip@"/rtbContent.txt"))
      {
         while(!%fileObject.isEOF())
         {
            %line = %fileObject.readLine();
            if(striPos(%line,"id:") !$= -1)
            {
               %id = getWord(%line,1);
            }
            else if(striPos(%line,"version:") !$= -1)
            {
               %version = getWord(%line,1);
            }
         }
         %fileObject.close();
      }
      %fileObject.delete();

      if(!%mod[%id])
         %modArray = %modArray@%id@" "@%version@"\t";

      %filepath = findNextFile("Add-Ons/*_*/rtbContent.txt");
   }   
   
   if(strLen(%modArray) > 0)
      %modArray = getSubStr(%modArray,0,strLen(%modArray)-1);
      
   $RTB::CContentDownload::Cache::ModArray = %modArray;

   return %modArray;
}

//*********************************************************
//* Module Package
//*********************************************************
package RTBC_ContentDownload
{
   function disconnectedCleanup()
   {
      if($RTB::CContentDownload::Cache::Downloading)
      {
         RTBMM_TransferQueue.clear();
         RTBMM_FileGrabber_Init();
      }
         
      deleteVariables("$RTB::CContentDownload::Cache::*");
      $RTB::CContentDownload::Addons = 0;
      Parent::disconnectedCleanup();
   }
};
activatePackage(RTBC_ContentDownload);