using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class AssetLibrarySorter
{
    public static void LibrarySorter()
    {
        int totalFiles = 0;
        Array[] totalArrayMap = new Array[20];
        string currentDirectory = Directory.GetCurrentDirectory();
        string[] files = Directory.GetFiles(Path.Combine(currentDirectory, @"Assets\ReMap\Scripts\Tools", "rpakModelFile"), "*.txt");
        foreach (string file in files)
        {
            int lineCount = File.ReadAllLines(file).Length;

            string[] arrayMap = new string[lineCount];

            string mapPath = file;
            string mapName = Path.GetFileNameWithoutExtension(file);

            int row = 0;

            using (StreamReader reader = new StreamReader(mapPath))
            {
                string modelPath;
                while ((modelPath = reader.ReadLine()) != null)
                {
                    arrayMap[row] = modelPath;
                    row++;
                }
            }

            totalArrayMap[totalFiles] = arrayMap;
            totalFiles++;

            if ( !Directory.Exists( Path.Combine(currentDirectory, @"Assets\Prefabs", mapName ) ) )
                Directory.CreateDirectory( Path.Combine(currentDirectory, @"Assets\Prefabs", mapName ) );

            string modelName ; string modelReplacePath; string category;
            GameObject prefabToAdd; GameObject prefabInstance;
            GameObject objectToAdd; GameObject objectInstance;

            foreach ( string modelPath in arrayMap )
            {
                modelName = Path.GetFileNameWithoutExtension(modelPath);

                if ( File.Exists(Path.Combine(currentDirectory, @"Assets\ReMap\Lods - Dont use these\Models\", modelName + "_LOD0.fbx") ) )
                {
                    modelReplacePath = modelPath.Replace("/", "#").Replace(".rmdl", ".prefab");
                    category = modelPath.Split("/")[1];

                    if ( !File.Exists(Path.Combine(currentDirectory, @"Assets\Prefabs\" + mapName, modelReplacePath) ) )
                    {

                        prefabToAdd = AssetDatabase.LoadAssetAtPath(@"Assets\ReMap\Lods - Dont use these\EmptyPrefab.prefab", typeof(UnityEngine.Object) ) as GameObject;
                        objectToAdd = AssetDatabase.LoadAssetAtPath( @"Assets\ReMap\Lods - Dont use these\Models\" + modelName + "_LOD0.fbx", typeof(UnityEngine.Object) )as GameObject;
                            if ( prefabToAdd == null || objectToAdd == null ) return;
                        prefabInstance = UnityEngine.Object.Instantiate(prefabToAdd) as GameObject;
                        objectInstance = UnityEngine.Object.Instantiate(objectToAdd) as GameObject;
                            if ( prefabInstance == null || objectInstance == null ) return;

                        prefabInstance.name = modelReplacePath.Replace(".prefab", "");
                        objectInstance.name = modelName + "_LOD0";

                        prefabInstance.transform.position = new Vector3( 0, 0, 0 );
                        prefabInstance.transform.eulerAngles = new Vector3( 0, 0, 0 );
                        objectInstance.transform.position = new Vector3( 0, 0, 0 );
                        objectInstance.transform.eulerAngles = new Vector3( 0, -90, 0 );

                        objectInstance.transform.SetParent(prefabInstance.transform);

                        PrefabUtility.SaveAsPrefabAsset( prefabInstance, Path.Combine(currentDirectory, @"Assets\Prefabs\" + mapName, modelReplacePath) );

                        UnityEngine.Object.DestroyImmediate( prefabInstance );
                    }
                }
            }
        }
    }

    public static void SetModelLabels()
    {
        foreach (var guid in AssetDatabase.FindAssets("mdl#", new [] {"Assets/Prefabs"}))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            string category = assetPath.ToString().Split("#")[1];
            AssetDatabase.SetLabels(asset, new []{category});
        }
    }
}