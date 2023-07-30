
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
        ExtractMissingModels,
        CheckTextures,
        CreateMaterialsList,
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
        static string relativeLegionPlus = $"Assets/ReMap/LegionPlus";
        static string relativeLegionPlusExportedFiles = $"Assets/ReMap/LegionPlus/exported_files";
        static string relativeLegionExecutive = $"{UnityInfo.currentDirectoryPath}/{relativeLegionPlus}/LegionPlus.exe";

        internal static Dictionary< string, Texture2D > existingTextures = new Dictionary< string, Texture2D >();

        internal static List< string > missingTextures = new List< string >();

        internal static StringBuilder legionArgument = new StringBuilder();
        internal static List< string > legionArguments = new List< string >();
        
        public static void Init()
        {
            libraryData = RpakManagerWindow.FindLibraryDataFile();

            LibrarySorterWindow window = ( LibrarySorterWindow ) GetWindow( typeof( LibrarySorterWindow ), false, "Prefab Fix Manager" );
            window.minSize = new Vector2( 650, 600 );
            window.Show();
        }

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
                    WindowUtility.WindowUtility.CreateButton( "Check Lods Scale", "", () => AwaitTask( TaskType.FixLodsScale ) );
                    WindowUtility.WindowUtility.CreateButton( "Create Rpak List", "", () => AwaitTask( TaskType.CreateRpakList ) );

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                    WindowUtility.WindowUtility.CreateButton( "Extract Missing Models", "", () => AwaitTask( TaskType.ExtractMissingModels ) );
                    WindowUtility.WindowUtility.CreateButton( "Check Textures", "", () => AwaitTask( TaskType.CheckTextures ) );
                    WindowUtility.WindowUtility.CreateButton( "Update Materials List", "", () => AwaitTask( TaskType.CreateMaterialsList ) );
                    WindowUtility.WindowUtility.CreateButton( "Unusued Textures", "", () => AwaitTask( TaskType.DeleteUnusuedTextures ) );
                    

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

                case TaskType.ExtractMissingModels:
                    if ( !DoStartTask() ) return;
                    if ( LegionExporting.GetValidRpakPaths() ) return;
                    await Models.ExtractMissingModels();
                    break;

                case TaskType.CheckTextures:
                    if ( !DoStartTask() ) return;
                    await CheckTextures();
                    break;
                
                case TaskType.CreateMaterialsList:
                    if ( !DoStartTask() ) return;
                    await Materials.CreateMaterialData();
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
                    if ( LegionExporting.GetValidRpakPaths() ) return;
                    await SortFolder( data );
                    await SetModelLabels( data.Name );
                    RpakManagerWindow.SaveJson();
                    break;

                case TaskType.FixAllPrefabsData:
                    if ( !DoStartTask() ) return;
                    CheckExisting();
                    if ( LegionExporting.GetValidRpakPaths() ) return;
                    foreach ( RpakData _data in libraryData.RpakList )
                    {
                        await SortFolder( _data );
                        await SetModelLabels( _data.Name );
                    }
                    RpakManagerWindow.SaveJson();
                    break;

                case TaskType.FixSpecificPrefabData:
                    checkExist = true;
                    await Models.FixPrefab( arg );
                    await SetModelLabels( arg );
                break;
            }
        }

        internal static async Task SortFolder( RpakData data )
        {
            string rpakPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}";

            if ( !Directory.Exists( rpakPath ) )
            {
                ReMapConsole.Log( $"[Library Sorter] Creating directory: {UnityInfo.relativePathPrefabs}/{data.Name}", ReMapConsole.LogType.Info );
                Directory.CreateDirectory( rpakPath );
            }

            GameObject parent; GameObject obj; string modelName;

            float progress = 0.0f; int min = 0; int max = data.Data.Count;

            List< string > modelImporter = new List< string >();

            List< string > lod0List = new List< string >();

            // Legion Launch Arguments
            StringBuilder assets = new StringBuilder();
            List< string > assetsList = new List< string >();

            // Check if LOD0 doesn't exist
            foreach ( string model in data.Data )
            {
                modelName = Path.GetFileNameWithoutExtension( model );

                EditorUtility.DisplayProgressBar( $"Checking LOD0 ({min}/{max})", $"Processing... ({modelName})", progress );

                // If the .fbx doesn't exist in Asset/ReMap/.../Models
                if ( !Helper.LOD0_Exist( modelName ) )
                {
                    assets.Append( $"{modelName}," );

                    // Executive launch args can't be more long than 8191 characters
                    if ( assets.Length > 5000 )
                    {
                        assets.Remove( assets.Length - 1, 1 );

                        assetsList.Add( assets.ToString() );

                        assets = new ();
                    }

                    lod0List.Add( modelName );
                }

                progress += 1.0f / max; min++;
            }

            // Reset progress bar
            progress = 0.0f; min = 0; max = lod0List.Count;

            if ( assets.Length > 0 || assetsList.Count > 0 )
            {
                // Remove last ','
                assets.Remove( assets.Length - 1, 1 );
                assetsList.Add( assets.ToString() );

                string loading = "";
                int loadingCount = 0;

                int i = 1; int j = assetsList.Count;

                // Try to export missing .fbx file using Legion
                foreach ( string argument in assetsList )
                {
                    Task legionTask = LegionExporting.ExtractModelFromLegion( argument );

                    string count = j > 1 ? $" ({i}/{j})" : "";

                    while ( !legionTask.IsCompleted )
                    {
                        EditorUtility.DisplayProgressBar( $"Legion Extraction{count}", $"Extracting files{loading}", 0.0f );

                        loading = new string( '.', loadingCount++ % 4 );

                        await Helper.Wait( 1.0 );
                    }

                    i++;
                }
            }

            // Reset progress bar
            progress = 0.0f; min = 0;

            // Try to get new .fbx files and its textures and move it
            foreach ( string model in lod0List )
            {
                modelName = Path.GetFileNameWithoutExtension( model );

                string rFilePath = $"{relativeLegionPlusExportedFiles}/models/{modelName}/{modelName}_LOD0.fbx";
                string rMatsPath = $"{relativeLegionPlusExportedFiles}/models/{modelName}/_images";
                string rGotoPathModel = $"{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx";
                string rGotoPathMaterial = $"{UnityInfo.relativePathMaterials}";

                EditorUtility.DisplayProgressBar( $"Checking New LOD0", $"Processing... ({min++}/{max})", progress );
                progress += 1.0f / max;

                if ( !Helper.MoveFile( rFilePath, rGotoPathModel ) )
                {
                    continue;
                }

                foreach ( string matsFile in Directory.GetFiles( rMatsPath ) )
                {
                    string fileName = Path.GetFileName( matsFile );

                    if ( fileName.Contains( "_albedoTexture" ) || fileName.Contains( "0x" ) )
                    {
                        Helper.MoveFile( matsFile, $"{UnityInfo.currentDirectoryPath}/{rGotoPathMaterial}/{fileName}", false );
                    }
                    else
                    {
                        Helper.DeleteFile( matsFile, false );
                    }
                }

                string dir = $"{UnityInfo.currentDirectoryPath}/{relativeLegionPlusExportedFiles}/models/{modelName}";
                if ( Directory.Exists( dir ) )
                {
                    Directory.Delete( dir, true );

                    if ( File.Exists( $"{dir}.meta" ) )
                    {
                        File.Delete( $"{dir}.meta" );
                    }
                }

                modelImporter.Add( rGotoPathModel );
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Fix or Create Models in Prefabs/'rpakName'
            foreach ( string model in data.Data )
            {
                modelName = Path.GetFileNameWithoutExtension( model );
                string unityName = UnityInfo.GetUnityModelName( model );

                if ( !File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{data.Name}/{unityName}.prefab" ) )
                {
                    parent = Helper.CreateGameObject( unityName );
                    obj = Helper.CreateGameObject( "", $"{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx", parent );

                    if ( !Helper.IsValid( parent ) || !Helper.IsValid( obj ) )
                        continue;

                    parent.AddComponent< PropScript >();
                    parent.transform.position = Vector3.zero;
                    parent.transform.eulerAngles = Vector3.zero;

                    obj.transform.position = Vector3.zero;
                    obj.transform.eulerAngles = Models.FindAnglesOffset( model );
                    obj.transform.localScale = new Vector3(1, 1, 1);

                    parent.tag = Helper.GetObjTagNameWithEnum( ObjectType.Prop );

                    Models.CheckBoxColliderComponent( parent );

                    //AssetDatabase.SetLabels( ( UnityEngine.Object ) parent, new[] { model.Split( '/' )[1] } );

                    PrefabUtility.SaveAsPrefabAsset( parent, $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}.prefab" );

                    UnityEngine.Object.DestroyImmediate( parent );

                    ReMapConsole.Log( $"[Library Sorter] Created and saved prefab: {UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}", ReMapConsole.LogType.Info ); 
                }
                else if ( checkExist )
                {
                    parent = Helper.CreateGameObject( $"{UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}.prefab" );
                    obj = parent.GetComponentsInChildren< Transform >()[1].gameObject;
                    
                    if ( !Helper.IsValid( parent ) || !Helper.IsValid( obj ) )
                        continue;

                    parent.transform.position = Vector3.zero;
                    parent.transform.eulerAngles = Vector3.zero;
                    obj.transform.eulerAngles = Models.FindAnglesOffset( model );
                    obj.transform.position = Vector3.zero;

                    Models.CheckBoxColliderComponent( parent );

                    //AssetDatabase.SetLabels( ( UnityEngine.Object ) parent, new[] { model.Split( '/' )[1] } );

                    PrefabUtility.SavePrefabAsset( parent );

                    ReMapConsole.Log( $"[Library Sorter] Fixed and saved prefab: {UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}", ReMapConsole.LogType.Success );
                }

                // Update progress bar
                progress += 1.0f / max;
                EditorUtility.DisplayProgressBar( $"Sorting Folder ({min++}/{max})", $"Processing... {modelName}", progress );
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Set Scale 100 to .fbx files
            min = 0; max = modelImporter.Count; progress = 0.0f;
            foreach ( string modelPath in modelImporter )
            {
                ModelImporter importer = AssetImporter.GetAtPath( modelPath ) as ModelImporter;
                if ( Helper.IsValid( importer ) && importer.globalScale != 100 )
                {
                    importer.globalScale = 100;
                    importer.SaveAndReimport();
                }
                
                progress += 1.0f / max;
                EditorUtility.DisplayProgressBar( $"ReImport LOD0", $"Processing... ({min++}/{max})", progress );
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            data.Update = DateTime.UtcNow.ToString();
        }

        internal static Task SetModelLabels( string specificModelOrFolderOrnull = null )
        {
            string specificFolder = $"";
            string specificModel = $"mdl#";

            if ( Helper.IsValid( specificModelOrFolderOrnull ) )
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

        // We keep this func but don't use it lol
        internal static async Task RenameTextures()
        {
            string[] modeltextureGUID = AssetDatabase.FindAssets( "t:model", new [] { UnityInfo.relativePathModel } );

            float progress = 0.0f; int min = 0; int max = modeltextureGUID.Length;
            
            foreach ( var guid in modeltextureGUID )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                GameObject obj = Helper.CreateGameObject( "", assetPath );

                foreach ( Renderer renderer in obj.GetComponentsInChildren< Renderer >() )
                {
                    if ( Helper.IsValid( renderer ) )
                    {
                        foreach ( Material mat in renderer.sharedMaterials )
                        {
                            if ( Helper.IsValid( mat ) )
                            {
                                string name = mat.name;

                                // Obtain the name of the main map albedo
                                if( mat.HasProperty( "_MainTex" ) )
                                {
                                    Texture mainTexture = mat.mainTexture;

                                    string texturePath = AssetDatabase.GetAssetPath( mainTexture );

                                    if ( !string.IsNullOrEmpty( texturePath ) )
                                    {
                                        string fileName = Path.GetFileNameWithoutExtension( texturePath );

                                        if ( fileName.StartsWith( "0x" ) )
                                        {
                                            string newName = $"{name}_albedoTexture.dds";
                                            string dirToTexture = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}";

                                            if ( File.Exists( $"{dirToTexture}/{fileName}" ) && !File.Exists( $"{dirToTexture}/{newName}" ) )
                                            {
                                                System.IO.File.Move( $"{dirToTexture}/{fileName}", $"{dirToTexture}/{newName}" );
                                            }

                                            if ( File.Exists( $"{dirToTexture}/{fileName}.meta" ) && !File.Exists( $"{dirToTexture}/{newName}.meta" ) )
                                            {
                                                System.IO.File.Move( $"{dirToTexture}/{fileName}.meta", $"{dirToTexture}/{newName}.meta" );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                UnityEngine.Object.DestroyImmediate( obj );

                progress += 1.0f / max;

                EditorUtility.DisplayProgressBar( $"Renaming Textures", $"Processing... ({min++}/{max})", progress );
            }

            await Helper.Wait();

            AssetDatabase.Refresh();
        }

        internal static async Task CheckTextures( GameObject objScene = null )
        {
            if ( LegionExporting.GetValidRpakPaths() ) return;

            existingTextures = await GetAllTextures();

            missingTextures = new List< string >();

            for ( int i = 0 ; i < 2; i++ )
            {
                if ( i == 1 && !CheckDialog( $"Texture Checker", $"Do you want try to set missings materials ?" ) )
                {
                    break;
                }

                if ( i == 1 )
                {
                    existingTextures = await GetAllTextures();
                }

                if ( !Helper.IsValid( objScene ) )
                {
                    string[] modeltextureGUID = AssetDatabase.FindAssets( "t:prefab", new [] { $"{UnityInfo.relativePathPrefabs}" } );

                    int min = 0; int max = modeltextureGUID.Length; float progress = 0.0f;
                
                    foreach ( var guid in modeltextureGUID )
                    {
                        EditorUtility.DisplayProgressBar( $"Texture Checker", $"Checking Textures... ({min++}/{max})", progress );

                        string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                        GameObject obj = Helper.CreateGameObject( "", assetPath );

                        SetMaterialsToObject( obj, assetPath );

                        UnityEngine.Object.DestroyImmediate( obj );

                        progress += 1.0f / max;
                    }

                    AssetDatabase.Refresh();
                }
                else
                {
                    SetMaterialsToObject( objScene, AssetDatabase.GetAssetPath( PrefabUtility.GetCorrespondingObjectFromSource( objScene ) ) );
                }

                if ( i == 0 && CheckDialog( $"Texture Checker", $"{missingTextures.Count} Missing. Do you want extract them ?" ) )
                {
                    await ExportMissingTextures();
                }
            }

            EditorUtility.ClearProgressBar();
        }

        internal static async Task ExportMissingTextures()
        {
            if ( Helper.IsEmpty( missingTextures ) )
                return;

            // Legion Launch Arguments
            legionArgument = new StringBuilder();
            legionArguments = new List< string >();

            foreach ( string arg in missingTextures )
            {
                legionArgument.Append( $"{arg}," );

                // Executive launch args can't be more long than 8191 characters
                if ( legionArgument.Length > 5000 )
                {
                    // Remove last ','
                    legionArgument.Remove( legionArgument.Length - 1, 1 );

                    legionArguments.Add( legionArgument.ToString() );

                    legionArgument = new ();
                }
            }

            legionArgument.Remove( legionArgument.Length - 1, 1 );
            legionArguments.Add( legionArgument.ToString() );

            string loading = ""; int loadingCount = 0;
            int min = 1; int max = legionArguments.Count; float progress = 0.0f;

            foreach ( string argument in legionArguments )
            {
                Task legionTask = LegionExporting.ExtractModelFromLegion( argument );

                string count = max > 1 ? $" ({min}/{max})" : "";

                while ( !legionTask.IsCompleted )
                {
                    EditorUtility.DisplayProgressBar( $"Legion Extraction{count}", $"Extracting files{loading}", 0.0f );

                    loading = new string( '.', loadingCount++ % 4 );

                    await Helper.Wait( 1.0 );
                }

                foreach ( string folder in Directory.GetDirectories( $"{UnityInfo.currentDirectoryPath}/{relativeLegionPlusExportedFiles}/materials" ) )
                {
                    string name = Path.GetFileName( folder );

                    if ( !Helper.MoveFile( $"{folder}/{name}_albedoTexture.dds", $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}/{name}_albedoTexture.dds" ) )
                    {
                        
                    }

                    if ( missingTextures.Contains( name ) ) missingTextures.Remove( name );
                }

                min++;
            }

            min = 0; max = missingTextures.Count; progress = 0.0f;

            string dir;

            foreach ( string texture in missingTextures )
            {
                string filePath = $"{UnityInfo.currentDirectoryPath}/{relativeLegionPlusExportedFiles}/materials/{texture}/{texture}_albedoTexture.dds";
                string gotoPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}/{texture}_albedoTexture.dds";

                EditorUtility.DisplayProgressBar( $"Texture Sorter", $"Checking Textures... ({min++}/{max})", progress );

                progress += 1.0f / max;

                if ( !Helper.MoveFile( filePath, gotoPath, false ) )
                {
                    foreach ( string file in Directory.GetFiles( $"{UnityInfo.currentDirectoryPath}/{relativeLegionPlusExportedFiles}/materials/{texture}" ) )
                    {
                        string fileName = Path.GetFileName( file );

                        Helper.MoveFile( file, $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}/{fileName}", false );
                    }
                }

                dir = $"{relativeLegionPlusExportedFiles}/materials/{texture}";
                if ( Directory.Exists( dir ) )
                {
                    Directory.Delete( dir, true );

                    if ( File.Exists( $"{dir}.meta" ) )
                    {
                        File.Delete( $"{dir}.meta" );
                    }
                }

                min++;
            }

            dir = $"{UnityInfo.currentDirectoryPath}/{relativeLegionPlusExportedFiles}/materials";
            if ( Directory.Exists( dir ) )
            {
                Directory.Delete( dir, true );

                if ( File.Exists( $"{dir}.meta" ) )
                {
                    File.Delete( $"{dir}.meta" );
                }
            }

            await DeleteNotUsedTexture();
        }

        internal static async Task< Dictionary< string, Texture2D > > GetAllTextures()
        {
            Dictionary< string, Texture2D > dictionary = new Dictionary< string, Texture2D >();

            string[] modeltextureGUID = AssetDatabase.FindAssets( "t:model", new [] { UnityInfo.relativePathModel } );

            float progress = 0.0f; int min = 0; int max = modeltextureGUID.Length;
            
            foreach ( var guid in modeltextureGUID )
            {
                EditorUtility.DisplayProgressBar( $"Getting Textures", $"Processing... ({min}/{max})", progress );

                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                GameObject obj = Helper.CreateGameObject( "", assetPath );

                foreach ( Renderer renderer in obj.GetComponentsInChildren< Renderer >() )
                {
                    if ( !Helper.IsValid( renderer ) )
                        continue;

                    foreach ( Material mat in renderer.sharedMaterials )
                    {
                        if ( !Helper.IsValid( mat ) || !mat.HasProperty( "_MainTex" ) )
                            continue;

                        string name = mat.name;

                        if ( dictionary.ContainsKey( name ) )
                            continue;

                        // Obtain the name of the main map albedo
                        Texture mainTexture = mat.mainTexture;

                        string texturePath = AssetDatabase.GetAssetPath( mainTexture );

                        if ( !string.IsNullOrEmpty( texturePath ) )
                        {
                            Texture2D texture = AssetDatabase.LoadAssetAtPath< Texture2D >( texturePath );

                            dictionary.Add( name, texture );
                        }
                    }
                }

                UnityEngine.Object.DestroyImmediate( obj );

                progress += 1.0f / max; min++;
            }
            
            await Helper.Wait();

            return dictionary;
        }

        internal static void SetMaterialsToObject( GameObject obj, string assetPath )
        {
            foreach ( Renderer renderer in obj.GetComponentsInChildren< Renderer >() )
            {
                if ( Helper.IsValid( renderer ) )
                {
                    foreach ( Material mat in renderer.sharedMaterials )
                    {
                        if ( !Helper.IsValid( mat ) || !mat.HasProperty( "_MainTex" ) )
                            continue;
                            
                        string name = mat.name;

                        if ( existingTextures.ContainsKey( name ) )
                        {
                            mat.mainTexture = existingTextures[ name ];
                        }
                        else
                        {
                            missingTextures.Add( name );
                        }
                    }
                }
            }

            if ( !string.IsNullOrEmpty( assetPath ) )
            {
                PrefabUtility.SaveAsPrefabAsset( obj, assetPath );
            }
        }

        internal static async Task CreateMaterials()
        {
            string[] textureGUID = AssetDatabase.FindAssets( "t:texture", new [] { UnityInfo.relativePathMaterials } );

            foreach ( var guid in textureGUID )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guid );

                if ( !assetPath.Contains( "_albedoTexture" ) )
                    continue;

                string materialPath =  assetPath.Replace( ".dds", ".mat" );

                if ( File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}/{materialPath}" ) )
                    continue;

                Texture2D texture = AssetDatabase.LoadAssetAtPath< Texture2D >( assetPath );

                Material newMaterial = new Material( Shader.Find( "Standard" ) );

                newMaterial.mainTexture = texture;

                AssetDatabase.CreateAsset( newMaterial, materialPath );
            }

            await Helper.Wait();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static Task DeleteNotUsedTexture()
        {
            List< string > texturesList = new List< string >();

            string[] modeltextureGUID = AssetDatabase.FindAssets( "t:model", new [] { UnityInfo.relativePathModel, $"{UnityInfo.relativePathLods}/Developer_Lods" } );

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
                if ( Helper.IsValid( importer ) )
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
