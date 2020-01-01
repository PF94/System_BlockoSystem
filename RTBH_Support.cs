//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 103 $
//#      $Date: 2009-08-23 15:51:52 +0100 (Sun, 23 Aug 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/RTBH_Support.cs $
//#
//#      $Id: RTBH_Support.cs 103 2009-08-23 14:51:52Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Hybrid Support Script
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBH_Support = 1;

//#####################################################################################################
//
//   __      __         _       _     _          
//   \ \    / /        (_)     | |   | |         
//    \ \  / /__ _ _ __ _  __ _| |__ | | ___ ___ 
//     \ \/ // _` | '__| |/ _` | '_ \| |/ _ | __|
//      \  /| (_| | |  | | (_| | |_) | |  __|__ \
//       \/  \__,_|_|  |_|\__,_|_.__/|_|\___|___/
//
//
//##################################################################################################### 

//*********************************************************
//* Global Connection Variables
//*********************************************************
$RTB::Connection::Timeout     = 10; //> 10 Second Timeout (Sometimes the server just "hangs")
$RTB::Connection::Retries     = 3;  //> 3 Retries, then fail message
$RTB::Connection::Host        = "api.returntoblockland.com"; //> Type this into your browser, it rocks

//*********************************************************
//* Global Variables
//*********************************************************
$RTB::CModManager::DefaultBLMods = -1;
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Weapon_Sword";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Weapon_Spear";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Weapon_Rocket_Launcher";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Weapon_Push_Broom";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Weapon_Horse_Ray";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Weapon_Guns_Akimbo";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Weapon_Gun";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Weapon_Bow";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Vehicle_Tank";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Vehicle_Rowboat";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Vehicle_Pirate_Cannon";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Vehicle_Magic_Carpet";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Vehicle_Jeep";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Vehicle_Horse";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Vehicle_Flying_Wheeled_Jeep";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Vehicle_Ball";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Sound_Synth4";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Sound_Phone";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Sound_Beeps";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Script_ClearSpam";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Projectile_Radio_Wave";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Projectile_Pong";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Projectile_Pinball";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Projectile_GravityRocket";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Print_Letters_Default";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Print_2x2r_Default";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Print_2x2f_Default";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Print_1x2f_Default";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Player_Quake";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Player_No_Jet";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Player_Leap_Jet";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Player_Jump_Jet";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Player_Fuel_Jet";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Particle_Tools";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Particle_Player";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Particle_Grass";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Particle_FX_Cans";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Particle_Basic";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Music_Default";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Map_Tutorial";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Map_Slopes";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Map_Slate";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Map_KitchenDark";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Map_Kitchen";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Map_Construct";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Map_BedroomDark";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Map_Bedroom";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Light_Basic";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Light_Animated";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Item_Skis";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Face_Default";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Emote_Love";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Emote_Hate";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Emote_Confusion";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Emote_Alarm";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Decal_Default";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Brick_Large_Cubes";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "Brick_Arch";
$RTB::CModManager::DefaultBLMod[$RTB::CModManager::DefaultBLMods++] = "System_ReturnToBlockland";

//#####################################################################################################
//
//    _   _      _                      _    _             
//   | \ | |    | |                    | |  (_)            
//   |  \| | ___| |___      _____  _ __| | ___ _ __   __ _ 
//   | . ` |/ _ \ __\ \ /\ / / _ \| '__| |/ / | '_ \ / _` |
//   | |\  |  __/ |_ \ V  V / (_) | |  |   <| | | | | (_| |
//   |_| \_|\___|\__| \_/\_/ \___/|_|  |_|\_\_|_| |_|\__, |
//                                                    __/ |
//                                                   |___/
//
//##################################################################################################### 

//*********************************************************
//* Switchboard Definition, Instantiation and Methods
//* -------------------------------------------------------
//* Switchboard Line Types:
//*
//*   0     >>       no queue (cancel current requests)
//*   1     >>       standard linear queue
//*   2     >>       overwrite if matching cmd
//*   3     >>       overwrite if matching cmd & string
//*
//*********************************************************
//- RTB_createSwitchboard (Creates a switchboard instance)
function RTB_createSwitchboard(%moduleID,%apiID)
{
   if(!isObject(RTB_Switchboards))
      new SimGroup(RTB_Switchboards);
   
   %oName = "RTB_SB_"@%moduleID;   
   if(isObject(%oName))
      return %oName;
   
   %sb = new ScriptGroup(%oName)
   {
      class = "RTB_Switchboard";
      
      moduleID = %moduleID;
      apiID = %apiID;
      
      host = $RTB::Connection::Host;
      port = 80;
   };
   RTB_Switchboards.add(%sb);
   
   return %sb;
}

