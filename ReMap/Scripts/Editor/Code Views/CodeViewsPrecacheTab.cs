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
        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical();
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsWindow.ShowSquirrelFunction();
            if ( CodeViewsWindow.ShowFunction )
            {
                CodeViewsWindow.Space( 4 );
                CodeViewsWindow.OptionalFunctionName();

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

            if ( CodeViewsWindow.ShowFunction )
            {
                code += $"void function {CodeViewsWindow.functionName}()"; PageBreak( ref code );
                code += "{"; PageBreak( ref code );
                code += Helper.ReMapCredit();
            }

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );

            code += BuildObjectsWithEnum( ObjectType.Prop, BuildType.Precache, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            return code;
        }
    }
}