//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 39 $
//#      $Date: 2009-02-23 10:45:55 +0000 (Mon, 23 Feb 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/old/RTBC_ServerControl.cs $
//#
//#      $Id: RTBC_ServerControl.cs 39 2009-02-23 10:45:55Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Server Control (RTBSC/CServerControl)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBC_ServerControl = 1;

//*********************************************************
//* Variable Declarations
//*********************************************************
$RTB::CServerControl::SO::Cats = 0;
$RTB::CServerControl::SO::Options = 0;

//*********************************************************
//* Initialisation of required objects
//*********************************************************
if(!isObject(RTB_ServerControl))
	exec("./RTB_ServerControl.gui");

if(!$RTB::CServerControl::AppliedMaps)
{
   RTB_addControlMap("keyboard","ctrl s","Server Control","RTBSC_ToggleSC");
   $RTB::CServerControl::AppliedMaps = 1;
}

//- RTBSC_ToggleSC (Toggles the server control window)
function RTBSC_ToggleSC(%val)
{
   if(!%val)
      return;
      
   if($IamAdmin !$= 2)
      return;
      
   if(!$RTB::CServerControl::Cache::ServerHasRTB)
      return;
      
   if(RTB_ServerControl.isAwake())
      canvas.popDialog(RTB_ServerControl);
   else
      canvas.pushDialog(RTB_ServerControl);
}

//*********************************************************
//* Settings Registration
//*********************************************************
//- RTBSC_cacheServerOption (Creates a manifest of options for the server to reference - what a mess!)
function RTBSC_cacheServerOption(%optionName,%type,%style,%description,%category)
{
   if($RTB::CServerControl::Cache::DownloadedServerOptions)
      return;
      
   if($RTB::CServerControl::SO::Options[%category] $= "")
   {
      $RTB::CServerControl::SO::Cat[$RTB::CServerControl::SO::Cats] = %category;
      $RTB::CServerControl::SO::Cats++;
      
      $RTB::CServerControl::SO::Options[%category] = 0;
   }
   $RTB::CServerControl::SO::CatOption[%category,$RTB::CServerControl::SO::Options[%category]] = %optionName TAB %type TAB %style TAB %description TAB $RTB::CServerControl::SO::Options;
   $RTB::CServerControl::SO::Option[$RTB::CServerControl::SO::Options] = %optionName TAB %type TAB %style TAB %description;
   $RTB::CServerControl::SO::Options++;
   $RTB::CServerControl::SO::Options[%category]++;
}

//*********************************************************
//* Server Options Definitions (Must match on server)
//*********************************************************
RTBSC_cacheServerOption("Server Name","string 150","100 7 200 18","The Server Name is what is displayed to people who are browsing for servers to join.","Main Settings");
RTBSC_cacheServerOption("Welcome Message","string 255","100 7 200 18","The Welcome Message is what is sent to users when they join the server. %1 is replaced with the player's name.","Main Settings");
RTBSC_cacheServerOption("Max Players","playerlist 1 32","100 7 84 16","The Max Players is the max ammount of people allowed in the server. This includes the Server Host and the Admin. You can set this to less than the current number of players in the server.","Main Settings");
RTBSC_cacheServerOption("Server Password","string 30","100 7 200 18","The Server Password prevents people without the password from joining the server.","Main Settings");
RTBSC_cacheServerOption("Admin Password","string 30","100 7 200 18","The Admin Password allows people to enter a password to become Admin.","Main Settings");
RTBSC_cacheServerOption("Super Admin Password","string 30","130 7 170 18","The Super Admin Password allows people to enter a password to become a Super Admin.","Main Settings");
RTBSC_cacheServerOption("E-Tard Filer","bool","80 0 140 30","The E-Tard Filter prevents people from saying words in the e-tard word list.","Chat Settings");
RTBSC_cacheServerOption("E-Tard Words","string 255","80 7 220 18","Words should be separated by commas. A space before and after means it must be a whole word, not a part of a bigger one.","Chat Settings");
RTBSC_cacheServerOption("Max Bricks per Second","int 0 999","200 7 100 18","The Max Bricks per Second is how many bricks users are allowed to place per second. For fast macroing, this should be set to 0.","Gameplay Settings");
RTBSC_cacheServerOption("Falling Damage","bool","200 0 140 30","Falling Damage means falling from large heights will kill players.","Gameplay Settings");
RTBSC_cacheServerOption("\"Too Far\" Distance for Building","int 0 9999","200 7 100 18","The Too Far distance is how close people have to be to their ghost brick to plant it.","Gameplay Settings");
RTBSC_cacheServerOption("Wrench Events Admin Only","bool","200 0 140 30"," Wrench Events can be made Admin Only by using this setting.","Gameplay Settings");
RTBSC_cacheServerOption("Bricks lose ownership after (minutes)","int -1 99999","200 7 100 18","This will make bricks lose their ownership if the owner is gone for more than the number of minutes in the box. This means any player will be able to build on or destroy those bricks. -1 disables this.","Gameplay Settings");

