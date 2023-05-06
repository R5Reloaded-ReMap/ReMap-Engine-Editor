using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using WindowUtility;

namespace CodeViewsWindow
{
    public enum MenuType
    {
        Large,
        Medium,
        Small
    }

    public class CodeViewsMenu
    {
        // Debug Only
            static string strRef = "strRef";
            static bool boolRef = true;
            static int intRef = 1;
            static Vector3 vecRef = new Vector3( 0, 0, 0 );
        //

        internal static FunctionRef EmptyFunctionRef = () => { };
        internal static FunctionRef[] EmptyFunctionRefArray = new FunctionRef[0];

        internal static Color GUI_SettingsColor = new Color( 255f, 255f, 255f );

        internal static FunctionRef[] DevMenu = new FunctionRef[]
        {
            () => CreateMenu( CodeViewsWindow.DevMenuDebugInfo, EmptyFunctionRefArray, MenuType.Small, "Hide Debug Info", "Show Debug Info", "Get infos from current window" )
        };

        internal static FunctionRef[] FieldPreview = new FunctionRef[]
        {
            () => OptionalTextField( ref strRef, "text ref", "text field", null, MenuType.Large ),
            () => OptionalToggle( ref boolRef, "toggle ref", "toggle field", null, MenuType.Large ),
            () => OptionalIntField( ref intRef, "int ref", "int field", null, MenuType.Large ),
            () => OptionalVector3Field( ref vecRef, "vector ref", "vector field", null, MenuType.Large ),
            () => OptionalTextInfo( "info ref", "info field", null, MenuType.Large ),
            () => OptionalButton( "button ref", "button field", EmptyFunctionRef, null, MenuType.Large ),

            () => CreateMenu( "SubFieldPreview", SubFieldPreview, MenuType.Small, "Sub Field Preview", "Sub Field Preview", "" )
        };

        internal static FunctionRef[] SubFieldPreview = new FunctionRef[]
        {
            () => OptionalTextField( ref strRef, "text ref", "text field", null, MenuType.Small ),
            () => OptionalToggle( ref boolRef, "toggle ref", "toggle field", null, MenuType.Small ),
            () => OptionalIntField( ref intRef, "int ref", "int field", null, MenuType.Small ),
            () => OptionalVector3Field( ref vecRef, "vector ref", "vector field", null, MenuType.Small ),
            () => OptionalTextInfo( "info ref", "info field", null, MenuType.Small ),
            () => OptionalButton( "button ref", "button field", EmptyFunctionRef, null, MenuType.Small )
        };

        internal static void SharedFunctions()
        {
            #if ReMapDev
                CreateMenu( CodeViewsWindow.DevMenu, DevMenu, MenuType.Large, "Dev Menu", "Dev Menu", "" );
                CreateMenu( "FieldPreview", FieldPreview, MenuType.Large, "Field Preview", "Field Preview", "" );
            #endif
        }

        internal static void SelectionMenu()
        {
            CodeViewsMenu.CreateMenu( CodeViewsWindow.SelectionMenu, EmptyFunctionRefArray, MenuType.Large, "Disable Selection Only", "Enable Selection Only", "If true, generates the code of the selection only", true );
        }

        internal static MenuInit CreateMenu( string name, FunctionRef functionRef, MenuType menuType = MenuType.Large, string trueText = "", string falseText = "", string tooltip = "", bool refresh = false, bool enableSeparator = true )
        {
            return CreateMenu( name, new FunctionRef[] { functionRef }, menuType, trueText, falseText, tooltip, refresh, enableSeparator );
        }

