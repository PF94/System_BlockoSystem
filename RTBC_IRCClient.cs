//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 112 $
//#      $Date: 2009-09-05 18:17:49 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/RTBC_IRCClient.cs $
//#
//#      $Id: RTBC_IRCClient.cs 112 2009-09-05 17:17:49Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   IRC Client (RTBIC/CIRCClient)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBC_IRCClient = 1;

//*********************************************************
//* GUI Modification
//*********************************************************
if(!isObject(MM_RTBIRCClientButton))
{
   %btn = new GuiBitmapButtonCtrl(MM_RTBIRCClientButton)
   {
      profile = "GuiDefaultProfile";
      horizSizing = "relative";
      vertSizing = "relative";
      position = "416 160";
      extent = "224 40";
      minExtent = "8 2";
      visible = "1";
      text = " ";
      groupNum = "-1";
      buttonType = "PushButton";
      bitmap = $RTB::Path@"images/buttons/menu/btnIRCClient";
      command = "canvas.pushdialog(RTB_IRCClient);";
      lockAspectRatio = "1";
      alignLeft = "1";
      overflowImage = "0";
      mKeepCached = "1";
      mColor = "255 255 255 255";
   };
   MainMenuGui.add(%btn);
}
function MM_RTBIRCClientButton::onMouseEnter(%this)
{
   if($Pref::Audio::MenuSounds)
	   alxPlay(Note9Sound);
}

//*********************************************************
//* Variable Declarations
//*********************************************************
$RTB::CIRCClient::Server = "irc.centralchat.net";
$RTB::CIRCClient::Port = "6667";
$RTB::CIRCClient::RetryCount = "5";

//*********************************************************
//* Initialisation of required objects
//*********************************************************
if(!isObject(RTB_IRCClient))
	exec("./RTB_IRCClient.gui");
	
if(!isObject(RTBIC_MessageVector))
	new MessageVector(RTBIC_MessageVector);
	
if(!isObject(RTBIC_UserManifest))
	new SimGroup(RTBIC_UserManifest);
	
if(!isObject(RTBIC_SessionManifest))
   new SimGroup(RTBIC_SessionManifest);
   
if(!isObject(RTBIC_NotificationManifest))
   new SimGroup(RTBIC_NotificationManifest);
   
if(!$RTB::CIRCClient::AppliedMaps)
{
   RTB_addControlMap("keyboard","i","Toggle IRC","RTBIC_toggleIRC");
   $RTB::CIRCClient::AppliedMaps = 1;
}
   
if(!isObject(RTBIC_PMSound))
	new AudioProfile(RTBIC_PMSound)
	{
		fileName = "./sounds/RTB_ircPM.wav";
		description = "AudioGui";
		preload = "1";
	};

//*********************************************************
//* GUI Callbacks
//*********************************************************
function RTB_IRCClient::onWake(%this)
{
   RTBIC_MessageVectorCtrl.attach(RTBIC_MessageVector);
   
   for(%i=0;%i<RTBIC_SessionManifest.getCount();%i++)
   {
      %sess = RTBIC_SessionManifest.getObject(%i);
      %sess.window.messageVectorCtrl.attach(%sess.window.messageVector);
   }
   
   RTBIC_refreshScroll();
   
   if($RTB::Options::IC::CustomUsername)
      RTBIC_CustomUserBlock.setVisible(0);
      
   schedule(10,0,"RTBIC_makeFirstResponder");
}

function RTBIC_makeFirstResponder()
{
      if(isObject($RTB::CIRCClient::CurrPane.window.messageBox))
         $RTB::CIRCClient::CurrPane.window.messageBox.makeFirstResponder(1);
      else
         RTBIC_MessageBox.makeFirstResponder(1);
}

function RTBIC_toggleIRC(%val)
{
   if(!%val)
      return;
      
   if(RTB_IRCClient.isAwake())
      canvas.popDialog(RTB_IRCClient);
   else
      canvas.pushDialog(RTB_IRCClient);
      
   schedule(10,0,"RTBIC_makeFirstResponder");
}

function RTBIC_SendMessage()
{
   %message = RTBIC_MessageBox.getValue();
   if(%message $= "")
      return;
      
   if(filterKey(%message))
   {
      RTBIC_MessageBox.setValue("");
      RTBIC_addLine("\c2** DO NOT SAY YOUR BLOCKLAND KEY INTO THE CHAT! **");
      return;
   }
      
   if(!RTBIC_SC.connected)
   {
      RTBIC_addLine("\c2ERROR: You are not connected.");
      RTBIC_MessageBox.setValue("");
      return;
   }
      
   if(getSubStr(%message,0,1) $= "/")
   {
      if(getWord(%message, 0) $= "/me" || getWord(%message, 0) $= "/action")
		{
			%vars = getWords(%message,1,getWordCount(%message)-1);
			%message = "PRIVMSG "@RTBIC_SC.currentchannel@" :\c0ACTION "@%vars@"\c0";
			RTBIC_SC.sendLine(%message);
			RTBIC_addLine("\c4* "@$RTB::CIRCClient::Cache::NickName SPC %vars);
		}
      else if(getWord(%message,0) $= "/ignore")
      {
         %user = getWord(%message,1);
         if($Ignore::User[%user])
         {
            $Ignore::User[%user] = "";
            RTBIC_addLine("\c6* You are no longer ignoring "@%user@".");
         }
         else
         {
            $Ignore::User[%user] = 1;
            RTBIC_addLine("\c6* You are now ignoring "@%user@".");
         }
      }
      else if(getWord(%message,0) $= "/unignore")
      {
         %user = getWord(%message,1);
         if($Ignore::User[%user])
         {
            $Ignore::User[%user] = "";
            RTBIC_addLine("\c6* You are no longer ignoring "@%user@".");
         }
      }
		else if(getWord(%message,0) $= "/msg")
		{
         %vars = getWords(%message,1,getWordCount(%message)-1);
			RTBIC_SC.sendLine("PRIVMSG "@%vars);

         %username = getWord(%message,1);
         if(RTBIC_hasSession(%username))
         {
            %sess = RTBIC_getSession(%username);
            RTBIC_SetPane(%sess);
            RTBIC_ScrollToTab(%username);
         }
         else
         {
            %sess = RTBIC_createSession(%username);
            RTBIC_SetPane(%sess);
            %sess.window.messageVector.pushBackLine("\c6* Now talking to "@%username@"...",0);
            RTBIC_ScrollToTab(%username);
         }
         %sess.window.messageVector.pushBackLine("\c0<"@$RTB::CIRCClient::Cache::NickName@"> "@getWords(%message,2,getWordCount(%message)-1),0);
		}
		else if(getWord(%message,0) $= "/slap")
		{
         %name = getField(RTBIC_UserList.getValue(),1);
         %num = getRandom(1,2);
         if(%num $= 1)
         {
            if(%name !$= "")
               %vars = "slaps "@%name@" around with a moose at large!";
            else
               %vars = "gets slapped around with a moose at large!";
         }
         else if(%num $= 2)
         {
            if(%name !$= "")
               %vars = "slaps "@%name@" around with a whale bone!";
            else
               %vars = "gets slapped around with a whale bone!";
         }
			%message = "PRIVMSG "@RTBIC_SC.currentchannel@" :\c0ACTION "@%vars@"\c0";
			
			RTBIC_SC.sendLine(%message);
			RTBIC_addLine("\c4* "@$RTB::CIRCClient::Cache::NickName SPC %vars);
		}
      else if(getWord(%message,0) $= "/kick")
      {
         RTBIC_SC.sendLine("KICK "@RTBIC_SC.currentchannel@" "@getWord(%message,1));
      }
      else if(getWord(%message,0) $= "/hop")
      {
         RTBIC_SC.sendLine("PART #rtb");
         RTBIC_SC.sendLine("JOIN #rtb");
      }
      else if(getWord(%message,0) $= "/join")
      {
         //nope.
         return;
      }
		else
         RTBIC_SC.sendLine(getSubStr(%message,1,strLen(%message)));
   }
   else
   {
      RTBIC_SC.sendLine("PRIVMSG "@RTBIC_SC.currentChannel SPC ":"@%message);
      RTBIC_addLine("\c5<"@$RTB::CIRCClient::Cache::NickName@">\c0" SPC %message);
   }
   RTBIC_MessageBox.setValue("");
}
function RTBIC_UserList::onSelect(%this, %id, %text)
{
   if($RTB::CIRCClient::Cache::WasSort)
   {
      $RTB::CIRCClient::Cache::WasSort = 0;
      return;
   }
      
	%user = getWord(%text,1);
	if(%user $= $RTB::CIRCClient::Cache::LastULClickP && %user !$= "" && vectorDist($Sim::Time,$RTB::CIRCClient::Cache::LastULClick) < 2 && $RTB::Options::IC::AllowPM)
	{
		if(%user $= $RTB::CIRCClient::Cache::NickName)
		{
		}
		else if(RTBIC_hasSession(%user))
		{
			%sess = RTBIC_getSession(%user);
			RTBIC_SetPane(%sess);
			RTBIC_ScrollToTab(%user);
		}
		else
		{
			%sess = RTBIC_createSession(%user);
			RTBIC_SetPane(%sess);
         %sess.window.messageVector.pushBackLine("\c6* Now talking to "@%user@"...",0);
         RTBIC_ScrollToTab(%user);
		}
	}
	$RTB::CIRCClient::Cache::LastULClickP = %user;
	$RTB::CIRCClient::Cache::LastULClick = $Sim::Time;
}

