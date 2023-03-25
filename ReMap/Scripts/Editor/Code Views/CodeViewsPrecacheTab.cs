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
        internal static void OnGUITab()
        {
            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    CodeViewsWindow.ShowSquirrelFunction();
                    if ( CodeViewsWindow.ShowFunction ) CodeViewsWindow.OptionalFunctionName();
                GUILayout.EndHorizontal();
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