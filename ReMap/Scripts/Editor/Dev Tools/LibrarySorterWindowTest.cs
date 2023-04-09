using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    internal enum TaskType
    {
        FixPrefabsTags,
        FixPrefabsLabels,
        FixPrefabsData
    }

    public class LibrarySorterWindowTest : EditorWindow
    {
        internal static LibraryData libraryData;
        Vector2 scrollPos = Vector2.zero;
        Vector2 scrollPosFixPrefabs = Vector2.zero;

        static bool fixFolders = true;
        
        #if ReMapDev
            [ MenuItem( "ReMap Dev Tools/Prefabs Management/Windows/Prefab Fix Manager Test", false, 100 ) ]
            public static void Init()
            {
                libraryData = RpakManagerWindow.FindLibraryDataFile();

                LibrarySorterWindowTest window = ( LibrarySorterWindowTest )GetWindow( typeof( LibrarySorterWindowTest ), false, "Prefab Fix Manager" );
                window.minSize = new Vector2( 650, 600 );
                window.Show();
            }
        #endif

        private void OnEnable()
        {
            libraryData = RpakManagerWindow.FindLibraryDataFile();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal();

                    WindowUtility.WindowUtility.CreateButton( "Rpak Manager", "", () => RpakManagerWindow.Init() );
                    WindowUtility.WindowUtility.CreateButton( "Fix Prefabs Tags", "", () => AwaitTask( TaskType.FixPrefabsTags ) );
                    WindowUtility.WindowUtility.CreateButton( "Check Prefabs Labels", "", () => AwaitTask( TaskType.FixPrefabsLabels ) );

                GUILayout.EndHorizontal();

                GUILayout.Space( 4 );

                scrollPos = EditorGUILayout.BeginScrollView( scrollPos );
                    fixFolders = EditorGUILayout.BeginFoldoutHeaderGroup( fixFolders, "Fix Folders" );
                    if ( fixFolders )
                    {
                        scrollPosFixPrefabs = EditorGUILayout.BeginScrollView( scrollPosFixPrefabs );
                            foreach ( RpakData data in libraryData.RpakList )
                            {
                                GUILayout.BeginHorizontal();
                                    WindowUtility.WindowUtility.CreateButton( $"{data.Name}", "", () => AwaitTask( TaskType.FixPrefabsData, null, data ) );
                                    WindowUtility.WindowUtility.CreateButton( $"Find Missing", "", null, 140 );
                                    GUILayout.Label( $"Lastest Ckeck: {data.Update}", GUILayout.Width( 216 ) );
                                GUILayout.EndHorizontal();
                            }
                        GUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        internal static async void AwaitTask( TaskType taskType, string arg = null, RpakData data = null )
        {
            if ( !CheckDialog( "Library Sorter", "Are you sure to start this task ?" ) ) return;

            switch ( taskType )
            {
                case TaskType.FixPrefabsTags:
                    await FixPrefabsTags();
                    break;
                case TaskType.FixPrefabsLabels:
                    await SetModelLabels( arg );
                    break;
                case TaskType.FixPrefabsData:
                    await SortFolder( data );
                break;
            }
        }

        public static Task SortFolder( RpakData data )
        {
            string rpakPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}";

            if (!Directory.Exists( rpakPath ))
            {
                ReMapConsole.Log( $"[Library Sorter] Creating directory: {UnityInfo.relativePathPrefabs}/{data.Name}", ReMapConsole.LogType.Info );
                Directory.CreateDirectory( rpakPath );
            }
            /*
            //////////////////////////

            int datalen = data.Data.Length;

            string modelReplacePath;
            GameObject prefabToAdd; GameObject prefabInstance;
            GameObject objectToAdd; GameObject objectInstance;

            string json = System.IO.File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}" );
            List< PrefabOffset > offsets = JsonUtility.FromJson< PrefabOffsetList >( json ).List;

            int i = 0;
            foreach ( string model in data.Data )
            {
                model = Path.GetFileNameWithoutExtension(modelPath);

                if (File.Exists($"{currentDirectory}/{relativeModel}/{model + "_LOD0.fbx"}"))
                {
                    modelReplacePath = modelPath.Replace("/", "#").Replace(".rmdl", ".prefab");

                    EditorUtility.DisplayProgressBar("Sorting Files", $"Sorting: {mapName}/{modelReplacePath}", (i + 1) / (float)arraylen);

                    if (!File.Exists($"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}"))
                    {
                        prefabToAdd = AssetDatabase.LoadAssetAtPath($"{relativeEmptyPrefab}", typeof(UnityEngine.Object)) as GameObject;
                        objectToAdd = AssetDatabase.LoadAssetAtPath($"{relativeModel}/{model + "_LOD0.fbx"}", typeof(UnityEngine.Object)) as GameObject;

                        if (prefabToAdd == null || objectToAdd == null)
                        {
                            ReMapConsole.Log($"[Library Sorter] Error loading prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                            continue;
                        }

                        prefabInstance = UnityEngine.Object.Instantiate(prefabToAdd) as GameObject;
                        objectInstance = UnityEngine.Object.Instantiate(objectToAdd) as GameObject;

                        if (prefabInstance == null || objectInstance == null)
                        {
                            ReMapConsole.Log($"[Library Sorter] Error creating prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                            continue;
                        }

                        prefabInstance.AddComponent<PropScript>();

                        prefabInstance.name = modelReplacePath.Replace(".prefab", "");
                        objectInstance.name = model + "_LOD0";

                        prefabInstance.transform.position = Vector3.zero;
                        prefabInstance.transform.eulerAngles = Vector3.zero;

                        objectInstance.transform.parent = prefabInstance.transform;
                        objectInstance.transform.position = Vector3.zero;
                        objectInstance.transform.eulerAngles = FindAnglesOffset( modelPath, offsets );
                        objectInstance.transform.localScale = new Vector3(1, 1, 1);

                        prefabInstance.tag = Helper.GetObjTagNameWithEnum(ObjectType.Prop);

                        CheckBoxColliderComponent( prefabInstance );

                        PrefabUtility.SaveAsPrefabAsset(prefabInstance, $"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}");

                        UnityEngine.Object.DestroyImmediate(prefabInstance);
                        ReMapConsole.Log($"[Library Sorter] Creating prefab: {relativePrefabs}/{mapName}/{modelReplacePath}", ReMapConsole.LogType.Info);
                    }
                    else
                    {
                        if(!checkandfixifexists)
                            continue;

                        UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath($"{relativePrefabs}/{mapName}/{modelReplacePath}", typeof(UnityEngine.Object)) as GameObject;
                        if (loadedPrefabResource == null)
                        {
                            ReMapConsole.Log($"[Library Sorter] Error loading prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                            continue;
                        }

                        Transform child = loadedPrefabResource.GetComponentsInChildren<Transform>()[1];

                        loadedPrefabResource.transform.position = Vector3.zero;
                        loadedPrefabResource.transform.eulerAngles = Vector3.zero;
                        child.transform.eulerAngles = FindAnglesOffset( modelPath, offsets );
                        child.transform.position = Vector3.zero;

                        CheckBoxColliderComponent( loadedPrefabResource );

                        PrefabUtility.SavePrefabAsset(loadedPrefabResource);

                        ReMapConsole.Log($"[Library Sorter] Fixed and saved prefab: {relativePrefabs}/{mapName}/{modelReplacePath}", ReMapConsole.LogType.Success);
                    }
                }
                i++;
            }

            ReMapConsole.Log($"[Library Sorter] Setting labels for prefabs in: {mapName}", ReMapConsole.LogType.Info);
            EditorUtility.ClearProgressBar();
            LibrarySorterWindow.SetFolderLabels(mapName);
            //////////////////////////
            */

            data.Update = DateTime.Now.ToString();

            //ReMapConsole.Log($"[Library Sorter] Finished sorting models", ReMapConsole.LogType.Success);

            return Task.CompletedTask;
        }

        public static Task SetModelLabels( string specificModelOrFolderOrnull = null )
        {
            string specificFolder = $"";
            string specificModel = $"mdl#";

            if ( specificModelOrFolderOrnull != null )
            {
                if ( specificModelOrFolderOrnull.Contains("mdl#") )
                {
                    specificModel = specificModelOrFolderOrnull;
                }
                else specificFolder = $"/{specificModelOrFolderOrnull}";
            }

            string[] guids = AssetDatabase.FindAssets( $"{specificModel}", new [] {$"{UnityInfo.relativePathPrefabs}{specificFolder}"} );
            int i = 0; int total = guids.Length;
            foreach ( var guid in guids )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath( assetPath );
                string category = assetPath.Split( "#" )[1].Replace( "Assets/Prefabs/", "" ).Replace( ".prefab", "" ).ToLower();

                string[] modelnamesplit = assetPath.Split( "/" );
                string modelname = modelnamesplit[modelnamesplit.Length - 1].Replace( ".prefab", "" );

                string[] labels = AssetDatabase.GetLabels( asset );

                if( !labels.Contains( category ) )
                {
                    AssetDatabase.SetLabels(asset, new []{category});
                    ReMapConsole.Log( $"[Library Sorter] Setting label for {modelname} to {category}", ReMapConsole.LogType.Info );
                }

                EditorUtility.DisplayProgressBar( $"Sorting Tags {i}/{total}", $"Setting {modelname} to {category}", (i + 1) / (float)total ); i++;
            }

            ReMapConsole.Log($"[Library Sorter] Finished setting labels", ReMapConsole.LogType.Success);
            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static Task FixPrefabsTags()
        {
            string[] prefabs = AssetDatabase.FindAssets("t:prefab", new[] { UnityInfo.relativePathPrefabs });

            int i = 0; int total = prefabs.Length;
            foreach ( string prefab in prefabs )
            {
                string path = AssetDatabase.GUIDToAssetPath( prefab );
                string pathReplace = path.Replace( "Assets/Prefabs/", "" );

                if ( path.Contains( "_custom_prefabs" ) )
                {
                    i++;
                    continue;
                }

                UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath( $"{path}", typeof( UnityEngine.Object ) ) as GameObject;
                if ( loadedPrefabResource == null )
                {
                    ReMapConsole.Log( $"[Library Sorter] Error loading prefab: {path}", ReMapConsole.LogType.Error );
                    i++;
                    continue;
                }

                EditorUtility.DisplayProgressBar( $"Fixing Prefabs Tags {i}/{total}", $"Checking: {path}", ( i + 1 ) / ( float )total );

                if( loadedPrefabResource.tag != Helper.GetObjTagNameWithEnum( ObjectType.Prop ) )
                    loadedPrefabResource.tag = Helper.GetObjTagNameWithEnum( ObjectType.Prop );

                ReMapConsole.Log( $"[Library Sorter] Set {path} tag to: {Helper.GetObjTagNameWithEnum( ObjectType.Prop )}", ReMapConsole.LogType.Info );

                PrefabUtility.SavePrefabAsset( loadedPrefabResource ); i++;

                //await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            }

            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static bool CheckDialog( string title, string content, string trueText = "Yes", string falseText = "No" )
        {
            return EditorUtility.DisplayDialog( title, content, trueText, falseText );
        }
    }
}