        internal static MenuInit CreateMenu( string name, FunctionRef[] functionRef, MenuType menuType = MenuType.Large, string trueText = "", string falseText = "", string tooltip = "", bool refresh = false, bool enableSeparator = true )
        {
            MenuInit menu;

            if ( MenuInit.Exist( name ) )
            {
                menu = MenuInit.Find( name );
                menu.Content = functionRef; 
            }
            else menu = new MenuInit( name, functionRef, menuType );

            menu.EnableSeparator = enableSeparator;

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

                if ( !menu.EnableSeparator ) return;

                if ( menu.MenuType == MenuType.Medium || menu.MenuType == MenuType.Small ) GUILayout.BeginHorizontal();
                Space( menu.Space ); Separator( menu.SeparatorWidth );
                if ( menu.MenuType == MenuType.Medium || menu.MenuType == MenuType.Small ) GUILayout.EndHorizontal();
            }
            else if ( menu.MenuType == MenuType.Large ) Space( 10 );
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
        internal static void OptionalTextField( ref string reference, string text = "text field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Large )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Large ? 2 : 25;
            float labelSpace = menuType == MenuType.Large ? 92 : 89;
            float fieldSpace = menuType == MenuType.Large ? 220 : 200;

            GUILayout.BeginHorizontal();
                Space( space );

                WindowUtility.WindowUtility.CreateTextField( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalToggle( ref bool reference, string text = "toggle", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Large )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Large ? 2 : 25;
            float labelSpace = menuType == MenuType.Large ? 298 : 275;

            GUILayout.BeginHorizontal();
                Space( space );

                WindowUtility.WindowUtility.CreateToggle( ref reference, text, tooltip, labelSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalIntField( ref int reference, string text = "int field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Large )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Large ? 2 : 25;
            float labelSpace = menuType == MenuType.Large ? 226 : 219;
            float fieldSpace = menuType == MenuType.Large ? 86 : 70;

            GUILayout.BeginHorizontal();
                Space( space );

                WindowUtility.WindowUtility.CreateIntField( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalVector3Field( ref Vector3 reference, string text = "vector3 field", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Large )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Large ? 2 : 25;
            float labelSpace = menuType == MenuType.Large ? 102 : 79;
            float fieldSpace = menuType == MenuType.Large ? 210 : 210;

            GUILayout.BeginHorizontal();
                Space( space );

                WindowUtility.WindowUtility.CreateVector3Field( ref reference, text, tooltip, labelSpace, fieldSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalTextInfo( string text = "text", string tooltip = "", bool ? condition = null, MenuType menuType = MenuType.Large )
        {
            if ( condition != null && !condition.Value ) return;

            float space = menuType == MenuType.Large ? 2 : 25;
            float labelSpace = menuType == MenuType.Large ? 316 : 289;

            GUILayout.BeginHorizontal();
                Space( space );

                WindowUtility.WindowUtility.CreateTextInfo( text, tooltip, labelSpace );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalButton( string text = "button", string tooltip = "", FunctionRef functionRef = null, bool ? condition = null, MenuType menuType = MenuType.Large )
        {
            if ( condition != null && !condition.Value ) return;

            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            int space = menuType == MenuType.Large ? 2 : 25;
            float labelSpace = menuType == MenuType.Large ? 314 : 292;

            GUILayout.BeginHorizontal();
                Space( space );

                WindowUtility.WindowUtility.CreateButton( text, tooltip, functionRef, labelSpace, 20 );
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

                foreach ( string key in CodeViewsWindow.GenerateObjects.Keys )
                {
                    ObjectType? type = Helper.GetObjectTypeByObjName( key );
                    ObjectType typed = ( ObjectType ) type;

                    if ( CodeViewsWindow.IsHided( typed ) ) continue;
                    
                    Space( 4 );

                    GUILayout.BeginHorizontal();
                        bool value = CodeViewsWindow.GenerateObjects[key];
                        OptionalToggle( ref value, $"Build {key}", value ? $"Disable {key}" : $"Enable {key}" );
                    GUILayout.EndHorizontal();

                    if ( CodeViewsWindow.GenerateObjects[key] != value )
                    {
                        CodeViewsWindow.GenerateObjects[key] = value;
                        CodeViewsWindow.Refresh();
                        
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        return;
                    }
                }
            GUILayout.EndVertical();
        }

        internal static void OptionalAdditionalCodeOption()
        {
            GUILayout.BeginVertical();



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
        public bool EnableSeparator { get; set; }
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
            EnableSeparator = true;
            Content = functionRef;
            MenuType = menuType;

            switch ( menuType )
            {
                case MenuType.Large:
                    Space = 2;
                    Width = 292;
                    SeparatorWidth = 312;
                    break;
                case MenuType.Medium:
                    Space = 12;
                    Width = 282;
                    SeparatorWidth = 304;
                    break; 
                case MenuType.Small:
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