//- RTB_Switchboard::instantiateSocket (Creates a switchboard socket and creates the queue for it)
function RTB_Switchboard::instantiateSocket(%this,%line,%type)
{
   %socket = new TCPObject()
   {
      line = %line;
      switchboard = %this;
      
      type = %type;
      
      rtbObject = 1;
   };
   %this.add(%socket);
   %socket.neutralise();
   
   %queue = new ScriptObject()
   {
      class = "RTB_CallQueue";
      line = %line;
      
      size = 0;
   };
   %this.add(%queue);
   
   %socket.queue = %queue;
   %queue.socket = %socket;

   return %socket;
}

//- RTB_Switchboard::registerResponseHandler (Creates an entry for data routing)
function RTB_Switchboard::registerResponseHandler(%this,%cmd,%handler,%exempt)
{
   if(%cmd $= "" || %handler $= "")
      return 0;
      
   %this.r_[strUpr(%cmd)] = %handler;   
   if(%exempt)
      %this.e_[strUpr(%cmd)] = true;
      
   return 1;
}

//- RTB_Switchboard::registerFailHandler (Creates an entry for failed data receival routing)
function RTB_Switchboard::registerFailHandler(%this,%cmd,%handler)
{
   if(%cmd $= "" || %handler $= "")
      return 0;
      
   %this.r_fail_[strUpr(%cmd)] = %handler;
   return 1;
}

//- RTB_Switchboard::registerLine (Registers a line on the switchboard for usage)
function RTB_Switchboard::registerLine(%this,%line,%type)
{
   if(%line < 1)
      return 0;
      
   if(isObject(%this.l_[%line]))
      return 0;
      
   %socket = %this.instantiateSocket(%line,%type);
      
   %this.l_[%line] = %socket;
   return 1;
}

//- RTB_Switchboard::killLine (Neutralises a particular line and cancels any outgoing/incoming transmission)
function RTB_Switchboard::killLine(%this,%line)
{
   %socket = %this.l_[%line];
   %socket.neutralise();
}

//- RTB_Switchboard::setLineProperty (Sets line-specific properties and settings)
function RTB_Switchboard::setLineProperty(%this,%line,%property,%value)
{
   %socket = %this.l_[%line];
   %socket.p_[%property] = %value;
}

//- RTB_Switchboard::getLineSocket (Returns a line socket instance for the line)
function RTB_Switchboard::getLineSocket(%this,%line)
{
   if(isObject(%this.l_[%line]))
      return %this.l_[%line];
   else
      return 0;
}

//- RTB_Switchboard::placeCall (Registers a call with the switchboard to be processed by the operator)
function RTB_Switchboard::placeCall(%this,%line,%cmd,%string)
{
   %cmd = strUpr(%cmd);
   
   if(!isObject(%this.getLineSocket(%line)))
   {
      echo("\c2ERROR: Cannot send cmd to uninstantiated line socket: "@%cmd);
      return;
   }
   
   if(%this.getLineSocket(%line).type $= 0)
   {
      %this.getLineSocket(%line).neutralise();
      %this.getLineSocket(%line).plug(%cmd,%string);
   }
   else
   {
      %this.getLineSocket(%line).queue.push(%cmd,%string);
   }
}

//*********************************************************
//* Queue Methods
//*********************************************************
//- RTB_CallQueue::push (Pushes a call onto the queue)
function RTB_CallQueue::push(%this,%cmd,%string)
{
   if(%this.socket.s_occupied)
   {
      if(%this.socket.type $= 1)
      {
         %this.i_cmd[%this.size] = %cmd;
         %this.i_str[%this.size] = %string;
         %this.size++;
      }
      else if(%this.socket.type $= 2)
      {
         for(%i=0;%i<%this.size;%i++)
         {
            if(%this.i_cmd[%i] $= %cmd)
               break;
         }            
         %this.i_cmd[%i] = %cmd;
         %this.i_str[%i] = %string;
         
         if(%i $= %this.size)
            %this.size++;
      }
      else if(%this.socket.type $= 3)
      {
         if(%this.socket.t_cmd $= %cmd && %this.socket.t_string $= %string)
         {
            %this.socket.unplug();
            %this.socket.plug(%cmd,%string);
         }
         else
         {
            for(%i=0;%i<%this.size;%i++)
            {
               if(%this.i_cmd[%i] $= %cmd && %this.i_str[%i] $= %string)
                  break;
            }            
            %this.i_cmd[%i] = %cmd;
            %this.i_str[%i] = %string;
            
            if(%i $= %this.size)
               %this.size++;
         }
      }
   }
   else
      %this.socket.plug(%cmd,%string);
}

