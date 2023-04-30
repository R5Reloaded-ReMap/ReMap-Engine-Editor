using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using WindowUtility;

namespace CodeViewsWindow
{
    public enum MenuType
    {
        Menu,
        SubMenu
    }

    public class CodeViewsMenu
    {
        private static int GUI_MenuSize = 297;
        private static int GUI_SubMenuSize = 274;
        private static int GUI_SubMenuSpace = 25;
        internal static Color GUI_SettingsColor = new Color( 255f, 255f, 255f );

        internal static FunctionRef[] DevMenu = new FunctionRef[]
        {
            () => CreateMenu( CodeViewsWindow.DevMenuDebugInfo, SubEmptyMenu, MenuType.SubMenu, "Hide Debug Info", "Show Debug Info", "Get infos from current window" )
        };

        internal static FunctionRef[] SubEmptyMenu = new FunctionRef[0];

        internal static void SharedFunctions()
        {
            #if ReMapDev
                CreateMenu( CodeViewsWindow.DevMenu, DevMenu, MenuType.Menu, "Dev Menu", "Dev Menu", "" );
            #endif
        }

        internal static void SelectionMenu()
        {
            CodeViewsMenu.CreateMenu( CodeViewsWindow.SelectionMenu, CodeViewsMenu.SubEmptyMenu, MenuType.Menu, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", true );
        }

        internal static MenuInit CreateMenu( string name, FunctionRef[] functionRef, MenuType menuType = MenuType.Menu, string trueText = "", string falseText = "", string tooltip = "", bool refresh = false )
        {
            MenuInit menu;

            if ( MenuInit.Exist( name ) )
            {
                menu = MenuInit.Find( name );
                menu.Content = functionRef; 
            }
            else menu = new MenuInit( name, functionRef, menuType );

            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent( menu.IsOpen ? trueText : falseText, tooltip );
            GUIContent buttonContentInfo = new GUIContent( menu.IsOpen ? CodeViewsWindow.enableLogo : CodeViewsWindow.disableLogo, tooltip );

            GUILayout.BeginHorizontal();

            Space( menu.Space );

            if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 ) ) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( menu.Width ) ) )
            {
                menu.IsOpen = !menu.IsOpen;

                if ( refresh ) CodeViewsWindow.Refresh();
            }
            GUILayout.EndHorizontal();

            Internal_FunctionInit( menu );

            return menu;
        }

        internal static void Internal_FunctionInit( MenuInit menu )
        {
            if ( menu.Content.Length != 0 && menu.IsOpen )
            {
                CallFunctions( menu.Content );

                if ( menu.MenuType == MenuType.SubMenu ) GUILayout.BeginHorizontal();
                Space( menu.Space ); Separator( menu.SeparatorWidth );
                if ( menu.MenuType == MenuType.SubMenu ) GUILayout.EndHorizontal();
            }
            else if ( menu.MenuType == MenuType.Menu ) Space( 10 );
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

            float space = menuType == MenuType.Menu ? 2 : 25;
            float labelSpace = menuType == MenuType.Menu ? 96 : 89;
            float fieldSpace = menuType == MenuType.Menu ? 220 : 200;

            GUILayout.BeginHorizontal();
                Space( space );

                WindowUtility.WindowUtility.CreateTextField( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalToggle( ref bool reference, ref bool referenceTemp, string text = "toggle", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Menu ? 2 : 25;
            float labelSpace = menuType == MenuType.Menu ? 302 : 279;

            GUILayout.BeginHorizontal();
                Space( space );

                WindowUtility.WindowUtility.CreateToggle( ref reference, text, tooltip, labelSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalIntField( ref int reference, string text = "int field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float labelSpace = menuType == MenuType.Menu ? 240 : 219;
            float fieldSpace = menuType == MenuType.Menu ? 77 : 70;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );

                WindowUtility.WindowUtility.CreateIntField( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalVector3Field( ref Vector3 reference, string text = "vector3 field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float labelSpace = menuType == MenuType.Menu ? 107 : 89;
            float fieldSpace = menuType == MenuType.Menu ? 210 : 200;

            GUILayout.BeginHorizontal();
                if ( menuType == MenuType.SubMenu ) Space( GUI_SubMenuSpace );

                WindowUtility.WindowUtility.CreateVector3Field( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalTextInfo( string text = "text", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Menu )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Menu ? 316 : 289;

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

            int space = menuType == MenuType.Menu ? 314 : 292;

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
                    WindowUtility.WindowUtility.CreateButton( "Check All", "", () => CheckOptionalAdvancedOption( true ), 156 );

                    WindowUtility.WindowUtility.CreateButton( "Uncheck All", "", () => CheckOptionalAdvancedOption( false ), 156 );
                GUILayout.EndHorizontal();

                foreach ( string key in CodeViewsWindow.GenerateObjectsFunction.Keys )
                {
                    ObjectType? type = Helper.GetObjectTypeByObjName( key );
                    ObjectType typed = ( ObjectType ) type;

                    if ( CodeViewsWindow.IsHided( typed ) ) continue;
                    
                    Space( 4 );

                    GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField( new GUIContent( $"Build {key}", CodeViewsWindow.GenerateObjectsFunctionTemp[key] ? $"Disable {key}" : $"Enable {key}" ), GUILayout.Width( 296 ) );
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
    }

    public class MenuInit
    {
        public static List < MenuInit > MenuArray = new List< MenuInit >();
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public FunctionRef[] Content { get; set; }
        public MenuType MenuType { get; set; }

        public int Space { get; set; }
        public int Width { get; set; }
        public int SeparatorWidth { get; set; }

        public MenuInit( string name, FunctionRef[] functionRef, MenuType menuType )
        {
            if ( Exist( name ) ) return;

            Name = name;
            IsOpen = false;
            Content = functionRef;
            MenuType = menuType;

            switch ( menuType )
            {
                case MenuType.Menu:
                    Space = 2;
                    Width = 292;
                    SeparatorWidth = 312;
                    break;                
                case MenuType.SubMenu:
                    Space = 25;
                    Width = 269;
                    SeparatorWidth = 291;
                break;
            }

            MenuArray.Add( this );
        }

        public static bool Exist( string name )
        {
            foreach ( MenuInit menu in MenuArray )
            {
                if ( menu.Name == name ) return true;
            }

            return false;
        }

        public static MenuInit Find( string name )
        {
            foreach ( MenuInit menu in MenuArray )
            {
                if ( menu.Name == name ) return menu;
            }

            return null;
        }

        public static bool IsEnable( string name )
        {
            MenuInit menu = Find( name );

            if ( menu != null )
            {
                return menu.IsOpen;
            }

            return false;
        }

        public static void SetBool( string name, bool value )
        {
            MenuInit menu = Find( name );

            if ( menu != null )
            {
                menu.IsOpen = value;
            }
        }
    }
}
