using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class CodeViews : EditorWindow
{
    static string code_text = "";
    static Vector2 scroll;
    int tab = 0;
    int tab_temp = 0;

    bool OnlyExportMap = true;
    bool OnlyExportMap_temp = true;
    bool UseStartingOffset = false;
    bool UseStartingOffset_temp = false;
    static bool DisableStartingOffsetString = false;
    static bool DisableStartingOffsetString_temp = false;
    bool ShowAdvanced = false;
    public static bool UseOriginOffset = false;
    public static Vector3 OriginOffset;

    bool UseStartingOffsetLocPair = false;
    bool UseStartingOffsetLocPair_temp = false;

    // Gen Settings
    bool GenerateProps = true;
    bool GenerateButtons = true;
    bool GenerateJumppads = true;
    bool GenerateBubbleShields = true;
    bool GenerateDoors = true;
    bool GenerateLootBins = true;
    bool GenerateZipLines = true;
    bool GenerateWeaponRacks = true;
    bool GenerateTriggers = true;

    //Counts
    int mapcodecount = 0;
    int datatablecount = 0;
    int scriptentcount = 0;
    int soundentcount = 0;

    [MenuItem("ReMap/Code Views", false, 25)]
    public static void Init()
    {
        TagHelper.CheckAndCreateTags();

        CodeViews window = (CodeViews)GetWindow(typeof(CodeViews), false, "Code Views");
        window.minSize = new Vector2(1000, 500);
        window.Show();
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

        GetLatestCounts();
        GenerateCorrectCode();
    }

    private void EditorSceneManager_sceneSaved(UnityEngine.SceneManagement.Scene arg0)
    {
        GetLatestCounts();
        GenerateCorrectCode();
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        GetLatestCounts();
        GenerateCorrectCode();
    }

    void GetLatestCounts()
    {
        mapcodecount = UnityInfo.GetAllCount();
        datatablecount = UnityInfo.GetPropCount();
        scriptentcount = UnityInfo.GetPropCount();
        soundentcount = UnityInfo.GetSoundCount();
    }

    void GenerateCorrectCode()
    {
        switch (tab) {
            case 0: //Map Code
                GenerateMap(OnlyExportMap, false);
                break;
            case 1: //DataTable Code
                GenerateDataTable(false);
                break;
            case 2: //Precache Code
                GeneratePrecacheCode(false);
                break;
            case 3: //Script.ent Code
                GenerateEntCode(false);
                break;
            case 4: //Sound.ent Code
                GenerateSoundEntCode(false);
                break;
            case 5: //NewLocPair Code
                GenerateNewLocPairCode(false);
                break;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal("box");
        tab = GUILayout.Toolbar (tab, new string[] {"Map Code", "DataTable Code", "Precache Code", "Script.ent Code", "Sound.ent Code", "NewLocPair Code"});
        if (GUILayout.Button("Reload Page", GUILayout.Width(100) )) GenerateCorrectCode();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        if(tab != tab_temp)
            GetLatestCounts();

        switch (tab) {
            case 0: //Map Code
                MapCodeGUI();
                if (tab != tab_temp)
                    GenerateMap(OnlyExportMap, false);
                tab_temp = tab;
                break;
            case 1: //DataTable Code
                DataTableGUI();
                if(tab != tab_temp)
                    GenerateDataTable(false);
                tab_temp = tab;
                break;
            case 2: //Precache Code
                PrecacheGUI();
                if(tab != tab_temp)
                    GeneratePrecacheCode(false);
                tab_temp = tab;
                break;
            case 3: //Script.ent Code
                EntCodeGUI();
                if (tab != tab_temp)
                    GenerateEntCode(false);
                tab_temp = tab;
                break;
            case 4: //Sound.ent Code
                SoundEntGUI();
                if (tab != tab_temp)
                    GenerateSoundEntCode(false);
                tab_temp = tab;
                break;
            case 5: //NewLocPair Code
                NewLocPairGUI();
                if (tab != tab_temp)
                    GenerateNewLocPairCode(false);
                tab_temp = tab;
                break;
        }
    }

    void SetCorrectColor(int count)
    {
        if(count < Helper.greenPropCount)
            GUI.contentColor = Color.green;
        else if((count < Helper.yellowPropCount)) 
            GUI.contentColor = Color.yellow;
        else
            GUI.contentColor = Color.red;
    }

    void SetCorrectEntityLabel(int count)
    {
        if(count < Helper.greenPropCount)
            GUILayout.Label("Status: Safe");
        else if((count < Helper.yellowPropCount)) 
            GUILayout.Label("Status: Safe");
        else
            GUILayout.Label("Status: Warning! Game could crash!");
    }

    void MapCodeGUI()
    {
        GUILayout.BeginVertical("box");
        SetCorrectColor(mapcodecount);
        GUILayout.Label("Entity Count: " + mapcodecount, EditorStyles.boldLabel);
        SetCorrectEntityLabel(mapcodecount);
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;
        
        GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            OnlyExportMap = EditorGUILayout.Toggle("Only Show Map Code", OnlyExportMap);
            if(OnlyExportMap != OnlyExportMap_temp) {
                OnlyExportMap_temp = OnlyExportMap;
                GenerateMap(OnlyExportMap, false);
            }
            UseStartingOffset = EditorGUILayout.Toggle("Use Map Origin Offset", UseStartingOffset);
            if(UseStartingOffset != UseStartingOffset_temp) {
                UseStartingOffset_temp = UseStartingOffset;
                GenerateMap(OnlyExportMap, false);
            }
            DisableStartingOffsetString = EditorGUILayout.Toggle("Hide Starting Vector", DisableStartingOffsetString);
            if(DisableStartingOffsetString != DisableStartingOffsetString_temp) {
                DisableStartingOffsetString_temp = DisableStartingOffsetString;
                GenerateMap(OnlyExportMap, false);
            }
            ShowAdvanced = EditorGUILayout.Toggle("Show Advanced Options", ShowAdvanced);
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();

            if (ShowAdvanced)
            {
                GUILayout.BeginVertical("box");
                GUILayout.BeginHorizontal();
                Helper.GenerateProps = EditorGUILayout.Toggle("Build Props", Helper.GenerateProps);
                if(Helper.GenerateProps != GenerateProps) {
                    GenerateProps = Helper.GenerateProps;
                    GenerateMap(OnlyExportMap, false);
                }

                Helper.GenerateButtons = EditorGUILayout.Toggle("Build Buttons", Helper.GenerateButtons);
                if(Helper.GenerateButtons != GenerateButtons) {
                    GenerateButtons = Helper.GenerateButtons;
                    GenerateMap(OnlyExportMap, false);
                }

                Helper.GenerateTriggers = EditorGUILayout.Toggle("Build Triggers", Helper.GenerateTriggers);
                if(Helper.GenerateTriggers != GenerateTriggers) {
                    GenerateTriggers = Helper.GenerateTriggers;
                    GenerateMap(OnlyExportMap, false);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                Helper.GenerateDoors = EditorGUILayout.Toggle("Build Doors", Helper.GenerateDoors);
                if(Helper.GenerateDoors != GenerateDoors) {
                    GenerateDoors = Helper.GenerateDoors;
                    GenerateMap(OnlyExportMap, false);
                }

                Helper.GenerateJumppads = EditorGUILayout.Toggle("Build Jumppads", Helper.GenerateJumppads);
                if(Helper.GenerateJumppads != GenerateJumppads) {
                    GenerateJumppads = Helper.GenerateJumppads;
                    GenerateMap(OnlyExportMap, false);
                }

                Helper.GenerateBubbleShields = EditorGUILayout.Toggle("Build BubbleShields", Helper.GenerateBubbleShields);
                if(Helper.GenerateBubbleShields != GenerateBubbleShields) {
                    GenerateBubbleShields = Helper.GenerateBubbleShields;
                    GenerateMap(OnlyExportMap, false);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                Helper.GenerateZipLines = EditorGUILayout.Toggle("Build ZipLines", Helper.GenerateZipLines);
                if(Helper.GenerateZipLines != GenerateZipLines) {
                    GenerateZipLines = Helper.GenerateZipLines;
                    GenerateMap(OnlyExportMap, false);
                }

                Helper.GenerateWeaponRacks = EditorGUILayout.Toggle("Build WeaponRacks", Helper.GenerateWeaponRacks);
                if(Helper.GenerateWeaponRacks != GenerateWeaponRacks) {
                    GenerateWeaponRacks = Helper.GenerateWeaponRacks;
                    GenerateMap(OnlyExportMap, false);
                }

                Helper.GenerateLootBins = EditorGUILayout.Toggle("Build LootBins", Helper.GenerateLootBins);
                if(Helper.GenerateLootBins != GenerateLootBins) {
                    GenerateLootBins = Helper.GenerateLootBins;
                    GenerateMap(OnlyExportMap, false);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateMap(OnlyExportMap, true);
        GUILayout.EndVertical();
    }

    void DataTableGUI()
    {
        GUILayout.BeginVertical("box");
        SetCorrectColor(datatablecount);
        GUILayout.Label("Entity Count: " + datatablecount, EditorStyles.boldLabel);
        SetCorrectEntityLabel(datatablecount);
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateDataTable(true);
        GUILayout.EndVertical();
    }

    void PrecacheGUI()
    {
        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GeneratePrecacheCode(true);
        GUILayout.EndVertical();
    }

    void EntCodeGUI()
    {
        GUILayout.BeginVertical("box");
        SetCorrectColor(scriptentcount);
        GUILayout.Label("Entity Count: " + scriptentcount, EditorStyles.boldLabel);
        SetCorrectEntityLabel(scriptentcount);
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateEntCode(true);
        GUILayout.EndVertical();
    }

    void SoundEntGUI()
    {
        GUILayout.BeginVertical("box");
        SetCorrectColor(soundentcount);
        GUILayout.Label("Entity Count: " + soundentcount, EditorStyles.boldLabel);
        SetCorrectEntityLabel(soundentcount);
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        UseOriginOffset = EditorGUILayout.Toggle("Add a origin offset", UseOriginOffset);
        if(UseOriginOffset) OriginOffset = EditorGUILayout.Vector3Field("Origin Offset", OriginOffset);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateSoundEntCode(true);
        GUILayout.EndVertical();
    }

    void NewLocPairGUI()
    {
        Helper.Is_Using_Starting_Offset = UseStartingOffsetLocPair;

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        UseOriginOffset = EditorGUILayout.Toggle("Add a origin offset", UseOriginOffset);
        if(UseOriginOffset) OriginOffset = EditorGUILayout.Vector3Field("Origin Offset", OriginOffset);

        UseStartingOffsetLocPair = EditorGUILayout.Toggle("Use Map Origin Offset", UseStartingOffsetLocPair);
        if(UseStartingOffsetLocPair != UseStartingOffsetLocPair_temp) {
            UseStartingOffsetLocPair_temp = UseStartingOffsetLocPair;
            GenerateNewLocPairCode(false);
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(code_text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Copy To Clipboard"))
            GenerateNewLocPairCode(true);
        GUILayout.EndVertical();
    }

    void GenerateMap(bool onlyMapCode, bool copyCode)
    {
        Helper.FixPropTags();

        Helper.Is_Using_Starting_Offset = UseStartingOffset;
        Helper.DisableStartingOffsetString = DisableStartingOffsetString;

        string mapcode = Helper.Credits + "\n" + $"void function {SceneManager.GetActiveScene().name.Replace(" ", "_")}()" + "\n{\n" +  Helper.ShouldAddStartingOrg(1);
        if(onlyMapCode)
            mapcode = Helper.ShouldAddStartingOrg(1);

        //Build Map Code
        mapcode += Helper.BuildMapCode(Helper.GenerateButtons, Helper.GenerateJumppads, Helper.GenerateBubbleShields, Helper.GenerateWeaponRacks, Helper.GenerateLootBins, Helper.GenerateZipLines, Helper.GenerateDoors, Helper.GenerateProps, Helper.GenerateTriggers);

        if(!onlyMapCode)
            mapcode += "}";
     
        if(copyCode) {
            GUIUtility.systemCopyBuffer = mapcode;
            mapcode = "";
            return;
        }

        code_text = mapcode;
        mapcode = "";

        ReMapConsole.Log("[Code Views] Map Code Generated", ReMapConsole.LogType.Success);
    }

    void GenerateDataTable(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string tableCode = Build.Props(Build.BuildType.DataTable);

        if (copycode_text) {
            GUIUtility.systemCopyBuffer = tableCode;
            tableCode = "";
            return;
        }
        
        code_text = tableCode;
        tableCode = "";

        ReMapConsole.Log("[Code Views] Datatable Code Generated", ReMapConsole.LogType.Success);
    }

    void GeneratePrecacheCode(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string precacheCode = Build.Props(Build.BuildType.Precache);
     
        if(copycode_text) {
            GUIUtility.systemCopyBuffer = precacheCode;
            precacheCode = "";
            return;
        }

        code_text = precacheCode;
        precacheCode = "";

        ReMapConsole.Log("[Code Views] Precache Code Generated", ReMapConsole.LogType.Success);
    }

    void GenerateEntCode(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build.Props(Build.BuildType.Ent);

        if(copycode_text) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        code_text = entCode;
        entCode = "";

        ReMapConsole.Log("[Code Views] Script.ent Code Generated", ReMapConsole.LogType.Success);
    }

    void GenerateSoundEntCode(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build.Sounds();

        if(copycode_text) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        code_text = entCode;
        entCode = "";

        ReMapConsole.Log("[Code Views] Sound.ent Code Generated", ReMapConsole.LogType.Success);
    }

    void GenerateNewLocPairCode(bool copycode_text)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build.NewLocPair();

        if(copycode_text) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        code_text = entCode;
        entCode = "";

        ReMapConsole.Log("[Code Views] NewLocPair Code Generated", ReMapConsole.LogType.Success);
    }
}