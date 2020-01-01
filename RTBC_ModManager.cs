//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 108 $
//#      $Date: 2009-09-05 11:39:30 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/RTBC_ModManager.cs $
//#
//#      $Id: RTBC_ModManager.cs 108 2009-09-05 10:39:30Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Mod Manager (RTBMM/CModManager)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBC_ModManager = 1;

//#####################################################################################################
//
//     _____   _    _   _____   
//    / ____| | |  | | |_   _|  
//   | |  __  | |  | |   | |    
//   | | |_ | | |  | |   | |    
//   | |__| |_| |__| |_ _| |_ _ 
//    \_____(_)\____/(_)_____(_)
//
//
//##################################################################################################### 

//*********************************************************
//* GUI Modification
//*********************************************************
if(!isObject(MM_RTBModManagerButton))
{
   %btn = new GuiBitmapButtonCtrl(MM_RTBModManagerButton)
   {
      profile = "GuiDefaultProfile";
      horizSizing = "relative";
      vertSizing = "relative";
      position = "416 120";
      extent = "224 40";
      minExtent = "8 2";
      visible = "1";
      text = " ";
      groupNum = "-1";
      buttonType = "PushButton";
      bitmap = $RTB::Path@"images/buttons/menu/btnMODManager";
      command = "canvas.pushdialog(RTB_ModManager);";
      lockAspectRatio = "1";
      alignLeft = "1";
      overflowImage = "0";
      mKeepCached = "1";
      mColor = "255 255 255 255";
   };
   MainMenuGui.add(%btn);
}
function MM_RTBModManagerButton::onMouseEnter(%this)
{
   if($Pref::Audio::MenuSounds)
	   alxPlay(Note8Sound);
}

//*********************************************************
//* Variable Declarations
//*********************************************************
$RTB::CModManager::Style::ColorDARKRED = "8B0000";
$RTB::CModManager::Style::ColorRED = "FF0000";
$RTB::CModManager::Style::ColorORANGE = "FFA500";
$RTB::CModManager::Style::ColorBROWN = "A52A2A";
$RTB::CModManager::Style::ColorYELLOW = "FFFF00";
$RTB::CModManager::Style::ColorGREEN = "008000";
$RTB::CModManager::Style::ColorOLIVE = "808000";
$RTB::CModManager::Style::ColorCYAN = "00FFFF";
$RTB::CModManager::Style::ColorBLUE = "0000FF";
$RTB::CModManager::Style::ColorDARKBLUE = "00008B";
$RTB::CModManager::Style::ColorINDIGO = "4B0082";
$RTB::CModManager::Style::ColorVIOLET = "EE82EE";
$RTB::CModManager::Style::ColorWHITE = "FFFFFF";
$RTB::CModManager::Style::ColorBLACK = "000000";

//*********************************************************
//* Initialisation of Required Elements
//*********************************************************
if(!isObject(RTB_ModManager))
	exec("./RTB_ModManager.gui");
if(!isObject(RTB_ModUpdates))
	exec("./RTB_ModUpdates.gui");

//*********************************************************
//* Load TCP Object
//*********************************************************
%RTBMM_SB = RTB_createSwitchboard("MM","APIMS");
%RTBMM_SB.registerResponseHandler("DEFAULT","RTBMM_Default_onReply");
%RTBMM_SB.registerFailHandler("DEFAULT","RTBMM_Default_onFail");
%RTBMM_SB.registerLine(1,0);
%RTBMM_SB.registerLine(2,2);
%RTBMM_SB.registerLine(3,3);
%RTBMM_SB.setLineProperty(3,"Keep-Alive",1);
$RTB::CModManager::Switchboard = %RTBMM_SB;

//*********************************************************
//* Load TCP Object
//*********************************************************
//- RTBMM_SendRequest (Compiles arguments into POST string for transfer)
function RTBMM_SendRequest(%cmd,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6,%arg7,%arg8,%arg9,%arg10)
{
   if($RTB::Barred::Download)
   {
      RTBMM_setBarred();
      return;
   }
   
   if(%line $= 1)
      deleteVariables("$RTB::CModManager::Cache*");
      
   for(%i=1;%i<11;%i++)
   {
      %arg[%i] = strReplace(%arg[%i],"\n","<br>");
      %string = %string@strReplace(urlEnc(%arg[%i]),"\t","")@"\t";
   }
   RTB_SB_MM.placeCall(%line,%cmd,%string);
}

//*********************************************************
//* Special Controllers
//*********************************************************
//- RTBMM_setBarred (Decides whether you are barred from using the mod manager or not)
function RTBMM_setBarred()
{
   if($RTB::Barred::Download)
   {
      if(isObject(RTBMM_WindowSwatch))
         RTBMM_WindowSwatch.delete();
         
      if(isObject(RTBMM_LoadingContentSwatch))
         RTBMM_LoadingContentSwatch.delete();
         
      RTBMM_BarredReason.setText("<just:center><font:Verdana Bold:18><color:AAAAAA>"@$RTB::Barred::Reason);
      RTBMM_BarredOverlay.setVisible(1);
   }
   else
   {
      RTBMM_BarredOverlay.setVisible(0);
   }
}

//*********************************************************
//* GUI Callbacks
//*********************************************************
//- RTB_ModManager::onWake (GUI wake callback, updates browser buttons + auth)
function RTB_ModManager::onWake()
{
   RTBMM_Zones_CheckButtons();
   RTBMM_Auth_Init();
   
   if(!$RTB::CModManager::Cache::OpenDirect)
      RTBMM_Zones_Refresh();
      
   if($RTB::CModManager::Cache::CurrentZone $= "")
      RTBMM_NewsFeedView_Init();
   
   if($AddOn__Weapon_Gun $= "")
      clientUpdateAddonsList();   
   
   if(!RTBMM_FileCache.refreshed)
      RTBMM_FileCache.refresh();
   
   if(!RTBMM_GroupManager.loaded)
      RTBMM_GroupManager.loadDat();
}

//- RTB_ModManager::onSleep (GUI sleep callback, refresh addons gui)
function RTB_ModManager::onSleep()
{
   if(AddOnsGui.isAwake())
   {
      Canvas.popDialog(AddOnsGui);
      Canvas.pushDialog(AddOnsGui);
   }
}

//*********************************************************
//* Forward/Backward Tracking
//*********************************************************
//- RTBMM_Zones_Track (Tracks current zone and return eval)
function RTBMM_Zones_Track(%zone,%return,%pagination)
{
   if(!isObject(RTBMM_ZoneTracker))
   {
      new ScriptObject(RTBMM_ZoneTracker)
      {
         currZone = -1;
         numZones = 0;
      };
   }
   
   //I have run out of milkshake so thats why the tracking queue will have to
   //be of an indeterminate length... can't work without milkshake
   if(RTBMM_ZoneTracker.zoneCmd[RTBMM_ZoneTracker.currZone] !$= %return)
   {
      RTBMM_ZoneTracker.currZone++;
      RTBMM_ZoneTracker.zone[RTBMM_ZoneTracker.currZone] = %zone;
      RTBMM_ZoneTracker.zoneCmd[RTBMM_ZoneTracker.currZone] = %return;
      RTBMM_ZoneTracker.numZones = RTBMM_ZoneTracker.currZone+1;
   }
   $RTB::CModManager::Cache::CurrentZone = %zone;
   $RTB::CModManager::Cache::PaginationTemplate = %pagination;
   
   RTBMM_Zones_CheckButtons();
}

//- RTBMM_Zones_Back (Moves back in history)
function RTBMM_Zones_Back()
{
   if(RTBMM_ZoneTracker.currZone >= 1)
   {
      RTBMM_ZoneTracker.currZone--;
      %return = RTBMM_ZoneTracker.zoneCmd[RTBMM_ZoneTracker.currZone];
      eval(%return);
   }
   RTBMM_Zones_CheckButtons();
}

//- RTBMM_Zones_Forward (Moves forward in history)
function RTBMM_Zones_Forward()
{
   if(RTBMM_ZoneTracker.currZone < (RTBMM_ZoneTracker.numZones-1))
   {
      RTBMM_ZoneTracker.currZone++;
      %return = RTBMM_ZoneTracker.zoneCmd[RTBMM_ZoneTracker.currZone];
      eval(%return);
   }
   RTBMM_Zones_CheckButtons();
}

//- RTBMM_Zones_Refresh (Refreshes current page)
function RTBMM_Zones_Refresh()
{
   if(RTBMM_ZoneTracker.numZones >= 1)
   {
      %return = RTBMM_ZoneTracker.zoneCmd[RTBMM_ZoneTracker.currZone];
      eval(%return);
   }
}

//- RTBMM_Zones_CheckButtons (checks to see if buttons should be active)
function RTBMM_Zones_CheckButtons()
{
   RTBMM_HistoryBack.setActive(0);
   RTBMM_HistoryForward.setActive(0);
   RTBMM_Refresh.setActive(1);
   
   if(RTBMM_ZoneTracker.numZones > 1)
   {
      if(RTBMM_ZoneTracker.currZone < (RTBMM_ZoneTracker.numZones-1))
         RTBMM_HistoryForward.setActive(1);
         
      if(RTBMM_ZoneTracker.currZone >= 1)
         RTBMM_HistoryBack.setActive(1);
   }
   
   if(RTBMM_ZoneTracker.currZone $= "")
      RTBMM_Refresh.setActive(0);
}

//*********************************************************
//* Package
//*********************************************************
package RTBC_ModManager
{
   function clientUpdateAddonsList()
   {
      if($AddOn__Weapon_Gun $= "")
         Parent::clientUpdateAddonsList();
   }
   
   function GuiMLTextCtrl::onUrl(%this,%url)
   {
      %explode = strReplace(%url,"-","\t");
      if(getField(%explode,0) $= "pagination")
      {
         %eval = $RTB::CModManager::Cache::PaginationTemplate;
         %pageNumber = getField(%explode,1);
         %eval = strReplace(%eval,"%%page%%",%pageNumber);
         
         schedule(10,0,"eval",%eval);
      }
      else if(getField(%explode,0) $= "comment")
      {
         if(getField(%explode,1) $= "pagination")
         {
            if(getField(%explode,2) $= "next")
               RTBMM_FileView_getCommentPage($RTB::CModManager::Cache::CommentCurrPage-1);
            else
               RTBMM_FileView_getCommentPage($RTB::CModManager::Cache::CommentCurrPage+1);
         }
      }
      else if(getField(%explode,0) $= "download")
      {
         RTBMM_TransferView_Add(getField(%explode,1));
      }
      else if(getField(%explode,0) $= "file")
      {
         if(!RTB_ModManager.isAwake())
            RTBMM_OpenModManager();
         RTBMM_FileView_Init(getField(%explode,1));
      }
      else if(getField(%explode,0) $= "pack")
      {
         if(!RTB_ModManager.isAwake())
            RTBMM_OpenModManager();
         RTBMM_PackView_Init(getField(%explode,1));
      }
      else if(getField(%explode,0) $= "rtb")
      {
         if(!RTB_ModManager.isAwake())
            RTBMM_OpenModManager();
         schedule(10,0,"RTBMM_FileView_Init",getField(%explode,1));
      }
      else
         Parent::onUrl(%this,%url);
   }
};
activatePackage(RTBC_ModManager);

//*********************************************************
//* Mouse Event Handling
//* -------------------------------------------------------
//* This setup allows mouseEventCtrls to contain other
//* controls within itself that you can mouseover without
//* the engine thinking you left the mouse event ctrl.
//* Its just a looping check to see if you're within the
//* bounds - and performance it'll reduce the number of
//* checks if you've left the mouse afk in the ctrl or
//* something. Shouldn't cause trouble really.
//*********************************************************
//- GuiMouseEventCtrl::onMouseEnter (Hack)
function GuiMouseEventCtrl::onMouseEnter(%this)
{   
   if(%this.eventType !$= "")
   {
      if(strLen(%this.eventCallbacks) == 4)
      {
         if(getSubStr(%this.eventCallbacks,0,1) $= 1)
         {
            %command = "Event_"@%this.eventType@"::onMouseEnter("@%this@");";
            eval(%command);
            %isSpecial = 1;
         }
      }
   }
   
   if(!%isSpecial)
   {
      if(isFunction(Parent,"onMouseEnter"))
      {
         Parent::onMouseEnter(Parent);
      }
   }
}
//- GuiMouseEventCtrl::onMouseLeave (Epic Hack, So bad it makes me sick)
function GuiMouseEventCtrl::onMouseLeave(%this)
{
   if(%this.persistent)
   {
      %position = %this.getCanvasPosition();
      %minX = getWord(%position,0);
      %maxX = getWord(%position,0) + getWord(%this.extent,0);
      %minY = getWord(%position,1);
      %maxY = getWord(%position,1) + getWord(%this.extent,1);
      %curX = getWord(Canvas.getCursorPos(),0);   
      %curY = getWord(Canvas.getCursorPos(),1);

      if(%curX > %minX && %curX < %maxX && %curY > %minY && %curY < %maxY)
      {
         if(%this.checks > 1000)
            %sch = 1000;
         else if(%this.checks > 500)
            %sch = 100;
         else
            %sch = 10;

         %this.schedule(%sch,"onMouseLeave");
         %this.checks++;
         return;
      }
   }
   
   if(%this.eventType !$= "")
   {
      if(strLen(%this.eventCallbacks) == 4)
      {
         if(getSubStr(%this.eventCallbacks,1,1) $= 1)
         {
            %command = "Event_"@%this.eventType@"::onMouseLeave("@%this@");";
            eval(%command);
            %isSpecial = 1;
            %this.checks = 0;
         }
      }
   }
   
   if(!%isSpecial)
   {
      if(isFunction(Parent,"onMouseLeave"))
      {
         Parent::onMouseLeave(Parent);
      }
   }
}
//- GuiMouseEventCtrl::onMouseDown (Hack)
function GuiMouseEventCtrl::onMouseDown(%this)
{
   if(%this.eventType !$= "")
   {
      if(strLen(%this.eventCallbacks) == 4)
      {
         if(getSubStr(%this.eventCallbacks,2,1) $= 1)
         {
            %command = "Event_"@%this.eventType@"::onMouseDown("@%this@");";
            eval(%command);
            %isSpecial = 1;
         }
      }
   }
   
   if(!%isSpecial)
   {
      if(isFunction(Parent,"onMouseDown"))
      {
         Parent::onMouseDown(Parent);
      }
   }
}
//- GuiMouseEventCtrl::onMouseUp (Hack)
function GuiMouseEventCtrl::onMouseUp(%this)
{
   if(%this.eventType !$= "")
   {
      if(strLen(%this.eventCallbacks) == 4)
      {
         if(getSubStr(%this.eventCallbacks,3,1) $= 1)
         {
            %command = "Event_"@%this.eventType@"::onMouseUp("@%this@");";
            eval(%command);
            %isSpecial = 1;
         }
      }
   }
   
   if(!%isSpecial)
   {
      if(isFunction(Parent,"onMouseUp"))
      {
         Parent::onMouseUp(Parent);
      }
   }
}

//*********************************************************
//* Default Communication Handling
//*********************************************************
//- RTBMM_Default_onConnected (onConnected Callback)
function RTBMM_Default_onConnected(%this)
{
}

//- RTBMM_Default_onReply (onReply Callback)
function RTBMM_Default_onReply(%this,%line)
{
}

//- RTBMM_Default_onDisconnected (onDisconnected Callback)
function RTBMM_Default_onDisconnected(%this)
{
}

//- RTBMM_Default_onFail (onFail Callback)
function RTBMM_Default_onFail(%this,%errorCode)
{
}

//*********************************************************
//* GUI Generation Management
//*********************************************************
//- RTBMM_GUI_Init (Creates intial gui for building into)
function RTBMM_GUI_Init()
{
   if(isObject(RTBMM_LoadingContentSwatch))
      RTBMM_LoadingContentSwatch.delete();
   
   $RTB::CModManager::GUI::CurrentY = 0;
   
   if(isObject(RTBMM_WindowSwatch))
      RTBMM_WindowSwatch.delete();
      
   RTBMM_WindowOverlay.clear();
   RTBMM_WindowOverlay.setVisible(0);
      
   %swatch = new GuiSwatchCtrl(RTBMM_WindowSwatch)
   {
      position = "1 1";
      extent = "679 555";
      color = "255 255 255 255";
   };
   RTBMM_ModWindow.add(%swatch);
}

//- RTBMM_GUI_PushControl (Adds control to the swatch)
function RTBMM_GUI_PushControl(%ctrl,%recalc)
{
   if(!isObject(RTBMM_WindowSwatch))
      RTBMM_GUI_Init();
      
   RTBMM_WindowSwatch.add(%ctrl);
   
   if(%recalc)
   {
      %totalOffset = getWord(%ctrl.position,1) + getWord(%ctrl.extent,1);
      $RTB::CModManager::GUI::CurrentY = %totalOffset;
      RTBMM_GUI_Resize();
   }
}

//- RTBMM_GUI_Resize (Resizes swatch based on Y variable)
function RTBMM_GUI_Resize()
{
	%PosX = getWord(RTBMM_WindowSwatch.position,0);
	%PosY = getWord(RTBMM_WindowSwatch.position,1);
	%ExtX = "679";
   %ExtY = 555;
   
   if($RTB::CModManager::GUI::CurrentY > %ExtY)
      %ExtY = $RTB::CModManager::GUI::CurrentY-1;
      
   RTBMM_WindowSwatch.resize(%PosX,%PosY,%ExtX,%ExtY);
}

//- RTBMM_GUI_AutoResize (Resizes swatch to fit gui inside)
function RTBMM_GUI_AutoResize()
{
	%PosX = getWord(RTBMM_WindowSwatch.position,0);
	%PosY = getWord(RTBMM_WindowSwatch.position,1);
	%ExtX = "679";
   %ExtY = 555;
   
   for(%i=0;%i<RTBMM_WindowSwatch.getCount();%i++)
   {
      %ctrl = RTBMM_WindowSwatch.getObject(%i);
      %extent = getWord(%ctrl.position,1) + getWord(%ctrl.extent,1);
      if(%extent > %ExtY)
         %ExtY = %extent-1;
   }
	RTBMM_WindowSwatch.resize(%PosX,%PosY,%ExtX,%ExtY);
	
	%ExtY = 1;
   for(%i=0;%i<RTBMM_WindowSwatch.getCount();%i++)
   {
      %ctrl = RTBMM_WindowSwatch.getObject(%i);
      %extent = getWord(%ctrl.position,1) + getWord(%ctrl.extent,1);
      if(%extent > %ExtY)
         %ExtY = %extent;
   }	
	$RTB::CModManager::GUI::CurrentY = %ExtY;
}

//- RTBMM_GUI_Offset (Offsets GUI in either direction and updates controls)
function RTBMM_GUI_Offset(%offset)
{
   $RTB::CModManager::GUI::CurrentY += %offset;
   RTBMM_GUI_Resize();
}

//- RTBMM_GUI_FadeIn (Fades a swatch in)
function RTBMM_GUI_FadeIn(%ctrl)
{
   if(!isObject(%ctrl))
      return;
      
   %color = getWords(%ctrl.color,0,2);
   %alpha = getWord(%ctrl.color,3);
   %alpha += 20;
   if(%alpha > 255)
      %alpha = 255;
   %ctrl.color = %color SPC %alpha;
   
   if(%alpha < 255)
      schedule(50,0,"RTBMM_GUI_FadeIn",%ctrl);
}

//- RTBMM_GUI_FadeOut (Fades out a swatch)
function RTBMM_GUI_FadeOut(%ctrl)
{
   if(!isObject(%ctrl))
      return;
      
   %color = getWords(%ctrl.color,0,2);
   %alpha = getWord(%ctrl.color,3);
   %alpha -= 20;
   if(%alpha < 0)
      %alpha = 0;
   %ctrl.color = %color SPC %alpha;
   
   if(%alpha > 0)
      schedule(50,0,"RTBMM_GUI_FadeOut",%ctrl);
}

//- RTBMM_GUI_Load (Shows a loading gui)
function RTBMM_GUI_Load(%text)
{
   RTBMM_WindowOverlay.clear();
   RTBMM_WindowOverlay.setVisible(0);
   
   if(!isObject(RTBMM_WindowSwatch))
   {
      RTBMM_GUI_Init();
      RTBMM_WindowSwatch.color = "0 0 0 0";
   }
      
   if(isObject(RTBMM_LoadingContentSwatch))
      RTBMM_LoadingContentSwatch.delete();
      
   if(%text $= "")
      %text = "Downloading Content...";
      
	%swatch = new GuiSwatchCtrl(RTBMM_LoadingContentSwatch)
	{
	   position = "100 35";
		extent = "679 555";
		color = "255 255 255 150";
		
		new GuiAnimatedBitmapCtrl()
		{
		   horizSizing = "center";
		   vertSizing = "bottom";
		   position = "324 240";
		   extent = "31 31";
		   bitmap = "./images/image_loadRing";
		   framesPerSecond = 15;
		   numFrames = 8;
		};
		
		new GuiMLTextCtrl()
		{
		   horizSizing = "center";
		   position = "140 275";
		   extent = "400 12";
		   profile = RTB_Verdana12Pt;
		   text = "<just:center><color:555555>"@%text;
		};
	};
   RTBMM_GUI_PushControl(%swatch);
   RTB_ModManager.getObject(0).add(%swatch);
}

//- RTBMM_GUI_FailLoad (Shows a failed loading gui)
function RTBMM_GUI_FailLoad(%text)
{
   if(isObject(RTBMM_LoadingContentSwatch))
      RTBMM_LoadingContentSwatch.delete();
      
   if(%text $= "")
      %text = "An Error Occurred";
      
	%swatch = new GuiSwatchCtrl(RTBMM_LoadingContentSwatch)
	{
	   fixedPosition = "1 1";
		extent = "680 553";
		color = "235 235 235 255";
		
		new GuiBitmapCtrl()
		{
		   horizSizing = "center";
		   vertSizing = "bottom";
		   position = "324 240";
		   extent = "31 31";
		   bitmap = "./images/image_loadRing_fail";
		};
		
		new GuiMLTextCtrl()
		{
		   horizSizing = "center";
		   position = "40 275";
		   extent = "600 12";
		   profile = RTB_Verdana12Pt;
		   text = "<just:center><color:555555>"@%text;
		};
	};
   RTBMM_GUI_PushControl(%swatch);
}

//- RTBMM_GUI_AnimateStar_Init (Fills a star progressively)
function RTBMM_GUI_AnimateStar_Init(%ctrl,%stage)
{
   if(!isObject(%ctrl))
      return;
      
   if(!$RTB::Options::MM::AnimateGUI)
   {
      %ctrl.setBitmap($RTB::Path @ "images/icon_star"@%stage);
      return;
   }
      
   %ctrl.currStage = 0;
   RTBMM_GUI_AnimateStar_Action(%ctrl,%stage);
}

//- RTBMM_GUI_AnimateStar_Action (Action for star filling)
function RTBMM_GUI_AnimateStar_Action(%ctrl,%stage)
{
   if(!isObject(%ctrl))
      return;
      
   %ctrl.setBitmap($RTB::Path @ "images/icon_star"@%ctrl.currStage);
   
   if(%ctrl.currStage >= %stage)
      return;
   
   %ctrl.currStage++;
   schedule(50,0,"RTBMM_GUI_AnimateStar_Action",%ctrl,%stage);
}

//- RTBMM_GUI_LoadRing_Clear (Clears loadring and schedule)
function RTBMM_GUI_LoadRing_Clear(%ctrl)
{
   %bitmap = new GuiBitmapCtrl()
   {
      position = %ctrl.position;
      extent = %ctrl.extent;
      bitmap = "./images/image_loadRing_clear";
   };
   %ctrl.getGroup().add(%bitmap);
   %ctrl.delete();
}

//*********************************************************
//* GUI Positioning Functions
//*********************************************************
//- RTBMM_GUI_CenterVert (Centers ctrlA inside ctrlB)
function RTBMM_GUI_CenterVert(%ctrlA,%ctrlB)
{
   if(isObject(%ctrlB))
   {
      %maxArea = getWord(%ctrlB.extent,1);
      %height = getWord(%ctrlA.extent,1);

      %yPosition = (%maxArea/2)-(%height/2);
      if(%ctrlB $= %ctrlA.getGroup())
         %ctrlA.position = getWord(%ctrlA.position,0) SPC (%yPosition+getWord(%ctrlB.position,1));
      else
         %ctrlA.position = getWord(%ctrlA.getCanvasPosition(),0) SPC (%yPosition+getWord(%ctrlB.getCanvasPosition(),1));
   }
   else
   {
      %ctrlB = %ctrlA.getGroup();
      %maxArea = getWord(%ctrlB.extent,1);
      %height = getWord(%ctrlA.extent,1);
      
      %yPosition = (%maxArea/2)-(%height/2);
      %ctrlA.position = getWord(%ctrlA.position,0) SPC %yPosition;
   }
}

//- RTBMM_GUI_CenterHoriz (Centers ctrlA inside ctrlB)
function RTBMM_GUI_CenterHoriz(%ctrlA,%ctrlB)
{
   if(isObject(%ctrlB))
   {
      %maxArea = getWord(%ctrlB.extent,0);
      %width = getWord(%ctrlA.extent,0);
      
      %xPosition = (%maxArea/2)-(%width/2);
      if(%ctrlB $= %ctrlA.getGroup())
         %ctrlA.position = (%xPosition+getWord(%ctrlB.position,0)) SPC getWord(%ctrlA.position,1);
      else
         %ctrlA.position = (%xPosition+getWord(%ctrlB.getCanvasPosition(),0)) SPC getWord(%ctrlA.getCanvasPosition(),1);
   }
   else
   {
      %ctrlB = %ctrlA.getGroup();
      %maxArea = getWord(%ctrlB.extent,0);
      %width = getWord(%ctrlA.extent,0);
      
      %xPosition = (%maxArea/2)-(%width/2);
      %ctrlA.position = %xPosition SPC getWord(%ctrlA.position,1);
   }
}

//- RTBMM_GUI_Center (Centers ctrlA inside ctrlB)
function RTBMM_GUI_Center(%ctrlA,%ctrlB)
{
   RTBMM_GUI_CenterHoriz(%ctrlA,%ctrlB);
   RTBMM_GUI_CenterVert(%ctrlA,%ctrlB);
}

//*********************************************************
//* Public GUI Generation Functions
//*********************************************************
//- RTBMM_GUI_createWindow (Creates an imitation GuiWindowCtrl comprised of resizable bitmaps)
function RTBMM_GUI_createWindow(%text)
{
   RTBMM_WindowOverlay.setVisible(1);
   
   %window = new GuiSwatchCtrl()
   {
      extent = "64 64";
      color = "0 0 0 0";
      
      new GuiBitmapCtrl()
      {
         extent = "6 28";
         minExtent = "1 1";
         bitmap = $RTB::Path@"images/ui/window_topLeft";
      };
      
      new GuiBitmapCtrl()
      {
         position = "6 0";
         extent = "53 28";
         horizSizing = "width";
         bitmap = $RTB::Path@"images/ui/window_topLoop";
         wrap = 1;
      };
      
      new GuiBitmapCtrl()
      {
         position = "59 0";
         extent = "5 28";
         minExtent = "1 1";
         horizSizing = "left";
         bitmap = $RTB::Path@"images/ui/window_topRight";
      };
      
      new GuiSwatchCtrl()
      {
         position = "4 31";
         extent = "56 29";
         horizSizing = "width";
         vertSizing = "height";
         color = "235 236 237 255";
      };
      
      new GuiBitmapCtrl()
      {
         position = "0 28";
         extent = "6 5";
         minExtent = "1 1";
         bitmap = $RTB::Path@"images/ui/content_topLeft";
      };
      
      new GuiBitmapCtrl()
      {
         position = "6 28";
         extent = "52 3";
         minExtent = "1 1";
         horizSizing = "width";
         bitmap = $RTB::Path@"images/ui/content_top";
         wrap = 1;
      };
      
      new GuiBitmapCtrl()
      {
         position = "58 28";
         extent = "6 5";
         minExtent = "1 1";
         horizSizing = "left";
         bitmap = $RTB::Path@"images/ui/content_topRight";
      };
      
      new GuiBitmapCtrl()
      {
         position = "0 33";
         extent = "4 24";
         minExtent = "1 1";
         vertSizing = "height";
         bitmap = $RTB::Path@"images/ui/content_left";
         wrap = 1;
      };
      
      new GuiBitmapCtrl()
      {
         position = "60 33";
         extent = "4 24";
         minExtent = "1 1";
         horizSizing = "left";
         vertSizing = "height";
         bitmap = $RTB::Path@"images/ui/content_right";
         wrap = 1;
      };
      
      new GuiBitmapCtrl()
      {
         position = "0 57";
         extent = "7 7";
         minExtent = "1 1";
         vertSizing = "top";
         bitmap = $RTB::Path@"images/ui/content_bottomLeft";
      };
      
      new GuiBitmapCtrl()
      {
         position = "57 57";
         extent = "7 7";
         minExtent = "1 1";
         horizSizing = "left";
         vertSizing = "top";
         bitmap = $RTB::Path@"images/ui/content_bottomRight";
      };
      
      new GuiBitmapCtrl()
      {
         position = "7 60";
         extent = "50 4";
         minExtent = "1 1";
         horizSizing = "width";
         vertSizing = "top";
         bitmap = $RTB::Path@"images/ui/content_bottom";
         wrap = 1;
      };
      
      new GuiMLTextCtrl()
      {
         position = "3 5";
         extent = "390 18";
         text = "<color:FAFAFA><just:left><font:Impact:18>  "@%text;
      };
      
      new GuiBitmapButtonCtrl()
      {
         position = "41 7";
         extent = "16 16";
         bitmap = $RTB::Path@"images/buttons/small/btnClose";
         horizSizing = "left";
         text = " ";
      };
   };
   %window.canvas = %window.getObject(3);
   
   %window.getObject(%window.getCount()-1).command = "RTBMM_GUI_closeWindow("@%window@");";
   %window.closeButton = %window.getObject(%window.getCount()-1);
   RTBMM_WindowOverlay.add(%window);
   
   return %window;
}

//- RTBMM_GUI_closeWindow (Closes a generated window and removes the overlay if necessary)
function RTBMM_GUI_closeWindow(%window)
{
   %window.delete();
   if(RTBMM_WindowOverlay.getCount() <= 0)
      RTBMM_WindowOverlay.setVisible(0);
}

//- RTBMM_GUI_createMessageBoxOK (Creates an OK message box from the above function)
function RTBMM_GUI_createMessageBoxOK(%title,%message,%ok)
{
   %window = RTBMM_GUI_createWindow(%title);
   %window.resize(0,0,300,100);
   RTBMM_GUI_Center(%window);
   
   %text = new GuiMLTextCtrl()
   {
      position = "10 10";
      extent = "275 12";
      text = "<font:Verdana:12><color:555555>"@%message;
   };
   %window.canvas.add(%text);
   %text.forceReflow();
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "0 35";
      vertSizing = "top";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnOK";
      command = %ok@"RTBMM_GUI_closeWindow("@%window@");";
   };
   %window.canvas.add(%button);
   RTBMM_GUI_CenterHoriz(%button);
   
   %window.resize(0,0,300,getWord(%text.extent,1)+85);
   RTBMM_GUI_Center(%window);
}

//- RTBMM_GUI_createMessageBoxOKCancel (Creates an OK/Cancel message box from the above function)
function RTBMM_GUI_createMessageBoxOKCancel(%title,%message,%ok,%cancel)
{
   %window = RTBMM_GUI_createWindow(%title);
   %window.resize(0,0,300,100);
   RTBMM_GUI_Center(%window);
   
   %text = new GuiMLTextCtrl()
   {
      position = "10 10";
      extent = "275 12";
      text = "<font:Verdana:12><color:555555>"@%message;
   };
   %window.canvas.add(%text);
   %text.forceReflow();
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "88 35";
      vertSizing = "top";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnOK";
      command = %ok@"RTBMM_GUI_closeWindow("@%window@");";
   };
   %window.canvas.add(%button);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "150 35";
      vertSizing = "top";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnCancel";
      command = %cancel@"RTBMM_GUI_closeWindow("@%window@");";
   };
   %window.canvas.add(%button);
   
   %window.resize(0,0,300,getWord(%text.extent,1)+85);
   RTBMM_GUI_Center(%window);
}

//- RTBMM_GUI_createHeader (Creates a standard header with value)
function RTBMM_GUI_createHeader(%type,%text)
{
   if(%type $= "")
      %type = 1;
     
   if(%type $= 1)
   {
      %borderProfile = RTBMM_CellLightProfile;
      %bitmapBackground = "./images/ui/header_light";
   }
   else
   {
      %borderProfile = RTBMM_CellDarkProfile;
      %bitmapBackground = "./images/ui/header_dark";
   }
      
   %header = new GuiBitmapCtrl()
   {
      position = 0 SPC $RTB::CModManager::GUI::CurrentY;
      extent = 680 SPC 28;
      bitmap = %bitmapBackground;
      wrap = 1;
      
      new GuiBitmapBorderCtrl()
      {
         profile = %borderProfile;
         horizSizing = "width";
         vertSizing = "height";
         position = 0 SPC 0;
         extent = 680 SPC 28;
      };
   };
   
   if(%text $= "")
   {
      RTBMM_GUI_PushControl(%header,1);
      return %header;
   }
   
   %ml = new GuiMLTextCtrl()
   {
      profile = RTBMM_PaginationProfile;
      position = "0 0";
      extent = "678 28";
      text = "<color:888888><font:Arial Bold:15><just:center>"@%text;
   };
   %header.add(%ml);
   RTBMM_GUI_PushControl(%header,1);
   
   %ml.forceReflow();
   RTBMM_GUI_Center(%ml);
   
   return %header;
}

//- RTBMM_GUI_createContent (creates a content row)
function RTBMM_GUI_createContent(%type,%height,%width1,%width2,%width3,%width4,%width5,%width6,%width7,%width8,%maxWidth)
{
   if(%type $= "")
      %type = 1;
      
   if(%type $= 1)
   {
      %bitmapBackground = "./images/ui/cell_gray";
      %borderProfile = RTBMM_CellLightProfile;
   }
   else if(%type $= 2)
   {
      %bitmapBackground = "./images/ui/cell_yellow";
      %borderProfile = RTBMM_CellYellowProfile;
   }
   
   if(%width1 $= "")
   {
      %width1 = 100;
      %width2 = "";
   }
      
   if(%height $= "")
      %height = 30;
      
   %i = 1;
   while(%width[%i] !$= "")
   {
      %i++;
   }
   %count = %i-1;
   
   if(%maxWidth $= "")
      %maxWidth = 680;

   %container = new GuiBitmapCtrl()
   {
      position = "0" SPC $RTB::CModManager::GUI::CurrentY;
      extent = %maxWidth SPC %height;
      bitmap = %bitmapBackground;
   };

   %currX = 0;
   for(%i=1;%i<%count+1;%i++)
   {
      %width = (%maxWidth/100)*%width[%i];
      if(strPos(%width,".") >= 0)
         %decimal = getSubStr(%width,strPos(%width,".")+1,strLen(%width));
      %width = mFloor(%width);
      %decimal = "0."@%decimal;

      %remainder += %decimal;
      %remainder = mFloatLength(%remainder,2);
      if(%remainder >= 1)
      {
         %remainder -= 1;
         %header.extent = getWord(%header.extent,0)+1 SPC %height;
         %currX += 1;
      }

      %header = new GuiBitmapBorderCtrl()
      {
         profile = %borderProfile;
         horizSizing = "width";
         vertSizing = "height";
         position = %currX SPC 0;
         extent = %width SPC %height;
      };
      %currX += (%width);
      %container.add(%header);
   }
   RTBMM_GUI_PushControl(%container,1);
   
   if(mFloatLength(%remainder,0) >= 1)
   {
      %header.extent = getWord(%header.extent,0)+1 SPC %height;
      %header.getObject(0).extent = getWord(%header.extent,0)+1 SPC %height-1;
      %header.getObject(0).getObject(0).extent = getWord(%header.extent,0)-2 SPC %height-2;
   }
   
   return %container;
}

