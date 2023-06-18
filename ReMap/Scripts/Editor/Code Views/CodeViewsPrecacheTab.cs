
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
    public class PrecacheTab
    {
        static FunctionRef[] SquirrelMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowFunction, SquirrelFunction, MenuType.Small, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", true )
        };

        static FunctionRef[] SquirrelFunction = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalButton( "Reset Name", "", () => CodeViewsWindow.ResetFunctionName(), null, MenuType.Small )
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
            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );


            StringBuilder code = new StringBuilder();

            if ( CodeViewsWindow.ShowFunctionEnable() )
            {
                AppendCode( ref code, $"void function {CodeViewsWindow.functionName}()" );
                AppendCode( ref code, "{" );
                AppendCode( ref code, Helper.ReMapCredit(), 0 );
            }

            AppendCode( ref code, await BuildObjectsWithEnum( ObjectType.Prop, BuildType.Precache, CodeViewsWindow.SelectionEnable() ), 0 );

            if ( CodeViewsWindow.ShowFunctionEnable() ) AppendCode( ref code, "}", 0 );

            return code.ToString();
        }
    }
}