//*********************************************************
//* Connection Management
//*********************************************************
function RTBIC_InitiateConnection()
{
   if(isObject(RTBIC_SC))
      return;
   
   new TCPObject(RTBIC_SC)
   {
      site = $RTB::CIRCClient::Server;
      port = $RTB::CIRCClient::Port;
      
      connected = 0;
   };
}

function RTBIC_SC::sendLine(%this,%line)
{
   %this.send(%line@"\r\n");
}

function RTBIC_SC::addHandle(%this,%handle,%routine)
{
   %this.dispatch[%handle] = %routine;
   return 1;
}

function RTBIC_Connect()
{
   RTBIC_SC.connect(RTBIC_SC.site@":"@RTBIC_SC.port);
   
   RTBIC_addLine("\c6Connecting...");
   
   RTBIC_ConnectBtn.setColor("255 255 50 255");
   RTBIC_ConnectBtn.text = "Stop Connecting";
   RTBIC_ConnectBtn.command = "RTBIC_Disconnect();";
   
   %this.retryCount = 0;
}

function RTBIC_Disconnect(%cleanup)
{
   if(!%cleanup)
      RTBIC_SC.disconnect();
      
   RTBIC_SC.connected = 0;
   RTBIC_SC.currentChannel = "";
      
   cancel(RTBIC_SC.retrySchedule);
   
   RTBIC_addLine("\c6Disconnected.");
   
   RTBIC_ConnectBtn.setColor("50 255 50 255");
   RTBIC_ConnectBtn.text = "Connect";
   RTBIC_ConnectBtn.command = "RTBIC_Connect();";
   
   for(%i=0;%i<RTB_IRCClient.getObject(0).getCount();%i++)
   {
      %ctrl = RTB_IRCClient.getObject(0).getObject(%i);
      if(%ctrl.isPane && %ctrl.getName() $= "")
         %ctrl.delete();
   }
   RTBIC_SetPane("");
   
   RTBIC_SessionManifest.clear();
   RTBIC_UserManifest.clear();
   RTBIC_drawUserList();
   RTBIC_TabSwatch.clear();
   
   RTBIC_refreshScroll();
}

//*********************************************************
//* Execute-Time Instructions
//*********************************************************
RTBIC_InitiateConnection();