//- RTBMM_GUI_createSplitHeader (creates a percentage-split header)
function RTBMM_GUI_createSplitHeader(%type,%width1,%text1,%width2,%text2,%width3,%text3,%width4,%text4,%width5,%text5,%width6,%text6,%width7,%text7,%width8,%text8)
{
   if(%type $= "")
      %type = 1;
      
   if(%type $= 1)
   {
      %bitmapBackground = "./images/ui/header_light";
      %borderProfile = RTBMM_CellLightProfile;
   }
   else
   {
      %bitmapBackground = "./images/ui/header_dark";
      %borderProfile = RTBMM_CellDarkProfile;
   }
      
   %i = 1;
   while(%width[%i] !$= "")
   {
      %i++;
   }
   %count = %i-1;
   
   %height = 28;
   %maxWidth = 680;

   %container = new GuiBitmapCtrl()
   {
      position = "0" SPC $RTB::CModManager::GUI::CurrentY;
      extent = %maxWidth SPC %height;
      bitmap = %bitmapBackground;
      wrap = 1;
   };

   %currX = 0;
   for(%i=1;%i<%count+1;%i++)
   {
      %width = (%maxWidth/100)*%width[%i];
      if(strPos(%width,".") >= 0)
         %decimal = getSubStr(%width,strPos(%width,".")+1,strLen(%width));
      %width = mFloor(%width);
      %decimal = "0."@%decimal;

      %remainder += %decimal;
      %remainder = mFloatLength(%remainder,2);
      if(%remainder >= 1)
      {
         %remainder -= 1;
         %header.extent = getWord(%header.extent,0)+1 SPC %height;
         %currX += 1;
      }

      %header = new GuiBitmapBorderCtrl()
      {
         profile = %borderProfile;
         horizSizing = "width";
         vertSizing = "height";
         position = %currX SPC 0;
         extent = %width SPC %height;
      };
      %currX += %width;
      %container.add(%header);
      
      if(%text[%i] !$= "")
      {
         %ml = new GuiMLTextCtrl()
         {
            profile = RTBMM_PaginationProfile;
            position = "0 6";
            vertSizing = "width";
            extent = %width SPC %height;
            text = "<color:888888><font:Arial Bold:15><just:center>"@%text[%i];
         };
         %header.add(%ml);
      }
   }
   RTBMM_GUI_PushControl(%container,1);
   
   if(mFloatLength(%remainder,0) >= 1)
   {
      %header.extent = getWord(%header.extent,0)+1 SPC %height;
      %header.getObject(0).extent = getWord(%header.extent,0)+1 SPC %height-1;
      %header.getObject(0).getObject(0).extent = getWord(%header.extent,0)-2 SPC %height-2;
   }
   
   return %container;
}

//- RTBMM_GUI_createFooter (Creates a footer, and adds pagination if necessary)
function RTBMM_GUI_createFooter(%type)
{      
   %pages = $RTB::CModManager::Cache::Pagination::NumPages;
   %currPage = $RTB::CModManager::Cache::Pagination::CurrPage;
   
   if(%pages <= 1)
   {
      RTBMM_GUI_createSplitHeader(2,"100"," ");   
      return;
   }
 
   %paginationText = "Goto Page ";
   if(%currPage > 1)
      %paginationText = %paginationText @ "<a:pagination-"@%currPage-1@">Previous</a> ";
   %paginationText = %paginationText @ ((%currPage $= 1) ? "<spush><color:666666><font:Verdana Bold:15>[1]<spop>" : "<a:pagination-1>1</a>");
 
   if(%pages > 5)
   {
      %start = min(max(1, %currPage-4), %pages-5);
      %end = max(min(%pages, %currPage+4), 6);
      
      %paginationText = %paginationText @ ((%start > 1) ? " <spush><color:DDDDDD><font:Verdana Bold:15>...<spop> " : " ");
      
      for(%i=%start+1;%i<%end;%i++)
      {
         %paginationText = %paginationText @ ((%currPage $= %i) ? "<spush><color:666666><font:Verdana Bold:15>["@%i@"]<spop>" : "<a:pagination-"@%i@">"@%i@"</a>");
         if(%i < %end-1)
            %paginationText = %paginationText @ " ";
      }
      
      %paginationText = %paginationText @ ((%end < %pages) ? " <spush><color:DDDDDD><font:Verdana Bold:15>...<spop> " : " ");
   }
   else
   {
      %paginationText = %paginationText @ " ";
      
      for(%i=2;%i<%pages;%i++)
      {
         %paginationText = %paginationText @ ((%currPage $= %i) ? "<spush><color:666666><font:Verdana Bold:15>["@%i@"]<spop>" : "<a:pagination-"@%i@">"@%i@"</a>");
         if(%i < %pages)
            %paginationText = %paginationText @ " ";
      }
   }
   %paginationText = %paginationText @ ((%currPage $= %i) ? "<spush><color:666666><font:Verdana Bold:15>["@%pages@"]<spop>" : "<a:pagination-"@%pages@">"@%pages@"</a>");
   
   if(%currPage < %pages)
      %paginationText = %paginationText @ " <a:pagination-"@%currPage+1@">Next</a>";
   
   RTBMM_GUI_createHeader(2,"<font:Verdana Bold:13><color:333333><just:right>"@%paginationText@"  ");
}

//- RTBMM_GUI_getXPlacement (Calculates next placement from supplied control)
function RTBMM_GUI_getXPlacement(%ctrl)
{
   %xPlace = getWord(%ctrl.position,0)+(getWord(%ctrl.extent,0));
   return %xPlace;
}

//- RTBMM_GUI_calcGUIContainer (Calculates extent of container to surround supplied controls)
function RTBMM_GUI_calcGUIContainer(%ctrl1,%ctrl2,%ctrl3,%ctrl4,%ctrl5,%ctrl6,%ctrl7,%ctrl8)
{
   %i = 1;
   while(%ctrl[%i] !$= "")
   {
      if(getWord(%ctrl[%i].position,0)+getWord(%ctrl[%i].extent,0) > %xMax)
         %xMax = getWord(%ctrl[%i].position,0)+getWord(%ctrl[%i].extent,0);
      if(getWord(%ctrl[%i].extent,1) > %yMax)
         %yMax = getWord(%ctrl[%i].extent,1);
         
      %i++;
   }
   
   %xMax++;
   %yMax+=2;
   return %xMax SPC %yMax;
}

//- RTBMM_GUI_calcWidthPercent (Calculates width from percentage)
function RTBMM_GUI_calcWidthPercent(%target,%width1,%width2,%width3,%width4,%width5,%width6,%width7,%width8)
{
   %i = 1;
   while(%width[%i] !$= "")
   {
      %i++;
   }
   %count = %i-1;
   %maxWidth = 680-(%count+1);
   
   for(%i=1;%i<%count+1;%i++)
   {
      %actualWidth[%i] = (%maxWidth/100)*%width[%i];
      if(strPos(%actualWidth[%i],".") >= 0)
         %decimal = getSubStr(%actualWidth[%i],strPos(%actualWidth[%i],".")+1,strLen(%actualWidth[%i]));
      %actualWidth[%i] = mFloor(%actualWidth[%i]);
      
      %decimal = "0."@%decimal;
      %remainder += %decimal;
      %remainder = mFloatLength(%remainder,1);
      if(%remainder >= 1)
      {
         %remainder -= 1;
         %actualWidth[%i-1] = %actualWidth[%i-1]+1;
      }
   }
   return %actualWidth[%target];
}

//- RTBMM_GUI_createMessage (Creates a standard message with value)
function RTBMM_GUI_createMessage(%value)
{
   %container = RTBMM_GUI_createContent(1,"30","100");
   
   %ml = new GuiMLTextCtrl()
   {
      position = "0 5";
      extent = "678 27";
      text = "<color:888888><font:Verdana:12><just:center>"@%value;
   };
   %container.getBottom(1).add(%ml);
   
   %ml.forceReflow();
   %height = getWord(%ml.extent,1)+10;
   %container.resize(getWord(%container.position,0),getWord(%container.position,1),getWord(%container.extent,0),%height);
   RTBMM_GUI_AutoResize();
   
   return %container;
}

//- RTBMM_GUI_createRatingSwatch (Creates rating box 88x16 with animated loading stars)
function RTBMM_GUI_createRatingSwatch(%rating)
{
   %swatch = new GuiSwatchCtrl()
   {
      extent = "88 50";
      color = "0 0 0 0";
   };
   
   if(strPos(%rating,"-") > 0)
   {
      %numRatings = getSubStr(%rating,strPos(%rating,"-")+1,strLen(%rating));
      %rating = getSubStr(%rating,0,strPos(%rating,"-"));
   }
      
   for(%i=0;%i<strLen(%rating);%i++)
   {
      %star = getSubStr(%rating,%i,1);
      
      %stctrl = new GuiBitmapCtrl()
      {
         position = (18*%i) SPC 0;
         extent = "16 16";
         bitmap = $RTB::Path@"images/icon_star0";
      };
      %swatch.add(%stctrl);
      RTBMM_GUI_CenterVert(%stctrl);
      schedule(%i*300,0,"RTBMM_GUI_AnimateStar_Init",%stctrl,%star);
      if(%numRatings !$= "")
         %stctrl.shift(0,-5);
   }
   if(%numRatings !$= "")
   {
      %s = (%numRatings == 1) ? "" : "s";
      %mlTextCtrl = new GuiMLTextCtrl()
      {
         position = "0 30";
         extent = "88";
         text = "<just:center><font:Arial:12><color:BBBBBB>"@%numRatings@" rating"@%s;
      };
      %swatch.add(%mlTextCtrl);
   }
   return %swatch;
}

//- RTBMM_GUI_addToMods (Flashes a little plus icon on the YourMods tab to indicate you have a new add-on)
function RTBMM_GUI_addToMods()
{
   %bitmap = new GuiBitmapCtrl()
   {
      extent = "16 16";
      position = "10 320";
      bitmap = $RTB::Path@"images/icon_add";
   };
   RTB_ModManager.getObject(0).add(%bitmap);
   
   %bitmap.setColor("1 1 1 0");
   for(%i=1;%i<6;%i++)
   {
      %bitmap.schedule(%i*30,"setColor","1 1 1 "@%i/5);
   }
   
   for(%i=1;%i<11;%i++)
   {
      %bitmap.schedule((%i*50)+700,"setColor","1 1 1 "@1-(%i/10));
   }
   %bitmap.schedule(2000,"delete");
}

//- RTBMM_GUI_setTransfers (Sets the number of transfers in the transfer queue and updates gui indicator)
function RTBMM_GUI_setTransfers(%number)
{
   if(%number <= 0)
      RTBMM_iconTransfers.setVisible(0);
   else
   {
      RTBMM_iconTransfers.setVisible(1);
      RTBMM_iconTransfers_cDigit.setVisible(0);
      RTBMM_iconTransfers_lDigit.setVisible(0);
      RTBMM_iconTransfers_rDigit.setVisible(0);
      
      if(%number > 9)
      {
         RTBMM_iconTransfers_lDigit.setVisible(1);
         RTBMM_iconTransfers_rDigit.setVisible(1);
         RTBMM_iconTransfers_lDigit.setValue("<color:FFFFFF><font:Verdana Bold:10><just:center>"@getSubStr(%number,0,1));
         RTBMM_iconTransfers_rDigit.setValue("<color:FFFFFF><font:Verdana Bold:10><just:center>"@getSubStr(%number,1,1));
      }
      else
      {
         RTBMM_iconTransfers_cDigit.setVisible(1);
         RTBMM_iconTransfers_cDigit.setValue("<color:FFFFFF><font:Verdana Bold:10><just:center>"@%number);
      }
   }
}

//#####################################################################################################
//
//                         _ _            _   _             
//       /\               | (_)          | | (_)            
//      /  \   _ __  _ __ | |_  ___  __ _| |_ _  ___  _ __  
//     / /\ \ | '_ \| '_ \| | |/ __|/ _` | __| |/ _ \| '_ \ 
//    / ____ \| |_) | |_) | | | (__| (_| | |_| | (_) | | | |
//   /_/    \_\ .__/| .__/|_|_|\___|\__,_|\__|_|\___/|_| |_|
//            | |   | |                                     
//            |_|   |_|                                     
//
//##################################################################################################### 

//#############################################################################
//#
//#   Transmission Layers
//#
//#############################################################################
//#
//# Layer 1: Browsing
//# Layer 2: Torquejax
//# Layer 3: Transfer Data Gather
//#
//#############################################################################

//#############################################################################
//#
//#   Function manifest for real-time system usage
//#
//#############################################################################
//#
//# + Error Handling                         //> Messages or Errors from server
//# ---------------------------------------------------------------------------
//# - RTBMM_onMessage()                      //> Center Message
//# - RTBMM_onError()                        //> Error Box
//# - RTBMM_onBarred()                       //> Incase you get barred (BAD!)
//#
//# + General Handling                       //> General Callback Routing
//# - RTBMM_onPagination()                   //> Pagination Callback
//#
//# + Auth                                   //> Logging into your RTB Profile
//# ---------------------------------------------------------------------------
//# - RTBMM_Auth_Init()                      //> Entrance
//# - RTBMM_Auth_onReply()                   //> Reply
//# - RTBMM_Auth_onFail()                    //> Failure
//#
//# + News Feed View                         //> News Feed View
//# ---------------------------------------------------------------------------
//# - RTBMM_NewsFeedView_Init()              //> Entrance
//# - RTBMM_NewsFeedView_onReplyStart()      //> Reply Start
//# - RTBMM_NewsFeedView_onReply()           //> Reply
//# - RTBMM_NewsFeedView_onFail()            //> Failure
//#
//# + Category View                          //> Category View
//# ---------------------------------------------------------------------------
//# - RTBMM_CategoryView_Init()              //> Entrance
//# - RTBMM_CategoryView_onReplyStart()      //> Reply Start
//# - RTBMM_CategoryView_onReply()           //> Reply
//# - RTBMM_CategoryView_onReplyStop()       //> Reply Stop
//# - RTBMM_CategoryView_onFail()            //> Failure
//#
//# + Section View                           //> Section View
//# ---------------------------------------------------------------------------
//# - RTBMM_SectionView_Init()               //> Entrance
//# - RTBMM_SectionView_onReplyStart()       //> Reply Start
//# - RTBMM_SectionView_onReply()            //> Reply
//# - RTBMM_SectionView_onReplyStop()        //> Reply Stop
//# - RTBMM_SectionView_onFail()             //> Failure
//#
//# + File View                              //> File View
//# ---------------------------------------------------------------------------
//# - RTBMM_FileView_Init()                  //> Entrance
//# - RTBMM_FileView_onReplyStart()          //> Reply Start
//# - RTBMM_FileView_onReply()               //> Reply
//# - RTBMM_FileView_onReplyStop()           //> Reply Stop
//# - RTBMM_FileView_onFail()                //> Failure
//# - RTBMM_FileView_Rate()                  //> Submit Rating
//# - RTBMM_FileView_onRateReply()           //> Rating Reply
//# - RTBMM_FileView_Report()                //> Report File
//# - RTBMM_FileView_SendReport()            //> Submits Report
//# - RTBMM_FileView_onReportReply()         //> Report Reply
//# - RTBMM_FileView_onReportFail()          //> Report Fail
//#
//# + Packs View
//# ---------------------------------------------------------------------------
//# - RTBMM_PacksView_Init()                 //> Entrance
//# - RTBMM_PacksView_onReplyStart()         //> Reply Start
//# - RTBMM_PacksView_onReply()              //> Reply
//# - RTMBM_PacksView_onFail()               //> Failure
//#
//# + Top List View                          //> Top List View
//# ---------------------------------------------------------------------------
//# - RTBMM_TopListView_Init()               //> Entrance
//# - RTBMM_TopListView_onReplyStart()       //> Reply Start
//# - RTBMM_TopListView_onReply()            //> Reply
//# - RTBMM_TopListView_onReplyStop()        //> Reply Stop
//# - RTBMM_TopListView_onFail()             //> Failure
//#
//# + Search View                            //> Search View
//# ---------------------------------------------------------------------------
//# - RTBMM_SearchView_Init()                //> Entrance
//# - RTBMM_SearchView_onReplyStart()        //> Reply Start
//# - RTBMM_SearchView_onReply()             //> Reply
//# - RTBMM_SearchView_onReplyStop()         //> Reply Stop
//# - RTBMM_SearchView_onFail()              //> Failure
//# - RTBMM_SearchView_doSearch()            //> Makes a search request
//# - RTBMM_SearchView_onSearchReplyStart()  //> Reply Start
//# - RTBMM_SearchView_onSearchReply()       //> Reply
//# - RTBMM_SearchView_onSearchReplyStop()   //> Reply End
//# - RTBMM_SearchView_onSearchReplyFail()   //> Failure
//#
//# + Recent View                            //> Recent View
//# ---------------------------------------------------------------------------
//# - RTBMM_RecentView_Init()                //> Entrance
//# - RTBMM_RecentView_onReplyStart()        //> Reply Start
//# - RTBMM_RecentView_onReply()             //> Reply
//# - RTBMM_RecentView_onReplyStop()         //> Reply Stop
//# - RTBMM_RecentView_onFail()              //> Failure
//#
//# + All Files View                         //> All Files View
//# ---------------------------------------------------------------------------
//# - RTBMM_AllFilesView_Init()              //> Entrance
//# - RTBMM_AllFilesView_onReplyStart()      //> Reply Start
//# - RTBMM_AllFilesView_onReply()           //> Reply
//# - RTBMM_AllFilesView_onReplyStop()       //> Reply Stop
//# - RTBMM_AllFilesView_onFail()            //> Failure
//#
//#############################################################################

//*********************************************************
//* Error Handling
//*********************************************************
//- RTBMM_onMessage (General message handler)
function RTBMM_onMessage(%tcp,%line)
{
}

//- RTBMM_onError (Error handling)
%RTBMM_SB.registerResponseHandler("ERROR","RTBMM_onError",1);
function RTBMM_onError(%tcp,%line)
{
   if(isObject(RTBMM_LoadingContentSwatch))
   {
      RTBMM_GUI_FailLoad(%line);
      %tcp.neutralise();
   }
   else
   {
      %tcp.neutralise();
      RTBMM_GUI_Init();
      RTBMM_GUI_createMessageBoxOK("Error","<just:center>"@%line);
   }
}

//- RTBMM_onBarred (Barred)
function RTBMM_onBarred(%tcp,%line)
{
}

//*********************************************************
//* General Handling
//*********************************************************
//- RTBMM_onPagination (Pagination Callback)
%RTBMM_SB.registerResponseHandler("PAGINATION","RTBMM_onPagination",1);
function RTBMM_onPagination(%tcp,%line)
{
   $RTB::CModManager::Cache::Pagination::NumPages = getField(%line,0);
   $RTB::CModManager::Cache::Pagination::CurrPage = getField(%line,1);
}

//*********************************************************
//* Auth
//*********************************************************
//- RTBMM_Auth_Init (Entrance)
function RTBMM_Auth_Init()
{
   if($RTB::CModManager::Session::LoggedIn)
   {
      RTB_ModManager.getObject(0).setText("Mod Manager - Logged in as "@$RTB::CModManager::Session::LoginName);
      return;
   }
      
   if($RTB::Barred::Download)
   {
      RTB_ModManager.getObject(0).setText("Mod Manager - Barred from Use");
      return;
   }
      
   RTBMM_SendRequest("AUTH",2);
   RTB_ModManager.getObject(0).setText("Mod Manager - Logging In...");
}

//- RTBMM_Auth_onReply (Reply)
%RTBMM_SB.registerResponseHandler("AUTH","RTBMM_Auth_onReply");
function RTBMM_Auth_onReply(%tcp,%line)
{
   if(getField(%line,0) $= "1")
   {
      $RTB::CModManager::Session::LoggedIn = 1;
      $RTB::CModManager::Session::LoginName = getField(%line,1);
      RTB_ModManager.getObject(0).setText("Mod Manager - Logged in as "@$RTB::CModManager::Session::LoginName);
   }
   else if(getField(%line,0) $= "0")
   {
      if(getField(%line,1) $= "2")
         RTB_ModManager.getObject(0).setText("Mod Manager - No Account");
      else
         RTB_ModManager.getObject(0).setText("Mod Manager - Error Occurred");
   }
}

//- RTBMM_Auth_onFail (Failure)
%RTBMM_SB.registerFailHandler("AUTH","RTBMM_Auth_onFail");
function RTBMM_Auth_onFail(%tcp,%error)
{
   RTB_ModManager.getObject(0).setText("Mod Manager - Connection Failed");
}

//*********************************************************
//* News Feed View
//*********************************************************
//- RTBMM_NewsFeedView_Init (Entrance)
function RTBMM_NewsFeedView_Init(%page)
{
   RTBMM_GUI_Load();
   
   for(%i=0;%i<RTBMM_FileCache.getCount();%i++)
   {
      %item = RTBMM_FileCache.getObject(%i);
      if(%item.file_platform $= "rtb" && %item.file_id > 0)
      {
         %files = %files@%item.file_id@"-";
      }
   }
   if(strLen(%files) > 1)
      %files = getSubStr(%files,0,strLen(%files)-1);

   RTBMM_SendRequest("GETNEWS",1,%page,%files);
   RTBMM_Zones_Track("NewsFeedView","RTBMM_NewsFeedView_Init("@%page@");","RTBMM_NewsFeedView_Init(%%page%%);");
}

//- RTBMM_NewsFeedView_onReplyStart (Reply)
function RTBMM_NewsFeedView_onReplyStart(%tcp)
{
   RTBMM_GUI_Init();  
}

//- RTBMM_NewsFeedView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETNEWS","RTBMM_NewsFeedView_onReply");
function RTBMM_NewsFeedView_onReply(%tcp,%line)
{
   if(getField(%line,0) $= "NEWS")
   {
      %news_id = getField(%line,1);
      %news_type = getField(%line,2);
      %news_subject = getField(%line,3);
      %news_date = getField(%line,4);
      %news_message = getField(%line,5);
      %news_author = getField(%line,6);
      %news_comments = getField(%line,7);
      
      %control = new GuiSwatchCtrl()
      {
         position = "5" SPC 5+$RTB::CModManager::GUI::CurrentY;
         extent = "669 228";
         color = "180 185 191 255";
      };
      
      %header = RTBMM_GUI_createHeader(1," ");
      %control.add(%header);
      %header.resize(1,1,668,28);
      
      %header_text = %header.getObject(1);
      %header_text.setText("<color:888888><font:Arial Bold:15>"@%news_subject);
      %header_text.shift(25,0);
      
      %cellType = 1;
      if(%news_type $= 1)
         %icon = "icon_newaddon";
      else if(%news_type $= 2)
      {
         %icon = "icon_update";
         %cellType = 2;
      }
      else if(%news_type $= 3)
         %icon = "icon_feed";
      
      %bitmap = new GuiBitmapCtrl()
      {
         position = "0 0";
         extent = "16 16";
         bitmap = $RTB::Path@"images/"@%icon;
      };
      %header.getBottom().add(%bitmap);
      RTBMM_GUI_CenterVert(%bitmap);
      %bitmap.shift(3,-1);
      
      %date = new GuiMLTextCtrl()
      {
         profile = "";
         position = "0 6";
         extent = "668 15";
         text = "<color:888888><font:Verdana:12><just:right>"@%news_date;
      };
      %header.add(%date);
      %date.shift(-5,1);
      
      %content = RTBMM_GUI_createContent(%cellType,171,100);
      %control.add(%content);
      
      %news_message = strreplace(%news_message,"<spcr>","<bitmap:"@$RTB::Path@"images/bullet_news>");
      %message = new GuiMLTextCtrl()
      {
         profile = "RTBMM_NewsContentProfile";
         position = "2 4";
         extent = "668 0";
         text = "<color:888888><font:Verdana:12><just:left>"@%news_message@"<br>";
      };
      %content.getBottom(1).add(%message);
      
      RTBMM_GUI_PushControl(%control);
      %message.forceReflow();
      
      %spacer = new GuiSwatchCtrl()
      {
         position = "0 "@getWord(%message.extent,1)+getWord(%message.position,1)-5;
         extent = "665 1";
         minExtent = "1 1";
         color = "255 255 255 255";
      };
      %content.getBottom(1).add(%spacer);
      if(%cellType $= "cellY")
         %spacer.color = "255 255 150 255";
         
      %spacer = new GuiSwatchCtrl()
      {
         position = "0 "@getWord(%message.extent,1)+getWord(%message.position,1)-6;
         extent = "665 1";
         minExtent = "1 1";
         color = "200 200 200 255";
      };
      if(%cellType $= "cellY")
         %spacer.color = "220 220 100 255";
         
      %content.getBottom(1).add(%spacer);
      %message.setText(%message.getText()@"<br> <font:Verdana Bold:12>by "@%news_author@"<just:right>"@%news_comments@" comments   ");
      %message.forceReflow();
      
      %content.resize(1,29,668,getWord(%message.extent,1)+12);
      
      %control.extent = "669" SPC getWord(%content.extent,1) + getWord(%content.position,1);
      
      $RTB::CModManager::GUI::CurrentY = getWord(%control.extent,1) + getWord(%control.position,1) + 5;
      RTBMM_GUI_Resize();
   }
}

//- RTBMM_NewsFeedView_onReplyStop (Reply)
function RTBMM_NewsFeedView_onReplyStop(%tcp)
{
   $RTB::CModManager::GUI::CurrentY += 5;
   RTBMM_GUI_Resize();
}

//- RTBMM_NewsFeedView_onFail (Failure)
%RTBMM_SB.registerFailHandler("GETNEWS","RTBMM_NewsFeedView_onFail");
function RTBMM_NewsFeedView_onFail(%tcp,%error)
{
   RTBMM_GUI_FailLoad();
}

//*********************************************************
//* Category View
//*********************************************************
//- RTBMM_CategoryView_Init (Entrance)
function RTBMM_CategoryView_Init()
{
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("GETCATEGORIES",1);
   RTBMM_Zones_Track("CategoryView","RTBMM_CategoryView_Init();");
}

//- RTBMM_CategoryView_onReplyStart (Reply)
function RTBMM_CategoryView_onReplyStart(%tcp)
{
   RTBMM_GUI_Init();
}

//- RTBMM_CategoryView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETCATEGORIES","RTBMM_CategoryView_onReply");
function RTBMM_CategoryView_onReply(%tcp,%line)
{
   if(getField(%line,0) $= "CATEGORY")
   {
      RTBMM_GUI_createSplitHeader(2,"100","<color:FAFAFA><just:left><font:Impact:18>  "@getField(%line,1));
      RTBMM_GUI_createSplitHeader(1,"70","<font:Arial Bold:15>Section","7","<font:Arial Bold:15>Files","23","<font:Arial Bold:15>Latest Addition");
   }
   else if(getField(%line,0) $= "SECTION")
   {
      %container = RTBMM_GUI_createContent(1,38,6,64,7,23);
      
      %c_icon = %container.getObject(0);
      %c_information = %container.getObject(1);
      %c_files = %container.getObject(2);
      %c_latest = %container.getObject(3);
      
      %swatch = new GuiSwatchCtrl()
      {
         position = "0 0";
         extent = %c_icon.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_icon.add(%swatch);
      
      %icon = new GuiBitmapCtrl()
      {
         extent = "26 26";
         bitmap = "./images/icon_section";
      };
      %c_icon.add(%icon);
      RTBMM_GUI_Center(%icon);

      %swatch = new GuiSwatchCtrl()
      {
         position = "0 0";
         extent = %c_information.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_information.add(%swatch);
      
      %text = new GuiMLTextCtrl()
      {
         position = "1 3";
         extent = "307 18";
         text = "<font:Arial Bold:15><color:888888> "@getField(%line,2);
      };
      %c_information.add(%text);
      
      %text = new GuiMLTextCtrl()
      {
         position = "3 19";
         extent = "450 18";
         text = "<font:Verdana:12><color:444444>"@getField(%line,3);
      };
      %c_information.add(%text);
      
      %swatch = new GuiSwatchCtrl()
      {
         position = "0 0";
         extent = %c_files.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_files.add(%swatch);
      
      %text = new GuiTextCtrl()
      {
         profile = RTB_Verdana12PtCenter;
         position = "0 7";
         extent = 100 SPC 18;
         text = "\c1"@getField(%line,5);
      };
      %c_files.add(%text);
      RTBMM_GUI_Center(%text);
      
      %swatch = new GuiSwatchCtrl()
      {
         position = "0 0";
         extent = %c_latest.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_latest.add(%swatch);
      
      %text = new GuiMLTextCtrl()
      {
         position = "0 7";
         extent = 150 SPC 18;
         text = "<just:center><font:Verdana:12><color:888888>"@getField(%line,4);
      };
      %c_latest.add(%text);
      %text.forceReflow();
      RTBMM_GUI_Center(%text);
      
      %mouseCtrl = new GuiMouseEventCtrl()
      {
         position = %container.position;
         extent = %container.extent;
         
         eventType = "sectionSelect";
         eventCallbacks = "1111";
         
         sectionID = getField(%line,1);
         container = %container;
      };
      RTBMM_GUI_PushControl(%mouseCtrl,1);
   }
}

//- Event_sectionSelect::onMouseEnter (MouseEnter Callback)
function Event_sectionSelect::onMouseEnter(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount();%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.visible = 1;
   }
}

//- Event_sectionSelect::onMouseLeave (MouseLeave Callback)
function Event_sectionSelect::onMouseLeave(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount();%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.visible = 0;
      %swatch.color = "255 255 255 100";
   }
}

//- Event_sectionSelect::onMouseDown (MouseDown Callback)
function Event_sectionSelect::onMouseDown(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount();%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.color = "255 230 230 150";
   }
}

//- Event_sectionSelect::onMouseUp (MouseUp Callback)
function Event_sectionSelect::onMouseUp(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount();%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.color = "255 255 255 100";
   }   
   RTBMM_SectionView_Init(%ctrl.sectionID);
}

//- RTBMM_CategoryView_onReplyStop (Reply)
function RTBMM_CategoryView_onReplyStop(%tcp)
{
   //RTBMM_GUI_createSplitHeader(2,100,"");
   RTBMM_GUI_createHeader(2);
}

//- RTBMM_CategoryView_onFail (Failure)
%RTBMM_SB.registerFailHandler("GETCATEGORIES","RTBMM_CategoryView_onFail");
function RTBMM_CategoryView_onFail(%tcp,%error)
{
   if(%error $= "DNS")
      RTBMM_GUI_FailLoad("Connection Failed");
   else
      RTBMM_GUI_FailLoad("Connection Failed");
}

//*********************************************************
//* Section View
//*********************************************************
//- RTBMM_SectionView_Init (Entrance)
function RTBMM_SectionView_Init(%section,%page)
{
   RTBMM_GUI_Load();
   
   $RTB::CModManager::Cache::CurrentSection = %section;
   
   RTBMM_SendRequest("GETSECTION",1,%section,%page);
   RTBMM_Zones_Track("SectionView","RTBMM_SectionView_Init("@%section@",\""@%page@"\");","RTBMM_SectionView_Init("@%section@",%%page%%);");
}

//- RTBMM_SectionView_onReplyStart (Reply)
function RTBMM_SectionView_onReplyStart(%tcp)
{
   RTBMM_GUI_Init();
}

//- RTBMM_SectionView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETSECTION","RTBMM_SectionView_onReply");
function RTBMM_SectionView_onReply(%tcp,%line)
{
   if(getWord(%line,0) $= "HEADER")
   {
      RTBMM_GUI_createSplitHeader(2,"100","<color:FAFAFA><just:left><font:Impact:18>  "@getField(%line,1));
      RTBMM_GUI_createSplitHeader(1,"58","<font:Arial Bold:15>File","15","<font:Arial Bold:15>Submitter","12","<font:Arial Bold:15>Downloads","15","<font:Arial Bold:15>Rating");
   }
   else if(getWord(%line,0) $= "FILE")
   {  
      $RTB::CModManager::Cache::ElementsAdded++;
      
      if(getField(%line,2) $= 1)
         %container = RTBMM_GUI_createContent(2,50,6,52,15,12,15);
      else
         %container = RTBMM_GUI_createContent(1,50,6,52,15,12,15);
      
      %c_icon = %container.getObject(0);     
      %c_information = %container.getObject(1);
      %c_submitter = %container.getObject(2);
      %c_downloads = %container.getObject(3);
      %c_rating = %container.getObject(4);
      
      if(getField(%line,2) $= 1)
      {
         %swatch = new GuiSwatchCtrl()
         {
            vertSizing = "height";
            position = "0 0";
            extent = %c_icon.extent;
            color = "255 255 255 100";
            visible = 0;
         };
         %c_icon.add(%swatch);
         %swatch = new GuiSwatchCtrl()
         {
            vertSizing = "center";
            extent = "16 32";
            color = "0 0 0 0";
         };
         %c_icon.add(%swatch);
         %icon = new GuiBitmapCtrl()
         {
            position = "0 3";
            extent = "16 16";
            bitmap = "./images/icons/"@getField(%line,3);
         };
         %swatch.add(%icon);
         %icon = new GuiBitmapCtrl()
         {
            position = "0 19";
            extent = "16 16";
            bitmap = "./images/icon_new";
         };
         %swatch.add(%icon);
         RTBMM_GUI_Center(%swatch,%c_icon);
      }
      else
      {
         %swatch = new GuiSwatchCtrl()
         {
            vertSizing = "height";
            position = "0 0";
            extent = %c_icon.extent;
            color = "255 255 255 100";
            visible = 0;
         };
         %c_icon.add(%swatch);
         %icon = new GuiBitmapCtrl()
         {
            vertSizing = "center";
            position = "0 0";
            extent = "16 16";
            bitmap = "./images/icons/"@getField(%line,3);
         };
         %c_icon.add(%icon);
         RTBMM_GUI_Center(%icon,%c_icon);
      }
      if(RTBMM_FileCache.get(getField(%line,1)) && !RTBMM_FileCache.get(getField(%line,1)).file_isContent)
      {
         %star = new GuiBitmapCtrl()
         {
            position = "24 -2";
            extent = "16 16";
            bitmap = "./images/bullet_star";
         };
         %c_icon.add(%star);
      }

      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_information.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_information.add(%swatch);
      %text = new GuiMLTextCtrl()
      {
         position = "1 3";
         extent = "327 18";
         text = "<font:Arial Bold:15><color:888888> "@getField(%line,4);
      };
      %c_information.add(%text);
      
      %mltext = new GuiMLTextCtrl()
      {
         position = "3 18";
         extent = "325 18";
         text = "<font:Verdana:12><color:444444>"@getField(%line,5);
      };
      %c_information.add(%mltext);
      %text = new GuiMLTextCtrl()
      {
         vertSizing = "top";
         position = "3 32";
         extent = "325 18";
         text = "<font:Verdana:12><color:666666>\xBB <color:888888>By "@getField(%line,6);
      };
      %c_information.add(%text);
      
      %mltext.forceReflow();
      %extent = getWord(%mltext.extent,1);
      %container.resize(getWord(%container.position,0),getWord(%container.position,1),getWord(%container.extent,0),%extent+36);
      RTBMM_GUI_AutoResize();

      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_submitter.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_submitter.add(%swatch);
      %text = new GuiTextCtrl()
      {
         profile = RTB_Verdana12PtCenter;
         position = "0 7";
         extent = 100 SPC 18;
         text = "\c1"@getField(%line,7);
      };
      %c_submitter.add(%text);
      RTBMM_GUI_Center(%text);
      
      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_downloads.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_downloads.add(%swatch);
      %text = new GuiMLTextCtrl()
      {
         position = "0 7";
         extent = 150 SPC 18;
         text = "<just:center><font:Verdana:12><color:888888>"@getField(%line,8);
      };
      %c_downloads.add(%text);
      %text.forceReflow();
      RTBMM_GUI_Center(%text);
      
      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_rating.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_rating.add(%swatch);
      %swatch = RTBMM_GUI_createRatingSwatch(getField(%line,9));
      %c_rating.add(%swatch);
      RTBMM_GUI_Center(%swatch);
      
      %mouseCtrl = new GuiMouseEventCtrl()
      {
         position = %container.position;
         extent = %container.extent;
         
         eventType = "fileSelect";
         eventCallbacks = "1111";
         
         persistent = 1;
         
         fileID = getField(%line,1);
         fileRecent = getField(%line,2);
         container = %container;
      };
      RTBMM_GUI_PushControl(%mouseCtrl,1);
      
      %downloadBtn = new GuiBitmapButtonCtrl()
      {
         position = "375 1";
         extent = "16 16";
         visible = 0;
         bitmap = $RTB::Path@"images/buttons/small/btnDownload";
         text = " ";
         command = "RTBMM_TransferView_Add("@getField(%line,1)@");";
      };
      %mouseCtrl.add(%downloadBtn);
      %container.dlIcon = %downloadBtn;
      
      %reportBtn = new GuiBitmapButtonCtrl()
      {
         position = "375" SPC getWord(%mouseCtrl.extent,1)-19;
         extent = "16 16";
         visible = 0;
         bitmap = $RTB::Path@"images/buttons/small/btnReport";
         text = " ";
         command = "RTBMM_FileView_Report("@getField(%line,1)@");";
      };
      %mouseCtrl.add(%reportBtn);
      %container.rpIcon = %reportBtn;
   }
}  

function Event_drag::onMouseDown(%ctrl)
{
   %offset = getWords(vectorSub(Canvas.getCursorPos(),%ctrl.drag.getCanvasPosition()),0,1);
   beginDrag(%ctrl,%offset);
}

