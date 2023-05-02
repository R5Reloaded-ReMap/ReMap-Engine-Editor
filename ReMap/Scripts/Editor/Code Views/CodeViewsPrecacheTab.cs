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
    public class PrecacheTab
    {
        static FunctionRef[] SquirrelMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowFunction, SquirrelFunction, MenuType.Small, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", true )
        };

        static FunctionRef[] SquirrelFunction = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function", null, MenuType.Small )
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenu, SquirrelMenu, MenuType.Large, "Function Menu", "Function Menu", "" );

            CodeViewsMenu.SelectionMenu();

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

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );

            code += await BuildObjectsWithEnum( ObjectType.Prop, BuildType.Precache, MenuInit.IsEnable( CodeViewsWindow.SelectionMenu ) );

            if ( MenuInit.IsEnable( CodeViewsWindow.SquirrelMenuShowFunction ) ) code += "}";

            return code;
        }
    }
}