//*********************************************************
//* Prefs Registration
//*********************************************************
//- RTBSC_cacheServerPref (Creates a manifest of prefs for the server to reference)
function RTBSC_cacheServerPref(%id,%prefName,%category,%type,%requiresRestart)
{
   if($RTB::CServerControl::Cache::DownloadedServerPrefs)
      return;
      
   if($RTB::CServerControl::Cache::SP::Prefs[%category] $= "")
   {
      $RTB::CServerControl::Cache::SP::Cat[$RTB::CServerControl::Cache::SP::Cats] = %category;
      $RTB::CServerControl::Cache::SP::Cats++;
      
      $RTB::CServerControl::Cache::SP::Prefs[%category] = 0;
   }
   $RTB::CServerControl::Cache::SP::CatPref[%category,$RTB::CServerControl::Cache::SP::Prefs[%category]] = %id TAB %prefName TAB %type TAB %requiresRestart;
   $RTB::CServerControl::Cache::SP::Pref[$RTB::CServerControl::Cache::SP::Prefs] = %id TAB %prefName TAB %type TAB %requiresRestart;
   $RTB::CServerControl::Cache::SP::Prefs++;
   $RTB::CServerControl::Cache::SP::Prefs[%category]++;
}

//*********************************************************
//* Main Control
//*********************************************************
package RTBC_ServerControl
{
   function disconnectedCleanup()
   {
     RTBSC_SO_Swatch.clear();
     deleteVariables("$RTB::CServerControl::Cache*");
     Parent::disconnectedCleanup();
   }
    
   function adminGui::onWake(%this)
   {
      Parent::onWake(%this);
      
      if(isObject(rtbServerControlBtn))
      {
         if($IamAdmin !$= 2 || $RTB::CServerControl::Cache::ServerHasRTB !$= 1)
            rtbServerControlBtn.delete();
         return;
      }
      
      if($IamAdmin !$= 2 || $RTB::CServerControl::Cache::ServerHasRTB !$= 1)
         return;
         
      %btn = new GuiBitmapButtonCtrl(rtbServerControlBtn)
      {
         profile = BlockButtonProfile;
         horizSizing = "left";
         vertSizing = "bottom";
         position = "205 275";
         extent = "98 38";
         command = "canvas.pushDialog(RTB_ServerControl);";
         text = "Server Control";
         bitmap = "base/client/ui/button1";
         mcolor = "100 255 50 255";
      };
      adminGui.getObject(0).add(%btn);
   }
   
   function unBanGui::onWake(%this)
   {
      Parent::onWake(%this);
      
      if(isObject(rtbClearBansBtn))
      {
         if($IamAdmin !$= 2 || $RTB::CServerControl::Cache::ServerHasRTB !$= 1)
            rtbClearBansBtn.delete();
         return;
      }
      
      if($IamAdmin !$= 2 || $RTB::CServerControl::Cache::ServerHasRTB !$= 1)
         return;
         
      %btn = new GuiBitmapButtonCtrl(rtbClearBansBtn)
      {
         profile = BlockButtonProfile;
         horizSizing = "left";
         vertSizing = "bottom";
         position = "502 100";
         extent = "91 38";
         command = "unBanGui.clickClear();";
         text = "Clear All";
         bitmap = "base/client/ui/button2";
         mcolor = "255 0 0 255";
      };
      unBanGui.getObject(0).add(%btn);
   }
   
   function NewPlayerListGui::update(%this,%client,%name,%blid,%isAdmin,%isSuperAdmin,%score)
   {
      Parent::update(%this,%client,%name,%blid,%isAdmin,%isSuperAdmin,%score);
      
      if($RTB::CServerControl::Cache::currentTab $= 2)
         RTBSC_Pane2::onView(RTBSC_Pane2);
   }
};
activatePackage(RTBC_ServerControl);

//- RTB_ServerControl::onWake (onwake callback for gui)
function RTB_ServerControl::onWake(%this)
{
   for(%i=3;%i<%this.getObject(0).getCount();%i++)
   {
      %pane = %this.getObject(0).getObject(%i);
      if(%pane.visible)
      {
         %activePane = %pane.getName();
         break;
      }
   }
   
   if(!isObject(%activePane))
   {
      RTB_ServerControl.setTab(1);
      %activePane = "RTBSC_Pane1";
      return;
   }
   %call = %activePane@"::onView("@%activePane.getID()@");";
   eval(%call);
}

