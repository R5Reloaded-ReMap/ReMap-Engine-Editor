
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
        internal static Vector2 windowSize;
        internal static int paramToggleSize = 190;
        internal static int tab = 0;
        internal static int tab_temp = 0;
        internal static int tabEnt = 0;
        internal static int tabEnt_temp = 0;
        internal static Vector3 StartingOffset = Vector3.zero;
        internal static int GUILayoutToggleSize = 180;
        internal static int GUILayoutVector3FieldSize = 210;
        internal static int GUILayoutFunctionFieldSize = 210;
        internal static int GUILayoutLabelSize = 40;

        internal static bool ShowOptions = false;
        internal static bool ShowAdvancedMenu = false;
        internal static bool ShowAdvancedMenuTemp = false;
        internal static bool ShowFunction = false;
        internal static bool ShowFunctionTemp = false;
        internal static bool ShowEntFunction = false;
        internal static bool ShowEntFunctionTemp = false;
        internal static bool EnableSelection = false;
        internal static bool EnableSelectionTemp = false;
        internal static int EntityCount = 0;
        internal static int EntFileID = 27;

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
        }

        void OnEnable()
        {
            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

            Refresh();
        }

        private void EditorSceneManager_sceneSaved( UnityEngine.SceneManagement.Scene arg0 )
        {
            Refresh();
        }

        private void EditorSceneManager_sceneOpened( UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode )
        {
            Refresh();
        }

        internal static void GenerateCorrectCode( bool copy )
        {
            code = "";

            switch ( tab )
            {
                case 0: // Squirrel Code
                    code += ScriptTab.GenerateCode( copy );
                    functionName = Helper.GetSceneName();
                    break;

                case 1: // DataTable Code
                    code += DataTableTab.GenerateCode( copy );
                    functionName = $"remap_datatable";
                    break;

                case 2: // Precache Code
                    code += PrecacheTab.GenerateCode( copy );
                    functionName = $"{Helper.GetSceneName()}_Precache";
                    break;

                case 3: // Ent Code
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            code += ScriptEntTab.GenerateCode( copy );
                            functionName = $"mp_rr_remap_script";
                            break;

                        case 1: // Sound Code
                            code += SoundEntTab.GenerateCode( copy );
                            functionName = $"mp_rr_remap_snd";
                            break;
                        case 2:  // Spawn Code
                            code += SpawnEntTab.GenerateCode( copy );
                            functionName = $"mp_rr_remap_spawn";
                        break;
                    }
                break;
            }

            if( copy ) GUIUtility.systemCopyBuffer = code;
        }

        void OnGUI()
        {
            MainTab();

            GetEditorWindowSize(); ShortCut();

            if( tab != tab_temp || tabEnt != tabEnt_temp )
            {
                Refresh();
                tab_temp = tab;
                tabEnt_temp = tabEnt;
            }

            switch ( tab )
            {
                case 0: // Squirrel Code
                    ScriptTab.OnGUITab();
                    break;

                case 1: // DataTable Code
                    DataTableTab.OnGUITab();
                    break;

                case 2: // Precache Code
                    PrecacheTab.OnGUITab();
                    break;

                case 3: // Ent Code
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            ScriptEntTab.OnGUITab();
                            break;

                        case 1: // Sound Code
                            SoundEntTab.OnGUITab();
                            break;

                        case 2: // Spawn Code
                            SpawnEntTab.OnGUITab();
                        break;
                    }
                break;
            }

            GUILayout.BeginVertical( "box" );
                    CodeViewsWindow.CodeOutput();
        
                    if ( GUILayout.Button( "Copy To Clipboard" ) ) GenerateCorrectCode( true );
            GUILayout.EndVertical();
        }

        internal static void MainTab()
        {
            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    tab = GUILayout.Toolbar ( tab, toolbarTab );
                    if ( GUILayout.Button("Refresh", GUILayout.Width( 100 ) ) ) Refresh();
                GUILayout.EndHorizontal();

                if ( tab == 3 )
                {
                    GUILayout.BeginHorizontal();
                        tabEnt = GUILayout.Toolbar ( tabEnt, toolbarSubTabEntCode );
                    GUILayout.EndHorizontal();           
                }

                GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                GUILayout.BeginHorizontal();
                        CodeViewsWindow.ObjectCount();
                        GUILayout.FlexibleSpace();
                        CodeViewsWindow.ExportButton();
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal static void ShortCut()
        {
            Event currentEvent = Event.current;

            if ( currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.R ) Refresh();
        } 

        internal static void OptionalUseOffset( string text = "Use Origin Offset" )
        {
            Helper.UseStartingOffset = EditorGUILayout.Toggle( text, Helper.UseStartingOffset, GUILayout.MaxWidth( CodeViewsWindow.GUILayoutToggleSize ) );
            if( Helper.UseStartingOffset != Helper.UseStartingOffsetTemp )
            {
                Helper.UseStartingOffsetTemp = Helper.UseStartingOffset;
                Refresh();
            }
        }

        internal static void OptionalShowOffset( string text = "Show Origin Offset" )
        {
            Helper.ShowStartingOffset = EditorGUILayout.Toggle( text, Helper.ShowStartingOffset, GUILayout.MaxWidth( GUILayoutToggleSize ) );
            if( Helper.ShowStartingOffset != Helper.ShowStartingOffsetTemp )
            {
                Helper.ShowStartingOffsetTemp = Helper.ShowStartingOffset;
                Refresh();
            }
        }

        internal static void OptionalOffsetField( string text = "Offset" )
        {
            EditorGUILayout.LabelField( text, GUILayout.MaxWidth( GUILayoutLabelSize ) );
            StartingOffset = EditorGUILayout.Vector3Field( "", StartingOffset, GUILayout.MaxWidth( GUILayoutVector3FieldSize ) );
        }

        internal static void ShowSquirrelFunction( string text = "Show Squirrel Function" )
        {
            ShowFunction = EditorGUILayout.Toggle( text, ShowFunction, GUILayout.MaxWidth( GUILayoutToggleSize ) );
            if( ShowFunction != ShowFunctionTemp )
            {
                ShowFunctionTemp = ShowFunction;
                Refresh();
            }
        }

        internal static void ShowSquirrelEntFunction( string text = "Show Full File" )
        {
            ShowEntFunction = EditorGUILayout.Toggle( text, ShowEntFunction, GUILayout.MaxWidth( GUILayoutToggleSize ) );
            if( ShowEntFunction != ShowEntFunctionTemp )
            {
                ShowEntFunctionTemp = ShowEntFunction;
                Refresh();
            }

            if ( ShowEntFunction )
            {
                EditorGUILayout.LabelField( "Ent ID", GUILayout.MaxWidth( GUILayoutLabelSize ) );

                string userInput = EditorGUILayout.TextField( EntFileID.ToString(), GUILayout.MaxWidth( GUILayoutLabelSize ) );

                userInput = Regex.Replace( userInput, "[^0-9]", "" );

                if ( int.TryParse( userInput, out EntFileID ) )
                {
                    EntFileID = int.Parse( userInput );
                }
            }
        }

        internal static void ObjectCount()
        {
            string info = tab == 2 ? "Models Precached Count" : "Entity Count";
            GUILayout.BeginHorizontal();

                SetCorrectColor( EntityCount );
                GUILayout.Label( $" // {info}: {EntityCount} | {SetCorrectEntityLabel( EntityCount )}", EditorStyles.boldLabel );
                GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        internal static void OptionalAdvancedOption( string text = "Show Advanced Options" )
        {
            GUILayout.BeginVertical();

                ShowAdvancedMenu = EditorGUILayout.Toggle( text, ShowAdvancedMenu, GUILayout.MaxWidth( GUILayoutToggleSize ) );

                if( ShowAdvancedMenu != ShowAdvancedMenuTemp )
                {
                    ShowAdvancedMenuTemp = ShowAdvancedMenu;
                    Refresh();
                }

                if ( !ShowAdvancedMenu )
                {
                    GUILayout.EndVertical();
                    return;
                }

                int idx = 0; //List< string > keysToModify = new List< string >();
                GUILayout.BeginHorizontal();
                    foreach ( string key in GenerateObjectsFunction.Keys )
                    {
                        ObjectType? type = Helper.GetObjectTypeByObjName( key );
                        ObjectType typed = ( ObjectType ) type;

                        if ( IsHided( typed ) ) continue;

                        //if ( !IsValidScriptEntParam( typed ) ) continue;

                        GenerateObjectsFunctionTemp[key] = EditorGUILayout.Toggle( $"Build {key}", GenerateObjectsFunctionTemp[key], GUILayout.Width( paramToggleSize ) );

                        if ( GenerateObjects[key] != GenerateObjectsFunctionTemp[key] )
                        {
                            GenerateObjects[key] = GenerateObjectsFunctionTemp[key];
                            Refresh();
                        }

                        if ( idx == Mathf.FloorToInt( windowSize.x / paramToggleSize ) - 1 )
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            idx = 0;
                        } else idx++;
                    }
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal static void OptionalFunctionName( string text = "Function Name" )
        {
            EditorGUILayout.LabelField( text, GUILayout.Width( 92 ) );
            functionName = EditorGUILayout.TextField( "", functionName, GUILayout.Width( GUILayoutFunctionFieldSize ) );
        }

        internal static void OptionalSelection( string text = "Build Selection Only" )
        {
            EnableSelection = EditorGUILayout.Toggle( text, EnableSelection, GUILayout.MaxWidth( GUILayoutToggleSize ) );
            if( EnableSelection != EnableSelectionTemp )
            {
                EnableSelectionTemp = EnableSelection;
                Refresh();
            }
        }

        internal static void ExportButton( string text = "Export Code" )
        {
            if ( GUILayout.Button( text, GUILayout.Width( 100 ) ) ) ExportFunction();
        }

        internal static void ExportFunction()
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

        internal static void CodeOutput()
        {
            scroll = EditorGUILayout.BeginScrollView( scroll );
                GUILayout.TextArea( code, GUILayout.ExpandHeight( true ) );
            EditorGUILayout.EndScrollView();
        }

        internal static void Refresh()
        {
            EntityCount = 0; GenerateCorrectCode( false );
        }

        private static void SetCorrectColor( int count )
        {
            if( count < greenPropCount )
                GUI.contentColor = Color.green;

            else if( ( count < yellowPropCount ) ) 
                GUI.contentColor = Color.yellow;

            else GUI.contentColor = Color.red;
        }

        public static bool IsHided( ObjectType objectType )
        {
            // Ensure the objectData is not empty
            GameObject[] objectData;
            if ( EnableSelection )
                objectData = Helper.GetSelectedObjectWithEnum( objectType );
            else objectData = Helper.GetObjArrayWithEnum( objectType );

            if ( objectData.Length == 0 ) return true;

            // Check if objectType is declared hiden or not
            foreach ( ObjectType hidedObjectType in GenerateIgnore )
            {
                if ( hidedObjectType == objectType ) return true;
            }

            return false;
        }

        private static string SetCorrectEntityLabel( int count )
        {
            if( count < greenPropCount )
                return "Status: Safe";

            else if( ( count < yellowPropCount ) ) 
                return "Status: Safe";

            else return "Status: Warning! Game could crash!";
        }

        private static void GetEditorWindowSize()
        {
            EditorWindow editorWindow = windowInstance;
            if ( editorWindow != null )
            {
                windowSize = new Vector2( editorWindow.position.width, editorWindow.position.height );
            }
        }
    }
}