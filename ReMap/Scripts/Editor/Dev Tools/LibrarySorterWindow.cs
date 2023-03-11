using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
public class LibrarySorterWindow : EditorWindow
{
    public static string currentDirectory = Directory.GetCurrentDirectory().Replace("\\","/");
    public static string relativeEmptyPrefab = $"Assets/ReMap/Lods - Dont use these/EmptyPrefab.prefab";
    public static string relativeLods = $"Assets/ReMap/Lods - Dont use these";
    public static string relativeModel = $"Assets/ReMap/Lods - Dont use these/Models";
    public static string relativeMaterials = $"Assets/ReMap/Lods - Dont use these/Materials";
    public static string relativePrefabs = $"Assets/Prefabs";
    public static string relativeRpakFile = $"Assets/ReMap/Resources/rpakModelFile";

    public static string[] protectedFolders = { "_custom_prefabs" };

    static RpakContentJson rpakContent = new RpakContentJson();

    Vector2 scrollPos = Vector2.zero;

    bool byfile = true;
    bool findmissing = true;
    bool byprefab = true;
    bool options = true;
    string search = "";
    string search_tmp = "";

    static bool checkandfixifexists = false;

    static List<string> allprefabs = new List<string>();

    #if ReMapDev
        [MenuItem("ReMap Dev Tools/Asset Library Sorter/Sort Labels", false, 100)]
        public static async void SetModelLabelsInit()
        {
            await SetModelLabels();
        }

        [MenuItem("ReMap Dev Tools/Asset Library Sorter/Check Models Files", false, 100)]
        public static void Init()
        {
            LibrarySorterWindow window = (LibrarySorterWindow)GetWindow(typeof(LibrarySorterWindow), false, "Check Models Files");
            window.minSize = new Vector2(650, 600);
            window.Show();
        }

        [MenuItem("ReMap Dev Tools/Asset Library Sorter/Delete Not Used Texture", false, 100)]
        public static void TextureInit()
        {
            DeleteNotUsedTexture();
        }

        [MenuItem("ReMap Dev Tools/Asset Library Sorter/Rpak List", false, 100)]
        public static async void RpakListInit()
        {
            await RpakList();
        }
    #endif

