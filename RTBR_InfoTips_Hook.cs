//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 39 $
//#      $Date: 2009-02-23 10:45:55 +0000 (Mon, 23 Feb 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/old/RTBR_InfoTips_Hook.cs $
//#
//#      $Id: RTBR_InfoTips_Hook.cs 39 2009-02-23 10:45:55Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Info Tips Hook (RTBIT/RInfoTipsHook)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBR_InfoTips_Hook = 1;

//*********************************************************
//* The Meat
//*********************************************************
//- RTB_addInfoTip (allows add-ons to add their own info tips)
function RTB_addInfoTip(%tip,%nobindtip,%category)
{
   if(%tip $= "")
   {
      echo("\c2ERROR: No tip specified in RTB_addInfoTip");
      return 0;
   }
   
   //category is deprecated for ???
   
   $RTB::InfoTip[$RTB::InfoTips++] = %tip TAB %nobindtip;
   
   return 1;
}