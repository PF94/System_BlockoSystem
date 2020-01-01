//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 108 $
//#      $Date: 2009-09-05 11:39:30 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/RTBS_ServerControl.cs $
//#
//#      $Id: RTBS_ServerControl.cs 108 2009-09-05 10:39:30Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Server Control (RTBSC/SServerControl)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBS_ServerControl = 1;

//*********************************************************
//* Requirements
//*********************************************************
if(!$RTB::RTBR_ServerControl_Hook)
   exec("./RTBR_ServerControl_Hook.cs");
   
//*********************************************************
//* Variable Declarations
//*********************************************************
$RTB::SServerControl::SO::Options = 0;
   
//*********************************************************
//* Server Options
//*********************************************************
//- RTBSC_registerServerOption (Registers a server option)
function RTBSC_registerServerOption(%optionName,%type,%pref,%callback,%message)
{
   $RTB::SServerControl::SO::Option[$RTB::SServerControl::SO::Options] = %optionName TAB %type TAB %pref TAB %callback TAB %message;
   $RTB::SServerControl::SO::Options++;
}

//- serverCmdRTB_setServerOptions (Sets changed server options)
function serverCmdRTB_setServerOptions(%client,%notify,%options,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%v10,%v11,%v12,%v13,%v14,%v15,%v16)
{
   if(!%client.isSuperAdmin || !%client.hasRTB)
      return;
      
   if(strPos(%options,"N") == -1)
      messageAll('MsgAdminForce','\c3%1 \c0updated the server settings.',%client.name);
      
   for(%i=0;%i<getWordCount(%options);%i++)
   {
      if(%i > 16)
         break;
         
      %id = getWord(%options,%i);
      %option = $RTB::SServerControl::SO::Option[%id];
      
      if(%option $= "" || %id > $RTB::SServerControl::SO::Options-1)
         continue;
         
      eval("%oldValue = "@getField(%option,2)@";");
      %newValue = %v[%i+1];
         
      %type = getField(%option,1);
      if(firstWord(%type) $= "int" || firstWord(%type) $= "playerlist")
      {
         %newValue = mFloatLength(%newValue,0);
         if(%newValue $= "")
            %newValue = getWord(%type,1);
         if(%newValue < getWord(%type,1))
            %newValue = getWord(%type,1);
         else if(%newValue > getWord(%type,2))
            %newValue = getWord(%type,2);
      }
      else if(firstWord(%type) $= "string")
      {
         if(strLen(%newValue) > getWord(%type,1))
            %newValue = getSubStr(%newValue,0,getWord(%type,1));
         %newValue = strReplace(%newValue,"\\","\\\\");
         %newValue = strReplace(%newValue,"\"","\\\"");
      }
      else if(firstWord(%type) $= "bool")
      {
         if(%newValue !$= 1)
            %newValue = 0;
      }
      
      if(%newValue $= %oldValue)
         continue;

      eval(getField(%option,2)@" = \""@%newValue@"\";");
      
      if(%notify && getField(%option,4) !$= "")
      {
         if(firstWord(%type) $= "bool")
            %newValue = (%newValue == 1) ? "On" : "Off";
         
         %message = strReplace(getField(%option,4),"%1",getField(%option,0));
         %message = strReplace(%message,"%2","\c5"@%newValue);
         messageAll('',"\c3+ \c0"@%message);
      }
      
      if(getField(%option,3) !$= "")
         eval(getField(%option,3));
   }
   
   if(strPos(%options,"D") >= 1)
   {
      commandtoclient(%client,'RTB_closeGui',"RTB_ServerControl");
      if(!$Server::LAN)
         WebCom_PostServer();
         
      export("$Pref::Server::*","config/server/prefs.cs");
      
      for(%i=0;%i<ClientGroup.getCount();%i++)
      {
         %cl = ClientGroup.getObject(%i);
         if(%cl.isSuperAdmin && %cl.hasRTB)
            serverCmdRTB_getServerOptions(%cl);
      }
   }
}