function beginDrag(%ctrl,%offset)
{
   if(isEventPending(%ctrl.sch))
      cancel(%ctrl.sch);
   if(!isObject(%ctrl))
      return;
      
   %ctrl.drag.position = getWords(vectorSub(Canvas.getCursorPos(),%offset),0,1);
   %ctrl.sch = schedule(10,0,"beginDrag",%ctrl,%offset);
}

function returntoplace(%ctrl,%time,%x_pos,%y_pos,%x_chng,%y_chng,%duration)
{
   if(%time $= "")
   {
      %duration = 500;
      %time = 0;
      %x_targ = "50";
      %y_targ = "50";
      %x_pos = getWord(%ctrl.drag.position,0);
      %y_pos = getWord(%ctrl.drag.position,1);
      %x_chng = firstWord(vectorSub(%x_targ,%x_pos));
      %y_chng = firstWord(vectorSub(%y_targ,%y_pos));
   }

   %new_x = mfloor(Anim_EaseInOut(%time,%x_pos,%x_chng,%duration));
   %new_y = mfloor(Anim_EaseInOut(%time,%y_pos,%y_chng,%duration));
   //return;
   %ctrl.drag.position = %new_x SPC %new_y;
   
   //if(%new_x $= 50 && %new_y $= 50)
     // return;   
   if(%time > %duration)
   {
      return;
   }
   
   $sch = schedule(10,0,"returntoplace",%ctrl,%time+10,%x_pos,%y_pos,%x_chng,%y_chng,%duration);
}

function Event_drag::onMouseLeave(%ctrl)
{

}

function Event_drag::onMouseUp(%ctrl)
{
   cancel(%ctrl.sch);
   returntoplace(%ctrl);
}

//- Event_fileSelect::onMouseEnter (MouseEnter Callback)
function Event_fileSelect::onMouseEnter(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount()&&(%i<%ctrl.recurseHover||!%ctrl.recurseHover);%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.visible = 1;
   }
   %container.dlIcon.setVisible(1);
   %container.rpIcon.setVisible(1);
}

//- Event_fileSelect::onMouseLeave (MouseLeave Callback)
function Event_fileSelect::onMouseLeave(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount()&&(%i<%ctrl.recurseHover||!%ctrl.recurseHover);%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.visible = 0;
      %swatch.color = "255 255 255 100";
   }
   %container.dlIcon.setVisible(0);
   %container.rpIcon.setVisible(0);
}

//- Event_fileSelect::onMouseDown (MouseDown Callback)
function Event_fileSelect::onMouseDown(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount()&&(%i<%ctrl.recurseHover||!%ctrl.recurseHover);%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.color = "255 230 230 150";
   }
}

//- Event_fileSelect::onMouseUp (MouseUp Callback)
function Event_fileSelect::onMouseUp(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount()&&(%i<%ctrl.recurseHover||!%ctrl.recurseHover);%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.color = "255 255 255 100";
   }   
   RTBMM_FileView_Init(%ctrl.fileID);
}

//- RTBMM_SectionView_onReplyStop (Reply)
function RTBMM_SectionView_onReplyStop(%tcp)
{
   if($RTB::CModManager::Cache::ElementsAdded <= 0)
   {
      RTBMM_GUI_createMessage("<br>There are no files in this section.<br>");
   }
      
   RTBMM_GUI_createFooter(1);
}

//- RTBMM_SectionView_onFail (Failure)
%RTBMM_SB.registerFailHandler("GETSECTION","RTBMM_SectionView_onFail");
function RTBMM_SectionView_onFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//*********************************************************
//* File View
//*********************************************************
//- RTBMM_FileView_Init (Entrance)
function RTBMM_FileView_Init(%id)
{
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("GETFILE",1,%id);
   RTBMM_Zones_Track("FileView","RTBMM_FileView_Init(\""@%id@"\");");
}

//- RTBMM_FileView_onReplyStart (Start)
function RTBMM_FileView_onReplyStart(%tcp)
{
   RTBMM_GUI_Init();
}

//- RTBMM_FileView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETFILE","RTBMM_FileView_onReply");
function RTBMM_FileView_onReply(%tcp,%line)
{
   if(getField(%line,0) $= "HEAD")
   {
      RTBMM_GUI_createSplitHeader("cell1","100","<color:FAFAFA><just:left><font:Impact:18>  "@getField(%line,1));
   }
   else if(getField(%line,0) $= "DEFS")
   {
      $RTB::CModManager::Cache::CurrentFile = getField(%line,1);
   }
   else if(getField(%line,0) $= "FILE")
   {
      RTBMM_SectionView_onReply(%tcp,%line);
   }
   else if(getField(%line,0) $= "INFO")
   {
      %info = RTBMM_GUI_createContent(1,30,20,80);
      %label = %info.getObject(0);
      %content = %info.getObject(1);
      
      %label_txt = new GuiMLTextCtrl()
      {
         position = "6 7";
         extent = "128 30";
         text = "<font:Arial Bold:15><color:444444>"@getField(%line,1);
      };
      %label.add(%label_txt);
      
      %content_txt = new GuiMLTextCtrl()
      {
         position = "6 9";
         extent = "530 30";
         text = "<font:Verdana:12><color:666666>"@strTrim(RTBMM_parseBBCode(getField(%line,2)));
      };
      %content.add(%content_txt);
      %content_txt.forceReflow();
      %extent = getWord(%content_txt.extent,1);
      %info.resize(getWord(%info.position,0),getWord(%info.position,1),getWord(%info.extent,0),%extent+18);
      RTBMM_GUI_AutoResize();
   }
   else if(getField(%line,0) $= "RATE")
   {
      %info = RTBMM_GUI_createContent(1,30,20,80);
      %label = %info.getObject(0);
      %content = %info.getObject(1);
      
      %label_txt = new GuiMLTextCtrl()
      {
         position = "6 7";
         extent = "128 30";
         text = "<font:Arial Bold:15><color:444444>Rating:";
      };
      %label.add(%label_txt);
      
      %rating = getSubStr(getField(%line,1),0,5);
      %rating = RTBMM_GUI_createRatingSwatch(%rating);
      %content.add(%rating);
      RTBMM_GUI_CenterVert(%rating);
      
      %numRatings = getSubStr(getField(%line,1),6,strLen(getField(%line,1)));
      %s = (%numRatings == 1) ? "" : "s";
      %mlTextCtrl = new GuiMLTextCtrl()
      {
         position = "95 0";
         extent = "88";
         text = "<just:left><font:Arial:12><color:AAAAAA>"@%numRatings@" rating"@%s;
      };
      %content.add(%mlTextCtrl);
      %mlTextCtrl.forceReflow();
      RTBMM_GUI_CenterVert(%mlTextCtrl);
      %mlTextCtrl.shift(0,1);
      
      $RTB::CModManager::Cache::FileRatingSwatch = %rating;
      $RTB::CModManager::Cache::FileRatingText = %mlTextCtrl;
   }
   else if(getField(%line,0) $= "SCREENSHOTS" && $RTB::Options::MM::DownloadScreenshots)
   {
      RTBMM_GUI_createSplitHeader("cell1","100","<color:FAFAFA><just:left><font:Impact:18>  Screenshots");
      
      %collage = getField(%line,9);
      %container = RTBMM_GUI_createContent(1,140,100).getObject(0);
      
      %sCount = 0;
      %screenMask = (getField(%line,1) !$= "")@(getField(%line,2) !$= "")@(getField(%line,3) !$= "")@(getField(%line,4) !$= "");
      for(%i=0;%i<4;%i++)
      {
         if(getSubStr(%screenMask,%i,1) $= "1")
         {
            %s[%sCount] = %i+1;
            %sCount++;
         }
      }
      
      %ssCon = new GuiSwatchCtrl()
      {
         position = "0 0";
         extent = (118*%sCount)+(%sCount*40)-40 SPC "90";
         color = "0 0 0 0";
      };
      %container.add(%ssCon);
      RTBMM_GUI_Center(%ssCon,%container);
      
      for(%i=0;%i<%sCount;%i++)
      {
         %ss = new GuiSwatchCtrl()
         {
            position = (118*%i)+(%i*40) SPC 0;
            extent = "118 90";
            color = "100 100 100 255";
            
            new GuiSwatchCtrl()
            {
               position = "1 1";
               extent = "116 88";
               color = "255 255 255 255";
               
               new GuiBitmapCtrl()
               {
                  position = "2 2";
                  extent = "112 84";
                  bitmap = "add-ons/system_blockosystem/images/image_checker";
                  wrap = 1;
                  
                  new GuiSwatchCtrl()
                  {
                     position = "0 0";
                     extent = "112 84";
                     color = "255 255 255 200";
                  };
               };
            };
         };
         %ssCon.add(%ss);
         
         $RTB::CModManager::Cache::Screen[%i] = %s[%i];
         $RTB::CModManager::Cache::ScreenControl[%i] = %ss.getObject(0);
         $RTB::CModManager::Cache::ScreenURL[%i] = getField(%line,%i+1);
         $RTB::CModManager::Cache::ScreenCaption[%i] = getField(%line,5+%i);
      }
      $RTB::CModManager::Cache::ScreenCount = %sCount;
      
      if($RTB::CModManager::PCache::CollageSHA $= getField(%line,9))
      {
         for(%i=0;%i<$RTB::CModManager::Cache::ScreenCount;%i++)
         {
            %id = $RTB::CModManager::Cache::Screen[%i];
            %ctrl = $RTB::CModManager::Cache::ScreenControl[%i];
            
            %img = new GuiSwatchCtrl()
            {
               position = "2 2";
               extent = "112 84";
               color = "255 255 255 255";
               
               new GuiBitmapCtrl()
               {
                  position = "0 0";
                  extent = "224 168";
                  bitmap = "config/client/rtb/cache/collage.png";
               };

               new GuiSwatchCtrl()
               {
                  position = "0 0";
                  extent = "112 84";
                  color = "255 255 255 255";
               };
            };
            %ctrl.clear();
            %ctrl.add(%img);
            
            %swatch = new GuiSwatchCtrl()
            {
               position = "0 0";
               extent = "112 84";
               color = "255 255 255 0";
            };
            %img.add(%swatch);
            
            %mouseCtrl = new GuiMouseEventCtrl()
            {
               position = "0 0";
               extent = "112 84";
               
               eventType = "screenshotSelect";
               eventCallbacks = "1111";
               
               screenID = %i;
               screenCaption = $RTB::CModManager::Cache::ScreenCaption[%i];
               swatch = %swatch;
            };
            %img.add(%mouseCtrl);
            
            if(%i $= 1)
               %img.getObject(0).position = "-112 0";
            else if(%i $= 2)
               %img.getObject(0).position = "0 -84";
            else if(%i $= 3)
               %img.getObject(0).position = "-112 -84";
               
            RTBMM_GUI_FadeOut(%img.getObject(1));
         }
      }
      else
      {
         RTBMM_ScreenGrabber.getCollage("/uploads/images/"@%collage@".png");
         $RTB::CModManager::PCache::CollageSHA = %collage;
      }
   }
   else if(getField(%line,0) $= "OPTS")
   {
      %content = RTBMM_GUI_createContent(1,48,45,55);
      %blurb = %content.getObject(0);
      %buttons = %content.getObject(1);
      
      if(getField(%line,1) $= 0)
      {
         %bitmap = "image_skull";
      }
      else if(getField(%line,1) $= 1)
      {
         %bitmap = "image_medal";
         %text = "This file has been approved by our moderators.<br>This means it appears to be safe to use.";
      }
      else if(getField(%line,1) $= 2)
      {
         %bitmap = "image_pirate";
      }
         
      %bmp = new GuiBitmapCtrl()
      {
         position = "15 7";
         extent = "32 32";
         bitmap = "./images/"@%bitmap;
      };
      %blurb.add(%bmp);
      
      %txt = new GuiMLTextCtrl()
      {
         position = "40 9";
         extent = "272 24";
         text = "<just:center><font:Verdana:12><color:444444>"@%text;
      };
      %blurb.add(%txt);
      RTBMM_GUI_CenterVert(%bmp);
      RTBMM_GUI_CenterVert(%txt);
      
      %swatch = new GuiSwatchCtrl()
      {
         position = "0 0";
         extent = "82 25";
         color = "0 0 0 0";
      };
      %currExtent = 182;
      %currPos = 90;
      
      %download = new GuiBitmapButtonCtrl()
      {
         position = "0 0";
         extent = "82 25";
         bitmap = "./images/buttons/large/gray/btnDownload";
         text = " ";
         command = "RTBMM_FileView_Download();";
      };
      %swatch.add(%download);
      
      if(!$RTB::Barred::Rate)
      {
         %rate = new GuiBitmapButtonCtrl()
         {
            position = %currPos@" 0";
            extent = "82 25";
            bitmap = "./images/buttons/large/gray/btnAddRating";
            text = " ";
            command = "RTBMM_FileView_Rate();";
         };
         %swatch.add(%rate);
         %swatch.extent = %currExtent-8 SPC 25;
         %currExtent += 90;
         %currPos += 90;
      }
      
      if(!$RTB::Barred::Comment)
      {
         %comment = new GuiBitmapButtonCtrl()
         {
            position = %currPos@" 0";
            extent = "82 25";
            bitmap = "./images/buttons/large/gray/btnComment";
            text = " ";
            command = "RTBMM_FileView_openCommentBox();";
         };
         %swatch.add(%comment);
         %swatch.extent = %currExtent-8 SPC 25;
         %currExtent += 90;
         %currPos += 90;
      }
      
      if($RTB::CModManager::Session::LoggedIn)
      {
         %report = new GuiBitmapButtonCtrl()
         {
            position = %currPos@" 0";
            extent = "82 25";
            bitmap = "./images/buttons/large/gray/btnReport";
            text = " ";
            command = "RTBMM_FileView_Report();";
         };
         %swatch.add(%report);
         %swatch.extent = %currExtent-8 SPC 25;
      }
      
      %buttons.add(%swatch);
      RTBMM_GUI_Center(%swatch);
   }
   else if(getField(%line,0) $= "COMMENTS")
   {
      if(getField(%line,1) <= 0)
      {
         $RTB::CModManager::Cache::FileFooter = RTBMM_GUI_createHeader();
         return;
      }

      $RTB::CModManager::Cache::Comments = getField(%line,1);
      $RTB::CModManager::Cache::CommentPages = getField(%line,2);
      $RTB::CModManager::Cache::CommentCurrPage = 1;
      $RTB::CModManager::Cache::CommentHeader = RTBMM_GUI_createSplitHeader("cell1","100","<color:FAFAFA><just:left><font:Impact:18>  Comments <font:Arial:12>"@getField(%line,1)@" comment"@((getField(%line,1) == 1) ? "" : "s"));
      
      %container = new GuiSwatchCtrl()
      {
         position = "0 "@$RTB::CModManager::GUI::CurrentY;
         extent = "680 10";
         color = "255 0 0 255";
      };
      RTBMM_GUI_PushControl(%container,1);
      
      $RTB::CModManager::Cache::CommentContainer = %container;
   }
   else if(getField(%line,0) $= "COMMENT")
   {
      RTBMM_FileView_createComment(getField(%line,1),getField(%line,2),getField(%line,3),getField(%line,4),getField(%line,5),getField(%line,6));
      $RTB::CModManager::Cache::CommentContainer.extent = "680" SPC $RTB::CModManager::Cache::CommentContainer.getLowestPoint();
      RTBMM_GUI_AutoResize();
   }
   else if(getField(%line,0) $= "ENDCOMMENTS")
   {
      $RTB::CModManager::Cache::CommentFooter = RTBMM_GUI_createHeader(2," ");
      RTBMM_FileView_drawCommentPagination();
   }
}

//- RTBMM_FileView_drawCommentPagination (i am not proud of this horrible, horrible hack)
function RTBMM_FileView_drawCommentPagination()
{
   if(isObject($RTB::CModManager::Cache::CommentHeader.pagination))
      $RTB::CModManager::Cache::CommentHeader.pagination.delete();
   if(isObject($RTB::CModManager::Cache::CommentFooter.pagination))
      $RTB::CModManager::Cache::CommentFooter.pagination.delete();

   if($RTB::CModManager::Cache::CommentPages > 1)
   {
      if($RTB::CModManager::Cache::CommentCurrPage < $RTB::CModManager::Cache::CommentPages)
      {
         %text = "<font:Verdana Bold:13><a:comment-pagination-prev>Previous</a>";
      }
      if($RTB::CModManager::Cache::CommentCurrPage > 1)
      {
         if(%text $= "")
            %text = "<font:Verdana Bold:13><a:comment-pagination-next>Next</a> ";
         else
            %text = "<font:Verdana Bold:13><a:comment-pagination-next>Next</a> | <a:comment-pagination-prev>Previous</a>";
      }
      
      %ctrl = new GuiMLTextCtrl()
      {
         profile = RTBMM_PaginationProfile;
         position = "477 7";
         extent = "200 13";
         text = "<just:right>"@%text;
      };
      $RTB::CModManager::Cache::CommentHeader.add(%ctrl);
      $RTB::CModManager::Cache::CommentHeader.pagination = %ctrl;

      %ctrl = new GuiMLTextCtrl()
      {
         profile = RTBMM_PaginationProfile;
         position = "477 7";
         extent = "200 13";
         text = "<just:right>"@%text;
      };
      $RTB::CModManager::Cache::CommentFooter.add(%ctrl);
      $RTB::CModManager::Cache::CommentFooter.pagination = %ctrl;      
   }
}

//- RTBMM_FileView_openCommentBox (opens a comment box for users to post comments)
function RTBMM_FileView_openCommentBox()
{
   %window = RTBMM_GUI_createWindow("Post a Comment");
   %window.resize(0,0,350,200);
   RTBMM_GUI_Center(%window);
   
   %background = new GuiSwatchCtrl()
   {
      position = "5 5";
      extent = "318 120";
      color = "200 200 200 255";
      
      new GuiSwatchCtrl()
      {
         position = "1 1";
         extent = "316 118";
         color = "255 255 255 255";
      };
   };
   %window.canvas.add(%background);
   
   %textedit = new GuiScrollCtrl()
   {
      profile = RTBMM_ScrollProfile;
      position = "0 5";
      extent = "337 120";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOn";
      childMargin = "8 1";
      
      new GuiMLTextEditCtrl(RTBMM_FileView_Comment)
      {
         profile = RTBMM_MLEditProfile;
         position = "3 1";
         extent = "310 10";
         lineSpacing = 2;
         allowColorChars = 0;
         maxChars = "-1";
         maxBitmapHeight = "-1";
      };
   };
   %window.canvas.add(%textedit);
   
   %textedit.getObject(0).makeFirstResponder(1);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "280 133";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnOK";
      command = "RTBMM_FileView_sendComment("@%window@");";
   };
   %window.canvas.add(%button);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "220 133";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnCancel";
      command = %window.closeButton.command;
   };
   %window.canvas.add(%button);
}

//- RTBMM_FileView_sendComment (sends a comment for a file)
function RTBMM_FileView_sendComment(%window)
{
   if(RTBMM_FileView_Comment.getValue() $= "")
   {
      RTBMM_GUI_createMessageBoxOK("Ooops","<just:center><br>You have not entered a comment.<br>");
      return;
   }
   
   %overlay = new GuiSwatchCtrl()
   {
      position = "0 0";
      extent = %window.canvas.extent;
      color = "255 255 255 200";
   };
   %window.canvas.add(%overlay);
   
   %bitmap = new GuiAnimatedBitmapCtrl()
   {
      extent = "31 31";
      bitmap = "./images/image_loadRing";
      framesPerSecond = 15;
      numFrames = 8;
   };
   %overlay.add(%bitmap);
   RTBMM_GUI_Center(%bitmap);
   %bitmap.shift(0,-10);
   
   %mlCtrl = new GuiMLTextCtrl()
   {
      extent = "200 0";
      text = "<just:center><font:Verdana:12><color:666666>Submitting Comment...";
   };
   %overlay.add(%mlCtrl);
   RTBMM_GUI_Center(%mlCtrl);
   %mlCtrl.shift(0,12);
   
   $RTB::CModManager::Cache::LoadRing = %bitmap;
   $RTB::CModManager::Cache::LoadText = %mlCtrl;
   
   RTBMM_SendRequest("POSTCOMMENT",2,$RTB::CModManager::Cache::CurrentFile,RTBMM_FileView_Comment.getValue());
}

//- RTBMM_FileView_onCommentPosted (a callback when the comment is sent)
%RTBMM_SB.registerResponseHandler("POSTCOMMENT","RTBMM_FileView_onCommentPosted");
function RTBMM_FileView_onCommentPosted(%tcp,%line)
{
   RTBMM_GUI_LoadRing_Clear($RTB::CModManager::Cache::LoadRing);
   if(getField(%line,0) $= 1)
   {
      if($RTB::CModManager::Cache::Comments <= 0)
      {
         RTBMM_Zones_Refresh();
         return;
      }
      
      $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Your comment has been submitted.");
      RTBMM_FileView_getCommentPage(1);
   }
   else
   {
      $RTB::CModManager::Cache::LoadRing.setBitmap($RTB::Path@"images/image_loadRingFail");
      if(getField(%line,1) $= 1)
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>You are not logged in!");
      else if(getField(%line,1) $= 2)
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>File not found.");
      else if(getField(%line,1) $= 3)
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>You did not write a comment.");
      else
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Unknown Error Occurred.");
   }
}

//- RTBMM_FileView_onCommentFail (Fail result from sending a report)
%RTBMM_SB.registerFailHandler("SENDCOMMENT","RTBMM_FileView_onCommentFail");
function RTBMM_FileView_onCommentFail(%tcp,%fail)
{
   RTBMM_GUI_LoadRing_Clear($RTB::CModManager::Cache::LoadRing);
   $RTB::CModManager::Cache::LoadRing.setBitmap($RTB::Path@"images/image_loadRingFail");
   $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Connection Failed. Please try again later.");
}

//- RTBMM_FileView_getCommentPage (gets a page of comments for display)
function RTBMM_FileView_getCommentPage(%page)
{
   RTBMM_FileView_collapseComments();
   
   if(isObject($RTB::CModManager::Cache::CommentHeader.pagination))
      $RTB::CModManager::Cache::CommentHeader.pagination.delete();
   if(isObject($RTB::CModManager::Cache::CommentFooter.pagination))
      $RTB::CModManager::Cache::CommentFooter.pagination.delete();
      
   $RTB::CModManager::Cache::CommentCurrPage = %page;
   
   RTBMM_SendRequest("GETCOMMENTPAGE",2,$RTB::CModManager::Cache::CurrentFile,%page);
}

//- RTBMM_FileView_onCommentReply (handler for comment replies)
%RTBMM_SB.registerResponseHandler("GETCOMMENTPAGE","RTBMM_FileView_onCommentReply");
function RTBMM_FileView_onCommentReply(%tcp,%line)
{
   if(getField(%line,0) $= "COMMENT")
   {
      RTBMM_FileView_createComment(getField(%line,1),getField(%line,2),getField(%line,3),getField(%line,4),getField(%line,5),getField(%line,6));
      $RTB::CModManager::Cache::CommentContainer.extent = "680" SPC $RTB::CModManager::Cache::CommentContainer.getLowestPoint();
   }
}

//- RTBMM_FileView_onCommentReplyStop (callback when comments are done downloading)
function RTBMM_FileView_onCommentReplyStop()
{
   RTBMM_GUI_FadeOut($RTB::CModManager::Cache::CommentCover);
   $RTB::CModManager::Cache::CommentCover.clear();
   RTBMM_FileView_sizeupComments(0,120,getWord($RTB::CModManager::Cache::CommentContainer.extent,1)-120,1000);
   
   RTBMM_FileView_drawCommentPagination();
}

//- RTBMM_FileView_createComment (creates content container for comments)
function RTBMM_FileView_createComment(%author,%title,%comments,%blid,%date,%message)
{
   %container = RTBMM_GUI_createContent(1,120,25,75);
   %info = %container.getObject(0);
   %content = %container.getObject(1);      

   %author = new GuiMLTextCtrl()
   {
      position = "6 7";
      extent = "128 30";
      text = "<font:Arial Bold:15><color:666666>"@%author;
   };
   %info.add(%author);

   %title = new GuiMLTextCtrl()
   {
      position = "5 23";
      extent = "128 30";
      text = "<font:Verdana:12><color:888888>"@%title;
   };
   %info.add(%title);

   %data = new GuiMLTextCtrl()
   {
      position = "5 53";
      extent = "128 30";
      text = "<font:Verdana Bold:12><color:888888>Comments: <font:Verdana:12>"@%comments;
   };
   %info.add(%data);

   %data = new GuiMLTextCtrl()
   {
      position = "5 66";
      extent = "128 30";
      text = "<font:Verdana Bold:12><color:888888>Blockland ID: <font:Verdana:12>"@%blid;
   };
   %info.add(%data);

   %date = new GuiMLTextCtrl()
   {
      position = "7 7";
      extent = "300 30";
      text = "<font:Verdana:12><color:AAAAAA>\xBB "@%date;
   };
   %content.add(%date);

   %div = new GuiSwatchCtrl()
   {
      position = "2 25";
      extent = "505 1";
      minExtent = "1 1";
      color = "255 255 255 255";
   };
   %content.add(%div);

   %div = new GuiSwatchCtrl()
   {
      position = "2 24";
      extent = "505 1";
      minExtent = "1 1";
      color = "200 200 200 255";
   };
   %content.add(%div);

   %comment = new GuiMLTextCtrl()
   {
      position = "6 31";
      extent = "500 30";
      text = "<font:Verdana:12><color:666666>"@%message;
   };
   %content.add(%comment);
   
   %comment.forceReflow();
   if((getWord(%comment.extent,1)+50) > 120)
      %container.resize(getWord(%container.position,0),getWord(%container.position,1),getWord(%container.extent,0),getWord(%comment.extent,1)+50);

   %container.vertSizing = "top";
   %container.position = "0" SPC $RTB::CModManager::Cache::CommentContainer.getLowestPoint();
   $RTB::CModManager::Cache::CommentContainer.add(%container);
   
   return %container;
}

//- RTBMM_FileView_collapseComments (creates a loading window over the comments area and collapses it)
function RTBMM_FileView_collapseComments()
{
   %container = $RTB::CModManager::Cache::CommentContainer;
   
   %cover = new GuiSwatchCtrl()
   {
      position = %container.position;
      extent = %container.extent;
      color = "255 255 255 0";
      
      new GuiAnimatedBitmapCtrl()
      {
         vertSizing = "center";
         horizSizing = "center";
         extent = "31 31";
         bitmap = "./images/image_loadRing";
         framesPerSecond = 15;
         numFrames = 8;
      };
   };
   RTBMM_GUI_PushControl(%cover);
   
   RTBMM_GUI_FadeIn(%cover);
   $RTB::CModManager::Cache::CommentCover = %cover;
   RTBMM_FileView_sizedownComments(0,getWord(%container.extent,1),"-"@getWord(%container.extent,1)-120,100);
}

//- RTBMM_FileView_sizedownComments (resizes the comments area to go real small)
function RTBMM_FileView_sizedownComments(%time,%begin,%change,%duration)
{
   if(%time $= %duration)
   {
      $RTB::CModManager::Cache::CommentCover.color = "255 255 255 255";
      $RTB::CModManager::Cache::CommentContainer.clear();
      return;
   }
   
   %ctrl_con = $RTB::CModManager::Cache::CommentContainer;   
   %oldExtent = getWord(%ctrl_con.extent,1);
   %newExtent = mceil(Anim_EaseInOut(%time,%begin,%change,%duration));
   %ctrl_con.resize(getWord(%ctrl_con.position,0),getWord(%ctrl_con.position,1),getWord(%ctrl_con.extent,0),%newExtent);
   %ctrl_con.expColSch = schedule(10,0,"RTBMM_FileView_sizedownComments",%time+10,%begin,%change,%duration);
   
   %ctrl = $RTB::CModManager::Cache::CommentFooter;
   %ctrl.position = vectorAdd(%ctrl.position,"0" SPC %newExtent-%oldExtent);
   
   %ctrl = $RTB::CModManager::Cache::CommentCover;
   %ctrl.resize(getWord(%ctrl_con.position,0),getWord(%ctrl_con.position,1),getWord(%ctrl_con.extent,0),getWord(%ctrl_con.extent,1));
   RTBMM_GUI_AutoResize();
}

//- RTBMM_FileView_sizeupComments (resizes the comments area to go bigger)
function RTBMM_FileView_sizeupComments(%time,%begin,%change,%duration)
{
   if(%time $= %duration)
   {
      $RTB::CModManager::Cache::CommentCover.delete();
      return;
   }
   
   %ctrl = $RTB::CModManager::Cache::CommentContainer;   
   %oldExtent = getWord(%ctrl.extent,1);
   %newExtent = mceil(Anim_EaseInOut(%time,%begin,%change,%duration));
   %ctrl.resize(getWord(%ctrl.position,0),getWord(%ctrl.position,1),getWord(%ctrl.extent,0),%newExtent);
   %ctrl.expColSch = schedule(10,0,"RTBMM_FileView_sizeupComments",%time+10,%begin,%change,%duration);
   
   %ctrlF = $RTB::CModManager::Cache::CommentFooter;
   %ctrlF.position = vectorAdd(%ctrl.position,"0" SPC %newExtent);
   RTBMM_GUI_AutoResize();
}

//- Event_screenshotSelect::onMouseEnter (MouseEnter Callback)
function Event_screenshotSelect::onMouseEnter(%ctrl)
{
   %ctrl.swatch.color = "255 255 255 100";
}

//- Event_screenshotSelect::onMouseLeave (MouseLeave Callback)
function Event_screenshotSelect::onMouseLeave(%ctrl)
{
   %ctrl.swatch.color = "255 255 255 0";
}

//- Event_screenshotSelect::onMouseDown (MouseDown Callback)
function Event_screenshotSelect::onMouseDown(%ctrl)
{
   %ctrl.swatch.color = "255 255 255 150";
}

//- Event_screenshotSelect::onMouseUp (MouseUp Callback)
function Event_screenshotSelect::onMouseUp(%ctrl)
{
   %ctrl.swatch.color = "255 255 255 100";
   RTBMM_FileView_ShowScreenshot(%ctrl.screenID,%ctrl.screenCaption);
}

//- RTBMM_FileView_Download (Downloads the file)
function RTBMM_FileView_Download()
{
   %file_id = $RTB::CModManager::Cache::CurrentFile;
   
   RTBMM_TransferQueue.addItem(%file_id);
}

//- RTBMM_FileView_Rate (Opens a window to submit a rating for a file)
function RTBMM_FileView_Rate()
{
   %window = RTBMM_GUI_createWindow("Rate");
   %window.resize(0,0,200,80);
   RTBMM_GUI_Center(%window);
   
   %swatch = new GuiSwatchCtrl()
   {
      position = "0 8";
      color = "0 0 0 0";
      extent = "16 16";
   };
   
   %mlTextCtrl = new GuiMLTextCtrl()
   {
      position = "0 28";
      extent = "200";
      text = "<just:center><font:Arial:12><color:AAAAAA>Click a star to rate";
   };
   %window.canvas.add(%mlTextCtrl);
   %mlTextCtrl.forceReflow();
   RTBMM_GUI_CenterHoriz(%mlTextCtrl);
   
   $RTB::CModManager::Cache::RatingText = %mlTextCtrl;
   
   for(%i=0;%i<5;%i++)
   {
      %star = new GuiBitmapCtrl()
      {
         position = (20*%i) SPC 0;
         extent = "16 16";
         bitmap = "./images/icon_star0";
      };
      %swatch.add(%star);
      %swatch.extent = getWord(%star.position,0)+16;
      
      %mouseEvent = new GuiMouseEventCtrl()
      {
         position = vectorSub(%star.position,"2 2");
         extent = "20 20";
         
         eventType = "rateStarSelect";
         eventCallbacks = "1101";
         
         text = %mlTextCtrl;
         id = %i+1;
         starSwatch = %swatch;
      };
      %swatch.add(%mouseEvent);
   }
   %window.canvas.add(%swatch);
   
   RTBMM_GUI_CenterHoriz(%swatch);
}

//- Event_rateStarSelect::onMouseEnter (MouseEnter Callback)
function Event_rateStarSelect::onMouseEnter(%ctrl)
{
   %swatch = %ctrl.starSwatch;
   
   for(%i=0;%i<%swatch.getCount();%i++)
   {
      if(%swatch.getObject(%i).getClassName() !$= "GuiBitmapCtrl")
         continue;
         
      if(%numDone $= %ctrl.id)
         break;
      
      %numDone++;
      %swatch.getObject(%i).setBitmap($RTB::Path@"images/icon_star4");
   }
}

//- Event_rateStarSelect::onMouseLeave (MouseLeave Callback)
function Event_rateStarSelect::onMouseLeave(%ctrl)
{
   %swatch = %ctrl.starSwatch;
   
   for(%i=0;%i<%swatch.getCount();%i++)
   {
      if(%swatch.getObject(%i).getClassName() !$= "GuiBitmapCtrl")
         continue;

      %swatch.getObject(%i).setBitmap($RTB::Path@"images/icon_star0");
   }
}

//- Event_rateStarSelect::onMouseUp (MouseUp Callback)
function Event_rateStarSelect::onMouseUp(%ctrl)
{
   %swatch = %ctrl.starSwatch;
   
   for(%i=0;%i<%swatch.getCount();%i++)
   {
      if(%swatch.getObject(%i).getClassName() !$= "GuiBitmapCtrl")
         continue;

      if(%numDone $= %ctrl.id)
         break;
      
      %numDone++;
      %swatch.getObject(%i).setBitmap($RTB::Path@"images/icon_starS");
   }
   %ctrl.text.setValue("<just:center><font:Arial:12><color:AAAAAA>Submitting your rating of "@%ctrl.id@"...");
   
   RTBMM_SendRequest("ADDRATING",3,$RTB::CModManager::Cache::CurrentFile,%ctrl.id);
   
   for(%i=0;%i<%swatch.getCount();%i++)
   {
      if(%swatch.getObject(%i).getClassName() !$= "GuiBitmapCtrl")
      {
         %swatch.getObject(%i).delete();
         %i--;
      }
   }
}

//- RTBMM_FileView_onRatingReply (Reply from server when submitting a rating)
%RTBMM_SB.registerResponseHandler("ADDRATING","RTBMM_FileView_onRatingReply");
function RTBMM_FileView_onRatingReply(%tcp,%line)
{
   %text = $RTB::CModManager::Cache::RatingText;
   %success = getField(%line,0);
   if(%success)
   {
      %swatch = $RTB::CModManager::Cache::FileRatingSwatch;
      if(isObject(%swatch))
      {
         %rating = getSubStr(getField(%line,2),0,5);
         %rating = RTBMM_GUI_createRatingSwatch(%rating);
         %swatch.getGroup().add(%rating);
         RTBMM_GUI_CenterVert(%rating);
         %swatch.delete();
         
         $RTB::CModManager::Cache::FileRatingSwatch = %rating;
         
         %numRatings = getSubStr(getField(%line,2),6,strLen(getField(%line,2)));
         %s = (%numRatings == 1) ? "" : "s";
         
         $RTB::CModManager::Cache::FileRatingText.setValue("<just:left><font:Arial:12><color:AAAAAA>"@%numRatings@" rating"@%s);
      }

      if(isObject(%text))
      {
         %update = getField(%line,1);
         if(%update $= 2)
            %text.setValue("<just:center><font:Arial:12><color:AAAAAA>Your rating has been changed. Thanks!");
         else
            %text.setValue("<just:center><font:Arial:12><color:AAAAAA>Your rating has been saved. Thanks!");
      }
   }
   else
   {
      if(!isObject(%text))
         return;
         
      %error = getField(%line,1);
      if(%error $= 1)
         %text.setValue("<just:center><font:Arial:12><color:AAAAAA>You are not logged in!");
      else if(%error $= 2)
         %text.setValue("<just:center><font:Arial:12><color:AAAAAA>File not found!");
      else if(%error $= 3)
         %text.setValue("<just:center><font:Arial:12><color:AAAAAA>You can't rate your own stuff!");
   }
}

