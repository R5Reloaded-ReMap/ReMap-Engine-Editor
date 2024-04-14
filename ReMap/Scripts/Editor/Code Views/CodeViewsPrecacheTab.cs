using System.Text;
using System.Threading.Tasks;
using Build;
using UnityEngine;
using WindowUtility;
using static Build.Build;

namespace CodeViews
{
    public class PrecacheTab
    {
        private static readonly FunctionRef[] SquirrelMenu =
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenuShowFunction, SquirrelFunction, MenuType.Small, "Hide Squirrel Function", "Show Squirrel Function", "If true, display the code as a function", true )
        };

        private static readonly FunctionRef[] SquirrelFunction =
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "Function Name", "Change the name of the function", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalButton( "Reset Name", "", () => CodeViewsWindow.ResetFunctionName(), null, MenuType.Small, true )
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.SquirrelMenu, SquirrelMenu, MenuType.Large, "Function Menu", "Function Menu" );

            CodeViewsMenu.SelectionMenu();

            CodeViewsMenu.SharedFunctions();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            Helper.ForceHideBoolToGenerateObjects( CodeViewsWindow.EmptyObjectType );

            var code = new StringBuilder();

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