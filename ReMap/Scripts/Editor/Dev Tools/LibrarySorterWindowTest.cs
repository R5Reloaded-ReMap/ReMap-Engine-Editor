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
        FixPrefabsData,
        FixAllPrefabsData,
        FixSpecificPrefabData
    }

    public class LibrarySorterWindowTest : EditorWindow
    {
        internal static LibraryData libraryData;
        internal static List< PrefabOffset > prefabOffset;
        internal static bool checkExist = false;
        Vector2 scrollPos = Vector2.zero;
        Vector2 scrollPosSearchPrefabs = Vector2.zero;

        static bool foldoutFixFolders = true;
        static bool foldoutSearchPrefab = true;
        static string searchEntry = "";
        static string search = "";
        static List< string > searchResult = new List< string >();
        
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
            prefabOffset = FindPrefabOffsetFile();
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
                    foldoutFixFolders = EditorGUILayout.BeginFoldoutHeaderGroup( foldoutFixFolders, "Fix Folders" );
                    if ( foldoutFixFolders )
                    {
                        foreach ( RpakData data in libraryData.RpakList )
                        {
                            GUILayout.BeginHorizontal();
                                if ( WindowUtility.WindowUtility.CreateButton( $"{data.Name}", "", () => AwaitTask( TaskType.FixPrefabsData, null, data ) ) )
                                {
                                    GUILayout.EndHorizontal();
                                    EditorGUILayout.EndFoldoutHeaderGroup();
                                    GUILayout.EndScrollView();
                                    GUILayout.EndVertical();
                                    return;
                                }
                                WindowUtility.WindowUtility.CreateButton( $"Find Missing", "", null, 140 );
                                GUILayout.Label( $"Lastest Ckeck: {data.Update}", GUILayout.Width( 216 ) );
                            GUILayout.EndHorizontal();
                        }
                        WindowUtility.WindowUtility.CreateButton( $"Check All", "", () => AwaitTask( TaskType.FixAllPrefabsData ) );
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();

                    foldoutSearchPrefab = EditorGUILayout.BeginFoldoutHeaderGroup( foldoutSearchPrefab, "Fix Prefabs" );
                    if ( foldoutSearchPrefab )
                    {
                        searchEntry = EditorGUILayout.TextField( searchEntry );

                        if ( searchEntry.Length >= 3 )
                        {
                            if ( searchEntry != search )
                            {
                                search = searchEntry;
                                SearchPrefabs( search );
                            }
                        }

                        scrollPosSearchPrefabs = EditorGUILayout.BeginScrollView( scrollPosSearchPrefabs );

                            GUILayout.Space( 10 );
                            if ( searchEntry.Length >= 3 )
                            {
                                foreach ( string prefab in searchResult )
                                {
                                    string prefabName = Path.GetFileNameWithoutExtension( prefab );
                                    WindowUtility.WindowUtility.CreateButton( $"{prefabName}", "", () => AwaitTask( TaskType.FixSpecificPrefabData, prefabName ) );
                                }
                            }
                            else
                            {
                                GUILayout.Label("Search must be at least 3 characters long.");
                            }

                        GUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        internal static async void AwaitTask( TaskType taskType, string arg = null, RpakData data = null )
        {
            switch ( taskType )
            {
                case TaskType.FixPrefabsTags:
                    if ( !DoStartTask() ) return;
                    await FixPrefabsTags();
                    break;
                case TaskType.FixPrefabsLabels:
                    if ( !DoStartTask() ) return;
                    await SetModelLabels( arg );
                    break;
                case TaskType.FixPrefabsData:
                    if ( !DoStartTask() ) return;
                    checkExist = CheckDialog( "Check Existing Prefabs", "Do you want check existing prefabs ?" );
                    await SortFolder( data );
                    await SetModelLabels( data.Name );
                    break;
                case TaskType.FixAllPrefabsData:
                    if ( !DoStartTask() ) return;
                    checkExist = CheckDialog( "Check Existing Prefabs", "Do you want check existing prefabs ?" );
                    foreach ( RpakData _data in libraryData.RpakList )
                    {
                        await SortFolder( _data );
                        await SetModelLabels( _data.Name );
                    }
                    break;
                case TaskType.FixSpecificPrefabData:
                    //await FixPrefab( arg );
                    await SetModelLabels( arg );
                break;
            }
        }

        internal static Task SortFolder( RpakData data )
        {
            string rpakPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}";

            if (!Directory.Exists( rpakPath ))
            {
                ReMapConsole.Log( $"[Library Sorter] Creating directory: {UnityInfo.relativePathPrefabs}/{data.Name}", ReMapConsole.LogType.Info );
                Directory.CreateDirectory( rpakPath );
            }

            string modelName; string modelReplacePath;
            GameObject prefabToAdd; GameObject prefabInstance;
            GameObject objectToAdd; GameObject objectInstance;

            if ( prefabOffset == null ) prefabOffset = FindPrefabOffsetFile();

            int i = 0; int total = data.Data.Count;
            foreach ( string model in data.Data )
            {
                modelName = Path.GetFileNameWithoutExtension( model );

                if ( File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{modelName + "_LOD0.fbx"}" ) )
                {
                    modelReplacePath = model.Replace("/", "#").Replace(".rmdl", ".prefab");

                    EditorUtility.DisplayProgressBar( $"Sorting {data.Name} Folder {i}/{total}", $"Sorting: {modelReplacePath}", ( i + 1 ) / ( float )total );

                    if (!File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{data.Name}/{modelReplacePath}" ) )
                    {
                        prefabToAdd = AssetDatabase.LoadAssetAtPath( $"{UnityInfo.relativePathEmptyPrefab}", typeof( UnityEngine.Object ) ) as GameObject;
                        objectToAdd = AssetDatabase.LoadAssetAtPath( $"{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx", typeof( UnityEngine.Object ) ) as GameObject;

                        if ( prefabToAdd == null || objectToAdd == null )
                        {
                            ReMapConsole.Log($"[Library Sorter] Error loading prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                            continue;
                        }

                        prefabInstance = UnityEngine.Object.Instantiate( prefabToAdd ) as GameObject;
                        objectInstance = UnityEngine.Object.Instantiate( objectToAdd ) as GameObject;

                        if (prefabInstance == null || objectInstance == null)
                        {
                            ReMapConsole.Log($"[Library Sorter] Error creating prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                            continue;
                        }

                        prefabInstance.AddComponent< PropScript >();

                        prefabInstance.name = modelReplacePath.Replace(".prefab", "");
                        objectInstance.name = modelName + "_LOD0";

                        prefabInstance.transform.position = Vector3.zero;
                        prefabInstance.transform.eulerAngles = Vector3.zero;

                        objectInstance.transform.parent = prefabInstance.transform;
                        objectInstance.transform.position = Vector3.zero;
                        objectInstance.transform.eulerAngles = FindAnglesOffset( model, prefabOffset );
                        objectInstance.transform.localScale = new Vector3(1, 1, 1);

                        prefabInstance.tag = Helper.GetObjTagNameWithEnum( ObjectType.Prop );

                        CheckBoxColliderComponent( prefabInstance );

                        PrefabUtility.SaveAsPrefabAsset( prefabInstance, $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}/{modelReplacePath}" );

                        UnityEngine.Object.DestroyImmediate( prefabInstance );
                        ReMapConsole.Log( $"[Library Sorter] Created and saved prefab: {UnityInfo.relativePathPrefabs}/{data.Name}/{modelReplacePath}", ReMapConsole.LogType.Info );
                    }
                    else
                    {
                        if( !checkExist ) continue;

                        UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath( $"{UnityInfo.relativePathPrefabs}/{data.Name}/{modelReplacePath}", typeof( UnityEngine.Object ) ) as GameObject;
                        if ( loadedPrefabResource == null )
                        {
                            ReMapConsole.Log($"[Library Sorter] Error loading prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                            continue;
                        }

                        Transform child = loadedPrefabResource.GetComponentsInChildren< Transform >()[1];

                        loadedPrefabResource.transform.position = Vector3.zero;
                        loadedPrefabResource.transform.eulerAngles = Vector3.zero;
                        child.transform.eulerAngles = FindAnglesOffset( model, prefabOffset );
                        child.transform.position = Vector3.zero;

                        CheckBoxColliderComponent( loadedPrefabResource );

                        PrefabUtility.SavePrefabAsset( loadedPrefabResource );

                        ReMapConsole.Log( $"[Library Sorter] Fixed and saved prefab: {UnityInfo.relativePathPrefabs}/{data.Name}/{modelReplacePath}", ReMapConsole.LogType.Success );
                    }
                }
                i++;
            }

            EditorUtility.ClearProgressBar();

            data.Update = DateTime.UtcNow.ToString();

            RpakManagerWindow.SaveJson();

            return Task.CompletedTask;
        }

        private static Vector3 FindAnglesOffset( string searchTerm, List< PrefabOffset > list )
        {
            Vector3 returnedVector = new Vector3( 0, -90, 0 );

            PrefabOffset offset = list.Find( o => o.ModelName == searchTerm );
            if ( offset != null )
            {
                returnedVector = offset.Rotation;
                ReMapConsole.Log( $"[Library Sorter] Angle override found for {searchTerm}, setting angles to: {returnedVector}", ReMapConsole.LogType.Info );
            }

            return returnedVector;
        }

        private static void CheckBoxColliderComponent( GameObject go )
        {
            BoxCollider collider = go.GetComponent<BoxCollider>();

            if( collider == null ) collider = go.AddComponent<BoxCollider>();

            Bounds bounds = new Bounds();

            foreach(Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);

                BoxCollider BoxColliderChild = renderer.GetComponent<BoxCollider>();

                if(BoxColliderChild != null) DestroyImmediate(BoxColliderChild, true);
            }

            foreach(Renderer renderer in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            collider.center = bounds.center;
            collider.size = bounds.size;
        }

        internal static Task SetModelLabels( string specificModelOrFolderOrnull = null )
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

        internal static List< PrefabOffset > FindPrefabOffsetFile()
        {
            string json = System.IO.File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}" );
            return JsonUtility.FromJson< PrefabOffsetList >( json ).List;
        }

        private static void SearchPrefabs( string search = "" )
        {
            string[] prefabs = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Prefabs"});
            searchResult = new List<string>();
    
            foreach ( string prefab in prefabs )
            {
                string path = AssetDatabase.GUIDToAssetPath( prefab );

                if( !path.Contains("mdl#") )
                    continue;
    
                if( !path.Contains("all_models") )
                    continue;
    
                if( search != "" && !path.Contains( search ) )
                    continue;
    
                searchResult.Add( path );
            }
        }

        internal static bool DoStartTask()
        {
            return CheckDialog( "Library Sorter", "Are you sure to start this task ?" );
        }

        internal static bool CheckDialog( string title, string content, string trueText = "Yes", string falseText = "No" )
        {
            return EditorUtility.DisplayDialog( title, content, trueText, falseText );
        }
    }
}