    void OnGUI()
    {
        GUILayout.Label("WARNING: ONLY USE THIS IF YOU KNOW WHAT YOU ARE DOING!", EditorStyles.boldLabel);
        GUILayout.Label("This will sort all the models in the library into their respective folders.", EditorStyles.boldLabel);
        GUILayout.Space(20);

        string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f) != "modelAnglesOffset.txt" && Path.GetFileName(f) != "lastestFolderUpdate.txt").ToArray();
        string[] timestamps = File.ReadAllLines($"{currentDirectory}/{relativeRpakFile}/lastestFolderUpdate.txt");

        options = EditorGUILayout.BeginFoldoutHeaderGroup(options, "Options");
        if (options)
        {
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create all_models.txt file", GUILayout.Width(400)))
                CreateAllModelsList();
            GUILayout.FlexibleSpace();
            checkandfixifexists = EditorGUILayout.Toggle("Fix existing prefabs", checkandfixifexists);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Fix Tags", GUILayout.Width(400)))
                FixPropTags();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        byfile = EditorGUILayout.BeginFoldoutHeaderGroup(byfile, "Sort By File");
        if (byfile)
        {
            GUILayout.BeginVertical("box");
            foreach (string file in files)
            {
                if(protectedFolders.Contains(Path.GetFileNameWithoutExtension(file)))
                    continue;

                string mapName = Path.GetFileNameWithoutExtension(file);
                string timestamp = "Not Updated";
                foreach( string verifiedFile in timestamps ) if ( verifiedFile.Contains(mapName) ) timestamp = timestamps.Where(t => t.Contains(mapName)).FirstOrDefault().Split('|')[1].Replace("[ ", "").Replace(" ]", "");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"{mapName}", GUILayout.Width(400)))
                    LibrarySortFolder(file);
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Last Update: {timestamp}");
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Sort All"))
                if (EditorUtility.DisplayDialog("Sort All", "Are you sure you want to sort all files?", "Yes", "No"))
                    LibrarySortAll();
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        findmissing = EditorGUILayout.BeginFoldoutHeaderGroup(findmissing, "Find Missing By File");
        if (findmissing)
        {
            GUILayout.BeginVertical("box");
            foreach (string file in files)
            {
                if(protectedFolders.Contains(Path.GetFileNameWithoutExtension(file)))
                    continue;

                string mapName = Path.GetFileNameWithoutExtension(file);
                if (GUILayout.Button(mapName))
                    FindMissing(file);
            }
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

    public static void FixPropTags()
    {
        string[] prefabs = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Prefabs"});
        allprefabs = new List<string>();

        foreach (string prefab in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefab);
            if (path.Contains("_custom_prefabs"))
                continue;

            UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath($"{path}", typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Library Sorter] Error loading prefab: {path}", ReMapConsole.LogType.Error);
                continue;
            }

            if(loadedPrefabResource.tag != "Prop")
                loadedPrefabResource.tag = "Prop";

            ReMapConsole.Log($"[Library Sorter] Set {path} tag to: Prop", ReMapConsole.LogType.Info);

            PrefabUtility.SavePrefabAsset(loadedPrefabResource);
        }
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

            CheckBoxColliderComponent( loadedPrefabResource );

            loadedPrefabResource.transform.position = Vector3.zero;
            loadedPrefabResource.transform.eulerAngles = Vector3.zero;
            child.transform.eulerAngles = FindAnglesOffset(file);
            child.transform.position = Vector3.zero;

            PrefabUtility.SavePrefabAsset(loadedPrefabResource);

            ReMapConsole.Log($"[Library Sorter] Fixed and saved prefab: {file}", ReMapConsole.LogType.Success);
        }
    }

    public static async void FindMissing(string file)
    {
        ReMapConsole.Log($"[Library Sorter] Reading file: {file}", ReMapConsole.LogType.Warning);
        string[] models = File.ReadAllLines(file);
        string mapName = Path.GetFileNameWithoutExtension(file);

        int i = 0;
        foreach (string model in models)
        {
            if(model.Contains("/pov") || model.Contains("/ptpov") || model.Contains("_pov"))
                continue; 

            string modelname = model.Replace("/", "#").Replace(".rmdl", ".prefab");

            EditorUtility.DisplayProgressBar("Checking for missing models", $"Checking: {mapName}/{modelname}", (i + 1) / (float)models.Length);

            if (!File.Exists($"{currentDirectory}/{relativePrefabs}/{mapName}/{modelname}"))
            {
                ReMapConsole.Log($"[Library Sorter] Missing prefab: {mapName}/{modelname}", ReMapConsole.LogType.Error);
            }

            await Task.Yield();
            i++;
        }

        ReMapConsole.Log($"[Library Sorter] Finished", ReMapConsole.LogType.Success);
        EditorUtility.ClearProgressBar();
    }

    public static async Task SortFolder(string file)
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

        int arraylen = arrayMap.Length;

        int i = 0;
        foreach (string modelPath in arrayMap)
        {
            modelName = Path.GetFileNameWithoutExtension(modelPath);

            if (File.Exists($"{currentDirectory}/{relativeModel}/{modelName + "_LOD0.fbx"}"))
            {
                modelReplacePath = modelPath.Replace("/", "#").Replace(".rmdl", ".prefab");
    
                EditorUtility.DisplayProgressBar("Sorting Files", $"Sorting: {mapName}/{modelReplacePath}", (i + 1) / (float)arraylen);
    
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
    
                    prefabInstance.tag = "Prop";
    
                    CheckBoxColliderComponent( prefabInstance );
    
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, $"{currentDirectory}/{relativePrefabs}/{mapName}/{modelReplacePath}");
    
                    UnityEngine.Object.DestroyImmediate(prefabInstance);
                    ReMapConsole.Log($"[Library Sorter] Creating prefab: {relativePrefabs}/{mapName}/{modelReplacePath}", ReMapConsole.LogType.Info);
                }
                else
                {
                    if(!checkandfixifexists)
                        continue;
    
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
    
                    CheckBoxColliderComponent( loadedPrefabResource );
    
                    PrefabUtility.SavePrefabAsset(loadedPrefabResource);
    
                    ReMapConsole.Log($"[Library Sorter] Fixed and saved prefab: {relativePrefabs}/{mapName}/{modelReplacePath}", ReMapConsole.LogType.Success);
                }
            }

            await Task.Yield();
            i++;
        }

        ReMapConsole.Log($"[Library Sorter] Setting labels for prefabs in: {mapName}", ReMapConsole.LogType.Info);
        EditorUtility.ClearProgressBar();
        LibrarySorterWindow.SetFolderLabels(mapName);
    }

    public static void CheckBoxColliderComponent( GameObject go )
    {
        BoxCollider collider = go.GetComponent<BoxCollider>();
    
        if( collider == null ) collider = go.AddComponent<BoxCollider>();
    
        Bounds bounds = new Bounds();
    
        foreach(Renderer renderer in go.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
    
            BoxCollider BoxColliderChild = renderer.GetComponent<BoxCollider>();
    
            if(BoxColliderChild != null) DestroyImmediate(BoxColliderChild, true);
        }
    
        collider.center = bounds.center;
        collider.size = bounds.size;
    }

    private static bool IsContainsModelName( string filePath, string modelName )
    {
        return System.IO.File.ReadAllText(filePath).Contains( modelName.Replace( "#", "/" ) );
    }

    public static async void LibrarySortAll()
    {
        string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => IsNotExcludedFile(f)).ToArray();
        foreach (string file in files)
        {
            if(protectedFolders.Contains(Path.GetFileNameWithoutExtension(file)))
                continue;

            await SortFolder(file);
            UpdateLastestSort(file);
        }

        ReMapConsole.Log($"[Library Sorter] Finished sorting models", ReMapConsole.LogType.Success);
    }

    public static async void LibrarySortFolder(string file)
    {
        string mapPath = $"{currentDirectory}/{relativePrefabs}/{Path.GetFileNameWithoutExtension(file)}";
        if (!Directory.Exists(mapPath))
        {
            ReMapConsole.Log($"[Library Sorter] Creating directory: {relativePrefabs}/{Path.GetFileNameWithoutExtension(file)}", ReMapConsole.LogType.Info);
            Directory.CreateDirectory(mapPath);
        }

        await SortFolder(file);
        UpdateLastestSort(file);

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
        await LibrarySorterWindow.SetModelLabels(mapName);
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
            string category = assetPath.Split("#")[1].Replace(".prefab", "").ToLower();

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

    public static void DeleteNotUsedTexture()
    {
        List<string> texturesList = new List<string>();

        string[] modeltextureGUID = AssetDatabase.FindAssets("t:model", new [] {"Assets/ReMap/Lods - Dont use these/Models"});

        foreach (var guid in modeltextureGUID)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string[] dependencie = AssetDatabase.GetDependencies(assetPath);
            foreach( string dependencies in dependencie )
            {
                string fileName = Path.GetFileNameWithoutExtension(dependencies);
                if ( Path.GetExtension(dependencies) == ".dds" && !texturesList.Contains(fileName))
                {
                    texturesList.Add(fileName);
                }
            }
        }

        string[] usedTextures = texturesList.ToArray();

        string[] defaultAssetGUID = AssetDatabase.FindAssets("t:defaultAsset", new [] {"Assets/ReMap/Lods - Dont use these/Materials"});
        int j = 0;
        foreach (var guid in defaultAssetGUID)
        {
            string defaultAssetPath = AssetDatabase.GUIDToAssetPath(guid);

            if ( Path.GetExtension(defaultAssetPath) == ".dds")
            {
                File.Delete(defaultAssetPath);
                File.Delete(defaultAssetPath + ".meta");
                j++;
            }
        }

        string[] textureGUID = AssetDatabase.FindAssets("t:texture", new [] {"Assets/ReMap/Lods - Dont use these/Materials"});
        int i = 0;
        foreach (var guid in textureGUID)
        {
                string texturePath = AssetDatabase.GUIDToAssetPath(guid);

            if( !usedTextures.Contains(Path.GetFileNameWithoutExtension(texturePath)) )
            {
                File.Delete(texturePath);
                File.Delete(texturePath + ".meta");
                i++;
            }
        }

        ReMapConsole.Log($"{i} textures not used have been deleted", ReMapConsole.LogType.Success);
        ReMapConsole.Log($"{j} native assets have been deleted", ReMapConsole.LogType.Success);
        ReMapConsole.Log($"Total used textures: {usedTextures.Length} for {modeltextureGUID.Length} models", ReMapConsole.LogType.Info);
    }

    /// <summary>
    /// Create all_model.txt
    /// </summary>
    public static async void CreateAllModelsList()
    {
        if(File.Exists($"{currentDirectory}/{relativeRpakFile}/all_models.txt"))
            File.Delete($"{currentDirectory}/{relativeRpakFile}/all_models.txt");

        File.Create($"{currentDirectory}/{relativeRpakFile}/all_models.txt");

        List<string> allModels = new List<string>();

        string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f) != "modelAnglesOffset.txt" && Path.GetFileName(f) != "lastestFolderUpdate.txt" && Path.GetFileName(f) != "all_models.txt").ToArray();
        foreach (string file in files)
        {
            if(protectedFolders.Contains(Path.GetFileNameWithoutExtension(file)))
                continue;

            ReMapConsole.Log($"[Library Sorter] Reading {file}", ReMapConsole.LogType.Info);

            string[] lines = File.ReadAllLines(file);
            int i = 0;
            foreach (string line in lines)
            {
                if (!allModels.Contains(line))
                {
                    allModels.Add(line);
                    EditorUtility.DisplayProgressBar("Creating all_models.txt", $"Adding {line} to all_models.txt", (i + 1) / (float)lines.Length);
                    ReMapConsole.Log($"[Library Sorter] Added {line} to all_models.txt", ReMapConsole.LogType.Info);
                }
            }

            ReMapConsole.Log($"[Library Sorter] Finished reading {file}", ReMapConsole.LogType.Success);

            await Task.Yield();
        }

        allModels.Sort();

        ReMapConsole.Log($"[Library Sorter] Writing all_models.txt", ReMapConsole.LogType.Info);
        File.WriteAllLines( $"{currentDirectory}/{relativeRpakFile}/all_models.txt", allModels);

        ReMapConsole.Log($"[Library Sorter] Finished writing all_models.txt", ReMapConsole.LogType.Success);
        EditorUtility.DisplayProgressBar("Creating all_models.txt", "Finished writing all_models.txt", 1);
        EditorUtility.ClearProgressBar();
    }

    public static void UpdateLastestSort(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string registerUpdatesFile = $"{currentDirectory}/{relativeRpakFile}/lastestFolderUpdate.txt";
        if (!File.Exists(registerUpdatesFile))
            File.Create(registerUpdatesFile);

        List<string> line = File.ReadAllLines(registerUpdatesFile).ToList();

        int index = -1;

        foreach(string lines in line) if(lines.Contains($"[ {fileName} ]")) index = line.FindIndex(s => s.Contains(fileName));

        if ( index != -1 )
        {
            line[index] = $"[ {fileName} ]|[ {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")} ]";
        }
        else
        {
            line.Add($"[ {fileName} ]|[ {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")} ]");
        }

        line.Sort();

        File.WriteAllLines(registerUpdatesFile, line);
    }

    private static bool IsNotExcludedFile(string filePath, bool excludedAllModel = false)
    {
        string fileName = Path.GetFileName(filePath);
        string[] excludedFiles;

        if ( excludedAllModel )
        {
            excludedFiles = new string[] { "modelAnglesOffset.txt", "lastestFolderUpdate.txt", "all_models.txt" };
        }
        else
        {
            excludedFiles = new string[] { "modelAnglesOffset.txt", "lastestFolderUpdate.txt" };
        }

        return !excludedFiles.Contains(fileName);
    }

    public static async Task RpakList()
    {
        ResetRpakContentJson();

        await CreateRpakList();
    }

    private static async Task CreateRpakList()
    {
        //RpakInfoScript info = go.GetComponent<RpakInfoScript>();

        //if( info == null ) info = go.AddComponent<RpakInfoScript>();

        //string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => IsNotExcludedFile(f)).ToArray();

        //info.rpakList = new String[0];

        //foreach ( string path in files )
        //{
        //    string fileName = Path.GetFileNameWithoutExtension(path);

        //    if ( IsContainsModelName( path, go.name ) )
        //    {
        //        int listLength = info.rpakList.Length;
        //        Array.Resize( ref info.rpakList, listLength + 1 );
        //        info.rpakList[listLength] = fileName;
        //    }
        //}

        string[] files = Directory.GetFiles($"{currentDirectory}/{relativePrefabs}/all_models", "*.prefab", SearchOption.TopDirectoryOnly).ToArray();
        string[] folders = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => IsNotExcludedFile(f, true)).ToArray();

        foreach ( string path in files )
        {
            RpakContentClass content = new RpakContentClass();
            content.modelName = Path.GetFileNameWithoutExtension(path);

            /* foreach ( string path_ in folders )
            {
                string fileName = Path.GetFileNameWithoutExtension(path_);

                if ( IsContainsModelName( path, content.modelName ) )
                {
                    int listLength = content.location.Length;
                    Array.Resize( ref content.location, listLength + 1 );
                    content.location[listLength] = fileName;
                }
            } */
            
            rpakContent.List.Add(content);
            ReMapConsole.Log($"[TEST] {Path.GetFileNameWithoutExtension(path)}", ReMapConsole.LogType.Info);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
        }

        string json = JsonUtility.ToJson(rpakContent);
        System.IO.File.WriteAllText($"{currentDirectory}/{relativePrefabs}/RpakList.json", json);
    }

    private static void ResetRpakContentJson()
    {
        rpakContent = new RpakContentJson();
        rpakContent.List = new List<RpakContentClass>();
    }
}