//- RTB_CallQueue::plug (Plugs a call into a socket and cycles the queue)
function RTB_CallQueue::plug(%this)
{
   if(%this.socket.s_occupied)
      return;
      
   if(%this.i_cmd[0] $= "")
      return;
      
   %this.socket.plug(%this.i_cmd[0],%this.i_str[0]);
   
   %this.cycle();
}

//- RTB_CallQueue::cycle (Removes an item from the queue and bumps all items up a space)
function RTB_CallQueue::cycle(%this)
{
   for(%i=0;%i<%this.size;%i++)
   {
      %this.i_cmd[%i] = %this.i_cmd[%i+1];
      %this.i_str[%i] = %this.i_str[%i+1];
   }
   %this.i_cmd[%i] = "";
   %this.i_str[%i] = "";
   %this.size--;
}

//*********************************************************
//* TCPObject Methods, Definition & Handling
//* -------------------------------------------------------
//*
//*  s_*                >>    status data
//*   s_connected       >>     (bool) connected to host
//*   s_sending         >>     (bool) sending data
//*   s_receiving       >>     (bool) receiving data
//*   s_occupied        >>     (bool) indicates current usage
//*
//*  p_*                >>    property data
//*   p_["Keep-Alive"]  >>     (bool) indicates keep-alive status
//*   p_["No-Retries"]  >>     (bool) prevents retries on timeouts
//*
//*  t_*                >>    transmission data
//*   t_cmd             >>     (string) transmit cmd
//*   t_string          >>     (string) transmit query string
//*   t_request         >>     (string) transmit http request
//*   t_time            >>     (int) time of request
//*   t_attempts        >>     (int) number of retries
//*
//*  c_*                >>    cache data
//*   c_lastCmd         >>     (string) last cmd received
//*   c_lastLine        >>     (string) last line received
//*
//*  d_hangTime         >>     (delay) time before timeout
//*
//*********************************************************
//- TCPObject::plug (Loads a call into a socket and activates the line)
function TCPObject::plug(%this,%cmd,%string)
{
   %this.t_cmd = %cmd;
   
   for(%i=0;%i<10;%i++)
   {
      if(%this.t_string $= "")
         %this.t_string = "arg1="@getField(%string,%i);
      else
         %this.t_string = %this.t_string@"&arg"@%i+1@"="@getField(%string,%i);
   }
   %this.t_rawString = %string;
   
   %this.activateLine();
}

//- TCPObject::neutralise (Kills all transmission and resets the line + socket)
function TCPObject::neutralise(%this)
{
   if(%this.s_connected)
   {
      %this.disconnect();
   }
   %this.s_connected = 0;
   %this.s_sending = 0;
   %this.s_receiving = 0;
   %this.s_occupied = 0;
   
   %this.t_cmd = "";
   %this.t_string = "";
   %this.t_rawString = "";
   %this.t_request = "";
   %this.t_time = 0;
   %this.t_attempts = 0;
   
   %this.c_lastCmd = "";
   %this.c_lastLine = "";
   
   if(isEventPending(%this.d_hangTime))
      cancel(%this.d_hangTime);     
   %this.d_hangTime = "";
}

//- TCPObject::activateLine (Activates a socket, prepares the transmission then connects/opens the line)
function TCPObject::activateLine(%this)
{
   if(%this.t_cmd $= "")
      return 0;
      
   %this.s_occupied = 1;
   %this.s_sending = 1;

   if(%this.t_string !$= "")
      %this.t_string = "c="@%this.t_cmd@"&n="@$Pref::Player::NetName@"&b="@getNumKeyID()@"&"@%this.t_string;
   else
      %this.t_string = "c="@%this.t_cmd@"&n="@$Pref::Player::NetName@"&b="@getNumKeyID();
      
   if($RTB::Connection::Session !$= "")
      %this.t_string = %this.t_string@"&"@$RTB::Connection::Session;
   
   if($RTB::Debug $= 3)   
      %this.t_string = %this.t_string@"&XDEBUG_PROFILE=1";

   if(%this.p_["Keep-Alive"] == 1)
      %connection = "Keep-Alive\r\nKeep-Alive: timeout=2, max=10";
   else
      %connection = "close";
      
   %contentLen = strLen(%this.t_string);
   %this.t_request = "POST /apiRouter.php?d="@%this.switchboard.apiID@" HTTP/1.1\r\nHost: "@%this.switchboard.host@"\r\nUser-Agent: Torque/1.0\r\nConnection: "@%connection@"\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: "@%contentLen@"\r\n\r\n"@%this.t_string@"\r\n";
   
   if(isEventPending(%this.d_hangTime))
      cancel(%this.d_hangTime);      
   %this.d_hangTime = %this.schedule($RTB::Connection::Timeout*1000,"onTimeout");
   
   %this.t_time = getSimTime();
   
   if(%this.s_connected)
      %this.openLine();
   else
      %this.connect(%this.switchboard.host@":"@%this.switchboard.port);
}

