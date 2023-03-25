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
    public class PrecacheTab
    {
        internal static void OnGUIPrecacheTab()
        {
            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal( "box" );

                    CodeViewsWindow.ObjectCount();

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal( "box" );

                        CodeViewsWindow.ShowFunction = EditorGUILayout.Toggle( "Show Squirrel Function", CodeViewsWindow.ShowFunction, GUILayout.MaxWidth( 180 ) );
                        if( CodeViewsWindow.ShowFunction != CodeViewsWindow.ShowFunctionTemp )
                        {
                            CodeViewsWindow.ShowFunctionTemp = CodeViewsWindow.ShowFunction;
                            CodeViewsWindow.GenerateCorrectCode();
                        }

                        GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

                GUILayout.BeginVertical( "box" );

                    CodeViewsWindow.scroll = EditorGUILayout.BeginScrollView( CodeViewsWindow.scroll );
                        GUILayout.TextArea( CodeViewsWindow.code, GUILayout.ExpandHeight( true ) );
                    EditorGUILayout.EndScrollView();

                GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.BeginVertical( "box" );
        
                if (GUILayout.Button( "Copy To Clipboard" ) ) GenerateCode( true );

            GUILayout.EndVertical();
        }

        internal static string GenerateCode( bool copy )
        {
            string code = "";

            if ( CodeViewsWindow.ShowFunction )
            {
                code += $"{Helper.GetSquirrelSceneNameFunction( false )}_Precache()"; PageBreak( ref code );
                code += "{"; PageBreak( ref code );
                code += Helper.ReMapCredit();
            }

            code += BuildObjectsWithEnum( ObjectType.Prop, BuildType.Precache );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            return code;
        }
    }
}