//- RTB_ServerControl::setTab (Sets the tab for the server control window)
function RTB_ServerControl::setTab(%this,%id)
{
   for(%i=5;%i<%this.getObject(0).getCount();%i++)
   {
      %pane = %this.getObject(0).getObject(%i);
      %pane.visible = 0;
   }
   %pane = "RTBSC_Pane"@%id;
   %pane.visible = 1;
   %call = %pane@"::onView("@%pane@");";
   eval(%call);
   
   $RTB::CServerControl::Cache::currentTab = %id;
}

//*********************************************************
//* Server Options (SO) Pane 1
//*********************************************************
//- RTBSC_Pane1::onView (Callback for viewing Pane 1)
function RTBSC_Pane1::onView(%this)
{
   RTBSC_applySettingsToControls();
   
   commandtoserver('RTB_getServerOptions');
   RTBSC_SO_Tip.setText("Click a Setting Name to view some information about it.");
}

//- RTBSC_Pane1::render (Draws the list of items)
function RTBSC_Pane1::render()
{
   %ctrl = RTBSC_SO_Swatch.getID();
   %ctrl.clear();
   %y = 1;
   
   for(%i=0;%i<$RTB::CServerControl::SO::Cats;%i++)
   {
      %category = $RTB::CServerControl::SO::Cat[%i];

      %header = new GuiSwatchCtrl() {
         profile = "GuiDefaultProfile";
         horizSizing = "right";
         vertSizing = "bottom";
         position = "2 "@%y;
         extent = "330 30";
         minExtent = "8 2";
         visible = "1";
         color = "0 0 0 50";
         
         new GuiTextCtrl() {
            profile = "RTBMM_MainText";
            horizSizing = "right";
            vertSizing = "center";
            position = "5 6";
            extent = "200 18";
            minExtent = "8 2";
            visible = "1";
            text = "\c1"@%category;
            maxLength = "255";
         };
      };
      %ctrl.add(%header);
      %ctrl.resize(0,1,316,%y+31);
      %y+=31;
      
      for(%k=0;%k<$RTB::CServerControl::SO::Options[%category];%k++)
      {
         %option = $RTB::CServerControl::SO::CatOption[%category,%k];
         
         if(%k%2 $= 0)
            %color = "0 0 0 10";
         else
            %color = "0 0 0 0";
            
         %ctrlRow = new GuiSwatchCtrl() {
            profile = "GuiDefaultProfile";
            horizSizing = "right";
            vertSizing = "bottom";
            position = "2 "@%y;
            extent = "353 29";
            minExtent = "8 2";
            visible = "1";
            color = %color;
            
            new GuiTextCtrl() {
               profile = "GuiTextProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = "5 5";
               extent = "66 18";
               minExtent = "8 2";
               visible = "1";
               text = "\c2"@getField(%option,0)@":";
               maxLength = "255";
            };
         };
         %ctrl.add(%ctrlRow);
         
         %type = getField(%option,1);
         %style = getField(%option,2);
         %settingTip = new GuiBitmapButtonCtrl()
         {
            position = "2 "@%y;
            extent = getWord(%style,0)-10@" 29";
            bitmap = "base/client/ui/button1";
            text = " ";
            mColor = "0 0 0 0";
            command = "RTBSC_displaySettingTip("@getField(%option,4)@");";
         };
         %ctrl.add(%settingTip);
         
         %inputName = "RTBSC_SO_Opt"@getField(%option,4);
         if(firstWord(%type) $= "string")
         {
            %input = new GuiTextEditCtrl(%inputName) {
               profile = "RTBMM_TextEditProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = getWords(%style,0,1);
               extent = getWords(%style,2,3);
               minExtent = "8 2";
               visible = "1";
               maxLength = getWord(%type,1);
               historySize = "0";
               password = "0";
               tabComplete = "0";
               sinkAllKeyEvents = "0";
            };
            %ctrlRow.add(%input);
         }
         else if(firstWord(%type) $= "int")
         {
            %input = new GuiTextEditCtrl(%inputName) {
               profile = "RTBMM_TextEditProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = getWords(%style,0,1);
               extent = getWords(%style,2,3);
               minExtent = "8 2";
               visible = "1";
               maxLength = "50";
               historySize = "0";
               password = "0";
               tabComplete = "0";
               sinkAllKeyEvents = "0";
               fieldMin = getWord(%type,1);
               fieldMax = getWord(%type,2);
               command = "RTBSC_PF_ValidateInt($ThisControl);";
            };
            %ctrlRow.add(%input);
         }
         else if(firstWord(%type) $= "bool")
         {
            %input = new GuiCheckboxCtrl(%inputName) {
               profile = "RTBMM_CheckboxProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = getWords(%style,0,1);
               extent = getWords(%style,2,3);
               minExtent = "8 2";
               visible = "1";
               text = " ";
            };
            %ctrlRow.add(%input);
         }
         else if(firstWord(%type) $= "playerlist")
         {
            %input = new GuiPopupMenuCtrl(%inputName) {
               profile = "RTBMM_PopupProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = getWords(%style,0,1);
               extent = getWords(%style,2,3);
               minExtent = "8 2";
               visible = "1";
               text = " ";
            };
            %ctrlRow.add(%input);
            
            for(%l=getWord(%type,1);%l<=getWord(%type,2);%l++)
            {
               %s = "s";
               if(%l $= 1)
                  %s = "";
               %input.add(%l@" Player"@%s,%l);
            }
         }
         %ctrl.resize(0,1,316,%y+30);
         %y+=30;
      }
   }
}

