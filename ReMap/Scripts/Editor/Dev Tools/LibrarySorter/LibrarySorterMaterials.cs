
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
    public class Materials
    {
        internal static MaterialData MaterialData;

        private static string MaterialDataJsonPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathTextureDataList}";

        internal static readonly string materialDirectory = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}";
        internal static readonly string extractedMaterialDirectory = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathLegionPlusExportedFiles}/materials";

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

            string jsonData = JsonUtility.ToJson( materialData );
            System.IO.File.WriteAllText( UnityInfo.relativePathTextureDataList, jsonData );
            
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

                await AppendNewTexture( missingMaterialList );

                //MoveMaterials( missingMaterialList );
            }

            Helper.DeleteDirectory( extractedMaterialDirectory, false );

            EditorUtility.ClearProgressBar();
        }

        internal static async Task AppendNewTexture( List< string > missingMaterialList )
        {
            foreach ( string materialDir in Directory.GetDirectories( extractedMaterialDirectory ) )
            {
                string materialName = Path.GetFileName( materialDir.Replace( "\\", "/" ) );

                if ( !missingMaterialList.Contains( materialName ) )
                    continue;

                string[] files = Directory.GetFiles( $"{materialDir.Replace( "\\", "/" )}" );

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
                else
                {
                    await MaterialWindow.ShowWindow( materialDir, materialName );
                }
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

        internal static async Task SetMaterialsToObject( GameObject obj )
        {
            if ( !Helper.IsValid( MaterialData ) ) MaterialData = GetMaterialData();

            List< string > missingTextures = new List< string >();

            foreach ( Renderer renderer in obj.GetComponentsInChildren< Renderer >() )
            {
                if ( Helper.IsValid( renderer ) )
                {
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
                            missingTextures.Add( name );
                        }
                    }
                }
            }

            string assetPath = AssetDatabase.GetAssetPath( PrefabUtility.GetCorrespondingObjectFromSource( obj ) );

            if ( !string.IsNullOrEmpty( assetPath ) )
            {
                PrefabUtility.SaveAsPrefabAsset( obj, assetPath );
            }

            if ( LibrarySorterWindow.CheckDialog( $"Texture Checker", $"{missingTextures.Count} Materials Missing. Do you want try to extract them ?" ) )
            {
                await ExtractMissingMaterials( missingTextures );
            }
        }

        internal static void SaveMaterialData()
        {
            string jsonData = JsonUtility.ToJson( MaterialData );
            System.IO.File.WriteAllText( UnityInfo.relativePathTextureDataList, jsonData );

            MaterialData = GetMaterialData();
        }

        internal static string GetAssetPath( string absolutePath )
        {
            absolutePath = absolutePath.Replace( "\\", "/" ).Split( "Assets/" )[1];
            return $"Assets/{absolutePath}";
        }
    }


    public class MaterialWindow : EditorWindow
    {
        static string materialName = "";
        static Dictionary< string, Texture2D > ddsTextures = new Dictionary< string, Texture2D >();
        static Vector2 scroll = Vector2.zero;
        static MaterialWindow windowInstance;

        public static async Task ShowWindow( string path, string matName )
        {
            materialName = matName;

            RefreshMaterialWindow( path );

            windowInstance = GetWindow< MaterialWindow >( "MaterialWindow" );

            scroll = Vector2.zero;

            while ( windowInstance != null && windowInstance )
            {
                await Helper.Wait( 1.0 );
            }
        }

        private void OnGUI()
        {
            WindowUtility.WindowUtility.CreateTextInfoCentered( $"\"{materialName}\" Albedo texture not found, please select the texture", "", 0, 20 );

            scroll = EditorGUILayout.BeginScrollView( scroll );

            GUILayout.BeginVertical();

            foreach ( var texture in ddsTextures )
            {
                string textureName = Path.GetFileName( texture.Key );

                Texture2D preview = AssetPreview.GetAssetPreview( texture.Value );

                int previewWidth, previewHeight;

                if ( Helper.IsValid( preview ) )
                {
                    previewWidth = preview.width;
                    previewHeight = preview.height;
                }
                else
                {
                    previewWidth = 200;
                    previewHeight = 200;
                }

                GUILayout.BeginHorizontal();

                if ( GUILayout.Button( textureName.Replace( ".dds", "" ), GUILayout.Height( previewHeight ) ) )
                {
                    if ( Helper.MoveFile( $"{UnityInfo.currentDirectoryPath}/{texture.Key.Replace( "\\", "/" )}", $"{Materials.materialDirectory}/{textureName}".Replace( "\\", "/" ), false ) )
                    {
                        Materials.MaterialData.MaterialList.Add
                        (
                            new MaterialClass()
                            {
                                Name = materialName,
                                Path = Materials.GetAssetPath( $"{Materials.materialDirectory}/{textureName}" )
                            }
                        );

                        windowInstance.Close();
                        windowInstance = null;

                        Materials.SaveMaterialData();
                    }
                }

                if ( Helper.IsValid( preview ) )
                {
                    GUILayout.Label( preview, GUILayout.Width( previewWidth ), GUILayout.Height( previewHeight ) );
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();

            WindowUtility.WindowUtility.FlexibleSpace();

            if ( GUILayout.Button( "Ignore Texture" ) )
            {
                windowInstance.Close();
                windowInstance = null;
            }

            WindowUtility.WindowUtility.Space( 4f );
        }

        private static void RefreshMaterialWindow( string path )
        {
            ddsTextures.Clear();

            if ( !Directory.Exists( path ) || string.IsNullOrEmpty( path ) )
                return;

            var info = new DirectoryInfo( path );
            var fileInfo = info.GetFiles();

            foreach ( var file in fileInfo )
            {
                if ( file.Extension.ToLower() == ".dds" )
                {
                    var assetPath = $"{Materials.GetAssetPath( path )}/{file.Name}";
                    var texture = AssetDatabase.LoadAssetAtPath< Texture2D >( assetPath );

                    if ( texture != null )
                    {
                        ddsTextures.Add( assetPath, texture );
                    }
                }
            }
        }
    }
}
