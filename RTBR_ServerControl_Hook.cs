//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 39 $
//#      $Date: 2009-02-23 10:45:55 +0000 (Mon, 23 Feb 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/old/RTBR_ServerControl_Hook.cs $
//#
//#      $Id: RTBR_ServerControl_Hook.cs 39 2009-02-23 10:45:55Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Server Control Hook (RTBSC/RServerControlHook)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBR_ServerControl_Hook = 1;

//*********************************************************
//* Variable Declarations
//*********************************************************
$RTB::SServerControl::SP::Cats = 0;
$RTB::SServerControl::SP::Prefs = 0;

//*********************************************************
//* Requirements
//*********************************************************
if(isFile("config/server/rtb/modPrefs.cs"))
   exec("config/server/rtb/modPrefs.cs");

//*********************************************************
//* The Meat
//*********************************************************
//- RTB_registerPref (Registers a pref to be sent to clients)
function RTB_registerPref(%name,%cat,%pref,%vartype,%mod,%default,%requiresRestart,%hostOnly,%callback)
{
   %pref = strReplace(%pref,"$","");
   
   if(%name $= "")
   {
      echo("\c2ERROR: No user-friendly name for pref supplied in RTB_registerPref");
      return 0;
   }
   else if(%pref $= "")
   {
      echo("\c2ERROR: No pref value supplied in RTB_registerPref");
      return 0;
   }
   else if(%vartype $= "")
   {
      echo("\c2ERROR: No pref variable type supplied in RTB_registerPref");
      return 0;
   }
      
   if(%requiresRestart !$= 1)
      %requiresRestart = 0;
      
   if(%hostOnly !$= 1)
      %hostOnly = 0;
   
   if(%mod $= "")
      %mod = "Unknown";
   
   for(%i=0;%i<$RTB::SServerControl::SP::Prefs;%i++)
   {
      %checkpref = getField($RTB::SServerControl::SP::Pref[%i],1);
      if(%pref $= %checkpref)
      {
         echo("\c2ERROR: $"@%pref@" pref has already been registered to add-on: "@getField($RTB::SServerControl::SP::Pref[%i],4)@" in RTB_registerPref");
         return 0;
      }
   }
   
   if(%cat $= "")
      %cat = "Misc.";
   
   %pType = firstWord(%vartype);
   if(%pType $= "int")
   {
      if(getWordCount(%vartype) $= 3)
      {
         %max = getWord(%vartype,2);
         if(%max <= %min)
         {
            echo("\c2ERROR: Integer max value supplied for pref ("@%pref@") is less than or equal to min value in RTB_registerPref");
            return 0;
         }
      }
      else
      {
         echo("\c2ERROR: Integer variable type expects 2 parameters in RTB_registerPref");
         return 0;
      }
   }
   else if(%pType $= "string")
   {
      %length = getWord(%vartype,1);
      if(%length <= 0)
      {
         echo("\c2ERROR: Invalid string length supplied for pref ("@%pref@") in RTB_registerPref");
         return 0;
      }
   }
   else if(%pType $= "list")
   {
      if(getWordCount(%vartype)%2 $= 0)
      {
         echo("\c2ERROR: Invalid list values supplied for pref ("@%pref@") in RTB_registerPref");
         return 0;
      }
   }
   //else if(%pType $= "float")
   //{
      //if(getWordCount(%vartype) !$= 4)
      //{
         //echo("\c2ERROR: Invalid float values supplied for pref ("@%pref@") in RTB_registerPref");
         //return 0;
      //}
   //}
   else if(%pType $= "bool")
   {
   }
   else
   {
      echo("\c2ERROR: Invalid pref type supplied ("@%pType@") for pref "@%pref@" in RTB_registerPref");
      return 0;
   }
   
   eval("%currVal = $"@%pref@";");
   if(%currVal $= "")
      eval("$"@%pref@" = \""@%default@"\";");
   
   $RTB::SServerControl::SP::Pref[$RTB::SServerControl::SP::Prefs] = %name TAB %pref TAB %cat TAB %vartype TAB %mod TAB %requiresRestart TAB %hostOnly TAB %callback;
   $RTB::SServerControl::SP::PrefDefault[$RTB::SServerControl::SP::Prefs] = %default;
   $RTB::SServerControl::SP::Prefs++;
   return 1;
}