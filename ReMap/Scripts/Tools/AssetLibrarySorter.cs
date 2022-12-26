using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Threading.Tasks;

public class AssetLibrarySorter
{
    public static void LibrarySorter()
    {
        int totalFiles = 0;
        Array[] totalArrayMap = new Array[20];
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativeLods =$"Assets/ReMap/Lods - Dont use these";
        string relativePrefabs =$"Assets/Prefabs";
        string relativeRpakFile = $"Assets/ReMap/Scripts/Tools/rpakModelFile";
        string relativeEmptyPrefab = $"Assets/ReMap/Lods - Dont use these/EmptyPrefab.prefab";
        string relativeModel =$"Assets/ReMap/Lods - Dont use these/Models";

        string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt");
        foreach (string file in files)
        {
            int lineCount = File.ReadAllLines(file).Length;

            string[] arrayMap = new string[lineCount];

            string mapName = Path.GetFileNameWithoutExtension(file);

            int row = 0;

            using (StreamReader reader = new StreamReader(file))
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

            string mapPath = $"{currentDirectory}/{relativePrefabs}/{mapName}";
            if ( !Directory.Exists( mapPath ) )
                Directory.CreateDirectory( mapPath );

            string modelName ; string modelReplacePath; string category;
            GameObject prefabToAdd; GameObject prefabInstance;
            GameObject objectToAdd; GameObject objectInstance;

            foreach ( string modelPath in arrayMap )
            {
                modelName = Path.GetFileNameWithoutExtension(modelPath.Split(";")[0]);

                if (File.Exists($"{currentDirectory}/{relativeModel}/{modelName + "_LOD0.fbx"}"))
                {
                    modelReplacePath = modelPath.Split(";")[0].Replace("/", "#").Replace(".rmdl", ".prefab");
                    category = modelPath.Split(";")[0].Split("/")[1];

                    if ( !File.Exists( $"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}" ) )
                    {
                        
                        prefabToAdd = AssetDatabase.LoadAssetAtPath($"{relativeEmptyPrefab}", typeof(UnityEngine.Object) ) as GameObject;
                        objectToAdd = AssetDatabase.LoadAssetAtPath( $"{relativeModel}/{modelName + "_LOD0.fbx"}", typeof(UnityEngine.Object) )as GameObject;
                            if ( prefabToAdd == null || objectToAdd == null ) return;
                        prefabInstance = UnityEngine.Object.Instantiate(prefabToAdd) as GameObject;
                        objectInstance = UnityEngine.Object.Instantiate(objectToAdd) as GameObject;
                            if ( prefabInstance == null || objectInstance == null ) return;

                        prefabInstance.name = modelReplacePath.Replace(".prefab", "");
                        objectInstance.name = modelName + "_LOD0";

                        prefabInstance.transform.position = new Vector3( 0, 0, 0 );
                        prefabInstance.transform.eulerAngles = new Vector3( 0, 0, 0 );
                        objectInstance.transform.position = new Vector3( 0, 0, 0 );

                        string[] parts = modelPath.Split(";")[1].Replace("(", "").Replace(")", "").Split(',');
    
                        float x = float.Parse(parts[0]);
                        float y = float.Parse(parts[1]);
                        float z = float.Parse(parts[2]);
                        Vector3 vector = new Vector3(x, y, z);
                        objectInstance.transform.eulerAngles = vector;

                        objectInstance.transform.SetParent( prefabInstance.transform );

                        PrefabUtility.SaveAsPrefabAsset( prefabInstance, $"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}" );

                        UnityEngine.Object.DestroyImmediate( prefabInstance );
                    }
                }
            }
        }
    }

    public static Task SetModelLabels()
    {
        string[] guids = AssetDatabase.FindAssets("mdl#", new [] {"Assets/Prefabs"});
        int i = 0;
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            string category = assetPath.Split("#")[1];
            AssetDatabase.SetLabels(asset, new []{category});

            string[] modelnamesplit = assetPath.Split("/");
            string modelname = modelnamesplit[modelnamesplit.Length - 1].Replace(".prefab", "");
            EditorUtility.DisplayProgressBar("Sorting Tags", "Setting " + modelname + " to " + category, (i + 1) / (float)guids.Length);
            i++;
        }

        EditorUtility.ClearProgressBar();

        return Task.CompletedTask;
    }
}