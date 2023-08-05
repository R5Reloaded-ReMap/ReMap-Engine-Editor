
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        internal static List< InfoMessage > infoList = InfoMessage.CreateInfoSerialized( 2 );

        // Menus
        // Show / Hide Settings Menu
        internal static bool ShowSettingsMenu = false;

        // Menu Settings Names
        internal static readonly string SquirrelMenu = "SquirrelMenu";
        internal static readonly string SquirrelMenuShowFunction = "SquirrelMenuFunction";
        internal static readonly string SquirrelMenuShowPrecacheCode = "SquirrelMenuPrecache";
        internal static readonly string SquirrelMenuShowAdditionalCode = "SquirrelMenuInBlockAdditionalCode";

        internal static readonly string OffsetMenu = "OffsetMenu";
        internal static readonly string OffsetMenuOffset = "OffsetMenuOffset";
        internal static readonly string OffsetMenuShowOffset = "OffsetMenuShowOffset";

        internal static readonly string SelectionMenu = "SelectionMenu";

        internal static readonly string LiveCodeMenu = "LiveCodeMenu";
        internal static readonly string LiveCodeMenuGetApexInfo = "LiveCodeMenuGetApexInfo";
        internal static readonly string LiveCodeMenuRespawn = "LiveCodeMenuRespawn";
        internal static readonly string LiveCodeMenuTeleportation = "LiveCodeMenuTeleportation";
        internal static readonly string LiveCodeMenuAutoSend = "LiveCodeMenuAutoSend";
        internal static readonly string LiveCodeMenuAdvanced = "LiveCodeMenuAdvanced";

        internal static readonly string AdditionalCodeMenu = "AdditionalCodeMenu";

        internal static readonly string AdvancedMenu = "AdvancedMenu";

        internal static readonly string FullFileEntMenu = "FullFileEntMenu";
        internal static readonly string FullFileEntSubMenu = "FullFileEntSubMenu";

        internal static readonly string TipsMenu = "";

        internal static readonly string DevMenu = "DevMenu";
        internal static readonly string DevMenuDebugInfo = "DevMenuDebugInfo";
        //

        internal static Task GenerationIsActive = Task.CompletedTask;
        internal static Task SendingObjects = Task.CompletedTask;

        internal static int EntityCount = 0;
        internal static int SendedEntityCount = 0;
        internal static int NotExitingModel = 0;
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

        // Empty ObjectType[]
        internal static readonly ObjectType[] EmptyObjectType = new ObjectType[0];

        private static WindowStruct windowStruct = new WindowStruct()
        {
            MainTab = new[] { "Squirrel Code", "DataTable Code", "Precache Code", "Ent Code", "Other Code" },

            SubTab = new Dictionary< int, string[] >()
            {
                { 0, new[] { "Server Script", "Client Script", "Additional Code" } },
                { 3, new[] { "script.ent", "snd.ent", "spawn.ent" } },
                { 4, new[] { "Camera Path" } }
            },

            SubTabGUI = new Dictionary< ( int, int ), GUIStruct >()
            {
                // Squirrel Code
                {
                    // Server Script
                    ( 0, 0 ),
                    new GUIStruct()
                    {
                        Name = "Squirrel Code (Server)",

                        OnGUI = new FunctionRef[] { () => ScriptTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                fileInfo = new[] { "Squirrel Server Code Export", "", $"{functionName}.nut", "nut" };
                            }

                            AdditionalCodeTab.AdditionalCodeInit();

                            GenerationIsActive = GenerateCorrectCode( () => ScriptTab.GenerateCode() );
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", Helper.GetSceneName() );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                            windowStruct.ReStoreInfo( ref functionName, "FuncName" );
                        }
                    }
                },

                {
                    // Client Script
                    ( 0, 1 ),
                    new GUIStruct()
                    {
                        Name = "Squirrel Code (Client)",

                        OnGUI = new FunctionRef[] { () => ScriptClientTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                fileInfo = new[] { "Squirrel Client Code Export", "", $"CL_{functionName}.nut", "nut" };
                            }

                            GenerationIsActive = GenerateCorrectCode( () => ScriptClientTab.GenerateCode() );
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", $"CL_{Helper.GetSceneName()}" );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                        }
                    }
                },

                {
                    // Additional Code
                    ( 0, 2 ),
                    new GUIStruct()
                    {
                        Name = "Additional Code",

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                fileInfo = new[] { "", "", "", "" };
                                isAdditionalCodeWindow = true;
                            }

                            AdditionalCodeTab.AdditionalCodeInit();
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", Helper.GetSceneName() );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                        }
                    }
                },

                // DataTable Code
                {
                    ( 1, 0 ),
                    new GUIStruct()
                    {
                        Name = "DataTable Code",

                        OnGUI = new FunctionRef[] { () => DataTableTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                fileInfo = new[] { "DataTable Code Export", "", $"{functionName}.csv", "csv" };
                            }

                            GenerationIsActive = GenerateCorrectCode( () => DataTableTab.GenerateCode() );
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "remap_datatable" );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                        }
                    }
                },

                // Precache Code
                {
                    ( 2, 0 ),
                    new GUIStruct()
                    {
                        Name = "Precache Code",

                        OnGUI = new FunctionRef[] { () => PrecacheTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                infoCount = "Models Precached Count";
                                fileInfo = new[] { "Precache Code Export", "", $"{functionName}.nut", "nut" };
                            }

                            GenerationIsActive = GenerateCorrectCode( () => PrecacheTab.GenerateCode() );
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", $"{Helper.GetSceneName()}_Precache" );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                        }
                    }
                },

                // Ent Code
                {
                    // Script Code
                    ( 3, 0 ),
                    new GUIStruct()
                    {
                        Name = "Script Ent Code",

                        OnGUI = new FunctionRef[] { () => ScriptEntTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                fileInfo = new[] { "Ent Code Export", "", $"{functionName}.ent", "ent" };
                            }

                            GenerationIsActive = GenerateCorrectCode( () => ScriptEntTab.GenerateCode() );
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "mp_rr_remap_script" );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                        }
                    }
                },
                
                {
                    // Sound Code
                    ( 3, 1 ),
                    new GUIStruct()
                    {
                        Name = "Sound Ent Code",

                        OnGUI = new FunctionRef[] { () => SoundEntTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                fileInfo = new[] { "Ent Code Export", "", $"{functionName}.ent", "ent" };
                            }

                            GenerationIsActive = GenerateCorrectCode( () => SoundEntTab.GenerateCode() );
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "mp_rr_remap_snd" );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                        }
                    }
                },

                {
                    // Spawn Code
                    ( 3, 2 ),
                    new GUIStruct()
                    {
                        Name = "Spawn Ent Code",

                        OnGUI = new FunctionRef[] { () => SpawnEntTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                fileInfo = new[] { "Ent Code Export", "", $"{functionName}.ent", "ent" };
                            }

                            GenerationIsActive = GenerateCorrectCode( () => CameraPathTab.GenerateCode() );
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "mp_rr_remap_spawn" );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                        }
                    }
                },

                // Other Code
                {
                    // Camera Path Code
                    ( 4, 0 ),
                    new GUIStruct()
                    {
                        Name = "Camera Path Code",

                        OnGUI = new FunctionRef[] { () => CameraPathTab.OnGUISettingsTab() },

                        OnStartGUI = () =>
                        {
                            if ( windowStruct.OnWindowChange() ) // Execute scope if script changes page
                            {
                                infoCount = "Paths Count";
                                fileInfo = new[] { "Camera Path Code Export", "", $"{functionName}.nut", "nut" };
                            }

                            GenerationIsActive = GenerateCorrectCode( () => CameraPathTab.GenerateCode() );
                        },

                        InitCallback = () => // Load on start
                        {
                            windowStruct.StoreInfo( "FuncNameBase", "remap_camera_path" );
                            windowStruct.StoreInfo( "FuncName", windowStruct.GetStoredInfo< string >( "FuncNameBase" ) );
                        }
                    }
                },
            },

            MainTabCallback = () =>
            {
                WindowUtility.WindowUtility.CreateButton( "Refresh", "", () => Refresh( false ), 100 );
            },

            // PostRefresh is executed first, then OnStartGUI of the window, then Refresh.
            PostRefreshCallback = () =>
            {
                windowStruct.StoreInfo( "FuncName", functionName );
                infoCount = "Entity Count";
                isAdditionalCodeWindow = false;
            },

            InitCallback = () => // Load on start
            {
                windowStruct.PostRefreshCallback();
                #if RMAPDEV
                    ShowSettingsMenu = true;
                #endif
            },

            RefreshCallback = () =>
            {
                windowStruct.ReStoreInfo( ref functionName, "FuncName" );
                Refresh( false );
            }

        };
        
        public static void Init()
        {
            windowInstance = ( CodeViewsWindow ) GetWindow( typeof( CodeViewsWindow ), false, "Code Views" );
            windowInstance.minSize = new Vector2( 1230, 500 );
            windowInstance.Show();
        }

        void OnEnable()
        {
            TagHelper.CheckAndCreateTags();

            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

            enableLogo = Resources.Load( "icons/codeViewEnable" ) as Texture2D;
            disableLogo = Resources.Load( "icons/codeViewDisable" ) as Texture2D;

            Helper.SetShowStartingOffset( true );

            if ( !Helper.IsValid( windowInstance ) ) windowInstance = ( CodeViewsWindow ) GetWindow( typeof( CodeViewsWindow ), false, "Code Views" );
            
            windowStruct.Awake();
        }

        void OnGUI()
        {
            MainTab();

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
                ShortCut();
                
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

        public static bool IsHidden( ObjectType objectType )
        {
            // Check if objectType are flaged hide
            if ( Helper.GenerateIgnore.Any( o => o == objectType ) )
                return true;

            // Ensure the objectData is not empty
            if ( !Helper.IsObjectTypeExistInScene( objectType ) )
                return true;
            
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

            GUI.FocusControl( null );

            windowStruct.OnStartGUICallback();

            CodeViewsSearchWindow.Refresh();
        }

        internal static void CopyCode()
        {
            GUIUtility.systemCopyBuffer = code;
        }

        internal static void GoToPage( int idx )
        {
            if ( idx < 0 || idx >= codeSplit.Length )
            {
                Debug.LogWarning("Index out of range"); return;
            }

            // Determine the page where the index is located
            int targetPage = idx / maxBuildLine;

            // Set the current page to the target page
            currentPage = targetPage;

            // Update the start and end items
            itemStart = currentPage * maxBuildLine;
            itemEnd = itemStart + maxBuildLine;
            if ( itemEnd > codeSplit.Length ) 
            {
                itemEnd = codeSplit.Length;
            }

            // Estimate scroll position, +60f every 4 lines
            // Modulo idx to keep the index below maxBuildLine
            scroll.y = ( float ) 60 * ( idx % maxBuildLine / 4 );

            HighlightString( currentPage, idx );

            windowInstance.Focus();
        }

        private static async void HighlightString( int page, int idx )
        {
            float time = 0.0f; CodeViewsSearchWindow.SearchedString = idx; bool verify = true;

            while ( true )
            {
                verify = ( page == currentPage && CodeViewsSearchWindow.SearchedString == idx );

                if ( !verify || time >= 4.0f ) break;

                await Task.Delay( TimeSpan.FromSeconds( 0.1 ) );

                time += 0.1f;
            }

            if ( verify )
            {
                CodeViewsSearchWindow.SearchedString = -1;
            }
        }

        internal static async void SetScrollView( Vector2 scroll )
        {
            while ( !GenerationIsActive.IsCompleted ) await Helper.Wait( 1 ); // 1 secondes

            CodeViewsWindow.scroll = scroll;
        }

        internal static async void ExportFunction()
        {
            Helper.FixPropTags();

            EditorSceneManager.SaveOpenScenes();

            Refresh();
            
            await Helper.Wait( 1 ); // 1 secondes

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
            windowStruct.Awake();
        }

        private void EditorSceneManager_sceneOpened( UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode )
        {
            windowStruct.Awake();
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

                if ( maxLength && GenerationIsActive.IsCompleted )
                {
                    itemStart = currentPage * maxBuildLine;
                    itemEnd = itemStart + maxBuildLine;
                    int end = itemEnd > codeSplit.Length ? codeSplit.Length : itemEnd;

                    GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                    GUILayout.BeginHorizontal();
                        WindowUtility.WindowUtility.ShowPageInfo( currentPage, maxPage, itemStart, end, " // Page", "line" );

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

                infoList[ 0 ].ShowMessage();

                if ( infoList[ 1 ].HasMessage() && infoList[ 0 ].HasMessage() )
                {
                    WindowUtility.WindowUtility.Separator( 2, 12 );
                    infoList[ 1 ].ShowMessage();
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
            GUIStyle style = new GUIStyle( GUI.skin.textArea )
            {
                fontSize = 12,
                wordWrap = false,
                richText = true
            };

            scroll = EditorGUILayout.BeginScrollView( scroll );

            var startIndex = maxLength ? itemStart : 0;
            var endIndex = maxLength ? Math.Min( itemEnd, codeSplit.Length ) : codeSplit.Length;
            var lines = codeSplit.Skip( startIndex ).Take( endIndex - startIndex ).Select
            (
                ( line, index ) => index == CodeViewsSearchWindow.SearchedString ? $"<color=green>{line}</color>" : line
            );

            var originalText = string.Join( "\n", lines );
            var resultText = EditorGUILayout.TextArea( originalText, style, GUILayout.ExpandHeight( true ) );

            // Reset Text
            if ( resultText != originalText )
            {
                GUI.FocusControl( null );
                originalText = resultText;
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

            if ( currentEvent.type == EventType.KeyDown )
            {
                if ( currentEvent.control ) // Ctrl key is pressed
                {
                    switch ( currentEvent.keyCode )
                    {
                        case KeyCode.R:
                            Refresh(); // Press Ctrl + R for Refresh the page
                            break;

                        case KeyCode.F:
                            CodeViewsSearchWindow.Init(); // Press Ctrl + F for Search some code
                        break;
                    }
                }
            }
        }

        internal static void ResetFunctionName()
        {
            windowStruct.ReStoreInfo( ref functionName, "FuncNameBase" );
        }

        private static async Task GenerateCorrectCode( FunctionRefAsyncString functionRef )
        {
            EditorUtility.DisplayProgressBar( $"Generating Code", $"Getting objects in scene...", 0.0f );
                Helper.GetObjectsInScene();
            EditorUtility.ClearProgressBar();

            EntityCount = 0; code = "";

            NotExitingModel = 0;

            LibrarySorter.RpakManagerWindow.libraryData = LibrarySorter.RpakManagerWindow.FindLibraryDataFile();

            functionName = Helper.ReplaceBadCharacters( functionName );

            infoList[ 0 ].SetInfoMessage( $" // Generating code...", true );

            CodeViewsMenu.VerifyGenerateObjects();

            EditorUtility.DisplayProgressBar( $"Generating Code", $"Processing...", 0.0f );
                code += await functionRef();
            EditorUtility.ClearProgressBar();

            maxLength = code.Split( "\n" ).Length > maxBuildLine;
            codeSplit = code.Split( "\n" );
            maxPage = codeSplit.Length == 0 ? 1 : codeSplit.Length / maxBuildLine + 1;

            if ( currentPage + 1 > maxPage ) currentPage = maxPage - 1;

            infoList[ 0 ].SetInfoMessage( $" // {infoCount}: {EntityCount} | {SetCorrectEntityLabel( EntityCount )}", true, SetCorrectColor( EntityCount ) );

            if ( NotExitingModel != 0 )
            {
                infoList[ 1 ].RemoveToQueue( "NotExitingModelMessage" );
                infoList[ 1 ].AddToQueueMessage( "NotExitingModelMessage", $"{NotExitingModel} Models don't exist and are not generated", 6, true, new Color( 254, 156, 84 ) );
            }
        }

        private static string SetCorrectEntityLabel( int count )
        {
            if ( count < greenPropCount )
                return "Status: Safe";

            else if ( ( count < yellowPropCount ) ) 
                return "Status: Safe";

            else return "Status: Warning! Game could crash!";
        }

        private static Color SetCorrectColor( int count )
        {
            if ( count < greenPropCount )
                return Color.green;

            else if ( count < yellowPropCount ) 
                return Color.yellow;

            else return Color.red;
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

    internal class InfoMessage
    {
         public static readonly int simultaneousDisplay = 2;

        public string name = "InfoMessage";

        public string message = "";
        public bool bold = false;
        public int duration = 0;
        public Color color = Color.white;

        public DateTime showDuration = DateTime.Now;

        private List< InfoMessage > queue = new List< InfoMessage >();

        public static List< InfoMessage > CreateInfoSerialized( int num )
        {
            List< InfoMessage > list = new List< InfoMessage >();

            for ( int i = 0; i < num; i++ )
            {
                list.Add( new InfoMessage() );
            }

            return list;
        }

        public void SetInfoMessage( string msg, bool bold = false, Color color = default )
        {
            this.message = msg;
            this.bold = bold;
            this.color = color;
        }

        public void AddToQueueMessage( string name, string msg, int duration, bool bold = false, Color color = default )
        {
            if ( this.Exists( name ) )
                return;

            InfoMessage infoMessage = new InfoMessage();
            infoMessage.name = name;
            infoMessage.message = msg;
            infoMessage.bold = bold;
            infoMessage.duration = duration;
            infoMessage.color = color;

            if ( this.QueueIsEmpty() )
            {
                this.message = infoMessage.message;
                infoMessage.showDuration = DateTime.Now.AddSeconds( duration );
            }

            this.queue.Add( infoMessage );
        }

        public void RemoveToQueue( string name )
        {
            for ( int i = 0; i < this.queue.Count; i++ )
            {
                if ( this.queue[ i ].name == name )
                {
                    this.queue.RemoveAt( i );
                }
            }
        }

        public void ShowMessage()
        {
            if ( this.QueueIsEmpty() && this.HasMessage() )
            {
                InternalShowMessage( this );
            }
            else if ( !this.QueueIsEmpty() )
            {
                if ( this.QueueCount() >= 1 && DateTime.Now > this.queue[0].showDuration )
                {
                    this.queue.RemoveAt( 0 );
                    this.MessageClear();
        
                    if ( this.QueueIsEmpty() )
                        return;

                    this.message = this.queue[0].message;
                    this.queue[0].showDuration = DateTime.Now.AddSeconds( this.queue[0].duration );
                }
            
                InternalShowMessage( this.queue[0] );
            }
        }

        private void InternalShowMessage( InfoMessage infoMessage )
        {
            GUI.contentColor = infoMessage.color;

            GUILayout.Label( infoMessage.message, infoMessage.bold ? EditorStyles.boldLabel : GUI.skin.label );

            GUI.contentColor = Color.white;
        }

        public bool HasMessage()
        {
            return !string.IsNullOrEmpty( this.message );
        }

        public void MessageClear()
        {
            this.message = "";
        }

        public bool QueueIsEmpty()
        {
            return this.queue.Count == 0;
        }

        public int QueueCount()
        {
            return this.queue.Count;
        }

        public bool Exists( string name )
        {
            return this.queue.Any( m => m.name == name );
        }
    }
}