//- RTBMM_FileView_showScreenshot (Opens a window to show an enlarged screenshot)
function RTBMM_FileView_showScreenshot(%id,%captionText)
{
   %window = RTBMM_GUI_createWindow("Screenshot");
   %window.resize(0,0,500,435);
   RTBMM_GUI_Center(%window);
   
   %img = new GuiBitmapCtrl()
   {
      extent = "500 379";
      bitmap = "";
      visible = 0;
   };
   %window.canvas.add(%img);
   
   %div = new GuiSwatchCtrl()
   {
      position = "0 379";
      extent = "500 3";
      color = "189 192 195 255";
   };
   %window.canvas.add(%div);
   
   %caption = new GuiMLTextCtrl()
   {
      position = "4 385";
      extent = "800 14";
      text = "<color:777777><font:Verdana:12>"@%captionText;
   };
   %window.canvas.add(%caption);
   
   %loading = new GuiAnimatedBitmapCtrl()
   {
      extent = "31 31";
      bitmap = "./images/image_loadRing";
      bitmap = "./images/image_loadRing";
      framesPerSecond = 15;
      numFrames = 8;
   };
   %window.canvas.add(%loading);
   RTBMM_GUI_Center(%loading);
   
   %url = "/uploads/images/"@$RTB::CModManager::Cache::ScreenURL[%id]@".png";
   RTBMM_ScreenGrabber.getScreenshot(%url,%window);
}

//- RTBMM_FileView_Report (Opens a form to allow the user to report the file to Mod Reviewers)
function RTBMM_FileView_Report(%id)
{
   if(%id $= "")
      %id = $RTB::CModManager::Cache::CurrentFile;
   if(%id $= "")
      %id = 0;
      
   %window = RTBMM_GUI_createWindow("Report File");
   %window.resize(0,0,400,230);
   RTBMM_GUI_Center(%window);
   
   %label = new GuiMLTextCtrl()
   {
      position = "10 12";
      extent = "";
      text = "<font:Verdana:12><color:555555>Reason:";
   };
   %window.canvas.add(%label);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_Report_Reason)
   {
      profile = RTBMM_PopupProfile;
      position = "54 9";
      extent = "130 17";
   };
   %window.canvas.add(%popup);
   %popup.add("Stolen",1);
   %popup.add("Abusive",2);
   %popup.add("Doesn't Work",3);
   %popup.add("Breaks Stuff",4);
   %popup.add("Contains Exploits",5);
   %popup.add("Other",6);
   %popup.setSelected(1);
   
   %label = new GuiMLTextCtrl()
   {
      position = "202 12";
      extent = "";
      text = "<font:Verdana:12><color:555555>Severity:";
   };
   %window.canvas.add(%label);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_Report_Severity)
   {
      profile = RTBMM_PopupProfile;
      position = "252 9";
      extent = "130 17";
   };
   %window.canvas.add(%popup);
   %popup.add("Normal",1);
   %popup.add("High",2);
   %popup.add("Critical",3);
   %popup.setSelected(1);
   
   %label = new GuiMLTextCtrl()
   {
      position = "10 40";
      extent = "";
      text = "<font:Verdana:12><color:555555>Summary:";
   };
   %window.canvas.add(%label);
   $RTB::CModManager::Cache::Report::Summary = %label;
   
   %textedit = new GuiTextEditCtrl(RTBMM_Report_Summary)
   {
      profile = RTBMM_TextEditProfile;
      position = "66 38";
      extent = "316 16";
   };
   %window.canvas.add(%textedit);
   
   %label = new GuiMLTextCtrl()
   {
      position = "10 64";
      extent = "";
      text = "<font:Verdana:12><color:555555>Description:";
   };
   %window.canvas.add(%label);
   $RTB::CModManager::Cache::Report::Description = %label;
   
   %textedit = new GuiScrollCtrl()
   {
      profile = RTBMM_TextEditProfile;
      position = "9 82";
      extent = "373 73";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOff";
      
      new GuiMLTextEditCtrl(RTBMM_Report_Description)
      {
         profile = RTBMM_MLEditProfile;
         position = "3 1";
         extent = "364 73";
      };
   };
   %window.canvas.add(%textedit);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "324 163";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnOK";
      command = "RTBMM_FileView_SendReport("@%window@","@%id@");";
   };
   %window.canvas.add(%button);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "260 163";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnCancel";
      command = %window.closeButton.command;
   };
   %window.canvas.add(%button);
   
   %buglink = new GuiMLTextCtrl()
   {
      profile = "RTBMM_NewsContentProfile";
      position = "12 170";
      extent = "236 12";
      text = "<color:AAAAAA>Warning: Abuse will result in a ban.";
   };
   %window.canvas.add(%buglink);
}

//- RTBMM_FileView_SendReport (Processes + Sends the File Report)
function RTBMM_FileView_SendReport(%window,%id)
{
   if(RTBMM_Report_Summary.getValue() $= "")
   {
      $RTB::CModManager::Cache::Report::Summary.setValue("<font:Verdana Bold:12><color:EE0000>Summary:");
      %errors = 1;
   }
   else
      $RTB::CModManager::Cache::Report::Summary.setValue("<font:Verdana:12><color:555555>Summary:");

   if(RTBMM_Report_Description.getValue() $= "")
   {   
      $RTB::CModManager::Cache::Report::Description.setValue("<font:Verdana Bold:12><color:EE0000>Description:");
      %errors = 1;
   }
   else
      $RTB::CModManager::Cache::Report::Description.setValue("<font:Verdana:12><color:555555>Description:");
   
   if(%errors)
      return;
   
   %overlay = new GuiSwatchCtrl()
   {
      position = "0 0";
      extent = %window.canvas.extent;
      color = "255 255 255 200";
   };
   %window.canvas.add(%overlay);
   
   %bitmap = new GuiAnimatedBitmapCtrl()
   {
      extent = "31 31";
      bitmap = "./images/image_loadRing";
      framesPerSecond = 15;
      numFrames = 8;
   };
   %overlay.add(%bitmap);
   RTBMM_GUI_Center(%bitmap);
   %bitmap.shift(0,-10);
   
   %mlCtrl = new GuiMLTextCtrl()
   {
      extent = "200 0";
      text = "<just:center><font:Verdana:12><color:666666>Sending Report...";
   };
   %overlay.add(%mlCtrl);
   RTBMM_GUI_Center(%mlCtrl);
   %mlCtrl.shift(0,12);
   
   $RTB::CModManager::Cache::LoadRing = %bitmap;
   $RTB::CModManager::Cache::LoadText = %mlCtrl;
   
   RTBMM_SendRequest("SENDREPORT",3,%id,RTBMM_Report_Reason.getSelected(),RTBMM_Report_Severity.getSelected(),RTBMM_Report_Summary.getValue(),RTBMM_Report_Description.getValue());
}

//- RTBMM_FileView_onReportReply (Reply from sending a report)
%RTBMM_SB.registerResponseHandler("SENDREPORT","RTBMM_FileView_onReportReply");
function RTBMM_FileView_onReportReply(%tcp,%line)
{
   RTBMM_GUI_LoadRing_Clear($RTB::CModManager::Cache::LoadRing);
   if(getField(%line,0) $= 1)
   {
      $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Your report has been received.");
   }
   else
   {
      $RTB::CModManager::Cache::LoadRing.setBitmap($RTB::Path@"images/image_loadRingFail");
      if(getField(%line,1) $= 1)
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>You are not logged in!");
      else if(getField(%line,1) $= 2)
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>File not found.");
      else
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Unknown Error Occurred.");
   }
}

//- RTBMM_FileView_onReportFail (Fail result from sending a report)
%RTBMM_SB.registerFailHandler("SENDREPORT","RTBMM_FileView_onReportFail");
function RTBMM_FileView_onReportFail(%tcp,%fail)
{
   RTBMM_GUI_LoadRing_Clear($RTB::CModManager::Cache::LoadRing);
   $RTB::CModManager::Cache::LoadRing.setBitmap($RTB::Path@"images/image_loadRingFail");
   $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Connection Failed. Please try again later.");
}

//- RTBMM_FileView_ReportBug (Opens a form to allow the user to report a bug)
function RTBMM_FileView_ReportBug(%id)
{
   if(%id $= "")
      %id = $RTB::CModManager::Cache::CurrentFile;
   if(%id $= "")
      %id = 0;
      
   %window = RTBMM_GUI_createWindow("Report Bug");
   %window.resize(0,0,400,230);
   RTBMM_GUI_Center(%window);
   
   %label = new GuiMLTextCtrl()
   {
      position = "10 12";
      extent = "";
      text = "<font:Verdana:12><color:555555>Reason:";
   };
   %window.canvas.add(%label);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_Report_Reason)
   {
      profile = RTBMM_PopupProfile;
      position = "54 9";
      extent = "130 17";
   };
   %window.canvas.add(%popup);
   %popup.add("Stolen",1);
   %popup.add("Abusive",2);
   %popup.add("Doesn't Work",3);
   %popup.add("Breaks Stuff",4);
   %popup.add("Contains Exploits",5);
   %popup.add("Other",6);
   %popup.setSelected(1);
   
   %label = new GuiMLTextCtrl()
   {
      position = "202 12";
      extent = "";
      text = "<font:Verdana:12><color:555555>Severity:";
   };
   %window.canvas.add(%label);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_Report_Severity)
   {
      profile = RTBMM_PopupProfile;
      position = "252 9";
      extent = "130 17";
   };
   %window.canvas.add(%popup);
   %popup.add("Low",0);
   %popup.add("Medium",1);
   %popup.add("High",2);
   %popup.setSelected(0);
   
   %label = new GuiMLTextCtrl()
   {
      position = "10 40";
      extent = "";
      text = "<font:Verdana:12><color:555555>Summary:";
   };
   %window.canvas.add(%label);
   $RTB::CModManager::Cache::Report::Summary = %label;
   
   %textedit = new GuiTextEditCtrl(RTBMM_Report_Summary)
   {
      profile = RTBMM_TextEditProfile;
      position = "66 38";
      extent = "316 16";
   };
   %window.canvas.add(%textedit);
   
   %label = new GuiMLTextCtrl()
   {
      position = "10 64";
      extent = "";
      text = "<font:Verdana:12><color:555555>Description:";
   };
   %window.canvas.add(%label);
   $RTB::CModManager::Cache::Report::Description = %label;
   
   %textedit = new GuiScrollCtrl()
   {
      profile = RTBMM_TextEditProfile;
      position = "9 82";
      extent = "373 73";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOff";
      
      new GuiMLTextEditCtrl(RTBMM_Report_Description)
      {
         profile = RTBMM_MLEditProfile;
         position = "3 1";
         extent = "364 73";
      };
   };
   %window.canvas.add(%textedit);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "324 163";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnOK";
      command = "RTBMM_FileView_SendReport("@%window@","@%id@");";
   };
   %window.canvas.add(%button);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "260 163";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnCancel";
      command = %window.closeButton.command;
   };
   %window.canvas.add(%button);
   
   %buglink = new GuiMLTextCtrl()
   {
      profile = "RTBMM_NewsContentProfile";
      position = "12 170";
      extent = "236 12";
      text = "<a:reportbug>Click here to report a bug instead</a>";
   };
   %window.canvas.add(%buglink);
}

//- RTBMM_FileView_Screenshots (Opens a bigger version of the screenshot viewer to browse them all)
function RTBMM_FileView_ShowScreenshots(%selector)
{
   %window = RTBMM_GUI_createWindow("Screenshots");
   %window.resize(0,0,500,375);
   RTBMM_GUI_Center(%window);
   
   %img = new GuiBitmapCtrl()
   {
      extent = %window.canvas.extent;
      bitmap = "screenshots/BLPetrol";
   };
   %window.canvas.add(%img);
}

//- RTBMM_FileView_onReplyStop (Stop Reply)
function RTBMM_FileView_onReplyStop(%tcp)
{
}

//- RTBMM_FileView_onFail (Fail)
%RTBMM_SB.registerFailHandler("GETFILE","RTBMM_FileView_onFail");
function RTBMM_FileView_onFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//*********************************************************
//* Packs View
//*********************************************************
//- RTBMM_PacksView_Init (Entrance)
function RTBMM_PacksView_Init()
{
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("GETPACKS",1);
   RTBMM_Zones_Track("PacksView","RTBMM_PacksView_Init();");
}

//- RTBMM_PacksView_onReplyStart (Reply Start)
function RTBMM_PacksView_onReplyStart(%tcp)
{
   RTBMM_GUI_Init();
   RTBMM_GUI_createSplitHeader(2,"100","<color:FAFAFA><just:left><font:Impact:18>  Content Packs");
   RTBMM_GUI_createSplitHeader(1,58,"Pack",17,"Date",10,"Items",15,"Download");
}

//- RTBMM_PacksView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETPACKS","RTBMM_PacksView_onReply");
function RTBMM_PacksView_onReply(%tcp,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6,%arg7)
{
   if(%arg1 $= "PACK")
   {
      %container = RTBMM_GUI_createContent(1,50,6,52,17,10,15);
      
      %c_icon = %container.getObject(0);
      %c_information = %container.getObject(1);
      %c_date = %container.getObject(2);
      %c_items = %container.getObject(3);
      %c_download = %container.getObject(4);
      
      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_icon.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_icon.add(%swatch);
      %icon = new GuiBitmapCtrl()
      {
         vertSizing = "center";
         position = "0 0";
         extent = "16 16";
         bitmap = "./images/icons/"@%arg4;
      };
      %c_icon.add(%icon);
      RTBMM_GUI_Center(%icon);

      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_information.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_information.add(%swatch);
      %text = new GuiMLTextCtrl()
      {
         position = "1 3";
         extent = "327 18";
         text = "<font:Arial Bold:15><color:888888> "@%arg3;
      };
      %c_information.add(%text);
      
      %mltext = new GuiMLTextCtrl()
      {
         position = "3 18";
         extent = "325 18";
         text = "<font:Verdana:12><color:444444>"@%arg5;
      };
      %c_information.add(%mltext);
      
      %mltext.forceReflow();
      %extent = getWord(%mltext.extent,1);
      %container.resize(getWord(%container.position,0),getWord(%container.position,1),getWord(%container.extent,0),%extent+36);
      RTBMM_GUI_AutoResize();

      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_date.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_date.add(%swatch);
      %text = new GuiMLTextCtrl()
      {
         position = "0 7";
         extent = 200 SPC 18;
         text = "<just:center><font:Verdana:12><color:888888>"@%arg6;
      };
      %c_date.add(%text);
      %text.forceReflow();
      RTBMM_GUI_Center(%text);
      
      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_items.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_items.add(%swatch);
      %text = new GuiMLTextCtrl()
      {
         position = "0 7";
         extent = 150 SPC 18;
         text = "<just:center><font:Verdana:12><color:888888>"@%arg7;
      };
      %c_items.add(%text);
      %text.forceReflow();
      RTBMM_GUI_Center(%text);
      
      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_download.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_download.add(%swatch);
      %text = new GuiMLTextCtrl()
      {
         position = "0 7";
         extent = 150 SPC 18;
         text = "<just:center><font:Verdana:12><color:888888>"@%arg7;
      };
      %c_download.add(%text);
      %text.forceReflow();
      RTBMM_GUI_Center(%text);
      
      %mouseCtrl = new GuiMouseEventCtrl()
      {
         position = %container.position;
         extent = %container.extent;
         
         eventType = "packSelect";
         eventCallbacks = "1111";
         
         packID = %arg2;
         container = %container;
      };
      RTBMM_GUI_PushControl(%mouseCtrl,1);
   }
}

//- Event_packSelect::onMouseEnter (MouseEnter Callback)
function Event_packSelect::onMouseEnter(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount();%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.visible = 1;
   }
}

//- Event_packSelect::onMouseLeave (MouseLeave Callback)
function Event_packSelect::onMouseLeave(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount();%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.visible = 0;
      %swatch.color = "255 255 255 100";
   }
}

//- Event_packSelect::onMouseDown (MouseDown Callback)
function Event_packSelect::onMouseDown(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount();%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.color = "255 230 230 150";
   }
}

//- Event_packSelect::onMouseUp (MouseUp Callback)
function Event_packSelect::onMouseUp(%ctrl)
{
   %container = %ctrl.container;
   for(%i=0;%i<%container.getCount();%i++)
   {
      %parent = %container.getObject(%i);
      %swatch = %parent.getObject(0);
      %swatch.color = "255 255 255 100";
   }   
   RTBMM_PackView_Init(%ctrl.packID);
}

//- RTBMM_PacksView_onReplyStop (Stop Reply)
function RTBMM_PacksView_onReplyStop(%tcp)
{
   RTBMM_GUI_createHeader(2," ");
}

//- RTBMM_PacksView_onFail (Fail)
%RTBMM_SB.registerFailHandler("GETPACKS","RTBMM_PacksView_onFail");
function RTBMM_PacksView_onFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//*********************************************************
//* Packs View
//*********************************************************
//- RTBMM_PacksView_Init (Entrance)
function RTBMM_PackView_Init(%id)
{
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("GETPACK",1,%id);
   RTBMM_Zones_Track("PackView","RTBMM_PackView_Init("@%id@");");
   
   $RTB::CModManager::Cache::TotalItems = 0;
   $RTB::CModManager::Cache::ItemsSelected = 0;
}

//- RTBMM_PackView_onReplyStart (Reply Start)
function RTBMM_PackView_onReplyStart(%tcp)
{
   RTBMM_GUI_Init();
}

//- RTBMM_PackView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETPACK","RTBMM_PackView_onReply");
function RTBMM_PackView_onReply(%tcp,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6,%arg7,%arg8,%arg9,%arg10,%arg11)
{
   if(%arg1 $= "HEADER")
   {
      RTBMM_GUI_createSplitHeader(2,"100","<color:FAFAFA><just:left><font:Impact:18>  "@%arg2);
      if($RTB::CModManager::Cache::PackName $= "")
         $RTB::CModManager::Cache::PackName = %arg2;
   }
   else if(%arg1 $= "SPLITHEADER")
   {
      RTBMM_GUI_createSplitHeader(1,66,"File",15,"Rating",14,"Downloads",5," ");
   }
   else if(%arg1 $= "FOOTER")
   {
      RTBMM_GUI_createSplitHeader(2,"100","<color:FAFAFA><just:left><font:Impact:18>  Download Pack");
      %message = RTBMM_GUI_createMessage("<br><spush><font:Verdana Bold:12>You can download all the files in this pack by clicking the download button below.<spop><br>You can tick/untick files on the right to pick which ones you want to download.<br><br><br><br><br>");
      
      %downloadButton = new GuiBitmapButtonCtrl()
      {
         extent = "82 25";
         text = " ";
         bitmap = $RTB::Path@"images/buttons/large/gray/btnDownload";
         command = "RTBMM_PackView_Download();";
      };
      %message.add(%downloadButton);
      RTBMM_GUI_Center(%downloadButton);
      %downloadButton.shift(0,26);
      
      %itemCount = new GuiMLTextCtrl()
      {
         text = "<color:BBBBBB><font:Arial:12><just:center>"@$RTB::CModManager::Cache::ItemsSelected@" items";
      };
      %message.add(%itemCount);
      RTBMM_GUI_Center(%itemCount);
      %itemCount.shift(0,32);
      
      $RTB::CModManager::Cache::ItemCounter = %itemCount;
   }
   else if(%arg1 $= "INFO")
   {
      %info = RTBMM_GUI_createContent(1,30,20,80);
      %label = %info.getObject(0);
      %content = %info.getObject(1);
      
      %label_txt = new GuiMLTextCtrl()
      {
         position = "6 7";
         extent = "128 30";
         text = "<font:Arial Bold:15><color:444444>"@%arg2;
      };
      %label.add(%label_txt);
      
      %content_txt = new GuiMLTextCtrl()
      {
         position = "6 9";
         extent = "530 30";
         text = "<font:Verdana:12><color:666666>"@%arg3;
      };
      %content.add(%content_txt);
      %content_txt.forceReflow();
      %extent = getWord(%content_txt.extent,1);
      %info.resize(getWord(%info.position,0),getWord(%info.position,1),getWord(%info.extent,0),%extent+18);
      RTBMM_GUI_AutoResize();
   }
   else if(%arg1 $= "PACK")
   {
      %container = RTBMM_GUI_createContent(1,50,6,60,15,14,5);
      
      %c_icon = %container.getObject(0);
      %c_information = %container.getObject(1);
      %c_rating = %container.getObject(2);
      %c_downloads = %container.getObject(3);
      %c_toggle = %container.getObject(4);
      
      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_icon.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_icon.add(%swatch);
      %icon = new GuiBitmapCtrl()
      {
         vertSizing = "center";
         position = "0 0";
         extent = "16 16";
         bitmap = "./images/icons/"@%arg3;
      };
      %c_icon.add(%icon);
      if(RTBMM_FileCache.get(%arg2))
      {
         %star = new GuiBitmapCtrl()
         {
            position = "24 -2";
            extent = "16 16";
            bitmap = "./images/bullet_star";
         };
         %c_icon.add(%star);
      }
      RTBMM_GUI_Center(%icon);

      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_information.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_information.add(%swatch);
      %text = new GuiMLTextCtrl()
      {
         position = "1 3";
         extent = "376 18";
         text = "<font:Arial Bold:15><color:888888> "@%arg4;
      };
      %c_information.add(%text);
      %text = new GuiMLTextCtrl()
      {
         vertSizing = "top";
         position = "3 32";
         extent = "325 18";
         text = "<font:Verdana:12><color:666666>\xBB <color:888888>By "@%arg6;
      };
      %c_information.add(%text);
      
      %mltext = new GuiMLTextCtrl()
      {
         position = "3 18";
         extent = "376 18";
         text = "<font:Verdana:12><color:444444>"@%arg5;
      };
      %c_information.add(%mltext);
      
      %mltext.forceReflow();
      %extent = getWord(%mltext.extent,1);
      %container.resize(getWord(%container.position,0),getWord(%container.position,1),getWord(%container.extent,0),%extent+36);
      RTBMM_GUI_AutoResize();
      
      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_rating.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_rating.add(%swatch);
      %rating_swatch = RTBMM_GUI_createRatingSwatch(%arg8);
      %c_rating.add(%rating_swatch);
      RTBMM_GUI_Center(%rating_swatch);
      
      %swatch = new GuiSwatchCtrl()
      {
         vertSizing = "height";
         position = "0 0";
         extent = %c_downloads.extent;
         color = "255 255 255 100";
         visible = 0;
      };
      %c_downloads.add(%swatch);
      %text = new GuiMLTextCtrl()
      {
         position = "0 7";
         extent = 150 SPC 18;
         text = "<just:center><font:Verdana:12><color:888888>"@%arg9;
      };
      %c_downloads.add(%text);
      %text.forceReflow();
      RTBMM_GUI_Center(%text);
      
      %mouseCtrl = new GuiMouseEventCtrl()
      {
         position = %container.position;
         extent = 646 SPC restWords(%container.extent);
         
         eventType = "fileSelect";
         eventCallbacks = "1111";
         
         persistent = 1;         
         
         recurseHover = 4;         
         
         fileID = %arg2;
         container = %container;
      };
      RTBMM_GUI_PushControl(%mouseCtrl,1);
      
      %downloadBtn = new GuiBitmapButtonCtrl()
      {
         position = "430 1";
         extent = "16 16";
         visible = 0;
         bitmap = $RTB::Path@"images/buttons/small/btnDownload";
         text = " ";
         command = "RTBMM_TransferView_Add("@%arg2@");";
      };
      %mouseCtrl.add(%downloadBtn);
      %container.dlIcon = %downloadBtn;
      
      %reportBtn = new GuiBitmapButtonCtrl()
      {
         position = "430" SPC getWord(%mouseCtrl.extent,1)-19;
         extent = "16 16";
         visible = 0;
         bitmap = $RTB::Path@"images/buttons/small/btnReport";
         text = " ";
         command = "RTBMM_FileView_Report("@%arg2@");";
      };
      %mouseCtrl.add(%reportBtn);
      %container.rpIcon = %reportBtn;
      
      %checkbox = new GuiCheckboxCtrl()
      {
         profile = RTBMM_CheckBoxProfile;
         position = "9 0";
         extent = "138 50";
         text = "";
      };
      %c_toggle.add(%checkbox);
      RTBMM_GUI_CenterVert(%checkbox);
      %checkbox.command = "RTBMM_PackView_clickCheckbox("@%checkbox@","@$RTB::CModManager::Cache::TotalItems@");";

      $RTB::CModManager::Cache::Item[$RTB::CModManager::Cache::TotalItems] = %arg2;
      
      if(%arg10 !$= RTBMM_FileCache.get(%arg2).file_version || !RTBMM_FileCache.get(%arg2))
      {
         %checkbox.setValue(1);
         $RTB::CModManager::Cache::ItemDL[$RTB::CModManager::Cache::TotalItems] = 1;
         $RTB::CModManager::Cache::ItemsSelected++;
      }
      $RTB::CModManager::Cache::TotalItems++;
   }
}

//- RTBMM_PackView_clickCheckbox (Handles selections for pack downloading)
function RTBMM_PackView_clickCheckbox(%checkbox,%id)
{
   if(%checkbox.getValue() $= 0)
   {
      $RTB::CModManager::Cache::ItemsSelected--;
      $RTB::CModManager::Cache::ItemDL[%id] = 0;
   }
   else
   {
      $RTB::CModManager::Cache::ItemsSelected++;
      $RTB::CModManager::Cache::ItemDL[%id] = 1;
   }
   
   %s = ($RTB::CModManager::Cache::ItemsSelected == 1)?"":"s";
   $RTB::CModManager::Cache::ItemCounter.setValue("<color:BBBBBB><font:Arial:12><just:center>"@$RTB::CModManager::Cache::ItemsSelected@" item"@%s);
}

//- RTBMM_PackView_Download (Download selected items in this pack)
function RTBMM_PackView_Download()
{
   if($RTB::CModManager::Cache::ItemsSelected <= 0)
   {
      RTBMM_GUI_createMessageBoxOK("Hmm?","You should probably atleast tick ONE file to download!");
      return;
   }

   if(RTBMM_GroupManager.hasGroup($RTB::CModManager::Cache::PackName))
      RTBMM_GroupManager.deleteGroup($RTB::CModManager::Cache::PackName);
   %group = RTBMM_ModsView_createGroup($RTB::CModManager::Cache::PackName,1);
   for(%i=0;%i<$RTB::CModManager::Cache::TotalItems;%i++)
   {
      if($RTB::CModManager::Cache::ItemDL[%i])
      {
         RTBMM_TransferQueue.addItem($RTB::CModManager::Cache::Item[%i],0,%group);
      }
   }
}

//- RTBMM_PackView_onReplyStop (Reply stop)
function RTBMM_PackView_onReplyStop(%tcp)
{
   RTBMM_GUI_createHeader(2," ");
}

//- RTBMM_PackView_onFail (Fail)
%RTBMM_SB.registerFailHandler("GETPACK","RTBMM_PackView_onFail");
function RTBMM_PackView_onFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//*********************************************************
//* Top List View
//*********************************************************
//- RTBMM_TopListView_Init (Entrance)
function RTBMM_TopListView_Init()
{
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("GETTOPLIST",1);
   RTBMM_Zones_Track("TopListView","RTBMM_TopListView_Init();");
}

//- RTBMM_TopListView_onReplyStart (Reply Start)
function RTBMM_TopListView_onReplyStart(%tcp)
{
   RTBMM_SectionView_onReplyStart(%tcp);
}

//- RTBMM_TopListView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETTOPLIST","RTBMM_TopListView_onReply");
function RTBMM_TopListView_onReply(%tcp,%line)
{
   RTBMM_SectionView_onReply(%tcp,%line);
}

//- RTBMM_TopListView_onReplyStop (Reply Stop)
function RTBMM_TopListView_onReplyStop(%tcp)
{
   RTBMM_GUI_createFooter("cell2");
}

//- RTBMM_TopListView_onFail (Fail)
%RTBMM_SB.registerFailHandler("GETTOPLISt","RTBMM_TopListView_onFail");
function RTBMM_TopListView_onFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//*********************************************************
//* Search View
//*********************************************************
//- RTBMM_SearchView_Init (Entrance)
function RTBMM_SearchView_Init()
{
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("GETSEARCH",1);
   RTBMM_Zones_Track("SearchView","RTBMM_SearchView_Init();");
}

//- RTBMM_SearchView_onReplyStart (Reply Start)
function RTBMM_SearchView_onReplyStart(%tcp)
{
   RTBMM_GUI_Init();
   
   RTBMM_GUI_createSplitHeader(2,"100","<color:FAFAFA><just:left><font:Impact:18>  Search Query");
   %content = RTBMM_GUI_createContent(1,40,30,70);

   %mlText = new GuiMLTextCtrl()
   {
      extent = "200 25";
      text = "<font:Verdana Bold:13><color:444444>Keywords:<br><font:Verdana:12>Enter a keyword to search for.";
   };
   %content.getObject(0).add(%mlText);
   RTBMM_GUI_Center(%mlText);
   %mlText.shift(3,0);
   
   %textEdit = new GuiTextEditCtrl(RTBMM_SearchView_Keyword)
   {
      profile = "RTBMM_TextEditProfile";
      extent = "200 20";
      altCommand = "RTBMM_SearchView_doSearch();";
   };
   %content.getObject(1).add(%textEdit);
   RTBMM_GUI_CenterVert(%textEdit);
   %textEdit.shift(10,0);
   %textEdit.schedule(1,"makeFirstResponder",1);
   
   %content = RTBMM_GUI_createContent(1,40,30,70);
   
   %mlText = new GuiMLTextCtrl()
   {
      extent = "200 25";
      text = "<font:Verdana Bold:13><color:444444>Author:<br><font:Verdana:12>Enter a username to search for.";
   };
   %content.getObject(0).add(%mlText);
   RTBMM_GUI_Center(%mlText);
   %mlText.shift(3,0);
   
   %textEdit = new GuiTextEditCtrl(RTBMM_SearchView_Author)
   {
      profile = "RTBMM_TextEditProfile";
      extent = "200 20";
      altCommand = "RTBMM_SearchView_doSearch();";
   };
   %content.getObject(1).add(%textEdit);
   RTBMM_GUI_CenterVert(%textEdit);
   %textEdit.shift(10,0);
   
   RTBMM_GUI_createSplitHeader(2,"100","<color:FAFAFA><just:left><font:Impact:18>  Search Options");
   
   %content = RTBMM_GUI_createContent(1,40,25,25,25,25);
   
   %mlText = new GuiMLTextCtrl()
   {
      extent = "165 25";
      text = "<font:Verdana Bold:13><color:444444>Section:<br><font:Verdana:12>Select a section to search in.";
   };
   %content.getObject(0).add(%mlText);
   RTBMM_GUI_Center(%mlText);
   %mlText.shift(3,0);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_SearchView_Section)
   {
      profile = "RTBMM_PopupProfile";
      extent = "140 17";
   };
   %content.getObject(1).add(%popup);
   RTBMM_GUI_Center(%popup);
   %popup.add("All",999);
   %popup.setSelected(999);
   
   %mlText = new GuiMLTextCtrl()
   {
      extent = "165 25";
      text = "<font:Verdana Bold:13><color:444444>Search Alternatives:<br><font:Verdana:12>Search different fields.";
   };
   %content.getObject(2).add(%mlText);
   RTBMM_GUI_Center(%mlText);
   %mlText.shift(3,0);
   
   %mlText = new GuiMLTextCtrl()
   {
      extent = "165 25";
      text = "<font:Verdana:12><color:444444>Search Summary";
   };
   %content.getObject(3).add(%mlText);
   RTBMM_GUI_Center(%mlText);
   %mlText.shift(25,-2);
   
   %checkbox = new GuiCheckboxCtrl(RTBMM_SearchView_SearchSummary)
   {
      profile = "RTBMM_CheckBoxProfile";
      extent = "140 16";
      text = "";
   };
   %content.getObject(3).add(%checkbox);
   RTBMM_GUI_Center(%checkbox);
   %checkbox.shift(-5,-8);
   
   %mlText = new GuiMLTextCtrl()
   {
      extent = "165 25";
      text = "<font:Verdana:12><color:444444>Search Description";
   };
   %content.getObject(3).add(%mlText);
   RTBMM_GUI_Center(%mlText);
   %mlText.shift(25,14);
   
   %checkbox = new GuiCheckboxCtrl(RTBMM_SearchView_SearchDescription)
   {
      profile = "RTBMM_CheckBoxProfile";
      extent = "140 16";
      text = "";
   };
   %content.getObject(3).add(%checkbox);
   RTBMM_GUI_Center(%checkbox);
   %checkbox.shift(-5,8);
   
   %content = RTBMM_GUI_createContent(1,40,25,25,25,25);
   
   %mlText = new GuiMLTextCtrl()
   {
      extent = "165 25";
      text = "<font:Verdana Bold:13><color:444444>Category:<br><font:Verdana:12>Select a category to search in.";
   };
   %content.getObject(0).add(%mlText);
   RTBMM_GUI_Center(%mlText);
   %mlText.shift(3,0);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_SearchView_Category)
   {
      profile = "RTBMM_PopupProfile";
      extent = "140 17";
   };
   %content.getObject(1).add(%popup);
   RTBMM_GUI_Center(%popup);
   %popup.add("All",999);
   %popup.setSelected(999);
   
   %mlText = new GuiMLTextCtrl()
   {
      extent = "165 25";
      text = "<font:Verdana Bold:13><color:444444>Sort by:<br><font:Verdana:12>Sort your results.";
   };
   %content.getObject(2).add(%mlText);
   RTBMM_GUI_Center(%mlText);
   %mlText.shift(3,0);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_SearchView_SortBy)
   {
      profile = "RTBMM_PopupProfile";
      extent = "70 17";
   };
   %content.getObject(3).add(%popup);
   RTBMM_GUI_Center(%popup);
   %popup.shift(-40,0);
   %popup.add("Name",0);
   %popup.add("Downloads",1);
   %popup.add("Rating",2);
   %popup.add("Submit Date",3);
   %popup.setSelected(0);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_SearchView_SortDir)
   {
      profile = "RTBMM_PopupProfile";
      extent = "70 17";
   };
   %content.getObject(3).add(%popup);
   RTBMM_GUI_Center(%popup);
   %popup.shift(40,0);
   %popup.add("Ascending",0);
   %popup.add("Descending",1);
   %popup.setSelected(0);
   
   %content = RTBMM_GUI_createContent(1,60,100);
   
   %button = new GuiBitmapButtonCtrl()
   {
      extent = "82 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnSearch";
      command = "RTBMM_SearchView_DoSearch();";
   };
   %content.getObject(0).add(%button);
   RTBMM_GUI_Center(%button);
   
   RTBMM_GUI_createSplitHeader("cell1","100"," ");
}

function RTBMM_SearchView_Category::onSelect(%this,%id,%name)
{
   %selection = RTBMM_SearchView_Section.getSelected();
   RTBMM_SearchView_Section.clear();
   RTBMM_SearchView_Section.add("All",999);
   
   for(%i=1;%i<$RTB::CModManager::Cache::Secs+1;%i++)
   {
      %sec = $RTB::CModManager::Cache::Sec[%i];
      if(getField(%sec,2) $= %id || %id $= "999")
         RTBMM_SearchView_Section.add(getField(%sec,0),getField(%sec,1));
   }
   
   RTBMM_SearchView_Section.setSelected(%selection);
   if(RTBMM_SearchView_Section.getSelected() == 0)
      RTBMM_SearchView_Section.setSelected(999);
}

//- RTBMM_SearchView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETSEARCH","RTBMM_SearchView_onReply");
function RTBMM_SearchView_onReply(%tcp,%line)
{
   if(getField(%line,0) $= "CAT")
   {
      $RTB::CModManager::Cache::Cat[$RTB::CModManager::Cache::Cats++] = getField(%line,2) TAB getField(%line,1);
      RTBMM_SearchView_Category.add(getField(%line,2),getField(%line,1));
   }
   else if(getField(%line,0) $= "SEC")
   {
      $RTB::CModManager::Cache::Sec[$RTB::CModManager::Cache::Secs++] = getField(%line,3) TAB getField(%line,1) TAB getField(%line,2);
      RTBMM_SearchView_Section.add(getField(%line,3),getField(%line,1));
   }
}

//- RTBMM_SearchView_onReplyStop (Reply Stop)
function RTBMM_SearchView_onReplyStop(%tcp)
{
}

//- RTBMM_SearchView_onReplyFail (Fail)
%RTBMM_SB.registerFailHandler("GETSEARCH","RTBMM_SearchView_onReplyFail");
function RTBMM_SearchView_onReplyFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//- RTBMM_SearchView_DoSearch (Starts a search)
function RTBMM_SearchView_DoSearch(%page,%vars)
{
   if($RTB::CModManager::Cache::CurrentZone $= "SearchView")
   {
      %keyword = RTBMM_SearchView_Keyword.getValue();
      %author = RTBMM_SearchView_Author.getValue();
      %section = RTBMM_SearchView_Section.getSelected();
      %category = RTBMM_SearchView_Category.getSelected();
      %searchSummary = RTBMM_SearchView_SearchSummary.getValue();
      %searchDescription = RTBMM_SearchView_SearchDescription.getValue();
      %sortBy = RTBMM_SearchView_SortBy.getSelected();
      %sortDir = RTBMM_SearchView_SortDir.getSelected();
      %vars = %keyword TAB %author TAB %section TAB %category TAB %searchSummary TAB %searchDescription TAB %sortBy TAB %sortDir;
   }
   else
   {
      %keyword = getField(%vars,0);
      %author = getField(%vars,1);
      %section = getField(%vars,2);
      %category = getField(%vars,3);
      %searchSummary = getField(%vars,4);
      %searchDescription = getField(%vars,5);
      %sortBy = getField(%vars,6);
      %sortDir = getField(%vars,7);
   }
   
   if(%page $= "")
      %page = 1;
   
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("DOSEARCH",1,%keyword,%author,%section,%category,%searchSummary,%searchDescription,%sortBy,%sortDir,%page);
   RTBMM_Zones_Track("DoSearchView","RTBMM_SearchView_DoSearch(\""@%page@"\",\""@%vars@"\");","RTBMM_SearchView_DoSearch(%%page%%,\""@%vars@"\");");
}

