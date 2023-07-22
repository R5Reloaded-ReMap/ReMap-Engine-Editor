
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    internal enum TaskType
    {
        FixPrefabsTags,
        FixPrefabsLabels,
        DeleteUnusuedTextures,
        FixLodsScale,
        CreateRpakList,
        FixPrefabsData,
        FixAllPrefabsData,
        FixSpecificPrefabData
    }

    public class LibrarySorterWindow : EditorWindow
    {
        internal static LibraryData libraryData;
        internal static RpakData all_model;
        internal static bool checkExist = false;
        Vector2 scrollPos = Vector2.zero;
        Vector2 scrollPosSearchPrefabs = Vector2.zero;

        static bool foldoutFixFolders = true;
        static bool foldoutSearchPrefab = true;
        static string searchEntry = "";
        static string search = "";
        static List< string > searchResult = new List< string >();
        
        #if ReMapDev
            [ MenuItem( "ReMap Dev Tools/Prefabs Management/Prefab Fix Manager", false, 100 ) ]
            public static void Init()
            {
                libraryData = RpakManagerWindow.FindLibraryDataFile();

                LibrarySorterWindow window = ( LibrarySorterWindow )GetWindow( typeof( LibrarySorterWindow ), false, "Prefab Fix Manager" );
                window.minSize = new Vector2( 650, 600 );
                window.Show();
            }
        #endif

        private void OnEnable()
        {
            libraryData = RpakManagerWindow.FindLibraryDataFile();
            all_model = RpakManagerWindow.FindAllModel();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal();

                    WindowUtility.WindowUtility.CreateButton( "Rpak Manager Window", "", () => RpakManagerWindow.Init() );
                    WindowUtility.WindowUtility.CreateButton( "Offset Manager Window", "", () => OffsetManagerWindow.Init() );

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                    WindowUtility.WindowUtility.CreateButton( "Check Prefabs Tags", "", () => AwaitTask( TaskType.FixPrefabsTags ) );
                    WindowUtility.WindowUtility.CreateButton( "Check Prefabs Labels", "", () => AwaitTask( TaskType.FixPrefabsLabels ) );
                    WindowUtility.WindowUtility.CreateButton( "Check Textures", "", () => AwaitTask( TaskType.DeleteUnusuedTextures ) );
                    WindowUtility.WindowUtility.CreateButton( "Check Lods Scale", "", () => AwaitTask( TaskType.FixLodsScale ) );
                    WindowUtility.WindowUtility.CreateButton( "Create Rpak List", "", () => AwaitTask( TaskType.CreateRpakList ) );

                GUILayout.EndHorizontal();

                GUILayout.Space( 4 );

                scrollPos = EditorGUILayout.BeginScrollView( scrollPos );
                    foldoutFixFolders = EditorGUILayout.BeginFoldoutHeaderGroup( foldoutFixFolders, "Fix Folders" );
                    if ( foldoutFixFolders )
                    {
                        foreach ( RpakData data in libraryData.RpakList )
                        {
                            GUILayout.BeginHorizontal();
                                if ( WindowUtility.WindowUtility.CreateButton( $"{data.Name}", "", () => AwaitTask( TaskType.FixPrefabsData, null, data ), 260 ) )
                                {
                                    GUILayout.EndHorizontal();
                                    EditorGUILayout.EndFoldoutHeaderGroup();
                                    GUILayout.EndScrollView();
                                    GUILayout.EndVertical();
                                    return;
                                }
                                WindowUtility.WindowUtility.CreateButton( $"Find Missing", "TODO", 160 );
                                WindowUtility.WindowUtility.CreateTextInfo( $"Lastest Check: {data.Update}", "" );
                            GUILayout.EndHorizontal();
                        }
                        WindowUtility.WindowUtility.CreateButton( $"Check All", "", () => AwaitTask( TaskType.FixAllPrefabsData ) );
                        GUILayout.Space( 4 );
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
                                if ( searchResult.Count != 0 )
                                {
                                    foreach ( string prefab in searchResult )
                                    {
                                        string prefabName = UnityInfo.GetUnityModelName( prefab );
                                        WindowUtility.WindowUtility.CreateButton( $"{prefabName}", "", () => AwaitTask( TaskType.FixSpecificPrefabData, prefabName ) );
                                    }
                                }
                                else WindowUtility.WindowUtility.CreateTextInfoCentered( "No results.", "" );
                            }
                            else if ( searchEntry.Length != 0 )
                            {
                                WindowUtility.WindowUtility.CreateTextInfoCentered( "Search must be at least 3 characters long.", "" );
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

                case TaskType.DeleteUnusuedTextures:
                    if ( !DoStartTask() ) return;
                    await DeleteNotUsedTexture();
                    break;

                case TaskType.FixLodsScale:
                    if ( !DoStartTask() ) return;
                    await SetScale100ToFBX();
                    break;

                case TaskType.CreateRpakList:
                    if ( !DoStartTask() ) return;
                    await CreateRpakList();
                    break;

                case TaskType.FixPrefabsData:
                    if ( !DoStartTask() ) return;
                    CheckExisting();
                    await SortFolder( data );
                    await SetModelLabels( data.Name );
                    RpakManagerWindow.SaveJson();
                    break;

                case TaskType.FixAllPrefabsData:
                    if ( !DoStartTask() ) return;
                    CheckExisting();
                    foreach ( RpakData _data in libraryData.RpakList )
                    {
                        await SortFolder( _data );
                        await SetModelLabels( _data.Name );
                    }
                    RpakManagerWindow.SaveJson();
                    break;

                case TaskType.FixSpecificPrefabData:
                    checkExist = true;
                    await FixPrefab( arg );
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

            int i = 0; int total = data.Data.Count;
            foreach ( string model in data.Data )
            {
                modelName = Path.GetFileNameWithoutExtension( model );

                if ( File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx" ) )
                {
                    modelReplacePath = model.Replace("/", "#").Replace(".rmdl", ".prefab");

                    EditorUtility.DisplayProgressBar( $"Sorting {data.Name} Folder {i}/{total}", $"Sorting: {modelReplacePath}", ( i + 1 ) / ( float )total );

                    if ( !File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{data.Name}/{modelReplacePath}" ) )
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
                        objectInstance.transform.eulerAngles = FindAnglesOffset( model );
                        objectInstance.transform.localScale = new Vector3(1, 1, 1);

                        prefabInstance.tag = Helper.GetObjTagNameWithEnum( ObjectType.Prop );

                        CheckBoxColliderComponent( prefabInstance );

                        PrefabUtility.SaveAsPrefabAsset( prefabInstance, $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}/{modelReplacePath}" );

                        UnityEngine.Object.DestroyImmediate( prefabInstance );
                        ReMapConsole.Log( $"[Library Sorter] Created and saved prefab: {UnityInfo.relativePathPrefabs}/{data.Name}/{modelReplacePath}", ReMapConsole.LogType.Info );
                    }
                    else
                    {
                        if ( !checkExist ) continue;

                        UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath( $"{UnityInfo.relativePathPrefabs}/{data.Name}/{modelReplacePath}", typeof( UnityEngine.Object ) ) as GameObject;
                        if ( loadedPrefabResource == null )
                        {
                            ReMapConsole.Log($"[Library Sorter] Error loading prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                            continue;
                        }

                        Transform child = loadedPrefabResource.GetComponentsInChildren< Transform >()[1];

                        loadedPrefabResource.transform.position = Vector3.zero;
                        loadedPrefabResource.transform.eulerAngles = Vector3.zero;
                        child.transform.eulerAngles = FindAnglesOffset( model );
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

            return Task.CompletedTask;
        }

        public static Vector3 FindAnglesOffset( string searchTerm )
        {
            Vector3 returnedVector = new Vector3( 0, -90, 0 );

            PrefabOffset offset = FindPrefabOffsetFile().Find( o => o.ModelName == searchTerm );
            if ( offset != null )
            {
                returnedVector = offset.Rotation;
                ReMapConsole.Log( $"[Library Sorter] Angle override found for {searchTerm}, setting angles to: {returnedVector}", ReMapConsole.LogType.Info );
            }

            return returnedVector;
        }

        private static void CheckBoxColliderComponent( GameObject go )
        {
            BoxCollider collider = go.GetComponent< BoxCollider >();

            if ( !Helper.IsValid( collider ) ) collider = go.AddComponent< BoxCollider >();

            float x = 0, y = 0, z = 0;

            foreach( Renderer o in go.GetComponentsInChildren< Renderer >() )
            {
                if( o.bounds.size.x > x ) x = o.bounds.size.x;

                if( o.bounds.size.y > y ) y = o.bounds.size.y;

                if( o.bounds.size.z > z ) z = o.bounds.size.z;
            }

            collider.size = new Vector3( x, y, z );
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

                EditorUtility.DisplayProgressBar( $"Sorting Tags {i}/{total}", $"Setting {modelname} to {category}", ( i + 1 ) / ( float )total ); i++;
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
            }

            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static Task DeleteNotUsedTexture()
        {
            List< string > texturesList = new List< string >();

            string[] modeltextureGUID = AssetDatabase.FindAssets( "t:model", new [] { UnityInfo.relativePathModel } );

            int i = 0; int total = modeltextureGUID.Length;
            foreach ( var guid in modeltextureGUID )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                string[] dependencie = AssetDatabase.GetDependencies( assetPath );
                foreach( string dependencies in dependencie )
                {
                    string fileName = Path.GetFileNameWithoutExtension( dependencies );
                    if ( Path.GetExtension( dependencies ) == ".dds" && !texturesList.Contains( fileName ) )
                    {
                        texturesList.Add( fileName );
                    }
                }

                EditorUtility.DisplayProgressBar( $"Obtaining dependencies {i}/{total}", $"Checking: {guid}", ( i + 1 ) / ( float )total ); i++;
            }

            string[] usedTextures = texturesList.ToArray();

            string[] defaultAssetGUID = AssetDatabase.FindAssets( "t:defaultAsset", new [] { UnityInfo.relativePathMaterials } );
            int j = 0; total = defaultAssetGUID.Length;
            foreach ( var guid in defaultAssetGUID )
            {
                string defaultAssetPath = AssetDatabase.GUIDToAssetPath( guid );

                if ( Path.GetExtension( defaultAssetPath ) == ".dds")
                {
                    File.Delete( defaultAssetPath );
                    File.Delete( defaultAssetPath + ".meta");
                    j++;
                }

                EditorUtility.DisplayProgressBar( $"Checking default assets {j}/{total}", $"Checking: {guid}", ( j + 1 ) / ( float )total ); j++;
            }

            string[] textureGUID = AssetDatabase.FindAssets("t:texture", new [] { UnityInfo.relativePathMaterials });
            int k = 0; total = textureGUID.Length;
            foreach ( var guid in textureGUID )
            {
                string texturePath = AssetDatabase.GUIDToAssetPath( guid );

                if( !usedTextures.Contains(Path.GetFileNameWithoutExtension( texturePath ) ) )
                {
                    File.Delete( texturePath );
                    File.Delete( texturePath + ".meta");
                    k++;
                }

                EditorUtility.DisplayProgressBar( $"Checking textures {k}/{total}", $"Checking: {guid}", ( k + 1 ) / ( float )total ); k++;
            }

            ReMapConsole.Log( $"{j} native assets have been deleted", ReMapConsole.LogType.Success );
            ReMapConsole.Log( $"{k} textures not used have been deleted", ReMapConsole.LogType.Success );
            ReMapConsole.Log( $"Total used textures: {usedTextures.Length} for {modeltextureGUID.Length} models", ReMapConsole.LogType.Info );

            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static Task SetScale100ToFBX()
        {
            string[] models = AssetDatabase.FindAssets( "t:Model", new string[] { UnityInfo.relativePathModel } );

            List< ModelImporter > modelImporter = new List< ModelImporter >();

            int i = 0; int total = models.Length;
            foreach ( string model in models )
            {
                string path = AssetDatabase.GUIDToAssetPath( model );
                EditorUtility.DisplayProgressBar( $"Checking FBX Scale {i}/{total}", $"Checking: {Path.GetFileName( path )}", ( i + 1 ) / ( float )models.Length);
                ModelImporter importer = AssetImporter.GetAtPath( path ) as ModelImporter;
                if ( importer != null )
                {
                    importer.globalScale = 100;
                    modelImporter.Add( importer );
                } i++;
            }

            EditorUtility.ClearProgressBar();

            foreach ( ModelImporter model in modelImporter ) model.SaveAndReimport();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return Task.CompletedTask;
        }

        internal static Task FixPrefab( string prefabName )
        {
            string[] prefabs = AssetDatabase.FindAssets( prefabName, new [] { UnityInfo.relativePathPrefabs } );

            int i = 0; int total = prefabs.Length;
            foreach ( string prefab in prefabs )
            {
                string file = AssetDatabase.GUIDToAssetPath( prefab );
                string rpakName = UnityInfo.GetApexModelName( prefabName, true );

                EditorUtility.DisplayProgressBar( $"Fixing Prefabs {i}/{total}", $"Prefab: {prefab}", ( i + 1 ) / ( float )total );

                UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath( file, typeof( UnityEngine.Object ) ) as GameObject;
                if ( loadedPrefabResource == null )
                {
                    ReMapConsole.Log($"[Library Sorter] Error loading prefab: {file}", ReMapConsole.LogType.Error );
                    continue;
                }

                Transform child = loadedPrefabResource.GetComponentsInChildren< Transform >()[1];

                CheckBoxColliderComponent( loadedPrefabResource );

                loadedPrefabResource.transform.position = Vector3.zero;
                loadedPrefabResource.transform.eulerAngles = Vector3.zero;
                child.transform.eulerAngles = FindAnglesOffset( rpakName );
                child.transform.position = Vector3.zero;

                PrefabUtility.SavePrefabAsset( loadedPrefabResource );

                ReMapConsole.Log( $"[Library Sorter] Fixed and saved prefab: {file}", ReMapConsole.LogType.Success ); i++;
            }

            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static List< PrefabOffset > FindPrefabOffsetFile()
        {
            string json = System.IO.File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}" );
            return JsonUtility.FromJson< PrefabOffsetList >( json ).List;
        }

        private static Task CreateRpakList()
        {
            RpakContentJson rpakContentJson = CreateRpakContentJson();

            string[] files = Directory.GetFiles( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/all_models", "*.prefab", SearchOption.TopDirectoryOnly ).ToArray();

            int i = 0; int total = files.Length;

            foreach ( string file in files )
            {
                RpakContentClass content = new RpakContentClass();
                content.modelName = Path.GetFileNameWithoutExtension( file );
                content.location = new List< string >();
                string rpakName = "";

                foreach ( RpakData data in libraryData.RpakList )
                {
                    if ( data.Name == RpakManagerWindow.allModelsDataName ) continue;

                    rpakName = UnityInfo.GetApexModelName( content.modelName, true );

                    if ( data.Data.Contains( rpakName ) )
                    {
                        content.location.Add( data.Name );
                    }
                }

                rpakContentJson.List.Add( content );

                EditorUtility.DisplayProgressBar( $"Checking Rpak Content {i}/{total}", $"Checking {content.modelName}", ( i + 1 ) / ( float )total );
                i++;
            }

            EditorUtility.ClearProgressBar();

            string json = JsonUtility.ToJson( rpakContentJson );
            System.IO.File.WriteAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/RpakList.json", json );

            return Task.CompletedTask;
        }

        private static RpakContentJson CreateRpakContentJson()
        {
            RpakContentJson rpakContent = new RpakContentJson();
            rpakContent.List = new List< RpakContentClass >();

            return rpakContent;
        }

        private static void SearchPrefabs( string search = "" )
        {
            searchResult = new List< string >();

            foreach ( string prefab in all_model.Data )
            {
                if( search != "" && !prefab.Contains( search ) )
                    continue;
    
                searchResult.Add( prefab );
            }
        }

        internal static void CheckExisting()
        {
            checkExist = CheckDialog( "Check Existing Prefabs", "Do you want check existing prefabs ?" );
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
