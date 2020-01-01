//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 112 $
//#      $Date: 2009-09-05 18:17:49 +0100 (Sat, 05 Sep 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/RTBR_GUITransfer_Hook.cs $
//#
//#      $Id: RTBR_GUITransfer_Hook.cs 112 2009-09-05 17:17:49Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   GUI Transfer Hook (RTBRT/RGUITransferHook)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBR_GUITransfer_Hook = 1;

//*********************************************************
//* Variable Declarations
//*********************************************************
$RTB::RGUITransfer::Controls = 0;

//*********************************************************
//* Registering Controls
//*********************************************************
//- RTBRT_registerControl (Registers a control)
function RTBRT_registerControl(%control,%props)
{
   if(RTBRT_controlRegistered(%control))
      return;
      
   $RTB::RGUITransfer::Control[$RTB::RGUITransfer::Controls] = %control;
   $RTB::RGUITransfer::ControlProps[$RTB::RGUITransfer::Controls] = %props;
   $RTB::RGUITransfer::Controls++;
}

//- RTBRT_controlRegistered (Checks to see if a control has already been registered)
function RTBRT_controlRegistered(%control)
{
   for(%i=0;%i<$RTB::RGUITransfer::Controls;%i++)
   {
      if($RTB::RGUITransfer::Control[%i] $= %control)
         return 1;
   }
   return 0;
}

//- RTBRT_getControlProps (Gets the properties of a control)
function RTBRT_getControlProps(%control)
{
   for(%i=0;%i<$RTB::RGUITransfer::Controls;%i++)
   {
      if($RTB::RGUITransfer::Control[%i] $= %control)
         return $RTB::RGUITransfer::ControlProps[%i];
   }
   return 0;
}

//- RTBRT_getControlCRC (Calculates a CRC for comparison between server & client)
function RTBRT_getControlCRC()
{
   for(%i=0;%i<$RTB::RGUITransfer::Controls;%i++)
   {
      %string = %string@$RTB::RGUITransfer::Control[%i]@$RTB::RGUITransfer::ControlProps[%i];
   }
   return getStringCRC(%string);
}

//*********************************************************
//* GUI Data Handling
//*********************************************************
if(!isObject(RTBRT_GUIManifest))
{
   new ScriptGroup(RTBRT_GUIManifest)
   {
   };
}

//*********************************************************
//* GUI Registration Hook
//*********************************************************
//- RTB_registerGUI (Registers a gui for transfer to the client)
function RTB_registerGUI(%path)
{
   if(!isFile(%path))
      return;
      
   %file = new FileObject();
   %file.openForRead(%path);
   %line = %file.readLine();
   if(strPos(%line,"//") >= 0)
      %line = %file.readLine();
   %file.delete();
   
   if(strPos(%line,"new GuiControl") $= 0)
   {
      %controlName = getSubStr(%line,strPos(%line,"(")+1,strLen(%line));
      %controlName = getSubStr(%controlName,0,strPos(%controlName,")"));
   }
   else
   {
      echo("\c2ERROR: Downloadable GUI must start with a GuiControl Object in RTB_registerGUI ("@%path@")");
      return;
   }
   
   for(%i=0;%i<RTBRT_GUIManifest.getCount();%i++)
   {
      %obj = RTBRT_GUIManifest.getObject(%i);
      if(%obj.name $= %controlName)
      {
         %obj.delete();
         break;
      }
   }
   
   if(!isObject(%controlName))
   {
      if($Server::Dedicated)
         RTBRT_parseGUI(%path);
      else
         exec(%path);
         
      if(!isObject(%controlName))
      {
         echo("\c2ERROR: Execution failed for gui in RTB_registerGUI ("@%path@")");
         return;
      }
   }
   
   %sg = new ScriptGroup()
   {
      name = %controlName;
      elements = 0;
   };
   RTBRT_GUIManifest.add(%sg);
   RTBRT_GUIManifest.items++;
   %controlName.script = %sg;
   
   RTBRT_traverseGui(%controlName,%controlName,0);
}

