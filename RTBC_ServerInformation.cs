//#############################################################################
//#
//#   Return to Blockland - Version 3.5
//#
//#   -------------------------------------------------------------------------
//#
//#      $Rev: 39 $
//#      $Date: 2009-02-23 10:45:55 +0000 (Mon, 23 Feb 2009) $
//#      $Author: Ephialtes $
//#      $URL: http://svn.returntoblockland.com/trunk/old/RTBC_ServerInformation.cs $
//#
//#      $Id: RTBC_ServerInformation.cs 39 2009-02-23 10:45:55Z Ephialtes $
//#
//#   -------------------------------------------------------------------------
//#
//#   Server Information (RTBSI/CServerInformation)
//#
//#############################################################################
//Register that this module has been loaded
$RTB::RTBC_ServerInformation = 1;

//*********************************************************
//* Initialisation of Required Elements
//*********************************************************
if(!isObject(RTB_ServerInformation))
	exec("./RTB_ServerInformation.gui");

//*********************************************************
//* Load Switchboard
//*********************************************************
%RTBSI_SB = RTB_createSwitchboard("SI","APISB");
%RTBSI_SB.registerLine(1,1);
%RTBSI_SB.registerLine(2,1);
$RTB::CServerInformation::Switchboard = %RTBSI_SB;

//*********************************************************
//* Request Gateway
//*********************************************************
//- RTBSI_SendRequest (compiles arguments into POST string for transfer)
function RTBSI_SendRequest(%cmd,%line,%arg1,%arg2,%arg3,%arg4,%arg5,%arg6,%arg7,%arg8,%arg9,%arg10)
{
   for(%i=1;%i<11;%i++)
   {
      %string = %string@strReplace(urlEnc(%arg[%i]),"\t","")@"\t";
   }
   RTB_SB_SI.placeCall(%line,%cmd,%string);
}

//*********************************************************
//* Meat
//*********************************************************
//- RTBSI_getRTBServers (retrieves a list of rtb servers from the rtb api)
function RTBSI_getRTBServers()
{
   deleteVariables("$RTB::CServerInformation::Cache*");
   RTBSI_SendRequest("MASTERRTB",1);
}

//- RTBSI_onMasterReply (reply from rtb api)
%RTBSI_SB.registerResponseHandler("MASTERRTB","RTBSI_onMasterReply");
function RTBSI_onMasterReply(%tcp,%line,%ip,%port)
{
   %search = strReplace(%ip,".","_")@"X"@%port;
   %soIndex = $ServerSOFromIP[%search];
   %so = $ServerSO[%soIndex];

   if(isObject(%so))
   {
      %so.hasRTB = 1;
      %so.display();
   }
   $RTB::CServerInformation::Cache::RTBIP[%search] = 1;
}

//- joinServerGui::info (retrieves specific info on the selected server)
function joinServerGui::info(%this)
{
   %address = getField(JS_serverList.getValue(),9);
   %ip = getSubStr(%address,0,strPos(%address,":"));
   %port = getSubStr(%address,strPos(%address,":")+1,strLen(%address));
   
   Canvas.pushDialog(RTB_ServerInformation);
   RTBSI_LoadSwatch.setVisible(true);
   RTBSI_FailSwatch.setVisible(false);
   RTBSI_PlayerList.clear();
   
   RTBSI_ModScroll.clear();
   %modSwatch = new GuiSwatchCtrl(RTBSI_ModWindow)
   {
      position = "0 0";
      extent = "383 292";
      color = "0 0 0 0";
   };
   RTBSI_ModScroll.add(%modSwatch);
   
   RTBSI_SendRequest("GETSERVERINFO",1,%ip,%port);
}

//- RTBSI_onServerInfoStart (callback of the start of transmission)
function RTBSI_onServerInfoStart()
{
   RTBSI_LoadSwatch.setVisible(false);
}

