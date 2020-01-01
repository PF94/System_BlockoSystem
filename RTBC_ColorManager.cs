//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 48 $
//#      $Date: 2009-03-14 13:47:40 +0000 (Sat, 14 Mar 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.ephialtes.co.uk/RTBSVN/branches/2030/RTBC_ColorManager.cs $
//#
//#      $Id: RTBC_ColorManager.cs 48 2009-03-14 13:47:40Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Color Manager (RTBCM/CColorManager)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBC_ColorManager = 1;

//*********************************************************
//* Initialisation of required objects
//*********************************************************
if(!isObject(RTB_ColorManager))
	exec("./RTB_ColorManager.gui");

//*********************************************************
//* GUI Modification
//*********************************************************
if(!isObject(btnColorManager))
{
   for(%inc=0;%inc<startMissionGui.getObject(0).getCount();%inc++)
   {
      %ctrl = startMissionGui.getObject(0).getObject(%inc);
      if(%ctrl.text $= "Add-Ons")
      {
         %btn = new GuiBitmapButtonCtrl(btnColorManager)
         {
            profile = BlockButtonProfile;
            horizSizing = "right";
            vertSizing = "top";
            position = vectorAdd(%ctrl.position,"120 0");
            extent = "113 19";
            command = "canvas.pushDialog(RTB_ColorManager);";
            text = "Color Manager";
            bitmap = "base/client/ui/button1";
            mColor = "255 255 255 255";
         };
         startMissionGui.getObject(0).add(%btn);
         break;
      }
   }
}


//*********************************************************
//* Extract Default Colorset
//*********************************************************
if(!isFile("Add-Ons/Colorset_Default/colorSet.txt"))
{
   %foA = new FileObject();
   %foB = new FileObject();
   %foA.openForWrite("Add-Ons/Colorset_Default/colorSet.txt");
   %foB.openForRead($RTB::Path @ "Colorset_Default.txt");
   while(!%foB.isEOF())
   {
      %foA.writeLine(%foB.readLine());
   }
   %foA.close();
   %foA.delete();
   %foB.close();
   %foB.openForWrite("Add-Ons/Colorset_Default/description.txt");
   %foB.writeLine("Title: Blockland Default");
   %foB.writeLine("Author: Eric Hartman");
   %foB.close();
   %foB.delete();
}

if(!isFile("config/server/colorSet.txt")) 
{ 
   %foA = new FileObject(); 
   %foB = new FileObject(); 
   %foA.openForWrite("config/server/colorSet.txt"); 
   %foB.openForRead($RTB::Path @ "Colorset_Default.txt"); 
   while(!%foB.isEOF()) 
   { 
      %foA.writeLine(%foB.readLine()); 
   } 
   %foA.close(); 
   %foA.delete(); 
   %foB.close(); 
   %foB.delete(); 
} 

