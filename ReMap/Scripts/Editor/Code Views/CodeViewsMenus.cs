using UnityEditor;
using UnityEngine;
using WindowUtility;

namespace CodeViewsWindow
{
    internal enum MenuType
    {
        Menu,
        SubMenu
    }

    public class CodeViewsMenu
    {
        private static int GUI_MenuSize = 297;
        private static int GUI_SubMenuSize = 274;
        private static int GUI_SubMenuSpace = 26;
        internal static Color GUI_SettingsColor = new Color( 255f, 255f, 255f );

        internal static FunctionRef[] DevMenu = new FunctionRef[]
        {
            () => CreateSubMenu( SubEmptyMenu, "Hide Debug Info", "Show Debug Info", "Get infos from current window", ref CodeViewsWindow.EnableDevInfo )
        };

        internal static FunctionRef[] SubEmptyMenu = new FunctionRef[0];

        //internal static FunctionRef[] EnablePushMapCode = new FunctionRef[1];

        internal static void SharedFunctions()
        {
            #if ReMapDev
            CreateMenu( DevMenu, "Dev Menu", "Dev Menu", "", ref CodeViewsWindow.ShowDevMenu );
            #endif
        }


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

                WindowUtility.WindowUtility.CreateTextField( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalToggle( ref bool reference, ref bool referenceTemp, string text = "toggle", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Menu ? 302 : 279;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );

                WindowUtility.WindowUtility.CreateToggle( ref reference, text, tooltip, space );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalIntField( ref int reference, string text = "int field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float labelSpace = menuType == MenuType.Menu ? 240 : 224;
            float fieldSpace = menuType == MenuType.Menu ? 77 : 70;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );

                WindowUtility.WindowUtility.CreateIntField( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalVector3Field( ref Vector3 reference, string text = "vector3 field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float labelSpace = menuType == MenuType.Menu ? 107 : 98;
            float fieldSpace = menuType == MenuType.Menu ? 210 : 196;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );

                WindowUtility.WindowUtility.CreateVector3Field( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalTextInfo( string text = "text", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Menu ? 316 : 286;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );
                WindowUtility.WindowUtility.CreateTextInfo( text, tooltip, space );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalButton( string text = "button", string tooltip = "", FunctionRef functionRef = null, bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            int space = menuType == MenuType.Menu ? 320 : 297;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );

                WindowUtility.WindowUtility.CreateButton( text, tooltip, functionRef, space, 20 );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalAdvancedOption()
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                    WindowUtility.WindowUtility.CreateButton( "Check All", "", () => CheckOptionalAdvancedOption( true ), 158 );

                    Space( 1 );

                    WindowUtility.WindowUtility.CreateButton( "Uncheck All", "", () => CheckOptionalAdvancedOption( false ), 158 );
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

        private static void CheckOptionalAdvancedOption( bool value )
        {
            Helper.ForceSetBoolToGenerateObjects( Helper.GetAllObjectTypeInArray(), value );
            CodeViewsWindow.Refresh();
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