//- RTBMM_SearchView_onSearchReplyStart (Begins Reply)
function RTBMM_SearchView_onSearchReplyStart(%tcp,%line)
{
   RTBMM_GUI_Init();
   RTBMM_GUI_createSplitHeader("cell1","100","<color:FAFAFA><just:left><font:Impact:18>  Search Results");
   RTBMM_GUI_createSplitHeader("cell2","58","<font:Arial Bold:15>File","15","<font:Arial Bold:15>Submitter","12","<font:Arial Bold:15>Downloads","15","<font:Arial Bold:15>Rating");
}

//- RTBMM_SearchView_onSearchReply (Handle Search Reply)
%RTBMM_SB.registerResponseHandler("DOSEARCH","RTBMM_SearchView_onSearchReply");
function RTBMM_SearchView_onSearchReply(%tcp,%line)
{
   if(%line $= 0)
   {
      RTBMM_GUI_createMessage("<br><br>There were no results.<br><br>");
   }
   else
      RTBMM_SectionView_onReply(%tcp,%line);
}

//- RTBMM_SearchView_onSearchReplyStop (Stops Reply)
function RTBMM_SearchView_onSearchReplyStop(%tcp,%line)
{
   RTBMM_GUI_createFooter();
}

//- RTBMM_SearchView_onSearchReplyFail (Fail)
%RTBMM_SB.registerFailHandler("DOSEARCH","RTBMM_SearchView_onSearchReplyFail");
function RTBMM_SearchView_onSearchReplyFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//*********************************************************
//* Recent View
//*********************************************************
//- RTBMM_RecentView_Init (Entrance)
function RTBMM_RecentView_Init(%page)
{
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("GETRECENT",1,%page);
   RTBMM_Zones_Track("RecentView","RTBMM_RecentView_Init(\""@%page@"\");","RTBMM_RecentView_Init(%%page%%);");
}

//- RTBMM_RecentView_onReplyStart (Reply Start)
function RTBMM_RecentView_onReplyStart(%tcp)
{
   RTBMM_SectionView_onReplyStart(%tcp);
}

//- RTBMM_RecentView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETRECENT","RTBMM_RecentView_onReply");
function RTBMM_RecentView_onReply(%tcp,%line)
{
   RTBMM_SectionView_onReply(%tcp,%line);
}

//- RTBMM_RecentView_onReplyStop (Reply Stop)
function RTBMM_RecentView_onReplyStop(%tcp)
{
   if($RTB::CModManager::Cache::ElementsAdded <= 0)
      RTBMM_GUI_createMessage("<br>There have been no files submitted in the past 5 days.<br>");
      
   RTBMM_GUI_createFooter("cell2");
}

//- RTBMM_RecentView_onFail (Fail)
%RTBMM_SB.registerFailHandler("GETRECENT","RTBMM_SectionView_onFail");
function RTBMM_RecentView_onFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//*********************************************************
//* All Files View
//*********************************************************
//- RTBMM_AllFilesView_Init (Entrance)
function RTBMM_AllFilesView_Init(%page)
{
   RTBMM_GUI_Load();
   
   RTBMM_SendRequest("GETALLFILES",1,%page);
   RTBMM_Zones_Track("AllFilesView","RTBMM_AllFilesView_Init(\""@%page@"\");","RTBMM_AllFilesView_Init(%%page%%);");
}

//- RTBMM_AllFilesView_onReplyStart (Reply Start)
function RTBMM_AllFilesView_onReplyStart(%tcp)
{
   RTBMM_SectionView_onReplyStart(%tcp);
}

//- RTBMM_AllFilesView_onReply (Reply)
%RTBMM_SB.registerResponseHandler("GETALLFILES","RTBMM_AllFilesView_onReply");
function RTBMM_AllFilesView_onReply(%tcp,%line)
{
   RTBMM_SectionView_onReply(%tcp,%line);
}

//- RTBMM_AllFilesView_onReplyStop (Reply Stop)
function RTBMM_AllFilesView_onReplyStop(%tcp)
{
   if($RTB::CModManager::Cache::ElementsAdded <= 0)
      RTBMM_GUI_createMessage("<br>There are ... no files? Just assume something has gone horribly wrong.<br>");
      
   RTBMM_GUI_createFooter("cell2");
}

//- RTBMM_AllFilesView_onFail (Fail)
%RTBMM_SB.registerFailHandler("GETALLFILES","RTBMM_AllFilesView_onFail");
function RTBMM_AllFilesView_onFail(%tcp,%error)
{
   RTBMM_CategoryView_onFail(%tcp,%error);
}

//*********************************************************
//* Downloads View
//*********************************************************
//- RTBMM_TransferView_Init (Entrance)
function RTBMM_TransferView_Init()
{
   RTBMM_GUI_Init();
   
   $RTB::CModManager::Switchboard.killLine(1);
   
   RTBMM_Zones_Track("TransferView","RTBMM_TransferView_Init();");
   
   RTBMM_GUI_createHeader(2,"<color:FAFAFA><just:left><font:Impact:18>  Transfers");
   RTBMM_GUI_createSplitHeader(1,"88","<font:Arial Bold:15>Add-On","12","<font:Arial Bold:15>Options");
   
   RTBMM_TransferQueue.updateIndicator();   
   
   if(RTBMM_TransferQueue.getCount() > 0)
      RTBMM_TransferView_Draw();
   else
      RTBMM_GUI_createMessage("<br>There are currently no transfers.<br>");
      
   RTBMM_GUI_createHeader(2,"");
}

//- RTBMM_TransferView_Draw (Draws the entire transfer queue to the GUI)
function RTBMM_TransferView_Draw()
{
   for(%i=0;%i<RTBMM_TransferQueue.getCount();%i++)
   {
      %queue = RTBMM_TransferQueue.getObject(%i);

      %row = RTBMM_GUI_createContent(1,70,10,78,12);
      %icon = %row.getObject(0);
      %info = %row.getObject(1);
      %opts = %row.getObject(2);
      
      if((%i+1) > 9)
      {
         %position = new GuiBitmapCtrl()
         {
            position = "-14 4";
            extent = "64 60";
            bitmap = "./images/image_large"@getSubStr(%i+1,0,1);
         };
         %icon.add(%position);

         %position = new GuiBitmapCtrl()
         {
            position = "17 4";
            extent = "64 60";
            bitmap = "./images/image_large"@getSubStr(%i+1,1,1);
         };
         %icon.add(%position);
      }
      else
      {
         %position = new GuiBitmapCtrl()
         {
            position = "0 0";
            extent = "64 60";
            bitmap = "./images/image_large"@%i+1;
         };
         %icon.add(%position);
         RTBMM_GUI_Center(%position);
      }
      
      %title = new GuiMLTextCtrl()
      {
         position = "6 4";
         extent = "360 13";
      };
      %info.add(%title);
      
      %desc = new GuiMLTextCtrl()
      {
         position = "6 19";
         extent = "360 13";
      };
      %info.add(%desc);
      
      %speed = new GuiMLTextCtrl()
      {
         position = "259 4";
         extent = "262 12";
      };
      %info.add(%speed);
      
      %done = new GuiMLTextCtrl()
      {
         position = "259 19";
         extent = "262 12";
      };
      %info.add(%done);
      
      %load_bg = new GuiBitmapCtrl()
      {
         position = "0 36";
         extent = "517 26";
         bitmap = "./images/image_loadbg";
      };
      %info.add(%load_bg);
      
      %load = new GuiProgressCtrl()
      {
         position = "1 1";
         profile = RTBMM_ProgressBar;
         extent = "515 24";
      };
      %load_bg.add(%load);
      
      %red = new GuiSwatchCtrl()
      {
         position = "1 1";
         extent = "515 24";
         color = "255 0 0 200";
      };
      %load_bg.add(%red);
      
      %load_ov = new GuiBitmapCtrl()
      {
         extent = "517 26";
         bitmap = "./images/image_loadov";
      };
      %load_bg.add(%load_ov);
      
      %load_txt = new GuiMLTextCtrl()
      {
         extent = "400 12";
      };
      %load_bg.add(%load_txt);
      
      RTBMM_GUI_CenterHoriz(%load_bg);
      RTBMM_GUI_Center(%load_txt);
      
      %transfer = new GuiBitmapButtonCtrl()
      {
         position = "5 4";
         extent = "68 18";
         text = " ";
         bitmap = "./images/buttons/medium/btnTransfer";
      };
      %opts.add(%transfer);
      RTBMM_GUI_CenterHoriz(%transfer);
      
      %cancel = new GuiBitmapButtonCtrl()
      {
         position = "5 24";
         extent = "68 18";
         text = " ";
         bitmap = "./images/buttons/medium/btnRemove";
         command = "RTBMM_TransferView_Remove("@%queue.id@");";
      };
      %opts.add(%cancel);
      RTBMM_GUI_CenterHoriz(%cancel);
      
      %up = new GuiBitmapButtonCtrl()
      {
         position = "5 44";
         extent = "28 18";
         text = " ";
         bitmap = "./images/buttons/medium/btnUp";
         command = "RTBMM_TransferView_MoveUp("@%queue.id@");";
      };
      %opts.add(%up);
      
      %down = new GuiBitmapButtonCtrl()
      {
         position = "35 44";
         extent = "38 18";
         text = " ";
         bitmap = "./images/buttons/medium/btnDown";
         command = "RTBMM_TransferView_MoveDown("@%queue.id@");";
      };
      %opts.add(%down);
      
      %queue.g_row = %row;
      %queue.g_title = %title;
      %queue.g_desc = %desc;
      %queue.g_speed = %speed;
      %queue.g_done = %done;
      %queue.g_progress = %load;
      %queue.g_red = %red;
      %queue.g_progress_text = %load_txt;
      %queue.g_transfer = %transfer;
      %queue.g_cancel = %cancel;
      %queue.g_up = %up;
      %queue.g_down = %down;
      %queue.update();
   }
}

//- RTBMM_TransferView_Add (GUI-handler for downloading files)
function RTBMM_TransferView_Add(%id)
{
   if(RTBMM_TransferQueue.getCount() $= 99)
   {
      RTBMM_GUI_createMessageBoxOK("Oh dear!","You can't download more than 99 files at a time!");
      return;
   }
   
   if(!RTBMM_TransferQueue.addItem(%id))
      RTBMM_GUI_createMessageBoxOK("Whooops","You already have this file in your transfers list.");
}

//- RTBMM_TransferView_Remove (Removes an item)
function RTBMM_TransferView_Remove(%id)
{
   RTBMM_TransferQueue.removeItem(%id);
   RTBMM_TransferView_Init();
}

//- RTBMM_TransferView_MoveUp (Moves an item in the queue up in the list)
function RTBMM_TransferView_MoveUp(%id)
{
   %position = RTBMM_TransferQueue.getItemPos(%id);
   if(%position == 0)
      return;
      
   RTBMM_TransferQueue.swap(%position,%position-1);
   RTBMM_TransferView_Init();
}

//- RTBMM_TransferView_MoveDown (Moves an item in the queue down in the list)
function RTBMM_TransferView_MoveDown(%id)
{
   %position = RTBMM_TransferQueue.getItemPos(%id);
   if(%position == RTBMM_TransferQueue.getCount()-1)
      return;
      
   RTBMM_TransferQueue.swap(%position,%position+1);
   RTBMM_TransferView_Init();
}

//- RTBMM_TransferView_RequestTransfer (Attempts to begin a file transfer)
function RTBMM_TransferView_RequestTransfer(%id)
{
   if(RTBMM_FileGrabber.connected || isObject(RTBMM_FileGrabber.queue))
      RTBMM_GUI_createMessageBoxOK("Ooops","There is already a file being downloaded. Cancel or wait for that to finish then try again.");
   else
      RTBMM_FileGrabber.loadItem(%id);
}

//- RTBMM_TransferView_HaltTransfer (Halts a file transfer)
function RTBMM_TransferView_HaltTransfer(%id)
{
   if(RTBMM_FileGrabber.queue !$= RTBMM_TransferQueue.getObject(%id))
      RTBMM_GUI_createMessageBoxOK("Ooops","This file is not currently in transfer.");
   else
      RTBMM_FileGrabber.halt();
}

//- RTBMM_TransferView_onFileData (On data being returned about a file)
%RTBMM_SB.registerResponseHandler("GETFILEDATA","RTBMM_TransferView_onFileData");
function RTBMM_TransferView_onFileData(%tcp,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6)
{
   if(!%item = RTBMM_TransferQueue.getItem(%arg1))
      return;
      
   %item.live = 1;
      
   if(%arg2 $= 1)
   {
      %item.status = 1;
      
      %item.title = %arg3;
      %item.desc = "Submitted by "@%arg4;
      %item.zip = %arg5;
      %item.filesize = %arg6;
      %item.progress_text = "Waiting to Download";
      
      if(isFile("Add-Ons/"@%arg5) && !isFile("Add-Ons/"@ strReplace(%arg5,".zip","") @"/rtbContent.txt"))
         %item.content_only = 0;
   }
   else
   {
      if(%arg3 $= 0 || %arg3 $= 2)  //File not Found
      {
         %item.status = 0;
         %item.desc = "File could not be found.";
         %item.progress_text = "File Missing";
      }
      else if(%arg3 $= 1)  //File Failed
      {
         %item.status = 0;
         %item.desc = "File has been failed by a moderator.";
         %item.progress_text = "File in Fail Bin";
      }
      else  //Don't Know
      {
         %item.status = 0;
         %item.desc = "Ambiguous error code was returned from server.";
         %item.progress_text = "Error Occurred";
      }
   }
   %item.update();
   
   RTBMM_FileGrabber.poke();
}

//- RTBMM_TransferView_onDataFail (When data fails to be returned)
%RTBMM_SB.registerFailHandler("GETFILEDATA","RTBMM_TransferView_onDataFail");
function RTBMM_TransferView_onDataFail(%tcp)
{
   if(!%item = RTBMM_TransferQueue.getItem(getField(%tcp.t_rawString,0)))
      return;
      
   %item.status = 0;
   %item.desc = "Could not connect to the RTB Service";
   %item.progress_text = "Unable to Connect";

   %item.update();
}

//*********************************************************
//* Mods View
//*********************************************************
//- RTBMM_ModsView_Init (Entrance)
function RTBMM_ModsView_Init(%mode)
{
   RTBMM_GUI_Init();
   
   $RTB::CModManager::Switchboard.killLine(1);
   
   RTBMM_Zones_Track("ModsView","RTBMM_ModsView_Init(\""@%mode@"\");");
   
   %header = RTBMM_GUI_createHeader(2,"<color:FAFAFA><just:left><font:Impact:18>  Your Mods");
   
   %expandBtn = new GuiBitmapButtonCtrl()
   {
      position = "399 4";
      extent = "68 18";
      text = " ";
      bitmap = "./images/buttons/medium/btnExpand";
      command = "RTBMM_ModsView_ExpandAll();";
   };
   %header.add(%expandBtn);
   
   %collapseBtn = new GuiBitmapButtonCtrl()
   {
      position = "468 4";
      extent = "68 18";
      text = " ";
      bitmap = "./images/buttons/medium/btnCollapse";
      command = "RTBMM_ModsView_CollapseAll();";
   };
   %header.add(%collapseBtn);
   
   %sectionsBtn = new GuiBitmapButtonCtrl()
   {
      position = "537 4";
      extent = "68 18";
      text = " ";
      bitmap = "./images/buttons/medium/btnSections";
      command = "RTBMM_ModsView_Init(\"sections\");";
   };
   %header.add(%sectionsBtn);
   
   %groupsBtn = new GuiBitmapButtonCtrl()
   {
      position = "606 4";
      extent = "68 18";
      text = " ";
      bitmap = "./images/buttons/medium/btnGroups";
      command = "RTBMM_ModsView_Init(\"groups\");";
   };
   %header.add(%groupsBtn);
   
   $RTB::CModManager::Cache::SectionHeader = %header;
   
   if(%mode $= "groups")
      RTBMM_ModsView_InitGroupsView();
   else
      RTBMM_ModsView_InitSectionView();

   $RTB::CModManager::Cache::Section[$RTB::CModManager::Cache::NumSections] = RTBMM_GUI_createHeader(2," ");
   $RTB::CModManager::Cache::NumSections++;
}

//- RTBMM_ModsView_InitSectionView (section-based view of add-ons)
function RTBMM_ModsView_InitSectionView()
{
   %header = $RTB::CModManager::Cache::SectionHeader;
   
   %syncBtn = new GuiBitmapButtonCtrl()
   {
      position = "184 4";
      extent = "68 18";
      text = " ";
      bitmap = "./images/buttons/medium/btnSync";
      command = "RTBMM_ModsView_syncAddons();";
   };
   %header.add(%syncBtn);
   
   %enableBtn = new GuiBitmapButtonCtrl()
   {
      position = "261 4";
      extent = "68 18";
      text = " ";
      bitmap = "./images/buttons/medium/btnEnable";
      command = "RTBMM_ModsView_EnableAll();";
   };
   %header.add(%enableBtn);
   
   %disableBtn = new GuiBitmapButtonCtrl()
   {
      position = "330 4";
      extent = "68 18";
      text = " ";
      bitmap = "./images/buttons/medium/btnDisable";
      command = "RTBMM_ModsView_DisableAll();";
   };
   %header.add(%disableBtn);   
   
   $RTB::CModManager::Cache::NumSections = 0;
   
   %sections = 0;
   for(%i=0;%i<RTBMM_FileCache.getCount();%i++)
   {
      %file = RTBMM_FileCache.getObject(%i);
      %type = %file.file_type;
      
      if(!%added[%type])
      {
         %added[%type] = 1;
         %files[%type] = 0;
         %section[%sections++] = %type;
      }
      %file[%files[%type]++,%type] = %file;
   }
   
   %sortString = "";      
   for(%i=1;%i<%sections+1;%i++)
   {
      %sortString = %sortString@%i@"=>"@%section[%i]@",";
   }
   %sortString = getSubStr(%sortString,0,strLen(%sortString)-1);
   %sortString = strReplace(sortFields(%sortString),",","\t");
   
   %k = 0;
   for(%i=0;%i<getFieldCount(%sortString);%i++)
   {
      %block = strReplace(getField(%sortString,%i),"=>","\t");
      %key = getField(%block,0);
      %value = getField(%block,1);
      %section[%k++] = %value;
   }
   
   %collapse = 0;
   if($RTB::CModManager::SectionCollapsed["Default Add-Ons"] $= "" || $RTB::CModManager::SectionCollapsed["Default Add-Ons"] $= 1)
         %collapse = 1;
   RTBMM_ModsView_createSectionRow("Default Add-Ons",%collapse);
   
   for(%i=1;%i<%sections+1;%i++)
   {
      if(%section[%i] $= "RTB2 Add-Ons")
         %hasRTB2AddOns = 1;
      if(%section[%i] $= "Non-RTB Add-Ons")
         %hasNonRTBAddOns = 1;
      if(%section[%i] $= "Unpackaged Add-Ons")
         %hasUnpackagedAddOns = 1;
      if(%section[%i] $= "Content-Only Add-Ons")
         %hasContentOnlyAddOns = 1;
      if(%section[%i] $= "Default Add-Ons" || %section[%i] $= "Non-RTB Add-Ons" || %section[%i] $= "RTB2 Add-Ons" || %section[%i] $= "Unpackaged Add-Ons" || %section[%i] $= "Content-Only Add-Ons")
         continue;
      
      %collapse = 0;
      if($RTB::CModManager::SectionCollapsed[%section[%i]])
         %collapse = 1;
      
      RTBMM_ModsView_createSectionRow(%section[%i],%collapse);
   }
   
   if(%hasNonRTBAddOns)
   {
      %collapse = 0;
      if($RTB::CModManager::SectionCollapsed["Non-RTB Add-Ons"] $= "" || $RTB::CModManager::SectionCollapsed["Non-RTB Add-Ons"] $= 1)
            %collapse = 1;
      RTBMM_ModsView_createSectionRow("Non-RTB Add-Ons",%collapse);
   }
   
   if(%hasUnpackagedAddOns)
   {
      %collapse = 0;
      if($RTB::CModManager::SectionCollapsed["Unpackaged Add-Ons"] $= "" || $RTB::CModManager::SectionCollapsed["Unpackaged Add-Ons"] $= 1)
            %collapse = 1;
      RTBMM_ModsView_createSectionRow("Unpackaged Add-Ons",%collapse);
   }
   
   if(%hasRTB2AddOns)
   {
      %collapse = 0;
      if($RTB::CModManager::SectionCollapsed["RTB2 Add-Ons"] $= "" || $RTB::CModManager::SectionCollapsed["RTB2 Add-Ons"] $= 1)
            %collapse = 1;
      RTBMM_ModsView_createSectionRow("RTB2 Add-Ons",%collapse);
   }
   
   if(%hasContentOnlyAddOns)
   {
      %collapse = 0;
      if($RTB::CModManager::SectionCollapsed["Content-Only Add-Ons"] $= "" || $RTB::CModManager::SectionCollapsed["Content-Only Add-Ons"] $= 1)
            %collapse = 1;
      RTBMM_ModsView_createSectionRow("Content-Only Add-Ons",%collapse);
   }
}

//- RTBMM_ModsView_createSectionRow (creates a section row)
function RTBMM_ModsView_createSectionRow(%name,%hide)
{
   %files = 0;
   for(%i=0;%i<RTBMM_FileCache.getCount();%i++)
   {
      if(RTBMM_FileCache.getObject(%i).file_type $= %name)
      {
         %file[%files] = RTBMM_FileCache.getObject(%i);
         %files++;
      }
   }
   
   if(%files <= 0)
      return;
   
   %container = new GuiSwatchCtrl()
   {
      position = 0 SPC $RTB::CModManager::GUI::CurrentY;
      extent = "680 28";
      color = "255 255 255 255";
   };
   RTBMM_GUI_PushControl(%container);
   
   %container.sec_id = $RTB::CModManager::Cache::NumSections;
   $RTB::CModManager::Cache::Section[$RTB::CModManager::Cache::NumSections] = %container;
   $RTB::CModManager::Cache::NumSections++;
   
   %s = (%files $= 1)?"":"s";
   %header = RTBMM_GUI_createHeader(1,"<just:left>        "@%name@"  <font:Arial:12>"@%files[%section[%i]]@" Add-On"@%s);
   %container.header = %header;
   %container.name = %name;
   %container.files = %files[%section[%i]];
   %bitmap = new GuiBitmapCtrl()
   {
      position = "5 6";
      extent = "16 16";
      bitmap = "./images/icon_arrowdown";
   };
   %header.add(%bitmap);
   %container.add(%header);
   %header.position = "0 0";
   %container.bitmap = %bitmap;
   
   %mouseEvent = new GuiMouseEventCtrl()
   {
      extent = "680 28";
      
      eventType = "sectionClick";
      eventCallbacks = "0001";
      
      id = $RTB::CModManager::Cache::NumSections-1;
      type = 1;
      
      container = %container;
   };
   %container.add(%mouseEvent);
   %container.mouseEvent = %mouseEvent;

   %sortString = "";      
   for(%i=0;%i<%files;%i++)
   {
      %sortString = %sortString@%file[%i]@"=>"@%file[%i].file_title@",";
   }
   %sortString = getSubStr(%sortString,0,strLen(%sortString)-1);
   %sortString = strReplace(sortFields(%sortString),",","\t");
   for(%j=0;%j<getFieldCount(%sortString);%j++)
   {
      %field = strReplace(getField(%sortString,%j),"=>","\t");
      %row = RTBMM_ModsView_createModRow(getField(%field,0));
      if(!isObject(%row))
         continue;
      %container.add(%row);
      %container.bringToFront(%row);
      
      %row.vertSizing = "top";
      %row.position = 0 SPC getWord(%container.extent,1);
      %container.extent = vectorAdd(%container.extent,"0" SPC getWord(%row.extent,1));
   }
   %container.originalHeight = getWord(%container.extent,1);
   
   if(%hide)
   {
      %mouseEvent.type = 2;
      RTBMM_ModsView_collapseSection($RTB::CModManager::Cache::NumSections-1,1);
   }
}

//- Event_sectionClick::onMouseUp (callback for clicking on a section header)
function Event_sectionClick::onMouseUp(%this)
{
   if(%this.type $= 1)
   {
      if(RTBMM_ModsView_collapseSection(%this.id))
      {
         %this.type = 2;
         %secName = $RTB::CModManager::Cache::Section[%this.id].name;
         $RTB::CModManager::SectionCollapsed[%secName] = 1;
      }
   }
   else if(%this.type $= 2)
   {
      if(RTBMM_ModsView_expandSection(%this.id))
      {
         %this.type = 1;
         %secName = $RTB::CModManager::Cache::Section[%this.id].name;
         $RTB::CModManager::SectionCollapsed[%secName] = 0;
      }
   }
}

//- RTBMM_ModsView_createModRow (creates a mod row for the section view)
function RTBMM_ModsView_createModRow(%cache)
{
   if(!isObject(%cache))
      return;
      
   %content = RTBMM_GUI_createContent(1,40,6,38,6,50);
   %c_icon = %content.getObject(0);
   %c_info = %content.getObject(1);
   %c_state = %content.getObject(2);
   %c_opts = %content.getObject(3);
   %content.setName("ModsViewRow_"@%cache.file_var);
   %content.file_title = %cache.file_title;   
   
   %icon = new GuiBitmapCtrl()
   {
      extent = "16 16";
      bitmap = "./images/icons/"@%cache.file_icon;
   };
   %c_icon.add(%icon);
   RTBMM_GUI_Center(%icon);
   
   %title_text = new GuiMLTextCtrl()
   {
      position = "2 4";
      extent = "307 18";
      text = "<font:Arial Bold:14><color:777777> "@%cache.file_title;
   };
   %c_info.add(%title_text);
   
   %text = new GuiMLTextCtrl()
   {
      position = "4 20";
      extent = "450 18";
      text = "<font:Verdana:12><color:999999>by "@%cache.file_author;
   };
   %c_info.add(%text);
  
   if(%cache.file_platform $= "rtb" || %cache.file_platform $= "rtb2")
      %title_text.setText(%title_text.text@" <color:999999><font:Arial:12>v"@%cache.file_version);  
  
   if(%cache.file_platform $= "rtb" || %cache.file_isContent)
   { 
      %icon = new GuiBitmapButtonCtrl()
      {
         position = "235 1";
         extent = "16 16";
         text = " ";
         bitmap = "./images/buttons/small/btnEye";
         command = "RTBMM_FileView_Init("@%cache.file_id@");";
      };
      %c_info.add(%icon);
      
      if(%cache.file_platform $= "rtb")
      {
         %reportBtn = new GuiBitmapButtonCtrl()
         {
            position = "235 18";
            extent = "16 16";
            text = " ";
            bitmap = $RTB::Path@"images/buttons/small/btnReport";
            command = "RTBMM_FileView_Report("@%cache.file_id@");";
         };
         %c_info.add(%reportBtn);
      }
   }
   
   %enabled = $AddOn__[%cache.file_var];
   if(%cache.file_isContent)
   {
      %swatch = new GuiSwatchCtrl()
      {
         position = %c_state.position;
         extent = %c_state.extent;
         color = "150 150 150 50";
      };
      %c_state.getGroup().add(%swatch);
      
      %icon = new GuiBitmapCtrl()
      {
         extent = "16 16";
         bitmap = "./images/icon_tickGray";
         swatch = %swatch;
      };
      %c_state.add(%icon);
   }
   else if(%enabled $= 1 || %cache.file_special !$= "")
   {
      %swatch = new GuiSwatchCtrl()
      {
         position = %c_state.position;
         extent = %c_state.extent;
         color = "0 255 0 50";
      };
      %c_state.getGroup().add(%swatch);
      
      %icon = new GuiBitmapCtrl()
      {
         extent = "16 16";
         bitmap = "./images/icon_tick";
         swatch = %swatch;
      };
      %c_state.add(%icon);
   }
   else
   {
      %swatch = new GuiSwatchCtrl()
      {
         position = %c_state.position;
         extent = %c_state.extent;
         color = "255 0 0 50";
      };
      %c_state.getGroup().add(%swatch);
      
      %icon = new GuiBitmapCtrl()
      {
         extent = "16 16";
         bitmap = "./images/icon_cross";
         swatch = %swatch;
      };
      %c_state.add(%icon);
   }
   RTBMM_GUI_Center(%icon);
   
   if(%cache.file_isContent)
   {
      %btnLeft = new GuiBitmapButtonCtrl()
      {
         position = "7 6";
         extent = "82 25";
         text = " ";
         command = "RTBMM_TransferView_Add(\""@%cache.file_id@"\");";
         bitmap = "./images/buttons/large/gray/btnDownloadContent";
      };
      %c_opts.add(%btnLeft);
   }
   else if(%cache.file_special $= "clientside")
   {
      %btnLeft = new GuiBitmapCtrl()
      {
         position = "7 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnClient_i";
      };
      %c_opts.add(%btnLeft);
   }
   else if(%cache.file_special $= "map")
   {
      %btnLeft = new GuiBitmapCtrl()
      {
         position = "7 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnMap_i";
      };
      %c_opts.add(%btnLeft);
   }
   else if(%cache.file_special $= "decal")
   {
      %btnLeft = new GuiBitmapCtrl()
      {
         position = "7 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnDecal_i";
      };
      %c_opts.add(%btnLeft);
   }
   else if(%cache.file_special $= "colorset")
   {
      %btnLeft = new GuiBitmapCtrl()
      {
         position = "7 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnColorset_i";
      };
      %c_opts.add(%btnLeft);
   }
   else
   {
      if(%enabled $= 1)
      {
         %btnLeft = new GuiBitmapButtonCtrl()
         {
            position = "7 6";
            extent = "82 25";
            text = " ";
            command = "RTBMM_ModsView_DisableAddon(\""@%cache.file_var@"\");";
            bitmap = "./images/buttons/large/gray/btnDisable";
         };
         %c_opts.add(%btnLeft);
      }
      else
      {
         %btnLeft = new GuiBitmapButtonCtrl()
         {
            position = "7 6";
            extent = "82 25";
            text = " ";
            command = "RTBMM_ModsView_EnableAddon(\""@%cache.file_var@"\");";
            bitmap = "./images/buttons/large/gray/btnEnable";
         };
         %c_opts.add(%btnLeft);
      }
   }
   
   if(%cache.file_isContent)
   {
      %btnA = new GuiBitmapCtrl()
      {
         position = "87 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnPlaceholder_n";
      };
      %c_opts.add(%btnA);

      %btnB = new GuiBitmapCtrl()
      {
         position = "167 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnPlaceholder_n";
      };
      %c_opts.add(%btnB);
   }
   else if(%cache.file_platform $= "rtb")
   {      
      %btnBug = new GuiBitmapButtonCtrl()
      {
         position = "87 6";
         extent = "82 25";
         text = " ";
         command = "RTBMM_ModsView_Report("@%cache.file_id@");";
         bitmap = "./images/buttons/large/gray/btnSendBug";
      };
      %c_opts.add(%btnBug);

      %btnUpdate = new GuiBitmapButtonCtrl()
      {
         position = "167 6";
         extent = "82 25";
         text = " ";
         command = "RTBMM_ModsView_updateAddon("@%cache@","@%content@");";
         bitmap = "./images/buttons/large/gray/btnUpdate";
      };
      %c_opts.add(%btnUpdate);
   }
   else
   {  
      %btnBug = new GuiBitmapCtrl()
      {
         position = "87 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnSendBug_i";
      };
      %c_opts.add(%btnBug);

      %btnUpdate = new GuiBitmapCtrl()
      {
         position = "167 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnUpdate_i";
      };
      %c_opts.add(%btnUpdate);
   }
   
   if(!%cache.file_default)
   {
      %btnDelete = new GuiBitmapButtonCtrl()
      {
         position = "247 6";
         extent = "82 25";
         text = " ";
         command = "RTBMM_GUI_createMessageBoxOKCancel(\"Are you sure?\",\"<just:center>Are you sure you want to permanently delete the selected add-on?\",\"RTBMM_ModsView_deleteAddon("@%cache@","@%content@");\");";
         bitmap = "./images/buttons/large/gray/btnDelete";
      };
      %c_opts.add(%btnDelete);
   }
   else
   {
      %btnDelete = new GuiBitmapCtrl()
      {
         position = "247 6";
         extent = "82 25";
         text = " ";
         bitmap = "./images/buttons/large/gray/btnDelete_i";
      };
      %c_opts.add(%btnDelete);
   }
   %cache.physical_row = %content;
   
   return %content;
}

//- RTBMM_ModsView_insertModRow (inserts a mod row live into the mods view)
function RTBMM_ModsView_insertModRow(%cache)
{
   for(%i=0;%i<$RTB::CModManager::Cache::NumSections-1;%i++)
   {
      %sec = $RTB::CModManager::Cache::Section[%i];
      if(%sec.name $= %cache.file_type)
      {
         %targetSection = %sec;
         break;
      }
   }
   
   if(!isObject(%targetSection))
   {
      RTBMM_Zones_Refresh();
      return;
   }
   else
   {
      %targetSection.files++;
      %s = (%targetSection.files == 1) ? "" : "s";
      %targetSection.header.getObject(1).setText("<color:888888><font:Arial Bold:15><just:left>        "@%targetSection.name@"  <font:Arial:12>"@%targetSection.files@" Add-On"@%s);
      
      %content = RTBMM_ModsView_createModRow(%cache,1);
      RTBMM_GUI_Offset(-(getWord(%content.extent,1)));
      
      for(%i=0;%i<%targetSection.getCount();%i++)
      {
         %row = %targetSection.getObject(0);
         if(!%targetSection.isMember(%content))
         {
            if(alphaCompare(%cache.file_title,%row.file_title) $= 2)
            {
               %row.position = vectorSub(%row.position,0 SPC getWord(%content.extent,1));
               %targetSection.add(%content);
               %targetSection.pushToBack(%content);
               %content.vertSizing = "top";
               %content.position = vectorAdd(%row.position,0 SPC getWord(%content.extent,1));
               %i++;
            }
            else if(%i $= %targetSection.getCount()-2)
            {
               %targetSection.add(%content);
               %targetSection.pushToBack(%content);
               %content.vertSizing = "top";
               %content.position = vectorSub("0 28",0 SPC getWord(%content.extent,1));
               %i++;
            }
         }
         else if(%row.file_title !$= "")
         {          
            %row.position = vectorSub(%row.position,0 SPC getWord(%content.extent,1));
         }
         %targetSection.pushToBack(%row);
      }
      %targetSection.originalHeight = vectorAdd(%targetSection.originalHeight,getWord(%content.extent,1));
      %sec_id = %targetSection.sec_id;
      
      if(%targetSection.mouseEvent.type $= 1)
      {
         %targetSection.resize(getWord(%targetSection.position,0),getWord(%targetSection.position,1),getWord(%targetSection.extent,0),getWord(%targetSection.extent,1)+getWord(%content.extent,1));
         for(%i=0;%i<$RTB::CModManager::Cache::NumSections;%i++)
         {
            %sec = $RTB::CModManager::Cache::Section[%i];
            if(%i > %sec_id)
               %sec.position = vectorAdd(%sec.position,0 SPC getWord(%content.extent,1));         
         }
      }
   }
}

//- RTBMM_ModsView_collapseAll (collapses all the sections)
function RTBMM_ModsView_collapseAll()
{
   for(%i=0;%i<$RTB::CModManager::Cache::NumSections;%i++)
   {
      if(RTBMM_ModsView_collapseSection(%i,1))
      {
         %sec = $RTB::CModManager::Cache::Section[%i];
         %sec.mouseEvent.type = 2;
         $RTB::CModManager::SectionCollapsed[%sec.name] = 1;
      }
   }
}

