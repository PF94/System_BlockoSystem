//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 108 $
//#      $Date: 2009-09-05 11:39:30 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/RTBC_Profiles.cs $
//#
//#      $Id: RTBC_Profiles.cs 108 2009-09-05 10:39:30Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Profiles
//#
//#############################################################################

new AudioProfile(Note12Sound: Note11Sound)
{
	filename = "./sounds/Synth4_12.wav";
};

if(!isObject(GuiCheckBoxBoldProfile)) new GuiControlProfile(GuiCheckBoxBoldProfile : GuiCheckBoxProfile)
{
   fontType = "Arial Bold";
};

if(!isObject(RTBMM_CellLightProfile)) new GuiControlProfile(RTBMM_CellLightProfile : GuiBitmapBorderProfile)
{
   bitmap = $RTB::Path@"images/ui/cellArray_light";
};

if(!isObject(RTBMM_CellYellowProfile)) new GuiControlProfile(RTBMM_CellYellowProfile : RTBMM_CellLightProfile)
{
   bitmap = $RTB::Path@"images/ui/cellArray_yellow";
};

if(!isObject(RTBMM_CellDarkProfile)) new GuiControlProfile(RTBMM_CellDarkProfile : RTBMM_CellLightProfile)
{
   bitmap = $RTB::Path@"images/ui/cellArray_dark";
};

if(!isObject(RTBMM_CheckBoxProfile)) new GuiControlProfile(RTBMM_CheckBoxProfile : GuiCheckBoxProfile)
{
   bitmap = $RTB::Path@"images/ui/rtbCheck";
};

if(!isObject(RTBMM_PopupProfile)) new GuiControlProfile(RTBMM_PopupProfile : GuiPopupMenuProfile)
{
   fillColor = "227 228 230 255";
   borderColor = "189 192 194 255";
   fontSize = 12;
   fontType = "Verdana";
   fontColor = "64 64 64 255";
   fontColors[0] = "64 64 64 255";
};

if(!isObject(RTBMM_TextEditProfile)) new GuiControlProfile(RTBMM_TextEditProfile : GuiTextEditProfile)
{
   fillColor = "255 255 255 255";
   borderColor = "188 191 193 255";
   fontSize = 12;
   fontType = "Verdana";
   fontColor = "64 64 64 255";
   fontColors[0] = "64 64 64 255";
};

if(!isObject(RTBMM_MLEditProfile)) new GuiControlProfile(RTBMM_MLEditProfile : GuiMLTextEditProfile)
{
   fontSize = 12;
   fontType = "Verdana";
   fontColor = "64 64 64 255";
   fontColors[0] = "64 64 64 255";
};

if(!isObject(RTBMM_ScrollProfile)) new GuiControlProfile(RTBMM_ScrollProfile)
{
   fontType = "Book Antiqua";
   fontSize = 22;
   justify = center;
   fontColor = "0 0 0";
   fontColorHL = "130 130 130";
   fontColorNA = "255 0 0";
   fontColors[0] = "0 0 0";
   fontColors[1] = "0 255 0";  
   fontColors[2] = "0 0 255"; 
   fontColors[3] = "255 255 0";
   hasBitmapArray = true;
   
   bitmap = "./images/ui/scrollArray";
};

if(!isObject(RTBIT_TipProfile)) new GuiControlProfile (RTBIT_TipProfile)
{
	doFontOutline = 1;
	fontType = "Verdana";
	fontSize = 13;
	fontOutlineColor = "0 0 0 3";
};

if(!isObject(RTBIC_VectorProfile)) new GuiControlProfile(RTBIC_VectorProfile)
{
	fontType = "Verdana";
	fontSize = 12;
	fontColors[0] = "0 0 0";         // Black
	fontColors[1] = "50 50 255";     // Baby Blue (??)
	fontColors[2] = "255 0 0";       // Red
	fontColors[3] = "50 150 50";     // Green
	fontColors[4] = "150 50 150";    // Purple
	fontColors[5] = "255 100 50";    // Orange
	fontColors[6] = "100 100 100";   // Gray
	fontColors[7] = "0 0 0";         // Not used
	fontColors[8] = "0 0 0";         // Not used
	fontColors[9] = "0 0 0";         // Not used
};

if(!isObject(RTBIC_MessageProfile)) new GuiControlProfile(RTBIC_MessageProfile : GuiTextEditProfile)
{
   fontType = "Verdana";
   fontSize = 12;
};

if(!isObject(RTBMM_PaginationProfile)) new GuiControlProfile(RTBMM_PaginationProfile)
{
   fontColor = "230 230 230 255";
   fontType = "Verdana Bold";
   fontSize = "12";
   justify = "Center";
   fontColors[1] = "230 230 230";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0"; 
   fontColorLink = "230 230 230 255";
   fontColorLinkHL = "255 255 255 255";
};

if(!isObject(RTBMM_NewsContentProfile)) new GuiControlProfile(RTBMM_NewsContentProfile)
{
   fontColor = "230 230 230 255";
   fontType = "Verdana Bold";
   fontSize = "12";
   justify = "Center";
   fontColors[1] = "230 230 230";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0"; 
   fontColorLink = "150 150 150 255";
   fontColorLinkHL = "200 200 200 255";
};