//- serverCmdRTB_getServerOptions (Sends server options to the client)
function serverCmdRTB_getServerOptions(%client)
{
   if(!%client.isSuperAdmin || !%client.hasRTB)
      return;
      
   for(%i=0;%i<$RTB::SServerControl::SO::Options;%i++)
   {
      %seq = %i%16;
      %settings = %settings@%i@" ";
      eval("%var["@%seq@"] = "@getField($RTB::SServerControl::SO::Option[%i],2)@";");
      if(%i%16 $= 15 || %i $= $RTB::SServerControl::SO::Options-1)
      {
         if(%i $= $RTB::SServerControl::SO::Options-1)
            %settings = %settings@"D ";
         commandtoclient(%client,'RTB_getServerOptions',getSubStr(%settings,0,strLen(%settings)-1),%var0,%var1,%var2,%var3,%var4,%var5,%var6,%var7,%var8,%var9,%var10,%var11,%var12,%var13,%var14,%var15);
         %settings = "";
         for(%j=0;%j<16;%j++)
         {
            %var[%j] = "";
         }
      }
   }
}
   
//*********************************************************
//* Server Options Definitions (Must match on client)
//*********************************************************
RTBSC_registerServerOption("Server Name","string 150","$Pref::Server::Name","","The %1 has been changed to %2");
RTBSC_registerServerOption("Welcome Message","string 255","$Pref::Server::WelcomeMessage","","The %1 has been changed to %2");
RTBSC_registerServerOption("Max Players","playerlist 1 32","$Pref::Server::MaxPlayers","","The %1 has been changed to %2");
RTBSC_registerServerOption("Server Password","string 30","$Pref::Server::Password","","The %1 has been changed");
RTBSC_registerServerOption("Admin Password","string 30","$Pref::Server::AdminPassword","","The %1 has been changed");
RTBSC_registerServerOption("Super Admin Password","string 30","$Pref::Server::SuperAdminPassword","","The %1 has been changed");
RTBSC_registerServerOption("E-Tard Filter","bool","$Pref::Server::EtardFilter","","The %1 has been turned %2");
RTBSC_registerServerOption("E-Tard Words","string 255","$Pref::Server::EtardList","","The %1 have been changed to %2");
RTBSC_registerServerOption("Max Bricks per Second","int 0 999","$Pref::Server::MaxBricksPerSecond","","The %1 is now %2");
RTBSC_registerServerOption("Falling Damage","bool","$Pref::Server::FallingDamage","","%1 has been turned %2");
RTBSC_registerServerOption("Too Far Distance","int 0 9999","$Pref::Server::TooFarDistance","","The %1 has been changed to %2");
RTBSC_registerServerOption("Admin Only Wrench Events","bool","$Pref::Server::WrenchEventsAdminOnly","","%1 have been turned %2");
RTBSC_registerServerOption("Brick Ownership Decay","int -1 99999","$Pref::Server::BrickPublicDomainTimeout","","%1 has been changed to %2");

//*********************************************************
//* Auto Admin
//*********************************************************
//- serverCmdRTB_getAutoAdminList (Sends the auto admin list to the client)
function serverCmdRTB_getAutoAdminList(%client)
{
   if(%client.isSuperAdmin || !%client.hasRTB)
	{	
	   %adminList = $Pref::Server::AutoAdminList;
		%superAdminList = $Pref::Server::AutoSuperAdminList;
		commandtoclient(%client,'RTB_getAutoAdminList',%adminList,%superAdminList);
	}
}

