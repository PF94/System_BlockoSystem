//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 110 $
//#      $Date: 2009-09-05 11:48:12 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/RTBC_Authentication.cs $
//#
//#      $Id: RTBC_Authentication.cs 110 2009-09-05 10:48:12Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Client Authentication (RTBCA/CAuthentication)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBC_Authentication = 1;

//*********************************************************
//* Load Switchboard
//*********************************************************
%RTBCA_SB = RTB_createSwitchboard("CA","APICA");
%RTBCA_SB.registerLine(1,1);
%RTBCA_SB.registerLine(2,1);

//*********************************************************
//* Request Gateway
//*********************************************************
//- RTBCA_SendRequest (Compiles arguments into POST string for transfer)
function RTBCA_SendRequest(%cmd,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6,%arg7,%arg8,%arg9,%arg10)
{
   for(%i=1;%i<11;%i++)
   {
      %string = %string@strReplace(urlEnc(%arg[%i]),"\t","")@"\t";
   }
   RTB_SB_CA.placeCall(%line,%cmd,%string);
}

//*********************************************************
//* Meat
//*********************************************************
//- RTBCA_Post (Updates RTB API with player's details)
function RTBCA_Post()
{
   if(!$RTB::Options::CA::AuthWithRTB)
	   return;

   %name = $pref::Player::NetName;
   
   if(isEventPending($RTB::CAuthentication::Auth))
      cancel($RTB::CAuthentication::Auth);
      
   $RTB::CAuthentication::Auth = schedule(180000,0,"RTBCA_Post");
   RTBCA_SendRequest("AUTH",1,RTBCA_EstablishLocation(),RTBCA_GetPlaytime(),$RTB::Version,$Version);
}

//- RTBCA_onAuthResponse (Handles any special requests from the API)
%RTBCA_SB.registerResponseHandler("AUTH","RTBCA_onAuthResponse");
function RTBCA_onAuthResponse(%this,%line)
{
   if(getField(%line,0) $= "WELCOME")
   {
      //Nothing
   }
   else if(%line $= "PREFS")
      RTBCA_SendPrefs();
}

//- RTBCA_onPushUpdate (Check for updates if the API commands it)
%RTBCA_SB.registerResponseHandler("PUSHUPDATE","RTBCA_onPushUpdate");
function RTBCA_onPushUpdate()
{
   RTBCU_Update();
}

//- RTBCA_SendPrefs (Updates player's RTB profile with privacy settings)
function RTBCA_SendPrefs()
{
   %name = $pref::Player::NetName;
   RTBCA_SendRequest("UPDATEPREFS",2,$RTB::Options::CA::Privacy::ShowOnline,$RTB::Options::CA::Privacy::ShowServer,$RTB::Options::SA::Privacy::ShowPlayers);
}

//*********************************************************
//* Support Functions
//*********************************************************
//- RTBCA_GetPlaytime (Gets a nice formatted string of how long the user has had the game open)
function RTBCA_GetPlaytime()
{
   %timestring = getTimeString($Sim::Time);
   return %timestring;
}

//- RTBCA_EstablishLocation (Decides where the player is currently)
function RTBCA_EstablishLocation()
{
   if(!$RTB::Options::CA::Privacy::ShowServer)
      return "UNK";
      
   if(isObject(ServerConnection))
   {
      %address = ServerConnection.getAddress();
      if(%address $= "local")
      {
         if($Server::LAN)
            if($Server::ServerType $= "SinglePlayer")
               %address = "SNG";
            else
               %address = "LAN";
         else
            %address = "LOC "@$Pref::Server::Port;
      }
      else
      {
         %address = getSubStr(%address,3,strLen(%address));
         %address = strReplace(%address,":"," ");  
         %ip = getWord(%address,0);
         %port = getWord(%address,1);
         
         if(strPos(%ip,"192.") $= 0 || strPos(%ip,"10.") $= 0)
            %address = "NWK "@%ip@":"@%port;
         else
            %address = "WEB "@%ip@":"@%port;
      }
      %location = %address;
   }
   else
      %location = "NOS";
      
   return %location;
}

//*********************************************************
//* Packaged Functions
//*********************************************************
package RTBC_Authentication
{
   function MM_AuthBar::blinkSuccess(%this)
   {
      Parent::blinkSuccess(%this);
      RTBCA_Post();
   }
   
   function disconnectedCleanup()
   {
      Parent::disconnectedCleanup();
      schedule(1000,0,"RTBCA_Post");
   }
   
   function GameConnection::onConnectionAccepted(%this,%a,%b,%c,%d,%e,%f,%g,%h,%i,%j,%k)
   {
      Parent::onConnectionAccepted(%this,%a,%b,%c,%d,%e,%f,%g,%h,%i,%j,%k);
      RTBCA_Post();
   }
};
activatePackage(RTBC_Authentication);