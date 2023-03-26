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
    public class ScriptTab
    {
        internal static void OnGUITab()
        {
            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    CodeViewsWindow.ShowSquirrelFunction();
                    if ( CodeViewsWindow.ShowFunction ) CodeViewsWindow.OptionalFunctionName();
                GUILayout.EndHorizontal();

                GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                GUILayout.BeginHorizontal();
                    CodeViewsWindow.OptionalUseOffset();
                    if ( Helper.UseStartingOffset ) CodeViewsWindow.OptionalShowOffset();
                    if ( Helper.UseStartingOffset && Helper.ShowStartingOffset ) CodeViewsWindow.OptionalOffsetField();
                GUILayout.EndHorizontal();

                    GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                GUILayout.BeginHorizontal();
                    CodeViewsWindow.OptionalSelection();
                GUILayout.EndHorizontal();

                    GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                GUILayout.BeginHorizontal();
                    CodeViewsWindow.OptionalAdvancedOption();
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

            code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction, CodeViewsWindow.StartingOffset.x, CodeViewsWindow.StartingOffset.y, CodeViewsWindow.StartingOffset.z );

            Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );
            
            code += Helper.BuildMapCode( BuildType.Script, CodeViewsWindow.EnableSelection );

            if ( CodeViewsWindow.ShowFunction ) code += "}";

            return code;
        }
    }
}