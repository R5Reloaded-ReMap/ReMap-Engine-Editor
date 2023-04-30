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
    public class SpawnEntTab
    {
        static FunctionRef[] FullFileMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.FullFileEntSubMenu, EntMenu, MenuType.SubMenu, "Hide Full File", "Show Full File", "If true, display the code as ent file", true )
        };

        static FunctionRef[] EntMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "File Name", "Change the name of the file", null, MenuType.SubMenu ),
            () => CodeViewsMenu.OptionalIntField( ref CodeViewsWindow.EntFileID, "Ent ID", "Set the map ID", null, MenuType.SubMenu )
        };

        static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuOffset, OffsetSubMenu, MenuType.SubMenu, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", true )
        };

        static FunctionRef[] OffsetSubMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "Starting Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", null, MenuType.SubMenu )
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.FullFileEntMenu, FullFileMenu, MenuType.Menu, "Full File", "Full File", "" );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenu, OffsetMenu, MenuType.Menu, "Offset Menu", "Offset Menu", "" );

            CodeViewsMenu.SelectionMenu();

            CodeViewsMenu.SharedFunctions();
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            string code = "";

            if ( MenuInit.IsEnable( CodeViewsWindow.FullFileEntSubMenu ) )
            {
                code += $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}\n";
            }

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[] { ObjectType.SpawnPoint }, true );

            code += await Helper.BuildMapCode( BuildType.EntFile, MenuInit.IsEnable( CodeViewsWindow.SelectionMenu ) );

            if ( MenuInit.IsEnable( CodeViewsWindow.FullFileEntSubMenu ) ) code += "\u0000";

            return code;
        }
    }
}