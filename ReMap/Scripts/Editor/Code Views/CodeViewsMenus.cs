using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using Build;
using static Build.Build;

namespace CodeViewsWindow
{
    internal delegate void FunctionRef();

    public class CodeViewsMenu
    {
        internal static void FunctionInit( FunctionRef[] functionRefs, bool value )
        {
            if ( functionRefs.Length != 0 && value )
            {
                foreach ( FunctionRef functionRef in functionRefs )
                {
                    CodeViewsWindow.Space( 4 );
                    functionRef();
                }
                CodeViewsWindow.Space( 6 );
                CodeViewsWindow.Separator();
            } 
            else
            {
                CodeViewsWindow.Space( 10 );
                
            }
        }

        internal static void CreateMenu( FunctionRef[] functionRefs, string trueText, string falseText, string tooltip, ref bool value )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent( value ? trueText : falseText, tooltip );
            GUIContent buttonContentInfo = new GUIContent( value ? CodeViewsWindow.enableLogo : CodeViewsWindow.disableLogo, tooltip );
        
            GUILayout.BeginHorizontal();
                if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 )) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( CodeViewsWindow.GUILayoutButtonSize )))
                {
                    value = !value;
                    CodeViewsWindow.Refresh();
                }
            GUILayout.EndHorizontal();

            FunctionInit( functionRefs, value );
        }

        internal static void AdvancedMenu( FunctionRef[] functionRefs, string trueText, string falseText, string tooltip )
        {

        }


        //   ██████╗ ██████╗ ████████╗██╗ ██████╗ ███╗   ██╗███████╗
        //  ██╔═══██╗██╔══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║██╔════╝
        //  ██║   ██║██████╔╝   ██║   ██║██║   ██║██╔██╗ ██║███████╗
        //  ██║   ██║██╔═══╝    ██║   ██║██║   ██║██║╚██╗██║╚════██║
        //  ╚██████╔╝██║        ██║   ██║╚██████╔╝██║ ╚████║███████║
        //   ╚═════╝ ╚═╝        ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝
        internal static void OptionalTextField( ref string reference, string text = "text field", string tooltip = "" )
        {
            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( 96 ) );
                reference = EditorGUILayout.TextField( new GUIContent( "", tooltip ), reference, GUILayout.Width( 220 ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalToggle( ref bool reference, ref bool referenceTemp, string text = "toggle", string tooltip = "" )
        {
            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( 302 ) );
                reference = EditorGUILayout.Toggle( "", reference, GUILayout.MaxWidth( 0 ) );

                if( reference != referenceTemp ) referenceTemp = reference;

            GUILayout.EndHorizontal();
        }

        internal static void OptionalAdvancedOption()
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                    if ( GUILayout.Button( "Check All", buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( CodeViewsWindow.GUILayoutButtonSize / 2 + 10 )))
                    {
                        Helper.ForceSetBoolToGenerateObjects( Helper.GetAllObjectTypeInArray(), true );
                        CodeViewsWindow.Refresh();
                    }

                    CodeViewsWindow.Space( 1 );

                    if ( GUILayout.Button( "Uncheck All", buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( CodeViewsWindow.GUILayoutButtonSize / 2 + 10 )))
                    {
                        Helper.ForceSetBoolToGenerateObjects( Helper.GetAllObjectTypeInArray(), false );
                        CodeViewsWindow.Refresh();
                    }
                GUILayout.EndHorizontal();

                foreach ( string key in CodeViewsWindow.GenerateObjectsFunction.Keys )
                {
                    ObjectType? type = Helper.GetObjectTypeByObjName( key );
                    ObjectType typed = ( ObjectType ) type;

                    if ( CodeViewsWindow.IsHided( typed ) ) continue;
                    
                    CodeViewsWindow.Space( 4 );

                    GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField( new GUIContent( $"Build {key}", CodeViewsWindow.GenerateObjectsFunctionTemp[key] ? $"Disable {key}" : $"Enable {key}" ), GUILayout.Width( 302 ) );
                        CodeViewsWindow.GenerateObjectsFunctionTemp[key] = EditorGUILayout.Toggle( "", CodeViewsWindow.GenerateObjectsFunctionTemp[key], GUILayout.MaxWidth( 0 ) );
                    GUILayout.EndHorizontal();

                    if ( CodeViewsWindow.GenerateObjects[key] != CodeViewsWindow.GenerateObjectsFunctionTemp[key] )
                    {
                        CodeViewsWindow.GenerateObjects[key] = CodeViewsWindow.GenerateObjectsFunctionTemp[key];
                        CodeViewsWindow.Refresh();
                    }
                }
            GUILayout.EndVertical();
        }

    }

}
