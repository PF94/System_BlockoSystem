//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 112 $
//#      $Date: 2009-09-05 18:17:49 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/server.cs $
//#
//#      $Id: server.cs 112 2009-09-05 17:17:49Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Server Initiation
//#
//#############################################################################

//*********************************************************
//* Demo Users
//*********************************************************
if(!isUnlocked())
   return;
   
//*********************************************************
//* RTB Variables
//*********************************************************
$RTB::Version = "BLOCKO SYSTEM PRE-ALPHA";

//*********************************************************
//* Dedicated Handling
//*********************************************************
if($Server::Dedicated)
   exec("./dedicated.cs");

//*********************************************************
//* Load Modules
//*********************************************************
exec("./RTBS_Authentication.cs");
exec("./RTBS_ContentDownload.cs");
exec("./RTBS_GUITransfer.cs");
exec("./RTBS_ServerControl.cs");