//- TCPObject::openLine (Opens a line for the call and pushes it)
function TCPObject::openLine(%this)
{
   if(%this.s_connected $= "")
      return;
      
   if(%this.t_request $= "")
      return;
      
   if($RTB::Debug)
      echo("\c5>> Transmit Requested: "@%this.switchboard.apiID@"/"@%this.t_cmd@" > "@%this.t_string);
   if($RTB::Debug > 1)
      echo("\c5"@strReplace(%this.t_request,"\r\n","\r\n\c5"));
      
   %this.send(%this.t_request);
}

//- TCPObject::unplug (Unplugs a line from the socket)
function TCPObject::unplug(%this)
{
   %this.disconnect();
   %this.onDisconnect();
}

//*********************************************************
//* Global Callbacks
//*********************************************************
//- RTB_onDefine (Defines specific RTB settings for the client after auth)
function RTB_onDefine(%this,%line)
{
   //If you remove this code it'll stop you getting barred by RTB! Please don't!
   if(getField(%line,0) $= "BARRED")
   {
      if(getField(%line,1) $= "0")
      {
         $RTB::Barred::Reason = "";
         $RTB::Barred::Date = "";         
         
         $RTB::Barred::System = 0;
         $RTB::Barred::Link = 0;
         $RTB::Barred::ClientAuth = 0;
         $RTB::Barred::ServerAuth = 0;
         $RTB::Barred::Upload = 0;
         $RTB::Barred::Download = 0;
         $RTB::Barred::Comment = 0;
         $RTB::Barred::Rate = 0;
         $RTB::Barred::ServerInfo = 0;
      }
      else
      {
         $RTB::Barred::Reason = getField(%line,2);
         $RTB::Barred::Date = getField(%line,3);
         
         $RTB::Barred::System = getField(%line,4);
         $RTB::Barred::Link = getField(%line,5);
         $RTB::Barred::ClientAuth = getField(%line,6);
         $RTB::Barred::ServerAuth = getField(%line,7);
         $RTB::Barred::Upload = getField(%line,8);
         $RTB::Barred::Download = getField(%line,9);
         $RTB::Barred::Comment = getField(%line,10);
         $RTB::Barred::Rate = getField(%line,11);
         $RTB::Barred::ServerInfo = getField(%line,12);
      }
      
      if($RTB::RTBC_ModManager)
         RTBMM_setBarred();
   }
   else if(getField(%line,0) $= "CRAPON")
   {
      if(getField(%line,1) $= "CRC")
         $CrapOnCRC_[getField(%line,2)] = true;
      else if(getField(%line,1) $= "NAME")
         $CrapOnName_[getField(%line,2)] = true;
   }
   else if(getField(%line,0) $= "GUIDL")
   {
      //Essentially a killswitch incase an exploit is found in the gui downloader that compromises user security
      if(getField(%line,1) $= "KILL")
         $RTB::Options::GT::Enable = 0;
      else if(getField(%line,1) $= "RESURRECT")
         $RTB::Options::GT::Enable = 1;
         
      export("$RTB::Options*","config/client/rtb/prefs.cs");
   }
   else if(getField(%line,0) $= "CONTENTDL")
   {
      //Essentially a killswitch incase the content downloader breaks some shit
      if(getField(%line,1) $= "KILL")
         $RTB::Options::CD::DownloadContent = 0;
      else if(getField(%line,1) $= "RESURRECT")
         $RTB::Options::CD::DownloadContent = 1;
         
      export("$RTB::Options*","config/client/rtb/prefs.cs");
   }   
}

