using System.Text;
using System.Threading.Tasks;
using Build;
using UnityEngine;
using WindowUtility;
using static Build.Build;

namespace CodeViews
{
    public class CameraPathTab
    {
        private static readonly FunctionRef[] SquirrelMenu =
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowFunction, SquirrelFunction, MenuType.Medium, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", true )
        };

        private static readonly FunctionRef[] SquirrelFunction =
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalButton( "Reset Name", "", () => CodeViewsWindow.ResetFunctionName(), null, MenuType.Small, true )
        };

        private static readonly FunctionRef[] OffsetMenu =
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuOffset, OffsetSubMenu, MenuType.Medium, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", true )
        };

        private static readonly FunctionRef[] OffsetSubMenu =
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuShowOffset, CodeViewsMenu.EmptyFunctionRefArray, MenuType.Medium, "Hide Origin Offset", "Show Origin Offset", "Show/Hide \"vector startingorg = < 0, 0, 0 >\"", true ),
            () => CodeViewsMenu.OptionalTextInfo( "Starting Origin (Apex Vector)", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", MenuInit.IsEnable( CodeViewsWindow.OffsetMenuShowOffset ), MenuType.Small ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "- Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", MenuInit.IsEnable( CodeViewsWindow.OffsetMenuShowOffset ), MenuType.Small )
        };

        private static readonly ObjectType[] forceShow =
        {
            ObjectType.CameraPath
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenu, SquirrelMenu, MenuType.Large, "Function Menu", "Function Menu" );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenu, OffsetMenu, MenuType.Large, "Offset Menu", "Offset Menu" );

            CodeViewsMenu.SelectionMenu();

            CodeViewsMenu.SharedFunctions();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            Helper.ForceHideBoolToGenerateObjects( forceShow, true );

            var code = new StringBuilder();

            if ( CodeViewsWindow.ShowFunctionEnable() )
            {
                AppendCode( ref code, $"void function {CodeViewsWindow.functionName}()" );
                AppendCode( ref code, "{" );
                AppendCode( ref code, Helper.ReMapCredit(), 0 );
            }

            AppendCode( ref code, Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z ), 0 );

            AppendCode( ref code, await Helper.BuildMapCode( BuildType.Script, CodeViewsWindow.SelectionEnable() ), 0 );

            if ( CodeViewsWindow.ShowFunctionEnable() ) AppendCode( ref code, "}" );

            return code.ToString();
        }
    }
}