//- RTBSC_displaySettingTip (Allows user to click a setting and view details on it)
function RTBSC_displaySettingTip(%id)
{
   RTBSC_SO_Tip.setText(getField($RTB::CServerControl::SO::Option[%id],3));
}

//- clientCmdRTB_getServerOptions (Response from server when requesting settings)
function clientCmdRTB_getServerOptions(%options,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%v10,%v11,%v12,%v13,%v14,%v15,%v16)
{
   for(%i=0;%i<getWordCount(%options);%i++)
   {
      if(getWord(%options,%i) $= "D")
      {
         $RTB::CServerControl::Cache::DownloadedServerOptions = 1;
         continue;
      }
      
      %oldSetting = $RTB::CServerControl::SO::Value[getWord(%options,%i)];
      %value = %v[%i+1];
      %value = strReplace(%value,"\c0","\\c0");
      %value = strReplace(%value,"\c1","\\c1");
      %value = strReplace(%value,"\c2","\\c2");
      %value = strReplace(%value,"\c3","\\c3");
      %value = strReplace(%value,"\c4","\\c4");
      %value = strReplace(%value,"\c5","\\c5");
      %value = strReplace(%value,"\c6","\\c6");
      %value = strReplace(%value,"\c7","\\c7");
      %value = strReplace(%value,"\c8","\\c8");
      %value = strReplace(%value,"\c9","\\c9");
      %value = strReplace(%value,"\n","\\n");
      $RTB::CServerControl::SO::Value[getWord(%options,%i)] = %value;
      %ctrl = "RTBSC_SO_Opt"@getWord(%options,%i);
      
      if(RTB_ServerControl.isAwake() && $RTB::CServerControl::Cache::currentTab $= 1)
      {
         if(%ctrl.getClassName() $= "GuiPopupMenuCtrl")
            %oldValue = %ctrl.getSelected();
         else
            %oldValue = %ctrl.getValue();
            
         if(%oldSetting !$= %oldValue && $RTB::CServerControl::Cache::DownloadedServerOptions)
         {
            MessageBoxYesNo("Uh Oh!","Another user has changed some settings that you have changed but not saved. Would you like to replace your changes with the new values?","RTBSC_applySettingsToControls();");
         }
         else
         {
            if(%ctrl.getClassName() $= "GuiPopupMenuCtrl")
               %ctrl.setSelected(%value);
            else
               %ctrl.setValue(%value);
         }
      }
   }
}

//- RTBSC_applySettingsToControls (Applies all the cached server options to their controls)
function RTBSC_applySettingsToControls()
{
   for(%i=0;%i<$RTB::CServerControl::SO::Options;%i++)
   {
      %option = $RTB::CServerControl::SO::Option[%i];
      %ctrl = "RTBSC_SO_Opt"@%i;

      if(!isObject(%ctrl))
         continue;
         
      %value = $RTB::CServerControl::SO::Value[%i];
      
      if(firstWord(getField(%option,1)) $= "playerlist")
         %ctrl.setSelected(%value);
      else
         %ctrl.setValue(%value);
   }
}