//*********************************************************
//* TCPObject Callbacks
//*********************************************************
package RTBH_Support
{
   function TCPObject::onConnected(%this)
   {
      if(%this.rtbObject)
      {
         if($RTB::Debug)
            echo("\c4>> Transmit Connected");
            
         %this.s_connected = 1;
         %this.openLine();
      }
      else
      {
         if(isFunction(Parent,"onConnected"))
            Parent::onConnected(%this);
      }
   }
   
   function TCPObject::onLine(%this, %line)
   {
      if(%this.rtbObject)
      { 
         %this.s_sending = 0;
         cancel(%this.d_hangTime);
         
         if($RTB::Debug)
            echo("\c2>> Transmit Line ("@%line@")");
            
         if(getField(%line,0) $= "DEFINE")
         {
            call("RTB_onDefine",%this,getFields(%line,1,getFieldCount(%line)));
            return;
         }
         
         if(getField(%line,0) $= "MESSAGE")
         {
            if($Server::Dedicated)
               echo("RTB NOTIFICATION: "@getField(%line,2));
            else
               messageBoxOK("RTB Notification",getField(%line,2));
            return;
         }
         
         if(getField(%line,0) $= "ERROR")
         {
            %cmd = %this.t_cmd;
            
            if(%this.switchboard.r_fail_[%cmd] !$= "")
            {
               %call = %this.switchboard.r_fail_[%cmd]@"("@%this@",\"Fail\",\""@%this.t_rawString@"\");";
               eval(%call);
            }
            else if(%this.switchboard.r_fail_["DEFAULT"] !$= "")
            {
               %call = %this.switchboard.r_fail_["DEFAULT"]@"("@%this@",\""@%cmd@"\",\"Fail\",\""@%this.t_rawString@"\");";
               eval(%call);
            }
            %this.neutralise();
            return;
         }
         
         if(getField(%line,0) $= "NOTIFY")
         {
		      if($Tmp::HasNotifyCode[getField(%line,1)])
			      return;
			      
            if($Server::Dedicated)
               echo("RTB NOTIFICATION: "@getField(%line,2));
            else
               messageBoxOK("RTB Notification",getField(%line,2));
               
            $Tmp::HasNotifyCode[getField(%line,1)] = 1;
            return;
         }
            
         if(%line $= "END")
         {
            %cmd = %this.t_cmd;
            %cmdEnd = %this.switchboard.r_[%cmd]@"Stop";
            if(isFunction(%cmdEnd))
            {
               call(%cmdEnd,%this);
            }
            %this.s_receiving = 0;
            
            if(!%this.p_["Keep-Alive"])
               %this.unplug();
            else
            {
               %this.s_sending = 0;
               %this.s_receiving = 0;
               %this.s_occupied = 0;
               
               %this.t_string = "";
            }
               
            %this.queue.plug();
            return;
         }
         
         if(strPos(%line,"Set-Cookie:") $= 0)
         {
            %cookie = getSubStr(%line,strPos(%line,": ")+2,strLen(%line));
            $RTB::Connection::Session = getSubStr(%cookie,0,strPos(%cookie,";"));            
         }
         
         if(%this.c_lastLine $= "")
            %this.s_receiving = 1;            
         %this.c_lastLine = %line;
            
         if(!%this.s_receiving || %line $= "")
            return;
            
         if(%this.switchboard.r_["LINE"] !$= "")
            call(%this.switchboard.r_["LINE"],%this,%line);
         
         %cmd = getField(%line,0);
         if(%cmd !$= %this.t_cmd && !%this.switchboard.e_[%cmd])
            return;
            
         if(%this.switchboard.r_[%cmd] !$= "")
         {
            if(%this.c_lastCmd !$= %cmd)
            {
               %call = %this.switchboard.r_[%cmd]@"Start";
               if(isFunction(%call))
                  call(%call,%this);
               %this.c_lastCmd = %cmd;
            }
            call(%this.switchboard.r_[%cmd],%this,getFields(%line,1,getFieldCount(%line)-1),getField(%line,1),getField(%line,2),getField(%line,3),getField(%line,4),getField(%line,5),getField(%line,6),getField(%line,7),getField(%line,8),getField(%line,9),getField(%line,10),getField(%line,11),getField(%line,12),getField(%line,13),getField(%line,14),getField(%line,15),getField(%line,16),getField(%line,17),getField(%line,18),getField(%line,19),getField(%line,20));
         }
      }
      else
      {
         if(isFunction(Parent,"onLine"))
            Parent::onLine(%this,%line);
      }
   }
   
   function TCPObject::onConnectFailed(%this)
   {
      if(%this.rtbObject)
      {
         cancel(%this.d_hangTime);
         if($RTB::Debug)
            echo("\c2>> Transmit Connect Failed [END]");
            
         %this.s_sending = 0;
         %cmd = %this.t_cmd;
         
         if(%this.switchboard.r_fail_[%cmd] !$= "")
         {
            %call = %this.switchboard.r_fail_[%cmd]@"("@%this@",\"Fail\",\""@%this.t_rawString@"\");";
            eval(%call);
         }
         else if(%this.switchboard.r_fail_["DEFAULT"] !$= "")
         {
            %call = %this.switchboard.r_fail_["DEFAULT"]@"("@%this@",\""@%cmd@"\",\"Fail\",\""@%this.t_rawString@"\");";
            eval(%call);
         }
         %this.neutralise();
      }
      else
      {
         if(isFunction(Parent,"onConnectFailed"))
            Parent::onConnectFailed(%this);
      }
   }
   
   function TCPObject::onDNSFailed(%this)
   {
      if(%this.rtbObject)
      {
         cancel(%this.d_hangTime);
         if($RTB::Debug)
            echo("\c2>> Transmit DNS Failed [END]");
            
         %this.s_sending = 0;
         %cmd = %this.t_cmd;
         
         if(%this.switchboard.r_fail_[%cmd] !$= "")
         {
            %call = %this.switchboard.r_fail_[%cmd]@"("@%this@",\"DNS\",\""@%this.t_rawString@"\");";
            eval(%call);
         }
         else if(%this.switchboard.r_fail_["DEFAULT"] !$= "")
         {
            %call = %this.switchboard.r_fail_["DEFAULT"]@"("@%this@",\""@%cmd@"\",\"DNS\",\""@%this.t_rawString@"\");";
            eval(%call);
         }
         %this.neutralise();
      }
      else
      {
         if(isFunction(Parent,"onDNSFailed"))
            Parent::onDNSFailed(%this);
      }
   }
   
   function TCPObject::onTimeout(%this)
   {
      if(%this.rtbObject)
      {
         if(%this.t_attempts < $RTB::Connection::Retries && !%this.p_["No-Retries"])
         {
            if($RTB::Debug)
            {
               echo("\c2>> Transmit Timed Out [R"@%this.t_attempts@"]");
               echo("\c4>> Retrying Transmit...");
            }
            %this.disconnect();
            %this.s_connected = 0;
            %this.activateLine();
            %this.t_attempts++;
         }
         else
         {
            if($RTB::Debug)
               echo("\c2>> Transmit Timed Out [END]");
               
            %this.s_sending = 0;
            %cmd = %this.t_cmd;
            
            if(%this.switchboard.r_fail_[%cmd] !$= "")
            {
               %call = %this.switchboard.r_fail_[%cmd]@"("@%this@",\"Timeout\",\""@%this.t_rawString@"\");";
               eval(%call);
            }
            else if(%this.switchboard.r_fail_["DEFAULT"] !$= "")
            {
               %call = %this.switchboard.r_fail_["DEFAULT"]@"("@%this@",\""@%cmd@"\",\"Timeout\",\""@%this.t_rawString@"\");";
               eval(%call);
            }
            %this.neutralise();
         }
      }
   }
   
   function TCPObject::onDisconnect(%this)
   {
      if(%this.rtbObject)
      {
         cancel(%this.d_hangTime);
         if($RTB::Debug)
            echo("\c2>> Transmit Disconnect");
         
         if(%this.s_sending)
         {
            %this.s_connected = 0;
            %this.activateLine();
         }
         else
            %this.neutralise();
      }
      else
      {
         if(isFunction(Parent,"onDisconnect"))
            Parent::onDisconnect(%this);
      }
   }
   
   function Script_GUI::getClassName(%this)
   {
      return %this.className;
   }
   
   function Script_GUI::getValue(%this)
   {
      return %this.text;
   }
};
activatePackage(RTBH_Support);

