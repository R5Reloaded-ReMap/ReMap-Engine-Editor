
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using Build;
using static Build.Build;
using WindowUtility;

namespace CodeViews
{
    internal enum AdditionalCodeType
    {
        Head,
        InBlock,
        Below
    }

    public class CodeViewsWindow : EditorWindow
    {
        internal static CodeViewsWindow windowInstance;
        internal static string code = "";
        internal static string[] codeSplit = new string[0];
        internal static string functionName = "Unnamed";
        internal static Vector2 scroll;
        internal static Vector2 scrollSettings;
        internal static int objectTypeInSceneCount = 0;
        internal static int objectTypeInSceneCount_temp = 0;
        internal static Vector3 StartingOffset = Vector3.zero;

        private static bool isAdditionalCodeWindow = false;

        private static string infoCount = "Entity Count";

        internal static string additionalCodeHead = "";
        internal static string additionalCodeInBlock = "";
        internal static string additionalCodeBelow = "";

        private static string[] fileInfo = new string[4];

        // Menus
        // Show / Hide Settings Menu
        internal static bool ShowSettingsMenu = false;

        // Menu Settings Names
        internal static string SquirrelMenu = "SquirrelMenu";
        internal static string SquirrelMenuShowFunction = "SquirrelMenuFunction";
        internal static string SquirrelMenuShowPrecacheCode = "SquirrelMenuPrecache";
        internal static string SquirrelMenuShowAdditionalCode = "SquirrelMenuInBlockAdditionalCode";

        internal static string OffsetMenu = "OffsetMenu";
        internal static string OffsetMenuOffset = "OffsetMenuOffset";
        internal static string OffsetMenuShowOffset = "OffsetMenuShowOffset";

        internal static string SelectionMenu = "SelectionMenu";

        internal static string LiveCodeMenu = "LiveCodeMenu";
        internal static string LiveCodeMenuRespawn = "LiveCodeMenuRespawn";
        internal static string LiveCodeMenuTeleportation = "LiveCodeMenuTeleportation";
        internal static string LiveCodeMenuAutoSend = "LiveCodeMenuAutoSend";
        internal static string LiveCodeMenuAdvanced = "LiveCodeMenuAdvanced";

        internal static string AdditionalCodeMenu = "AdditionalCodeMenu";

        internal static string AdvancedMenu = "AdvancedMenu";

        internal static string FullFileEntMenu = "FullFileEntMenu";
        internal static string FullFileEntSubMenu = "FullFileEntSubMenu";

        internal static string DevMenu = "DevMenu";
        internal static string DevMenuDebugInfo = "DevMenuDebugInfo";
        //

        internal static bool GenerationIsActive = false;

        public static bool SendingObjects = false;
        internal static int EntityCount = 0;
        internal static int SendedEntityCount = 0;
        internal static int EntFileID = 27;
        internal static Vector3 InfoPlayerStartOrigin = Vector3.zero;
        internal static Vector3 InfoPlayerStartAngles = Vector3.zero;

        internal static Texture2D enableLogo;
        internal static Texture2D disableLogo;

        public static int greenPropCount = 1500;
        public static int yellowPropCount = 3000;

        // Page utility
        internal static bool maxLength = false;
        internal static int maxBuildLine = 800;
        private static int itemStart = 0;
        private static int itemEnd = 0;
        private static int currentPage = 0;
        private static int maxPage = 0;

