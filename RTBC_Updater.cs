//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 39 $
//#      $Date: 2009-02-23 10:45:55 +0000 (Mon, 23 Feb 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/old/RTBC_Updater.cs $
//#
//#      $Id: RTBC_Updater.cs 39 2009-02-23 10:45:55Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Client Updater (RTBCU/RTBCUpdater)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBC_Updater = 1;

//*********************************************************
//* Initialisation of required objects
//*********************************************************
if(!isObject(RTB_Updater))
	exec("./RTB_Updater.gui");
	
//*********************************************************
//* Load Switchboard
//*********************************************************
%RTBCU_SB = RTB_createSwitchboard("CU","APIUM");
%RTBCU_SB.registerLine(1,1);

//*********************************************************
//* Request Gateway
//*********************************************************
//- RTBCU_SendRequest (Compiles arguments into POST string for transfer)
function RTBCU_SendRequest(%cmd,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6,%arg7,%arg8,%arg9,%arg10)
{
   for(%i=1;%i<11;%i++)
   {
      %string = %string@strReplace(urlEnc(%arg[%i]),"\t","")@"\t";
   }
   RTB_SB_CU.placeCall(%line,%cmd,%string);
}

//*********************************************************
//* Meat
//*********************************************************
//- RTBCU_Update (Sends a request with current version to find newer ones)
%RTBCU_SB.registerResponseHandler("GETUPDATES","RTBCU_onUpdateReply");
function RTBCU_Update()
{
   if($RTB::Options::AU::Enable && !$RTB::CUpdater::Cache::HasBeenPrompted)
   {
      $RTB::CUpdater::Cache::HasBeenPrompted = 1;
      RTBCU_SendRequest("GETUPDATES",1,$RTB::Beta,$RTB::Version);
   }
}

//- RTBCU_onUpdateReply (Reply from version controller)
function RTBCU_onUpdateReply(%tcp,%line,%av)
{
   if(%av)
   {
      %version = getField(%line,1);
      %date = getField(%line,2);
      %filesize = getField(%line,3);
      
      canvas.pushDialog(RTB_Updater);
      RTBCU_Version.setText("v"@%version);
      RTBCU_Date.setText(%date);
      RTBCU_Size.setText(byteRound(%filesize));
      RTBCU_Speed.setText("N/A");
      RTBCU_Done.setText("0kb");
      
      RTBCU_Progress.setValue(0);
      RTBCU_ProgressText.setValue("Ready to Download");
      
      RTBCU_UpdateButton.setActive(1);
      RTBCU_UpdateButton.command = "RTBCU_downloadUpdate(\""@%version@"\");";
      RTBCU_ChangeLogButton.command = "RTBCU_getChangeLog(\""@%version@"\");";
   }
}

//- RTBCU_getChangeLog (Retrieves the version changelog)
%RTBCU_SB.registerResponseHandler("GETCHANGELOG","RTBCU_onChangeLog");
function RTBCU_getChangeLog(%version)
{
   RTBCU_SendRequest("GETCHANGELOG",1,%version);
   RTBCU_ChangeLog_Title.setText("Change Log for RTB v"@%version@":");
   RTBCU_ChangeLog_Text.setText("");
   
   MessagePopup("Please Wait","Locating Change Log for RTB v"@%version@"...");
}

//- RTBCU_onChangeLog (Upon receiving changelog data)
function RTBCU_onChangeLog(%this,%line)
{
   if(%line $= 0)
   {
      MessageBoxOK("Oh Dear","The Change Log for this update could not be found. Sorry.");
      MessagePopup("","",1);
   }
   else
      RTBCU_ChangeLog_Text.setText(RTBCU_ChangeLog_Text.getText()@%line@"<br>");
}

//- RTBCU_onChangeLogStop (Callback for end of changelog transmission)
function RTBCU_onChangeLogStop()
{
   if(RTBCU_ChangeLog_Text.getText() !$= "")
   {
      canvas.pushDialog(RTBCU_ChangeLog);
   }
   MessagePopup("","",1);
}
RTBCU_Update();

//*********************************************************
//* Update Downloading
//*********************************************************
//- RTBCU_InitFC (Creates a file connection)
function RTBCU_InitFC()
{
   if(isObject(RTBCU_FC))
   {
      RTBCU_FC.disconnect();
      RTBCU_FC.delete();
   }
      
   new TCPObject(RTBCU_FC)
   {
      host = "api.returntoblockland.com";
      port = 80;
   };
}