//- RTBSC_Pane1::saveOptions (Save all the changes and send to the server)
function RTBSC_Pane1::saveOptions(%this)
{
   for(%i=0;%i<$RTB::CServerControl::SO::Options;%i++)
   {
      %option = $RTB::CServerControl::SO::Option[%i];
      %ctrl = "RTBSC_SO_Opt"@%i;
      if(firstWord(getField(%option,1)) $= "playerlist")
         %value = %ctrl.getSelected();
      else
         %value = %ctrl.getValue();
         
      if($RTB::CServerControl::SO::Value[%i] !$= %value)
      {
         %changes = %changes@%i@" ";
      }
   }
   
   if(getWordCount(%changes) >= 1 && strLen(%changes) >= 2)
   {
      %changes = getSubStr(%changes,0,strLen(%changes)-1);
      
      for(%i=0;%i<getWordCount(%changes);%i++)
      {
         %option = $RTB::CServerControl::SO::Option[getWord(%changes,%i)];
         %ctrl = "RTBSC_SO_Opt"@getWord(%changes,%i);
         if(firstWord(getField(%option,1)) $= "playerlist")
            %value = %ctrl.getSelected();
         else
            %value = %ctrl.getValue();
         %seq = %i%16;
         
         %var[%seq] = %value;
         %options = %options@getWord(%changes,%i)@" ";
         
         if(%i%16 $= 15 || %i $= getWordCount(%changes)-1)
         {
            if(%numSent > 0)
               %options = %options@"N ";
            if(%i $= getWordCount(%changes)-1)
               %options = %options@"D ";
            %numSent++;
            commandtoserver('RTB_setServerOptions',$RTB::Options::NotifyOfSettings,getSubStr(%options,0,strLen(%options)-1),%var0,%var1,%var2,%var3,%var4,%var5,%var6,%var7,%var8,%var9,%var10,%var11,%var12,%var13,%var14,%var,%var15);
            %options = "";
            for(%j=0;%j<16;%j++)
            {
               %var[%j] = "";
            }
         }
      }
   }
   else
   {
      MessageBoxOK("Ooops","You have not made any changes to save.");
      return;
   }
}

//*********************************************************
//* Auto Admin (AA) Pane 2
//*********************************************************
//- RTBSC_Pane2::onView (Callback for viewing Pane 2)
function RTBSC_Pane2::onView(%this)
{
	commandtoserver('RTB_getAutoAdminList');

	RTBSC_AA_RemoveAuto.setVisible(1);
	RTBSC_AA_Add_Status.clear();
	RTBSC_AA_Add_Status.add("Admin",0);
	RTBSC_AA_Add_Status.add("Super Admin",1);
	RTBSC_AA_Add_Status.setSelected(0);
	RTBSC_AA_Add_ID.setValue("");
	RTBSC_AA_AddStatus.setVisible(0);
	
	RTBSC_AA_PlayerList.clear();
	for(%i=0;%i<NPL_List.rowCount();%i++)
	{
	   %row = getFields(NPL_List.getRowText(%i),0,1) TAB getField(NPL_List.getRowText(%i),3);
	   %id = NPL_List.getRowId(%i);
      RTBSC_AA_PlayerList.addRow(%id,%row);
	}
}

//- RTBSC_AA_AdminList::onSelect
function RTBSC_AA_AdminList::onSelect(%this,%id,%text)
{
	RTBSC_AA_RemoveAuto.setVisible(0);
}

//- RTBSC_Pane2::addAutoStatus (Adds a person as an auto admin)
function RTBSC_Pane2::addAutoStatus(%this)
{
	%bl_id = RTBSC_AA_Add_ID.getValue();
	%status = RTBSC_AA_Add_Status.getValue();

	if(%bl_id $= "" || %bl_id < 0)
		messageBoxOK("Whoopsie Daisy","You haven't entered a valid BL_ID.");
	else if(%status !$= "Admin" && %status !$= "Super Admin")
		messageBoxOK("Whoopsie Daisy","You haven't selected a valid Status.");
	else if(!isInt(%bl_id))
		messageBoxOK("Whoopsie Daisy","You have entered an invalid BL_ID.");
	else
	{
		RTBSC_AA_Add_Status.setSelected(0);
		RTBSC_AA_Add_ID.setValue("");
		RTBSC_AA_AddStatus.setVisible(0);
		commandToServer('RTB_addAutoStatus',%bl_id,%status);
	}
}

//- RTBSC_Pane2::removeAutoStatus (Removes a person from the auto admin list)
function RTBSC_Pane2::removeAutoStatus(%this)
{
	%bl_id = getField(RTBSC_AA_AdminList.getValue(),0);
	if(%bl_id  $= "")
		messageBoxOK("Whoopsie Daisy","You haven't selected a valid BL_ID.");
	else
		commandToServer('RTB_removeAutoStatus',%bl_id);
}

//- clientCmdRTB_getAutoAdminList (Gets the contents of the server admin/super admin lists)
function clientCmdRTB_getAutoAdminList(%adminList,%superAdminList)
{
	RTBSC_AA_AdminList.clear();
	for(%i=0;%i<getWordCount(%superAdminList);%i++)
	{
		%id = getWord(%superAdminList,%i);
		if(%id $= "")
		   continue;
		if(RTBSC_AA_AdminList.findTextIndex(%id TAB "Super Admin") $= -1 && %id >= 0)
			RTBSC_AA_AdminList.addRow(%k++,%id TAB "Super Admin");
	}
	for(%i=0;%i<getWordCount(%adminList);%i++)
	{
		%id = getWord(%adminList,%i);
		if(%id $= "")
		   continue;
		if(RTBSC_AA_AdminList.findTextIndex(%id TAB "Admin") $= -1 && RTBSC_AA_AdminList.findTextIndex(%id TAB "Super Admin") $= -1 && %id >= 0)
			RTBSC_AA_AdminList.addRow(%k++,%id TAB "Admin");
	}
	RTBSC_AA_AdminList.sortNumerical(0,1);
	RTBSC_AA_RemoveAuto.setVisible(1);
}