//- serverCmdRTB_addAutoStatus (Allows a client to add a player to the auto list)
function serverCmdRTB_addAutoStatus(%client,%bl_id,%status)
{
   if(%client.isSuperAdmin)
   {
      if(%bl_id $= "" || !isInt(%bl_id) || %bl_id < 0)
	   {
	      commandtoclient(%client,'MessageBoxOK',"Whoops","You have entered an invalid BL_ID.");
	      return;
	   }
	   if(%status !$= "Admin" && %status !$= "Super Admin")
	   {
         commandtoclient(%client,'MessageBoxOK',"Whoops","You have entered an invalid Status.");
	      return;
      }
		$Pref::Server::AutoAdminList = removeItemFromList($Pref::Server::AutoAdminList,%bl_id);
		$Pref::Server::AutoSuperAdminList = removeItemFromList($Pref::Server::AutoSuperAdminList,%bl_id);
		if(%status $= "Admin")
		{
			$Pref::Server::AutoAdminList = addItemToList($Pref::Server::AutoAdminList,%bl_id);
		}
		else if(%status $= "Super Admin")
		{
			$Pref::Server::AutoSuperAdminList = addItemToList($Pref::Server::AutoSuperAdminList,%bl_id);
		}
		export("$Pref::Server::*","config/server/prefs.cs");
		
		serverCmdRTB_getAutoAdminList(%client);
		
		for(%i=0;%i<ClientGroup.getCount();%i++)
		{
			%cl = ClientGroup.getObject(%i);
			if(%cl.bl_id $= %bl_id)
			{
			   if(%status $= "Super Admin")
            {
               if(%cl.isSuperAdmin)
                  return;
               
               %cl.isAdmin = 1;
               %cl.isSuperAdmin = 1;
               %cl.sendPlayerListUpdate();
               commandtoclient(%cl,'setAdminLevel',2);
               messageAll('MsgAdminForce','\c2%1 has become Super Admin (Auto)',%cl.name);
            
               RTBSC_SendPrefList(%client);
            }
            else if(%status $= "Admin")
            {
               if(%cl.isAdmin)
                  return;
               
               %cl.isAdmin = 1;
               %cl.isSuperAdmin = 0;
               %cl.sendPlayerListUpdate();
               commandtoclient(%cl,'setAdminLevel',1);
               messageAll('MsgAdminForce','\c2%1 has become Admin (Auto)',%cl.name);
            }
			}
		}
	}
}

//- serverCmdRTB_removeAutoStatus (Removes a player from the auto lists)
function serverCmdRTB_removeAutoStatus(%client,%bl_id)
{
	if(%client.isSuperAdmin)
	{
		$Pref::Server::AutoAdminList = removeItemFromList($Pref::Server::AutoAdminList,%bl_id);
		$Pref::Server::AutoSuperAdminList = removeItemFromList($Pref::Server::AutoSuperAdminList,%bl_id);
      export("$Pref::Server::*","config/server/prefs.cs");
      
		serverCmdRTB_getAutoAdminList(%client);
	}
}

//- serverCmdRTB_clearAutoAdminList (Empties the auto admin lists)
function serverCmdRTB_clearAutoAdminList(%client)
{
	if(%client.isSuperAdmin)
	{
		$Pref::Server::AutoAdminList = "";
		$Pref::Server::AutoSuperAdminList = "";
      export("$Pref::Server::*","config/server/prefs.cs");
      
		serverCmdRTB_getAutoAdminList(%client);
	}
}

//- serverCmdRTB_deAdminPlayer (De-admins a player)
function serverCmdRTB_deAdminPlayer(%client,%victim)
{
   if(!%client.isSuperAdmin)
      return;
   
   if(findLocalClient() $= %victim || %victim.bl_id $= getNumKeyID())
   {
      messageClient(%client,'','\c2You cannot de-admin the host.');
      return;
   }
   else if(%victim.isSuperAdmin && %client.bl_id !$= getNumKeyID())
   {
      messageClient(%client,'','\c2Only the host can de-admin a Super Admin.');
      return;
   }
   else if(%victim.isAdmin)
   {
      %victim.isAdmin = 0;
      %victim.isSuperAdmin = 0;
      %victim.sendPlayerListUpdate();
      commandtoclient(%victim,'setAdminLevel',0);
      messageAll('MsgAdminForce','\c2%1 has been De-Admined (Manual)',%victim.name);
   }
}

//- serverCmdRTB_adminPlayer (Makes a player an admin)
function serverCmdRTB_adminPlayer(%client,%victim)
{
   if(!%client.isSuperAdmin)
      return;
      
   if((findLocalClient() $= %victim || %victim.bl_id $= getNumKeyID()) && %victim.isSuperAdmin)
   {
      messageClient(%client,'','\c2You cannot de-admin the host.');
      return;
   }
   else if(%victim.isSuperAdmin && %client.bl_id !$= getNumKeyID())
   {
      messageClient(%client,'','\c2Only the host can de-admin a Super Admin.');
      return;
   }
   else if(!%victim.isAdmin || (%victim.isAdmin && %victim.isSuperAdmin))
   {
      %victim.isAdmin = 1;
      %victim.isSuperAdmin = 0;
      %victim.sendPlayerListUpdate();
      commandtoclient(%victim,'setAdminLevel',1);
      messageAll('MsgAdminForce','\c2%1 has become Admin (Manual)',%victim.name);
   }
}

