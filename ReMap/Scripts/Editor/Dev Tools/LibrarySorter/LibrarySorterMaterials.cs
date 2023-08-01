
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class Materials
    {
        internal static MaterialData MaterialData = GetMaterialData();

        private static string MaterialDataJsonPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathTextureDataList}";

        internal static readonly string materialDirectory = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}";
        internal static readonly string extractedMaterialDirectory = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathLegionPlusExportedFiles}/materials";

        internal static bool checkNonAlbedoTexture = false;

        internal static MaterialData GetMaterialData()
        {
            MaterialData materialData;

            if ( !File.Exists( MaterialDataJsonPath ) )
            {
                materialData = new MaterialData();
                materialData.MaterialList = new List< MaterialClass >();
            }
            else
            {
                string jsonData = System.IO.File.ReadAllText( MaterialDataJsonPath );
                materialData = JsonUtility.FromJson< MaterialData >( jsonData );
            }

            return materialData;
        }

        internal static async Task CreateMaterialData()
        {
            MaterialData materialData = new MaterialData();
            materialData.MaterialList = new List< MaterialClass >();

            string[] modeltextureGUID = AssetDatabase.FindAssets( "t:model", new [] { UnityInfo.relativePathModel, UnityInfo.relativePathDevLods } );

            int min = 1; int max = modeltextureGUID.Length; float progress = 0.0f;
            int rmin = 0; int rmax = 1;
            
            foreach ( var guid in modeltextureGUID )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                GameObject obj = Helper.CreateGameObject( "", assetPath );

                EditorUtility.DisplayProgressBar( $"MaterialData ({min}/{max})", $"Processing... {obj.name} ({rmin}/{rmax})", progress );

                Renderer[] renderers = obj.GetComponentsInChildren< Renderer >(); rmax = renderers.Length;

                rmin = 0;

                foreach ( Renderer renderer in renderers )
                {
                    rmin++;
                    
                    if ( !Helper.IsValid( renderer ) )
                        continue; 
                     

                    foreach ( Material mat in renderer.sharedMaterials )
                    {
                        if ( !Helper.IsValid( mat ) || !mat.HasProperty( "_MainTex" ) )
                            continue;

                        string name = mat.name;

                        if ( materialData.MaterialList.Any( material => material.Name == name ) )
                            continue;

                        string texturePath = AssetDatabase.GetAssetPath( mat.mainTexture );

                        if ( !string.IsNullOrEmpty( texturePath ) )
                        {
                            materialData.MaterialList.Add
                            (
                                new MaterialClass()
                                {
                                    Name = name,
                                    Path = texturePath
                                }
                            );
                        }
                    }
                }

                UnityEngine.Object.DestroyImmediate( obj );

                progress += 1.0f / max; min++;
            }

            EditorUtility.ClearProgressBar();

            Helper.CreateDirectory( UnityInfo.relativePathTextureData );

            SaveMaterialData( materialData );
            
            await Helper.Wait();
        }

        internal static async Task ExtractMissingMaterials( List< string > missingMaterialList )
        {
            if ( LegionExporting.GetValidRpakPaths() )
            {
                Helper.Ping( "No valid path for LegionPlus." );
                return;
            }

            StringBuilder legionArgument = new StringBuilder();
            List< string > legionArguments = new List< string >();

            foreach ( string textureName in missingMaterialList )
            {
                legionArgument.Append( $"{textureName}," );

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
            int min = 1; int max = legionArguments.Count;

            foreach ( string argument in legionArguments )
            {
                Task legionTask = LegionExporting.ExtractModelFromLegion( argument );

                string countInfo = max > 1 ? $" ({min}/{max})" : "";

                while ( !legionTask.IsCompleted )
                {
                    EditorUtility.DisplayProgressBar( $"Legion Extraction{countInfo}", $"Extracting files{loading}", 0.0f );

                    loading = new string( '.', loadingCount++ % 4 );

                    await Helper.Wait( 1.0 );
                }

                min++;

                EditorUtility.ClearProgressBar();

                AppendNewTexture( missingMaterialList );

                Helper.DeleteDirectory( extractedMaterialDirectory, false, false );

                //MoveMaterials( missingMaterialList );
            }

            Helper.DeleteDirectory( extractedMaterialDirectory, false, false );

            EditorUtility.ClearProgressBar();
        }

        internal static async void AppendNewTexture( List< string > missingMaterialList, bool byPassCheck = false )
        {
            string[] directories = Directory.GetDirectories( extractedMaterialDirectory );

            int i = 0; int j = directories.Length;

            await CheckDXT1Format( directories );

            foreach ( string materialDir in directories )
            {
                string rmaterialDir = materialDir.Replace( "\\", "/" );

                string materialName = Path.GetFileName( rmaterialDir ); i++;

                if ( !missingMaterialList.Contains( materialName ) )
                    continue;

                string[] files = Directory.GetFiles( $"{rmaterialDir}" );

                var fileToMove = files.FirstOrDefault( f => f.Contains( "_albedoTexture" ) || f.Contains( "_albedo2Texture" ) );

                if ( Helper.IsValid( fileToMove ) )
                {
                    if ( Helper.MoveFile( fileToMove, $"{materialDirectory}/{Path.GetFileName( fileToMove )}", false ) )
                    {
                        MaterialData.MaterialList.Add
                        (
                            new MaterialClass()
                            {
                                Name = materialName,
                                Path = GetAssetPath( $"{materialDirectory}/{Path.GetFileName( fileToMove )}" )
                            }
                        );

                        SaveMaterialData();
                    }
                }
                else if ( checkNonAlbedoTexture || byPassCheck )
                {
                    MaterialWindowSelector.AddNewMaterialSelection( rmaterialDir );
                }
            }
        }

        internal static async Task CheckDXT1Format( string[] directories )
        {
            int dirIdx = 0; 
            int dirCount = directories.Length; 
            float progress = 0.0f;

            var semaphore = new SemaphoreSlim( 10 );

            foreach ( string path in directories )
            {
                var info = new DirectoryInfo( path );
                var fileInfo = info.GetFiles();

                EditorUtility.DisplayProgressBar( $"DXT1 Format Checker", $"Patching DDS Files => ({dirIdx}/{dirCount})", 0.0f );

                var tasks = fileInfo.Select( file => ProcessFileWithSemaphore( file, path, semaphore ) ).ToArray();

                await Task.WhenAll( tasks );

                dirIdx++; 
                progress += 1.0f / dirCount;
            }

            EditorUtility.ClearProgressBar();
        }

        private static async Task ProcessFileWithSemaphore( FileInfo file, string path, SemaphoreSlim semaphore )
        {
            await semaphore.WaitAsync();

            try
            {
                if ( file.Extension.ToLower() == ".dds" && !TextureConverter.IsDXT1( $"{path}/{file.Name}" ) )
                {
                    await TextureConverter.ConvertDDSToDXT1( $"{path}/{file.Name}" );
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        internal static void MoveMaterials( List< string > missingMaterialList )
        {
            foreach ( string materialDir in Directory.GetDirectories( extractedMaterialDirectory ) )
            {
                string materialName = Path.GetFileName( materialDir.Replace( "\\", "/" ) );

                if ( !missingMaterialList.Contains( materialName ) )
                    continue;

                foreach ( string texture in Directory.GetFiles( $"{materialDir.Replace( "\\", "/" )}" ) )
                {
                    string textureName = Path.GetFileName( texture );

                    if ( textureName.Contains( "0x" ) || textureName.Contains( "_albedoTexture" ) )
                    {
                        Helper.MoveFile( texture, $"{materialDirectory}/{textureName}", false );
                    }
                }
            }
        }

        internal static async Task SetMaterialsToObject( GameObject obj, bool reset = false )
        {
            MaterialData = GetMaterialData();

            List< string > missingTextures = new List< string >();

            Renderer[] renderers = obj.GetComponentsInChildren< Renderer >();

            Texture2D textureDefault = AssetDatabase.LoadAssetAtPath< Texture2D >( MaterialData.GetPath( "dev_grey_512" ) );

            int min = 1; int max = renderers.Length; float progress = 0.0f;

            foreach ( Renderer renderer in renderers )
            {
                if ( Helper.IsValid( renderer ) )
                {
                    EditorUtility.DisplayProgressBar( $"Material Setter", $"Processing... ( {renderer.name} => {min}/{max} )", progress );

                    foreach ( Material mat in renderer.sharedMaterials )
                    {
                        if ( !Helper.IsValid( mat ) || !mat.HasProperty( "_MainTex" ) )
                            continue;
                            
                        string name = mat.name;

                        if ( MaterialData.ContainsName( name ) )
                        {
                            mat.mainTexture = AssetDatabase.LoadAssetAtPath< Texture2D >( MaterialData.GetPath( name ) );
                        }
                        else if ( !missingTextures.Contains( name ) )
                        {
                            mat.mainTexture = textureDefault;

                            missingTextures.Add( name );
                        }
                    }
                }

                min++; progress += 1.0f / max;
            }

            string assetPath = AssetDatabase.GetAssetPath( PrefabUtility.GetCorrespondingObjectFromSource( obj ) );

            if ( !string.IsNullOrEmpty( assetPath ) && Path.GetExtension( assetPath ) == ".prefab" )
            {
                EditorUtility.DisplayProgressBar( $"Saving Prefab", $"Processing...", progress );

                PrefabUtility.SaveAsPrefabAsset( obj, assetPath );
            }

            EditorUtility.ClearProgressBar();

            if ( reset )
                return;

            if ( LibrarySorterWindow.CheckDialog( $"Texture Checker", $"{missingTextures.Count} Materials Missing. Do you want try to extract them ?" ) )
            {
                checkNonAlbedoTexture = LibrarySorterWindow.CheckDialog( $"Texture Checker", $"Do you want to manually choose the textures not found or ignore them ?" );

                bool resetTextures = LibrarySorterWindow.CheckDialog( $"Texture Checker", $"Do you want apply textures once extracted ?" );

                await ExtractMissingMaterials( missingTextures );

                if ( resetTextures )
                {
                    ReSetMaterialsToObject( obj );
                }
            }
        }

        internal static async void ReSetMaterialsToObject( GameObject obj )
        {
            await SetMaterialsToObject( obj, true );
        }

        internal static void SaveMaterialData( MaterialData data = null )
        {
            MaterialData materialData = Helper.IsValid( data ) ? data : MaterialData;

            UnityInfo.SortListByKey( materialData.MaterialList, x => x.Name );

            string jsonData = JsonUtility.ToJson( materialData );
            System.IO.File.WriteAllText( UnityInfo.relativePathTextureDataList, jsonData );

            MaterialData = GetMaterialData();
        }

        internal static string GetAssetPath( string absolutePath )
        {
            absolutePath = absolutePath.Replace( "\\", "/" ).Split( "Assets/" )[1];
            return $"Assets/{absolutePath}";
        }
    }


    public class MaterialWindowSelector : EditorWindow
    {
        private static MaterialWindowSelector windowInstance;
        private static Dictionary< string, List< Texture2D > > materialList = new Dictionary< string, List< Texture2D > >();
        private static Vector2 scroll = Vector2.zero;
        private static int scale = 256;

        public static void AddNewMaterialSelection( string directoryPath )
        {
            if ( !Helper.IsValid( materialList ) )
            {
                materialList = new Dictionary< string, List< Texture2D > >();
            }

            if ( !materialList.ContainsKey( directoryPath ) )
            {
                materialList.Add( directoryPath, GetTexturesInPath( directoryPath ) );
            }

            if ( !Helper.IsValid( windowInstance ) )
            {
                windowInstance = GetWindow< MaterialWindowSelector >( "Material Selector Window" );
                windowInstance.minSize = new Vector2( 1060, 800 );
                windowInstance.maxSize = new Vector2( 1060, 800 );
            }
        }

        private static List< Texture2D > GetTexturesInPath( string path )
        {
            List< Texture2D > list = new List< Texture2D >();

            var info = new DirectoryInfo( path );
            var fileInfo = info.GetFiles();

            foreach ( var file in fileInfo )
            {
                if ( file.Extension.ToLower() != ".dds" )
                    continue;

                var assetPath = $"{Materials.GetAssetPath( path )}/{file.Name}";
                Texture2D texture = AssetDatabase.LoadAssetAtPath< Texture2D >( assetPath );

                if ( !Helper.IsValid( texture ) )
                    continue;

                list.Add( texture );
            }

            return list;
        }

        private void OnGUI()
        {
            int remainingTexture = materialList.Count;

            GUILayout.BeginVertical();

            WindowUtility.WindowUtility.CreateTextInfoCentered( $"Model Queued: {remainingTexture}" );

            WindowUtility.WindowUtility.GetEditorWindowSize( windowInstance );

            scroll = EditorGUILayout.BeginScrollView( scroll );

            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            int loopLimit = Math.Min( remainingTexture, 10 ); // Show only 10 maximum textures settings for performances

            for ( int i = 0; i < loopLimit; i++ )
            {
                string path = materialList.Keys.ElementAt( i );

                string textureName = Path.GetFileNameWithoutExtension( path );

                int idx = 0;
                GUILayout.BeginVertical( "box" );

                WindowUtility.WindowUtility.CreateTextInfoCentered( $"{textureName}" );

                GUILayout.BeginHorizontal();

                foreach ( Texture2D texture in materialList[path] )
                {
                    if ( idx % 4 == 0 )
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }

                    GUIContent buttonContent = new GUIContent( texture );

                    if ( GUILayout.Button( buttonContent, GUILayout.Width( scale ), GUILayout.Height( scale ) ) )
                    {
                        if ( Event.current.button == 1 ) // Right click
                        {
                            MaterialManagerPreview.ShowPreview( texture );
                        }
                        else if ( Event.current.button == 0 ) // Left click
                        {
                            string filePath = $"{UnityInfo.currentDirectoryPath}/{AssetDatabase.GetAssetPath( texture )}";
                            string fileName = Path.GetFileName( filePath );

                            // #TOFIX
                            if ( Helper.MoveFile( filePath, $"{Materials.materialDirectory}/{fileName}", false ) )
                            {
                                Materials.MaterialData.RemoveMaterial( textureName );

                                Materials.MaterialData.Add
                                (
                                    new MaterialClass()
                                    {
                                        Name = textureName,
                                        Path = Materials.GetAssetPath( $"{Materials.materialDirectory}/{fileName}" )
                                    }
                                );

                                Materials.SaveMaterialData();

                                GUILayout.EndHorizontal();
                                GUILayout.EndVertical();
                                EditorGUILayout.EndScrollView();
                                GUILayout.EndVertical();

                                materialList.Remove( path );

                                MaterialManagerWindow.Refresh();

                                //Helper.DeleteDirectory( $"{Path.GetDirectoryName( path ).Replace( "\\", "/" )}/{textureName}" );

                                return;
                            }
                        }
                    }

                    idx++;
                }

                GUILayout.EndHorizontal();

                if ( GUILayout.Button( "Ignore Texture", buttonStyle ) )
                {
                    materialList.Remove( path );
                }

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
    }


    public class MaterialManagerWindow : EditorWindow
    {
        static MaterialData materialData = Materials.GetMaterialData();
        static Dictionary< string, Texture2D > texturesDictionary = new Dictionary< string, Texture2D >();
        static Vector2 scroll = Vector2.zero;
        
        static int scale = 256;
        public static void Init()
        {
            GetWindow< MaterialManagerWindow >( "Material Manager" );
        }

        void OnEnable()
        {
            Refresh();
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView( scroll );

            GUILayout.BeginVertical();

            int i = 0;

            foreach ( var texture in texturesDictionary )
            {
                if ( i == 40 )
                {
                    GUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();
                    return;
                }

                GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
                buttonStyle.alignment = TextAnchor.MiddleCenter;

                GUIContent buttonContent = new GUIContent( texture.Value );

                GUILayout.BeginVertical( "box" );

                string path = AssetDatabase.GetAssetPath( texture.Value );

                WindowUtility.WindowUtility.CreateTextInfoCentered( $"{texture.Key} => {Path.GetFileNameWithoutExtension( path )}", "", 0, 20 );

                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();

                if ( GUILayout.Button( $"Remove {texture.Key}", buttonStyle, GUILayout.Height( scale / 2 ) ) )
                {
                    materialData.RemoveMaterial( texture.Key );
                    Materials.SaveMaterialData( materialData );
                    Refresh();

                    Helper.DeleteFile( path );

                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    return;
                }

                if ( GUILayout.Button( $"ReImport {texture.Key}", buttonStyle, GUILayout.Height( scale / 2 ) ) )
                {
                    ReImportMaterial( texture.Key );
                }

                GUILayout.EndVertical();

                if ( GUILayout.Button( buttonContent, GUILayout.Width( scale ), GUILayout.Height( scale ) ) )
                {
                    MaterialManagerPreview.ShowPreview( texture.Value );
                }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal(); i++;
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private static async void ReImportMaterial( string materialName )
        {
            if ( LegionExporting.GetValidRpakPaths() )
            {
                Helper.Ping( "No valid path for LegionPlus." );
                return;
            }

            List< string > singleMaterialList = new List< string > { materialName };

            // #TOFIX
            //await LegionExporting.ExtractModelFromLegion( materialName );

            Materials.AppendNewTexture( singleMaterialList, true );
        }

        internal static void Refresh()
        {
            texturesDictionary = LoadTextures();
        }

        private static Dictionary< string, Texture2D > LoadTextures()
        {
            Dictionary< string, Texture2D > textureToLoad = new Dictionary< string, Texture2D >();

            materialData = Materials.GetMaterialData();

            foreach ( MaterialClass material in materialData.MaterialList )
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath< Texture2D >( material.Path );

                if ( !Helper.IsValid( texture ) || textureToLoad.ContainsKey( material.Name ) )
                    continue;

                textureToLoad.Add( material.Name, texture );
            }

            return textureToLoad;
        }
    }

    public class MaterialManagerPreview : EditorWindow
    {
        public static MaterialManagerPreview window;
        public static Texture2D texturePreview;

        public static void ShowPreview( Texture2D texture )
        {
            window = GetWindow< MaterialManagerPreview >( "Material Preview" );

            texturePreview = texture;

            // Set the max size of the window based on the texture size
            window.maxSize = new Vector2( texture.width, texture.height );
        }

        private void OnGUI()
        {
            if( Helper.IsValid( texturePreview ) )
            {
                // Create a GUIStyle that aligns content in the center
                GUIStyle centeredStyle = GUI.skin.GetStyle( "Label" );
                centeredStyle.alignment = TextAnchor.MiddleCenter;

                // Get the window size
                Vector2 windowSize = new Vector2( position.width, position.height );
            
                // Create a GUILayoutOption array with window size
                GUILayoutOption[] options = new GUILayoutOption[]
                {
                    GUILayout.Width( windowSize.x ),
                    GUILayout.Height( windowSize.y )
                };

                GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    // Draw the texture using the centeredStyle and options
                    GUILayout.Label( texturePreview, centeredStyle, options );
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
}
