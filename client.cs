//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 112 $
//#      $Date: 2009-09-05 18:17:49 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/client.cs $
//#
//#      $Id: client.cs 112 2009-09-05 17:17:49Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Client Initiation
//#
//#############################################################################

//*********************************************************
//* Debug
//* -------------------------------------------------------
//* Level 0 -> Off
//* Level 1 -> Low
//* Level 2 -> High
//* Level 3 -> API Profiling/Debugging (DO NOT USE)
//*********************************************************
$RTB::Debug = 0;

//*********************************************************
//* Demo Users
//* -------------------------------------------------------
//* Believe it or not this is here for a reason. Go ahead
//* and remove it and be amazed and astounded when every-
//* thing falls to shit before your fucking eyes. Idiots.
//*********************************************************
if(!isUnlocked())
   return;

//*********************************************************
//* RTB Variables
//*********************************************************
$RTB::Version = "3.5";
$RTB::Path = "Add-Ons/System_ReturnToBlockland/";

//*********************************************************
//* Load Preferences
//*********************************************************
$RTB::Options::AU::Enable = 1;
$RTB::Options::CA::AuthWithRTB = 1;
$RTB::Options::CA::Privacy::ShowOnline = 1;
$RTB::Options::CA::Privacy::ShowServer = 1;
$RTB::Options::CD::DownloadContent = 1;
$RTB::Options::GT::Enable = 1;
$RTB::Options::IC::AllowPM = 1;
$RTB::Options::IC::AutoConnect = 1;
$RTB::Options::IC::PMAudioNotify = 1;
$RTB::Options::IC::PMVisualNotify = 1;
$RTB::Options::IC::Filter::ShowActions = 1;
$RTB::Options::IC::Filter::ShowConnects = 1;
$RTB::Options::IC::Filter::ShowDisconnects = 0;
$RTB::Options::IT::Enable = 1;
$RTB::Options::IT::ShowAddonTips = 1;
$RTB::Options::MM::AnimateGUI = 1;
$RTB::Options::MM::WarnFailed = 1;
$RTB::Options::MM::DisableFailed = 1;
$RTB::Options::MM::CheckForUpdates = 1;
$RTB::Options::MM::DownloadScreenshots = 1;
$RTB::Options::SA::PostServer = 1;
$RTB::Options::SA::Privacy::ShowPlayers = 1;

if(isFile("config/client/rtb/prefs.cs"))
	exec("config/client/rtb/prefs.cs");
else
{
   echo("Exporting rtb prefs");
   export("$RTB::Options*","config/client/rtb/prefs.cs");
}

if(isFile("config/server/ADD_ON_LIST.cs"))
   exec("config/server/ADD_ON_LIST.cs");
else
   exec("base/server/defaultAddonList.cs");
   
$AddOn__System_ReturnToBlockland = 1;
export("$AddOn__*","config/server/ADD_ON_LIST.cs");

//*********************************************************
//* RTB Support Functions
//*********************************************************
//- RTB_LoadDefaultPrefs (Deletes RTB variables and replaces them with default ones)
function RTB_LoadDefaultPrefs()
{
   deleteVariables("$RTB::Options*");
   $RTB::Options::AU::Enable = 1;
   $RTB::Options::CA::AuthWithRTB = 1;
   $RTB::Options::CA::Privacy::ShowOnline = 1;
   $RTB::Options::CA::Privacy::ShowServer = 1;
   $RTB::Options::GT::Enable = 1;
   $RTB::Options::IC::AllowPM = 1;
   $RTB::Options::IC::AutoConnect = 1;
   $RTB::Options::IC::PMAudioNotify = 1;
   $RTB::Options::IC::PMVisualNotify = 1;
   $RTB::Options::IC::Filter::ShowActions = 1;
   $RTB::Options::IC::Filter::ShowConnects = 1;
   $RTB::Options::IC::Filter::ShowDisconnects = 0;
   $RTB::Options::IT::Enable = 1;
   $RTB::Options::IT::ShowAddonTips = 1;
   $RTB::Options::MM::AnimateGUI = 1;
   $RTB::Options::MM::WarnFailed = 1;
   $RTB::Options::MM::DisableFailed = 1;
   $RTB::Options::MM::CheckForUpdates = 1;
   $RTB::Options::MM::DownloadScreenshots = 1;
   $RTB::Options::SA::PostServer = 1;
   $RTB::Options::SA::Privacy::ShowPlayers = 1;
   echo("Exporting rtb prefs");
   export("$RTB::Options*","config/client/rtb/prefs.cs");
}

//- If this pref is unset, we have outdated prefs so purge & reload
if($RTB::Options::CA::AuthWithRTB $= "")
{
   RTB_LoadDefaultPrefs();
}

//*********************************************************
//* Load Prerequisites
//*********************************************************
exec("./RTBC_Profiles.cs");
exec("./RTBH_Support.cs");

