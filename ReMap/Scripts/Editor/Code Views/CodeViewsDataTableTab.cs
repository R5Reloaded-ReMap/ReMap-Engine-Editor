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
        internal static void OnGUITab()
        {
            GUILayout.BeginVertical( "box" );
                CodeViewsWindow.ShowOptions = EditorGUILayout.Foldout( CodeViewsWindow.ShowOptions, "Options", true );

                if ( CodeViewsWindow.ShowOptions )
                {
                    GUILayout.BeginHorizontal();
                        CodeViewsWindow.OptionalUseOffset();
                        if ( Helper.UseStartingOffset ) CodeViewsWindow.OptionalOffsetField();
                        GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                    GUILayout.BeginHorizontal();
                        CodeViewsWindow.OptionalSelection();
                    GUILayout.EndHorizontal();
                }
            GUILayout.EndVertical();
        }

        internal static string GenerateCode( bool copy )
        {
            string code = "";

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );

            code += BuildObjectsWithEnum( ObjectType.Prop, BuildType.DataTable, CodeViewsWindow.EnableSelection );

            return code;
        }
    }
}