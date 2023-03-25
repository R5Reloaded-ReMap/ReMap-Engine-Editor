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

                    CodeViewsWindow.ShowSquirrelFunction();

                    if ( CodeViewsWindow.ShowFunction ) CodeViewsWindow.OptionalFunctionName();

                    GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

                CodeViewsWindow.CodeOutput();
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
                code += $"void function {CodeViewsWindow.functionName}()"; PageBreak( ref code );
                code += "{"; PageBreak( ref code );
                code += Helper.ReMapCredit();
            }

            code += BuildObjectsWithEnum( ObjectType.Prop, BuildType.Precache );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            return code;
        }
    }
}