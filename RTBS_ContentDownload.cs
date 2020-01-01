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
//#   Content Download (RTBSD/SContentDownload)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBS_ContentDownload = 1;

//*********************************************************
//* Content Notification
//*********************************************************
//- GameConnection::transmitAddons (sends all the add-ons to the client)
function GameConnection::transmitAddons(%client)
{
   %mission = $Server::MissionFile;
   if(strpos(%mission,"Add-Ons") $= 0)
   {
      %zip = getSubStr(%mission,8,strLen(%mission));
      %zip = getSubStr(%zip,0,strPos(%zip,"/"));
      if(isFile("Add-Ons/"@%zip@"/rtbInfo.txt"))
      {
         %fileObject = new FileObject();
         if(%fileObject.openForRead("Add-Ons/"@%zip@"/rtbInfo.txt"))
         {
            while(!%fileObject.isEOF())
            {
               %line = %fileObject.readLine();
               if(striPos(%line,"id:") !$= -1)
               {
                  %id = getWord(%line,1);
                  commandtoclient(%client,'RTB_receiveMap',%id,strReplace(%mission,".mis",""));
                  break;
               }
            }
            %fileObject.close();
         }
         %fileObject.delete();
      }
   }
   
   %list = RTBSD_getModList();
   %list = getWords(%list,1,getWordCount(%list));
   while(getWordCount(%list) > 0)
   {
      if(getWordCount(%list) >= 40)
      {
         commandtoclient(%client,'RTB_receiveAddons',getWords(%list,0,40));
         %list = getWords(%list,41,getWordCount(%list));
      }
      else
      {
         commandtoclient(%client,'RTB_receiveAddons',getWords(%list,0,getWordCount(%list)));
         %list = "";
      }
   }
   commandtoclient(%client,'RTB_receiveAddonsComplete');
}

//*********************************************************
//* Support
//*********************************************************
function RTBSD_getModList()
{
   if($RTB::SContentDownload::Cache::ModArray !$= "")
      return $RTB::SContentDownload::Cache::ModArray;
   
   %modArray = "n ";
   %filepath = findFirstFile("Add-Ons/*_*/server.cs");
   while(strlen(%filepath) > 0)
   {
      %zip = getSubStr(%filepath,8,strLen(%filepath)-strLen("/server.cs")-strLen("Add-Ons/"));
      if($AddOn__[%zip] $= 1)
      {
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
         
         if(%type $= "rtb")
         {
            %modArray = %modArray@%id@" ";
         }
      }
      %filepath = findNextFile("Add-Ons/*_*/server.cs");
   }
   if(strLen(%modArray) > 0)
      %modArray = getSubStr(%modArray,0,strLen(%modArray)-1);
      
   $RTB::SContentDownload::Cache::ModArray = %modArray;

   return %modArray;
}

//*********************************************************
//* Module Package
//*********************************************************
package RTBS_ContentDownload
{
   function GameConnection::loadMission(%this)
   {
      if(%this.isAIControlled())
      {
         Parent::loadMission(%this);
      }
      else
      {
         if(%this.hasRTB && %this.rtbVersion > 3.3)
         {
            %this.currentPhase = -1;
            if(getWordCount(RTBSD_getModList()) >= 2 && findLocalClient() !$= %this)
            {
               %this.currentPreparePhase = 0;
               commandToClient(%this,'MissionPreparePhase1');
            }
            else
            {
               %this.currentPreparePhase = 1;
               commandToClient(%this,'MissionPreparePhase2',RTBRT_getControlCRC(),RTBRT_GUIManifest.getCount(),RTBRT_GUIManifest.elements);
            }
            echo("*** Sending mission load to client: " @ $Server::MissionFile);
         }
         else
            Parent::loadMission(%this);
      }
   }
   
   function serverCmdMissionPreparePhase1Ack(%client,%skip)
   {
      if(%client.currentPhase !$= "-1" || %client.currentPreparePhase !$= "0")
         return;
         
      if(%skip)
      {
         %client.currentPreparePhase = 1;
         commandToClient(%client,'MissionPreparePhase2',RTBRT_getControlCRC(),RTBRT_GUIManifest.getCount(),RTBRT_GUIManifest.elements);
         return;
      }
      %client.currentPreparePhase = 1;
      %client.transmitAddons();
   }
   
   function serverCmdMissionPreparePhase1End(%client)
   {
      if(%client.currentPhase !$= "-1" || %client.currentPreparePhase !$= "1")
         return;
         
      commandToClient(%client,'MissionPreparePhase2',RTBRT_getControlCRC(),RTBRT_GUIManifest.getCount(),RTBRT_GUIManifest.elements);
   }
};
activatePackage(RTBS_ContentDownload);