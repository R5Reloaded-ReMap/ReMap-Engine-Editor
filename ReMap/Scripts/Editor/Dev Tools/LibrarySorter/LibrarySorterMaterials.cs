
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
            if ( !LegionExporting.GetValidRpakPaths() )
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

                Helper.DeleteDirectory( extractedMaterialDirectory, false, false );
            }

            Helper.DeleteDirectory( extractedMaterialDirectory, false, false );

            EditorUtility.ClearProgressBar();
        }

        internal static async Task AppendNewTexture( List< string > missingMaterialList, bool byPassCheck = false )
        {
            string[] directories = Directory.GetDirectories( extractedMaterialDirectory );

            int i = 0; int j = directories.Length;

            //await CheckDXT1Format( directories );

            await TextureConverter.ResizeTextures( $"{UnityInfo.relativePathLegionPlusExportedFiles}/materials" );

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

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

            while ( !MaterialWindowSelector.MaterialListIsEmpty() )
            {
                await Helper.Wait( 1 );
            }
        }

        internal static async Task CheckDXT1Format( string[] directories )
        {
            int dirIdx = 0; 
            int dirCount = directories.Length; 
            float progress = 0.0f;

            var semaphore = new SemaphoreSlim( 16 );

            foreach ( string path in directories )
            {
                var info = new DirectoryInfo( path );
                var fileInfo = info.GetFiles();

                EditorUtility.DisplayProgressBar( $"DXT1 Format Checker", $"Patching DDS Files => ({dirIdx}/{dirCount})", progress );

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

            Texture2D textureDefault = AssetDatabase.LoadAssetAtPath< Texture2D >( MaterialData.GetPath( "dev_gray_512" ) );

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

            if ( reset || missingTextures.Count == 0 )
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
}
