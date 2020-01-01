//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 39 $
//#      $Date: 2009-02-23 10:45:55 +0000 (Mon, 23 Feb 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/old/dedicated.cs $
//#
//#      $Id: dedicated.cs 39 2009-02-23 10:45:55Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Dedicated Handling
//#
//#############################################################################

//*********************************************************
//* Variables
//*********************************************************
$RTB::Options::AU::Enable = 1;
$RTB::Options::CA::AuthWithRTB = 1;
$RTB::Options::CA::Privacy::ShowOnline = 1;
$RTB::Options::CA::Privacy::ShowServer = 1;
$RTB::Options::GT::Enable = 1;
$RTB::Options::IC::AllowPM = 1;
$RTB::Options::IC::AutoConnect = 1;
$RTB::Options::IC::PMAudioNotify = 1;
$RTB::Options::IC::PMVisualNotify = 1;
$RTB::Options::IC::Filter::ShowActions = 1;
$RTB::Options::IC::Filter::ShowConnects = 1;
$RTB::Options::IC::Filter::ShowDisconnects = 0;
$RTB::Options::IT::Enable = 1;
$RTB::Options::IT::ShowAddonTips = 1;
$RTB::Options::MM::AnimateGUI = 1;
$RTB::Options::MM::WarnFailed = 1;
$RTB::Options::MM::DisableFailed = 1;
$RTB::Options::MM::CheckForUpdates = 1;
$RTB::Options::MM::DownloadScreenshots = 1;
$RTB::Options::SA::PostServer = 1;
$RTB::Options::SA::Privacy::ShowPlayers = 1;

if(isFile("config/client/rtb/prefs.cs"))
	exec("config/client/rtb/prefs.cs");
else
{
   echo("Exporting rtb prefs");
   export("$RTB::Options*","config/client/rtb/prefs.cs");
}

//*********************************************************
//* Load Prerequisites
//*********************************************************
exec("./RTBH_Support.cs");

//*********************************************************
//* Load Modules
//*********************************************************
exec("./RTBD_Updater.cs");

//*********************************************************
//* Dedicated Help
//*********************************************************
//- rtbHelp (displays a help menu)
function rtbHelp()
{
   RTBDU_drawSpacer();
   RTBDU_drawDOSRow("");
   RTBDU_drawDOSRow("Saving bricks on the server:");
   RTBDU_drawDOSRow("Enter saveBricks(); for more help.");
   RTBDU_drawDOSRow("");
   RTBDU_drawDOSRow("");
   RTBDU_drawDOSRow("Updating RTB:");
   RTBDU_drawDOSRow("Check for updates by typing checkRTBUpdates();");
   RTBDU_drawDOSRow("");
   RTBDU_drawSpacer();
}