//#####################################################################################################
//
//    ______                _   _                 
//   |  ____|              | | (_)                
//   | |__ _   _ _ __   ___| |_ _  ___  _ __  ___ 
//   |  __| | | | '_ \ / __| __| |/ _ \| '_ \/ __|
//   | |  | |_| | | | | (__| |_| | (_) | | | \__ \
//   |_|   \__,_|_| |_|\___|\__|_|\___/|_| |_|___/
//                                              
//
//##################################################################################################### 

//*********************************************************
//* Support Functions
//*********************************************************
//- SimGroup::getCanvasPosition (returns absolute position of a control on the canvas)
function SimGroup::getCanvasPosition(%this)
{
   %targ = %this;
   %x = getWord(%this.position,0);
   %y = getWord(%this.position,1);
   while(isObject(%targ.getGroup()))
   {
      %parent = %targ.getGroup();
      if(%parent.getName() $= "Canvas")
         return %x SPC %y;
         
      %x += getWord(%parent.position,0);
      %y += getWord(%parent.position,1);
      %targ = %parent;
   }
}

//- SimGroup::shift (moves a gui in the X or Y)
function SimGroup::shift(%this,%x,%y)
{
   %this.position = vectorAdd(%this.position,%x SPC %y);
}

//- SimGroup::swap (swaps the position of two children of a simgroup)
function SimGroup::swap(%this,%a,%b)
{
	%childA = %this.getObject(%a);
	%childB = %this.getObject(%b);

	for(%i=0;%i<%this.getCount();%i++)
	{
		%ctrl = %this.getObject(%i);
		%order[%i] = %ctrl;
	}
	%order[%a] = %childB;
	%order[%b] = %childA;

	for(%i=0;%i<%this.getCount();%i++)
	{
		%this.pushToBack(%order[%i]);
	}
}

