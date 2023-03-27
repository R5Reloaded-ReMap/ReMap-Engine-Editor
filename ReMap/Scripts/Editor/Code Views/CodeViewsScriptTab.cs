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

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.imagePosition = ImagePosition.ImageLeft;

            GUIContent buttonContent = new GUIContent(" Your Text", CodeViewsWindow.ShowFunction ? "console.infoicon.sml" : "console.warnicon.sml");
        
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.Height(20), GUILayout.Width(340)))
            {
                CodeViewsWindow.ShowFunction = !CodeViewsWindow.ShowFunction;
            }

            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            CodeViewsWindow.ShowSquirrelFunction();
            if ( CodeViewsWindow.ShowFunction ) CodeViewsWindow.OptionalFunctionName();

            GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

            CodeViewsWindow.OptionalUseOffset();
            if ( Helper.UseStartingOffset ) CodeViewsWindow.OptionalShowOffset();
            if ( Helper.UseStartingOffset && Helper.ShowStartingOffset ) CodeViewsWindow.OptionalOffsetField();

            GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

            CodeViewsWindow.OptionalSelection();

            GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

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