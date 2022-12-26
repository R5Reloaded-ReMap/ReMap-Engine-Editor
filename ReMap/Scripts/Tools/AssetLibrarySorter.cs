using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class AssetLibrarySorter
{
    static string currentDirectory = Directory.GetCurrentDirectory();
    static string relativeEmptyPrefab = $"Assets/ReMap/Lods - Dont use these/EmptyPrefab.prefab";
    static string relativeLods =$"Assets/ReMap/Lods - Dont use these";
    static string relativeModel =$"Assets/ReMap/Lods - Dont use these/Models";
    static string relativePrefabs =$"Assets/Prefabs";
    static string relativeRpakFile = $"Assets/ReMap/Scripts/Tools/rpakModelFile";

    public static void LibrarySorter()
    {
        string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f) != "modelAnglesOffset.txt").ToArray();
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

            string mapPath = $"{currentDirectory}/{relativePrefabs}/{mapName}";
            if ( !Directory.Exists( mapPath ) )
                Directory.CreateDirectory( mapPath );

            string modelName ; string modelReplacePath;
            GameObject prefabToAdd; GameObject prefabInstance;
            GameObject objectToAdd; GameObject objectInstance;

            foreach ( string modelPath in arrayMap )
            {
                modelName = Path.GetFileNameWithoutExtension(modelPath);

                if (File.Exists($"{currentDirectory}/{relativeModel}/{modelName + "_LOD0.fbx"}"))
                {
                    modelReplacePath = modelPath.Replace("/", "#").Replace(".rmdl", ".prefab");

                    if ( !File.Exists( $"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}" ) )
                    {
                        
                        prefabToAdd = AssetDatabase.LoadAssetAtPath($"{relativeEmptyPrefab}", typeof(UnityEngine.Object) ) as GameObject;
                        objectToAdd = AssetDatabase.LoadAssetAtPath( $"{relativeModel}/{modelName + "_LOD0.fbx"}", typeof(UnityEngine.Object) )as GameObject;
                            if ( prefabToAdd == null || objectToAdd == null ) return;
                        prefabInstance = UnityEngine.Object.Instantiate(prefabToAdd) as GameObject;
                        objectInstance = UnityEngine.Object.Instantiate(objectToAdd) as GameObject;
                            if ( prefabInstance == null || objectInstance == null ) return;

                        prefabInstance.AddComponent<PropScript>();

                        prefabInstance.name = modelReplacePath.Replace(".prefab", "");
                        objectInstance.name = modelName + "_LOD0";

                        prefabInstance.transform.position = new Vector3( 0, 0, 0 );
                        prefabInstance.transform.eulerAngles = new Vector3( 0, 0, 0 );
                        objectInstance.transform.localPosition = new Vector3( 0, 0, 0 );
                        objectInstance.transform.localEulerAngles = FindAnglesOffset(modelPath);

                        objectInstance.transform.localScale = new Vector3(1, 1, 1);

                        objectInstance.transform.SetParent( prefabInstance.transform );

                        PrefabUtility.SaveAsPrefabAsset( prefabInstance, $"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}" );

                        UnityEngine.Object.DestroyImmediate( prefabInstance );
                    }
                }
            }

            AssetLibrarySorter.SetFolderLabels(mapName);
        }
    }

    public static Vector3 FindAnglesOffset(string searchTerm)
    {
        Vector3 returnedVector = new Vector3( 0, -90, 0 );

        using (StreamReader reader = new StreamReader($"{currentDirectory}/{relativeRpakFile}/modelAnglesOffset.txt"))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(searchTerm) && !line.Contains("//"))
                {
                    string[] parts = line.Split(";")[1].Replace("(", "").Replace(")", "").Split(",");
    
                    float x = float.Parse(parts[0]);
                    float y = float.Parse(parts[1]);
                    float z = float.Parse(parts[2]);
                    returnedVector = new Vector3( x, y, z );
                    break;
                }
            }
        }

        return returnedVector;
    }

    public static async void SetFolderLabels(string mapName)
    {
        await AssetLibrarySorter.SetModelLabels(mapName);
    }

    public static Task SetModelLabels( string specificFolderOrnull = null )
    {
        string specificFolder = $""; 
        
        if ( specificFolderOrnull != null )
            specificFolder = $"/{specificFolderOrnull}";
        
        string[] guids = AssetDatabase.FindAssets("mdl#", new [] {$"Assets/Prefabs{specificFolder}"});
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