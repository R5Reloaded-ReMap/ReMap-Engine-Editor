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
    public class DataTableTab
    {
        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsWindow.OptionalUseOffset();
            if ( Helper.UseStartingOffset ) 
            {
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalOffsetField();
                CodeViewsWindow.Space( 6 );
                CodeViewsWindow.Separator();
            } else CodeViewsWindow.Space( 10 );

            CodeViewsWindow.OptionalSelection();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        internal static string GenerateCode()
        {
            string code = "";

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );

            code += BuildObjectsWithEnum( ObjectType.Prop, BuildType.DataTable, CodeViewsWindow.EnableSelection );

            return code;
        }
    }
}