//*********************************************************
//* The Meat
//*********************************************************
function RTB_ColorManager::onWake(%this)
{
   if(isReadonly("config/server/colorSet.txt"))
   {
      messageBoxOK("Oops!","It appears as though your colorset.txt file is read-only. That means you can't use the color manager feature.\n\nYou can try deleting colorSet.txt in the config/server/ folder to fix this problem.");
      canvas.popDialog(%this);
      return;
   }
   
   RTBCM_loadSets();

   %k = 0;
   RTBCM_Sets.clear();
   
   $RTB::MCCM::Selected = "";
   %currCS = new FileObject();
   %chckCS = new FileObject();
   for(%i=0;%i<$RTB::MCCM::Colorsets;%i++)
   {
      %colorSet = "Add-Ons/"@getField($RTB::MCCM::Colorset[%i],0)@"/colorSet.txt";
      %currCS.openForRead("config/server/colorSet.txt");
      %chckCS.openForRead(%colorSet);
      %match = %colorSet;
      while(!%currCS.isEOF() && !%match !$= "")
      {
         if(%currCS.readLine() !$= %chckCS.readLine())
            %match = "";
         else if(%currCS.isEOF() && !%chckCS.isEOF() || !%currCS.isEOF() && %chckCS.isEOF())
            %match = "";
            
         if(%match $= "")
            break;
      }
      %currCS.close();
      %chckCS.close();
      
      if(%match !$= "")
      {
         $RTB::MCCM::Selected = %match;
         break;
      }
   }
   %currCS.delete();
   %chckCS.delete();
   

   if($RTB::MCCM::Selected $= "")
   {
      %bg = new GuiSwatchCtrl()
      {
         profile = GuiDefaultProfile;
         position = "0" SPC %k*20;
         extent = "200 20";
         color = "200 200 200 255";
      };
      RTBCM_Sets.add(%bg);
      
      %ctrl = new GuiRadioCtrl()
      {
         profile = GuiRadioProfile;
         position = "4" SPC %k*20;
         extent = "200 20";
         text = " Unknown Colorset";
         group = 1;
         command = "RTBCM_previewSet(\"config/server/colorSet.txt\");$RTB::MCCM::Selected = \"config/server/colorSet.txt\";";
      };
      RTBCM_Sets.add(%ctrl);
      
      %ctrl.setValue(1);
      RTBCM_previewSet("config/server/colorSet.txt");
      
      %k++;
   }
   
   for(%i=$RTB::MCCM::Colorsets-1;%i>=0;%i--)
   {
      %data = $RTB::MCCM::Colorset[%i];
      %file = getField(%data,0);
      %name = getField(%data,1);
      
      if(%k%2 $= 0)
      {
         %bg = new GuiSwatchCtrl()
         {
            profile = GuiDefaultProfile;
            position = "0" SPC %k*20;
            extent = "200 20";
            color = "200 200 200 255";
         };
         RTBCM_Sets.add(%bg);
      }
      
      %ctrl = new GuiRadioCtrl()
      {
         profile = GuiRadioProfile;
         position = "4" SPC %k*20;
         extent = "200 20";
         text = " "@%name;
         group = 1;
         command = "RTBCM_previewSet(\"Add-Ons/"@%file@"/colorSet.txt\");$RTB::MCCM::Selected = \"Add-Ons/"@%file@"/colorSet.txt\";";
      };
      RTBCM_Sets.add(%ctrl);
      
      if("Add-Ons/"@%file@"/colorSet.txt" $= $RTB::MCCM::Selected)
      {
         %ctrl.setValue(1);
         RTBCM_previewSet($RTB::MCCM::Selected);
      }
      
      %k++;
      %lowestPoint = RTBCM_Sets.getLowestPoint();
      if(%lowestPoint <= (getWord(RTBCM_Sets.getGroup().extent,1)-2))
         RTBCM_Sets.resize(1,1,getWord(RTBCM_Sets.extent,0),(getWord(RTBCM_Sets.getGroup().extent,1)-2));
      else
         RTBCM_Sets.resize(1,1,getWord(RTBCM_Sets.extent,0),%lowestPoint);
   }
}

//*********************************************************
//* Usage Functions
//*********************************************************
//- RTB_ColorManager::saveSet (saves the set for usage in the server)
function RTB_ColorManager::saveSet(%this)
{
   %sel = $RTB::MCCM::Selected;
   if(%sel $= "" || !isFile(%sel))
   {
      MessageBoxOK("Whoops","Please make a valid selection.");
      return;
   }
   
   if(%sel $= "config/server/colorSet.txt")
   {
      MessageBoxOK("Ooops","This color set is already loaded.");
      return;
   }
   canvas.popDialog(%this);

   %input = new FileObject();
   %output = new FileObject();
   if(%input.openForRead(%sel))
   {
      if(%output.openForWrite("config/server/colorSet.txt"))
      {
         while(!%input.isEOF())
         {
            %output.writeLine(%input.readLine());
         }
         %output.close();
         
         MessageBoxOK("Hooray","The selected color set was loaded successfully.");
      }
      else
         MessageBoxOK("ERROR","Color set could not be saved because your colorSet.txt was invalid.");

      %input.close();
   }
   else
      MessageBoxOK("ERROR","Color set could not be saved because the selection was invalid.");
      
   %input.delete();
   %output.delete();
}

