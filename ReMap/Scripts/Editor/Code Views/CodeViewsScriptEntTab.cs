
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using Build;
using static Build.Build;
using WindowUtility;

namespace CodeViews
{
    public class ScriptEntTab
    {
        static FunctionRef[] FullFileMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.FullFileEntSubMenu, EntMenu, MenuType.Medium, "Hide Full File", "Show Full File", "If true, display the code as ent file", true )
        };

        static FunctionRef[] EntMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "File Name", "Change the name of the file", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalIntField( ref CodeViewsWindow.EntFileID, "Ent ID", "Set the map ID", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalTextInfo( "Info Player Start", "Settings of where to spawn the player", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.InfoPlayerStartOrigin, "- Origin", "Set origin to \"Info Player Start\"", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.InfoPlayerStartAngles, "- Angles", "Set angles to \"Info Player Start\"", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalButton( "Get Player Origin", "Change origin from player position\nif you have your game on", () => LiveMap.GetApexPlayerInfo( PageType.ENT ), !CodeViewsWindow.SendingObjects, MenuType.Medium )
        };

        static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuOffset, OffsetSubMenu, MenuType.Medium, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", true )
        };

        static FunctionRef[] OffsetSubMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextInfo( "Starting Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "- Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalButton( "Get Player Origin", "Change origin from player position\nif you have your game on", () => LiveMap.GetApexPlayerInfo(), !CodeViewsWindow.SendingObjects, MenuType.Medium )
        };

        static FunctionRef[] AdvancedMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalAdvancedOption()
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.FullFileEntMenu, FullFileMenu, MenuType.Large, "Full File", "Full File", "" );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenu, OffsetMenu, MenuType.Large, "Offset Menu", "Offset Menu", "" );

            CodeViewsMenu.SelectionMenu();

            CodeViewsMenu.CreateMenu( CodeViewsWindow.AdvancedMenu, AdvancedMenu, MenuType.Large, "Advanced Options", "Advanced Options", "Choose the objects you want to\ngenerate or not" );

            CodeViewsMenu.SharedFunctions();
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            ObjectType[] showOnly = new ObjectType[]
            {
                ObjectType.Prop,
                ObjectType.VerticalZipLine,
                ObjectType.NonVerticalZipLine,
                ObjectType.SingleDoor,
                ObjectType.DoubleDoor,
                ObjectType.HorzDoor,
                ObjectType.VerticalDoor,
              //ObjectType.JumpTower,
                ObjectType.LootBin,
                ObjectType.FuncWindowHint
            };

            Helper.ForceHideBoolToGenerateObjects( showOnly, true );

            Vector3 IPSAngles = CodeViewsWindow.InfoPlayerStartAngles;
            Vector3 IPSOrigin = CodeViewsWindow.InfoPlayerStartOrigin;


            StringBuilder code = new StringBuilder();

            if ( MenuInit.IsEnable( CodeViewsWindow.FullFileEntSubMenu ) )
            {
                AppendCode( ref code, $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}" );
                AppendCode( ref code,  "{" );
                AppendCode( ref code,  "\"spawnflags\" \"0\"" );
                AppendCode( ref code,  "\"scale\" \"1\"" );
                AppendCode( ref code, $"\"angles\" \"{Helper.ReplaceComma( IPSAngles.x )} {Helper.ReplaceComma( IPSAngles.y )} {Helper.ReplaceComma( IPSAngles.z )}\"" );
                AppendCode( ref code, $"\"origin\" \"{Helper.ReplaceComma( IPSOrigin.x )} {Helper.ReplaceComma( IPSOrigin.y )} {Helper.ReplaceComma( IPSOrigin.z )}\"" );
                AppendCode( ref code,  "\"classname\" \"info_player_start\"" );
                AppendCode( ref code,  "}" );
            }

            AppendCode( ref code, await Helper.BuildMapCode( BuildType.EntFile, CodeViewsWindow.SelectionEnable() ), 0 );

            if ( MenuInit.IsEnable( CodeViewsWindow.FullFileEntSubMenu ) ) AppendCode( ref code, "\u0000", 0 );

            return code.ToString();
        }
    }
}