//- RTBMM_ModsView_collapseSection (collapses a mod view section)
function RTBMM_ModsView_collapseSection(%id,%noAnimate)
{
   %sec = $RTB::CModManager::Cache::Section[%id];
   if(%sec.sec_id $= "")
      return;
      
   if(!$RTB::Options::MM::AnimateGUI || %noAnimate)
   {
      %sec.bitmap.position = "4 5";
      %sec.bitmap.setBitmap($RTB::Path@"images/icon_arrowright");
      RTBMM_ModsView_instantCollapseSection(%id);
      return 1;
   }

   if(!isEventPending(%sec.expColSch))
   {
      %sec.bitmap.position = "4 5";
      %sec.bitmap.setBitmap($RTB::Path@"images/icon_arrowright");
      if(%sec.files > 3)
         %time = 400;
      else
         %time = 100*%sec.files;
         
      if(%sec.files <= 0)
         %time = 100;
         
      RTBMM_ModsView_doCollapse(%id,0,getWord(%sec.extent,1),(28-getWord(%sec.extent,1))-1,%time);
      return 1;
   }
   return 0;
}

//- RTBMM_ModsView_doCollapse (looping function to animate the section collapsing)
function RTBMM_ModsView_doCollapse(%id,%time,%begin,%change,%duration)
{
   if(%time $= %duration)
      return;

   %sec = $RTB::CModManager::Cache::Section[%id];
   if(!isObject(%sec))
      return;
      
   %oldExtent = getWord(%sec.extent,1);
   %newExtent = mceil(Anim_EaseInOut(%time,%begin,%change,%duration));
   %sec.resize(getWord(%sec.position,0),getWord(%sec.position,1),getWord(%sec.extent,0),%newExtent);
   %sec.expColSch = schedule(10,0,"RTBMM_ModsView_doCollapse",%id,%time+10,%begin,%change,%duration);
   
   for(%i=%id+1;%i<$RTB::CModManager::Cache::NumSections;%i++)
   {
      %sec = $RTB::CModManager::Cache::Section[%i];
      %sec.position = vectorAdd(%sec.position,"0" SPC %newExtent-%oldExtent);
   }
   RTBMM_GUI_AutoResize();
}

//- RTBMM_ModsView_instantCollapseSection (Instantly collapses a section)
function RTBMM_ModsView_instantCollapseSection(%id)
{
   %sec = $RTB::CModManager::Cache::Section[%id];
   if(!isObject(%sec))
      return;

   %oldExtent = getWord(%sec.extent,1);
   %sec.resize(getWord(%sec.position,0),getWord(%sec.position,1),680,28);

   for(%i=%id+1;%i<$RTB::CModManager::Cache::NumSections;%i++)
   {
      %sec = $RTB::CModManager::Cache::Section[%i];
      %sec.position = vectorAdd(%sec.position,"0" SPC 28-%oldExtent);
   }
   RTBMM_GUI_AutoResize();
}

//- RTBMM_ModsView_expandAll (expands all the sections)
function RTBMM_ModsView_expandAll()
{
   for(%i=0;%i<$RTB::CModManager::Cache::NumSections;%i++)
   {
      if(RTBMM_ModsView_expandSection(%i,1))
      {
         %sec = $RTB::CModManager::Cache::Section[%i];
         %sec.mouseEvent.type = 1;
         $RTB::CModManager::SectionCollapsed[%sec.name] = 0;
      }
   }
}

//- RTBMM_ModsView_expandSection (Expands a section)
function RTBMM_ModsView_expandSection(%id,%noAnimate)
{
   %sec = $RTB::CModManager::Cache::Section[%id];
   if(%sec.sec_id $= "")
      return;
      
   if(!$RTB::Options::MM::AnimateGUI || %noAnimate)
   {
      %sec.bitmap.position = "5 6";
      %sec.bitmap.setBitmap($RTB::Path@"images/icon_arrowdown");
      RTBMM_ModsView_instantExpandSection(%id);
      return 1;
   }
   
   if(!isEventPending(%sec.expColSch))
   {
      %sec.bitmap.position = "5 6";
      %sec.bitmap.setBitmap($RTB::Path@"images/icon_arrowdown");
      if(%sec.files > 3)
         %time = 400;
      else
         %time = 100*%sec.files;
         
      if(%sec.files <= 0)
         %time = 100;
         
      RTBMM_ModsView_doExpand(%id,0,getWord(%sec.extent,1),%sec.originalHeight-28,%time);
      return 1;
   }
   return 0;
}

//- RTBMM_ModsView_doExpand (looping function to animate the section expanding)
function RTBMM_ModsView_doExpand(%id,%time,%begin,%change,%duration)
{
   if(%time $= %duration)
      return;

   %sec = $RTB::CModManager::Cache::Section[%id];
   if(!isObject(%sec))
      return;
      
   %oldExtent = getWord(%sec.extent,1);
   %newExtent = mceil(Anim_EaseInOut(%time,%begin,%change,%duration));
   %sec.resize(getWord(%sec.position,0),getWord(%sec.position,1),getWord(%sec.extent,0),%newExtent);
   %sec.expColSch = schedule(10,0,"RTBMM_ModsView_doExpand",%id,%time+10,%begin,%change,%duration);
   
   for(%i=%id+1;%i<$RTB::CModManager::Cache::NumSections;%i++)
   {
      %sec = $RTB::CModManager::Cache::Section[%i];
      %sec.position = vectorAdd(%sec.position,"0" SPC %newExtent-%oldExtent);
   }
   RTBMM_GUI_AutoResize();
}

//- RTBMM_ModsView_instantExpandSection (Instantly expands a section)
function RTBMM_ModsView_instantExpandSection(%id)
{
   %sec = $RTB::CModManager::Cache::Section[%id];
   if(!isObject(%sec))
      return;

   %oldExtent = getWord(%sec.extent,1);
   %sec.resize(getWord(%sec.position,0),getWord(%sec.position,1),680,%sec.originalHeight);
   %newExtent = getWord(%sec.extent,1);

   for(%i=%id+1;%i<$RTB::CModManager::Cache::NumSections;%i++)
   {
      %sec = $RTB::CModManager::Cache::Section[%i];
      %sec.position = vectorAdd(%sec.position,"0" SPC %newExtent-%oldExtent);
   }
   RTBMM_GUI_AutoResize();
}

//- RTBMM_ModsView_EnableAll (enables all add-ons)
function RTBMM_ModsView_EnableAll()
{
   for(%i=0;%i<RTBMM_FileCache.getCount();%i++)
   {
      %file = RTBMM_FileCache.getObject(%i);
      
      if(%file.file_special $= "colorset" || %file.file_special $= "decal" || %file.file_special $= "map" || %file.file_special $= "clientside")
         continue;
         
      $AddOn__[%file.file_var] = 1;
   }
   export("$AddOn__*","config/server/ADD_ON_LIST.cs");
   
   RTBMM_ModsView_Init();
}

//- RTBMM_ModsView_DisableAll (disables all add-ons)
function RTBMM_ModsView_DisableAll()
{
   for(%i=0;%i<RTBMM_FileCache.getCount();%i++)
   {
      %file = RTBMM_FileCache.getObject(%i);
      
      if(%file.file_special $= "colorset" || %file.file_special $= "decal" || %file.file_special $= "map" || %file.file_special $= "clientside")
         continue;
         
      $AddOn__[%file.file_var] = 0;
   }
   export("$AddOn__*","config/server/ADD_ON_LIST.cs");
   
   RTBMM_ModsView_Init();
}

//- RTBMM_ModsView_EnableAddon (Sets an add-on to enabled and updates gui)
function RTBMM_ModsView_EnableAddon(%var)
{
   $AddOn__[%var] = 1;
   export("$AddOn__*","config/server/ADD_ON_LIST.cs");
   
   %container = "ModsViewRow_"@%var;
   %c_state = %container.getObject(2);
   %c_opts = %container.getObject(3);
   
   %c_state.getObject(0).setBitmap($RTB::Path@"images/icon_tick");
   %c_state.getObject(0).swatch.color = "0 255 0 50";
   
   %c_opts.getObject(0).setBitmap($RTB::Path@"images/buttons/large/gray/btnDisable");
   %c_opts.getObject(0).command = "RTBMM_ModsView_DisableAddon(\""@%var@"\");";
}

//- RTBMM_ModsView_DisableAddon (Sets an add-on to disabled and updates gui)
function RTBMM_ModsView_DisableAddon(%var)
{
   $AddOn__[%var] = 0;
   export("$AddOn__*","config/server/ADD_ON_LIST.cs");
   
   %container = "ModsViewRow_"@%var;
   %c_state = %container.getObject(2);
   %c_opts = %container.getObject(3);

   %c_state.getObject(0).setBitmap($RTB::Path@"images/icon_cross");
   %c_state.getObject(0).swatch.color = "255 0 0 50";
   
   %c_opts.getObject(0).setBitmap($RTB::Path@"images/buttons/large/gray/btnEnable");
   %c_opts.getObject(0).command = "RTBMM_ModsView_EnableAddon(\""@%var@"\");";
}

//- RTBMM_ModsView_Report (Opens a form to allow the user to report a bug in the addon)
function RTBMM_ModsView_Report(%id)
{
   if(%id $= "")
   {
      RTBMM_GUI_createMessageBoxOK("Ooops","You have not selected a file to report a bug for.");
      return;
   }
      
   %window = RTBMM_GUI_createWindow("Report Bug");
   %window.resize(0,0,400,200);
   RTBMM_GUI_Center(%window);
   
   %label = new GuiMLTextCtrl()
   {
      position = "10 10";
      extent = "";
      text = "<font:Verdana:12><color:555555>Summary:";
   };
   %window.canvas.add(%label);
   $RTB::CModManager::Cache::Report::Summary = %label;
   
   %textedit = new GuiTextEditCtrl(RTBMM_Report_Summary)
   {
      profile = RTBMM_TextEditProfile;
      position = "66 8";
      extent = "316 16";
   };
   %window.canvas.add(%textedit);
   
   %label = new GuiMLTextCtrl()
   {
      position = "10 34";
      extent = "";
      text = "<font:Verdana:12><color:555555>Description:";
   };
   %window.canvas.add(%label);
   $RTB::CModManager::Cache::Report::Description = %label;
   
   %label = new GuiMLTextCtrl()
   {
      position = "202 34";
      extent = "";
      text = "<font:Verdana:12><color:555555>Severity:";
   };
   %window.canvas.add(%label);
   
   %popup = new GuiPopupMenuCtrl(RTBMM_Report_Severity)
   {
      profile = RTBMM_PopupProfile;
      position = "252 31";
      extent = "130 17";
   };
   %window.canvas.add(%popup);
   %popup.add("Low",1);
   %popup.add("Medium",2);
   %popup.add("High",3);
   %popup.setSelected(1);
   
   %textedit = new GuiScrollCtrl()
   {
      profile = RTBMM_TextEditProfile;
      position = "9 52";
      extent = "373 73";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOff";
      
      new GuiMLTextEditCtrl(RTBMM_Report_Description)
      {
         profile = RTBMM_MLEditProfile;
         position = "3 1";
         extent = "364 73";
      };
   };
   %window.canvas.add(%textedit);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "324 133";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnOK";
      command = "RTBMM_ModsView_SendReport("@%window@","@%id@");";
   };
   %window.canvas.add(%button);
   
   %button = new GuiBitmapButtonCtrl()
   {
      position = "260 133";
      extent = "58 25";
      text = " ";
      bitmap = "./images/buttons/large/gray/btnCancel";
      command = %window.closeButton.command;
   };
   %window.canvas.add(%button);
   
   %buglink = new GuiMLTextCtrl()
   {
      profile = "RTBMM_NewsContentProfile";
      position = "12 140";
      extent = "236 12";
      text = "<color:AAAAAA>Warning: Abuse will result in a ban.";
   };
   %window.canvas.add(%buglink);
}

//- RTBMM_ModsView_SendReport (Processes + Sends the File Report)
function RTBMM_ModsView_SendReport(%window,%id)
{
   if(RTBMM_Report_Summary.getValue() $= "")
   {
      $RTB::CModManager::Cache::Report::Summary.setValue("<font:Verdana Bold:12><color:EE0000>Summary:");
      %errors = 1;
   }
   else
      $RTB::CModManager::Cache::Report::Summary.setValue("<font:Verdana:12><color:555555>Summary:");

   if(RTBMM_Report_Description.getValue() $= "")
   {   
      $RTB::CModManager::Cache::Report::Description.setValue("<font:Verdana Bold:12><color:EE0000>Description:");
      %errors = 1;
   }
   else
      $RTB::CModManager::Cache::Report::Description.setValue("<font:Verdana:12><color:555555>Description:");
   
   if(%errors)
      return;
   
   %overlay = new GuiSwatchCtrl()
   {
      position = "0 0";
      extent = %window.canvas.extent;
      color = "255 255 255 200";
   };
   %window.canvas.add(%overlay);
   
   %bitmap = new GuiAnimatedBitmapCtrl()
   {
      extent = "31 31";
      bitmap = "./images/image_loadRing";
      framesPerSecond = 15;
      numFrames = 8;
   };
   %overlay.add(%bitmap);
   RTBMM_GUI_Center(%bitmap);
   %bitmap.shift(0,-10);
   
   %mlCtrl = new GuiMLTextCtrl()
   {
      extent = "200 0";
      text = "<just:center><font:Verdana:12><color:666666>Sending Report...";
   };
   %overlay.add(%mlCtrl);
   RTBMM_GUI_Center(%mlCtrl);
   %mlCtrl.shift(0,12);
   
   $RTB::CModManager::Cache::LoadRing = %bitmap;
   $RTB::CModManager::Cache::LoadText = %mlCtrl;
   
   %details = RTBMM_FileCache.get(%id);
   RTBMM_SendRequest("SENDBUGREPORT",3,%id,%details.file_version,RTBMM_Report_Summary.getValue(),RTBMM_Report_Description.getValue(),$Version,$RTB::Version,RTBMM_Report_Severity.getSelected());
}

//- RTBMM_ModsView_onReportReply (Reply from sending a report)
%RTBMM_SB.registerResponseHandler("SENDBUGREPORT","RTBMM_ModsView_onReportReply");
function RTBMM_ModsView_onReportReply(%tcp,%line)
{
   RTBMM_GUI_LoadRing_Clear($RTB::CModManager::Cache::LoadRing);
   if(getField(%line,0) $= 1)
   {
      $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Your report has been received.");
   }
   else
   {
      $RTB::CModManager::Cache::LoadRing.setBitmap($RTB::Path@"images/image_loadRingFail");
      if(getField(%line,1) $= 1)
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>You are not logged in!");
      else if(getField(%line,1) $= 2)
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>File not found.");
      else
         $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Unknown Error Occurred.");
   }
}

//- RTBMM_ModsView_onReportFail (Fail result from sending a report)
%RTBMM_SB.registerFailHandler("SENDBUGREPORT","RTBMM_ModsView_onReportFail");
function RTBMM_ModsView_onReportFail(%tcp,%fail)
{
   RTBMM_GUI_LoadRing_Clear($RTB::CModManager::Cache::LoadRing);
   $RTB::CModManager::Cache::LoadRing.setBitmap($RTB::Path@"images/image_loadRingFail");
   $RTB::CModManager::Cache::LoadText.setValue("<just:center><font:Verdana:12><color:666666>Connection Failed. Please try again later.");
}

//- RTBMM_ModsView_updateAddon (Checks for updates for a certain add-on)
function RTBMM_ModsView_updateAddon(%cache,%content)
{
   if(%cache.checkingForUpdates)
      return;
      
   %cache.checkingForUpdates = 1;
   
   %content.getObject(0).getObject(0).setVisible(0);
   %ring = new GuiAnimatedBitmapCtrl()
   {
      extent = "26 26";
      bitmap = "./images/image_loadRing";
      framesPerSecond = 15;
      numFrames = 8;
   };
   %content.getObject(0).add(%ring);
   RTBMM_GUI_Center(%ring);
   %content.getObject(1).getObject(1).oldValue = %content.getObject(1).getObject(1).getValue();
   %content.getObject(1).getObject(1).setValue("<font:Verdana:12><color:999999>Checking for Updates...");
   
   RTBMM_SendRequest("GETUPDATE",3,%cache.file_id,%cache.file_version,%content);
}

//- RTBMM_ModsView_onUpdateReply (Reply Handler)
%RTBMM_SB.registerResponseHandler("GETUPDATE","RTBMM_ModsView_onUpdateReply");
function RTBMM_ModsView_onUpdateReply(%tcp,%line)
{
   %file_id = getField(%tcp.t_rawString,0);
   %content = getField(%tcp.t_rawString,2);
   
   RTBMM_FileCache.get(%file_id).checkingForUpdates = 0;

   if(!isObject(%content))
      return;
      
   %content.getObject(0).getObject(0).setVisible(1);
   %content.getObject(0).getObject(1).delete();

   if(%line $= 1)
   {
      RTBMM_GUI_createMessageBoxOKCancel("Update Found!","An update has been found for this add-on, would you like to download it now?","RTBMM_TransferQueue.addItem("@%file_id@");","");
      %content.getObject(1).getObject(1).setValue("<font:Verdana:12><color:999999>An update was found.");
   }
   else
   {
      %content.getObject(1).getObject(1).setValue("<font:Verdana:12><color:FF6666>No updates were found ...");
   }
   schedule(2000,0,"RTBMM_ModsView_resetDesc",%content.getObject(1).getObject(1));
}

//- RTBMM_ModsView_resetDesc (Resets control value)
function RTBMM_ModsView_resetDesc(%ctrl)
{
   if(!isObject(%ctrl))
      return;
      
   %ctrl.setValue(%ctrl.oldValue);
}

//- RTBMM_ModsView_deleteAddon (Action to remove a row and alter the gui)
function RTBMM_ModsView_deleteAddon(%cache,%row,%noDel)
{
   %section = %row.getGroup();
   %id = %section.sec_id;
   %extent = getWord(%row.extent,1);
   %pos = getWord(%row.position,1);
   %row.delete();
   
   %section.files--;
   %s = (%section.files == 1) ? "" : "s";
   %section.header.getObject(1).setText("<color:888888><font:Arial Bold:15><just:left>        "@%section.name@"  <font:Arial:12>"@%section.files@" Add-On"@%s);

   if(%section.getCount() <= 2)
   {
      %extent = getWord(%section.extent,1);
      %section.delete();

      for(%i=%id+1;%i<$RTB::CModManager::Cache::NumSections;%i++)
      {
         %sec = $RTB::CModManager::Cache::Section[%i];
         %sec.position = vectorSub(%sec.position,"0" SPC %extent);
      }
   }
   else
   {
      for(%i=0;%i<%section.getCount()-1;%i++)
      {
         %row = %section.getObject(%i);
         if(getWord(%row.position,1) > %pos)
            %row.position = vectorSub(%row.position,0 SPC %extent);
      }
      
      %section.extent = vectorSub(%section.extent,0 SPC %extent);
      %section.originalHeight -= %extent;

      for(%i=%id+1;%i<$RTB::CModManager::Cache::NumSections;%i++)
      {
         %sec = $RTB::CModManager::Cache::Section[%i];
         %sec.position = vectorSub(%sec.position,"0" SPC %extent);
      }
      
      for(%i=0;%i<%section.getCount()-1;%i++)
      {
         %row = %section.getObject(%i);
         if(%row.getClassName() $= "GuiMouseEventCtrl")
         {
            %section.pushToBack(%row);
            continue;
         }
      }
   }
   RTBMM_GUI_AutoResize();

   if(!%noDel)
   {
      %filepath = %cache.file_path;
      fileDelete(%filepath);
      %cache.delete();
      
      for(%i=0;%i<RTBMM_GroupManager.getCount();%i++)
      {
         %group = RTBMM_GroupManager.getObject(%i);
         %group.removeItem(%cache);
      }
   }
}

//- RTBMM_ModsView_syncAddons (Attempts to match old add-ons to the new system)
function RTBMM_ModsView_syncAddons()
{
   %window = RTBMM_GUI_createWindow("Sync Add-Ons");
   %window.resize(0,0,600,500);
   %window.setName("RTBMM_SyncAddons");
   RTBMM_GUI_Center(%window);
   
   %text = new GuiMLTextCtrl()
   {
      position = "8 8";
      extent = "580 1";
      text = "<color:666666><font:Arial:13>RTB has attempted to find matches for all your RTB v2 Add-Ons that exist on the RTB v3 Downloads System. Below are the results turned up by our matching process which checks the name of zip files and file titles. Please check the following:<br><br>        <bitmap:add-ons/system_blockosystem/images/bullet_news>That you either have a direct match of filename, or a close match of file title.<br>        <bitmap:add-ons/system_blockosystem/images/bullet_news>That the authors are the same as the add-on you are trying to sync.<br><br>Select the match you'd like to use, and then press the sync button to download them. Note that the old versions of your files will be deleted so if you still want them, take a copy before completing this process.";
   };
   %window.canvas.add(%text);
   
   %parent = new GuiSwatchCtrl()
   {
      position = "10 120";
      extent = "560 300";
      color = "200 200 200 255";
      
      new GuiSwatchCtrl()
      {
         position = "1 1";
         extent = "558 298";
         color = "245 245 245 255";
      };
   };
   %window.canvas.add(%parent);
   
   %scroll = new GuiScrollCtrl()
   {
      profile = RTBMM_ScrollProfile;
      position = "10 120";
      extent = "574 300";
      hScrollBar = "AlwaysOff";
      
      new GuiSwatchCtrl(RTBMM_SyncMods_Window)
      {
         position = "1 1";
         extent = "558 298";
         color = "0 0 0 0";
         
         new GuiSwatchCtrl()
         {
            position = "1 1";
            extent = "558 30";
            color = "0 0 0 0";
            
            new GuiSwatchCtrl()
            {
               position = "0 0";
               extent = "243 30";
               color = "215 215 215 255";
               
               new GuiMLTextCtrl()
               {
                  position = "35 8";
                  extent = "172 14";
                  text = "<just:center><color:888888><font:Arial Bold:14>Non-Synchronised Add-On";
               };
            };
            
            new GuiSwatchCtrl()
            {
               position = "244 0";
               extent = "40 30";
               color = "215 215 215 255";
            };
            
            new GuiSwatchCtrl()
            {
               position = "285 0";
               extent = "271 30";
               color = "215 215 215 255";
               
               new GuiMLTextCtrl()
               {
                  position = "35 8";
                  extent = "172 14";
                  text = "<just:center><color:888888><font:Arial Bold:14>Closest RTB Match";
               };
            };
         };
      };
   };
   %window.canvas.add(%scroll);
   
   %syncBtn = new GuiBitmapButtonCtrl()
   {
      position = "255 430";
      extent = "82 25";
      bitmap = $RTB::Path@"images/buttons/large/gray/btnSync";
      command = "RTBMM_ModsView_syncComplete();";
      text = " ";
   };
   %window.canvas.add(%syncBtn);
   
   %loadOverlay = new GuiSwatchCtrl()
   {
      position = "11 121";
      extent = "558 298";
      color = "255 255 255 150";
      
		new GuiAnimatedBitmapCtrl()
		{
		   position = "263 133";
		   extent = "31 31";
		   bitmap = "./images/image_loadRing";
		   framesPerSecond = 15;
		   numFrames = 8;
		};
   };
   %window.canvas.add(%loadOverlay);
   $RTB::CModManager::Sync::LoadOverlay = %loadOverlay;
   
   RTBMM_ModsView_syncStart();
}

//- RTBMM_ModsView_syncStart (starts the sync process)
function RTBMM_ModsView_syncStart()
{
   $RTB::CModManager::Sync::CurrFile = 0;
   $RTB::CModManager::Sync::NumFiles = 0;
   $RTB::CModManager::SyncResults::CBoxes = 0;
   for(%i=0;%i<RTBMM_FileCache.getCount();%i++)
   {
      %file = RTBMM_FileCache.getObject(%i);
      if(%file.file_platform !$= "rtb" && %file.file_type !$= "Default Add-Ons" &&  !%file.file_isContent)
      {
         $RTB::CModManager::Sync::File[$RTB::CModManager::Sync::NumFiles] = %file.file_path;
         $RTB::CModManager::Sync::NumFiles++;
      }
   }
   
   if($RTB::CModManager::Sync::NumFiles > 0)
      RTBMM_ModsView_syncProcess();
   else
      RTBMM_GUI_createMessageBoxOK("Oh Dear","You don't have any Add-Ons that need to be synchronised!");
}

//- RTBMM_ModsView_syncProcess (attempts to sync a single add-on)
function RTBMM_ModsView_syncProcess()
{
   if(!isObject(RTBMM_SyncMods_Window))
      return;
      
   if($RTB::CModManager::Sync::CurrFile >= $RTB::CModManager::Sync::NumFiles)
   {
      RTBMM_ModsView_syncFinish();
      return;
   }
      
   %target = $RTB::CModManager::Sync::File[$RTB::CModManager::Sync::CurrFile];
   if(isObject(RTBMM_FileCache.getByPath(%target)))
   {
      %file = RTBMM_FileCache.getByPath(%target);
      RTBMM_SendRequest("SYNCADDON",3,%file.file_zip,%file.file_title,%file);
      $RTB::CModManager::Sync::CurrFile++;
   }
}

//- RTBMM_ModsView_syncReply (sync reply from the server)
%RTBMM_SB.registerResponseHandler("SYNCADDON","RTBMM_ModsView_syncReply");
function RTBMM_ModsView_syncReply(%tcp,%line,%find)
{
   if(!isObject(RTBMM_SyncMods_Window))
      return;
      
   %cache = getField(%tcp.t_rawString,2);
   
   if(%find)
      RTBMM_ModsView_syncDisplay(1,%cache,getField(%line,1),getField(%line,2),getField(%line,3),getField(%line,4),getField(%line,5),getField(%line,6));
   else
      RTBMM_ModsView_syncDisplay(0,%cache);
}

//- RTBMM_ModsView_syncDisplay (adds a result to the sync gui)
function RTBMM_ModsView_syncDisplay(%find,%cache,%file_id,%file_icon,%file_title,%file_author,%file_zip,%match)
{
   %rows = RTBMM_SyncMods_Window.getCount()-1;
   %ySpace = (%rows*40) + (%rows+1) + 31;
   
   %container = new GuiSwatchCtrl()
   {
      position = "1" SPC %ySpace;
      extent = "556 40";
      color = "0 0 0 0";
   };
   RTBMM_SyncMods_Window.add(%container);
   RTBMM_SyncMods_Window.resize(1,1,558,%ySpace+40+1);
   
   %icon = new GuiSwatchCtrl()
   {
      position = "0 0";
      extent = "40 40";
      color = "230 230 230 255";
      
      new GuiBitmapCtrl()
      {
         position = "12 12";
         extent = "16 16";
         bitmap = $RTB::Path@"images/icons/"@%cache.file_icon;
      };
   };
   %container.add(%icon);
   
   %info = new GuiSwatchCtrl()
   {
      position = "41 0";
      extent = "202 40";
      color = "230 230 230 255";
      
      new GuiMLTextCtrl()
      {
         position = "4 1";
         extent = "300 1";
         text = "<font:Arial Bold:14><color:777777>"@%cache.file_title;
      };
      
      new GuiMLTextCtrl()
      {
         position = "3 12";
         extent = "300 1";
         text = "<font:Arial:13><color:888888>by "@%cache.file_author;
      };
      
      new GuiMLTextCtrl()
      {
         position = "4 25";
         extent = "300 1";
         text = "<font:Arial:13><color:AAAAAA>"@%cache.file_zip;
      };
   };
   %container.add(%info);
   
   %icon = new GuiSwatchCtrl()
   {
      position = "244 0";
      extent = "40 40";
      color = "230 230 230 255";
      
      new GuiBitmapCtrl()
      {
         position = "0 12";
         extent = "16 16";
         bitmap = $RTB::Path@"images/bar_gray";
      };

      new GuiBitmapCtrl()
      {
         position = "8 12";
         extent = "16 16";
         bitmap = $RTB::Path@"images/bar_gray";
      };
      
      new GuiBitmapCtrl()
      {
         position = "16 12";
         extent = "16 16";
         bitmap = $RTB::Path@"images/bar_gray";
      };
      
      new GuiBitmapCtrl()
      {
         position = "24 12";
         extent = "16 16";
         bitmap = $RTB::Path@"images/bar_gray";
      };
   };
   %container.add(%icon);
   
   %icon.getObject(0).setBitmap($RTB::Path@"images/bar_red");
   
   if(%find)
   {
      if(%match <= 50)
         %icon.getObject(0).setBitmap($RTB::Path@"images/bar_red");
      else if(%match < 70)
      {
         %icon.getObject(0).setBitmap($RTB::Path@"images/bar_yellow");
         %icon.getObject(1).setBitmap($RTB::Path@"images/bar_yellow");
      }
      else if(%match < 100)
      {
         %icon.getObject(0).setBitmap($RTB::Path@"images/bar_yellow");
         %icon.getObject(1).setBitmap($RTB::Path@"images/bar_yellow");
         %icon.getObject(2).setBitmap($RTB::Path@"images/bar_yellow");
      }
      else if(%match $= 100)
      {
         %icon.getObject(0).setBitmap($RTB::Path@"images/bar_green");
         %icon.getObject(1).setBitmap($RTB::Path@"images/bar_green");
         %icon.getObject(2).setBitmap($RTB::Path@"images/bar_green");
         %icon.getObject(3).setBitmap($RTB::Path@"images/bar_green");
      }
      
      %icon = new GuiSwatchCtrl()
      {
         position = "285 0";
         extent = "40 40";
         color = "230 230 230 255";
         
         new GuiBitmapCtrl()
         {
            position = "12 12";
            extent = "16 16";
            bitmap = $RTB::Path@"images/icons/"@%file_icon;
         };
      };
      %container.add(%icon);
      
      %info = new GuiSwatchCtrl()
      {
         position = "326 0";
         extent = "202 40";
         color = "230 230 230 255";
         
         new GuiMLTextCtrl()
         {
            position = "4 1";
            extent = "300 1";
            text = "<font:Arial Bold:14><color:777777>"@%file_title;
         };
         
         new GuiMLTextCtrl()
         {
            position = "3 12";
            extent = "300 1";
            text = "<font:Arial:13><color:888888>by "@%file_author;
         };
         
         new GuiMLTextCtrl()
         {
            position = "4 25";
            extent = "300 1";
            text = "<font:Arial:13><color:AAAAAA>"@%file_zip;
         };
      };
      %container.add(%info);
      
      %tick = new GuiSwatchCtrl()
      {
         position = "529 0";
         extent = "27 40";
         color = "230 230 230 255";
         
         new GuiCheckboxCtrl()
         {
            profile = RTBMM_CheckBoxProfile;
            position = "8 5";
            text = " ";
         };
      };
      %container.add(%tick);
      
      if(%match $= 100)
         %tick.getObject(0).setValue(1);
         
      %tick.getObject(0).cache = %cache;
      %tick.getObject(0).syncTarget = %file_id;
      $RTB::CModManager::SyncResults::CBox[$RTB::CModManager::SyncResults::CBoxes] = %tick.getObject(0);
      $RTB::CModManager::SyncResults::CBoxes++;
   }
   else
   {
      %info = new GuiSwatchCtrl()
      {
         position = "285 0";
         extent = "243 40";
         color = "230 230 230 255";
         
         new GuiMLTextCtrl()
         {
            position = "24 3";
            extent = "195 32";
            text = "<font:Impact:32><color:D5D5D5><just:center>No Matches";
         };
      };
      %container.add(%info);
      
      %tick = new GuiSwatchCtrl()
      {
         position = "529 0";
         extent = "27 40";
         color = "230 230 230 255";
         
         new GuiCheckboxCtrl()
         {
            profile = RTBMM_CheckBoxProfile;
            position = "8 5";
            text = " ";
         };
         
         new GuiSwatchCtrl()
         {
            position = "0 0";
            extent = "27 40";
            color = "0 0 0 20";
         };
      };
      %container.add(%tick);
   }
}

//- RTBMM_ModsView_onReplyStop (sync reply is done so lets do the next)
function RTBMM_ModsView_syncReplyStop()
{
   RTBMM_ModsView_syncProcess();
}

//- RTBMM_ModsView_syncFinish (sync is complete so display results)
function RTBMM_ModsView_syncFinish()
{
   $RTB::CModManager::Sync::LoadOverlay.delete();
   
   %rows = RTBMM_SyncMods_Window.getCount()-1;
   %ySpace = (%rows*40) + (%rows+1) + 31;
   
   %swatch = new GuiSwatchCtrl()
   {
      position = "1" SPC %ySpace;
      extent = "556 30";
      color = "215 215 215 255";
   };
   RTBMM_SyncMods_Window.add(%swatch);
   RTBMM_SyncMods_Window.resize(1,1,558,%ySpace+31);
}

//- RTBMM_ModsView_syncComplete (process selected files)
function RTBMM_ModsView_syncComplete()
{
   if($RTB::CModManager::SyncResults::CBoxes <= 0)
   {
      RTBMM_GUI_closeWindow(RTBMM_SyncAddons);
      RTBMM_GUI_createMessageBoxOK("Oh Dear","No matches could be found for your add-ons.");
      return;
   }
   
   for(%i=0;%i<$RTB::CModManager::SyncResults::CBoxes;%i++)
   {
      if($RTB::CModManager::SyncResults::CBox[%i].getValue() $= 1)
         %ticked++;
   }
   
   if(%ticked <= 0)
   {
      RTBMM_GUI_createMessageBoxOK("Hmm?","You have not selected any add-ons to sync.");
      return;
   }
   
   for(%i=0;%i<$RTB::CModManager::SyncResults::CBoxes;%i++)
   {
      if($RTB::CModManager::SyncResults::CBox[%i].getValue() $= 1)
      {
         %cbox = $RTB::CModManager::SyncResults::CBox[%i];
         %cache = %cbox.cache;
         %file = %cbox.syncTarget;
         
         fileDelete(%cache.file_path);
         %cache.delete();
         
         RTBMM_TransferView_Add(%file);
      }
   }
   RTBMM_TransferView_Init();
}

//- RTBMM_ModsView_InitGroupsView (initates the groups view)
function RTBMM_ModsView_InitGroupsView()
{
   %header = $RTB::CModManager::Cache::SectionHeader;
   
   %header.getObject(1).setText("<color:FAFAFA><just:left><font:Impact:18>  Your Groups");
   
   %syncBtn = new GuiBitmapButtonCtrl()
   {
      position = "184 4";
      extent = "68 18";
      text = " ";
      bitmap = "./images/buttons/medium/btnCreate";
      command = "RTBMM_ModsView_createGroupAsk();";
   };
   %header.add(%syncBtn);   
   
   $RTB::CModManager::Cache::NumSections = 0;   
   
   if(RTBMM_GroupManager.getCount() <= 0)
   {
      RTBMM_GUI_createMessage("<br><br>You do not have any Add-On Groups.<br><br>");
      return;
   }
   else
   {
      %sortString = "";
      for(%i=0;%i<RTBMM_GroupManager.getCount();%i++)
      {
         %group = RTBMM_GroupManager.getObject(%i);
         %sortString = %sortString@%group.getID()@"=>"@%group.name@",";
      }
      %sortString = getSubStr(%sortString,0,strLen(%sortString)-1);
      %sortString = strReplace(sortFields(%sortString),",","\t");
      
      for(%i=0;%i<getFieldCount(%sortString);%i++)
      {
         %sort = strReplace(getField(%sortString,%i),"=>","\t");
         %id = getField(%sort,0);
         %name = getField(%sort,1);
         
         if($RTB::CModManager::SectionCollapsed["G_"@%name])
            RTBMM_ModsView_createGroupRow(%id,1);
         else
            RTBMM_ModsView_createGroupRow(%id,0);
      }
   }
}

function RTBMM_ModsView_createGroupRow(%group,%hide)
{
   %container = new GuiSwatchCtrl()
   {
      position = 0 SPC $RTB::CModManager::GUI::CurrentY;
      extent = "680 28";
      color = "255 255 255 255";
   };
   RTBMM_GUI_PushControl(%container);
   
   %container.sec_id = $RTB::CModManager::Cache::NumSections;
   $RTB::CModManager::Cache::Section[$RTB::CModManager::Cache::NumSections] = %container;
   $RTB::CModManager::Cache::NumSections++;
   
   %s = (%group.items $= 1)?"":"s";
   %header = RTBMM_GUI_createHeader(1,"<just:left>        "@%group.name@"  <font:Arial:12>"@%group.items@" Add-On"@%s);
   
   %container.header = %header;
   %container.name = "G_"@%group.name;
   %container.files = %group.items;
   %bitmap = new GuiBitmapCtrl()
   {
      position = "5 6";
      extent = "16 16";
      bitmap = "./images/icon_arrowdown";
   };
   %header.add(%bitmap);
   %container.add(%header);
   %header.position = "0 0";
   %container.bitmap = %bitmap;
   
   %mouseEvent = new GuiMouseEventCtrl()
   {
      extent = "610 28";
      
      eventType = "sectionClick";
      eventCallbacks = "0001";
      
      id = $RTB::CModManager::Cache::NumSections-1;
      type = 1;
      
      container = %container;
   };
   %container.add(%mouseEvent);
   %container.mouseEvent = %mouseEvent;
   
   %add = new GuiBitmapButtonCtrl()
   {
      position = "620 5";
      extent = "16 16";
      bitmap = "./images/buttons/small/btnAdd";
      text = " ";
      command = "RTBMM_ModsView_groupAddonSelect("@%group@");";
   };
   %header.add(%add);
   
   %remove = new GuiBitmapButtonCtrl()
   {
      position = "640 5";
      extent = "16 16";
      bitmap = "./images/buttons/small/btnRemove";
      text = " ";
      command = "RTBMM_ModsView_removeGroup("@%group@");";
   };
   %header.add(%remove);

   %content = RTBMM_GUI_createContent(1,40,100);
   %container.add(%content);
   %container.bringToFront(%content);
   
   %content.vertSizing = "top";
   %content.position = 0 SPC getWord(%container.extent,1);
   RTBMM_ModsView_renderGroupContent(%content,%group);
   
   if(%hide)
   {
      %mouseEvent.type = 2;
      RTBMM_ModsView_collapseSection($RTB::CModManager::Cache::NumSections-1,1);
   }
}

