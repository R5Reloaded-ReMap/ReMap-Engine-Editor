using System.Text;
using System.Threading.Tasks;
using Build;
using UnityEngine;
using WindowUtility;
using static Build.Build;

namespace CodeViews
{
    public class SoundEntTab
    {
        private static readonly FunctionRef[] FullFileMenu =
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.FullFileEntSubMenu, EntMenu, MenuType.Medium, "Hide Full File", "Show Full File", "If true, display the code as ent file", true )
        };

        private static readonly FunctionRef[] EntMenu =
        {
            () => CodeViewsMenu.OptionalTextField( ref CodeViewsWindow.functionName, "File Name", "Change the name of the file", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalButton( "Reset Name", "", () => CodeViewsWindow.ResetFunctionName(), null, MenuType.Small, true ),
            () => CodeViewsMenu.OptionalIntField( ref CodeViewsWindow.EntFileID, "Ent ID", "Set the map ID", null, MenuType.Small )
        };

        private static readonly FunctionRef[] OffsetMenu =
        {
            () => CodeViewsMenu.CreateMenu( CodeViewsWindow.OffsetMenuOffset, OffsetSubMenu, MenuType.Medium, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", true )
        };

        private static readonly FunctionRef[] OffsetSubMenu =
        {
            () => CodeViewsMenu.OptionalTextInfo( "Starting Origin (Apex Vector)", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", null, MenuType.Small ),
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "- Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"", null, MenuType.Small )
        };

        private static readonly ObjectType[] forceShow =
        {
            ObjectType.Sound
        };

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsMenu.CreateMenu( CodeViewsWindow.FullFileEntMenu, FullFileMenu, MenuType.Large, "Full File", "Full File" );

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

            if ( MenuInit.IsEnable( CodeViewsWindow.FullFileEntSubMenu ) )
                AppendCode( ref code, $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}" );

            AppendCode( ref code, await Helper.BuildMapCode( BuildType.EntFile, CodeViewsWindow.SelectionEnable() ), 0 );

            if ( MenuInit.IsEnable( CodeViewsWindow.FullFileEntSubMenu ) ) AppendCode( ref code, "\u0000", 0 );

            return code.ToString();
        }
    }
}