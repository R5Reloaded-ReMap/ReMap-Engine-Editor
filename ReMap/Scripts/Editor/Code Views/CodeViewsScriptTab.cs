using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using Build;
using static Build.Build;
using WindowUtility;

namespace CodeViewsWindow
{
    public class ScriptTab
    {
        internal static FunctionRef SendMapCodeButton = () => CodeViewsMenu.OptionalButton( "Send Map Code To Game", "Sends the code live if you have your game on\nNote: Some objects / features do not work,\nuse \"Restart Level And Write Script\" instead", () => LiveMap.Send(), null );

        static FunctionRef[] SquirrelMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowFunction, SquirrelFunction, MenuType.SubMenu, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", true )
        };

        static FunctionRef[] SquirrelFunction = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function", null, MenuType.SubMenu )
        };

        static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuOffset, OffsetSubMenu, MenuType.SubMenu, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", true )
        };

        static FunctionRef[] OffsetSubMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuShowOffset, CodeViewsMenu.EmptyFunctionRefArray, MenuType.SubMenu, "Hide Origin Offset", "Show Origin Offset", "Show/Hide \"vector startingorg = < 0, 0, 0 >\"", true ),
            () => CodeViewsMenu.OptionalTextInfo( "Starting Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", MenuInit.IsEnable( CodeViewsWindow.OffsetMenuShowOffset ), MenuType.SubMenu ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "- Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", MenuInit.IsEnable( CodeViewsWindow.OffsetMenuShowOffset ), MenuType.SubMenu )
        };

        static FunctionRef[] AdvancedMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalAdvancedOption()
        };

        internal static FunctionRef[] LiveCodeMenu = new FunctionRef[]
        {
            SendMapCodeButton,
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenuTeleportation, CodeViewsMenu.EmptyFunctionRefArray, MenuType.SubMenu, "Disable Teleport Player To Map", "Enable Teleport Player To Map", "Automaticly teleport all players to the map when sending the code to the game" ),
            () => CodeViewsMenu.OptionalButton( "Respawn Players", "Makes players respawn without regenerating the map", () => LiveMap.ReMapTeleportToMap(), null, MenuType.SubMenu ),
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenuAutoSend, CodeViewsMenu.EmptyFunctionRefArray, MenuType.SubMenu, "Disable Auto Send Live Map Code", "Enable Auto Send Live Map Code", "Automaticly sends live map code" ),
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenuAdvanced, SubLiveCodeAdvancedMenu, MenuType.SubMenu, "Hide Advanced", "Show Advanced", "Restart your game and rewrite\nthe script to spawn your map" )
        };

        internal static FunctionRef[] SubLiveCodeAdvancedMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalButton( "Restart Level And Write Script", "Generate your map in mp_rr_remap.nut\nand reload the level\nNote: Works only on the survival_dev playlist", () => LiveMap.ReloadLevel(), null, MenuType.SubMenu ),
            () => CodeViewsMenu.OptionalButton( "Reset Script", "Reset mp_rr_remap.nut", () => LiveMap.ReloadLevel( true ), null, MenuType.SubMenu )
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenu, SquirrelMenu, MenuType.Menu, "Function Menu", "Function Menu", "" );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenu, OffsetMenu, MenuType.Menu, "Offset Menu", "Offset Menu", "" );

            CodeViewsMenu.SelectionMenu();

            CodeViewsMenu.CreateMenu( CodeViewsWindow.LiveCodeMenu, LiveCodeMenu, MenuType.Menu, "Live Generation", "Live Generation", "Allows you to send commands to\nspawn prop if your game is open" );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.AdvancedMenu, AdvancedMenu, MenuType.Menu, "Advanced Options", "Advanced Options", "Choose the objects you want to\ngenerate or not" );

            CodeViewsMenu.SharedFunctions();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            string code = "";

            if ( MenuInit.IsEnable( CodeViewsWindow.SquirrelMenuShowFunction ) )
            {
                code += $"void function {CodeViewsWindow.functionName}()\n";
                code += "{\n";
                code += Helper.ReMapCredit();
            }

            code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z );

            ObjectType[] forceHide = new ObjectType[]
            {
                ObjectType.Sound,
                ObjectType.SpawnPoint,
                ObjectType.LiveMapCodePlayerSpawn
            };

            Helper.ForceHideBoolToGenerateObjects( forceHide );
            
            code += await Helper.BuildMapCode( BuildType.Script, MenuInit.IsEnable( CodeViewsWindow.SelectionMenu ) );

            if ( MenuInit.IsEnable( CodeViewsWindow.SquirrelMenuShowFunction ) ) code += "}";

            if ( MenuInit.IsEnable( CodeViewsWindow.LiveCodeMenuAutoSend ) ) LiveMap.Send();

            return code;
        }
    }
}