//- RTBSC_Pane2::clearAll (Clears all the auto admin entries)
function RTBSC_Pane2::clearAll(%this,%confirm)
{
	if(!%confirm)
	{
		MessageBoxYesNo("Really?","Are you sure you want to delete ALL the Auto Admin Entries?","RTBSC_Pane2::clearAll("@%this@",1);","");
		return;
	}
	commandtoserver('RTB_clearAutoAdminList');
}

//- RTBSC_Pane2::addFromList (Adds a selected player to the auto admin list)
function RTBSC_Pane2::addFromList(%this)
{
   RTBSC_AA_AddStatus.setVisible(1);
   RTBSC_AA_Add_ID.setValue(getField(RTBSC_AA_PlayerList.getValue(),2));
}

//- RTBSC_Pane2::deAdmin (De-admins a player)
function RTBSC_Pane2::deAdmin(%this)
{
   %sel = RTBSC_AA_PlayerList.getSelectedID();
   if(%sel $= "-1")
   {
      messageBoxOK("Ooops","You need to select someone from the right list to De-Admin.");
      return;
   }
   commandtoserver('RTB_DeAdminPlayer',%sel);
}

//- RTBSC_Pane2::admin (Sets a player to admin)
function RTBSC_Pane2::admin(%this)
{
   %sel = RTBSC_AA_PlayerList.getSelectedID();
   if(%sel $= "-1")
   {
      messageBoxOK("Ooops","You need to select someone from the right list to Admin.");
      return;
   }
   commandtoserver('RTB_AdminPlayer',%sel);
}

//- RTBSC_Pane2::superAdmin (Sets a player to super admin)
function RTBSC_Pane2::superAdmin(%this)
{
   %sel = RTBSC_AA_PlayerList.getSelectedID();
   if(%sel $= "-1")
   {
      messageBoxOK("Ooops","You need to select someone from the right list to Super Admin.");
      return;
   }
   commandtoserver('RTB_SuperAdminPlayer',%sel);
}

//*********************************************************
//* Preferences (PF) Pane 3
//*********************************************************
//- RTBSC_Pane3::onView (Callback for viewing Pane 3)
function RTBSC_Pane3::onView(%this)
{
   RTBSC_applyPrefsToControls();
}

