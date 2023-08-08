
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

                Helper.Ping( modelName );

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

        internal static async Task FixFolder( RpakData data )
        {
            Helper.CreateDirectory( $"{UnityInfo.relativePathPrefabs}/{data.Name}" );

            GameObject parent; GameObject obj; string modelName; string unityName;

            float progress = 0.0f; int min = 0; int max = data.Data.Count;

            // Fix or Create Models in Prefabs/'rpakName'
            foreach ( string model in data.Data )
            {
                modelName = Path.GetFileNameWithoutExtension( model );
                unityName = UnityInfo.GetUnityModelName( model );

                string localModelPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}.prefab";
                string lodsModelPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx";
                string allModelsFolder = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/all_models/{unityName}.prefab";

                if ( !File.Exists( localModelPath ) && File.Exists( lodsModelPath ) )
                {
                    if ( Helper.CopyFile( allModelsFolder, localModelPath, false ) )
                        continue;

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
                else if ( LibrarySorterWindow.checkExist && File.Exists( localModelPath ) )
                {
                    parent = Helper.CreateGameObject( $"{UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}.prefab" );
                    obj = parent.GetComponentsInChildren< Transform >()[0].gameObject;
                    
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

            await Helper.Wait();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            data.Update = DateTime.UtcNow.ToString();
        }

        internal static Task FixPrefab( string prefabName )
        {
            string[] prefabs = AssetDatabase.FindAssets( prefabName, new [] { UnityInfo.relativePathPrefabs } );

            int i = 0; int total = prefabs.Length;

            string countInfo = total < 1 ? $"({i}/{total})" : "";

            foreach ( string prefab in prefabs )
            {
                string file = AssetDatabase.GUIDToAssetPath( prefab );
                string rpakName = UnityInfo.GetApexModelName( prefabName, true );

                GameObject obj = Helper.CreateGameObject( "", file );

                EditorUtility.DisplayProgressBar( $"Fixing Prefabs {countInfo}", $"Prefab: {rpakName} ({prefab})", ( i + 1 ) / ( float ) total );

                Transform child = obj.GetComponentsInChildren< Transform >()[1];

                CheckBoxColliderComponent( obj );

                obj.transform.position = Vector3.zero;
                obj.transform.eulerAngles = Vector3.zero;
                child.transform.eulerAngles = FindAnglesOffset( rpakName );
                child.transform.position = Vector3.zero;

                PrefabUtility.SaveAsPrefabAsset( obj, file );

                UnityEngine.Object.DestroyImmediate( obj );

                ReMapConsole.Log( $"[Library Sorter] Fixed and saved prefab: {file}", ReMapConsole.LogType.Success ); i++;
            }

            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }


        private static bool HasValidName( string name )
        {
            if ( name.StartsWith( "ptpov_" ) )
                return false;

            if ( name.StartsWith( "pov_" ) )
                return false;

            if ( string.IsNullOrEmpty( name ) )
                return false;

            return true;
        }

        internal static void CheckBoxColliderComponent( GameObject go )
        {
            BoxCollider collider = go.GetComponent< BoxCollider >();

            if ( collider == null )
            {
                go.AddComponent< BoxCollider >();
                collider = go.GetComponent< BoxCollider >();
            }

            float x = 0, y = 0, z = 0;

            foreach( Renderer o in go.GetComponentsInChildren< Renderer >() )
            {
                if( o.bounds.size.x > x ) x = o.bounds.size.x;

                if( o.bounds.size.y > y ) y = o.bounds.size.y;

                if( o.bounds.size.z > z ) z = o.bounds.size.z;
            }

            collider.size = new Vector3( x, y, z );
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

                foreach ( string texture in Directory.GetFiles( $"{modelDir}/_images" ) )
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
