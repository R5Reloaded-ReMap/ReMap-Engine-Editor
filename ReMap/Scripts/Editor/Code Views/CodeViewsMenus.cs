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
        private static int GUI_SubMenuSpace = 26;
        internal static Color GUI_SettingsColor = new Color( 255f, 255f, 255f );

        internal static bool ShowDevMenu = false;
        internal static bool ShowSubDevMenu = false;

        internal static FunctionRef[] DevMenu = new FunctionRef[]
        {
            () => CreateSubMenu( SubDevMenu, "Window Info", "Window Info", "Get infos from current window", ref ShowSubDevMenu )
        };

        internal static FunctionRef[] SubDevMenu = new FunctionRef[]
        {
            () => OptionalTextInfo( $"Window Size: {CodeViewsWindow.windowSize.x} x {CodeViewsWindow.windowSize.y}", "Show the current size of the window", null, MenuType.SubMenu ),
            () => OptionalTextInfo( $"Scroll Size: {CodeViewsWindow.scroll.x} x {CodeViewsWindow.scroll.y}", "Show the current size of the code scroll", null, MenuType.SubMenu ),
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
                Space( GUI_SubMenuSpace );
            }

            if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 ) ) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( buttonWidth ) ) )
            {
                value = !value;
                CodeViewsWindow.Refresh();
            }
            GUILayout.EndHorizontal();

            if ( menuType == MenuType.SubMenu )
            {
                SubFunctionInit( functionRefs, value );
            } else FunctionInit( functionRefs, value );

            
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

        internal static void SubFunctionInit( FunctionRef[] functionRefs, bool value )
        {
            if ( functionRefs.Length != 0 && value )
            {
                CallFunctions( functionRefs );

                GUILayout.BeginHorizontal();
                Space( GUI_SubMenuSpace ); Separator( 296 );
                GUILayout.EndHorizontal();
            }
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
        internal static void OptionalTextField( ref string reference, string text = "text field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float labelSpace = menuType == MenuType.Menu ? 96 : 94;
            float fieldSpace = menuType == MenuType.Menu ? 220 : 200;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( labelSpace ) );
                reference = EditorGUILayout.TextField( new GUIContent( "", tooltip ), reference, GUILayout.Width( fieldSpace ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalToggle( ref bool reference, ref bool referenceTemp, string text = "toggle", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Menu ? 302 : 279;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );

                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( space ) );
                reference = EditorGUILayout.Toggle( "", reference, GUILayout.MaxWidth( 0 ) );

                if( reference != referenceTemp ) referenceTemp = reference;

            GUILayout.EndHorizontal();
        }

        internal static void OptionalIntField( ref int reference, string text = "int field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float labelSpace = menuType == MenuType.Menu ? 240 : 224;
            float fieldSpace = menuType == MenuType.Menu ? 77 : 70;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.MaxWidth( labelSpace ) );
                reference = EditorGUILayout.IntField( "", reference, GUILayout.MaxWidth( fieldSpace ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalVector3Field( ref Vector3 reference, string text = "vector3 field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float labelSpace = menuType == MenuType.Menu ? 107 : 98;
            float fieldSpace = menuType == MenuType.Menu ? 210 : 196;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.MaxWidth( labelSpace ) );
                reference = EditorGUILayout.Vector3Field( "", reference, GUILayout.MaxWidth( fieldSpace ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalTextInfo( string text = "text field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Menu ? 316 : 286;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( space ) );
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
