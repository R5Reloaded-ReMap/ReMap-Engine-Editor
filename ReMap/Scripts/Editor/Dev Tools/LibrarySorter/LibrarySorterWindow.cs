
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
                    await Models.ExtractMissingModels();
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
                    await Models.FixFolder( data );
                    await SetModelLabels( data.Name );
                    RpakManagerWindow.SaveJson();
                    break;

                case TaskType.FixAllPrefabsData:
                    if ( !DoStartTask() ) return;
                    CheckExisting();
                    foreach ( RpakData _data in libraryData.RpakList )
                    {
                        await Models.FixFolder( _data );
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