if(!isObject(RTBMA_ContentsProfile)) new GuiControlProfile(RTBMA_ContentsProfile)
{
   fontColor = "30 30 30 255";
   fontType = "Palatino Linotype";
   fontSize = "18";
   justify = "Left";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0"; 
   fontColorLink = "60 60 60 255";
   fontColorLinkHL = "0 0 0 255";
};

if(!isObject(RTBIC_ListProfile)) new GuiControlProfile(RTBIC_ListProfile)
{
	fontColor = "30 30 30 255";
	fontSize = 14;
	fontType = "Arial";
	justify = "Left";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";   
   fontColorLink = "60 60 60 255";
   fontColorLinkHL = "0 0 0 255";
};

if(!isObject(RTBMM_MainText)) new GuiControlProfile(RTBMM_MainText)
{
	fontColor = "30 30 30 255";
	fontSize = 18;
	fontType = "Impact";
	justify = "Left";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";   
};

if(!isObject(RTBMM_BlockText)) new GuiControlProfile(RTBMM_BlockText:BlockButtonProfile)
{
   fontColors[1] = "100 100 100";
	justify = "Left"; 
};

if(!isObject(RTBMM_GenText)) new GuiControlProfile(RTBMM_GenText)
{
	fontColor = "30 30 30 255";
	fontSize = 14;
	fontType = "Arial";
	justify = "Left";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";  
};

if(!isObject(RTBMM_FieldText)) new GuiControlProfile(RTBMM_FieldText)
{
	fontColor = "30 30 30 255";
	fontSize = 12;
	fontType = "Verdana Bold";
	justify = "Left";
   fontColors[1] = "150 150 150";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";   
};

if(!isObject(RTBMM_ProgressBar)) new GuiControlProfile(RTBMM_ProgressBar)
{
   fillColor = "0 200 0 100";
   border = 0; 
};

if(!isObject(RTMBS_MainMiddleText)) new GuiControlProfile(RTBMM_MainMiddleText)
{
	fontColor = "30 30 30 255";
	fontSize = 18;
	fontType = "Impact";
	justify = "center";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";   
};

if(!isObject(RTBMM_MiddleText)) new GuiControlProfile(RTBMM_MiddleText)
{
	fontColor = "30 30 30 255";
	fontSize = 14;
	fontType = "Arial";
	justify = "center";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";  
};

if(!isObject(RTB_Verdana12Pt)) new GuiControlProfile(RTB_Verdana12Pt)
{
	fontColor = "30 30 30 255";
	fontSize = 12;
	fontType = "Verdana";
	justify = "Left";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";   
   fontColorLink = "60 60 60 255";
   fontColorLinkHL = "0 0 0 255";
};

if(!isObject(RTB_Verdana12PtAuto)) new GuiControlProfile(RTB_Verdana12PtAuto)
{
	fontColor = "30 30 30 255";
	fontSize = 12;
	fontType = "Verdana";
	justify = "Left";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";
   autoSizeWidth = true;
   autoSizeHeight = true; 
};

if(!isObject(RTB_Impact17PtAuto)) new GuiControlProfile(RTB_Impact17PtAuto)
{
	fontColor = "68 68 68 255";
	fontSize = 17;
	fontType = "Impact";
	justify = "Left";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";
   autoSizeWidth = true;
};

if(!isObject(RTB_Verdana12PtCenter)) new GuiControlProfile(RTB_Verdana12PtCenter)
{
	fontColor = "30 30 30 255";
	fontSize = 12;
	fontType = "Verdana";
	justify = "Center";
   fontColors[1] = "100 100 100";
   fontColors[2] = "0 255 0";  
   fontColors[3] = "0 0 255"; 
   fontColors[4] = "255 255 0";   
};

if(!isObject(RTB_VersionProfile)) new GuiControlProfile(RTB_VersionProfile)
{
   tab = "0";
   canKeyFocus = "0";
   mouseOverSelected = "0";
   modal = "1";
   opaque = "0";
   fillColor = "200 200 200 255";
   fillColorHL = "200 200 200 255";
   fillColorNA = "200 200 200 255";
   border = "0";
   borderThickness = "1";
   borderColor = "0 0 0 255";
   borderColorHL = "128 128 128 255";
   borderColorNA = "64 64 64 255";
   fontType = "Arial";
   fontSize = "14";
   fontColors[0] = "255 255 255 255";
   fontColors[1] = "32 100 100 255";
   fontColors[2] = "0 0 0 255";
   fontColors[3] = "200 200 200 255";
   fontColors[4] = "0 0 204 255";
   fontColors[5] = "85 26 139 255";
   fontColors[6] = "0 0 0 0";
   fontColors[7] = "16 213 75 2";
   fontColors[8] = "0 0 0 0";
   fontColors[9] = "0 0 0 0";
   fontColor = "255 255 255 255";
   fontColorHL = "32 100 100 255";
   fontColorNA = "0 0 0 255";
   fontColorSEL = "200 200 200 255";
   fontColorLink = "0 0 204 255";
   fontColorLinkHL = "85 26 139 255";
   doFontOutline = "1";
   fontOutlineColor = "255 24 24 255";
   justify = "right";
   textOffset = "0 0";
   autoSizeWidth = "0";
   autoSizeHeight = "0";
   returnTab = "0";
   numbersOnly = "0";
   cursorColor = "0 0 0 255";
};