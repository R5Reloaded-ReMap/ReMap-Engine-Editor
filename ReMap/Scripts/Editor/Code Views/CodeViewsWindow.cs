
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
    public class CodeViewsWindow : EditorWindow
    {
        internal static CodeViewsWindow windowInstance;
        internal static string code = "";
        internal static string functionName = "unnamed";
        internal static string[] toolbarTab = new[] { "Squirrel Code", "DataTable Code", "Precache Code", "Ent Code" };
        internal static string[] toolbarSubTabEntCode = new[] { "script.ent", "snd.ent", "spawn.ent" };
        internal static Vector2 scroll;
        internal static Vector2 scrollSettings;
        internal static Vector2 windowSize;
        internal static int tab = 0;
        internal static int tab_temp = 0;
        internal static int tabEnt = 0;
        internal static int tabEnt_temp = 0;
        internal static Vector3 StartingOffset = Vector3.zero;
        internal static int GUILayoutButtonSize = 297; // 320 - 23

        internal static bool ShowSettings = false;
        internal static bool ShowAdvancedMenu = false;
        internal static bool ShowFunction = false;
        internal static bool ShowEntFunction = false;
        internal static bool ShowEntFunctionTemp = false;
        internal static bool EnableSelection = false;
        internal static int EntityCount = 0;
        internal static int EntFileID = 27;
        internal static Vector3 InfoPlayerStartOrigin = Vector3.zero;
        internal static Vector3 InfoPlayerStartAngles = Vector3.zero;

        internal static Color SettingsColor = new Color( 255f, 255f, 255f );
        internal static Texture2D enableLogo;
        internal static Texture2D disableLogo;

        // Gen Settings
        public static Dictionary< string, bool > GenerateObjects = Helper.ObjectGenerateDictionaryInit();
        public static Dictionary< string, bool > GenerateObjectsFunction = new Dictionary< string, bool >( GenerateObjects );
        public static Dictionary< string, bool > GenerateObjectsFunctionTemp = new Dictionary< string, bool >( GenerateObjects );
        public static ObjectType[] GenerateIgnore = new ObjectType[0];

        public static int greenPropCount = 1500;
        public static int yellowPropCount = 3000;


        [ MenuItem( "ReMap/Code Views", false, 25 ) ]
        public static void Init()
        {
            TagHelper.CheckAndCreateTags(); tab = 0; tabEnt = 0;

            windowInstance = ( CodeViewsWindow ) GetWindow( typeof( CodeViewsWindow ), false, "Code Views" );
            windowInstance.minSize = new Vector2( 1100, 500 );
            windowInstance.Show();
            GetFunctionName();
        }

        void OnEnable()
        {
            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

            enableLogo = Resources.Load( "icons/codeViewEnable" ) as Texture2D;
            disableLogo = Resources.Load( "icons/codeViewDisable" ) as Texture2D;

            Refresh();
        }

        void OnGUI()
        {
            MainTab();

            GetEditorWindowSize(); ShortCut();

            if( tab != tab_temp || tabEnt != tabEnt_temp )
            {
                GetFunctionName();
                Refresh();
                tab_temp = tab;
                tabEnt_temp = tabEnt;
            }

            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();

                    CodeOutput();
                    
                    if ( ShowSettings )
                    {
                        GUILayout.BeginVertical( "box", GUILayout.Width( 340 ) );
                            SettingsMenu();
                        GUILayout.EndVertical();
                    }

                GUILayout.EndHorizontal();
        
                if ( GUILayout.Button( "Copy To Clipboard" ) ) GenerateCorrectCode( true );
            GUILayout.EndVertical();
        }

        public static bool IsHided( ObjectType objectType )
        {
            // Ensure the objectData is not empty
            GameObject[] objectData;
            if ( EnableSelection )
                objectData = Helper.GetSelectedObjectWithEnum( objectType );
            else objectData = Helper.GetObjArrayWithEnum( objectType );

            if ( objectData.Length == 0 ) return true;

            // Check if objectType are flaged hide
            foreach ( ObjectType hidedObjectType in GenerateIgnore )
            {
                if ( hidedObjectType == objectType ) return true;
            }

            return false;
        }

        //  ██╗███╗   ██╗████████╗███████╗██████╗ ███╗   ██╗ █████╗ ██╗         ███████╗██╗   ██╗███╗   ██╗ ██████╗████████╗██╗ ██████╗ ███╗   ██╗███████╗
        //  ██║████╗  ██║╚══██╔══╝██╔════╝██╔══██╗████╗  ██║██╔══██╗██║         ██╔════╝██║   ██║████╗  ██║██╔════╝╚══██╔══╝██║██╔═══██╗████╗  ██║██╔════╝
        //  ██║██╔██╗ ██║   ██║   █████╗  ██████╔╝██╔██╗ ██║███████║██║         █████╗  ██║   ██║██╔██╗ ██║██║        ██║   ██║██║   ██║██╔██╗ ██║███████╗
        //  ██║██║╚██╗██║   ██║   ██╔══╝  ██╔══██╗██║╚██╗██║██╔══██║██║         ██╔══╝  ██║   ██║██║╚██╗██║██║        ██║   ██║██║   ██║██║╚██╗██║╚════██║
        //  ██║██║ ╚████║   ██║   ███████╗██║  ██║██║ ╚████║██║  ██║███████╗    ██║     ╚██████╔╝██║ ╚████║╚██████╗   ██║   ██║╚██████╔╝██║ ╚████║███████║
        //  ╚═╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝    ╚═╝      ╚═════╝ ╚═╝  ╚═══╝ ╚═════╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝

        //  ██╗   ██╗██╗    ██╗   ██╗████████╗██╗██╗     ██╗████████╗██╗   ██╗
        //  ██║   ██║██║    ██║   ██║╚══██╔══╝██║██║     ██║╚══██╔══╝╚██╗ ██╔╝
        //  ██║   ██║██║    ██║   ██║   ██║   ██║██║     ██║   ██║    ╚████╔╝ 
        //  ██║   ██║██║    ██║   ██║   ██║   ██║██║     ██║   ██║     ╚██╔╝  
        //  ╚██████╔╝██║    ╚██████╔╝   ██║   ██║███████╗██║   ██║      ██║   
        //   ╚═════╝ ╚═╝     ╚═════╝    ╚═╝   ╚═╝╚══════╝╚═╝   ╚═╝      ╚═╝   
        internal static void Refresh()
        {
            EntityCount = 0; GenerateCorrectCode( false );
        }


        //   ██████╗ ██████╗ ████████╗██╗ ██████╗ ███╗   ██╗ █████╗ ██╗         ██╗   ██╗██╗
        //  ██╔═══██╗██╔══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║██╔══██╗██║         ██║   ██║██║
        //  ██║   ██║██████╔╝   ██║   ██║██║   ██║██╔██╗ ██║███████║██║         ██║   ██║██║
        //  ██║   ██║██╔═══╝    ██║   ██║██║   ██║██║╚██╗██║██╔══██║██║         ██║   ██║██║
        //  ╚██████╔╝██║        ██║   ██║╚██████╔╝██║ ╚████║██║  ██║███████╗    ╚██████╔╝██║
        //   ╚═════╝ ╚═╝        ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝     ╚═════╝ ╚═╝

        internal static void ShowSquirrelFunction( string trueText = "Hide Squirrel Function", string falseText = "Show Squirrel Function", string tooltip = "If true, display the code as a function" )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent( ShowFunction ? trueText : falseText, tooltip );

            GUIContent buttonContentInfo = new GUIContent( ShowFunction ? enableLogo : disableLogo, tooltip );
        
            GUILayout.BeginHorizontal();
                if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 )) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( GUILayoutButtonSize )))
                {
                    ShowFunction = !ShowFunction;
                    Refresh();
                }
            GUILayout.EndHorizontal();
        }

        internal static void OptionalFunctionName( string text = "Function Name", string tooltip = "Change the name of the function" )
        {
            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( 96 ) );
                functionName = EditorGUILayout.TextField( new GUIContent( "", tooltip ), functionName, GUILayout.Width( 220 ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalUseOffset( string trueText = "Disable Origin Offset", string falseText = "Enable Origin Offset", string tooltip = "If true, add a position offset to objects" )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent( Helper.UseStartingOffset ? trueText : falseText, tooltip );

            GUIContent buttonContentInfo = new GUIContent( Helper.UseStartingOffset ? enableLogo : disableLogo, tooltip );

            GUILayout.BeginHorizontal();
                if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 )) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( GUILayoutButtonSize )))
                {
                    Helper.UseStartingOffset = !Helper.UseStartingOffset;
                    Refresh();
                }
            GUILayout.EndHorizontal();
        }

        internal static void OptionalShowOffset( string text = "Show Origin Offset", string tooltip = "Show/Hide \"vector startingorg = < 0, 0, 0 >\"" )
        {
            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( 302 ) );

                Helper.ShowStartingOffset = EditorGUILayout.Toggle( "", Helper.ShowStartingOffset, GUILayout.Width( 0 ) );

                if( Helper.ShowStartingOffset != Helper.ShowStartingOffsetTemp )
                {
                    Helper.ShowStartingOffsetTemp = Helper.ShowStartingOffset;
                    Refresh();
                }
            GUILayout.EndHorizontal();
        }

        internal static void OptionalOffsetField( string text = "Starting Origin", string tooltip = "Change origins in \"vector startingorg = < 0, 0, 0 >\"" )
        {
            GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.MaxWidth( 107 ) );

                StartingOffset = EditorGUILayout.Vector3Field( "", StartingOffset, GUILayout.MaxWidth( 210 ) );
                
            GUILayout.EndHorizontal();
        }

        internal static void OptionalSelection( string trueText = "Disable Selection Only", string falseText = "Enable Selection Only", string tooltip = "If true, generates the code of the selection only" )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent( EnableSelection ? trueText : falseText, tooltip );

            GUIContent buttonContentInfo = new GUIContent( EnableSelection ? enableLogo : disableLogo, tooltip );

            GUILayout.BeginHorizontal();
                if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 )) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( GUILayoutButtonSize )))
                {
                    EnableSelection = !EnableSelection;
                    Refresh();
                }
            GUILayout.EndHorizontal();
        }

        internal static void ShowSquirrelEntFunction( string trueText = "Hide Full File", string falseText = "Show Full File", string tooltip = "If true, display the code as ent file" )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent( ShowEntFunction ? trueText : falseText, tooltip );

            GUIContent buttonContentInfo = new GUIContent( ShowEntFunction ? enableLogo : disableLogo, tooltip );
        
            GUILayout.BeginHorizontal();
                if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 )) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( GUILayoutButtonSize )))
                {
                    ShowEntFunction = !ShowEntFunction;
                    Refresh();
                }
            GUILayout.EndHorizontal();
        }

        internal static void OptionalMapID()
        {
            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( "Ent ID", "Set the map ID" ), GUILayout.MaxWidth( 277 ) );

                string userInput = EditorGUILayout.TextField( EntFileID.ToString(), GUILayout.MaxWidth( 40 ) );
                userInput = Regex.Replace( userInput, "[^0-9]", "" );

                if ( int.TryParse( userInput, out EntFileID ) ) EntFileID = int.Parse( userInput );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalInfoPlayerStart()
        {
            EditorGUILayout.LabelField( new GUIContent( "Info Player Start", "Settings of where to spawn the player" ), GUILayout.MaxWidth( 107 ) );
            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( "- Origin", "Set origin to \"Info Player Start\"" ), GUILayout.MaxWidth( 107 ) );

                InfoPlayerStartOrigin = EditorGUILayout.Vector3Field( "", InfoPlayerStartOrigin, GUILayout.MaxWidth( 210 ) );
            GUILayout.EndHorizontal();
            CodeViewsWindow.Space( 4 );
            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( new GUIContent( "- Angles", "Set angles to \"Info Player Start\"" ), GUILayout.MaxWidth( 107 ) );

                InfoPlayerStartAngles = EditorGUILayout.Vector3Field( "", InfoPlayerStartAngles, GUILayout.MaxWidth( 210 ) );
            GUILayout.EndHorizontal();
        }

        internal static void OptionalAdvancedOption( string trueText = "Hide Advanced Options", string falseText = "Show Advanced Options", string tooltip = "Choose the objects you want to\ngenerate or not" )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUIContent buttonContent = new GUIContent( ShowAdvancedMenu ? trueText : falseText, tooltip );

            GUIContent buttonContentInfo = new GUIContent( ShowAdvancedMenu ? enableLogo : disableLogo, tooltip );
        
            GUILayout.BeginHorizontal();
                if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 20 )) || GUILayout.Button( buttonContent, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( GUILayoutButtonSize )))
                {
                    ShowAdvancedMenu = !ShowAdvancedMenu;
                    Refresh();
                }
            GUILayout.EndHorizontal();

            if ( !ShowAdvancedMenu ) return;

            GUILayout.BeginVertical();
                foreach ( string key in GenerateObjectsFunction.Keys )
                {
                    ObjectType? type = Helper.GetObjectTypeByObjName( key );
                    ObjectType typed = ( ObjectType ) type;

                    if ( IsHided( typed ) ) continue;
                    
                    Space( 4 );

                    GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField( new GUIContent( $"Build {key}", GenerateObjectsFunctionTemp[key] ? $"Disable {key}" : $"Enable {key}" ), GUILayout.Width( 302 ) );
                        GenerateObjectsFunctionTemp[key] = EditorGUILayout.Toggle( "", GenerateObjectsFunctionTemp[key], GUILayout.MaxWidth( 0 ) );
                    GUILayout.EndHorizontal();

                    if ( GenerateObjects[key] != GenerateObjectsFunctionTemp[key] )
                    {
                        GenerateObjects[key] = GenerateObjectsFunctionTemp[key];
                        Refresh();
                    }
                }
            GUILayout.EndVertical();
            Space( 6 );
            Separator();
        }

        internal static void Space( float value )
        {
            GUILayout.Space( value );
        }

        internal static void Separator()
        {
            Space( 2 );
            GUI.backgroundColor = SettingsColor;
            GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 4 ) );
            GUI.backgroundColor = Color.white;
        }


        //  ██████╗ ██████╗ ██╗██╗   ██╗ █████╗ ████████╗███████╗    ███████╗██╗   ██╗███╗   ██╗ ██████╗████████╗██╗ ██████╗ ███╗   ██╗███████╗
        //  ██╔══██╗██╔══██╗██║██║   ██║██╔══██╗╚══██╔══╝██╔════╝    ██╔════╝██║   ██║████╗  ██║██╔════╝╚══██╔══╝██║██╔═══██╗████╗  ██║██╔════╝
        //  ██████╔╝██████╔╝██║██║   ██║███████║   ██║   █████╗      █████╗  ██║   ██║██╔██╗ ██║██║        ██║   ██║██║   ██║██╔██╗ ██║███████╗
        //  ██╔═══╝ ██╔══██╗██║╚██╗ ██╔╝██╔══██║   ██║   ██╔══╝      ██╔══╝  ██║   ██║██║╚██╗██║██║        ██║   ██║██║   ██║██║╚██╗██║╚════██║
        //  ██║     ██║  ██║██║ ╚████╔╝ ██║  ██║   ██║   ███████╗    ██║     ╚██████╔╝██║ ╚████║╚██████╗   ██║   ██║╚██████╔╝██║ ╚████║███████║
        //  ╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝  ╚═╝  ╚═╝   ╚═╝   ╚══════╝    ╚═╝      ╚═════╝ ╚═╝  ╚═══╝ ╚═════╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝

        private void EditorSceneManager_sceneSaved( UnityEngine.SceneManagement.Scene arg0 )
        {
            Refresh();
        }

        private void EditorSceneManager_sceneOpened( UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode )
        {
            Refresh();
        }


        //  ███╗   ███╗ █████╗ ██╗███╗   ██╗    ██╗   ██╗██╗
        //  ████╗ ████║██╔══██╗██║████╗  ██║    ██║   ██║██║
        //  ██╔████╔██║███████║██║██╔██╗ ██║    ██║   ██║██║
        //  ██║╚██╔╝██║██╔══██║██║██║╚██╗██║    ██║   ██║██║
        //  ██║ ╚═╝ ██║██║  ██║██║██║ ╚████║    ╚██████╔╝██║
        //  ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝     ╚═════╝ ╚═╝
        private static void MainTab()
        {
            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    tab = GUILayout.Toolbar ( tab, toolbarTab );
                    if ( GUILayout.Button( new GUIContent( "Refresh", "Refresh Window" ), GUILayout.Width( 100 ) ) ) Refresh();
                GUILayout.EndHorizontal();

                if ( tab == 3 )
                {
                    GUILayout.BeginHorizontal();
                        tabEnt = GUILayout.Toolbar ( tabEnt, toolbarSubTabEntCode );
                    GUILayout.EndHorizontal();           
                }

                GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                GUILayout.BeginHorizontal();
                        ObjectCount();
                        GUILayout.FlexibleSpace();
                        ExportButton();
                        SettingsMenuButton();
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void SettingsMenu()
        {
            switch ( tab )
            {
                case 0: // Squirrel Code
                    ScriptTab.OnGUISettingsTab();
                    break;

                case 1: // DataTable Code
                    DataTableTab.OnGUISettingsTab();
                    break;

                case 2: // Precache Code
                    PrecacheTab.OnGUISettingsTab();
                    break;

                case 3: // Ent Code
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            ScriptEntTab.OnGUISettingsTab();
                            break;

                        case 1: // Sound Code
                            SoundEntTab.OnGUISettingsTab();
                            break;

                        case 2: // Spawn Code
                            SpawnEntTab.OnGUISettingsTab();
                        break;
                    }
                break;
            }
        }

        private static void ObjectCount()
        {
            string info = tab == 2 ? "Models Precached Count" : "Entity Count";
            GUILayout.BeginHorizontal();

                SetCorrectColor( EntityCount );
                GUILayout.Label( $" // {info}: {EntityCount} | {SetCorrectEntityLabel( EntityCount )}", EditorStyles.boldLabel );
                GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        private static void ExportButton( string text = "Export Code", string tooltip = "Export current code" )
        {
            if ( GUILayout.Button( new GUIContent( text, tooltip ), GUILayout.Width( 100 ) ) ) ExportFunction();
        }

        private static void SettingsMenuButton( string text = "Settings", string tooltip = "Show settings" )
        {
            if ( GUILayout.Button( new GUIContent( text, tooltip ), GUILayout.Width( 100 ) ) ) ShowSettings = !ShowSettings;
        }

        private static void CodeOutput()
        {
            scroll = EditorGUILayout.BeginScrollView( scroll );
                GUILayout.TextArea( code, GUILayout.ExpandHeight( true ) );
            EditorGUILayout.EndScrollView();
        }


        //  ██╗   ██╗██╗     ██╗   ██╗████████╗██╗██╗     ██╗████████╗██╗   ██╗
        //  ██║   ██║██║     ██║   ██║╚══██╔══╝██║██║     ██║╚══██╔══╝╚██╗ ██╔╝
        //  ██║   ██║██║     ██║   ██║   ██║   ██║██║     ██║   ██║    ╚████╔╝ 
        //  ██║   ██║██║     ██║   ██║   ██║   ██║██║     ██║   ██║     ╚██╔╝  
        //  ╚██████╔╝██║     ╚██████╔╝   ██║   ██║███████╗██║   ██║      ██║   
        //   ╚═════╝ ╚═╝      ╚═════╝    ╚═╝   ╚═╝╚══════╝╚═╝   ╚═╝      ╚═╝   

        private static void ShortCut()
        {
            Event currentEvent = Event.current;

            if ( currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.R ) Refresh();
        } 

        private static void GetEditorWindowSize()
        {
            EditorWindow editorWindow = windowInstance;
            if ( editorWindow != null )
            {
                windowSize = new Vector2( editorWindow.position.width, editorWindow.position.height );
            }
        }

        private static void GenerateCorrectCode( bool copy )
        {
            code = "";

            switch ( tab )
            {
                case 0: // Squirrel Code
                    code += ScriptTab.GenerateCode( copy );
                    break;

                case 1: // DataTable Code
                    code += DataTableTab.GenerateCode( copy );
                    break;

                case 2: // Precache Code
                    code += PrecacheTab.GenerateCode( copy );
                    break;

                case 3: // Ent Code
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            code += ScriptEntTab.GenerateCode( copy );
                            break;

                        case 1: // Sound Code
                            code += SoundEntTab.GenerateCode( copy );
                            break;
                        case 2:  // Spawn Code
                            code += SpawnEntTab.GenerateCode( copy );
                        break;
                    }
                break;
            }

            if( copy ) GUIUtility.systemCopyBuffer = code;
        }

        private static void GetFunctionName()
        {
            switch ( tab )
            {
                case 0: // Squirrel Code
                    functionName = Helper.GetSceneName();
                    break;

                case 1: // DataTable Code
                    functionName = $"remap_datatable";
                    break;

                case 2: // Precache Code
                    functionName = $"{Helper.GetSceneName()}_Precache";
                    break;

                case 3: // Ent Code
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            functionName = $"mp_rr_remap_script";
                            break;

                        case 1: // Sound Code
                            functionName = $"mp_rr_remap_snd";
                            break;
                        case 2:  // Spawn Code
                            functionName = $"mp_rr_remap_spawn";
                        break;
                    }
                break;
            }
        }

        private static string SetCorrectEntityLabel( int count )
        {
            if( count < greenPropCount )
                return "Status: Safe";

            else if( ( count < yellowPropCount ) ) 
                return "Status: Safe";

            else return "Status: Warning! Game could crash!";
        }

        private static void SetCorrectColor( int count )
        {
            if( count < greenPropCount )
                GUI.contentColor = Color.green;

            else if( ( count < yellowPropCount ) ) 
                GUI.contentColor = Color.yellow;

            else GUI.contentColor = Color.red;
        }

        private static void ExportFunction()
        {
            Helper.FixPropTags();

            EditorSceneManager.SaveOpenScenes();

            Refresh();

            string[] fileInfo = new string[4];

            switch ( tab )
            {
                case 0: // Squirrel Code
                    fileInfo = new[] { "Squirrel Code Export", "", $"{functionName}.nut", "nut" };
                    break;

                case 1: // DataTable Code
                    fileInfo = new[] { "DataTable Code Export", "", $"{functionName}.csv", "csv" };
                    break;

                case 2: // Precache Code
                    fileInfo = new[] { "Precache Code Export", "", $"{functionName}.nut", "nut" };
                    break;

                case 3: // Ent Code
                    fileInfo = new[] { "Precache Code Export", "", "", "ent" };
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            fileInfo[2] = $"{functionName}.ent";
                            break;

                        case 1: // Sound Code
                            fileInfo[2] = $"{functionName}.ent";
                            break;

                        case 2: // Spawn Code
                            fileInfo[2] = $"{functionName}.ent";
                        break;
                    }
                break;
            }

            var path = EditorUtility.SaveFilePanel( fileInfo[0], fileInfo[1], fileInfo[2], fileInfo[3] );

            if ( path.Length == 0 ) return;

            File.WriteAllText( path, code );
        }
    }
}