//*********************************************************
//* Load Modules
//*********************************************************
exec("./RTBC_Authentication.cs");
exec("./RTBC_ColorManager.cs");
exec("./RTBC_ContentDownload.cs");
exec("./RTBC_GUITransfer.cs");
exec("./RTBC_InfoTips.cs");
exec("./RTBC_IRCClient.cs");
exec("./RTBC_Manual.cs");
exec("./RTBC_ModManager.cs");
exec("./RTBC_Options.cs");
exec("./RTBC_ServerControl.cs");
exec("./RTBC_ServerInformation.cs");
exec("./RTBC_Updater.cs");

//*********************************************************
//* Load Replacement GUI
//*********************************************************
if(isObject(AddOnsGui))
{
   AddOnsGui.delete();
   exec("./BL_AddOnsGui.gui");
}

if(isObject(JoinServerGui))
{
   JoinServerGui.clear();
   exec("./BL_JoinServerGui.gui");
   JoinServerGui.add(RTBJS_window);
   RTBJS_window.setName("JS_window");
}

if(!isObject(MM_RTBForumsButton))
{
   %btn = new GuiBitmapButtonCtrl(MM_RTBForumsButton)
   {
      profile = "GuiDefaultProfile";
      horizSizing = "relative";
      vertSizing = "relative";
      position = "416 240";
      extent = "224 40";
      minExtent = "8 2";
      visible = "1";
      text = " ";
      groupNum = "-1";
      buttonType = "PushButton";
      bitmap = $RTB::Path@"images/buttons/menu/btnForums";
      command = "gotoWebPage(\"http://forum.returntoblockland.com/\");";
      lockAspectRatio = "1";
      alignLeft = "1";
      overflowImage = "0";
      mKeepCached = "1";
      mColor = "255 255 255 255";
   };
   MainMenuGui.add(%btn);
}
function MM_RTBForumsButton::onMouseEnter(%this)
{
   if($Pref::Audio::MenuSounds)
	   alxPlay(Note11Sound);
}

if(!isObject(MM_RTBLogo))
{
   %bitmap = new GuiBitmapCtrl(MM_RTBLogo)
   {
      profile = "GuiDefaultProfile";
      horizSizing = "relative";
      vertSizing = "relative";
      position = "460 -10";
      extent = "193 131";
      minExtent = "8 2";
      visible = "1";
      bitmap = $RTB::Path@"images/image_logo";
      lockAspectRatio = "1";
      alignLeft = "1";
      overflowImage = "0";
      mKeepCached = "1";
      mColor = "255 255 255 255";
   };
   MainMenuGui.add(%bitmap);
   
   %bitmap = new GuiBitmapCtrl(MM_RTBAmpersand)
   {
      profile = "GuiDefaultProfile";
      horizSizing = "relative";
      vertSizing = "relative";
      position = "414 16";
      extent = "36 64";
      minExtent = "8 2";
      visible = "1";
      bitmap = $RTB::Path@"images/image_ampersand";
      lockAspectRatio = "1";
      alignLeft = "1";
      overflowImage = "0";
      mKeepCached = "1";
      mColor = "255 255 255 255";
   };
   MainMenuGui.add(%bitmap);
   
   %swatch = new GuiSwatchCtrl()
   {
      profile = "GuiDefaultProfile";
      horizSizing = "relative";
      vertSizing = "relative";
      position = "385 160";
      extent = "95 45";
      minExtent = "8 2";
      visible = "1";
      color = "0 0 0 0";
   };
   MainMenuGui.add(%swatch);
   
   %swatch = new GuiSwatchCtrl()
   {
      profile = "GuiDefaultProfile";
      horizSizing = "relative";
      vertSizing = "relative";
      position = "368 200";
      extent = "146 80";
      minExtent = "8 2";
      visible = "1";
      color = "0 0 0 0";
   };
   MainMenuGui.add(%swatch);
   
   %swatch = new GuiSwatchCtrl()
   {
      profile = "GuiDefaultProfile";
      horizSizing = "relative";
      vertSizing = "relative";
      position = "397 278";
      extent = "167 80";
      minExtent = "8 2";
      visible = "1";
      color = "0 0 0 0";
   };
   MainMenuGui.add(%swatch);
   
   %version = new GuiTextCtrl()
   {
      profile = "RTB_VersionProfile";
      horizSizing = "left";
      vertSizing = "bottom";
      position = "469 1";
      extent = "165 16";
      minExtent = "8 2";
      visible = "1";
      text = "Version: "@$RTB::Version;
   };
   MainMenuGui.add(%version);
}

//*********************************************************
//* RTB Client Package
//*********************************************************
package RTB_Client
{
   function disconnectedCleanup()
   {
      deleteVariables("$RTB::CServerControl::Cache::*");
      Parent::disconnectedCleanup();
   }
   
	function onExit()
	{
		Parent::onExit();
		echo("Exporting rtb prefs");
		export("$RTB::Options*","config/client/rtb/prefs.cs");
	}
	
   function GameConnection::setConnectArgs(%a,%b,%c,%d,%e,%f)
   {
      Parent::setConnectArgs(%a,%b,%c,%d,%e,%f,$RTB::Version);
   }
   
   function loadMainMenu()
   {
      Parent::loadMainMenu();
      
      RTBMM_checkForUpdates();
      
      if($platform !$= "windows")
      {
         RTBCA_getWinFonts();
      }
   }
};
activatePackage(RTB_Client);