//- RTBCU_downloadUpdate (Attempts to download an rtb update)
function RTBCU_downloadUpdate(%version)
{
   if(!isWriteableFilename("Add-Ons/System_ReturnToBlockland.zip"))
   {
      MessageBoxOK("Oh Dear!","Your System_ReturnToBlockland.zip is read-only and cannot be overwritten.\n\nPlease download the latest RTB manually from our website, or set System_ReturnToBlockland.zip to not be read-only.");
      return;
   }
   RTBCU_InitFC();
   
   RTBCU_FC.targetVersion = %version;
   RTBCU_FC.connect(RTBCU_FC.host@":"@RTBCU_FC.port);
   
   RTBCU_UpdateButton.setActive(0);
   RTBCU_ProgressText.setText("Locating Update...");
}

//- RTBCU_FC::onConnected (Connection success callback)
function RTBCU_FC::onConnected(%this)
{
   %content = "c=DLUPDATE&n="@$Pref::Player::NetName@"&arg1="@%this.targetVersion@"&"@$RTB::Connection::Session;
   %contentLen = strLen(%content);
   
   %this.send("POST /apiRouter.php?d=APIUM HTTP/1.1\r\nHost: api.returntoblockland.com\r\nUser-Agent: Torque/1.0\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: "@%contentLen@"\r\n\r\n"@%content@"\r\n");
}

//- RTBCU_FC::onLine (Callback for line response)
function RTBCU_FC::onLine(%this,%line)
{
   if(strPos(%line,"404 Not Found") >= 0)
   {
      %this.disconnect();
      MessageBoxOK("Error!","An error occured with the updater service and the update could not be located.");
      canvas.popDialog(RTB_Updater);
      return;
   }
   
   if(strPos(%line,"DL-Result:") $= 0)
   {
      %this.dlResult = getWord(%line,1);
      if(getWord(%line,1) $= 0)
      {
         %this.disconnect();
         MessageBoxOK("Error!","An error occured with the updater service and the update could not be located.");
         Canvas.popDialog(RTB_Updater);
         return;
      }
   }
   
   if(strPos(%line,"Content-Length:") $= 0)
      %this.contentSize = getWord(%line,1);
      
   if(%line $= "")
   {
      if(%this.dlResult !$= 1)
      {
         %this.disconnect();
         MessageBoxOK("Error!","An error occured with the updater service and the update could not be located.");
         Canvas.popDialog(RTB_Updater);
         return;
      }
      %this.setBinarySize(%this.contentSize);
   }
}

//- RTBCU_FC::onBinChunk (On binary chunk received)
function RTBCU_FC::onBinChunk(%this,%chunk)
{
   if(%this.timeStarted $= "")
      %this.timeStarted = getSimTime();
      
   if(%chunk >= %this.contentSize)
   {
      if(isWriteableFilename("Add-Ons/System_ReturnToBlockland.zip"))
      {
         %this.saveBufferToFile("Add-Ons/System_ReturnToBlockland.zip");
         %this.disconnect();
         
         RTBCU_Progress.setValue(1);
         RTBCU_ProgressText.setText("Download Complete");
         RTBCU_Speed.setText("N/A");
         RTBCU_Done.setText(byteRound(%this.contentSize));
         
         MessageBoxOK("Huzzah!","You have successfully downloaded RTB v"@%this.targetVersion@".\n\nBlockland must now close to complete the install.","quit();");
      }
      else
      {
         MessageBoxOK("Oh Dear!","Unable to save RTB v"@%this.targetVersion@". Your System_ReturnToBlockland.zip is Read-Only and cannot be overwritten.\n\nPlease download the latest RTB manually from our website.");
      }
   }
   else
   {
      RTBCU_Progress.setValue(%chunk/%this.contentSize);
      RTBCU_ProgressText.setText(mFloor((%chunk/%this.contentSize)*100)@"%");
      RTBCU_Speed.setText(mFloatLength(%chunk/(getSimTime()-%this.timeStarted),2)@"kb/s");
      RTBCU_Done.setText(byteRound(%chunk));
   }
}