//*********************************************************
//* Support Functions
//*********************************************************
//- RTBCM_loadSets (loads and caches all available colorsets)
function RTBCM_loadSets()
{
   $RTB::MCCM::Colorsets = 0;
	%colorset = FindFirstFile("Add-Ons/Colorset_*/colorSet.txt");
	while(strLen(%colorset) > 0)
	{
      %path = getSubStr(%colorset,0,strLen(%colorset)-12);
      %file = getSubStr(%path,8,strLen(%path)-9);
      
      if(isFile(%path@"description.txt"))
      {  
         %title = "Unnamed Colorset";
         
         %fo = new FileObject();
         if(%fo.openForRead(%path@"description.txt"))
         {
            while(!%fo.isEOF())
            {
               %line = %fo.readLine();
               if(strPos(%line,"Title:") $= 0)
               {
                  %title = getWords(%line,1,getWordCount(%line)-1);
                  break;
               }
            }
            %fo.close();
         }
         else
            echo("\c2ERROR: Unable to open description.txt for reading in "@%file);
            
         $RTB::MCCM::Colorset[$RTB::MCCM::Colorsets] = %file TAB %title;
         $RTB::MCCM::Colorsets++;
      }
      else
         echo("\c2ERROR: Skipped "@%file@" due to missing description.txt");
      
		%colorset = FindNextFile("Add-Ons/Colorset_*/colorSet.txt");
	}
}

//- RTBCM_previewSet (generates a preview of a colorset)
function RTBCM_previewSet(%filepath)
{
   RTBCM_ColorsetPreview.clear();
   
   %file = new FileObject();
   if(%file.openForRead(%filepath))
   {
      while(!%file.isEOF())
      {
         %line = %file.readLine();
         if(%line !$= "" && strPos(%line,"DIV:") !$= 0)
         {
            %r = getWord(%line,0);
            %g = getWord(%line,1);
            %b = getWord(%line,2);
            %a = getWord(%line,3);

            if(!isInt(getWord(%line,0)))
            {
               %r = mFloor(%r*255);
               %g = mFloor(%g*255);
               %b = mFloor(%b*255);
               %a = mFloor(%a*255);
            }
            %color = %r SPC %g SPC %b SPC %a;

            %xPos = (%currCol*20);
            %yPos = (%currRow*20);
            
            if(%a < 255)
            {
               %b = new GuiBitmapCtrl()
               {
                  profile = "GuiDefaultProfile";
                  position = %xPos SPC %yPos;
                  extent = "20 20";
                  bitmap = $RTB::Path @ "images/image_checker16";
               };
               RTBCM_ColorsetPreview.add(%b);
            }
            
            %c = new GuiSwatchCtrl()
            {
               profile = "GuiDefaultProfile";
               position = %xPos SPC %yPos;
               extent = "20 20";
               color = %color;
            };
            RTBCM_ColorsetPreview.add(%c);
            
            %d = new GuiBitmapCtrl()
            {
               profile = "GuiDefaultProfile";
               position = %xPos SPC %yPos;
               extent = "20 20";
               bitmap = "base/client/ui/btnColor_n";
            };
            RTBCM_ColorsetPreview.add(%d);
            %currRow++;
            
            if(%currRow > %maxRow)
               %maxRow = %currRow;
         }
         else if(strPos(%line,"DIV:") $= 0)
         {
            %currRow = 0;
            %currCol++;
         }
      }
   }
   RTBCM_ColorsetPreview.extent = (%currcol*20) SPC (%maxRow*20);
   
   %file.close();
   %file.delete();
   
   %xpos = (mFloor(getWord(RTBCM_ColorSetPreview.getGroup().extent,0)/2))-(mFloor(getWord(RTBCM_ColorSetPreview.extent,0)/2));
   %ypos = (mFloor(getWord(RTBCM_ColorSetPreview.getGroup().extent,1)/2))-(mFloor(getWord(RTBCM_ColorSetPreview.extent,1)/2));
   RTBCM_ColorSetPreview.position = %xpos SPC %ypos;
}