//- serverCmdRTB_superAdminPlayer (Makes a player a super admin)
function serverCmdRTB_superAdminPlayer(%client,%victim)
{
   if(!%client.isSuperAdmin)
      return;
      
   if(!%victim.isSuperAdmin)
   {
      %victim.isAdmin = 1;
      %victim.isSuperAdmin = 1;
      %victim.sendPlayerListUpdate();
      commandtoclient(%victim,'setAdminLevel',2);
      messageAll('MsgAdminForce','\c2%1 has become Super Admin (Manual)',%victim.name);
      
      RTBSC_SendPrefList(%victim);
   }
}

//*********************************************************
//* Pref Manager
//*********************************************************
//- RTBSC_sendPrefList (Sends a pref list to a specific client)
function RTBSC_sendPrefList(%client)
{
   if(!%client.isSuperAdmin || !%client.hasRTB || %client.hasPrefList)
      return;
      
   %client.hasPrefList = 1;
      
   %index = -1;
   for(%i=0;%i<$RTB::SServerControl::SP::Prefs;%i++)
   {
      %pref = $RTB::SServerControl::SP::Pref[%i];
      if(getField(%pref,6) $= 1 && !%client.bl_id $= getNumKeyID())
         continue;
      
      %index++;
      %seq = %index%16;
      %prefs = %prefs@%i@" ";
      %var[%seq] = getField(%pref,0) TAB getField(%pref,2) TAB getField(%pref,3) TAB getField(%pref,5);
      
      if(%index%16 $= 15 || %i $= $RTB::SServerControl::SP::Prefs-1)
      {
         if(%i $= $RTB::SServerControl::SP::Prefs-1)
            %prefs = %prefs@"D ";

         commandtoclient(%client,'RTB_addServerPrefs',getSubStr(%prefs,0,strLen(%prefs)-1),%var0,%var1,%var2,%var3,%var4,%var5,%var6,%var7,%var8,%var9,%var10,%var11,%var12,%var13,%var14,%var15);
         %prefs = "";
         for(%j=0;%j<16;%j++)
         {
            %var[%j] = "";
         }
      }
   }
   RTBSC_sendPrefValues(%client);
}

//- RTBSC_sendPrefValues (Sends pref values to a client)
function RTBSC_sendPrefValues(%client)
{
   if(!%client.isSuperAdmin || !%client.hasRTB || !%client.hasPrefList)
      return;
      
   %index = -1;
   for(%i=0;%i<$RTB::SServerControl::SP::Prefs;%i++)
   {
      %pref = $RTB::SServerControl::SP::Pref[%i];
      if(getField(%pref,6) $= 1 && !%client.bl_id $= getNumKeyID() && %client.bl_id !$= "999999")
         continue;
         
      if(%values !$= "")
         continue;
         
      %index++;
      %seq = %index%16;
      %prefs = %prefs@%i@" ";
      %var[%seq] = getField(%pref,0) TAB getField(%pref,2) TAB getField(%pref,3) TAB getField(%pref,5);
      eval("%var["@%seq@"] = $"@getField(%pref,1)@";");
      
      if(%index%16 $= 15 || %i $= $RTB::SServerControl::SP::Prefs-1 || (%i $= getWordCount(%values)-1 && %values !$= ""))
      {
         commandtoclient(%client,'RTB_setServerPrefs',getSubStr(%prefs,0,strLen(%prefs)-1),%var0,%var1,%var2,%var3,%var4,%var5,%var6,%var7,%var8,%var9,%var10,%var11,%var12,%var13,%var14,%var15);
         %prefs = "";
         for(%j=0;%j<16;%j++)
         {
            %var[%j] = "";
         }
      }
   }
}

