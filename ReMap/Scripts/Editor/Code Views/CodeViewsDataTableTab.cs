using System.Text;
using System.Threading.Tasks;
using Build;
using UnityEngine;
using WindowUtility;
using static Build.Build;

namespace CodeViews
{
    public class DataTableTab
    {
        private static readonly FunctionRef[] OffsetMenu =
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuOffset, OffsetSubMenu, MenuType.Medium, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", true )
        };

        private static readonly FunctionRef[] OffsetSubMenu =
        {
            () => CodeViewsMenu.OptionalTextInfo( "Starting Origin (Apex Vector)", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "- Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", null, MenuType.Small )
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenu, OffsetMenu, MenuType.Large, "Offset Menu", "Offset Menu" );

            CodeViewsMenu.SelectionMenu();

            CodeViewsMenu.SharedFunctions();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            Helper.ForceHideBoolToGenerateObjects( CodeViewsWindow.EmptyObjectType );

            var code = new StringBuilder();

            AppendCode( ref code, await BuildObjectsWithEnum( ObjectType.Prop, BuildType.DataTable, CodeViewsWindow.SelectionEnable() ), 0 );

            return code.ToString();
        }
    }
}