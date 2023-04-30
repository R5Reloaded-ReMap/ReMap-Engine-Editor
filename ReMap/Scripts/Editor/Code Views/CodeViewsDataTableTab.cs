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
    public class DataTableTab
    {
        static FunctionRef[] OffsetMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalVector3Field( ref CodeViewsWindow.StartingOffset, "Starting Origin", "Change origins in \"vector startingorg = < 0, 0, 0 >\"" )
        };

        static FunctionRef[] SelectionMenu = new FunctionRef[0];

        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            //CodeViewsMenu.CreateMenu( OffsetMenu, "Disable Origin Offset", "Enable Origin Offset", "If true, add a position offset to objects", ref Helper.UseStartingOffset );

            //CodeViewsMenu.CreateMenu( SelectionMenu, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", ref CodeViewsWindow.EnableSelection );

            CodeViewsMenu.SharedFunctions();
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static async Task< string > GenerateCode()
        {
            string code = "";

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );

            code += await BuildObjectsWithEnum( ObjectType.Prop, BuildType.DataTable, MenuInit.IsEnable( CodeViewsWindow.SelectionMenu ) );

            return code;
        }
    }
}