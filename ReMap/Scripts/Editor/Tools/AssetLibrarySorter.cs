using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class AssetLibrarySorter : EditorWindow
{
    static string currentDirectory = Directory.GetCurrentDirectory();
    static string relativeEmptyPrefab = $"Assets/ReMap/Lods - Dont use these/EmptyPrefab.prefab";
    static string relativeLods =$"Assets/ReMap/Lods - Dont use these";
    static string relativeModel =$"Assets/ReMap/Lods - Dont use these/Models";
    static string relativePrefabs =$"Assets/Prefabs";
    static string relativeRpakFile = $"Assets/ReMap/Resources/rpakModelFile";

    Vector2 scrollPos = Vector2.zero;

    bool byfile = true;
    bool byprefab = false;
    string search = "";
    string search_tmp = "";

    static List<string> allprefabs = new List<string>();

    [MenuItem("ReMap/Dev Tools/Asset Library Sorter/Sort Labels", false, 100)]
    public static async void SetModelLabelsInit()
    {
        await SetModelLabels();
    }

    [MenuItem("ReMap/Dev Tools/Asset Library Sorter/Check Models Files", false, 100)]
    public static void Init()
    {
        AssetLibrarySorter window = (AssetLibrarySorter)GetWindow(typeof(AssetLibrarySorter), false, "Check Models Files");
        window.minSize = new Vector2(650, 600);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("WARNING: ONLY USE THIS IF YOU KNOW WHAT YOU ARE DOING!", EditorStyles.boldLabel);
        GUILayout.Label("This will sort all the models in the library into their respective folders.", EditorStyles.boldLabel);
        GUILayout.Space(20);

        byfile = EditorGUILayout.BeginFoldoutHeaderGroup(byfile, "Sort By File");
        if (byfile)
        {
            GUILayout.BeginVertical("box");
            string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f) != "modelAnglesOffset.txt").ToArray();
            foreach (string file in files)
            {
                string mapName = Path.GetFileNameWithoutExtension(file);
                if (GUILayout.Button(mapName))
                    SortFolder(file);
            }
            if (GUILayout.Button("Sort All"))
                if (EditorUtility.DisplayDialog("Sort All", "Are you sure you want to sort all files?", "Yes", "No"))
                    LibrarySorter();
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        byprefab = EditorGUILayout.BeginFoldoutHeaderGroup(byprefab, "Sort By File");
        if (byprefab)
        {
            search = EditorGUILayout.TextField("Search", search);
            if (search.Length >= 3)
            {
                if (search != search_tmp)
                {
                    search_tmp = search;
                    GetAllPrefabs(search);
                }
            }

            GUILayout.Space(10);
            if (search.Length >= 3)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                foreach (string prefab in allprefabs)
                {
                    string prefabname = Path.GetFileNameWithoutExtension(prefab);
                    if (GUILayout.Button(prefabname))
                    {
                        FixPrefab(prefabname);
                        SetModelLabels(prefabname);
                    }
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Search must be at least 3 characters long.");
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public static void GetAllPrefabs(string search = "")
    {
        string[] prefabs = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Prefabs"});
        allprefabs = new List<string>();

        foreach (string prefab in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefab);
            if(!path.Contains("mdl#"))
                continue;

            if(!path.Contains("all_models"))
                continue;

            if(search != "" && !path.Contains(search))
                continue;

            allprefabs.Add(path);
        }
    }

    public static void FixPrefab(string prefabname)
    {
        string[] prefabs = AssetDatabase.FindAssets(prefabname, new [] {"Assets/Prefabs"});

        foreach (string prefab in prefabs)
        {
            string file = AssetDatabase.GUIDToAssetPath(prefab);

            UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath(file, typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Library Sorter] Error loading prefab: {file}", ReMapConsole.LogType.Error);
                continue;
            }

            Transform child = loadedPrefabResource.GetComponentsInChildren<Transform>()[1];

            if (child.transform.eulerAngles == FindAnglesOffset(file))
                continue;

            loadedPrefabResource.transform.position = Vector3.zero;
            loadedPrefabResource.transform.eulerAngles = Vector3.zero;
            child.transform.eulerAngles = FindAnglesOffset(file);
            child.transform.position = Vector3.zero;

            PrefabUtility.SavePrefabAsset(loadedPrefabResource);

            ReMapConsole.Log($"[Library Sorter] Fixed and saved prefab: {file}", ReMapConsole.LogType.Success);
        }
    }

    public static void SortFolder(string file)
    {
        ReMapConsole.Log($"[Library Sorter] Reading file: {file}", ReMapConsole.LogType.Warning);
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
        if (!Directory.Exists(mapPath))
        {
            ReMapConsole.Log($"[Library Sorter] Creating directory: {relativePrefabs}/{mapName}", ReMapConsole.LogType.Info);
            Directory.CreateDirectory(mapPath);
        }


        string modelName; string modelReplacePath;
        GameObject prefabToAdd; GameObject prefabInstance;
        GameObject objectToAdd; GameObject objectInstance;

        foreach (string modelPath in arrayMap)
        {
            modelName = Path.GetFileNameWithoutExtension(modelPath);

            if (File.Exists($"{currentDirectory}/{relativeModel}/{modelName + "_LOD0.fbx"}"))
            {
                modelReplacePath = modelPath.Replace("/", "#").Replace(".rmdl", ".prefab");

                if (!File.Exists($"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}"))
                {
                    prefabToAdd = AssetDatabase.LoadAssetAtPath($"{relativeEmptyPrefab}", typeof(UnityEngine.Object)) as GameObject;
                    objectToAdd = AssetDatabase.LoadAssetAtPath($"{relativeModel}/{modelName + "_LOD0.fbx"}", typeof(UnityEngine.Object)) as GameObject;

                    if (prefabToAdd == null || objectToAdd == null)
                    {
                        ReMapConsole.Log($"[Library Sorter] Error loading prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                        continue;
                    }

                    prefabInstance = UnityEngine.Object.Instantiate(prefabToAdd) as GameObject;
                    objectInstance = UnityEngine.Object.Instantiate(objectToAdd) as GameObject;

                    if (prefabInstance == null || objectInstance == null)
                    {
                        ReMapConsole.Log($"[Library Sorter] Error creating prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                        continue;
                    }

                    prefabInstance.AddComponent<PropScript>();

                    prefabInstance.name = modelReplacePath.Replace(".prefab", "");
                    objectInstance.name = modelName + "_LOD0";

                    prefabInstance.transform.position = Vector3.zero;
                    prefabInstance.transform.eulerAngles = Vector3.zero;

                    objectInstance.transform.parent = prefabInstance.transform;
                    objectInstance.transform.position = Vector3.zero;
                    objectInstance.transform.eulerAngles = FindAnglesOffset(modelPath);
                    objectInstance.transform.localScale = new Vector3(1, 1, 1);

                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, $"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}");

                    UnityEngine.Object.DestroyImmediate(prefabInstance);
                    ReMapConsole.Log($"[Library Sorter] Creating prefab: {relativePrefabs}/{mapName}/{modelReplacePath}", ReMapConsole.LogType.Info);
                }
                else
                {
                    UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath($"{relativePrefabs}/{mapName}/{modelReplacePath}", typeof(UnityEngine.Object)) as GameObject;
                    if (loadedPrefabResource == null)
                    {
                        ReMapConsole.Log($"[Library Sorter] Error loading prefab: {modelReplacePath}", ReMapConsole.LogType.Error);
                        continue;
                    }

                    Transform child = loadedPrefabResource.GetComponentsInChildren<Transform>()[1];

                    if (child.transform.eulerAngles == FindAnglesOffset(modelPath))
                        continue;

                    loadedPrefabResource.transform.position = Vector3.zero;
                    loadedPrefabResource.transform.eulerAngles = Vector3.zero;
                    child.transform.eulerAngles = FindAnglesOffset(modelPath);
                    child.transform.position = Vector3.zero;

                    PrefabUtility.SavePrefabAsset(loadedPrefabResource);

                    ReMapConsole.Log($"[Library Sorter] Fixed and saved prefab: {relativePrefabs}/{mapName}/{modelReplacePath}", ReMapConsole.LogType.Success);
                }
            }
        }

        ReMapConsole.Log($"[Library Sorter] Setting labels for prefabs in: {mapName}", ReMapConsole.LogType.Info);
        AssetLibrarySorter.SetFolderLabels(mapName);
    }

    public static void LibrarySorter()
    {
        string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f) != "modelAnglesOffset.txt").ToArray();
        foreach (string file in files)
        {
            SortFolder(file);
        }

        ReMapConsole.Log($"[Library Sorter] Finished sorting models", ReMapConsole.LogType.Success);
    }
    

    public static void Dev_GetPrefabAnglesThatAreDiffrent()
    {
        string[] prefabs = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Prefabs"});
        List<string> prefabsList = new List<string>();

        foreach (string prefab in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefab);
            if(!path.Contains("mdl#"))
                continue;

            if(prefabsList.Contains(path))
                continue;

            GameObject prefabObject = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object)) as GameObject;

            Transform[] children = prefabObject.GetComponentsInChildren<Transform>();

            Vector3 normang = new Vector3(0, -90, 0);
            Vector3 normang2 = new Vector3(0, 270, 0);

            if (children[1].transform.eulerAngles != normang && children[1].transform.eulerAngles != normang2)
            {
                string name = Path.GetFileNameWithoutExtension(path).Replace(" ", "").Replace(".prefab", "");
                ReMapConsole.Log($"{name};( {children[1].transform.eulerAngles.x}, {children[1].transform.eulerAngles.y}, {children[1].transform.eulerAngles.z} )", ReMapConsole.LogType.Info);
                prefabsList.Add(path);
            }
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
                    string[] parts = line.Split(";")[1].Replace("(", "").Replace(")", "").Replace(" ", "").Split(",");

                    ReMapConsole.Log($"[Library Sorter] Angle override found for {searchTerm}, setting angles to: {line.Split(";")[1]}", ReMapConsole.LogType.Info);
    
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

    public static Task SetModelLabels( string specificModelOrFolderOrnull = null )
    {
        string specificFolder = $"";
        string specificModel = $"mdl#";
        
        if ( specificModelOrFolderOrnull != null )
        {
            if ( specificModelOrFolderOrnull.Contains("mdl#") )
                specificModel = specificModelOrFolderOrnull;
            else specificFolder = $"/{specificModelOrFolderOrnull}";
        }
        
        string[] guids = AssetDatabase.FindAssets($"{specificModel}", new [] {$"Assets/Prefabs{specificFolder}"});
        int i = 0;
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            string category = assetPath.Split("#")[1].Replace(".prefab", "");

            string[] modelnamesplit = assetPath.Split("/");
            string modelname = modelnamesplit[modelnamesplit.Length - 1].Replace(".prefab", "");

            string[] label = AssetDatabase.GetLabels(asset);
            if( !label.Contains(category) )
            {
                AssetDatabase.SetLabels(asset, new []{category});
                ReMapConsole.Log($"[Library Sorter] Setting label for {modelname} to {category}", ReMapConsole.LogType.Info);
            }

            EditorUtility.DisplayProgressBar("Sorting Tags", "Setting " + modelname + " to " + category, (i + 1) / (float)guids.Length);
            i++;
        }

        ReMapConsole.Log($"[Library Sorter] Finished setting labels", ReMapConsole.LogType.Success);
        EditorUtility.ClearProgressBar();

        return Task.CompletedTask;
    }
}