        private static WindowStruct windowStruct = new WindowStruct()
        {
            MainTab = new[] { "Squirrel Code", "DataTable Code", "Precache Code", "Ent Code", "Other Code" },

            SubTab = new Dictionary< int, string[] >()
            {
                { 0, new[] { "Script", "Additional Code" } },
                { 3, new[] { "script.ent", "snd.ent", "spawn.ent" } },
                { 4, new[] { "Camera Path" } }
            },

            SubTabGUI = new Dictionary< Tuple< int, int >, GUIStruct >()
            {
                // Squirrel Code
                {
                    // Script
                    WindowStruct.NewTuple( 0, 0 ),
                    new GUIStruct()
                    {
                        OnGUI = new FunctionRef[] { () => ScriptTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                ResetFunctionName();
                                infoCount = "Entity Count";
                                fileInfo = new[] { "Squirrel Code Export", "", $"{functionName}.nut", "nut" };
                                isAdditionalCodeWindow = false;
                            }

                            GenerateCorrectCode( () => ScriptTab.GenerateCode() );
                            AdditionalCodeTab.AdditionalCodeInit();
                        },

                        InitCallback = () =>
                        {
                            windowStruct.StoreInfo( "FuncNameBase", Helper.GetSceneName(), WindowStruct.NewTuple( 0, 0 ) );
                        }
                    }
                },

                {
                    // Additional Code
                    WindowStruct.NewTuple( 0, 1 ),
                    new GUIStruct()
                    {
                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                ResetFunctionName();
                                infoCount = "Entity Count";
                                fileInfo = new[] { "", "", "", "" };
                                isAdditionalCodeWindow = true;
                            }

                            AdditionalCodeTab.AdditionalCodeInit();
                        },

                        InitCallback = () =>
                        {
                            windowStruct.StoreInfo( "FuncNameBase", Helper.GetSceneName(), WindowStruct.NewTuple( 0, 1 ) );
                        }
                    }
                },

