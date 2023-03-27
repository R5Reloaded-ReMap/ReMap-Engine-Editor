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
        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical( "box" );
            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsWindow.ShowSquirrelFunction();
            if ( CodeViewsWindow.ShowFunction )
            {
                GUILayout.Space( 6 );
                CodeViewsWindow.OptionalFunctionName();

                CodeViewsWindow.Separator();
            }

                GUILayout.Space( 10 );

            CodeViewsWindow.OptionalUseOffset();
            if ( Helper.UseStartingOffset )
            {
                GUILayout.Space( 6 );
                CodeViewsWindow.OptionalShowOffset();
                if ( Helper.ShowStartingOffset ) CodeViewsWindow.OptionalOffsetField();

                CodeViewsWindow.Separator();
            } 

            GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 4 ) );

            CodeViewsWindow.OptionalSelection();

            GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 4 ) );

            CodeViewsWindow.OptionalAdvancedOption();

            GUILayout.EndScrollView();
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