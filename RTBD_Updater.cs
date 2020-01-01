//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 39 $
//#      $Date: 2009-02-23 10:45:55 +0000 (Mon, 23 Feb 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/old/RTBD_Updater.cs $
//#
//#      $Id: RTBD_Updater.cs 39 2009-02-23 10:45:55Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Auto-Updater for RTB
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBD_Updater = 1;

//*********************************************************
//* Load Switchboard
//*********************************************************
%RTBDU_SB = RTB_createSwitchboard("DU","APIUM");
%RTBDU_SB.registerLine(1,1);

//*********************************************************
//* Request Gateway
//*********************************************************
//- RTBDU_SendRequest (compiles arguments into POST string for transfer)
function RTBDU_SendRequest(%cmd,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6,%arg7,%arg8,%arg9,%arg10)
{
   for(%i=1;%i<11;%i++)
   {
      %string = %string@strReplace(urlEnc(%arg[%i]),"\t","")@"\t";
   }
   RTB_SB_DU.placeCall(%line,%cmd,%string);
}

//*********************************************************
//* Meat
//*********************************************************
//- RTBDU_Update (sends a request with current version to find newer ones)
%RTBDU_SB.registerResponseHandler("GETUPDATES","RTBDU_onUpdateReply");
function checkRTBUpdates()
{
   RTBDU_SendRequest("GETUPDATES",1,$RTB::Beta,$RTB::Version);
}

//- RTBDU_onUpdateFail (if the connection fails)
%RTBDU_SB.registerFailHandler("GETUPDATES","RTBDU_onUpdateFail");
function RTBDU_onUpdateFail()
{
      echo("");
      RTBDU_drawSpacer();
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("Could not connect to RTB Update Routing Service.");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawSpacer();
      echo("");
}

//- RTBCU_onUpdateReply (reply from version controller)
function RTBCU_onUpdateReply(%tcp,%line,%av)
{
   if(%av)
   {
      $RTB::DUpdater::Cache::Version = getField(%line,1);
      
      echo("");
      RTBDU_drawSpacer();
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("RTB v"@getField(%line,1)@" is Available");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("File Size: "@byteRound(getField(%line,3)));
      RTBDU_drawDOSRow("Release Date: "@getField(%line,2));
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("** Type doRTBUpdate(); to Install this Version **");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawSpacer();
      echo("");
   }
   else
   {
      echo("");
      RTBDU_drawSpacer();
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("No RTB Updates are available at this time.");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawSpacer();
      echo("");
   }
}

//*********************************************************
//* Update Downloading
//*********************************************************
//- RTBDU_InitFC (creates a file connection)
function RTBDU_InitFC()
{
   if(isObject(RTBDU_FC))
   {
      RTBDU_FC.disconnect();
      RTBDU_FC.delete();
   }
      
   new TCPObject(RTBDU_FC)
   {
      host = "api.returntoblockland.com";
      port = 80;
   };
}

//- doRTBUpdate (attempts to download an rtb update)
function doRTBUpdate()
{
   if($RTB::DUpdater::Cache::Version $= "")
   {
      echo("");
      RTBDU_drawSpacer();
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("No Version Selected!");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("Please do checkRTBUpdates(); first!");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawSpacer();
      echo("");
      return;
   }
   
   RTBDU_InitFC();
   
   RTBCU_FC.targetVersion = $RTB::DUpdater::Cache::Version;
   RTBCU_FC.connect(RTBDU_FC.host@":"@RTBDU_FC.port);
}

//- RTBDU_FC::onConnected (connection success callback)
function RTBDU_FC::onConnected(%this)
{
   %content = "c=DLUPDATE&n="@$Pref::Player::NetName@"&arg1="@%this.targetVersion@"&"@$RTB::Connection::Session;
   %contentLen = strLen(%content);
   
   %this.send("POST /apiRouter.php?d=APIUM HTTP/1.1\r\nHost: api.returntoblockland.com\r\nUser-Agent: Torque/1.0\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: "@%contentLen@"\r\n\r\n"@%content@"\r\n");
}

//- RTBDU_FC::onLine (callback for line response)
function RTBDU_FC::onLine(%this,%line)
{
   if(strPos(%line,"404 Not Found") >= 0)
      %error = 1;
   
   if(strPos(%line,"DL-Result:") $= 0)
      %error = 1;
   
   if(%error)
   {
      echo("");
      RTBDU_drawSpacer();
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("An error occurred while downloading RTB v"@%this.targetVersion);
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawSpacer();
      echo("");
      RTBDU_InitFC();
      return;
   }
   
   if(strPos(%line,"Content-Length:") $= 0)
      %this.contentSize = getWord(%line,1);
      
   if(%line $= "")
      %this.setBinarySize(%this.contentSize);
}

//- RTBDU_FC::onBinChunk (on binary chunk received)
function RTBDU_FC::onBinChunk(%this,%chunk)
{
   if(%chunk >= %this.contentSize)
   {
      %this.saveBufferToFile("Add-Ons/System_ReturnToBlockland.zip");
      %this.disconnect();
      
      echo("");
      RTBDU_drawSpacer();
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("RTB v"@%this.targetVersion@" has been downloaded and installed successfully.");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("You must now restart your server.");
      RTBDU_drawDOSRow("");
      RTBDU_drawDOSRow("");
      RTBDU_drawSpacer();
      echo("");
   }
}
checkRTBUpdates();