//- RTBSI_onServerInfo (callback for server info request)
%RTBSI_SB.registerResponseHandler("GETSERVERINFO","RTBSI_onServerInfo");
function RTBSI_onServerInfo(%tcp,%line,%type,%data)
{
   if(%type $= "PLIST")
   {
      %players = strReplace(%data,"~","\t");
      for(%i=0;%i<getFieldCount(%players);%i++)
      {
         %player = strReplace(getField(%players,%i),";","\t");

         %status = getField(%player,2);
         if(%status $= 3)
            %status = "H";
         else if(%status $= 2)
            %status = "S";
         else if(%status $= 1)
            %status = "A";
         else
            %status = "-";
         
         RTBSI_PlayerList.addRow(getField(%player,2),getField(%player,2) TAB %status TAB getField(%player,0) TAB getField(%player,3) TAB getField(%player,1),0);
      }
   }
   else if(%type $= "MLIST")
   {
      %mods = strReplace(%data,";","\t");
      for(%i=0;%i<getFieldCount(%mods);%i++)
      {
         %mod = strReplace(getField(%mods,%i),"~","\t");

         if(getField(%mod,0) $= "rtb")
         {
            %mod_id = getField(%mod,1);
            %mod_title = getField(%mod,2);
            %mod_author = "by "@getField(%mod,3);
            %mod_zip = getField(%mod,4);
            %mod_content = getField(%mod,5);
         }
         else
         {
            %mod_id = "";
            %mod_title = getField(%mod,1);
            %mod_author = getField(%mod,2);
            %mod_zip = getField(%mod,3);
            %mod_content = 0;
         }
         
         %yPosition = ((RTBSI_ModWindow.getCount()/3)*44)+(RTBSI_ModWindow.getCount()/3);
         if(%mod_zip $= "")
            continue;
            
         %mod_icon = (getField(%mod,0) $= "rtb" || getField(%mod,0) $= "rtb2") ? "icon_rtbLogo" : "icon_blLogo";
            
         if(isFile("Add-Ons/"@%mod_zip) || isFile("Add-Ons/"@getSubStr(%mod_zip,0,strLen(%mod_zip)-4)@"/description.txt"))
         {
            %mod_gotIcon = "icon_tick";
            %mod_got = 1;
         }
         else
         {
            %mod_gotIcon = "icon_cross";
            %mod_got = 0;
         }
         
         %iconSw = new GuiSwatchCtrl()
         {
            position = "0" SPC %yPosition;
            extent = "42 44";
            color = "210 210 210 255";
            
            new GuiBitmapCtrl()
            {
               position = "13 14";
               extent = "16 16";
               bitmap = "./images/"@%mod_icon;
            };
         };
         RTBSI_ModWindow.add(%iconSw);
         
         %infoSw = new GuiSwatchCtrl()
         {
            position = "43" SPC %yPosition;
            extent = "297 44";
            color = "220 220 220 255";
            
            new GuiMLTextCtrl()
            {
               position = "4 3";
               extent = "290 19";
               text = "<font:Arial Bold:14><color:444444>"@%mod_title@"<br><color:666666><font:Verdana:12>"@%mod_author@"<br><color:888888>"@%mod_zip;
            };
         };
         RTBSI_ModWindow.add(%infoSw);
         
         if(getField(%mod,0) $= "rtb" && %mod_content)
         {
            %plug = new GuiBitmapCtrl()
            {
               position = "-2 -1";
               extent = "16 16";
               bitmap = "./images/bullet_plugin";
            };
            %iconSw.add(%plug);
            
            %infoSw.getObject(0).setText(%infoSw.getObject(0).getText()@"<just:right><font:Verdana:12><color:999999>(contains content)");
            
            if(!%mod_got)
            {
               %dl = new GuiBitmapButtonCtrl()
               {
                  position = "274 8";
                  extent = "16 16";
                  text = " ";
                  bitmap = "./images/buttons/small/btnDisk";
               };
               %infoSw.add(%dl);
               %infoSw.sg_dlBtn = %dl;
            }
            
            %statusSw = new GuiSwatchCtrl()
            {
               position = "1 30";
               extent = "200 14";
               color = "220 220 220 255";
               visible = 0;
               
               new GuiMLTextCtrl()
               {
                  position = "3 0";
                  extent = "200 12";
                  text = "<font:Verdana Bold:12><color:666666>Hi";
               };
            };
            %infoSw.add(%statusSw);
            %infoSw.sg_statusSW = %statusSw;
            
            %progStd = new GuiSwatchCtrl()
            {
               position = "195 26";
               extent = "100 16";
               color = "220 220 220 255";
               visible = 0;
            };
            %infoSw.add(%progStd);
            %infoSw.sg_progStd = %progStd;
            
            for(%k=0;%k<8;%k++)
            {
               %dot = new GuiBitmapCtrl()
               {
                  position = %progStd.getCount()*12 SPC 0;
                  extent = "16 16";
                  bitmap = "./images/bullet_gray";
               };
               %progStd.add(%dot);
            }
            
            %progRed = new GuiSwatchCtrl()
            {
               position = "195 26";
               extent = "100 16";
               color = "220 220 220 255";
               visible = 0;
            };
            %infoSw.add(%progRed);
            %infoSw.sg_progRed = %progRed;
            
            for(%k=0;%k<8;%k++)
            {
               %dot = new GuiBitmapCtrl()
               {
                  position = %progRed.getCount()*12 SPC 0;
                  extent = "16 16";
                  bitmap = "./images/bullet_red";
               };
               %progRed.add(%dot);
            }
         }
         
         %optSw = new GuiSwatchCtrl()
         {
            position = "341" SPC %yPosition;
            extent = "42 44";
            color = "220 220 220 255";
            
            new GuiBitmapCtrl()
            {
               position = "13 14";
               extent = "16 16";
               bitmap = "./images/"@%mod_gotIcon;
            };
         };
         RTBSI_ModWindow.add(%optSw);
         
         %infoSw.sg_dlBtn.command = "RTBSI_downloadContent("@%mod_id@","@%infoSw@","@%optSw.getObject(0)@");";
         
         if(RTBMM_TransferQueue.hasItem(%mod_id))
         {
            %queue = RTBMM_TransferQueue.getItem(%mod_id);
            
            %queue.sg_progStd = %infoSw.sg_progStd;
            %queue.sg_progRed = %infoSw.sg_progRed;
            %queue.sg_statusSW = %infoSw.sg_statusSW;
            %queue.sg_indicator = %optSw.getObject(0);
            %queue.sg_dlBtn = %infoSw.sg_dlBtn;
            
            %queue.update();
         }
      }
      %yPosition += 44;
      if(%yPosition < 292)
         %yPosition = 292;
      RTBSI_ModWindow.resize(0,0,383,%yPosition);
   }
   else if(%type $= "0")
   {
      RTBSI_FailSwatch.setVisible(true);
   }
}

