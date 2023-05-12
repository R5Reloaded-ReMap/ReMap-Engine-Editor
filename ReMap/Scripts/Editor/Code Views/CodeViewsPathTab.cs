
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

namespace CodeViewsWindow
{
    public class CameraPathTab
    {
        static FunctionRef[] SquirrelMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowFunction, SquirrelFunction, MenuType.Medium, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", true )
        };

        static FunctionRef[] SquirrelFunction = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function", null, MenuType.Small )
        };

                static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuOffset, OffsetSubMenu, MenuType.Medium, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", true )
        };

        static FunctionRef[] OffsetSubMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuShowOffset, CodeViewsMenu.EmptyFunctionRefArray, MenuType.Medium, "Hide Origin Offset", "Show Origin Offset", "Show/Hide \"vector startingorg = < 0, 0, 0 >\"", true ),
            () => CodeViewsMenu.OptionalTextInfo( "Starting Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", MenuInit.IsEnable( CodeViewsWindow.OffsetMenuShowOffset ), MenuType.Small ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "- Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", MenuInit.IsEnable( CodeViewsWindow.OffsetMenuShowOffset ), MenuType.Small )
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenu, SquirrelMenu, MenuType.Large, "Function Menu", "Function Menu", "" );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenu, OffsetMenu, MenuType.Large, "Offset Menu", "Offset Menu", "" );

            CodeViewsMenu.SelectionMenu();

            CodeViewsMenu.SharedFunctions();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            
            ObjectType[] forceShow = new ObjectType[]
            {
                ObjectType.CameraPath,
            };

            Helper.ForceHideBoolToGenerateObjects( forceShow, true );


            StringBuilder code = new StringBuilder();

            if ( CodeViewsWindow.ShowFunctionEnable() )
            {
                code.Append( $"void function {CodeViewsWindow.functionName}()\n" );
                code.Append( "{\n" );
                code.Append( Helper.ReMapCredit() );
            }

            code.Append( Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z ) );
            
            code.Append( await Helper.BuildMapCode( BuildType.Script, CodeViewsWindow.SelectionEnable() ) );

            if ( CodeViewsWindow.ShowFunctionEnable() ) code.Append( "}\n" );

            return code.ToString();
        }
    }
}