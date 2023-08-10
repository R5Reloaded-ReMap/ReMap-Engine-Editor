
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
    public class Models
    {
        internal static readonly string modelDirectory = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}";
        internal static readonly string extractedModelDirectory = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathLegionPlusExportedFiles}/models";

        internal static async Task ExtractMissingModels()
        {
            if ( !LegionExporting.GetValidRpakPaths() )
            {
                Helper.Ping( "No valid path for LegionPlus." );
                return;
            }

            Materials.MaterialData = Materials.GetMaterialData();

            Dictionary< string, string > missingModelList = new Dictionary< string, string >();

            foreach ( string modelName in RpakManagerWindow.libraryData.GetAllModelsList() )
            {
                string lod0Name = Path.GetFileNameWithoutExtension( modelName );

                if ( Helper.LOD0_Exist( lod0Name ) )
                    continue;
                
                if ( !HasValidName( lod0Name ) )
                    continue;

                if ( missingModelList.ContainsKey( modelName ) )
                    continue;

                missingModelList.Add( modelName, lod0Name );
            }

            StringBuilder legionArgument = new StringBuilder();
            List< string > legionArguments = new List< string >();

            foreach ( string modelsName in missingModelList.Values )
            {
                legionArgument.Append( $"{modelsName}," );

                if ( legionArgument.Length > 5000 )
                {
                    // Remove last ','
                    legionArgument.Remove( legionArgument.Length - 1, 1 );

                    legionArguments.Add( legionArgument.ToString() );

                    legionArgument = new ();
                }
            }

            if ( legionArgument.Length > 1 )
            {
                legionArgument.Remove( legionArgument.Length - 1, 1 );
                legionArguments.Add( legionArgument.ToString() );
            }

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

                await MoveModels( missingModelList );

                min++;
            }

            Helper.DeleteDirectory( extractedModelDirectory, false, false );

            // Set Scale 100 to .fbx files
            min = 0; max = missingModelList.Count; float progress = 0.0f;
            foreach ( string modelName in missingModelList.Values )
            {
                string modelPath = $"{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx";
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
        }

        internal static async Task FixFolders( List< RpakData > rpaks )
        {
            foreach ( RpakData rpak in rpaks )
            {
                await FixFolder( rpak );
            }
        }

        internal static async Task FixFolder( RpakData rpak )
        {
            Helper.CreateDirectory( $"{UnityInfo.relativePathPrefabs}/{rpak.Name}" );

            int min = 0; int max = rpak.Data.Count; float progress = 0.0f; 

            // Fix or Create Models in Prefabs/'rpakName'
            foreach ( string apexName in rpak.Data )
            {
                if ( min == 30 )
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
                string prefabName = UnityInfo.GetUnityModelName( apexName );

                EditorUtility.DisplayProgressBar( $"Sorting '{rpak.Name}' Folder ({min++}/{max})", $"Processing... '{prefabName}'", progress );

                await FixPrefab( rpak, prefabName, LibrarySorterWindow.checkExist );
                
                progress += 1.0f / max;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            rpak.UpdateTime();
        }

        internal static async Task FixPrefab( RpakData rpak, string prefabName, bool checkExist = true )
        {
            string lodsName = prefabName.Split( '#' )[^1];
            string apexName = UnityInfo.GetApexModelName( prefabName, true );

            if ( !Helper.LOD0_Exist( lodsName ) || !rpak.Contains( apexName ) )
                return;

            await TryFix( rpak, prefabName, apexName, checkExist );
        }   

        internal static async Task FixPrefabs( string prefabName, bool checkExist = true )
        {
            string lodsName = prefabName.Split( '#' )[^1];
            string apexName = UnityInfo.GetApexModelName( prefabName, true );

            List< RpakData > rpaks = RpakManagerWindow.libraryData.RpakContains( apexName );

            if ( !Helper.LOD0_Exist( lodsName ) )
                return;

            List< Task > fixObjects = new List< Task >();

            foreach ( RpakData rpak in rpaks )
            {
                fixObjects.Add( TryFix( rpak, prefabName, apexName, checkExist ) );
            }

            await Task.WhenAll( fixObjects );
        }

        private static Task TryFix( RpakData rpak, string prefabName, string apexName, bool checkExist = true )
        {
            string filePath = $"{UnityInfo.relativePathPrefabs}/{rpak.Name}/{prefabName}.prefab";

            if ( File.Exists( $"{UnityInfo.currentDirectoryPath}/{filePath}" ) && checkExist )
            {
                UpdatePrefab( apexName, filePath );
            }
            else if ( !TryCopyFromAllModels( rpak, apexName ) )
            {
                CreatePrefab( apexName, filePath );
            }

            return Task.CompletedTask;
        }

        private static void CreatePrefab( string apexName, string filePath )
        {
            string unityName = UnityInfo.GetUnityModelName( apexName );
            string modelName = Path.GetFileNameWithoutExtension( apexName );

            GameObject prefab = Helper.CreateGameObject( unityName );

            if ( !Helper.IsValid( prefab ) )
                return;

            GameObject obj = Helper.CreateGameObject( "", $"{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx", prefab );

            if ( !Helper.IsValid( obj ) )
            {
                UnityEngine.Object.DestroyImmediate( prefab );
                return;
            } 

            prefab.AddComponent< PropScript >();
            prefab.transform.position = Vector3.zero;
            prefab.transform.eulerAngles = Vector3.zero;
            prefab.tag = Helper.GetObjTagNameWithEnum( ObjectType.Prop );

            CheckBoxColliderComponent( prefab );

            obj.transform.position = Vector3.zero;
            obj.transform.eulerAngles = Models.FindAnglesOffset( apexName );
            obj.transform.localScale = new Vector3( 1, 1, 1 );

            AssetDatabase.SetLabels( ( UnityEngine.Object ) prefab, new[] { apexName.Split( '/' )[1].ToLower() } );

            PrefabUtility.SaveAsPrefabAsset( prefab, $"{UnityInfo.currentDirectoryPath}/{filePath}" );

            UnityEngine.Object.DestroyImmediate( prefab );
        }

        private static void UpdatePrefab( string apexName, string path )
        {
            GameObject obj = Helper.CreateGameObject( "", path );

            obj.transform.position = Vector3.zero;
            obj.transform.eulerAngles = Vector3.zero;

            // [0] => Get 'obj', [1] => Get '..._LOD0'
            Transform child = obj.GetComponentsInChildren< Transform >()[1];

            CheckBoxColliderComponent( obj );

            child.transform.position = Vector3.zero;
            child.transform.eulerAngles = FindAnglesOffset( apexName );
            child.transform.localScale = new Vector3( 1, 1, 1 );

            AssetDatabase.SetLabels( ( UnityEngine.Object ) obj, new[] { apexName.Split( '/' )[1].ToLower() } );
            PrefabUtility.SaveAsPrefabAsset( obj, path );

            UnityEngine.Object.DestroyImmediate( obj );
        }

        private static bool TryCopyFromAllModels( RpakData rpak, string prefabName )
        {
            string targetPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{rpak.Name}/{prefabName}.prefab";

            string allModelsPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{RpakManagerWindow.allModelsDataName}/{prefabName}.prefab";
            string allModelsRetailPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{RpakManagerWindow.allModelsRetailDataName}/{prefabName}.prefab";

            if ( Helper.CopyFile( allModelsPath, targetPath, false ) )
                return true;

            if ( Helper.CopyFile( allModelsRetailPath, targetPath, false ) )
                return true;

            return false;
        }

        internal static bool HasValidName( string name )
        {
            if ( name.StartsWith( "ptpov_" ) )
                return false;

            if ( name.StartsWith( "pov_" ) )
                return false;

            if ( string.IsNullOrEmpty( name ) )
                return false;

            return true;
        }

        internal static void RemoveBoxColliderComponent( GameObject go )
        {
            BoxCollider[] colliders = go.GetComponentsInChildren< BoxCollider  >();

            foreach ( BoxCollider coll in colliders )
            {
                UnityEngine.Object.DestroyImmediate( coll );
            }
        }

        internal static void CheckBoxColliderComponent( GameObject go )
        {
            RemoveBoxColliderComponent( go );

            SkinnedMeshRenderer[] renderers = go.GetComponentsInChildren< SkinnedMeshRenderer >();

            foreach ( var renderer in renderers )
            {
                if ( renderer.sharedMesh != null )
                {
                    Vector3 rBoundsCenter = renderer.bounds.center;
                    Vector3 rBoundsSize = renderer.bounds.size;

                    BoxCollider box = renderer.transform.parent.gameObject.AddComponent< BoxCollider >();
                    box.center = new Vector3( rBoundsCenter.z, rBoundsCenter.x, rBoundsCenter.y );
                    box.size = new Vector3( rBoundsSize.z, rBoundsSize.x, rBoundsSize.y );
                }
            }
        }

        public static Vector3 FindAnglesOffset( string searchTerm )
        {
            Vector3 returnedVector = new Vector3( 0, -90, 0 );

            PrefabOffset offset = OffsetManagerWindow.FindPrefabOffsetFile().Find( o => o.ModelName == searchTerm );
            if ( Helper.IsValid( offset ) )
            {
                returnedVector = offset.Rotation;
                ReMapConsole.Log( $"[Library Sorter] Angle override found for {searchTerm}, setting angles to: {returnedVector}", ReMapConsole.LogType.Info );
            }

            return returnedVector;
        }

        internal static async Task MoveModels( Dictionary< string, string > checker )
        {
            foreach ( string modelDir in Directory.GetDirectories( extractedModelDirectory ) )
            {
                string modelName = Path.GetFileName( modelDir );

                EditorUtility.DisplayProgressBar( $"Legion Extraction", $"Moving File '{modelDir}'", 0.0f );

                if ( !checker.ContainsValue( modelName ) )
                    continue;

                Helper.MoveFile( $"{modelDir}/{modelName}_LOD0.fbx", $"{modelDirectory}/{modelName}_LOD0.fbx", false );

                string [] filesName = Directory.GetFiles( $"{modelDir}/_images" );

                if ( Materials.MaterialData.ContainsFilePath( filesName ) )
                    continue;

                foreach ( string texture in filesName )
                {
                    string textureName = Path.GetFileName( texture );

                    if ( !textureName.Contains( "0x" ) && !textureName.Contains( "_albedoTexture" ) )
                        continue;

                    Helper.MoveFile( texture, $"{Materials.materialDirectory}/{textureName}", false );
                }

                Directory.Delete( modelDir, true );
            }

            await Helper.Wait();
        }
    }
}