//- RTBSC_Pane3::render (Draws the list of items)
function RTBSC_Pane3::render()
{
   %ctrl = RTBSC_SP_Swatch.getID();
   %ctrl.clear();
   %y = 1;
   
   if($RTB::CServerControl::Cache::SP::Cats <= 0)
   {
      RTBSC_SP_Swatch.resize(0,1,316,280);
      %txt = new GuiTextCtrl()
      {
         profile = RTB_Verdana12PtAuto;
         position = "48 132";
         text = "This server has no preferences to manage.";
         horizSizing = "center";
         vertSizing = "center";
      };
      RTBSC_SP_Swatch.add(%txt);
      return;
   }
   
   for(%i=0;%i<$RTB::CServerControl::Cache::SP::Cats;%i++)
   {
      %category = $RTB::CServerControl::Cache::SP::Cat[%i];
      
      %header = new GuiSwatchCtrl() {
         profile = "GuiDefaultProfile";
         horizSizing = "right";
         vertSizing = "bottom";
         position = "2 "@%y;
         extent = "330 30";
         minExtent = "8 2";
         visible = "1";
         color = "0 0 0 50";
         
         new GuiTextCtrl() {
            profile = "RTBMM_MainText";
            horizSizing = "right";
            vertSizing = "center";
            position = "5 6";
            extent = "200 18";
            minExtent = "8 2";
            visible = "1";
            text = "\c1"@%category;
            maxLength = "255";
         };
      };
      %ctrl.add(%header);
      %ctrl.resize(0,1,316,%y+31);
      %y+=31;
      
      for(%k=0;%k<$RTB::CServerControl::Cache::SP::Prefs[%category];%k++)
      {
         %pref = $RTB::CServerControl::Cache::SP::CatPref[%category,%k];
         
         if(%k%2 $= 0)
            %color = "0 0 0 10";
         else
            %color = "0 0 0 0";
            
         if(getField(%pref,3))
            %star = "*";
         else
            %star = "";
            
         %ctrlRow = new GuiSwatchCtrl() {
            profile = "GuiDefaultProfile";
            horizSizing = "right";
            vertSizing = "bottom";
            position = "2 "@%y;
            extent = "353 29";
            minExtent = "8 2";
            visible = "1";
            color = %color;
            
            new GuiTextCtrl() {
               profile = "GuiTextProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = "5 5";
               extent = "66 18";
               minExtent = "8 2";
               visible = "1";
               text = "\c2"@getField(%pref,1)@%star@":";
               maxLength = "255";
            };
         };
         %ctrl.add(%ctrlRow);
         
         %type = getField(%pref,2);
         if(firstWord(%type) $= "string")
         {
            %input = new GuiTextEditCtrl() {
               profile = "RTBMM_TextEditProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = "150 7";
               extent = "150 16";
               minExtent = "8 2";
               visible = "1";
               maxLength = getWord(%type,1);
               historySize = "0";
               password = "0";
               tabComplete = "0";
               sinkAllKeyEvents = "0";
            };
            %ctrlRow.add(%input);
         }
         else if(firstWord(%type) $= "int")
         {
            %input = new GuiTextEditCtrl() {
               profile = "RTBMM_TextEditProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = "150 7";
               extent = "150 16";
               minExtent = "8 2";
               visible = "1";
               maxLength = strLen(getWord(%type,2));
               historySize = "0";
               password = "0";
               tabComplete = "0";
               sinkAllKeyEvents = "0";
               fieldMin = getWord(%type,1);
               fieldMax = getWord(%type,2);
               command = "RTBSC_PF_ValidateInt($ThisControl);";
            };
            %ctrlRow.add(%input);
         }
         else if(firstWord(%type) $= "bool")
         {
            %input = new GuiCheckboxCtrl() {
               profile = "RTBMM_CheckboxProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = "156 1";
               extent = "140 30";
               minExtent = "8 2";
               visible = "1";
               text = " ";
            };
            %ctrlRow.add(%input);
         }
         else if(firstWord(%type) $= "list")
         {
            %input = new GuiPopupMenuCtrl() {
               profile = "RTBMM_PopupProfile";
               horizSizing = "right";
               vertSizing = "bottom";
               position = "150 7";
               extent = "150 16";
               minExtent = "8 2";
               visible = "1";
               text = " ";
            };
            %ctrlRow.add(%input);
            
            for(%l=1;%l<getWordCount(%type);%l+=2)
            {
               %input.add(getWord(%type,%l),getWord(%type,%l+1));
            }
         }
         $RTB::CServerControl::Pref[getField(%pref,0)] = %input;
         %ctrl.resize(0,1,316,%y+30);
         %y+=30;
      }
   }
}

//- clientCmdRTB_addServerPrefs (Response from server to populate prefs menu)
function clientCmdRTB_addServerPrefs(%prefs,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%v10,%v11,%v12,%v13,%v14,%v15,%v16)
{
   for(%i=0;%i<getWordCount(%prefs);%i++)
   {
      if(getWord(%prefs,%i) $= "D")
      {
         $RTB::CServerControl::Cache::DownloadedServerPrefs = 1;
         RTBSC_Pane3::render();
         return;
      }
      RTBSC_cacheServerPref(getWord(%prefs,%i),getField(%v[%i+1],0),getField(%v[%i+1],1),getField(%v[%i+1],2),getField(%v[%i+1],3));
   }
}

//- clientCmdRTB_setServerPrefs (Response from server to cache prefs values on the client)
function clientCmdRTB_setServerPrefs(%prefs,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%v10,%v11,%v12,%v13,%v14,%v15,%v16)
{
   for(%i=0;%i<getWordCount(%prefs);%i++)
   {
      %oldPref = $RTB::CServerControl::Cache::SP::Value[getWord(%prefs,%i)];
      %value = %v[%i+1];
      $RTB::CServerControl::Cache::SP::Value[getWord(%prefs,%i)] = %value;
      %ctrl = $RTB::CServerControl::Pref[getWord(%prefs,%i)];
      
      if(RTB_ServerControl.isAwake() && $RTB::CServerControl::Cache::currentTab $= 3)
      {
         if(%ctrl.getClassName() $= "GuiPopupMenuCtrl")
            %oldValue = %ctrl.getSelected();
         else
            %oldValue = %ctrl.getValue();
               
         if(%value !$= %oldValue && $RTB::CServerControl::Cache::DownloadedServerPrefs)
         {
            MessageBoxYesNo("Uh Oh!","Another user has changed some prefs that you have changed but not saved. Would you like to replace your changes with the new values?","RTBSC_applyPrefsToControls();");
         }
         else
         {
            if(%ctrl.getClassName() $= "GuiPopupMenuCtrl")
               %ctrl.setSelected(%value);
            else
               %ctrl.setValue(%value);
         }
      }
   }
}