//- RTBMM_ModsView_renderGroupContent (renders the mod rows in the content control)
function RTBMM_ModsView_renderGroupContent(%content,%group)
{
   if(%group.items <= 0)
   {
      %icon = new GuiBitmapCtrl()
      {
         position = "20" SPC ((%j*31)+15);
         extent = "16 16";
         bitmap = "./images/icon_help";
      };
      %content.add(%icon);
      
      %text = new GuiMLTextCtrl()
      {
         position = "40" SPC ((%j*31)+17);
         extent = "400 10";
         text = "<font:Verdana:12><color:888888>It looks like you don't have any add-ons in this group!";
      };
      %content.add(%text);

      %content.resize(getWord(%content.position,0),getWord(%content.position,1),getWord(%content.extent,0),getWord(%icon.position,1)+31);
      %content.getGroup().extent = vectorAdd(%content.getGroup().extent,"0" SPC getWord(%content.extent,1));
      %content.getGroup().originalHeight = getWord(%content.getGroup().extent,1);
      RTBMM_GUI_AutoResize();
      return;
   }
   
   %sortString = "";
   for(%i=0;%i<%group.items;%i++)
   {
      %sortString = %sortString@%group.item[%i]@"=>"@%group.item[%i].file_title@",";
   }
   %sortString = getSubStr(%sortString,0,strLen(%sortString)-1);
   %sortString = strReplace(sortFields(%sortString),",","\t");
   
   for(%j=0;%j<getFieldCount(%sortString);%j++)
   {
      %field = strReplace(getField(%sortString,%j),"=>","\t");
      %id = getField(%field,0);

      if(%id.file_type $= "Unpackaged Add-Ons")
         %icon = "./images/icon_help";
      else if(%id.file_platform $= "bl")
         %icon = "./images/icon_blLogo";
      else
         %icon = "./images/icon_rtbLogo";

      %icon = new GuiBitmapCtrl()
      {
         position = "20" SPC ((%j*31)+15);
         extent = "16 16";
         bitmap = %icon;
      };
      %content.add(%icon);
      
      %text = new GuiMLTextCtrl()
      {
         position = "40" SPC ((%j*31)+17);
         extent = "400 10";
         text = "<font:Verdana Bold:12><color:666666>"@%id.file_title@" <font:Verdana:12><color:888888>by "@%id.file_author;
      };
      %content.add(%text);
      
      %text = new GuiMLTextCtrl()
      {
         position = "350" SPC ((%j*31)+17);
         extent = "400 10";
         text = "<font:Verdana Bold:12><color:666666>"@%id.file_type;
      };
      %content.add(%text);
      
      %text = new GuiMLTextCtrl()
      {
         position = "460" SPC ((%j*31)+17);
         extent = "400 10";
         text = "<font:Verdana:12><color:666666>"@%id.file_zip;
      };
      %content.add(%text);
      
      %remove = new GuiBitmapButtonCtrl()
      {
         position = "640" SPC ((%j*31)+15);
         extent = "16 16";
         bitmap = "./images/buttons/small/btnRemove";
         text = " ";
         command = "RTBMM_ModsView_removeItem("@%group@","@%id@");";
      };
      %content.add(%remove);      
      
      if(%j > 0)
      {
         %divider = new GuiBitmapCtrl()
         {
            position = "15" SPC ((%j*31)+7);
            extent = "650 2";
            bitmap = "./images/ui/cellDivider_light";
         };  
         %content.add(%divider);
      }
   }
   %content.resize(getWord(%content.position,0),getWord(%content.position,1),getWord(%content.extent,0),getWord(%icon.position,1)+31);
   %content.getGroup().extent = vectorAdd(%content.getGroup().extent,"0" SPC getWord(%content.extent,1));
   %content.getGroup().originalHeight = getWord(%content.getGroup().extent,1);
   RTBMM_GUI_AutoResize();
}

//- RTBMM_ModsView_createGroupAsk (creates a prompt to make a group)
function RTBMM_ModsView_createGroupAsk()
{
   %window = RTBMM_GUI_createWindow("Create a Group");
   %window.resize(0,0,275,110);
   RTBMM_GUI_Center(%window);
   
   %text = new GuiMLTextCtrl()
   {
      position = "5 5";
      extent = "300 10";
      text = "<font:Verdana:12><color:888888>Please enter a name for your new Group:";
   };
   %window.canvas.add(%text);
   
   %edit = new GuiTextEditCtrl()
   {
      profile = RTBMM_TextEditProfile;
      position = "6 21";
      extent = "256 16";
      accelerator = "enter";
      altCommand = "RTBMM_ModsView_createGroup($thisControl.getValue());RTBMM_GUI_closeWindow("@%window@");";
   };
   %window.canvas.add(%edit);

   %button = new GuiBitmapButtonCtrl()
   {
      position = "205 43";
      extent = "58 25";
      bitmap = "./images/buttons/large/gray/btnOK";
      text = " ";
      command = "RTBMM_ModsView_createGroup("@%edit@".getValue());RTBMM_GUI_closeWindow("@%window@");";
   };
   %window.canvas.add(%button);
}

//- RTBMM_ModsView_createGroup (creates a new group)
function RTBMM_ModsView_createGroup(%name,%skip)
{
   if(%name $= "")
   {
      if(!%skip)
      {
         RTBMM_GUI_createMessageBoxOK("Ooops","You have not entered a name for your new group.");
         RTBMM_ModsView_createGroupAsk();
      }
      return;
   }
   
   if(%group = RTBMM_GroupManager.addGroup(%name))
   {
      if(!%skip)
         RTBMM_ModsView_groupAddonSelect(%group,1);
      return %group;
   }
   else
   {
      if(!%skip)
         RTBMM_GUI_createMessageBoxOK("Oh No!","You've already got a group called "@%name@".");
      return 0;
   }
}

//- RTBMM_ModsView_groupAddonSelect (allows selection of included add-ons)
function RTBMM_ModsView_groupAddonSelect(%group,%closeDelete)
{
   %window = RTBMM_GUI_createWindow("Add-On Selector");
   %window.resize(0,0,315,500);
   RTBMM_GUI_Center(%window);
   
   if(%closeDelete)
   {
      %window.closeButton.command = %window.closeButton.command@%group@".delete();RTBMM_GroupManager.saveDat();";
   }
  
   %text = new GuiMLTextCtrl()
   {
      position = "8 8";
      extent = "280 1";
      text = "<color:666666><font:Arial:13>Browse the list and tick/untick the add-ons you want to be in this group. You can have as many as you want.";
   };
   %window.canvas.add(%text);
   
   %parent = new GuiSwatchCtrl()
   {
      position = "7 40";
      extent = "277 380";
      color = "200 200 200 255";
      
      new GuiSwatchCtrl()
      {
         position = "1 1";
         extent = "275 378";
         color = "245 245 245 255";
      };
   };
   %window.canvas.add(%parent);
   
   %scroll = new GuiScrollCtrl()
   {
      profile = RTBMM_ScrollProfile;
      position = "7 40";
      extent = "292 379";
      hScrollBar = "AlwaysOff";
      
      new GuiSwatchCtrl(RTBMM_GroupSelect_Window)
      {
         position = "1 1";
         extent = "558 298";
         color = "0 0 0 0";
         
         new GuiSwatchCtrl()
         {
            position = "1 1";
            extent = "558 30";
            color = "0 0 0 0";
            
            new GuiSwatchCtrl()
            {
               position = "0 0";
               extent = "242 30";
               color = "215 215 215 255";
               
               new GuiMLTextCtrl()
               {
                  position = "35 8";
                  extent = "172 14";
                  text = "<just:center><color:888888><font:Arial Bold:14>Add-On";
               };
            };
            
            new GuiSwatchCtrl()
            {
               position = "243 0";
               extent = "30 30";
               color = "215 215 215 255";
            };
         };
      };
   };
   %window.canvas.add(%scroll);
   
   %allBtn = new GuiBitmapButtonCtrl()
   {
      position = "10 432";
      extent = "21 18";
      bitmap = $RTB::Path@"images/buttons/medium/btnAll";
      command = "RTBMM_ModsView_selectAll("@%window@");";
      text = " ";
   };
   %window.canvas.add(%allBtn);
   
   %enabledBtn = new GuiBitmapButtonCtrl()
   {
      position = "32 435";
      extent = "50 15";
      bitmap = $RTB::Path@"images/buttons/medium/btnEnabled";
      command = "RTBMM_ModsView_selectEnabled("@%window@");";
      text = " ";
   };
   %window.canvas.add(%enabledBtn);  
   
   %enabledBtn = new GuiBitmapButtonCtrl()
   {
      position = "83 435";
      extent = "34 15";
      bitmap = $RTB::Path@"images/buttons/medium/btnNone";
      command = "RTBMM_ModsView_selectNone("@%window@");";
      text = " ";
   };
   %window.canvas.add(%enabledBtn);  
   
   %okBtn = new GuiBitmapButtonCtrl()
   {
      position = "225 430";
      extent = "58 25";
      bitmap = $RTB::Path@"images/buttons/large/gray/btnOK";
      command = "RTBMM_ModsView_saveAddOnSelection("@%window@","@%group@");";
      text = " ";
   };
   %window.canvas.add(%okBtn);
   
   %numCats = 0;
   $RTB::CModManager::GroupSelect::CBoxes = 0;
   for(%i=0;%i<RTBMM_FileCache.getCount();%i++)
   {
      %cache = RTBMM_FileCache.getObject(%i);
      if(%cache.file_special !$= "")
         continue;
         
      %sorter[%cache.file_type] = %sorter[%cache.file_type]@%cache@"=>"@%cache.file_title@",";
       
      if(!%added[%cache.file_type])
      {
         %cat[%numCats] = %cache.file_type;
         %numCats++;
         
         %added[%cache.file_type] = 1;
      }
   }
   
   for(%i=0;%i<%numCats;%i++)
   {
      %catSorter = %catSorter@%i@"=>"@%cat[%i]@",";
   }
   %catSorter = getSubStr(%catSorter,0,strLen(%catSorter)-1);
   %cats = strReplace(sortFields(%catSorter),",","\t");
   
   for(%i=0;%i<getFieldCount(%cats);%i++)
   {
      %cat = strReplace(getField(%cats,%i),"=>","\t");
      
      if(%sorter[getField(%cat,1)] $= "")
         continue;      
      
      %rows = RTBMM_GroupSelect_Window.getCount()-1;
      %ySpace = (%rows*40) + (%rows+1) + 31;
      
      %container = new GuiSwatchCtrl()
      {
         position = "1" SPC %ySpace;
         extent = "275 40";
         color = "0 0 0 0";
      };
      RTBMM_GroupSelect_Window.add(%container);
      RTBMM_GroupSelect_Window.resize(1,1,558,%ySpace+40);
      
      %check = new GuiSwatchCtrl()
      {
         position = "0 0";
         extent = "273 40";
         color = "225 225 225 255";
         
         new GuiBitmapCtrl()
         {
            position = "8 12";
            extent = "16 16";
            bitmap = $RTB::Path@"images/icon_arrowright";
         };
         
         new GuiMLTextCtrl()
         {
            position = "24 14";
            extent = "200 16";
            text = "<font:Arial Bold:14><color:888888> "@getField(%cat,1);
         };
         
         new GuiCheckboxCtrl()
         {
            profile = RTBMM_CheckBoxProfile;
            position = "251 13";
            extent = "16 16";
            text = " ";
            catBox = getField(%cat,1);
            command = "RTBMM_ModsView_clickGroupCat($thisControl);";
            numEnabled = 0;
            numItems = 0;
         };
      };
      %container.add(%check);
      %catCheck = %check.getObject(2);
      $RTB::CModManager::GroupSelect::CBox[$RTB::CModManager::GroupSelect::CBoxes] = %check.getObject(2);
      $RTB::CModManager::GroupSelect::CBoxes++;
      
      %sorter[getField(%cat,1)] = getSubStr(%sorter[getField(%cat,1)],0,strLen(%sorter[getField(%cat,1)])-1);
      %mods = strReplace(sortFields(%sorter[getField(%cat,1)]),",","\t");
      
      for(%j=0;%j<getFieldCount(%mods);%j++)
      {
         %mod = strReplace(getField(%mods,%j),"=>","\t");
         %cache = getField(%mod,0);
            
         %rows = RTBMM_GroupSelect_Window.getCount()-1;
         %ySpace = (%rows*40) + (%rows+1) + 31;
         
         %container = new GuiSwatchCtrl()
         {
            position = "1" SPC %ySpace;
            extent = "275 40";
            color = "0 0 0 0";
         };
         RTBMM_GroupSelect_Window.add(%container);
         RTBMM_GroupSelect_Window.resize(1,1,558,%ySpace+40);
         
         %icon = new GuiSwatchCtrl()
         {
            position = "0 0";
            extent = "40 40";
            color = "230 230 230 255";
            
            new GuiBitmapCtrl()
            {
               position = "12 12";
               extent = "16 16";
               bitmap = $RTB::Path@"images/icons/"@%cache.file_icon;
            };
         };
         %container.add(%icon);
         
         %info = new GuiSwatchCtrl()
         {
            position = "41 0";
            extent = "201 40";
            color = "230 230 230 255";
            
            new GuiMLTextCtrl()
            {
               position = "4 1";
               extent = "300 1";
               text = "<font:Arial Bold:14><color:777777>"@%cache.file_title;
            };
            
            new GuiMLTextCtrl()
            {
               position = "3 12";
               extent = "300 1";
               text = "<font:Arial:13><color:888888>by "@%cache.file_author;
            };
            
            new GuiMLTextCtrl()
            {
               position = "4 25";
               extent = "300 1";
               text = "<font:Arial:13><color:AAAAAA>"@%cache.file_zip;
            };
         };
         %container.add(%info);
         
         %check = new GuiSwatchCtrl()
         {
            position = "243 0";
            extent = "30 40";
            color = "230 230 230 255";
            
            new GuiCheckBoxCtrl()
            {
               profile = RTBMM_CheckBoxProfile;
               position = "8 12";
               extent = "16 16";
               text = " ";
               catCheck = %catCheck;
               command = "RTBMM_ModsView_clickGroupMod($thisControl);";
            };
         };
         %container.add(%check);

         if(%group.hasItem(%cache.file_zipname))
         {
            %check.getObject(0).setValue(1);
            %catCheck.numEnabled++;
         }
         %catCheck.numItems++;            
            
         %check.getObject(0).cache = %cache;
         $RTB::CModManager::GroupSelect::CBox[$RTB::CModManager::GroupSelect::CBoxes] = %check.getObject(0);
         $RTB::CModManager::GroupSelect::CBoxes++;
      }
      
      if(%catCheck.numEnabled >= %catCheck.numItems)
         %catCheck.setValue(1);
   }
}

//- RTBMM_ModsView_clickGroupCat (click for a category box of the selector)
function RTBMM_ModsView_clickGroupCat(%this)
{
   for(%i=0;%i<$RTB::CModManager::GroupSelect::CBoxes;%i++)
   {
      %cbox = $RTB::CModManager::GroupSelect::CBox[%i];
      if(%cbox.cache.file_type $= %this.catBox)
      {
         %cbox.setValue(%this.getValue());   
         %this.numEnabled++;
      }
   }
   
   if(%this.numEnabled > %this.numItems)
      %this.numEnabled = %this.numItems;
}

//- RTBMM_ModsView_clickGroupMod (click for a mod in a category of the selector)
function RTBMM_ModsView_clickGroupMod(%this)
{
   if(%this.getValue() $= 1)
      %this.catCheck.numEnabled++;
   else
      %this.catCheck.numEnabled--;
      
   if(%this.catCheck.numEnabled >= %this.catCheck.numItems)
   {
      %this.catCheck.numEnabled = %this.catCheck.numItems;
      %this.catCheck.setValue(1);
   }
   else
      %this.catCheck.setValue(0);
}

//- RTBMM_ModsView_selectAll (checks all boxes in the group add-on selector)
function RTBMM_ModsView_selectAll(%window)
{
   for(%i=0;%i<$RTB::CModManager::GroupSelect::CBoxes;%i++)
   {
      %cbox = $RTB::CModManager::GroupSelect::CBox[%i];
      %cbox.setValue(1);
      if(%cbox.numItems !$= "")
         %cbox.numEnabled = %cbox.numItems;
   }
}

//- RTBMM_ModsView_selectEnabled (checks all boxes that are currently enabled)
function RTBMM_ModsView_selectEnabled(%window)
{
   RTBMM_ModsView_selectNone();
   for(%i=0;%i<$RTB::CModManager::GroupSelect::CBoxes;%i++)
   {
      %cbox = $RTB::CModManager::GroupSelect::CBox[%i];
      if($AddOn__[%cbox.cache.file_var] $= 1)
      {
         %cbox.setValue(1);
         RTBMM_ModsView_clickGroupMod(%cbox);
      }
   }
}

//- RTBMM_ModsView_selectNone (unchecks all boxes in the group add-on selector)
function RTBMM_ModsView_selectNone(%window)
{
   for(%i=0;%i<$RTB::CModManager::GroupSelect::CBoxes;%i++)
   {
      %cbox = $RTB::CModManager::GroupSelect::CBox[%i];
      %cbox.setValue(0);
      if(%cbox.numItems !$= "")
         %cbox.numEnabled = 0;
   }
}

//- RTBMM_ModsView_saveAddOnSelection (saves the current selection of add-ons)
function RTBMM_ModsView_saveAddOnSelection(%window,%group)
{
   %group.items = 0;
   for(%i=0;%i<$RTB::CModManager::GroupSelect::CBoxes;%i++)
   {
      %cbox = $RTB::CModManager::GroupSelect::CBox[%i];
      if(%cbox.getValue() $= 1 && %cbox.cache !$= "")
      {
         %group.item[%group.items] = %cbox.cache;
         %group.items++;
      }
   }
   RTBMM_GroupManager.saveDat();
   RTBMM_GUI_closeWindow(%window);
   schedule(1,0,"RTBMM_ModsView_Init","groups");
}

//- RTBMM_ModsView_removeItem (removes an item from a group)
function RTBMM_ModsView_removeItem(%group,%id)
{
   %group.removeItem(%id);
   
   RTBMM_GroupManager.saveDat();
   schedule(1,0,"RTBMM_ModsView_Init","groups");
}

//- RTBMM_ModsView_removeGroup (removes a group)
function RTBMM_ModsView_removeGroup(%id,%conf)
{
   if(!%conf)
   {
      RTBMM_GUI_createMessageBoxOKCancel("Fo real?","Are you sure you want to delete this group:<br><br><lmargin:10>"@%id.name@" containing "@%id.items@" add-ons?","RTBMM_ModsView_removeGroup("@%id@",1);");
      return;
   }
   %id.delete();
   RTBMM_GroupManager.saveDat();
   schedule(1,0,"RTBMM_ModsView_Init","groups");
}

//*********************************************************
//* Update Manager
//*********************************************************
//- RTBMM_checkForUpdates (searches for updates for add-ons the user has)
function RTBMM_checkForUpdates()
{
   %fo = new FileObject();
   
   %filepath = findFirstFile("Add-Ons/*_*/rtbInfo.txt");
   while(strlen(%filepath) > 0)
   {
      %f_id = "";
      %f_version = "";
      %fo.openForRead(%filepath);
      
      %oldMod = 0;
      while(!%fo.isEOF())
      {
         %line = %fo.readLine();
         if(strPos(%line,"Name:") $= 0)
         {
            %oldMod = 1;
            break;
         }
         if(getWord(%line,0) $= "id:")
            %f_id = getWord(%line,1);
         if(getWord(%line,0) $= "version:")
            %f_version = getWord(%line,1);
      }
      %fo.close();
      
      if(%oldMod)
      {
         %filepath = findNextFile("Add-Ons/*_*/rtbInfo.txt");
         continue;
      }

      if(%f_id > 0 && %f_version > 0)
      {
         %files = %files@%f_id@"-"@%f_version@".";
      }
      %filepath = findNextFile("Add-Ons/*_*/rtbInfo.txt");
   }
   %fo.delete();

   if(strLen(%files) > 1)
      %files = getSubStr(%files,0,strLen(%files)-1);
   else
      return;

   RTBMM_SendRequest("GETUPDATES",1,%files);
}

//- RTBMM_onModUpdatesStart (callback for start of transmission)
function RTBMM_onModUpdatesStart()
{
   RTB_ModUpdates_Window.clear();
   RTB_ModUpdates_Window.resize(1,1,332,201);
   
   $RTB::CModManager::ModUpdates = 0;
}

//- RTBMM_onModUpdates (reply after requesting updates)
%RTBMM_SB.registerResponseHandler("GETUPDATES","RTBMM_onModUpdates");
function RTBMM_onModUpdates(%tcp,%line)
{
   if(getField(%line,0) $= 1)
   {
      Canvas.pushDialog(RTB_ModUpdates);
      
      $RTB::CModManager::ModUpdate[$RTB::CModManager::ModUpdates] = getField(%line,1);
      $RTB::CModManager::ModUpdates++;
      RTBMM_UpdateManager_addUpdate(getField(%line,2),getField(%line,3),getField(%line,4),getField(%line,5),getField(%line,6),getField(%line,7));
   }
}

//- RTBMM_UpdateManager_addUpdate (adds an update row to the gui)
function RTBMM_UpdateManager_addUpdate(%icon,%title,%authors,%version,%date,%currVersion)
{
   %count = RTB_ModUpdates_Window.getCount();
   %position = (45*%count) + %count;
   
   %container = new GuiSwatchCtrl()
   {
      position = "0" SPC %position;
      extent = "332 45";
      color = "0 0 0 0";
      
      new GuiSwatchCtrl()
      {
         position = "0 0";
         extent = "45 45";
         color = "220 220 220 255";
         
         new GuiBitmapCtrl()
         {
            position = "14 14";
            extent = "16 16";
            bitmap = "add-ons/system_blockosystem/images/icons/"@%icon;
         };
      };
      
      new GuiSwatchCtrl()
      {
         position = "46 0";
         extent = "300 45";
         color = "210 210 210 255";
         
         new GuiMLTextCtrl()
         {
            position = "4 2";
            extent = "280 23";
            text = "<font:Arial Bold:14><color:666666>"@%title@"<just:right><font:Arial:13>You have version "@%currVersion;
         };
         
         new GuiMLTextCtrl()
         {
            position = "4 15";
            extent = "300 23";
            text = "<font:Arial:13><color:888888>"@%authors;
         };
         
         new GuiMLTextCtrl()
         {
            position = "4 30";
            extent = "280 23";
            text = "<font:Verdana Bold:12><color:777777>Version "@%version@"<just:right><font:Arial:13>"@%date;
         };
      };
   };
   RTB_ModUpdates_Window.add(%container);
   RTB_ModUpdates_Window.resize(1,1,332,getWord(%container.position,1)+getWord(%container.extent,1));
}

//- RTBMM_downloadUpdates (downloads all updates found)
function RTBMM_downloadUpdates()
{
   for(%i=0;%i<$RTB::CModManager::ModUpdates;%i++)
   {
      RTBMM_TransferView_Add($RTB::CModManager::ModUpdate[%i]);
   }
   Canvas.popDialog(RTB_ModUpdates);
   RTBMM_OpenModManager();
   RTBMM_TransferView_Init();
}

//#####################################################################################################
//
//     _____                              _   
//    / ____|                            | |  
//   | (___  _   _ _ __  _ __   ___  _ __| |_ 
//    \___ \| | | | '_ \| '_ \ / _ \| '__| __|
//    ____) | |_| | |_) | |_) | (_) | |  | |_ 
//   |_____/ \__,_| .__/| .__/ \___/|_|   \__|
//                | |   | |                   
//                |_|   |_|                   
//
//
//##################################################################################################### 

//*********************************************************
//* Group Class
//*********************************************************
if(!isObject(RTBMM_GroupManager))
{
   new ScriptGroup(RTBMM_GroupManager);
}

//- RTBMM_GroupManager::load (loads the existing groups.dat file)
function RTBMM_GroupManager::loadDat(%this)
{
   %this.clear();
   %this.numGroups = 0;
   %this.loaded = 1;
   
   if(isFile("config/client/rtb/groups.dat"))
   {
      %fo = new FileObject();
      if(%fo.openForRead("config/client/rtb/groups.dat"))
      {
         while(!%fo.isEOF())
         {
            %number++;
            %line = %fo.readLine();
         
            if(%number%2)
            {
               %group = %this.addGroup(%line);
               
               if(!isObject(%group))
               {
                  %fo.readLine();
                  continue;
               }
            }
            else
            {
               if(!isObject(%group))
               {
                  %fo.delete();
                  echo("\c2ERROR: Parse error in groups.dat (RTBMM_GroupManager::load)");
                  return;
               }
               
               %line = strReplace(%line,",","\t");
               for(%i=0;%i<getFieldCount(%line);%i++)
               {
                  %group.addItem(getField(%line,%i));
               }
            }
         }
      }
      %fo.delete();
   }
}

//- RTBMM_GroupManager::save (saves the group manager)
function RTBMM_GroupManager::saveDat(%this)
{
   %fo = new FileObject();
   if(!%fo.openForWrite("config/client/rtb/groups.dat"))
   {
      echo("\c2ERROR: Unable to write to groups.dat (RTBMM_GroupManager::save)");
      %fo.delete();
      return;
   }
   
   for(%i=0;%i<%this.getCount();%i++)
   {
      %itemLine = "";
      %group = %this.getObject(%i);
      %fo.writeLine(%group.name);
      for(%j=0;%j<%group.items;%j++)
      {
         %itemLine = %itemLine@%group.item[%j].file_zipname@",";
      }
      if(%itemLine !$= "")
         %itemLine = getSubStr(%itemLine,0,strLen(%itemLine)-1);
      %fo.writeLine(%itemLine);
   }
   %fo.close();
   %fo.delete();
}

//- RTBMM_GroupManager::addGroup (creates a new group)
function RTBMM_GroupManager::addGroup(%this,%name)
{
   if(%this.hasGroup(%name))
      return 0;
      
   %group = new ScriptObject()
   {
      class = "RTB_Group";
      
      name = %name;
      items = 0;
   };
   %this.add(%group);
   
   return %group;
}

//- RTBMM_GroupManager::hasGroup (checks if a group exists)
function RTBMM_GroupManager::hasGroup(%this,%name)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).name $= %name)
         return 1;
   }
   return 0;
}

//- RTBMM_GroupManager::deleteGroup (deletes an existing group)
function RTBMM_GroupManager::deleteGroup(%this,%name)
{
   if(!%this.hasGroup(%name))
      return 0;
      
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).name $= %name)
      {
         %this.getObject(%i).delete();
         %this.saveDat();
         return 1;
      }
   }
   return 0;
}

//- RTB_Group::addItem (adds an item to a group)
function RTB_Group::addItem(%this,%zip)
{
   if(%this.hasItem(%zip))
      return;
      
   if(RTBMM_FileCache.getByZip(%zip))
   {
      %this.item[%this.items] = RTBMM_FileCache.getByZip(%zip);
      %this.items++;
   }
}

//- RTB_Group::hasItem (checks if this group already has this item)
function RTB_Group::hasItem(%this,%zip)
{
   for(%i=0;%i<%this.items;%i++)
   {
      if(%this.item[%i].file_zipname $= %zip)
         return 1;
   }
   return 0;
}

//- RTB_Group::removeItem (removes an item from a group)
function RTB_Group::removeItem(%this,%id)
{
   if(!%this.hasItem(%id.file_zipname))
      return;

   %k = 0;
   for(%i=0;%i<%this.items;%i++)
   {
      if(%this.item[%i] $= %id)
         continue;
      %this.item[%k] = %this.item[%i];
      %k++;
   }
   %this.items--;
}

//*********************************************************
//* Transfer Queue
//*********************************************************
if(!isObject(RTBMM_TransferQueue))
{
   new ScriptGroup(RTBMM_TransferQueue);
}

//- RTBMM_TransferQueue::items (Dumps out a list of items in the queue)
function RTBMM_TransferQueue::items(%this)
{
   echo(%this.getCount()@" items");
   echo("----------------------------------------");
}

//- RTBMM_TransferQueue::getItem (Returns the scriptobject of the file id in the transfer list)
function RTBMM_TransferQueue::getItem(%this,%id)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).id $= %id)
         return %this.getObject(%i);
   }
   return 0;
}

//- RTBMM_TransferQueue::getItemPos (Gets the physical item position in the transfer queue)
function RTBMM_TransferQueue::getItemPos(%this,%id)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).id $= %id)
         return %i;
   }
   return 0;
}

//- RTBMM_TransferQueue::hasItem (Returns whether queue has an item with the specified id in the queue)
function RTBMM_TransferQueue::hasItem(%this,%id)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).id $= %id)
         return %this.getObject(%i);
   }
   return 0;
}

//- RTBMM_TransferQueue::updateIndicator (Updates the indicator on the transfers button with number of transfers)
function RTBMM_TransferQueue::updateIndicator(%this)
{
   RTBMM_GUI_setTransfers(%this.getCount());
}

//- RTBMM_TransferQueue::addItem (Adds a file id to the transfer queue and grabs the data for it)
function RTBMM_TransferQueue::addItem(%this,%id,%contentOnly,%group)
{
   if(%this.hasItem(%id))
      return 0;
      
   %item = new ScriptObject()
   {
      class = "TransferItem";
      
      id = %id;
      title = "File "@%id;
      desc = "Downloading Details...";
      progress_text = "Downloading Details...";
      
      content_only = %contentOnly;      
      
      zip = "";
      
      speed = "0b/s";
      progress = 0;
      filesize = 0;
      completed = 0;
      
      status = 1;
      
      live = 0;
      
      group = %group;
   };
   %this.add(%item);
   
   %item.update();
   RTBMM_SendRequest("GETFILEDATA",3,%id);
   
   %this.updateIndicator();

   RTBMM_FileGrabber.poke();   
   
   return %item;
}

//- TransferItem::update (Updates the physical gui with new information in the transfer item object)
function TransferItem::update(%this,%force)
{
   if($RTB::CModManager::Cache::CurrentZone $= "TransferView" && RTB_ModManager.isAwake())
   {
      if(%force || !isObject(%this.g_row))
         RTBMM_TransferView_Init();
      else
      {
         %this.g_title.setValue("<color:666666><font:Verdana Bold:13>"@%this.title);
         %this.g_desc.setValue("<color:999999><font:Verdana Bold:12>"@%this.desc);
         
         %this.g_speed.setValue("<just:right><color:999999><font:Verdana:12>"@%this.speed);
         %this.g_done.setValue("<just:right><color:999999><font:Verdana:12>"@byteRound(%this.done)@"/"@byteRound(%this.filesize));
         
         %this.g_progress.setValue(%this.progress/100);
         
         %this.g_transfer.setActive(1);
         %this.g_cancel.setActive(1);
         %this.g_up.setActive(1);         
         %this.g_down.setActive(1);
         
         for(%i=0;%i<RTBMM_TransferQueue.getCount();%i++)
         {
            if(RTBMM_TransferQueue.getObject(%i) $= %this)
               break;
         }
         
         if(%i == 0)
            %this.g_up.setActive(0);
         if(%i == RTBMM_TransferQueue.getCount()-1)
            %this.g_down.setActive(0);
         
         if(%this.completed)
         {
            %this.g_transfer.setActive(0);
            %this.g_cancel.setActive(0);
            %this.g_up.setActive(0);
            %this.g_down.setActive(0);
         }
         
         if(%this.transferring)
         {
            %this.g_transfer.setBitmap($RTB::Path@"images/buttons/medium/btnCancel");
            %this.g_transfer.command = "RTBMM_TransferView_HaltTransfer("@%i@");";
         }
         else
         {
            %this.g_transfer.setBitmap($RTB::Path@"images/buttons/medium/btnTransfer");
            %this.g_transfer.command = "RTBMM_TransferView_RequestTransfer("@%i@");";
         }
         
         if(%this.status == 1)
         {
            %this.g_progress.setVisible(1);
            %this.g_red.setVisible(0);
            %this.g_progress_text.setValue("<just:center><font:Verdana:12><color:999999>"@%this.progress_text);
         }
         else if(%this.status == 0)
         {
            %this.g_progress.setValue(0);
            %this.g_progress.setVisible(0);
            %this.g_red.setVisible(1);
            %this.g_progress_text.setValue("<just:center><font:Verdana Bold:12><color:FFEEEE>"@%this.progress_text);
         }
      }
   }
   
   if(RTB_ServerInformation.isAwake())
   {
      if(!isObject(%this.sg_statusSW))
         return;
         
      %this.sg_statusSW.setVisible(1);
      %this.sg_dlBtn.setVisible(1);
      
      if(%this.transferring)
      {
         %this.sg_dlBtn.setVisible(0);
      }
      %this.sg_statusSW.getObject(0).setValue("<font:Verdana:12><color:999999>"@%this.progress_text);
         
      for(%i=0;%i<%this.sg_progStd.getCount();%i++)
      {
         if(%this.progress >= ((100/8)*(%i+1)))
            %this.sg_progStd.getObject(%i).setBitmap($RTB::Path@"images/bullet_green");
      }
         
      if(%this.status == 1)
      {
         %this.sg_progStd.setVisible(1);
         %this.sg_progRed.setVisible(0);
      }
      else if(%this.status == 0)
      {
         %this.sg_progRed.setVisible(1);
         %this.sg_progStd.setVisible(0);
      }
      
      if(%this.completed)
      {
         %this.sg_indicator.setBitmap($RTB::Path@"images/icon_tick");
      }
   }
   
   if(Canvas.getContent().getName() $= "LoadingGui")
   {
      if(!%this.live)
      {
         LoadingProgressTxt.setText("RETRIEVING RTB ADD-ON DETAILS");
         return;
      }

      if(%this.transferring)
      {
         %addonsDone = $RTB::CContentDownload::Addons-RTBMM_TransferQueue.getCount();
         LoadingProgress.setValue((%addonsDone/$RTB::CContentDownload::Addons) + ((1/$RTB::CContentDownload::Addons) * (%this.progress/100)));
      }
      
      if(RTBMM_TransferQueue.getObject(0) $= %this)
         LoadingProgressTxt.setText("Downloading " @ %this.zip @ " ...");
         
      if(%this.completed && %this.id = $RTB::CContentDownload::Cache::Map)
      {
         if(isFile($RTB::CContentDownload::Cache::MapImage@".png") || isFile($RTB::CContentDownload::Cache::MapImage@".jpg"))
            LOAD_MapPicture.setBitmap($RTB::CContentDownload::Cache::MapImage);
      }
   }
}

//- RTBMM_TransferQueue::removeItem (Removes an item from the transfer queue)
function RTBMM_TransferQueue::removeItem(%this,%id)
{
   if(%item = %this.getItem(%id))
   {
      if(RTBMM_FileGrabber.halted && RTBMM_FileGrabber.halter $= %item)
         RTBMM_FileGrabber.halted = 0;
         
      %position = %this.getItemPos(%id);
      %item.delete();
      if(%position < RTBMM_TransferQueue.getCount())
         RTBMM_TransferQueue.pushToBack(RTBMM_TransferQueue.getObject(%position));
      if($RTB::CModManager::Cache::CurrentZone $= "TransferView" && RTB_ModManager.isAwake())
         RTBMM_TransferView_Init();
      %this.updateIndicator();
      
      if(Canvas.getContent().getName() $= "LoadingGui")
         if(%this.getCount() <= 0)
         {
            $RTB::CContentDownload::Cache::Downloading = 0;
            commandtoserver('MissionPreparePhase1End');
         }
   }
}

