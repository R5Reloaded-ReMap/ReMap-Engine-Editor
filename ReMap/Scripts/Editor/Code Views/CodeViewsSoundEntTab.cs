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
    public class SoundEntTab
    {
        internal static void OnGUITab()
        {
            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    CodeViewsWindow.ShowSquirrelEntFunction();
                GUILayout.EndHorizontal();
                
                GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                GUILayout.BeginHorizontal();
                    CodeViewsWindow.OptionalSelection();
                GUILayout.EndHorizontal();

                GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                GUILayout.BeginHorizontal();
                    CodeViewsWindow.OptionalUseOffset();
                    if ( Helper.UseStartingOffset ) CodeViewsWindow.OptionalOffsetField();
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal static string GenerateCode( bool copy )
        {
            string code = "";

            if ( CodeViewsWindow.ShowEntFunction )
            {
                code += $"ENTITIES02 num_models={CodeViewsWindow.EntFileID}";
                PageBreak( ref code );
            }

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[] { ObjectType.Sound }, true );

            code += Helper.BuildMapCode( BuildType.EntFile, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowEntFunction ) code += "\u0000";

            return code;
        }
    }
}