//*********************************************************
//* Saving Bricks
//*********************************************************
//- saveBricks (function by Randy for saving bricks on a dedicated)
function saveBricks(%name,%events,%ownership)
{
   if(%name $= "" || %events $= "" || %ownership $= "" || (%events !$= 1 && %events !$= 0) || (%ownership !$= 1 && %ownership !$= 0))
   {
      echo("Usage: saveBricks(\"savename\",save events,save ownership);");
      return;
   }
   
	%path = "saves/" @ MissionInfo.saveName @ "/" @ %name @ ".bls";
	if(!isWriteableFileName(%path))
	{
		echo("Error: Cannot save to file: "@%path);
		return;
	}
	
	%bricks = 0;
	for(%i=0;%i<mainBrickGroup.getCount();%i++)
		%bricks += mainBrickGroup.getObject(%i).getCount();
		
   if(%bricks <= 0)
   {
      echo("Error: There are no bricks to save.");
      return;
   }
	echo("Saving to "@%path@" ...");
	
	%file = new FileObject();
	%file.openForWrite(%path);
	%file.writeLine("This is a Blockland save file.  You probably shouldn't modify it cause you'll screw it up.");
	%file.writeLine("1");
	%file.writeLine(%desc);
	for(%i=0;%i<64;%i++)
		%file.writeLine(getColorIDTable(%i));
	%file.writeLine("Linecount " @ %bricks);
	for(%d=0;%d<2;%d++)
	{
		for(%i=0;%i<mainBrickGroup.getCount();%i++)
		{
			%group = mainBrickGroup.getObject(%i);
			for(%a=0;%a<%group.getCount();%a++)
			{
				%brick = %group.getObject(%a);
				if(!(%d ^ %brick.isBasePlate()))
					continue;
				%print = (%brick.getDataBlock().subCategory $= "Prints") ? getPrintTexture(%brick.getPrintID()) : "";
				%file.writeLine(%brick.getDataBlock().uiName @ "\" " @ %brick.getPosition() SPC %brick.getAngleID() SPC %brick.isBasePlate() SPC %brick.getColorID() SPC %print SPC %brick.getColorFXID() SPC %brick.getShapeFXID() SPC %brick.isRayCasting() SPC %brick.isColliding() SPC %brick.isRendering());
				if(%ownership && %brick.isBasePlate() && !$Server::LAN)
					%file.writeLine("+-OWNER " @ getBrickGroupFromObject(%brick).bl_id);
				if(%events)
				{
					if(%brick.getName() !$= "")
						%file.writeLine("+-NTOBJECTNAME " @ %brick.getName());
					if(%brick.numEvents > 0)
					{
						for(%b=0;%b<%brick.numEvents;%b++)
						{
							%targetClass = %brick.eventTargetIdx[%b] >= 0 ? getWord(getField($InputEvent_TargetListfxDTSBrick_[%brick.eventInputIdx[%b]], %brick.eventTargetIdx[%b]), 1) : "fxDtsBrick";
							%paramList = $OutputEvent_parameterList[%targetClass, %brick.eventOutputIdx[%b]];
							%params = "";
							for(%c=0;%c<4;%c++)
							{
								if(firstWord(getField(%paramList, %c)) $= "dataBlock" && %brick.eventOutputParameter[%b, %c + 1] >= 0)
									%params = %params TAB %brick.eventOutputParameter[%b, %c + 1].uiName;
								else
									%params = %params TAB %brick.eventOutputParameter[%b, %c + 1];
							}
							%file.writeLine("+-EVENT" TAB %b TAB %brick.eventEnabled[%b] TAB %brick.eventInput[%b] TAB %brick.eventDelay[%b] TAB %brick.eventTarget[%b] TAB %brick.eventNT[%b] TAB %brick.eventOutput[%b] @ %params);
						}
					}
				}
				if(isObject(%brick.emitter))
					%file.writeLine("+-EMITTER " @ %brick.emitter.emitter.uiName @ "\" " @ %brick.emitterDirection);
				if(%brick.getLightID() >= 0)
					%file.writeLine("+-LIGHT " @ %brick.getLightID().getDataBlock().uiName @ "\" ");
				if(isObject(%brick.item))
					%file.writeLine("+-ITEM " @ %brick.item.getDataBlock().uiName @ "\" " @ %brick.itemPosition SPC %brick.itemDirection SPC %brick.itemRespawnTime);
				if(isObject(%brick.audioEmitter))
					%file.writeLine("+-AUDIOEMITTER " @ %brick.audioEmitter.getProfileID().uiName @ "\" ");
				if(isObject(%brick.vehicleSpawnMarker))
					%file.writeLine("+-VEHICLE " @ %brick.vehicleSpawnMarker.uiName @ "\" " @ %brick.reColorVehicle);
			}
		}
	}
	%file.close();
	%file.delete();
	
	echo(%bricks@" bricks saved to "@%path@" successfully.");
}

//*********************************************************
//* Support Functions
//*********************************************************
//- RTBDU_drawDOSRow (draws a centered string of text in a box)
function RTBDU_drawDOSRow(%string)
{
   %boxStart = ((80-70)-2)/2;
   %white = RTBDU_getWhitespace(%boxStart);
   
   %edgeSpace = (68-strLen(%string))/2;
   if(strPos(%edgeSpace,".5") >= 0)
      %minus = 1;
   %space = RTBDU_getWhitespace(%edgeSpace);
   %space2 = RTBDU_getWhitespace(%edgeSpace-%minus);
   
   echo(%white@"*"@%space@%string@%space2@"*");
}

//- RTBDU_drawSpacer (draws a spacer)
function RTBDU_drawSpacer()
{
   echo("    **********************************************************************");
}

//- RTBDU_getWhitespace (generates a string of whitespace for padding)
function RTBDU_getWhitespace(%length)
{
   for(%i=0;%i<%length;%i++)
   {
      %white = %white@" ";
   }
   return %white;
}