//- RTBSI_onServerInfoStop (end of data)
function RTBSI_onServerInfoStop()
{
   RTBSI_PlayerList.sort(0,0);
}

//- RTB_ServerInformation::onSleep (callback for closing of gui)
function RTB_ServerInformation::onSleep()
{
   $RTB::CServerInformation::Switchboard.killLine(1);
}

//- RTBSI_downloadContent (adds this file to the download queue for the mod manager)
function RTBSI_downloadContent(%id,%info,%icon)
{
   %queue = RTBMM_TransferQueue.addItem(%id,1);
   if(!isObject(%queue))
   {
      MessageBoxOK("Ooops","You are already downloading this add-on.");
      return;
   }
   
   %queue.sg_progStd = %info.sg_progStd;
   %queue.sg_progRed = %info.sg_progRed;
   %queue.sg_statusSW = %info.sg_statusSW;
   %queue.sg_indicator = %icon;
   %queue.sg_dlBtn = %info.sg_dlBtn;
   
   %queue.update();
}

//*********************************************************
//* Packaged Functions
//*********************************************************
package RTBC_ServerInformation
{
   function queryMasterTCPObj::connect(%this,%host)
   {
      Parent::connect(%this,%host);
      
      RTBSI_getRTBServers();
   }
   
   function ServerInfoSO_Add(%a,%b,%c,%d,%e,%f,%g,%h,%i,%j)
   {
      Parent::ServerInfoSO_Add(%a,%b,%c,%d,%e,%f,%g,%h,%i,%j);
      
      %search = strReplace(strReplace(%a,":","X"),".","_");
      if($RTB::CServerInformation::Cache::RTBIP[%search])
      {
         %soIndex = $ServerSOFromIP[%search];
         %so = $ServerSO[%soIndex];
         if(isObject(%so))
         {
            %so.hasRTB = 1;
            %so.display();
         }
      }
   }
   
   function ServerSO::serialize(%this)
   {
      %serialized = Parent::serialize(%this);
      
      if(%this.hasRTB)
         %hasRTB = "Yes";
      else
         %hasRTB = "No";
      
      return %serialized@"\t"@%hasRTB;
   }
};
activatePackage(RTBC_ServerInformation);