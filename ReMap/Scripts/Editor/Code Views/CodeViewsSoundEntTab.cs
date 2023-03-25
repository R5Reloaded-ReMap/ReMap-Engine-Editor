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
                GUILayout.BeginHorizontal( "box" );
                    CodeViewsWindow.OptionalUseOffset();
                    if ( Helper.UseStartingOffset ) CodeViewsWindow.OptionalOffsetField();
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal static string GenerateCode( bool copy )
        {
            string code = "";

            code += code += BuildObjectsWithEnum( ObjectType.Sound, BuildType.EntFile );

            return code;
        }
    }
}