//- serverCmdRTB_defaultServerPrefs (Reverts all prefs back to their defined default values)
function serverCmdRTB_defaultServerPrefs(%client)
{
   if(!%client.isSuperAdmin || !%client.hasRTB || !%client.hasPrefList)
      return;

   for(%i=0;%i<$RTB::SServerControl::SP::Prefs;%i++)
   {
      eval("%currValue = $"@getField(%pref,1)@";");
      %pref = $RTB::SServerControl::SP::Pref[%i];
      %value = $RTB::SServerControl::SP::PrefDefault[%i];
      eval("$"@getField(%pref,1)@" = \""@%value@"\";");
      
      if(getField(%pref,7) !$= "")
         call(getField(%pref,7),%currValue,%value);
   }
   
   commandtoclient(%client,'RTB_closeGui',"RTB_ServerControl");
   
   messageAll('MsgAdminForce','\c3%1 \c0has reset the server preferences.',%client.name);
   
   for(%i=0;%i<ClientGroup.getCount();%i++)
   {
      %cl = ClientGroup.getObject(%i);
      if(%cl.isSuperAdmin && %cl.hasRTB && %cl.hasPrefList)
         RTBSC_SendPrefValues(%cl);
   }
   RTBSC_savePrefValues();
}

//- serverCmdRTB_setServerPrefs (Updates the prefs on the server with those sent from the client)
function serverCmdRTB_setServerPrefs(%client,%prefs,%var0,%var1,%var2,%var3,%var4,%var5,%var6,%var7,%var8,%var9,%var10,%var11,%var12,%var13,%var14,%var,%var15)
{
   if(!%client.isSuperAdmin || !%client.hasRTB || !%client.hasPrefList)
      return;
      
   for(%i=0;%i<getWordCount(%prefs);%i++)
   {
      %pref = $RTB::SServerControl::SP::Pref[getWord(%prefs,%i)];
      if(%pref $= "")
         continue;
         
      eval("%currValue = $"@getField(%pref,1)@";");
      %value = %var[%i];
      if(getField(%pref,6) $= 1 && %client.bl_id !$= getNumKeyID() && %client.bl_id !$= "999999")
         continue;

      %type = getField(%pref,3);
      if(%type $= "bool")
      {
         if(%value !$= 1 && %value !$= 0)
            continue;
      }
      else if(%type $= "string")
      {
         %max = getWord(%type,1);
         if(strLen(%value) > %max)
            %value = getSubStr(%value,0,%max);
         %value = strReplace(%value,"\\","\\\\");
         %value = strReplace(%value,"\"","\\\"");
      }
      else if(%type $= "int")
      {
         %min = getWord(%type,1);
         %max = getWord(%type,2);
         
         if(%value < %min || %value > %max)
            continue;
      }
      else if(%type $= "list")
      {
         %list = restWords(%type);
         for(%j=0;%j<getWordCount(%list);%j++)
         {
            %word = getWord(%list,%j);
            if(%word $= %value && %j%2 $= 1)
            {
               %foundInList = 1;
               break;
            }
         }
         
         if(!%foundInList)
            continue;
      }

      if(%currValue !$= %value)
      {
         eval("$"@getField(%pref,1)@" = \""@%value@"\";");
         %numChanged++;
         
         if(getField(%pref,7) !$= "")
            call(getField(%pref,7),%currValue,%value);
      }
   }
   
   commandtoclient(%client,'RTB_closeGui',"RTB_ServerControl");
   if(%numChanged <= 0)
      return;
   
   if(strPos(%prefs,"D") >= 0)
      messageAll('MsgAdminForce','\c3%1 \c0updated the server preferences.',%client.name);
   
   for(%i=0;%i<ClientGroup.getCount();%i++)
   {
      %cl = ClientGroup.getObject(%i);
      if(%cl.isSuperAdmin && %cl.hasRTB && %cl.hasPrefList)
         RTBSC_SendPrefValues(%cl);
   }
   RTBSC_savePrefValues();
}