//*********************************************************
//* Connection Callbacks
//*********************************************************
function RTBIC_SC::onConnected(%this)
{
   RTBIC_ConnectBtn.setColor("255 50 50 255");
   RTBIC_ConnectBtn.text = "Disconnect";
   RTBIC_ConnectBtn.command = "RTBIC_Disconnect();";
   
   RTBIC_addLine("\c0Connected.");

   %this.connected = 1;
	%guestName = "Blockhead"@getNumKeyID();
	%this.sendLine("NICK "@%guestName);
	
	if($Pref::Player::NetName !$= "" && isUnlocked())
	   %userName = filterString(strReplace($Pref::Player::NetName," ","_"),"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789[]-");
   else if($Pref::Player::LANName !$= "")
      %userName = filterString(strReplace($Pref::Player::LANName," ","_"),"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789[]-");
   
	%this.sendLine("USER "@%guestName@" 0 * :"@%userName@"-"@getNumKeyID());
	
	$RTB::CIRCClient::Cache::NickName = %guestName;
	$RTB::CIRCClient::Cache::DesNickName = %userName;
	
	$RTB::Options::IC::CustomUser = filterString(strReplace($RTB::Options::IC::CustomUser," ","_"),"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
	RTBIC_CustomUser.setValue($RTB::Options::IC::CustomUser);
}

function RTBIC_SC::onLine(%this,%line)
{
   if($RTB::Debug)
      echo(%line);
      
	if(getSubStr(%line,0,1) $= ":")
		%line = nextToken(getSubStr(%line,1,strLen(%line)),prefix," ");
		
   %line = nextToken(%line,command," :");
   %line = nextToken(%line,params,"");
   
   %this.dispatchCommand(%command,%prefix,%params);
}

function RTBIC_SC::onConnectFailed(%this)
{
   %this.connected = 0;
   
   %this.retryCount++;
   if(%this.retryCount > $RTB::CIRCClient::RetryCount)
      RTBIC_addLine("\c2Could not connect.");
   else
   {
      RTBIC_addLine("\c6Connect Failed - Retrying ("@%this.retryCount@"/5)");
      %this.retrySchedule = %this.schedule(1000,"connect",%this.site@":"@%this.port);
   }
}

function RTBIC_SC::onDNSFailed(%this)
{
   %this.connected = 0;
   
   %this.retryCount++;
   if(%this.retryCount > $RTB::CIRCClient::RetryCount)
      RTBIC_addLine("\c2Could not connect.");
   else
   {
      RTBIC_addLine("\c6DNS Failed - Retrying ("@%this.retryCount@"/5)");
      %this.retrySchedule = %this.schedule(1000,"connect",%this.site@":"@%this.port);
   }
}

function RTBIC_SC::onDisconnect(%this)
{
   %this.connected = 0;
   
   RTBIC_Disconnect(1);
}

//*********************************************************
//* IRC Command Dispatcher
//*********************************************************
function RTBIC_SC::dispatchCommand(%this,%command,%prefix,%params)
{
   %prefix = strReplace(%prefix,"\"","\\\"");
   %params = strReplace(%params,"\"","\\\"");
   if(getsubstr(%params,strlen(%params)-1,1) $= "\\")
      %params = %params @ "\\";
      
   if(%this.dispatch[%command] !$= "")
      eval("RTBIC_SC::"@%this.dispatch[%command]@"("@%this@",\""@%prefix@"\",\""@%params@"\");");
   else
   { 
      if($RTB::Debug)
         error("ERROR: No dispatch route for command: "@%command);
   }
}

//*********************************************************
//* Dispatch Subroutines
//*********************************************************
RTBIC_SC.addHandle("NOTICE","onNotice");
function RTBIC_SC::onNotice(%this,%prefix,%params)
{
   if(strPos(%prefix,"Global") $= 0)
      return;
      
   nextToken(%prefix,sender,"!");
   %params = stripMLControlChars(nextToken(%params,params,":"));
   
   if(%sender $= "ChanServ" || %sender $= "NickServ")
   {
      %username = %sender;
      %message = %params;
      if(RTBIC_hasSession(%username))
         %sess = RTBIC_getSession(%username);
      else
         return;
         
      %sess.window.messageVector.pushBackLine("\c4"@%params,0);
      
      if(!%sess.window.visible)
         RTBIC_startFlashTab(%sess.tab);
         
      RTBIC_makeFirstResponder();
         
      if(!RTB_IRCClient.isAwake())
      {
         if($RTB::Options::IC::PMVisualNotify)
         {
            if(strLen(%message) > 40)
               %message = getSubStr(%message,0,40)@"...";
            RTBIC_pushNotification(%username,%message);
         }
         
         if($RTB::Options::IC::PMAudioNotify)
            alxPlay(RTBIC_PMSound);
      }
   }
   else
   {
      if(RTBIC_hasUser(%sender))
         RTBIC_addLine("\c2-"@%sender@"- "@%params);
      else
         RTBIC_addLine("\c4"@%params);
   }
}

RTBIC_SC.addHandle("JOIN","onJoin");
function RTBIC_SC::onJoin(%this,%prefix,%params)
{
	nextToken(%prefix,nick,"!");
   if(%nick $= $RTB::CIRCClient::Cache::NickName)
   {
      RTBIC_addLine("");
      RTBIC_addLine("\c1* You have joined the channel "@%params@".");
      %this.currentChannel = %params;
      %this.channelTab = RTBIC_pushTab(%params,"","RTBIC_forcePart();");
   }
   else
   {
      if($RTB::Options::IC::Filter::ShowConnects)
         RTBIC_addLine("\c1* "@%nick@" has joined "@%params@".");
      RTBIC_addUser(%nick);
   }
}

RTBIC_SC.addHandle("PART","onPart");
function RTBIC_SC::onPart(%this,%prefix,%params)
{
	nextToken(%prefix,nick,"!");
   if(%nick $= $RTB::CIRCClient::Cache::NickName)
   {
      RTBIC_addLine("\c1* You have left the channel "@%params@".");
      %this.currentChannel = "";
      RTBIC_popTab(%this.channelTab);
      RTBIC_UserManifest.clear();
      RTBIC_drawUserList();
   }
   else
   {
      if($RTB::Options::IC::Filter::ShowDisconnects)
         RTBIC_addLine("\c1* "@%nick@" has left "@%params@".");
      RTBIC_removeUser(%nick);
   }
}

RTBIC_SC.addHandle("QUIT","onQuit");
function RTBIC_SC::onQuit(%this,%prefix,%params)
{
	nextToken(%prefix,nick,"!");
	
   if($RTB::Options::IC::Filter::ShowDisconnects)
      RTBIC_addLine("\c1* "@%nick@" has quit.");
   RTBIC_removeUser(%nick);
}

RTBIC_SC.addHandle("332","onTopic");
function RTBIC_SC::onTopic(%this,%prefix,%params)
{
   %topic = nextToken(%params,prefix," :");
   RTBIC_addLine("\c3Topic is currently '"@%topic@"'");
}

RTBIC_SC.addHandle("TOPIC","onTopicChange");
function RTBIC_SC::onTopicChange(%this,%prefix,%params)
{
   %topic = nextToken(%params,prefix," :");
   RTBIC_addLine("\c3The Topic has been changed: '"@%topic@"'");
}

RTBIC_SC.addHandle("372","onMOTD");
function RTBIC_SC::onMOTD(%this,%prefix,%params)
{
   %msg = nextToken(%params,prefix,":");
   RTBIC_addLine("\c6"@%msg);
}

RTBIC_SC.addHandle("376","onMOTDEnd");
function RTBIC_SC::onMOTDEnd(%this,%prefix,%params)
{
   %this.sendLine("ISON "@$RTB::CIRCClient::Cache::NickName);
}

RTBIC_SC.addHandle("422","onMOTDMissing");
function RTBIC_SC::onMOTDMissing(%this,%prefix,%params)
{
   %this.sendLine("ISON "@$RTB::CIRCClient::Cache::NickName);
}

RTBIC_SC.addHandle("438","onError");
RTBIC_SC.addHandle("431","onError");
RTBIC_SC.addHandle("401","onError");
RTBIC_SC.addHandle("ERROR","onError");
function RTBIC_SC::onError(%this,%prefix,%params)
{
	%message = nextToken(%params,prefix,":");
	if(getWordcount(%prefix) > 1)
   {
      %username = getWord(%prefix,1);
      if(RTBIC_hasSession(%username))
         %sess = RTBIC_getSession(%username);
      else
         %sess = RTBIC_createSession(%username);
         
      %sess.window.messageVector.pushBackLine("\c2ERROR: "@%message,0);
      
      if(!%sess.window.visible)
         RTBIC_startFlashTab(%sess.tab);
         
      RTBIC_makeFirstResponder();
         
      if(!RTB_IRCClient.isAwake())
      {
         if($RTB::Options::IC::PMVisualNotify)
         {
            if(strLen(%message) > 40)
               %message = getSubStr(%message,0,40)@"...";
            RTBIC_pushNotification(%username,%message);
         }
         
         if($RTB::Options::IC::PMAudioNotify)
            alxPlay(RTBIC_PMSound);
      }
   }
   else
	   RTBIC_addLine("\c2ERROR: "@%message);
}

RTBIC_SC.addHandle("432","onErroneousNick");
function RTBIC_SC::onErroneousNick(%this,%prefix,%params)
{
	%msg = getWords(%params,4,getWordCount(%params));
	RTBIC_addLine("\c2* The Nickname "@getWord(%params,1)@" is not allowed. ("@%msg@")");
}

RTBIC_SC.addHandle("482","onNotOp");
function RTBIC_SC::onNotOp(%this,%prefix,%params)
{
   RTBIC_addLine("\c2* Oops you can't do that, you're not an Operator.");
}

RTBIC_SC.addHandle("461","onNotEnoughParams");
function RTBIC_SC::onNotEnoughParams(%this,%prefix,%params)
{
   RTBIC_addLine("\c2* You have not entered enough parameters for the command "@getWord(%params,1)@".");
}

RTBIC_SC.addHandle("442","onNotOnChannel");
function RTBIC_SC::onNotOnChannel(%this,%prefix,%params)
{
   RTBIC_addLine("\c2* You are not on the channel "@getWord(%params,1)@".");
}

RTBIC_SC.addHandle("474","onBanned");
function RTBIC_SC::onBanned(%this,%prefix,%params)
{
   RTBIC_addLine("\c2* You cannot join "@getWord(%params,1)@" because you are banned.");
}

RTBIC_SC.addHandle("NICK","onNick");
function RTBIC_SC::onNick(%this,%prefix,%params)
{
	%oldName = getSubStr(%prefix,0,strPos(%prefix,"!"));
	if(%oldName $= $RTB::CIRCClient::Cache::NickName)
	{
	   RTBIC_addLine("\c4* You are now known as "@%params@".");
	   $RTB::CIRCClient::Cache::NickName = %params;
	}
	else
	{
	   RTBIC_addLine("\c4"@%oldName@" is now known as "@%params@".");
	   if($Ignore::User[%oldName])
	   {
	      $Ignore::User[%params] = 1;
	      $Ignore::User[%oldName] = "";
	   }
	}
	   
   RTBIC_removeUser(%oldName);
   RTBIC_addUser(%params);
   
   if(RTBIC_hasSession(%oldName))
   {
      %session = RTBIC_getSession(%oldName);
      %session.username = %params;
      %session.tab.text = RTBIC_subTabName(%params);
      %session.tab.getObject(0).command = "RTBIC_endSession(\""@%params@"\");";
      %session.window.messageBox.altCommand = "RTBIC_sendSessionMessage(\""@%params@"\");";
      %session.window.getObject(3).command = "RTBIC_sendSessionMessage(\""@%params@"\");";
      %session.window.getObject(4).command = "RTBIC_endSession(\""@%params@"\");";
      %session.window.messageVector.pushBackLine("\c5* "@%oldName@" is now known as "@%params@".",0);
   }
}

RTBIC_SC.addHandle("MODE","onMode");
function RTBIC_SC::onMode(%this,%prefix,%params)
{
	nextToken(%prefix,nick," !");
	%mode = getSubStr(%params, StrLen(getWord(%params,0))+1, StrLen(%params));
	RTBIC_addLine("\c5* "@%nick@" has set mode: "@%mode);
	
	RTBIC_SC.sendLine("NAMES "@%this.currentChannel);
}

RTBIC_SC.addHandle("391","onTime");
function RTBIC_SC::onTime(%this,%prefix,%params)
{
	%time = getWord(%params,7);
	RTBIC_addLine("\c3Time: "@%time);
}

RTBIC_SC.addHandle("PRIVMSG","onMessage");
function RTBIC_SC::onMessage(%this,%prefix,%params)
{
   nextToken(%prefix,username,"!");
   %message = stripMLControlChars(nextToken(%params,destination," :"));
   %message = getSubStr(%params,strPos(%params,":")+1,strLen(%params));
   %message = stripMLControlChars(%message);
   
   if($Ignore::User[%username])
   {
      return;
   }
   
   if(%destination $= %this.currentChannel)
   {
      if(strPos(%message,"ACTION") $= 0)
		{
		   if($RTB::Options::IC::Filter::ShowActions)
		   {
            %message = "\c4* "@%username SPC getWords(%message,1,getWordCount(%message));
            RTBIC_addLine(%message);
		   }
			return;
		}
      RTBIC_addLine("\c0<"@%username@"> "@%message);
      
      if(!RTBIC_Window_Channel.visible)
         RTBIC_startFlashTab(RTBIC_SC.channelTab);
   }
   else if(%destination $= $RTB::CIRCClient::Cache::NickName && $RTB::Options::IC::AllowPM)
   {
      if(%username $= "StatServ" || %message $= "VERSION")
         return;
         
      if(RTBIC_hasSession(%username))
         %sess = RTBIC_getSession(%username);
      else
         %sess = RTBIC_createSession(%username);
         
      %sess.window.messageVector.pushBackLine("\c0<"@%username@"> "@%message,0);
      
      if(!%sess.window.visible)
         RTBIC_startFlashTab(%sess.tab);
         
      RTBIC_makeFirstResponder();
         
      if(!RTB_IRCClient.isAwake())
      {
         if($RTB::Options::IC::PMVisualNotify)
         {
            if(strLen(%message) > 40)
               %message = getSubStr(%message,0,40)@"...";
            RTBIC_pushNotification(%username,%message);
         }
         
         if($RTB::Options::IC::PMAudioNotify)
            alxPlay(RTBIC_PMSound);
      }
   }
}

RTBIC_SC.addHandle("303","onPresenceQueryReply");
function RTBIC_SC::onPresenceQueryReply(%this,%prefix,%params)
{
   %name = firstWord(nextToken(%params,prefix,":"));
   if(%name $= $RTB::CIRCClient::Cache::NickName)
   {
      if($RTB::Options::IC::CustomUsername && $RTB::Options::IC::CustomUser !$= "")
      {
         %this.sendLine("NICK "@$RTB::Options::IC::CustomUser);
         if($RTB::Options::IC::CustomPass !$= "")
            %this.sendLine("PRIVMSG NickServ IDENTIFY "@$RTB::Options::IC::CustomPass);
      }
      else
      {
         if($RTB::CIRCClient::Cache::DesNickName !$= "")
	         %this.sendLine("NICK "@$RTB::CIRCClient::Cache::DesNickName);
      }
         
      RTBIC_UserManifest.clear();
      %this.sendLine("JOIN #rtb");
   }
}

RTBIC_SC.addHandle("353","onNameList");
function RTBIC_SC::onNameList(%this,%prefix,%params)
{
   if(!%this.receivingNames)
   {
      %this.receivingNames = 1;
      RTBIC_UserManifest.clear();
   }
   
   %nameList = nextToken(%params,prefix,":");
   for(%i=0;%i<getWordCount(%nameList);%i++)
   {
      %name = getWord(%nameList,%i);
      RTBIC_addUser(%name,1);
   }
}

RTBIC_SC.addHandle("311","onWhoisPartA");
function RTBIC_SC::onWhoisPartA(%this,%prefix,%params)
{
   %username = getWord(%params,1);
   %nick = getWord(%params,2);
   %host = getWord(%params,3);
   %realname = nextToken(%params,prefix,":");
   
   RTBIC_addLine("\c4"@%username@" is "@%nick@"@"@%host@" * "@%realname);
}

RTBIC_SC.addHandle("307","onWhoisPartB");
function RTBIC_SC::onWhoisPartB(%this,%prefix,%params)
{
   %username = getWord(%params,1);
   
   RTBIC_addLine("\c4"@%username@" is a registered nick");
}

RTBIC_SC.addHandle("319","onWhoisPartC");
function RTBIC_SC::onWhoisPartC(%this,%prefix,%params)
{
   %username = getWord(%params,1);
   %channels = nextToken(%params,%prefix,":");
   
   RTBIC_addLine("\c4"@%username@" on "@%channels);
}

RTBIC_SC.addHandle("312","onWhoisPartD");
function RTBIC_SC::onWhoisPartD(%this,%prefix,%params)
{
   %username = getWord(%params,1);
   %server = getWord(%params,2);
   %servername = nextToken(%params,%prefix,":");
   
   RTBIC_addLine("\c4"@%username@" using "@%server@" "@%servername);
}

RTBIC_SC.addHandle("KICK","onKick");
function RTBIC_SC::onKick(%this,%prefix,%params)
{
	nextToken(%prefix,kicker,"!");
	%message = nextToken(%params,kickee,":");
	%kickee = getWord(%params,1);
	
	if(%kickee $= $RTB::CIRCClient::Cache::NickName)
	{
      if(%message $= %kicker)
      {
         RTBIC_addLine("\c2* You have been kicked by "@%kicker@". (No Reason)");
         RTBIC_popTab(RTBIC_SC.channelTab);
         RTBIC_UserManifest.clear();
         RTBIC_drawUserList();
         return;
      }
      else
      {
         RTBIC_addLine("\c2* You have been kicked by "@%kicker@". ("@%message@")");
         RTBIC_popTab(RTBIC_SC.channelTab);
         RTBIC_UserManifest.clear();
         RTBIC_drawUserList();
         return;
      }
	}
	else
	{
	   if(%message $= %kicker)
   		RTBIC_addLine("\c2* "@%kickee@" has been kicked by "@%kicker@". (No Reason)");
	   else
		   RTBIC_addLine("\c2* "@%kickee@" has been kicked by "@%kicker@". ("@%message@")",0);	

      RTBIC_removeUser(%kickee);
	}
}

RTBIC_SC.addHandle("433","onNickInUse");
function RTBIC_SC::onNickInUse(%this,%prefix,%params)
{
   %name = getWord(%params,1);
   RTBIC_addLine("\c2* Your name could not be changed to "@%name@" since it is already in use.");
}

RTBIC_SC.addHandle("366","onNameListDone");
function RTBIC_SC::onNameListDone(%this,%prefix,%params)
{
   %this.receivingNames = 0;
   RTBIC_drawUserList();
}

RTBIC_SC.addHandle("PING","onPing");
function RTBIC_SC::onPing(%this,%prefix,%params)
{
   %this.sendLine("PONG "@%params);
}

function RTBIC_ColorTest()
{
   RTBIC_addLine("\c00");
   RTBIC_addLine("\c11");
   RTBIC_addLine("\c22");
   RTBIC_addLine("\c33");
   RTBIC_addLine("\c44");
   RTBIC_addLine("\c55");
   RTBIC_addLine("\c66");
   RTBIC_addLine("\c77");
   RTBIC_addLine("\c88");
   RTBIC_addLine("\c99");
}

//*********************************************************
//* User Manifest
//*********************************************************
function RTBIC_lightRefresh()
{
   %value = $RTB::CIRCClient::Cache::SortSel;
   RTBIC_UserList.sort(0,1);
   RTBIC_drawBadges();
   
   if(%value !$= "" && %value > -1)
   {
      $RTB::CIRCClient::Cache::WasSort = 1;
      RTBIC_UserList.setSelectedById(%value);
   }
}

function RTBIC_drawUserList()
{
   %value = RTBIC_UserList.getSelectedId();
   
   RTBIC_UserList.clear();
   if(RTBIC_SC.currentChannel $= "")
   {
      RTBIC_drawBadges();
      return;
   }
      
   for(%i=0;%i<RTBIC_UserManifest.getCount();%i++)
   {
      %user = RTBIC_UserManifest.getObject(%i);
      RTBIC_UserList.addRow(%user,%user.status TAB %user.username,0);
   }
   
   RTBIC_UserList.sort(0,1);
   RTBIC_drawBadges();
   
   RTBIC_UserList.setSelectedById(%value);
}

function RTBIC_drawBadges()
{
   RTBIC_UserSwatch.clear();
   for(%i=0;%i<RTBIC_UserList.rowCount();%i++)
   {
      %entry = RTBIC_UserList.getRowText(%i);
      %status = getField(%entry,0);
      
      if(%status < 2)
      {
         %badgeBitmap = (%status $= 1) ? "./images/icon_silverBadge" : "./images/icon_goldBadge";
         %bitmap = new GuiBitmapCtrl()
         {
            position = "1" SPC (%i*16)+1;
            extent = "16 16";
            bitmap = %badgeBitmap;
         };
         RTBIC_UserSwatch.add(%bitmap);
      }
      else
      {
         %swatch = new GuiSwatchCtrl()
         {
            position = "1" SPC (%i*16)+1;
            extent = "16 16";
            color = "255 255 255 255";
         };
         RTBIC_UserSwatch.add(%swatch);
      }
   }
   if(RTBIC_UserSwatch.getCount() >= 1)
      %extentY = getWord(RTBIC_UserSwatch.getObject(RTBIC_UserSwatch.getCount()-1).position,1)+16;
   else
      %extentY = 16;
   RTBIC_UserSwatch.extent = "18" SPC %extentY;
}

function RTBIC_addUser(%username,%mass)
{
   %this = RTBIC_UserManifest;
      
   if(getSubStr(%username,0,1) $= "~")
      %status = "0";
   else if(getSubStr(%username,0,1) $= "@")
      %status = "0";
   else if(getSubStr(%username,0,1) $= "&")
      %status = "0";
   else if(getSubStr(%username,0,1) $= "+")
      %status = "1";
   else
      %status = "2";
      
   if(%status !$= "2")
      %fusername = getSubStr(%username,1,strLen(%username));
   else
      %fusername = %username;
   
   if(RTBIC_hasUser(%fusername))
   {
      %user = RTBIC_getUser(%fusername);
      %user.status = %status;
      %user.rawusername = %username;
   }
   else
   {
      %userSO = new ScriptObject()
      {
         class = "RTBIC_UserSO";
         username = %fusername;
         status = %status;
         rawusername = %username;
      };
      %this.add(%userSO);
      
      if(!%mass)
      {
         $RTB::CIRCClient::Cache::SortSel = RTBIC_UserList.getSelectedId();
         RTBIC_UserList.addRow(%userSO,%status TAB %fusername,0);
         RTBIC_lightRefresh();
      }
   }
}
function RTBIC_removeUser(%username)
{
   %this = RTBIC_UserManifest;
   if(!RTBIC_hasUser(%username))
      return;
      
   for(%i=0;%i<%this.getCount();%i++)
   {
      %so = %this.getObject(%i);
      if(%so.username $= %username)
      {
         $RTB::CIRCClient::Cache::SortSel = RTBIC_UserList.getSelectedId();
         RTBIC_UserList.removeRowById(%so);
         RTBIC_lightRefresh();
         %so.delete();
         return 1;
      }
   }
   return 0;
}
function RTBIC_getRawUser(%username)
{
   %this = RTBIC_UserManifest;
   for(%i=0;%i<%this.getCount();%i++)
   {
      %so = %this.getObject(%i);
      if(%so.rawusername $= %username)
         return %so;
   }
   return 0;
}
function RTBIC_getUser(%username)
{
   %this = RTBIC_UserManifest;
   for(%i=0;%i<%this.getCount();%i++)
   {
      %so = %this.getObject(%i);
      if(%so.username $= %username)
         return %so;
   }
   return 0;
}
function RTBIC_hasUser(%username)
{
   %this = RTBIC_UserManifest;
   for(%i=0;%i<%this.getCount();%i++)
   {
      %so = %this.getObject(%i);
      if(%so.username $= %username)
         return 1;
   }
   return 0;
}
function RTBIC_hasRawUser(%username)
{
   %this = RTBIC_UserManifest;
   for(%i=0;%i<%this.getCount();%i++)
   {
      %so = %this.getObject(%i);
      if(%so.rawusername $= %username)
         return 1;
   }
   return 0;
}
function RTBIC_UserManifest::dump(%this)
{
   echo(" RTBIC_UserManifest");
   echo("-------------------------------------");
   for(%i=0;%i<%this.getCount();%i++)
   {
      %so = %this.getObject(%i);
      echo(" "@%so.rawusername);
   }
   echo("-------------------------------------");
}
function RTBIC_forcePart()
{
   if(RTBIC_SC.currentChannel !$= "")
   {
      RTBIC_SC.sendLine("PART "@RTBIC_SC.currentChannel);
      RTBIC_popTab(RTBIC_SC.channelTab);
   }
}

//*********************************************************
//* Pane Management
//*********************************************************
function RTBIC_SetPane(%pane)
{
   for(%i=0;%i<RTB_IRCClient.getObject(0).getCount();%i++)
   {
      %ctrl = RTB_IRCClient.getObject(0).getObject(%i);
      if(%ctrl.isPane)
         %ctrl.setVisible(0);
   }
   
   for(%i=0;%i<RTBIC_TabSwatch.getCount();%i++)
   {
      %tab = RTBIC_TabSwatch.getObject(%i);
      %tab.setBitmap("base/client/ui/tab1");
   }
   
   if(%pane !$= "" && %pane !$= "main")
   {
      if(RTBIC_SessionManifest.isMember(%pane))
      {
         %pane.window.setVisible(1);
         %pane.window.messageBox.makeFirstResponder(1);
         if(%pane.tab.isFlashing)
            RTBIC_stopFlashTab(%pane.tab);
         %pane.tab.setBitmap("base/client/ui/tab1Use");
      }
      else
      {
         RTBIC_Window_Channel.setVisible(1);
         RTBIC_SendMessage.makeFirstResponder(1);
         if(RTBIC_SC.channelTab.isFlashing)
            RTBIC_stopFlashTab(RTBIC_SC.channelTab);
         RTBIC_SC.channelTab.setBitmap("base/client/ui/tab1Use");
      }
   }
   else
   {
      RTBIC_Window_Channel.setVisible(1);
      RTBIC_MessageBox.makeFirstResponder(1);
      if(RTBIC_SC.channelTab.isFlashing)
         RTBIC_stopFlashTab(RTBIC_SC.channelTab);
      RTBIC_SC.channelTab.setBitmap("base/client/ui/tab1Use");
   }
   if(%pane $= "")
      %pane = "main";
   $RTB::CIRCClient::CurrPane = %pane;
}

function RTBIC_coldOpenPane(%title)
{
   if(!RTBIC_hasTab(%title))
      return;
      
   canvas.pushDialog(RTB_IRCClient);
   %tab = RTBIC_getTab(%title);
   RTBIC_SetPane(%tab.session);
   RTBIC_ScrollToTab(%title);
   $RTB::CIRCClient::CurrPane.window.messageBox.makeFirstResponder(1);
}

//*********************************************************
//* Popup Management
//*********************************************************
function RTBIC_pushNotification(%title,%message)
{
   if(RTBIC_NotificationManifest.getCount() > 3)
      return;
   
   for(%i=0;%i<RTBIC_NotificationManifest.getCount();%i++)
   {
      if(RTBIC_NotificationManifest.getObject(%i).name $= %title)
      {
         %window = RTBIC_NotificationManifest.getObject(%i).window;
         %window.getObject(0).getObject(0).setText("<just:center>"@%message);
         %window.getObject(0).getObject(0).forceReflow();
         
         if(getWord(%window.getObject(0).getObject(0).extent,1) <= 24)
            %window.getObject(1).setVisible(0);
         else
            %window.getObject(1).setVisible(1);
         
         if(%window.status $= "goingDown")
         {
            cancel(%window.moveSchedule);
            RTBIC_scrollNotificationUp(%window);
         }
         else if(%window.status $= "holding")
         {
            cancel(%window.moveSchedule);
            %window.moveSchedule = schedule(3000,0,"RTBIC_scrollNotificationDown",%window);
         }
         return;
      }
   }
   
   %resX = getWord(getRes(),0);
   %resY = getWord(getRes(),1);
   
   %posX = %resX-150;
   %posY = %resY-(70*RTBIC_NotificationManifest.getCount());
   
   %window = new GuiWindowCtrl()
   {
      profile = GuiWindowProfile;
      position = %posX SPC %posY;
      extent = "150 70";
      horizSizing = "left";
      vertSizing = "top";
      text = %title;
      target = %resY-70;
      
      canClose = 1;
      canMinimize = 0;
      canMaximize = 0;
      canMove = 0;
      resizeWidth = 0;
      resizeHeight = 0;
      
      new GuiSwatchCtrl()
      {
         position = "9 33";
         extent = "132 25";
         color = "0 0 0 0";
         
         new GuiMLTextCtrl()
         {
            profile = RTB_Verdana12Pt;
            position = "0 0";
            vertSizing = "bottom";
            extent = "132 12";
            text = "<just:center>"@%message;
         };
      };
      
      new GuiTextCtrl()
      {
         profile = RTB_Verdana12Pt;
         position = "129 42";
         extent = "9 18";
         text = "...";
      };
      
      new GuiBitmapButtonCtrl()
      {
         position = "6 27";
         extent = "140 44";
         text = " ";
         bitmap = "./images/buttons/btnInvisible";
      };
   };
   Canvas.getObject(canvas.getCount()-1).add(%window);
   
   %so = new ScriptObject()
   {
      name = %title;
      window = %window;
   };
   RTBIC_NotificationManifest.add(%so);
   %window.manifest = %so;
   
   %window.closeCommand = %window.manifest@".delete();"@%window@".delete();";
   %window.getObject(2).command = %window.manifest@".delete();RTBIC_coldOpenPane(\""@%title@"\");"@%window@".delete();";
   %window.getObject(0).getObject(0).forceReflow();
   if(getWord(%window.getObject(0).getObject(0).extent,1) <= 24)
      %window.getObject(1).setVisible(0);
   
   RTBIC_scrollNotificationUp(%window);
}
function RTBIC_scrollNotificationUp(%window)
{
   if(!isObject(%window))
      return;
      
   %window.status = "goingUp";
   %target = %window.target;
   %current = getWord(%window.position,1);
   
   if(%current > %target)
      %window.position = vectorSub(%window.position,"0 5");
   else
   {
      %window.moveSchedule = schedule(5000,0,"RTBIC_scrollNotificationDown",%window);
      %window.status = "holding";
      return;
   }
   
   %window.moveSchedule = schedule(40,0,"RTBIC_scrollNotificationUp",%window);
}

function RTBIC_scrollNotificationDown(%window)
{
   if(!isObject(%window))
      return;
      
   %window.status = "goingDown";
   %target = getWord(getRes(),1);
   %current = getWord(%window.position,1);
   
   if(%current < %target)
      %window.position = vectorAdd(%window.position,"0 5");
   else
   {
      %window.manifest.delete();
      %window.delete();
      return;
   }
   
   %window.moveSchedule = schedule(40,0,"RTBIC_scrollNotificationDown",%window);
}

//*********************************************************
//* Tab Scrolling
//*********************************************************
function RTBIC_refreshScroll()
{
   for(%i=0;%i<RTBIC_TabSwatch.getCount();%i++)
   {
      %tab = RTBIC_TabSwatch.getObject(%i);
      %tab.position = (138*%i)+(%i) SPC "1";
   }
   if(RTBIC_TabSwatch.getCount() >= 1)
	   %xMax = getWord(RTBIC_TabSwatch.getObject(RTBIC_TabSwatch.getCount()-1).position,0)+136;
   else
      %xMax = 0;
	RTBIC_TabSwatch.extent = %xMax SPC "27";
	
   if(RTBIC_TabSwatch.getCount() < 5)
   {
      RTBIC_RightScroll.setVisible(0);
      RTBIC_LeftScroll.setVisible(0);
      RTBIC_TabSwatch.position = "0 0";
   }
   else
   {
      RTBIC_RightScroll.setVisible(1);
      RTBIC_LeftScroll.setVisible(1);
      RTBIC_RightScroll.setActive(1);
      RTBIC_LeftScroll.setActive(1);
      
      if(getWord(RTBIC_TabSwatch.position,0) < 0)
      {
         if(getWord(RTBIC_TabSwatch.extent,0)+getWord(RTBIC_TabSwatch.position,0) < 553)
            RTBIC_TabSwatch.position = vectorAdd(RTBIC_TabSwatch.position,553-(getWord(RTBIC_TabSwatch.extent,0)+getWord(RTBIC_TabSwatch.position,0)) SPC "0");
      }
      
      if(getWord(RTBIC_TabSwatch.extent,0)+getWord(RTBIC_TabSwatch.position,0) $= "553")
         RTBIC_RightScroll.setActive(0);
         
      if(getWord(RTBIC_TabSwatch.position,0) $= 0)
         RTBIC_LeftScroll.setActive(0);
   }
   
   %flashL = 0;
   %flashR = 0;
   for(%i=0;%i<RTBIC_TabSwatch.getCount();%i++)
   {
      %tab = RTBIC_TabSwatch.getObject(%i);
      if(%tab.isFlashing && !RTBIC_isTabVisible(%tab.text))
      {
         if(RTBIC_getTabDirection(%tab.text) $= "l")
            %flashL = 1;
         else if(RTBIC_getTabDirection(%tab.text) $= "r")
            %flashR = 1;
      }
   }
   
   if(%flashL && !RTBIC_LeftScroll.isFlashing)
      RTBIC_startFlashTab(RTBIC_LeftScroll);
   else if(!%flashL && RTBIC_LeftScroll.isFlashing)
      RTBIC_stopFlashTab(RTBIC_LeftScroll);
      
   if(%flashR && !RTBIC_RightScroll.isFlashing)
      RTBIC_startFlashTab(RTBIC_RightScroll);
   else if(!%flashR && RTBIC_RightScroll.isFlashing)
      RTBIC_stopFlashTab(RTBIC_RightScroll);
}
function RTBIC_ScrollRight()
{
   if(getWord(RTBIC_TabSwatch.extent,0)+getWord(RTBIC_TabSwatch.position,0) $= "553")
      return;
   RTBIC_TabSwatch.position = vectorSub(RTBIC_TabSwatch.position,"139 0");
   RTBIC_refreshScroll();
}
function RTBIC_ScrollLeft()
{
   if(getWord(RTBIC_TabSwatch.position,0) $= 0)
      return;
   RTBIC_TabSwatch.position = vectorAdd(RTBIC_TabSwatch.position,"139 0");
   RTBIC_refreshScroll();
}
function RTBIC_isTabVisible(%name)
{
   if(!RTBIC_hasTab(%name))
      return;
      
   %tab = RTBIC_getTab(%name);
   %virtPos = getWord(%tab.position,0)+getWord(RTBIC_TabSwatch.position,0);
   if(%virtPos >= 0 && %virtPos <= 417)
      return 1;
   else
      return 0;
}
function RTBIC_getTabVirtPos(%name)
{
   if(!RTBIC_hasTab(%name))
      return 0;
      
   %tab = RTBIC_getTab(%name);
   %virtPos = getWord(%tab.position,0)+getWord(RTBIC_TabSwatch.position,0);
   return %virtPos;
}
function RTBIC_getTabDirection(%name)
{
   if(!RTBIC_hasTab(%name))
      return 0;
      
   %tab = RTBIC_getTab(%name);
   %virtPos = RTBIC_getTabVirtPos(%name);
   if(%virtPos < 0)
      return "l";
   else if(%virtPos > 417)
      return "r";
   else
      return "v";
}
function RTBIC_hasTab(%name)
{
   for(%i=0;%i<RTBIC_TabSwatch.getCount();%i++)
   {
      %tab = RTBIC_TabSwatch.getObject(%i);
      if(%tab.text $= %name)
         return 1;
   }
   return 0;
}
function RTBIC_getTab(%name)
{
   for(%i=0;%i<RTBIC_TabSwatch.getCount();%i++)
   {
      %tab = RTBIC_TabSwatch.getObject(%i);
      if(%tab.text $= %name)
         return %tab;
   }
   return 0;
}
function RTBIC_ScrollToTab(%name)
{
   if(!RTBIC_hasTab(%name))
      return;
   if(RTBIC_isTabVisible(%name))
      return;
      
   %tab = RTBIC_getTab(%name);
   %dir = RTBIC_getTabDirection(%name);
   %virtPos = getWord(%tab.position,0)+getWord(RTBIC_TabSwatch.position,0);
   if(%dir $= "l")
   {
      RTBIC_TabSwatch.position = vectorSub(RTBIC_TabSwatch.position,%virtPos SPC "0");
      RTBIC_refreshScroll();
   }
   else if(%dir $= "r")
   {
      %moveDist = %virtPos-417;
      if(%moveDist < 0)
         %moveDist = %moveDist-139;
      RTBIC_TabSwatch.position = vectorSub(RTBIC_TabSwatch.position,%moveDist SPC "0");
      RTBIC_refreshScroll();
   }
}

//*********************************************************
//* Session Manifest Management
//*********************************************************
function RTBIC_createSession(%username)
{
   if(RTBIC_hasSession(%username))
      return;
      
   %sess = new ScriptObject()
   {
      username = %username;
   };
   RTBIC_SessionManifest.add(%sess);
   
   %sess.tab = RTBIC_pushTab(RTBIC_subTabName(%username),%sess,"RTBIC_endSession(\""@%username@"\");");
   %sess.window = RTBIC_generateWindow(%sess);
   %sess.tab.session = %sess;
   RTBIC_makeFirstResponder();
   
   return %sess;
}
function RTBIC_hasSession(%username)
{
   for(%i=0;%i<RTBIC_SessionManifest.getCount();%i++)
   {
      %sess = RTBIC_SessionManifest.getObject(%i);
      if(%sess.username $= %username)
         return 1;
   }
   return 0;
}
function RTBIC_getSession(%username)
{
   for(%i=0;%i<RTBIC_SessionManifest.getCount();%i++)
   {
      %sess = RTBIC_SessionManifest.getObject(%i);
      if(%sess.username $= %username)
         return %sess;
   }
   return 0;
}
function RTBIC_sendSessionMessage(%username)
{
   if(!RTBIC_hasSession(%username))
      return;
      
   %sess = RTBIC_getSession(%username);
   %message = %sess.window.messageBox.getValue();
   if(%message $= "")
      return;
      
   if(filterKey(%message))
   {
      %sess.window.messageBox.setValue("");
      %sess.window.messageVector.pushBackLine("\c2** DO NOT TELL PEOPLE YOUR BLOCKLAND KEY! **",0);
      return;
   }
   
   if(!RTBIC_SC.connected)
   {
      %sess.window.messageVector.pushBackLine("\c2ERROR: You are not connected.",0);
      %sess.window.messageBox.setValue("");
      return;
   }
      
   if(getSubStr(%message,0,1) $= "/")
   {
      if(getWord(%message, 0) $= "/me" || getWord(%message, 0) $= "/action")
      {
         %vars = getWords(%message,1,getWordCount(%message)-1);
         %message = "PRIVMSG "@%sess.username@" :\c0ACTION "@%vars@"\c0";
         RTBIC_SC.sendLine(%message);
         %sess.window.messageVector.pushBackLine("\c4* "@$RTB::CIRCClient::Cache::NickName SPC %vars,0);
      }
      else if(getWord(%message,0) $= "/ignore")
      {
         %user = getWord(%message,1);
         if($Ignore::User[%user])
         {
            $Ignore::User[%user] = "";
            %sess.window.messageVector.pushBackLine("\c6* You are no longer ignoring "@%user@".",0);
         }
         else
         {
            $Ignore::User[%user] = 1;
            %sess.window.messageVector.pushBackLine("\c6* You are now ignoring "@%user@".",0);
         }
      }
      else if(getWord(%message,0) $= "/unignore")
      {
         %user = getWord(%message,1);
         if($Ignore::User[%user])
         {
            $Ignore::User[%user] = "";
            %sess.window.messageVector.pushBackLine("\c6* You are no longer ignoring "@%user@".",0);
         }
      }
		else if(getWord(%message,0) $= "/msg")
		{
         %vars = getWords(%message,1,getWordCount(%message)-1);
			RTBIC_SC.sendLine("PRIVMSG "@%vars);
			
         %username = getWord(%message,1);
         if(RTBIC_hasSession(%username))
         {
            %sess = RTBIC_getSession(%username);
            RTBIC_SetPane(%sess);
            RTBIC_ScrollToTab(%username);
         }
         else
         {
            %sess = RTBIC_createSession(%username);
            RTBIC_SetPane(%sess);
            %sess.window.messageVector.pushBackLine("\c6* Now talking to "@%username@"...",0);
            RTBIC_ScrollToTab(%username);
         }
         %sess.window.messageVector.pushBackLine("\c0<"@$RTB::CIRCClient::Cache::NickName@"> "@getWords(%message,2,getWordCount(%message)-1),0);
		}
		else if(getWord(%message,0) $= "/slap")
		{
         %name = getField(RTBIC_UserList.getValue(),1);
         %num = getRandom(1,2);
         if(%num $= 1)
         {
            if(%name !$= "")
               %vars = "slaps "@%name@" around with a moose at large!";
            else
               %vars = "gets slapped around with a moose at large!";
         }
         else if(%num $= 2)
         {
            if(%name !$= "")
               %vars = "slaps "@%name@" around with a whale bone!";
            else
               %vars = "gets slapped around with a whale bone!";
         }
			%message = "PRIVMSG "@RTBIC_SC.currentchannel@" :\c0ACTION "@%vars@"\c0";
			
			RTBIC_SC.sendLine(%message);
			RTBIC_addLine("\c4* "@$RTB::CIRCClient::Cache::NickName SPC %vars);
		}
      else if(getWord(%message,0) $= "/kick")
      {
         RTBIC_SC.sendLine("KICK "@RTBIC_SC.currentchannel@" "@getWord(%message,1));
      }
      else if(getWord(%message,0) $= "/join")
      {
         if(RTBIC_SC.currentChannel !$= "")
            RTBIC_SC.sendLine("PART "@RTBIC_SC.currentChannel);
         RTBIC_SC.sendLine(getSubStr(%message,1,strLen(%message)));
      }
		else
         RTBIC_SC.sendLine(getSubStr(%message,1,strLen(%message)));
   }
   else
   {
      RTBIC_SC.sendLine("PRIVMSG "@%sess.username SPC %message);
      %sess.window.messageVector.pushBackLine("<"@$RTB::CIRCClient::Cache::NickName@">" SPC %message,0);
   }
   %sess.window.messageBox.setValue("");
}
function RTBIC_endSession(%username)
{
   if(!RTBIC_hasSession(%username))
      return;
      
   %sess = RTBIC_getSession(%username);
   if(%sess.window.visible)
   {
      for(%i=0;%i<RTBIC_TabSwatch.getCount();%i++)
      {
         %tab = RTBIC_TabSwatch.getObject(%i);
         if(%tab $= %sess.tab)
         {
            if(%i $= 0)
               break;
            %nextTab = RTBIC_TabSwatch.getObject(%i-1).session;
            break;
         }
      }
      RTBIC_SetPane(%nextTab);
   }
   %sess.window.delete();
   %sess.tab.delete();
   %sess.delete();
   
   RTBIC_refreshScroll();
}

//*********************************************************
//* GUI Manipulation
//*********************************************************
function RTBIC_addLine(%line)
{
   RTBIC_MessageVector.pushBackLine(%line,0);
}
function RTBIC_pushTab(%title,%window,%closeCommand)
{
	%button = new GuiBitmapButtonCtrl()
	{
		profile = BlockButtonProfile;
		extent = "136 25";
		position = (138*RTBIC_TabSwatch.getCount())+(RTBIC_TabSwatch.getCount()) SPC "1";
		command = "RTBIC_SetPane("@%window@");";
		text = %title;
		bitmap = "base/client/ui/tab1";
		color = "255 255 255 255";
		
		new GuiBitmapButtonCtrl()
		{
		   position = "117 8";
		   extent = "11 11";
		   bitmap = "./images/buttons/small/btnDelete";
		   text = " ";
		   command = %closeCommand;
		};
	};
	RTBIC_TabSwatch.add(%button);
	
	if(%closeCommand $= "")
	   %button.getObject(0).delete();
	
	if(RTBIC_TabSwatch.getCount() >= 1)
	   %xMax = getWord(RTBIC_TabSwatch.getObject(RTBIC_TabSwatch.getCount()-1).position,0)+136;
   else
      %xMax = 0;
	RTBIC_TabSwatch.extent = %xMax SPC "27";
	RTBIC_refreshScroll();
	
	return %button;
}
function RTBIC_popTab(%id)
{
   if(!isObject(%id))
      return;
      
   %id.delete();
   RTBIC_refreshScroll();
}
function RTBIC_startFlashTab(%tab)
{
	if(!isObject(%tab))
		return;

	%tab.isFlashing = 1;
	if(isEventPending(%tab.flashSchedule1))
		cancel(%tab.flashSchedule1);
	if(isEventPending(%tab.flashSchedule2))
		cancel(%tab.flashSchedule2);
	if(isEventPending(%tab.flashSchedule3))
		cancel(%tab.flashSchedule3);
	RTBIC_FlashTab(%tab);
	
	RTBIC_refreshScroll();
}
function RTBIC_FlashTab(%tab)
{
	if(!isObject(%tab))
		return;

	%tab.flashSchedule1 = %tab.schedule(500,"setColor","255 0 0 255");
	%tab.flashSchedule2 = %tab.schedule(750,"setColor","255 255 255 255");
	%tab.flashSchedule3 = schedule(750,0,"RTBIC_FlashTab",%tab);
}
function RTBIC_stopFlashTab(%tab)
{
	if(!isObject(%tab))
		return;

	%tab.setColor("255 255 255 255");
	cancel(%tab.flashSchedule1);
	cancel(%tab.flashSchedule2);
	cancel(%tab.flashSchedule3);
	%tab.isFlashing = 0;
}
function RTBIC_generateWindow(%sess)
{
	%swatch = new GuiSwatchCtrl()
	{
		profile = GuiDefaultProfile;
		position = "8 64";
		extent = "601 383";
		color = "0 0 0 0";
		visible = 0;
		   isPane = 1;
	};
	RTB_IRCClient.getObject(0).add(%swatch);

	%scroll = new GuiScrollCtrl()
	{
		profile = GuiScrollProfile;
		position = "4 2";
		extent = "591 352";
		childMargin = "4 0";
		columnWidth = 30;
		hScrollBar = "alwaysOff";
		vScrollBar = "dynamic";
	};
	%swatch.add(%scroll);

	%mVector = new GuiMessageVectorCtrl()
	{
		profile = RTBIC_VectorProfile;
		position = "5 0";
		extent = "568 2";
	};
	%scroll.add(%mVector);

	%rVector = new MessageVector();
	%mVector.attach(%rVector);
	%swatch.messageVector = %rVector;
	%swatch.messageVectorCtrl = %mVector;

	%messageBox = new GuiTextEditCtrl()
	{
		profile = RTBIC_MessageProfile;
		position = "55 362";
		extent = "422 16";
		accelerator = "enter";
		historySize = 10;
	};
	%swatch.add(%messageBox);
	%messageBox.altCommand = "RTBIC_sendSessionMessage(\""@%sess.username@"\");";
	
	%text = new GuiTextCtrl()
	{
		profile = RTB_Verdana12Pt;
		position = "3 361";
		extent = "47 18";
		text = "Message:";
	};
	%swatch.add(%text);

	%button = new GuiBitmapButtonCtrl()
	{
		profile = BlockButtonProfile;
		position = "483 358";
		extent = "54 22";
		command = "RTBIC_sendSessionMessage(\""@%sess.username@"\");";
		text = "Send";
		bitmap = "base/client/ui/button2";
		mColor = "255 255 255 255";
	};
	%swatch.add(%button);

	%button = new GuiBitmapButtonCtrl()
	{
		profile = BlockButtonProfile;
		position = "541 358";
		extent = "55 22";
		command = "RTBIC_endSession(\""@%sess.username@"\");";
		text = "Close";
		bitmap = "base/client/ui/button2";
		mColor = "255 255 255 255";
	};
	%swatch.add(%button);
	%swatch.session = %sess;
   %swatch.messageBox = %messageBox;
	return %swatch;
}

//*********************************************************
//* Support Functions
//*********************************************************
function RTBIC_subTabName(%name)
{
	if(strLen(%name) > 15)
		%name = getSubStr(%name,0,14)@"...";
	return %name;
}

function RTBIC_getNameList()
{
   if(RTBIC_SC.connected && RTBIC_SC.currentChannel !$= "")
   {
      RTBIC_SC.sendLine("NAMES "@RTBIC_SC.currentChannel);
   }
}

function RTBIC_parseMessageText(%text)
{
   for(%i=0;%i<strLen(%text);%i++)
   {
      if(getSubStr(%text,%i,7) $= "http://")
      {
         %beforeText = getSubStr(%text,0,%i);
         %middleText = "<a:";
      }
   }
}

//*********************************************************
//* Package
//*********************************************************
package RTBC_IRCClient
{
   function Canvas::setContent(%this,%content)
   {
      Parent::setContent(%this,%content);
      
      for(%i=0;%i<RTBIC_NotificationManifest.getCount();%i++)
      {
         %not = RTBIC_NotificationManifest.getObject(%i);
         %content.add(%not.window);
      }
   }
   
   function Canvas::pushDialog(%this,%content)
   {
      Parent::pushDialog(%this,%content);
      
      for(%i=0;%i<RTBIC_NotificationManifest.getCount();%i++)
      {
         %not = RTBIC_NotificationManifest.getObject(%i);
         %this.getObject(%this.getCount()-1).add(%not.window);
      }
   }
   
   function Canvas::popDialog(%this,%content)
   {
      Parent::popDialog(%this,%content);
      
      for(%i=0;%i<RTBIC_NotificationManifest.getCount();%i++)
      {
         %not = RTBIC_NotificationManifest.getObject(%i);
         %this.getObject(%this.getCount()-1).add(%not.window);
      }
   }
};
activatePackage(RTBC_IRCClient);

//*********************************************************
//* Execute-Time Instructions
//*********************************************************
if($RTB::Options::IC::AutoConnect && !RTBIC_SC.connected)
{
   RTBIC_Connect();
}
