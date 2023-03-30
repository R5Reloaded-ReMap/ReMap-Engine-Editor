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
    public class SpawnEntTab
    {
        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsWindow.ShowSquirrelEntFunction();
            if ( CodeViewsWindow.ShowEntFunction )
            {
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalFunctionName( "File Name", "Change the name of the file" );
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalMapID();
                CodeViewsWindow.Space( 6 );
                CodeViewsWindow.Separator();
            } else CodeViewsWindow.Space( 10 );

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

        internal static async Task< string > GenerateCode()
        {
            string code = "";

            if ( CodeViewsWindow.ShowEntFunction )
            {
                code += $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}\n";
            }

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[] { ObjectType.SpawnPoint }, true );

            code += await Helper.BuildMapCode( BuildType.EntFile, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowEntFunction ) code += "\u0000";

            return code;
        }
    }
}