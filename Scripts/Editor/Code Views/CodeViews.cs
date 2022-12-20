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
    bool UseOriginOffset = false;
    Vector3 OriginOffset;

    //Counts
    int mapcodecount = 0;
    int datatablecount = 0;
    int scriptentcount = 0;
    int soundentcount = 0;

    [MenuItem("R5Reloaded/Code Views", false, 25)]
    static void Init()
    {
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
        mapcodecount = Helper.GetAllCount();
        datatablecount = Helper.GetPropCount();
        scriptentcount = Helper.GetPropCount();
        soundentcount = Helper.GetSoundCount();
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
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
                SetCorrectColor(mapcodecount);
                GUILayout.Label("Map Code Entity Count: " + mapcodecount, EditorStyles.boldLabel);
                SetCorrectColor(datatablecount);
                GUILayout.Label("DataTable Entity Count: " + datatablecount, EditorStyles.boldLabel);
                SetCorrectColor(scriptentcount);
                GUILayout.Label("Script.ent Entity Count: " + scriptentcount, EditorStyles.boldLabel);
                SetCorrectColor(soundentcount);
                GUILayout.Label("Sound.ent Entity Count: " + soundentcount, EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
                SetCorrectColor(mapcodecount);
                SetCorrectEntityLabel(mapcodecount);
                SetCorrectColor(datatablecount);
                SetCorrectEntityLabel(datatablecount);
                SetCorrectColor(scriptentcount);
                SetCorrectEntityLabel(scriptentcount);
                SetCorrectColor(soundentcount);
                SetCorrectEntityLabel(soundentcount);
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        tab = GUILayout.Toolbar (tab, new string[] {"Map Code", "DataTable Code", "Precache Code", "Script.ent Code", "Sound.ent Code"});
        GUILayout.EndVertical();

        if(tab != tab_temp)
            GetLatestCounts();

        switch (tab) {
            case 0:
                MapCodeGUI();
                if (tab != tab_temp)
                    GenerateMap(OnlyExportMap, false);
                tab_temp = tab;
                break;
            case 1:
                DataTableGUI();
                if(tab != tab_temp)
                    GenerateDataTable(false);
                tab_temp = tab;
                break;
            case 2:
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
        if(UseStartingOffset) {
            DisableStartingOffsetString = EditorGUILayout.Toggle("    Hide Starting Vector", DisableStartingOffsetString);
            if(DisableStartingOffsetString != DisableStartingOffsetString_temp) {
                DisableStartingOffsetString_temp = DisableStartingOffsetString;
                GenerateMap(OnlyExportMap, false);
            }
        }
        GUILayout.EndVertical();

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
            GenerateEntCode(true);
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
        mapcode += Helper.BuildMapCode();

        if(!onlyMapCode)
            mapcode += "}";
     
        if(copyCode) {
            GUIUtility.systemCopyBuffer = mapcode;
            mapcode = "";
            return;
        }

        code_text = mapcode;
        mapcode = "";
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
    }
}