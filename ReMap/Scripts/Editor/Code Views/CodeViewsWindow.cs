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
    public class CodeViewsWindow : EditorWindow
    {
        internal static CodeViewsWindow windowInstance;
        internal static string code = "";
        internal static string[] toolbarTab = new[] { "Squirrel Code", "DataTable Code", "Precache Code", "Ent Code" };
        internal static string[] toolbarSubTabEntCode = new[] { "script.ent", "snd.ent", "spawn.ent" };
        internal static Vector2 scroll;
        internal static Vector2 windowSize;
        internal static int paramToggleSize = 190;
        internal static int tab = 0;
        internal static int tab_temp = 0;
        internal static int tabEnt = 0;
        internal static int tabEnt_temp = 0;

        internal static bool ShowAdvanced = false;
        internal static bool ShowFunction = false;
        internal static bool ShowFunctionTemp = false;
        internal static int EntityCount = 0;

        // Gen Settings
        public static Dictionary< string, bool > GenerateObjects = Helper.ObjectGenerateDictionaryInit();
        public static Dictionary< string, bool > GenerateObjectsFunction = Helper.ObjectGenerateDictionaryInit();
        public static Dictionary< string, bool > GenerateObjectsFunctionTemp = new Dictionary< string, bool >( GenerateObjects );
        public static string[] GenerateIgnore = new string[0];

        public static int greenPropCount = 1500;
        public static int yellowPropCount = 3000;


        [MenuItem("ReMap/Dev Test/Code Views", false, 25)]
        public static void Init()
        {
            TagHelper.CheckAndCreateTags();

            windowInstance = ( CodeViewsWindow ) GetWindow( typeof( CodeViewsWindow ), false, "Code Views" );
            windowInstance.minSize = new Vector2( 1100, 500 );
            windowInstance.Show();
        }

        void OnEnable()
        {
            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

            GetLatestCounts();
            GenerateCorrectCode();
        }

        private void EditorSceneManager_sceneSaved( UnityEngine.SceneManagement.Scene arg0 )
        {
            GetLatestCounts();
            GenerateCorrectCode();
        }

        private void EditorSceneManager_sceneOpened( UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode )
        {
            GetLatestCounts();
            GenerateCorrectCode();
        }

        private void GetLatestCounts()
        {
            switch ( tab )
            {
                case 0: // Squirrel Code
                    EntityCount = UnityInfo.GetAllCount();
                    break;

                case 1: // DataTable Code
                    EntityCount = UnityInfo.GetSpecificObjectCount( ObjectType.Prop );
                    break;

                case 2: // Precache Code
                    EntityCount = 0;
                    break;

                case 3: // Ent Code
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            EntityCount =
                            UnityInfo.GetSpecificObjectCount( ObjectType.Prop ) +
                            UnityInfo.GetSpecificObjectCount( ObjectType.VerticalZipLine ) +
                            UnityInfo.GetSpecificObjectCount( ObjectType.NonVerticalZipLine ) +
                            UnityInfo.GetSpecificObjectCount( ObjectType.Trigger );
                            break;

                        case 1: // Sound Code
                            EntityCount = UnityInfo.GetSpecificObjectCount( ObjectType.Sound );
                            break;
                        case 2:  // Spawn Code
                            EntityCount = UnityInfo.GetSpecificObjectCount( ObjectType.SpawnPoint );
                        break;
                    }
                break;
            }
        }

        internal static void GenerateCorrectCode()
        {
            code = "";
            PageBreak( ref code );

            switch ( tab )
            {
                case 0: // Squirrel Code
                    code = ScriptTab.GenerateCode( false, CodeViewsWindow.ShowFunction );
                    break;

                case 1: // DataTable Code
                    //str = ;
                    break;

                case 2: // Precache Code
                    //str = ;
                    break;

                case 3: // Ent Code
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            //str = ;
                            break;

                        case 1: // Sound Code
                            //str = ;
                            break;
                        case 2:  // Spawn Code
                            //str = ;
                        break;
                    }
                break;
            }
        }

        void OnGUI()
        {
            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal( "box" );
                    tab = GUILayout.Toolbar ( tab, toolbarTab );
                    if ( GUILayout.Button("Reload Page", GUILayout.Width( 100 ) ) )
                    {
                        GenerateCorrectCode(); GetLatestCounts();
                    }
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if ( tab == 3 )
            {
                GUILayout.BeginVertical( "box" );
                    GUILayout.BeginHorizontal( "box" );
                        tabEnt = GUILayout.Toolbar ( tabEnt, toolbarSubTabEntCode );
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GetEditorWindowSize();

            if( tab != tab_temp ) GetLatestCounts();

            switch ( tab )
            {
                case 0: // Squirrel Code
                    ScriptTab.OnGUIScriptTab();
                    break;

                case 1: // DataTable Code
                    break;

                case 2: // Precache Code
                    break;

                case 3: // Ent Code
                    switch ( tabEnt )
                    {
                        case 0: // Script Code
                            break;

                        case 1: // Sound Code
                            break;

                        case 2: // Spawn Code
                        break;
                    }
                break;
            }
        }

        private static bool IsIgnored( string key )
        {
            foreach ( string ignoredObj in GenerateIgnore )
            {
                if ( ignoredObj == key ) return true;
            }

            return false;
        }

        internal static void ObjectCount()
        {
            GUILayout.BeginHorizontal();

                SetCorrectColor( EntityCount );
                GUILayout.Label( $" // Entity Count: {EntityCount} | {SetCorrectEntityLabel( EntityCount )}", EditorStyles.boldLabel );
                GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        internal static void OptionalOption()
        {
            GUILayout.BeginVertical("box");

                int idx = 0;
                GUILayout.BeginHorizontal();
                    foreach ( string key in GenerateObjectsFunction.Keys )
                    {
                        if ( IsIgnored( key ) ) continue;

                        ObjectType? type = Helper.GetObjectTypeByObjName( key );
                        if ( type != null && UnityInfo.GetSpecificObjectCount( ( ObjectType ) type ) == 0 ) continue;

                        GenerateObjects[key] = EditorGUILayout.Toggle( $"Build {key}", GenerateObjects[key], GUILayout.Width( paramToggleSize ) );

                        if ( GenerateObjects[key] != GenerateObjectsFunctionTemp[key] )
                        {
                            GenerateObjectsFunctionTemp[key] = GenerateObjects[key];
                            CodeViewsWindow.GenerateCorrectCode();
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

            foreach ( string key in GenerateObjectsFunctionTemp.Keys )
            {
                GenerateObjectsFunction[key] = GenerateObjectsFunctionTemp[key];
            }
        }

        private static void SetCorrectColor( int count )
        {
            if( count < greenPropCount )
                GUI.contentColor = Color.green;

            else if( ( count < yellowPropCount ) ) 
                GUI.contentColor = Color.yellow;

            else GUI.contentColor = Color.red;
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