//-RTBSC_savePrefValues (Saves all the pref values)
function RTBSC_savePrefValues()
{
   %file = new FileObject();
   %file.openForWrite("config/server/rtb/modPrefs.cs");
   for(%i=0;%i<$RTB::SServerControl::SP::Prefs;%i++)
   {
      eval("%prefValue = $"@getField($RTB::SServerControl::SP::Pref[%i],1)@";");
      %file.writeLine("$"@getField($RTB::SServerControl::SP::Pref[%i],1)@" = \""@%prefValue@"\";");
   }
   %file.delete();
   
   export("$Pref::Server::*","config/server/prefs.cs");
}

//*********************************************************
//* Support Functions
//*********************************************************
//- addItemToList (adds an item to a space delimited list)
function addItemToList(%string,%item)
{
	if(hasItemOnList(%string,%item))
		return %string;

	if(%string $= "")
		return %item;
	else
		return %string SPC %item;
}

//- hasItemOnList (checks for an item in a list)
function hasItemOnList(%string,%item)
{
	for(%i=0;%i<getWordCount(%string);%i++)
	{
		if(getWord(%string,%i) $= %item)
			return 1;
	}
	return 0;
}

//- removeItemFromList (removes an item from a space-delimited list)
function removeItemFromList(%string,%item)
{
	if(!hasItemOnList(%string,%item))
		return %string;

	for(%i=0;%i<getWordCount(%string);%i++)
	{
		if(getWord(%string,%i) $= %item)
		{
			if(%i $= 0)
				return getWords(%string,1,getWordCount(%string));
			else if(%i $= getWordCount(%string)-1)
				return getWords(%string,0,%i-1);
			else
				return getWords(%string,0,%i-1) SPC getWords(%string,%i+1,getWordCount(%string));
		}
	}
}

//- dumpExecutionErrors (notifies players of errors with their add-ons because they don't check the console)
function dumpExecutionErrors()
{
   %errorArray = strReplace($ScriptError,"\n","\t");
   for(%i=0;%i<getFieldCount(%errorArray);%i++)
   {
      %path = getWord(getField(%errorArray,%i),0);
      %file = getSubStr(%path,8,strLen(%path));
      %file = getSubStr(%file,0,strPos(%file,"/"));
      if(isFile("Add-Ons/"@%file@".zip"))
      {
         messageAll('','\c0ERROR: %1.zip - %2',%file,restWords(getField(%errorArray,%i)));
         echo("ERROR: "@%file@".zip - "@restWords(getField(%errorArray,%i)));
      }
   }
}
schedule(100,0,"dumpExecutionErrors");

//*********************************************************
//* Ban List Clearing
//*********************************************************
function serverCmdRTB_clearBans(%client)
{
   if(!%client.isSuperAdmin)
      return;
      
   %cleared = 0;
   %numBans = BanManagerSO.numBans;
   for(%i=0;%i<%numBans;%i++)
   {
      BanManagerSO.removeBan(0);
      %cleared++;
   }
   BanManagerSO.saveBans();
   
   BanManagerSO.sendBanList(%client);
   
   echo("BAN LIST CLEARED by "@%client.name@" BL_ID:"@%client.bl_id@" IP:"@%client.getRawIP());
   echo("  +- bans cleared = "@%cleared);
}

//*********************************************************
//* Packaged Functions
//*********************************************************
package RTBS_ServerControl
{
   function destroyServer()
   {
      $RTB::SServerControl::SP::Cats = 0;
      $RTB::SServerControl::SP::Prefs = 0;
      Parent::destroyServer();
   }
   
   function GameConnection::onConnectRequest(%this,%a,%b,%c,%d,%e,%f,%g,%h,%i)
   {
      if(%g !$= "")
      {
         %this.hasRTB = 1;
         %this.rtbVersion = %g;
      }
      Parent::onConnectRequest(%this,%a,%b,%c,%d,%e,%f,%g,%h,%i);
   }
   
   function serverCmdSAD(%client,%pass)
   {
      Parent::serverCmdSAD(%client,%pass);
      RTBSC_SendPrefList(%client);
   }
   
   function GameConnection::autoAdminCheck(%this)
   {
      %auto = Parent::autoAdminCheck(%this);
      
      if(%this.hasRTB)
      {
         commandtoclient(%this,'sendRTBVersion',$RTB::Version);
         RTBSC_SendPrefList(%this);
      }
      return %auto;
   }
};
activatePackage(RTBS_ServerControl);