//- RTBSC_applyPrefsToControls (Applies all the cached server prefs to their controls)
function RTBSC_applyPrefsToControls()
{
   for(%i=0;%i<$RTB::CServerControl::Cache::SP::Prefs;%i++)
   {
      %pref = $RTB::CServerControl::Cache::SP::Pref[%i];
      %ctrl = $RTB::CServerControl::Pref[%i];
      if(!isObject(%ctrl))
         continue;
         
      %value = $RTB::CServerControl::Cache::SP::Value[%i];
      
      if(%ctrl.getClassName() $= "GuiPopupMenuCtrl")
         %ctrl.setSelected(%value);
      else
         %ctrl.setValue(%value);
   }
}

//- RTBSC_Pane3::saveOptions (Saves all the pref changes)
function RTBSC_Pane3::saveOptions()
{
   for(%i=0;%i<$RTB::CServerControl::Cache::SP::Prefs;%i++)
   {
      %pref = $RTB::CServerControl::Cache::SP::Pref[%i];
      %ctrl = $RTB::CServerControl::Pref[%i];
      if(%ctrl.getClassName() $= "GuiPopupMenuCtrl")
         %value = %ctrl.getSelected();
      else
         %value = %ctrl.getValue();
      %storedValue = $RTB::CServerControl::Cache::SP::Value[%i];
      
      if(%value !$= %storedValue)
      {
         %changes = %changes@%i@" ";
      }
   }
   
   if(getWordCount(%changes) >= 1 && strLen(%changes) >= 1)
   {
      %changes = getSubStr(%changes,0,strLen(%changes)-1);
      
      for(%i=0;%i<getWordCount(%changes);%i++)
      {
         %ctrl = $RTB::CServerControl::Pref[getWord(%changes,%i)];
         if(%ctrl.getClassName() $= "GuiPopupMenuCtrl")
            %value = %ctrl.getSelected();
         else
            %value = %ctrl.getValue();
         %seq = %i%16;
         
         %var[%seq] = %value;
         %prefs = %prefs@getWord(%changes,%i)@" ";
         
         if(%i%16 $= 15 || %i $= getWordCount(%changes)-1)
         {
            if(%numSent > 0)
               %prefs = %prefs@"N ";
            if(%i $= getWordCount(%changes)-1)
               %prefs = %prefs@"D ";
            %numSent++;
            commandtoserver('RTB_setServerPrefs',getSubStr(%prefs,0,strLen(%prefs)-1),%var0,%var1,%var2,%var3,%var4,%var5,%var6,%var7,%var8,%var9,%var10,%var11,%var12,%var13,%var14,%var,%var15);
            %prefs = "";
            for(%j=0;%j<16;%j++)
            {
               %var[%j] = "";
            }
         }
      }
   }
   else
   {
      MessageBoxOK("Ooops","You have not made any changes to save.");
      return;
   }
}

//- RTBSC_Pane3::resetOptions (Resets all options back to defaults)
function RTBSC_Pane3::resetOptions(%this,%confirm)
{
	if(!%confirm)
	{
		MessageBoxYesNo("Really?","Are you sure you want to reset ALL the server preferences to their default values?","RTBSC_Pane3::resetOptions("@%this@",1);","");
		return;
	}
	commandtoserver('RTB_defaultServerPrefs');
}

//- RTBSC_PF_ValidateInt (Checks whether a value is a valid integer)
function RTBSC_PF_ValidateInt(%ctrl)
{
   if(%ctrl.getValue() $= "")
      return;
   if(%ctrl.getValue() $= "-")
      return;      
      
   %value = mFloatLength(%ctrl.getValue(),0);
   
   if(%value > %ctrl.fieldMax)
      %value = %ctrl.fieldMax;
      
   if(%value < %ctrl.fieldMin)
      %value = %ctrl.fieldMin;
      
   %ctrl.setValue(%value);
}

//*********************************************************
//* Ban List Clearing
//*********************************************************
//- unBanGui::clickClear (clears all the bans on the banlist)
function unBanGui::clickClear(%this,%confirm)
{
	if(!%confirm)
	{
		MessageBoxYesNo("Really?","Are you sure you want to clear ALL the bans on the server ban list?","unBanGui::clickClear("@%this@",1);","");
		return;
	}
	unBan_list.clear();
	commandtoserver('RTB_clearBans');
}

//*********************************************************
//* Version Establishment
//*********************************************************
//- clientCmdSendRTBVersion (Receives the server's RTB version and whether it has RTB)
function clientCmdSendRTBVersion(%version)
{
   $RTB::CServerControl::Cache::ServerHasRTB = 1;
   $RTB::CServerControl::Cache::ServerRTBVersion = %version;
   
   RTBSC_Pane1.render();
   RTBSC_Pane3.render();
   
   $RTB::CServerControl::Cache::SP::Cats = 0;
   $RTB::CServerControl::Cache::SP::Prefs = 0;
}