//*********************************************************
//* Add-Ons Cache (No more RM faggotry)
//*********************************************************
if(!isObject(RTBMM_FileCache))
{
   new ScriptGroup(RTBMM_FileCache)
   {
      class = "FileCache";
      
      refreshTime = 0;
   };
}

//- FileCache::refresh (Refreshes the list of files discovered by the FC)
function FileCache::refresh(%this)
{
   %this.clear();
   %this.refreshTime = getSimTime();
   
   %filepath = findFirstFile("Add-Ons/*_*/description.txt");
   while(strlen(%filepath) > 0)
   {
      %zip = getSubStr(%filepath,0,strPos(strlwr(%filepath),"/description.txt"))@".zip";
      %this.addPath(%zip);
      %filepath = findNextFile("Add-Ons/*_*/description.txt");
   }
   %this.refreshed = 1;
}

//- FileCache::list (Lits all files in the cache)
function FileCache::list(%this)
{
   echo(%this.getCount());
   echo("-----------------------------------------------");
   for(%i=0;%i<%this.getCount();%i++)
   {
      echo(%this.getObject(%i).file_path SPC "\c2" @ %this.getObject(%i).file_zipname@"\c0 ("@%this.getObject(%i).file_type@")");
   }
   echo("");
}

//- FileCache::addPath (Adds a path to the file cache)
function FileCache::addPath(%this,%filepath)
{
   if(isFile(%filepath) || isFile(strReplace(%filepath,".zip","")@"/description.txt"))
   {
      if(%filepath $= "Add-Ons/System_ReturnToBlockland.zip" || %filepath $= "Add-Ons/Colorset_Default.zip")
         return;
         
      if(%this.exists(%filepath))
      {
         %oldCacheId = %this.getByPath(%filepath);
         %this.removeFile(%filepath);
      }
         
      %dir = getSubStr(%filepath,0,strPos(%filepath,".zip"))@"/";
      %zipname = fileBase(%filepath)@".zip";
      
      if(!isFile(%dir@"description.txt"))
         return;
      if(isFile(%dir@"rtbContent.txt"))
         %isContent = 1;
      if(isFile(%dir@"rtbInfo.txt"))
         %isRTB = 1;
   
      %fo = new FileObject();
      %fo.openForRead(%dir@"description.txt");
      while(!%fo.isEOF())
      {
         %l = %fo.readLine();
         if(strPos(%l,"Title:") $= 0)
            %f_title = trim(getSubStr(%l,6,strLen(%l)));
         else if(strPos(%l,"Author:") $= 0)
            %f_author = trim(getSubStr(%l,7,strLen(%l)));
         else if(%f_title !$= "" && %f_author !$= "")
            %f_desc = %l;
      }
      %fo.close();
      
      if(%f_title $= "")
         %f_title = fileBase(%filepath)@".zip";
      if(%f_author $= "")
         %f_author = "Unknown Author";
      
      %f_default = 0;
      %isOldRTB = 0;      
      if(%isRTB)
      {
         %fo.openForRead(%dir@"rtbInfo.txt");
         while(!%fo.isEOF())
         {
            %l = %fo.readLine();
            if(striPos(%l,"id:") $= 0)
               %f_id = trim(getSubStr(%l,3,strLen(%l)));
            else if(striPos(%l,"icon:") $= 0)
               %f_icon = trim(getSubStr(%l,5,strLen(%l)));
            else if(striPos(%l,"type:") $= 0)
               %f_type = trim(getSubStr(%l,5,strLen(%l)));
            else if(striPos(%l,"version:") $= 0)
               %f_version = trim(getSubStr(%l,8,strLen(%l)));
            else if(striPos(%l,"name:") $= 0)
               %isOldRTB = 1;
         }
      }
      %fo.close();
      
      if(%isContent)
      {
         %fo.openForRead(%dir@"rtbContent.txt");
         while(!%fo.isEOF())
         {
            %l = %fo.readLine();
            if(striPos(%l,"id:") $= 0)
               %f_id = trim(getSubStr(%l,3,strLen(%l)));
         }
      }
      %fo.close();
      %fo.delete();            
            
      if(%f_title $= "" || %f_author $= "" || %f_desc $= "" || ((%f_id $= "" || %f_icon $= "" || %f_type $= "" || %f_version $= "") && %isRTB))
         %isRTB = 0;

      if(!isFile(%filepath))
      {
         %f_type = "Unpackaged Add-Ons";
         %f_icon = "icon_help";
         %f_default = 1;
      }
      else if(%isContent)
      {
         %f_type = "Content-Only Add-Ons";
         %f_icon = "package_green";
      }
      else if(!%isRTB)
      {
         for(%i=0;%i<$RTB::CModManager::DefaultBLMods;%i++)
         {
            %f_type = "Non-RTB Add-Ons";
            %mod = $RTB::CModManager::DefaultBLMod[%i];
            if(%mod $= fileBase(%filepath))
            {
               %f_default = 1;
               %f_type = "Default Add-Ons";
               break;
            }
         }
         %f_icon = "blLogo";
         %f_platform = "bl";
      }
      else
      {
         if(%isOldRTB)
            %f_platform = "rtb2";
         else
            %f_platform = "rtb";
      }
      
      if(%isOldRTB)
      {
        %f_icon = "link_break";
        %f_type = "RTB2 Add-Ons";
      }
      
      if(getSubStr(fileBase(%filepath),0,4) $= "Map_")
         %f_special = "map";
      else if(getSubStr(fileBase(%filepath),0,6) $= "Decal_" || getSubStr(fileBase(%filepath),0,5) $= "Face_")
         %f_special = "decal";
      else if(isFile("Add-Ons/"@fileBase(%filepath)@"/client.cs") && !isFile("Add-Ons/"@fileBase(%filepath)@"/server.cs"))
         %f_special = "clientside";
      else if(getSubStr(fileBase(%filepath),0,9) $= "Colorset_")
         %f_special = "colorset";
      else
         %f_special = "";
         
      if(!clientIsValidAddOn(fileBase(%filepath),1))
         return;
      
      %file = new ScriptObject()
      {
         //standard props
         file_zip = %zipname;
         file_zipname = fileBase(%filepath);
         file_path = %filepath;
         file_title = %f_title;
         file_author = %f_author;
         file_desc = %f_desc;
         file_size = getFileLength(%filepath);
         file_var = getSafeVariableName(fileBase(%filepath));
         
         //management props
         file_platform = %f_platform;
         file_special = %f_special;
         file_default = %f_default;
         file_isContent = %isContent;
         
         //public props
         file_type = %f_type;         
         
         //rtb props
         file_id = %f_id;
         file_icon = %f_icon;
         file_version = %f_version;
      };
      %this.add(%file);
      
      if(%oldCacheId !$= "")
      {
         for(%i=0;%i<RTBMM_GroupManager.getCount();%i++)
         {
            %group = RTBMM_GroupManager.getObject(%i);
            for(%j=0;%j<%group.items;%j++)
            {
               if(%group.item[%j] $= %oldCacheId)
                  %group.item[%j] = %file;
            }
         }
      }
      
      return %file;
   }
}

//- FileCache::get (Returns the cache record for the file id)
function FileCache::get(%this,%id)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).file_id $= %id)
         return %this.getObject(%i);
   }
   return 0;
}

//- FileCache::getByPath (Returns the cache record for the filepath)
function FileCache::getByPath(%this,%path)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).file_path $= %path)
         return %this.getObject(%i);
   }
   return 0;
}

//- FileCache::getByZip (Returns the cache record for the zip)
function FileCache::getByZip(%this,%zip)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).file_zipname $= %zip)
         return %this.getObject(%i);
   }
   return 0;
}

//- FileCache::exists (Checks to see if the file cache knows about a file)
function FileCache::exists(%this,%filepath)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).file_path $= %filepath)
         return 1;
   }
   return 0;
}

//- FileCache::removeFile (Removes a file from the cache)
function FileCache::removeFile(%this,%filepath)
{
   for(%i=0;%i<%this.getCount();%i++)
   {
      if(%this.getObject(%i).file_path $= %filepath)
      {
         %this.getObject(%i).delete();
         return 1;
      }
   }
   return 0;
}

//*********************************************************
//* File Downloader
//*********************************************************
function RTBMM_FileGrabber_Init()
{
   if(isObject(RTBMM_FileGrabber))
   {
      RTBMM_FileGrabber.disconnect();
      RTBMM_FileGrabber.delete();
   }
   
   new TCPObject(RTBMM_FileGrabber)
   {
      host = "api.returntoblockland.com";
      port = 80;
      
      queue = 0;
      
      connected = 0;
   };
   
   RTBMM_FileGrabber.schedule(500,"poke");
}
RTBMM_FileGrabber_Init();

//- RTBMM_FileGrabber::poke (Pokes the FileGrabber to load a new item if appropriate)
function RTBMM_FileGrabber::poke(%this)
{
   if(%this.connected)
      return;
   if(isObject(%this.queue))
      return;
   if(%this.halted)
      return;
      
   for(%i=0;%i<RTBMM_TransferQueue.getCount();%i++)
   {
      if(!RTBMM_TransferQueue.getObject(%i).completed && RTBMM_TransferQueue.getObject(%i).status $= 1)
      {
         RTBMM_FileGrabber.loadItem(%i);
         return;
      }
   }
}

//- RTBMM_FileGrabber::loadItem (Loads an item from the transfer queue of %index)
function RTBMM_FileGrabber::loadItem(%this,%index)
{
   if(isObject(%this.queue))
      return;
   else if(%this.connected)
      RTBMM_FileGrabber_Init();
   
   if(isObject(%item = RTBMM_TransferQueue.getObject(%index)) && %item.live)
   {
      %this.queue = %item;
      %item.status = 1;
      %item.transferring = 1;
      %item.update();
      %this.doConnect();
   }
   else
      return 0;
}

//- RTBMM_FileGrabber::doConnect (Begins connecting to the server)
function RTBMM_FileGrabber::doConnect(%this)
{
   %this.queue.progress_text = "Connecting...";
   %this.queue.update();
   
   %this.connect(%this.host@":"@%this.port);
}

//- RTBMM_FileGrabber::onConnected (Begins the sending sequence and prepares string for sending)
function RTBMM_FileGrabber::onConnected(%this)
{
   %this.connected = 1;
   
   if(%this.queue.content_only)
      %content = "c=DOWNLOADCONTENT&n="@$Pref::Player::NetName@"&arg1="@%this.queue.id@"&"@$RTB::Connection::Session;
   else
      %content = "c=DOWNLOADFILE&n="@$Pref::Player::NetName@"&arg1="@%this.queue.id@"&"@$RTB::Connection::Session;
   %contentLen = strLen(%content);
   
   %this.queue.progress_text = "Connected - Awaiting Data";
   %this.queue.update();
   
   %this.send("POST /apiRouter.php?d=APIMS HTTP/1.1\r\nHost: api.returntoblockland.com\r\nUser-Agent: Torque/1.0\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: "@%contentLen@"\r\n\r\n"@%content@"\r\n");
}

//- RTBMM_FileGrabber::onLine (Callback for a line return)
function RTBMM_FileGrabber::onLine(%this,%line)
{
   if(!%this.isOK)
   {
      if(strPos(%line,"200 OK") >= 0)
      {
         %this.isOK = 1;
         %this.queue.status = 1;
      }
      else
      {
         if(isObject(%this.queue))
         {
            %this.queue.status = 0;
            %this.queue.progress_text = "HTTP Error "@restWords(%line);
            %this.queue.update();
         }
         %this.disconnect();
         RTBMM_FileGrabber_Init();
         return;
      }
   }
   
   if(strPos(%line,"Content-Length:") $= 0)
      %this.contentSize = getWord(%line,1);
      
   if(strPos(%line,"Result:") $= 0)
   {
      %this.dlResult = getWord(%line,1);
      if(getWord(%line,1) !$= 1)
      {
         if(getWord(%line,2) $= 0)
         {
            %this.queue.desc = "File could not be found.";
            %this.queue.progress_text = "File Missing";
         }
         else if(getWord(%line,2) $= 1)
         {
            %this.queue.desc = "File has been failed by a moderator.";
            %this.queue.progress_text = "File in Fail Bin";            
         }
         else if(getWord(%line,2) $= 2)
         {
            %this.queue.desc = "Physical zip file is missing - report this file!";
            %this.queue.progress_text = "Physical File Missing";            
         }
         else
         {
            %this.queue.desc = "Unknown Error Occurred";
            %this.queue.progress_text = "Error Occurred";  
         }
         %this.queue.status = 0;
         %this.queue.update();
         %this.disconnect();
         RTBMM_FileGrabber_Init();
         return;
      }
   }
      
   if(%line $= "")
   {
      if(%this.dlResult !$= 1)
      {
         %this.queue.desc = "Unknown Error Occurred";
         %this.queue.progress_text = "Error Occurred";  
         %this.queue.status = 0;
         %this.queue.update();
         %this.disconnect();
         RTBMM_FileGrabber_Init();
         return;
      }
      %this.setBinarySize(%this.contentSize);
   }
      
   %this.startTime = getSimTime();
      
   %this.queue.update();
}

//- RTBMM_FileGrabber::onBinChunk (Transfers are binary-chunked so this callback happens upon chunk receival)
function RTBMM_FileGrabber::onBinChunk(%this,%bin)
{
   %this.queue.speed = byteRound(mFloatLength(%bin/(getSimTime()-%this.startTime),2))@"/s";
   %this.queue.done = %bin;
   %this.queue.progress = mCeil((%bin/%this.queue.filesize)*100);
   %this.queue.progress_text = %this.queue.progress@"%";
   %this.queue.update();
   
   if(%bin >= %this.contentSize)
   {
      if(isWriteableFilename("Add-Ons/"@%this.queue.zip))
      {
         %this.queue.speed = "0b/s";
         %this.queue.done = %this.queue.filesize;
         %this.queue.progress = 100;
         %this.queue.completed = 1;
         %this.queue.progress_text = "100%";
         %this.queue.update();
         
         if(Canvas.getContent().getName() $= "LoadingGui")
            RTBMM_TransferQueue.schedule(1,"removeItem",%this.queue.id);
         else
            RTBMM_TransferQueue.schedule(2000,"removeItem",%this.queue.id);
         RTBMM_GUI_addToMods();
         
         if(isFile("Add-Ons/"@%this.queue.zip))
         {
            %cache = RTBMM_FileCache.getByPath("Add-Ons/"@%this.queue.zip);
            fileDelete(%cache.file_path);
            
            if(isObject(%cache.physical_row) && RTB_ModManager.isAwake() && $RTB::CModManager::Cache::CurrentZone $= "ModsView")
               RTBMM_ModsView_deleteAddon(%cache,%cache.physical_row,1);

            %cache.delete();
         }
         %this.saveBufferToFile("Add-Ons/"@%this.queue.zip);
         discoverFile("Add-Ons/"@%this.queue.zip);
         %cache = RTBMM_FileCache.addPath("Add-Ons/"@%this.queue.zip);
         if(isObject(%this.queue.group))
            %this.queue.group.addItem(%cache.file_zipname);
            
         RTBMM_GroupManager.saveDat();
         
         if($RTB::CModManager::Cache::CurrentZone $= "ModsView")
            RTBMM_ModsView_insertModRow(%cache);
         
         %addonName = getSubStr(%this.queue.zip,0,strLen(%this.queue.zip)-4);
         if(isFile("Add-Ons/"@%addonName@"/client.cs"))
         {
            echo("Client checking Add-On: "@%addonName);
            if(clientIsValidAddOn(%addonName,1))
            {
               echo("\c4Loading Add-On: "@%addonName@" \c1(CRC: "@getFileCRC("Add-Ons/"@%this.queue.zip)@")");
               if(ClientVerifyAddOnScripts(%addonName))
               {
                  exec("Add-Ons/"@getSubStr(%this.queue.zip,0,strLen(%this.queue.zip)-4)@"/client.cs");
               }
            }
         }
      }
      else
      {
         %this.queue.speed = "0b/s";
         %this.queue.done = %this.queue.filesize;
         %this.queue.progress = 100;
         %this.queue.completed = 0;
         %this.queue.desc = "Unable to write to "@%this.queue.zip;
         %this.queue.progress_text = "Cannot Save Download";  
         %this.queue.status = 0;
         %this.queue.update();
      }
      
      %this.disconnect();
      RTBMM_FileGrabber_Init();
      return;
   }
}

//- RTBMM_FileGrabber::halt (Halts filegrabber process)
function RTBMM_FileGrabber::halt(%this)
{
   if(isObject(%this.queue))
   {
      %this.queue.transferring = 0;
      %this.queue.done = 0;
      %this.queue.percent = 0;
      %this.queue.speed = "0b/s";
      %this.queue.status = 0;
      %this.queue.progress_text = "User Cancelled";
      %this.queue.update();
      
      %queue = %this.queue;
   }
      
   %this.disconnect();
   RTBMM_FileGrabber_Init();
   RTBMM_FileGrabber.halted = 1;
   RTBMM_FileGrabber.halter = %queue;
}

//*********************************************************
//* Screenshot Loader
//*********************************************************
function RTBMM_ScreenGrabber_Init()
{
   if(isObject(RTBMM_ScreenGrabber))
   {
      RTBMM_ScreenGrabber.disconnect();
      RTBMM_ScreenGrabber.delete();
   }
      
   new TCPObject(RTBMM_ScreenGrabber)
   {
      host = "forum.returntoblockland.com";
      port = 80;
      
      cmd = "";
   };
}
RTBMM_ScreenGrabber_Init();

//- RTBMM_ScreenGrabber::getCollage (Prepares screengrabber to grab a collage image, and connects)
function RTBMM_ScreenGrabber::getCollage(%this,%url)
{
   %this.grabMode = "collage";
   %this.cmd = "GET "@%url@" HTTP/1.1\r\nHost: forum.returntoblockland.com\r\n\r\n";
   %this.doConnect();
}

//- RTBMM_ScreenGrabber::getScreenshot (Prepares screengrabber to grab a fullsize image, and connects)
function RTBMM_ScreenGrabber::getScreenshot(%this,%url,%window)
{
   %this.window = %window;
   %this.grabMode = "screenshot";
   %this.cmd = "GET "@%url@" HTTP/1.1\r\nHost: forum.returntoblockland.com\r\n\r\n";
   %this.doConnect();
}

//- RTBMM_ScreenGrabber::doConnect (Begins connection sequence)
function RTBMM_ScreenGrabber::doConnect(%this)
{
   %this.connect(%this.host@":"@%this.port);
}

//- RTBMM_ScreenGrabber::onConnected (Connection callback - sends the cmd prepared earlier)
function RTBMM_ScreenGrabber::onConnected(%this)
{
   %this.send(%this.cmd);
}

//- RTBMM_ScreenGrabber::onLine (Line callback, checks filesize etc)
function RTBMM_ScreenGrabber::onLine(%this,%line)
{
   if(strPos(%line,"Content-Length:") $= 0)
      %this.contentSize = getWord(%line,1);
      
   if(%line $= "")
      %this.setBinarySize(%this.contentSize);
}

//- RTBMM_ScreenGrabber::onBinChunk (Receives binary chunks then saves collage on complete)
function RTBMM_ScreenGrabber::onBinChunk(%this,%bin)
{
   if(%bin >= %this.contentSize)
   {
      if(%this.grabMode $= "collage")
      {
         %this.saveBufferToFile("config/client/rtb/cache/collage.png");
         
         if(!isObject($RTB::CModManager::Cache::ScreenControl[0]))
         {
            %this.disconnect();
            RTBMM_ScreenGrabber_Init();
            return;
         }
            
         for(%i=0;%i<$RTB::CModManager::Cache::ScreenCount;%i++)
         {
            %id = $RTB::CModManager::Cache::Screen[%i];
            %ctrl = $RTB::CModManager::Cache::ScreenControl[%i];
            
            %img = new GuiSwatchCtrl()
            {
               position = "2 2";
               extent = "112 84";
               color = "255 255 255 255";
               
               new GuiBitmapCtrl()
               {
                  position = "0 0";
                  extent = "224 168";
                  bitmap = "config/client/rtb/cache/collage.png";
               };

               new GuiSwatchCtrl()
               {
                  position = "0 0";
                  extent = "112 84";
                  color = "255 255 255 255";
               };
            };
            %ctrl.clear();
            %ctrl.add(%img);
            
            %swatch = new GuiSwatchCtrl()
            {
               position = "0 0";
               extent = "112 84";
               color = "255 255 255 0";
            };
            %img.add(%swatch);
            
            %mouseCtrl = new GuiMouseEventCtrl()
            {
               position = "0 0";
               extent = "112 84";
               
               eventType = "screenshotSelect";
               eventCallbacks = "1111";
               
               screenID = %i;
               screenCaption = $RTB::CModManager::Cache::ScreenCaption[%i];
               swatch = %swatch;
            };
            %img.add(%mouseCtrl);
            
            if(%i $= 1)
               %img.getObject(0).position = "-112 0";
            else if(%i $= 2)
               %img.getObject(0).position = "0 -84";
            else if(%i $= 3)
               %img.getObject(0).position = "-112 -84";
               
            RTBMM_GUI_FadeOut(%img.getObject(1));
         }
      }
      else if(%this.grabMode $= "screenshot")
      {
         %this.saveBufferToFile("config/client/rtb/cache/screen.png");
         
         %window = %this.window;
         if(!isObject(%window))
         {
            %this.disconnect();
            RTBMM_ScreenGrabber_Init();
            return;
         }
         %window.canvas.getObject(3).delete();
         %window.canvas.getObject(0).setBitmap("config/client/rtb/cache/screen.png");      
         %window.canvas.getObject(0).setVisible(1);
      }
      %this.disconnect();
      RTBMM_ScreenGrabber_Init();
   }
}

//*********************************************************
//* Mod Manager Support Functions
//*********************************************************
//- RTBMM_parseBBCode (Parses bbCode into TorqueML)
function RTBMM_parseBBCode(%message)
{
   %message = strReplace(%message,"<br>[*]","<br><bitmap:add-ons/system_blockosystem/images/bullet_list>");
   %message = strReplace(%message,"[*]","<br><bitmap:add-ons/system_blockosystem/images/bullet_list>");
   return %message;
}

function RTBMM_OpenModManager()
{
   $RTB::CModManager::Cache::OpenDirect = 1;
   Canvas.pushDialog(RTB_ModManager);
   $RTB::CModManager::Cache::OpenDirect = 0;
}

//*********************************************************
//* New Add-On Window
//*********************************************************
function addonsGui::onWake()
{
   if(RTBMM_FileCache.getCount() <= 0)
      RTBMM_FileCache.refresh();
   
   if(!RTBMM_GroupManager.loaded)
      RTBMM_GroupManager.loadDat();
      
   clientUpdateAddOnsList();
   
   %prefix["Brick"] = "Bricks";
   %prefix["Emote"] = "Emotes";
   %prefix["Event"] = "Events";
   %prefix["Gamemode"] = "Gamemodes";
   %prefix["Item"] = "Items";
   %prefix["Light"] = "Lights";
   %prefix["Particle"] = "Particles";
   %prefix["Player"] = "Players";
   %prefix["Print"] = "Prints";
   %prefix["Projectile"] = "Projectiles";
   %prefix["Script"] = "Server Mods";
   %prefix["Server"] = "Server Mods";
   %prefix["Sound"] = "Sound Effects";
   %prefix["System"] = "Systems";
   %prefix["Tool"] = "Tools";
   %prefix["Vehicle"] = "Vehicles";
   %prefix["Weapon"] = "Weapons";
   
   %AOG_CategoryCount = 0;
   AOG_Scroll.clear();
   %file = findFirstFile("Add-Ons/*_*/server.cs");
   while(strLen(%file) > 0)
   {
      %filename = getSubStr(%file,8,strLen(%file));
      %filename = getSubStr(%filename,0,strPos(%filename,"/"));
      if(!isFile("Add-Ons/"@%filename@"/description.txt") || !isFile("Add-Ons/"@%filename@"/server.cs") || %filename $= "System_ReturnToBlockland")
      {
         %file = findNextFile("Add-Ons/*_*/server.cs");
         continue;
      }
      %file_prefix = getSubStr(%filename,0,strPos(%filename,"_"));
      %new_prefix = %file_prefix;
      if(%prefix[%new_prefix] !$= "")
            %new_prefix = %prefix[%new_prefix];
      if(%AOG_hasCategory[%new_prefix] !$= "")
      {
         %iC = %AOG_ItemCount[%AOG_hasCategory[%new_prefix]];
         %AOG_Category[%AOG_hasCategory[%new_prefix],%iC] = %filename;
         %AOG_ItemCount[%AOG_hasCategory[%new_prefix]]++;
      }
      else
      {
         %AOG_Category[%AOG_CategoryCount,0] = %new_prefix;
         %AOG_Category[%AOG_CategoryCount,1] = %filename;
         %AOG_ItemCount[%AOG_CategoryCount] = 2;
         %AOG_hasCategory[%new_prefix] = %AOG_CategoryCount;
         %AOG_CategoryCount++;
      }
      %file = findNextFile("Add-Ons/*_*/server.cs");
   }
   
   %sortString = "";      
   for(%i=0;%i<%AOG_CategoryCount;%i++)
   {
      %sortString = %sortString@%i@"=>"@%AOG_Category[%i,0]@",";
   }
   %sortString = getSubStr(%sortString,0,strLen(%sortString)-1);
   %sortString = strReplace(sortFields(%sortString),",","\t");
   
   %swatch = new GuiSwatchCtrl()
   {
      position = "0 0";
      extent = "261 1000";
      color = "0 0 0 0";
   };
   AOG_Scroll.add(%swatch);
   
   %AOG_nextPos = 1;
   for(%i=0;%i<getFieldCount(%sortString);%i++)
   {
      %keypair = strReplace(getField(%sortString,%i),"=>","\t");
      %cat = getField(%keypair,0);      
      
      %bg = new GuiSwatchCtrl()
      {
         position = "5" SPC %AOG_nextPos+2;
         extent = "13 13";
         color = "0 0 0 255";
      };
      %swatch.add(%bg);
      %AOG_CatCheck[%cat] = new GuiCheckboxCtrl()
      {
         profile = GuiCheckBoxBoldProfile;
         position = "5" SPC %AOG_nextPos;
         extent = "256 18";
         text = " "@%AOG_Category[%cat,0];
         category = %AOG_Category[%cat,0];
      };
      %AOG_CatCheck[%cat].command = "AOG_tickCategory("@%AOG_CatCheck[%cat]@");";
      %swatch.add(%AOG_CatCheck[%cat]);
      %AOG_nextPos += 18;
      %hr = new GuiSwatchCtrl()
      {
         position = "5" SPC %AOG_nextPos;
         extent = "256 2";
         color = "0 0 0 255";
      };
      %swatch.add(%hr);
      %AOG_nextPos += 5;
      
      %sortStringB = "";      
      for(%j=1;%j<%AOG_ItemCount[%cat];%j++)
      {
         %sortStringB = %sortStringB@%j@"=>"@%AOG_Category[%cat,%j]@",";
      }
      %sortStringB = getSubStr(%sortStringB,0,strLen(%sortStringB)-1);
      %sortStringB = strReplace(sortFields(%sortStringB),",","\t");
      
      for(%j=0;%j<getFieldCount(%sortStringB);%j++)
      {
         %keypair = strReplace(getField(%sortStringB,%j),"=>","\t");
         %checkbox = new GuiCheckboxCtrl()
         {
            position = "5" SPC %AOG_nextPos;
            extent = "256 18";
            text = getField(%keypair,1);
            varName = getSafeVariableName(getField(%keypair,1));
            parent = %AOG_CatCheck[%cat];
         };
         %checkbox.command = "AOG_tickAddon("@%checkbox@");";
         %swatch.add(%checkbox);
         %AOG_CatCheck[%cat].numChildren++;
         if($AddOn__[%checkbox.varName] $= 1)
         {
            %AOG_CatCheck[%cat].numEnabled++;
            %checkbox.setValue(1);
         }
         %AOG_nextPos += 18;
         
         %childID = %AOG_CatCheck[%cat].numChildren-1;
         %AOG_CatCheck[%cat].child[%childID] = %checkbox;
      }
      
      if(%AOG_CatCheck[%cat].numEnabled $= %AOG_CatCheck[%cat].numChildren)
         %AOG_CatCheck[%cat].setValue(1);
   }

   if(RTBMM_GroupManager.getCount() >= 1)
   {
      AOG_Groups.command = "AOG_scrollToGroups("@%AOG_nextPos@");";
   }
   else
   {
      AOG_Groups.command = "MessageBoxYesNo(\"Oops\",\"You haven't created any groups, would you like to make one now?\",\"RTBMM_OpenModManager();RTBMM_ModsView_Init(\\\"groups\\\");\");";
      %swatch.resize(0,0,261,%AOG_nextPos);
      return;
   }

   %sortString = "";   
   for(%i=0;%i<RTBMM_GroupManager.getCount();%i++)
   {
      %sortString = %sortString@RTBMM_GroupManager.getObject(%i)@"=>"@RTBMM_GroupManager.getObject(%i).name@",";
   }
   %sortString = getSubStr(%sortString,0,strLen(%sortString)-1);
   %sortString = strReplace(sortFields(%sortString),",","\t");
   
   for(%i=0;%i<getFieldCount(%sortString);%i++)
   {
      %keypair = strReplace(getField(%sortString,%i),"=>","\t");
      %group = getField(%keypair,0);      

      %bg = new GuiSwatchCtrl()
      {
         position = "5" SPC %AOG_nextPos+2;
         extent = "13 13";
         color = "0 0 0 255";
      };
      %swatch.add(%bg);
      %AOG_GroupCheck[%group] = new GuiCheckboxCtrl()
      {
         profile = GuiCheckBoxBoldProfile;
         position = "5" SPC %AOG_nextPos;
         extent = "256 18";
         text = " Group: "@%group.name;
         group = %group;
      };
      %AOG_GroupCheck[%group].command = "AOG_tickCategory("@%AOG_GroupCheck[%group]@");";
      %swatch.add(%AOG_GroupCheck[%group]);
      %AOG_nextPos += 18;
      %hr = new GuiSwatchCtrl()
      {
         position = "5" SPC %AOG_nextPos;
         extent = "256 2";
         color = "0 0 0 255";
      };
      %swatch.add(%hr);
      %AOG_nextPos += 5;
      
      %sortStringB = "";      
      for(%j=0;%j<%group.items;%j++)
      {
         %sortStringB = %sortStringB@%group.item[%j]@"=>"@%group.item[%j].file_zipname@",";
      }
      %sortStringB = getSubStr(%sortStringB,0,strLen(%sortStringB)-1);
      %sortStringB = strReplace(sortFields(%sortStringB),",","\t");
      
      for(%j=0;%j<getFieldCount(%sortStringB);%j++)
      {
         %keypair = strReplace(getField(%sortStringB,%j),"=>","\t");
         %checkbox = new GuiCheckboxCtrl()
         {
            position = "5" SPC %AOG_nextPos;
            extent = "256 18";
            text = getField(%keypair,1);
            varName = getSafeVariableName(getField(%keypair,0).file_var);
            parent = %AOG_GroupCheck[%group];
         };
         %checkbox.command = "AOG_tickAddon("@%checkbox@");";
         %swatch.add(%checkbox);
         %AOG_GroupCheck[%group].numChildren++;
         if($AddOn__[%checkbox.varName] $= 1)
         {
            %AOG_GroupCheck[%group].numEnabled++;
            %checkbox.setValue(1);
         }
         %AOG_nextPos += 18;
         
         %childID = %AOG_GroupCheck[%group].numChildren-1;
         %AOG_GroupCheck[%group].child[%childID] = %checkbox;
      }
      if(%AOG_GroupCheck[%group].numEnabled $= %AOG_GroupCheck[%group].numChildren)
         %AOG_GroupCheck[%group].setValue(1);
   }
   %swatch.resize(0,0,261,%AOG_nextPos);
}

//- AOG_tickCategory (Handles a category checkbox)
function AOG_tickCategory(%checkbox)
{
   for(%i=0;%i<%checkbox.numChildren;%i++)
   {
      %child = %checkbox.child[%i];
      %child.setValue(%checkbox.getValue());
      
      for(%j=0;%j<AOG_Scroll.getObject(0).getCount();%j++)
      {
         %obj = AOG_Scroll.getObject(0).getObject(%j);
         if(%obj.varName $= %child.varName)
         {
            if(%obj !$= %child)
            {
               %obj.setValue(%child.getValue());
               AOG_tickAddonAct(%obj);
            }
         }
      }
   }
   
   if(%checkbox.getValue() $= 1)
      %checkbox.numEnabled = %checkbox.numChildren;
   else
      %checkbox.numEnabled = 0;
}

//- AOG_selectNone (Deselects all add-ons)
function AOG_selectNone()
{
   for(%i=0;%i<AOG_Scroll.getObject(0).getCount();%i++)
   {
      %obj = AOG_Scroll.getObject(0).getObject(%i);
      if(%obj.getClassName() $= "GuiCheckboxCtrl")
      {
         %obj.setValue(0);
         if(%obj.numChildren >= 1)
            %obj.numEnabled = 0;
      }
   }
}

//- AOG_selectAll (Selects all add-ons)
function AOG_selectAll()
{
   for(%i=0;%i<AOG_Scroll.getObject(0).getCount();%i++)
   {
      %obj = AOG_Scroll.getObject(0).getObject(%i);
      if(%obj.getClassName() $= "GuiCheckboxCtrl")
      {
         %obj.setValue(1);
         if(%obj.numChildren >= 1)
            %obj.numEnabled = %obj.numChildren;
      }
   }
}

//- AOG_scrollToGroups (scrolls to groups)
function AOG_scrollToGroups(%pos)
{
   AOG_Scroll.getObject(0).resize(0,%pos*-1,261,getWord(AOG_Scroll.getObject(0).extent,1));
}

//- AOG_selectDefault (Selects all default add-ons)
function AOG_selectDefault()
{
   for(%i=0;%i<AOG_Scroll.getObject(0).getCount();%i++)
   {
      %obj = AOG_Scroll.getObject(0).getObject(%i);
      if(%obj.varName !$= "")
      {
         $AddOn__[%obj.varName] = 0;
      }
   }
   
   AOG_selectNone();
   exec("base/server/defaultAddonList.cs");
   
   for(%i=0;%i<AOG_Scroll.getObject(0).getCount();%i++)
   {
      %obj = AOG_Scroll.getObject(0).getObject(%i);
      if(%obj.varName !$= "")
      {
         if($AddOn__[%obj.varName])
         {
            %obj.setValue(1);
            AOG_tickAddonAct(%obj);
         }
      }
   }
}

//- AOG_selectMinimal (Selects all the kind-of necessary stuff)
function AOG_selectMinimal()
{
   %minimalList = " Brick_Arch Brick_Large_Cubes Light_Animated Light_Basic Particle_Basic Particle_FX_Cans Particle_Player Print_1x2f_Default Print_2x2f_Default Print_2x2r_Default Print_Letters_Default Sound_Synth4 Sound_Beeps Sound_Phone ";
   
   AOG_selectNone();
   for(%i=0;%i<AOG_Scroll.getObject(0).getCount();%i++)
   {
      %obj = AOG_Scroll.getObject(0).getObject(%i);
      if(%obj.varName !$= "")
      {
         if(strPos(%minimalList," "@%obj.varName@" ") >= 0)
         {
            %obj.setValue(1);
            AOG_tickAddonAct(%obj);
         }
      }
   }
}

//- AOG_tickAddon (Ticks all add-ons with same name)
function AOG_tickAddon(%checkbox)
{
   for(%i=0;%i<AOG_Scroll.getObject(0).getCount();%i++)
   {
      %obj = AOG_Scroll.getObject(0).getObject(%i);
      if(%obj.varName $= %checkbox.varName)
      {
         if(%obj !$= %checkbox)
            %obj.setValue(%checkbox.getValue());
         AOG_tickAddonAct(%obj);
      }
   }
}

//- AOG_tickAddonAct (Checks to see if category should be ticked too)
function AOG_tickAddonAct(%checkbox)
{
   if(%checkbox.getValue() $= 1)
      %checkbox.parent.numEnabled++;
   else
      %checkbox.parent.numEnabled--;
   
   if(%checkbox.parent.numEnabled >= %checkbox.parent.numChildren)
   {
      %checkbox.parent.numEnabled = %checkbox.parent.numChildren; 
      %checkbox.parent.setValue(1);
   }
   else
      %checkbox.parent.setValue(0);
}

//- addonsGui::onSleep (Save all the preferences and changes)
function addonsGui::onSleep()
{
   for(%i=0;%i<AOG_Scroll.getObject(0).getCount();%i++)
   {
      %obj = AOG_Scroll.getObject(0).getObject(%i);
      if(%obj.varName !$= "")
      {
         $AddOn__[%obj.varName] = %obj.getValue();
      }
   }
   export("$AddOn__*","config/server/ADD_ON_LIST.cs");
}