//*********************************************************
//* Script Parsing (Fucking dedicated servers...)
//*********************************************************
function RTBRT_parseGUI(%gui)
{
   %fileObject = new FileObject();
   if(!%fileObject.openForRead(%gui))
   {
      echo("\c2ERROR: Unable to open \""@%gui@"\" for parsing in RTBRT_parseGui");
      %fileObject.delete();
      return 0;
   }
   
   %depth = 0;
   while(!%fileObject.isEOF())
   {
      %line = trim(%fileObject.readLine());
      
      if(getSubStr(%line,0,2) $= "//" || %line $= "")
         continue;
         
      if(getSubStr(%line,0,3) $= "new" && strPos(%line,"{") > -1)
      {
         %type = getWord(%line,1);
         %type = getSubStr(%type,0,strPos(%type,"("));
      
         %ctrlName = getSubStr(%line,strPos(%line,"(")+1,strLen(%line));
         %ctrlName = getSubStr(%ctrlName,0,strPos(%ctrlName,")"));      
         
         %so = new ScriptGroup(%ctrlName)
         {
            class = "Script_GUI"; //Awesome hack
         };
         %so.className = %type;
         
         if(isObject(%parent[%depth]))
         {
            if(%depth $= 0)
            {
               echo("\c2ERROR: You can only have one gui control at a depth of 0 in a gui file ("@%gui@")");
               %parent[0].delete();
               %fileObject.delete();
               return;
            }
            else
               %parent[%depth].add(%so);
         }
         %depth++;
         %parent[%depth] = %so;
         
         %inObject = 1;
         continue;
      }
      
      if(%inObject)
      {
         if(%line $= "};")
         {
            %depth--;
            %inObject = 0;
            continue;
         }

         if(strPos(%line," = \"") > 0 && strPos(%line,"\";") > 1)
         {
            %attrib = getSubStr(%line,0,strPos(%line," = \""));
            %value = getSubStr(%line,strPos(%line," = \"")+4,strLen(%line));
            %value = getSubStr(%value,0,strLen(%value)-2);
            eval(%so@"."@%attrib@" = \""@%value@"\";");
         }
      }
   }
   %fileObject.delete();
}

//*********************************************************
//* Recursive GUI Analysis
//*********************************************************
//- RTBRT_traverseGui (Creates a manifest of details about the gui)
function RTBRT_traverseGui(%ctrl,%top,%depth)
{
   if(!isObject(%ctrl))
      return;
      
   %script = %top.script;
   if(!isObject(%script))
      return;
   
   %depth++;
   for(%i=0;%i<%ctrl.getCount();%i++)
   {
      %cCtrl = %ctrl.getObject(%i);

      %classname = %cCtrl.getClassName();      
      if(RTBRT_controlRegistered(%classname))
      {
         %script.elementClass[%script.elements] = %classname;
         %script.elementName[%script.elements] = %cCtrl.getName();         
         
         %elementProps = "";
         %propList = RTBRT_getControlProps(%classname);
         for(%j=0;%j<getWordCount(%propList);%j++)
         {
            %prop = getWord(%propList,%j);
            if(strPos(%prop,"=>") >= 1)
            {
               %prop = getSubStr(%prop,0,strPos(%prop,"=>"));
            }
            if(strPos(%prop,"f{") $= 0)
            {
               %prop = getSubStr(%prop,2,strLen(%prop)-3);
               eval("%propValue = "@%cCtrl@"."@%prop@"();");
            }
            else
            {
               eval("%propValue = "@%cCtrl@"."@%prop@";");
            }
            %elementProps = %elementProps@%propValue;
            if(%j < getWordCount(%propList)-1)
               %elementProps = %elementProps@"\t";
         }
         %script.elementProps[%script.elements] = %elementProps;
         %script.elementDepth[%script.elements] = %depth;
         %script.elements++;
         RTBRT_GUIManifest.elements++;
         
         RTBRT_traverseGui(%cCtrl,%top,%depth);
      }
   }
}

//*********************************************************
//* Transfer Definitions
//*
//* - Must match on client or it will halt + disable
//*   transfers, so no changing these!
//*********************************************************
RTBRT_registerControl("GuiBitmapButtonCtrl","profile horizSizing vertSizing position extent text bitmap mColor command closeOnSubmit");
RTBRT_registerControl("GuiButtonCtrl","profile horizSizing vertSizing position extent text=>f{setText} command closeOnSubmit");
RTBRT_registerControl("GuiBitmapCtrl","profile horizSizing vertSizing position extent bitmap");
RTBRT_registerControl("GuiTextEditCtrl","profile horizSizing vertSizing position extent");
RTBRT_registerControl("GuiTextCtrl","profile horizSizing vertSizing position extent f{getValue}=>f{setText}");
RTBRT_registerControl("GuiMLTextCtrl","profile horizSizing vertSizing position extent f{getValue}=>f{setText}");
RTBRT_registerControl("GuiSwatchCtrl","profile horizSizing vertSizing position extent color");
RTBRT_registerControl("GuiWindowCtrl","profile horizSizing vertSizing position extent text=>f{setText} resizeWidth resizeHeight canMove canClose canMinimize canMaximize minSize");
RTBRT_registerControl("GuiScrollCtrl","profile horizSizing vertSizing position extent hScrollBar vScrollBar childMargin rowHeight columnWidth");