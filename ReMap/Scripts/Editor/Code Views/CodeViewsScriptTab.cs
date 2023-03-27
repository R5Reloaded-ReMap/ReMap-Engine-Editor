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
        static bool toggleState;
        static GUIContent buttonContent; // test
        static bool buttonCondition = false;
        internal static void OnGUISettingsTab()
        {
            GUILayout.BeginVertical( "box" );

            EditorGUILayout.BeginHorizontal();
            toggleState = EditorGUILayout.Toggle("Toggle:", toggleState);

            GUIContent buttonContent;

            if (toggleState)
            {
                buttonContent = EditorGUIUtility.IconContent("console.infoicon.sml");
            }
            else
            {
                buttonContent = EditorGUIUtility.IconContent("console.warnicon.sml");
            }

            buttonContent.text = " My Text";
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            //buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.contentOffset = new Vector2( -60, 0 );
            buttonStyle.padding.left = 20;

            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.Width(200), GUILayout.Height(30)))
            {
                buttonCondition = !buttonCondition;
            }
            EditorGUILayout.EndHorizontal();


            CodeViewsWindow.scrollSettings = GUILayout.BeginScrollView( CodeViewsWindow.scrollSettings, false, false );

            GUIStyle toggleStyle = new GUIStyle( EditorStyles.toggle );
            toggleStyle.padding.left = 24;
            toggleStyle.margin.left = 0;
            toggleStyle.fixedWidth = 240;

            toggleState = EditorGUILayout.Toggle( "Custom Toggle", toggleState );
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

            
            // test
            if (toggleState)
            {
                buttonContent = EditorGUIUtility.IconContent("console.infoicon.sml");
            }
            else
            {
                buttonContent = EditorGUIUtility.IconContent("console.warnicon.sml");
            }

            //if (GUILayout.Button(buttonContent, GUILayout.Width(40), GUILayout.Height(20)))
            //{
            //    toggleState = !toggleState;
            //}
            // test

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