                // DataTable Code
                {
                    WindowStruct.NewTuple( 1, 0 ),
                    new GUIStruct()
                    {
                        OnGUI = new FunctionRef[] { () => DataTableTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                ResetFunctionName();
                                infoCount = "Entity Count";
                                fileInfo = new[] { "DataTable Code Export", "", $"{functionName}.csv", "csv" };
                                isAdditionalCodeWindow = false;
                            }

                            GenerateCorrectCode( () => DataTableTab.GenerateCode() );
                        },

                        InitCallback = () =>
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "remap_datatable", WindowStruct.NewTuple( 1, 0 ) );
                        }
                    }
                },

                // Precache Code
                {
                    WindowStruct.NewTuple( 2, 0 ),
                    new GUIStruct()
                    {
                        OnGUI = new FunctionRef[] { () => PrecacheTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                ResetFunctionName();
                                infoCount = "Models Precached Count";
                                fileInfo = new[] { "Precache Code Export", "", $"{functionName}.nut", "nut" };
                                isAdditionalCodeWindow = false;
                            }

                            GenerateCorrectCode( () => PrecacheTab.GenerateCode() );
                        },

                        InitCallback = () =>
                        {
                            windowStruct.StoreInfo( "FuncNameBase", $"{Helper.GetSceneName()}_Precache", WindowStruct.NewTuple( 2, 0 ) );
                        }
                    }
                },

                // Ent Code
                {
                    // Script Code
                    WindowStruct.NewTuple( 3, 0 ),
                    new GUIStruct()
                    {
                        OnGUI = new FunctionRef[] { () => ScriptEntTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                ResetFunctionName();
                                infoCount = "Entity Count";
                                fileInfo = new[] { "Ent Code Export", "", $"{functionName}.ent", "ent" };
                                isAdditionalCodeWindow = false;
                            }
                            
                            GenerateCorrectCode( () => ScriptEntTab.GenerateCode() );
                        },

                        InitCallback = () =>
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "mp_rr_remap_script", WindowStruct.NewTuple( 3, 0 ) );
                        }
                    }
                },
                
                {
                    // Sound Code
                    WindowStruct.NewTuple( 3, 1 ),
                    new GUIStruct()
                    {
                        OnGUI = new FunctionRef[] { () => SoundEntTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                ResetFunctionName();
                                infoCount = "Entity Count";
                                fileInfo = new[] { "Ent Code Export", "", $"{functionName}.ent", "ent" };
                                isAdditionalCodeWindow = false;
                            }

                            GenerateCorrectCode( () => SoundEntTab.GenerateCode() );
                        },

                        InitCallback = () =>
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "mp_rr_remap_snd", WindowStruct.NewTuple( 3, 1 ) );
                        }
                    }
                },

                {
                    // Spawn Code
                    WindowStruct.NewTuple( 3, 2 ),
                    new GUIStruct()
                    {
                        OnGUI = new FunctionRef[] { () => SpawnEntTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                ResetFunctionName();
                                infoCount = "Entity Count";
                                fileInfo = new[] { "Ent Code Export", "", $"{functionName}.ent", "ent" };
                                isAdditionalCodeWindow = false;
                            }

                            GenerateCorrectCode( () => SpawnEntTab.GenerateCode() );
                        },

                        InitCallback = () =>
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "mp_rr_remap_spawn", WindowStruct.NewTuple( 3, 2 ) );
                        }
                    }
                },

                // Other Code
                {
                    // Camera Path Code
                    WindowStruct.NewTuple( 4, 0 ),
                    new GUIStruct()
                    {
                        OnGUI = new FunctionRef[] { () => CameraPathTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                ResetFunctionName();
                                infoCount = "Paths Count";
                                fileInfo = new[] { "Camera Path Code Export", "", $"{functionName}.nut", "nut" };
                                isAdditionalCodeWindow = false;
                            }

                            GenerateCorrectCode( () => CameraPathTab.GenerateCode() );
                        },

                        InitCallback = () =>
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "remap_camera_path", WindowStruct.NewTuple( 4, 0 ) );
                        }
                    }
                },
            },

            MainTabCallback = () =>
            {
                WindowUtility.WindowUtility.CreateButton( "Refresh", "", () => Refresh( false ), 100 );
            },

            PostRefreshCallback = () =>
            {
                windowStruct.StoreInfo( "FuncName", functionName );
            },

            RefreshCallback = () =>
            {
                windowStruct.ReStoreInfo( ref functionName, "FuncName" );
                Refresh( false );
            }

        };
        

        [ MenuItem( "ReMap/Code Views", false, 25 ) ]
        public static void Init()
        {
            TagHelper.CheckAndCreateTags();

            windowInstance = ( CodeViewsWindow ) GetWindow( typeof( CodeViewsWindow ), false, "Code Views" );
            windowInstance.minSize = new Vector2( 1230, 500 );
            windowInstance.Show();

            Helper.SetShowStartingOffset( true );

            windowStruct.Awake();
        }

        void OnEnable()
        {
            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

            enableLogo = Resources.Load( "icons/codeViewEnable" ) as Texture2D;
            disableLogo = Resources.Load( "icons/codeViewDisable" ) as Texture2D;
            
            windowStruct.Awake();
        }

        void OnGUI()
        {
            MainTab();

            ShortCut();

            if ( isAdditionalCodeWindow )
            {
                GUILayout.BeginVertical( "box" );
                    scroll = EditorGUILayout.BeginScrollView( scroll );
                        if ( AdditionalCodeTab.showInfo )
                        {
                            AdditionalCodeTab.textInfoTemp = AdditionalCodeTab.textInfo;
                            EditorGUILayout.TextArea( AdditionalCodeTab.textInfoTemp, GUILayout.ExpandHeight( true ) );
                        }
                        else if ( !AdditionalCodeTab.isEmptyCode )
                        {
                            AdditionalCodeTab.activeCode.Content[ AdditionalCodeTab.windowStruct.SubTabIdx ].Code = EditorGUILayout.TextArea( AdditionalCodeTab.activeCode.Content[ AdditionalCodeTab.windowStruct.SubTabIdx ].Code, GUILayout.ExpandHeight( true ) );
                        }
                        else
                        {
                            WindowUtility.WindowUtility.FlexibleSpace();
                                WindowUtility.WindowUtility.CreateTextInfoCentered( "Empty Code Reference" );
                            WindowUtility.WindowUtility.FlexibleSpace();
                        }
                    EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginVertical( "box" );
                    GUILayout.BeginHorizontal();

                        CodeOutput();
                    
                        if ( ShowSettingsMenu )
                        {
                            GUILayout.BeginVertical( "box", GUILayout.Width( 340 ) );
                                windowStruct.ShowFunc();
                            GUILayout.EndVertical();
                        }

                    GUILayout.EndHorizontal();
        
                    if ( GUILayout.Button( "Copy To Clipboard" ) ) CopyCode();
                GUILayout.EndVertical();
            }
        }

        public static bool IsHided( ObjectType objectType )
        {
            // Ensure the objectData is not empty
            GameObject[] objectData = Helper.GetAllObjectTypeWithEnum( objectType, CodeViewsWindow.SelectionEnable() );

            if ( objectData.Length == 0 ) return true;

            // Check if objectType are flaged hide
            foreach ( ObjectType hidedObjectType in Helper.GenerateIgnore )
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
        internal static void Refresh( bool reSetScroll = true )
        {
            if ( reSetScroll ) SetScrollView( scroll );
            if ( itemEnd > codeSplit.Length ) currentPage = codeSplit.Length / maxBuildLine;

            GUI.FocusControl( null );

            windowStruct.SubTabGUI[ windowStruct.GetCurrentTabIdx() ].OnStartGUI();
        }

        internal static void CopyCode()
        {
            GUIUtility.systemCopyBuffer = code;
        }

        internal static async void SetScrollView( Vector2 scroll )
        {
            // Hack: As long as the code generation is not finished, then do not continue the script
            while ( GenerationIsActive ) await Task.Delay( 100 ); // 100 ms

            CodeViewsWindow.scroll = scroll;
        }

        internal static async void ExportFunction()
        {
            Helper.FixPropTags();

            EditorSceneManager.SaveOpenScenes();

            Refresh();

            // Hack: As long as the code generation is not finished, then do not continue the script
            while ( GenerationIsActive ) await Task.Delay( 100 ); // 100 ms

            var path = EditorUtility.SaveFilePanel( fileInfo[0], fileInfo[1], fileInfo[2], fileInfo[3] );

            if ( path.Length == 0 ) return;

            File.WriteAllText( path, code );
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

                windowStruct.ShowTab();

                GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                if ( isAdditionalCodeWindow )
                {
                    AdditionalCodeTab.windowStruct.ShowTab();

                    AdditionalCodeTab.MainTab();

                    GUILayout.EndVertical();
                    return;
                }

                GUILayout.BeginHorizontal();
                        ObjectCount();

                        if ( MenuInit.IsEnable( CodeViewsWindow.DevMenuDebugInfo ) )
                        {
                            CodeViewsMenu.Space( 10 );
                            WindowUtility.WindowUtility.GetEditorWindowSize( windowInstance );
                            WindowUtility.WindowUtility.GetScrollSize( scroll );
                        }

                        WindowUtility.WindowUtility.FlexibleSpace();

                        ExportButton();
                        SettingsMenuButton();
                GUILayout.EndHorizontal();

                if ( maxLength && !GenerationIsActive )
                {
                    itemStart = currentPage * maxBuildLine;
                    itemEnd = itemStart + maxBuildLine;
                    int end = itemEnd > codeSplit.Length ? codeSplit.Length : itemEnd;

                    GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                    GUILayout.BeginHorizontal();
                        GUILayout.Label( $" // Page {currentPage + 1} / {maxPage} ( {itemStart} - {end} lines )", EditorStyles.boldLabel, GUILayout.Width( 400 ) );

                        WindowUtility.WindowUtility.FlexibleSpace();

                        WindowUtility.WindowUtility.CreateButton( "Previous Page", "", () => { if ( currentPage > 0 ) currentPage--; }, 100 );

                        WindowUtility.WindowUtility.CreateButton( "Next Page", "", () => { if ( itemEnd < codeSplit.Length ) currentPage++; }, 100 );
                    GUILayout.EndHorizontal();
                }

            GUILayout.EndVertical();
        }

        private static void ObjectCount()
        {
            GUILayout.BeginHorizontal();

                if ( GenerationIsActive )
                {
                    GUILayout.Label( $" // Generating code...", EditorStyles.boldLabel );
                }
                else
                {
                    SetCorrectColor( EntityCount );
                    GUILayout.Label( $" // {infoCount}: {EntityCount} | {SetCorrectEntityLabel( EntityCount )}", EditorStyles.boldLabel );
                    GUI.contentColor = Color.white;
                }

                if ( SendingObjects )
                {
                    if ( LiveMap.ApexProcessIsActive() )
                    {
                        GUILayout.Label( $"|| Sending {SendedEntityCount} Objects to the game", EditorStyles.boldLabel );
                    }
                    else GUILayout.Label( $"|| Game not found !", EditorStyles.boldLabel );
                }

            GUILayout.EndHorizontal();
        }

        private static void ExportButton( string text = "Export Code", string tooltip = "Export current code" )
        {
            if ( GUILayout.Button( new GUIContent( text, tooltip ), GUILayout.Width( 100 ) ) ) ExportFunction();
        }

        private static void SettingsMenuButton( string text = "Settings", string tooltip = "Show settings" )
        {
            if ( GUILayout.Button( new GUIContent( text, tooltip ), GUILayout.Width( 100 ) ) ) ShowSettingsMenu = !ShowSettingsMenu;
        }

        private static void CodeOutput()
        {
            GUIStyle style = new GUIStyle( GUI.skin.textArea );
            style.fontSize = 12;
            style.wordWrap = false;

            scroll = EditorGUILayout.BeginScrollView( scroll );
                if ( maxLength )
                {
                    List< string > list = new List< string >();
                    for ( int i = itemStart; i < itemEnd && i < codeSplit.Length; i++ )
                    {
                        list.Add( codeSplit[ i ] );
                    }
                    GUILayout.TextArea( $"\n{string.Join( "\n", list ).ToString()}\n", style, GUILayout.ExpandHeight( true ) );
                }
                else
                {
                    GUILayout.TextArea( code, style, GUILayout.ExpandHeight( true ) );
                }
            EditorGUILayout.EndScrollView();
        }


        //  ██╗   ██╗██╗     ██╗   ██╗████████╗██╗██╗     ██╗████████╗██╗   ██╗
        //  ██║   ██║██║     ██║   ██║╚══██╔══╝██║██║     ██║╚══██╔══╝╚██╗ ██╔╝
        //  ██║   ██║██║     ██║   ██║   ██║   ██║██║     ██║   ██║    ╚████╔╝ 
        //  ██║   ██║██║     ██║   ██║   ██║   ██║██║     ██║   ██║     ╚██╔╝  
        //  ╚██████╔╝██║     ╚██████╔╝   ██║   ██║███████╗██║   ██║      ██║   
        //   ╚═════╝ ╚═╝      ╚═════╝    ╚═╝   ╚═╝╚══════╝╚═╝   ╚═╝      ╚═╝   

        internal static void ShortCut()
        {
            Event currentEvent = Event.current;

            if ( currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.R ) Refresh();
        }

        internal static void ResetFunctionName()
        {
            windowStruct.ReStoreInfo( ref functionName, "FuncNameBase" );
        }

        private static async void GenerateCorrectCode( FunctionRefAsyncString functionRef )
        {
            EntityCount = 0;

            code = "";

            functionName = Helper.ReplaceBadCharacters( functionName );

            GenerationIsActive = true;

            code += await functionRef();

            maxLength = code.Split( "\n" ).Length > maxBuildLine;
            codeSplit = code.Split( "\n" );
            maxPage = codeSplit.Length == 0 ? 1 : codeSplit.Length / maxBuildLine + 1;

            CodeViewsMenu.VerifyGenerateObjects();

            GenerationIsActive = false;
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

            else if( count < yellowPropCount ) 
                GUI.contentColor = Color.yellow;

            else GUI.contentColor = Color.red;
        }

        internal static void AppendAdditionalCode( AdditionalCodeType type, ref StringBuilder code, string codeToAdd, bool verify = true )
        {
            if ( !verify || string.IsNullOrEmpty( codeToAdd ) ) return;

            string codeToAdd_ = codeToAdd;

            Helper.ReplaceLocalizedString( ref codeToAdd_ );

            codeToAdd = codeToAdd_;
            
            switch ( type )
            {
                case AdditionalCodeType.Head:
                    Build.Build.AppendCode( ref code, codeToAdd, 0 );
                    break;

                case AdditionalCodeType.InBlock:
                    codeToAdd = "    " + codeToAdd.Replace( "\n", "\n    " );

                    Build.Build.AppendCode( ref code, codeToAdd, 0 );
                    break;

                case AdditionalCodeType.Below:
                    Build.Build.AppendCode( ref code );
                    
                    Build.Build.AppendCode( ref code, codeToAdd, 0 );
                break;
            }

            Build.Build.AppendCode( ref code );
        }


        internal static bool ShowFunctionEnable()
        {
            return MenuInit.IsEnable( SquirrelMenuShowFunction );
        }

        internal static bool ShowPrecacheEnable()
        {
            return MenuInit.IsEnable( SquirrelMenuShowPrecacheCode );
        }

        internal static bool AdditionalCodeEnable()
        {
            return MenuInit.IsEnable( SquirrelMenuShowAdditionalCode );
        }

        internal static bool SelectionEnable()
        {
            return MenuInit.IsEnable( SelectionMenu );
        }

        internal static bool AutoSendEnable()
        {
            return MenuInit.IsEnable( LiveCodeMenuAutoSend );
        }
    }
}