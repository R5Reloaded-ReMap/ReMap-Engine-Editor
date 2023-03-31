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
    internal enum MenuType
    {
        Menu,
        SubMenu
    }
    
    internal delegate void FunctionRef();

    public class CodeViewsMenu
    {
        private static int GUI_MenuSize = 297;
        private static int GUI_SubMenuSize = 274;
        internal static Color GUI_SettingsColor = new Color( 255f, 255f, 255f );

        internal static bool ShowDevMenu = false;
        internal static bool ShowSubDevMenu = false;

        internal static FunctionRef[] DevMenu = new FunctionRef[]
        {
            () => CreateSubMenu( SubDevMenu, "Sub Menu", "Sub Menu", "", ref ShowSubDevMenu )
        };

        internal static FunctionRef[] SubDevMenu = new FunctionRef[]
        {
            () => CodeViewsMenu.OptionalTextInfo( $"Window Size: {CodeViewsWindow.windowSize.x} x {CodeViewsWindow.windowSize.y}", "Show the current size of the window" )
        };

        internal static void CreateMenu( FunctionRef[] functionRefs, string trueText, string falseText, string tooltip, ref bool value )
        {
            Internal_CreateButton( functionRefs, trueText, falseText, tooltip, ref value, MenuType.Menu, GUI_MenuSize );
        }

        internal static void CreateSubMenu( FunctionRef[] functionRefs, string trueText, string falseText, string tooltip, ref bool value )
        {
            Internal_CreateButton( functionRefs, trueText, falseText, tooltip, ref value, MenuType.SubMenu, GUI_SubMenuSize );
        }

        internal static void Internal_CreateButton( FunctionRef[] functionRefs, string trueText, string falseText, string tooltip, ref bool value, MenuType menuType, int buttonWidth )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent( value ? trueText : falseText, tooltip );
            GUIContent buttonContentInfo = new GUIContent( value ? CodeViewsWindow.enableLogo : CodeViewsWindow.disableLogo, tooltip );

            GUILayout.BeginHorizontal();
            if ( menuType == MenuType.SubMenu )
            {
                Space( 26 );
            }

            if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 ) ) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( buttonWidth ) ) )
            {
                value = !value;
                CodeViewsWindow.Refresh();
            }
            GUILayout.EndHorizontal();

            FunctionInit( functionRefs, value );
        }

        internal static void FunctionInit( FunctionRef[] functionRefs, bool value )
        {
            if ( functionRefs.Length != 0 && value )
            {
                CallFunctions( functionRefs );

                Space( 6 ); Separator( 318 );
            }
            else Space( 10 );
        }

        internal static void CallFunctions( FunctionRef[] functionRefs )
        {
            foreach ( FunctionRef functionRef in functionRefs )
            {
                Space( 4 );
                functionRef();
            }
        }


        //   ██████╗ ██████╗ ████████╗██╗ ██████╗ ███╗   ██╗███████╗
        //  ██╔═══██╗██╔══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║██╔════╝
        //  ██║   ██║██████╔╝   ██║   ██║██║   ██║██╔██╗ ██║███████╗
        //  ██║   ██║██╔═══╝    ██║   ██║██║   ██║██║╚██╗██║╚════██║
        //  ╚██████╔╝██║        ██║   ██║╚██████╔╝██║ ╚████║███████║
        //   ╚═════╝ ╚═╝        ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝
        internal static void OptionalTextField( ref string reference, string text = "text field", string tooltip = "", bool ? condition = null )
        {
            if ( condition != null && !condition.Value ) return;

            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( 96 ) );
                reference = EditorGUILayout.TextField( new GUIContent( "", tooltip ), reference, GUILayout.Width( 220 ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalToggle( ref bool reference, ref bool referenceTemp, string text = "toggle", string tooltip = "", bool ? condition = null )
        {
            if ( condition != null && !condition.Value ) return;

            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( 302 ) );
                reference = EditorGUILayout.Toggle( "", reference, GUILayout.MaxWidth( 0 ) );

                if( reference != referenceTemp ) referenceTemp = reference;

            GUILayout.EndHorizontal();
        }

        internal static void OptionalIntField( ref int reference, string text = "int field", string tooltip = "", bool ? condition = null )
        {
            if ( condition != null && !condition.Value ) return; //EditorGUILayout.LabelField( new GUIContent( "Ent ID", "Set the map ID" ), GUILayout.MaxWidth( 277 ) );

            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.MaxWidth( 240 ) );
                reference = EditorGUILayout.IntField( "", reference, GUILayout.MaxWidth( 77 ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalVector3Field( ref Vector3 reference, string text = "vector3 field", string tooltip = "", bool ? condition = null )
        {
            if ( condition != null && !condition.Value ) return;

            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.MaxWidth( 107 ) );
                reference = EditorGUILayout.Vector3Field( "", reference, GUILayout.MaxWidth( 210 ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalTextInfo( string text = "text field", string tooltip = "", bool ? condition = null )
        {
            if ( condition != null && !condition.Value ) return;

            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( 316 ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalAdvancedOption()
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                    if ( GUILayout.Button( "Check All", buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 158 ) ) )
                    {
                        Helper.ForceSetBoolToGenerateObjects( Helper.GetAllObjectTypeInArray(), true );
                        CodeViewsWindow.Refresh();
                    }

                    Space( 2 );

                    if ( GUILayout.Button( "Uncheck All", buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 158 ) ) )
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
                    
                    Space( 4 );

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

        internal static void Space( float value )
        {
            GUILayout.Space( value );
        }

        internal static void Separator( int space )
        {
            GUI.backgroundColor = GUI_SettingsColor;
            GUILayout.Box( "", GUILayout.Width( space ), GUILayout.Height( 4 ) );
            GUI.backgroundColor = Color.white;
        }

        //  ██████╗ ███████╗██╗   ██╗    ███╗   ███╗███████╗███╗   ██╗██╗   ██╗
        //  ██╔══██╗██╔════╝██║   ██║    ████╗ ████║██╔════╝████╗  ██║██║   ██║
        //  ██║  ██║█████╗  ██║   ██║    ██╔████╔██║█████╗  ██╔██╗ ██║██║   ██║
        //  ██║  ██║██╔══╝  ╚██╗ ██╔╝    ██║╚██╔╝██║██╔══╝  ██║╚██╗██║██║   ██║
        //  ██████╔╝███████╗ ╚████╔╝     ██║ ╚═╝ ██║███████╗██║ ╚████║╚██████╔╝
        //  ╚═════╝ ╚══════╝  ╚═══╝      ╚═╝     ╚═╝╚══════╝╚═╝  ╚═══╝ ╚═════╝ 

    }

}
