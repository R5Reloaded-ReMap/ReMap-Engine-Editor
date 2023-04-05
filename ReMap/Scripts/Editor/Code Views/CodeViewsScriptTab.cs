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

namespace CodeViewsWindow
{
    public class ScriptTab
    {
        static FunctionRef[] SquirrelMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function" )
        };

        static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateSubMenu( OffsetSubMenu, "Hide Origin Offset", "Show Origin Offset", "Show/Hide \"vector startingorg = < 0, 0, 0 >\"", ref Helper.ShowStartingOffset )
        };

        static FunctionRef[] OffsetSubMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "Starting Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", Helper.ShowStartingOffset, MenuType.SubMenu )
        };

        static FunctionRef[] AdvancedMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalAdvancedOption()
        };

        internal static FunctionRef[] LiveCode = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalButton( "Send Map Code To Game", "Dev Test", () => LiveMap.Send(), null ),
            () => CodeViewsMenu.CreateSubMenu( CodeViewsMenu.SubEmptyMenu, "Disable Auto Send Live Map Code", "Enable Auto Send Live Map Code", "Automaticly Sends Live Map Code", ref CodeViewsWindow.EnableAutoLiveMapCode ),
            () => CodeViewsMenu.CreateSubMenu( SubLiveCodeAdvancedMenu, "Hide Advanced", "Show Advanced", "Restart your game and rewrite\nthe script to spawn your map", ref CodeViewsWindow.ShowSubAdvancedLiveMenu )
        };

        internal static FunctionRef[] SubLiveCodeAdvancedMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalButton( "Restart Level And Write Script", "Generate your map in mp_rr_remap.nut\nand reload the level", () => LiveMap.ReloadLevel(), null, MenuType.SubMenu ),
            () => CodeViewsMenu.OptionalButton( "Reset Script", "Reset mp_rr_remap.nut", () => LiveMap.ReloadLevel( true ), null, MenuType.SubMenu )
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( SquirrelMenu, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", ref CodeViewsWindow.ShowFunction );

            CodeViewsMenu.CreateMenu( OffsetMenu, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", ref Helper.UseStartingOffset );

            CodeViewsMenu.CreateMenu( CodeViewsMenu.SubEmptyMenu, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", ref CodeViewsWindow.EnableSelection );

            CodeViewsMenu.CreateMenu( LiveCode, "Hide Live Generation", "Show Live Generation", "Allows you to send commands to\nspawn prop if your game is open", ref CodeViewsWindow.ShowLiveMenu );

            CodeViewsMenu.CreateMenu( AdvancedMenu, "Hide Advanced Options", "Show Advanced Options", "Choose the objects you want to\ngenerate or not", ref CodeViewsWindow.ShowAdvancedMenu );

            CodeViewsMenu.SharedFunctions();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            string code = "";

            if ( CodeViewsWindow.ShowFunction )
            {
                code += $"void function {CodeViewsWindow.functionName}()\n";
                code += "{\n";
                code += Helper.ReMapCredit();
            }

            code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z );

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[] { ObjectType.Sound, ObjectType.SpawnPoint } );
            
            code += await Helper.BuildMapCode( BuildType.Script, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            if ( CodeViewsWindow.EnableAutoLiveMapCode ){
                LiveMap.BuildWaitMS = 25;
                LiveMap.Send();
                LiveMap.BuildWaitMS = 50;
            }

            return code;
        }
    }
}