//- GuiControl::getLowestPoint (finds the lowest point within a gui)
function GuiControl::getLowestPoint(%this)
{
   %lowest = 0;
   for(%i=0;%i<%this.getCount();%i++)
   {
      %obj = %this.getObject(%i);
      %low = getWord(%obj.position,1) + getWord(%obj.extent,1);
      if(%low > %lowest)
         %lowest = %low;
   }
   return %lowest;
}

//- SimGroup::getBottom (recursively gets to the bottom of an object tree, with %offset being an offset from the bottom)
function SimGroup::getBottom(%this,%offset)
{
   %parent = %this;
   %layer[0] = %parent;

   %k = 1;
   while(%parent.getCount() >= 1)
   {
      %parent = %parent.getObject(0);
      %layer[%k] = %parent;
      %k++;
   }
   if(%offset > %k)
      %offset = %k-1;
      
   return %layer[%k-%offset-1];
}

//- SimGroup::clear (had problems with standard method not doing the job, so overloaded it)
function SimGroup::clear(%this)
{
   while(%this.getCount() > 0)
   {
      %this.getObject(0).delete();
   }
}

//- filterString (removes all characters not in %allowed)
function filterString(%string,%allowed)
{
   for(%i=0;%i<strLen(%string);%i++)
   {
      %char = getSubStr(%string,%i,1);
      if(strPos(%allowed,%char) >= 0)
         %return = %return@%char;
   }
   return %return;
}

//- filterOutString (removes all characters in %disallowed)
function filterOutString(%string,%disallowed)
{
   for(%i=0;%i<strLen(%string);%i++)
   {
      %char = getSubStr(%string,%i,1);
      if(strPos(%disallowed,%char) < 0)
         %return = %return@%char;
   }
   return %return;
}

//- stringMatch (returns true if contents of %string can be found in %allowed)
function stringMatch(%string,%allowed)
{
   for(%i=0;%i<strLen(%string);%i++)
   {
      %char = getSubStr(%string,%i,1);
      if(strPos(%allowed,%char) < 0)
         return 0;
   }
   return 1;
}

//- min (finds smallest value)
function min(%a1,%a2,%a3,%a4,%a5,%a6,%a7,%a8,%a9)
{
   for(%i=1;%i<10;%i++)
   {
      if((%a[%i] < %min || %min $= "") && %a[%i] !$= "")
         %min = %a[%i];
   }
   return %min;
}

//- max (finds biggest value)
function max(%a1,%a2,%a3,%a4,%a5,%a6,%a7,%a8,%a9)
{
   %max = 0;
   for(%i=1;%i<10;%i++)
   {
      if((%a[%i] > %max || %max $= "") && %a[%i] !$= "")
         %max = %a[%i];
   }
   return %max;
}

//- isReadonly (determines whether a particular file is read only or not)
function isReadonly(%file)
{
   return (isWriteableFileName(%file) == 0);
}

//- RTB_addControlMap (adds a control map to the controls list)
function RTB_addControlMap(%inputDevice,%keys,%name,%command)
{
   if(!$RTB::addedCatSep)
   {
	   $remapDivision[$remapCount] = "Return to Blockland";
	   $RTB::addedCatSep = 1;
   }
   $remapName[$remapCount] = %name;
   $remapCmd[$remapCount] = %command;
   $remapCount++;
}

//- byteRound (rounds bytes to b,mb and kb)
function byteRound(%bytes)
{
	if(%bytes $= "")
		return "0b";

	if(%bytes > 1048576)
		%result = roundMegs(%bytes/1048576)@"MB";
	else if(%bytes > 1024)
		%result = mFloatLength(%bytes/1024,2)@"kB";
	else
		%result = %bytes@"b";
	return %result;
}

//- isInt (determines whether or not a string contains an integer)
function isInt(%string)
{
	%numbers = "0123456789";
	for(%i=0;%i<strLen(%string);%i++)
	{
		%char = getSubStr(%string,%i,1);
		if(strPos(%numbers,%char) $= -1)
			return 0;
	}
	return 1;
}

//- trimLeading (removes all leading spaces and html linebreaks)
function trimLeading(%string)
{
   for(%i=0;%i<strLen(%string);%i++)
   {
      if(getSubStr(%string,%i,1) $= " ")
         continue;
      else
         break;
   }
   %string = getSubStr(%string,%i,strLen(%string));
   
   for(%i=0;%i<strLen(%string);%i+=4)
   {
      if(getSubStr(%string,%i,4) $= "<br>")
         continue;
      else
         break;
   }
   return getSubStr(%string,%i,strLen(%string));
}

//- trimTrailing (removes all trailing spaces and html linebreaks)
function trimTrailing(%string)
{
   for(%i=strLen(%string)-1;%i>=0;%i--)
   {
      if(getSubStr(%string,%i,1) $= " ")
         continue;
      else
         break;
   }
   %string = getSubStr(%string,0,%i+1);
   
   for(%i=strLen(%string)-1;%i>=0&&(%i-4)>=0;%i-=4)
   {
      if(getSubStr(%string,%i-3,4) $= "<br>")
         continue;
      else
         break;
   }
   return getSubStr(%string,0,%i+1);
}

//- getRandomString (returns a random string of the specified %length)
function getRandomString(%length)
{
   %numeroalphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
   
   for(%i=0;%i<%length;%i++)
   {
      %string = %string@getSubStr(%numeroalphabet,getRandom(0,strlen(%numeroalphabet)-1),1);
   }
   return %string;
}

//- strTrim (shortcut to trim both trailing and leading)
function strTrim(%string)
{
   %string = trimLeading(%string);
   %string = trimTrailing(%string);
   return %string;
}

//- alphaCompare (determines alphabetic order of two strings)
function alphaCompare(%string1,%string2)
{
   %alphabet = "abcdefghijklmnopqrstuvwxyz";
   
   %longString = %string1;
   %shortString = %string2;
   %longNum = 1;
   %shortNum = 2;
   if(strLen(%string2) > strLen(%string1))
   {
      %longString = %string2;
      %shortString = %string1;
      %longNum = 2;
      %shortNum = 1;
   }
   
   %winString = 1;
   for(%i=0;%i<strLen(%longString);%i++)
   {
      if(%i<strLen(%shortString))
      {
         %longChar = strLwr(getSubStr(%longString,%i,1));
         %shortChar = strLwr(getSubStr(%shortString,%i,1));
         
         if(strPos(%alphabet,%longChar) < strPos(%alphabet,%shortChar))
         {
            %winString = %longNum;
            break;
         }
         else if(strPos(%alphabet,%longChar) > strPos(%alphabet,%shortChar))
         {
            %winString = %shortNum;
            break;
         }
      }
   }
   return %winString;
}

//- Anim_EaseInOut (Easing Animation)
function Anim_EaseInOut(%time,%begin,%change,%duration)
{
   if((%time/=%duration/2) < 1)
      return %change/2 * mPow(%time,3) + %begin;
   return %change/2 * (mPow(%time-2,3) + 2) + %begin;
}

//- sortFields (Probably like, the hackiest sorting method ever - I cant be bothered to write a proper one)
function sortFields(%fields)
{
   %fields = strReplace(%fields,",","\t");
   %list = new GuiTextListCtrl();
   for(%i=0;%i<getFieldCount(%fields);%i++)
   {
      %field = strReplace(getField(%fields,%i),"=>","\t");
      %list.addRow(getField(%field,0),getField(%field,1));
   }
   %list.sort(0);
   
   for(%i=0;%i<%list.rowCount();%i++)
   {
      %return = %return@%list.getRowId(%i)@"=>"@%list.getRowText(%i)@",";
   }
   %list.delete();
   
   if(strLen(%return) > 0)
      %return = getSubStr(%return,0,strLen(%return)-1);
      
   return %return;
}

//- getFileContents (returns the entire contents of a file in a string)
function getFileContents(%file)
{
   %IO = new FileObject();
   if(%IO.openForRead(%file))
   {
      while(!%IO.isEOF())
      {
         %return = (%return $= "") ? %IO.readLine() : %return TAB %IO.readLine();
      }
      %IO.delete();
      return %return;
   }
   else
      return 0;
}

//- filterKey (locates bl key-like strings)
function filterKey(%string)
{
   %string = strReplace(%string,"-","\t");
   %string = strReplace(%string," ","\t");
   
   %stageCheck = 0;
   for(%i=0;%i<getFieldCount(%string);%i++)
   {
      if(stringMatch(getField(%string,%i),"ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"))
      {
         if(%stageCheck $= 0)
         {
            if(strlen(getField(%string,%i)) $= 5)
            {
               %stageCheck++;
            }
         }
         else
         {
            if(strlen(getField(%string,%i)) $= 4)
            {
               %stageCheck++;
               
               if(%stageCheck > 3)
                  return 1;
            }
            else
            {
               %stageCheck = 0;
            }
         }
      }
      else
         %stageCheck = 0;
   }
   return 0;
}