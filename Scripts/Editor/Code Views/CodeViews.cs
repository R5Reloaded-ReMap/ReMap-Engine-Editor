using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class CodeViews : EditorWindow
{
    bool OnlyExportMap = true;
    bool temp1 = true;
    bool UseStartingOffset = false;
    bool temp2 = false;
    static bool DisableStartingOffsetString = false;
    bool temp3 = false;
    static string text = "";
    static Vector2 scroll;
    int tab = 0;
    int temptab = 0;
    bool UseOriginOffset = false;
    Vector3 OriginOffset;

    static int finalcount = 0;

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

        GenerateCorrectCode();
    }

    private void EditorSceneManager_sceneSaved(UnityEngine.SceneManagement.Scene arg0)
    {
        GenerateCorrectCode();
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        GenerateCorrectCode();
    }

    void GenerateCorrectCode()
    {
        switch (tab) {
            case 0: //Map Code
                finalcount = Helper.GetPropCount();
                GenerateMap(OnlyExportMap, false);
                break;
            case 1: //DataTable Code
                GenerateDataTable(false);
                break;
            case 2: //Precache Code
                GeneratePrecacheCode(false);
                break;
            case 3: //Script.ent Code
                GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
                finalcount = PropObjects.Length;
                GenerateEntCode(false);
                break;
            case 4: //Sound.ent Code
                GameObject[] SoundObjects = GameObject.FindGameObjectsWithTag("Sound");
                finalcount = SoundObjects.Length;
                GenerateSoundEntCode(false);
                break;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
        tab = GUILayout.Toolbar (tab, new string[] {"Map Code", "DataTable Code", "Precache Code", "Script.ent Code", "Sound.ent Code"});
        GUILayout.EndVertical();

        switch (tab) {
            case 0:
                MapCodeGUI();
                if (tab != temptab)
                {
                    finalcount = Helper.GetPropCount();
                    GenerateMap(OnlyExportMap, false);
                }
                temptab = tab;
                break;
            case 1:
                DataTableGUI();
                if(tab != temptab)
                    GenerateDataTable(false);
                temptab = tab;
                break;
            case 2:
                PrecacheGUI();
                if(tab != temptab)
                    GeneratePrecacheCode(false);
                temptab = tab;
                break;
            case 3: //Script.ent Code
                EntCodeGUI();
                if (tab != temptab)
                {
                    GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
                    finalcount = PropObjects.Length;
                    GenerateEntCode(false);
                }
                temptab = tab;
                break;
            case 4: //Sound.ent Code
                SoundEntGUI();
                if (tab != temptab)
                {
                    GameObject[] SoundObjects = GameObject.FindGameObjectsWithTag("Sound");
                    finalcount = SoundObjects.Length;
                    GenerateSoundEntCode(false);
                }
                temptab = tab;
                break;
        }
    }

    void MapCodeGUI()
    {
        if(finalcount < Helper.greenPropCount)
            GUI.contentColor = Color.green;
        else if((finalcount < Helper.yellowPropCount)) 
            GUI.contentColor = Color.yellow;
        else
            GUI.contentColor = Color.red;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Entity Count: " + finalcount);
        if(finalcount < Helper.greenPropCount)
            GUILayout.Label("Entity Status: Safe");
        else if((finalcount < Helper.yellowPropCount)) 
            GUILayout.Label("Entity Status: Safe");
        else
            GUILayout.Label("Entity Status: Warning! Game could crash!");
        GUILayout.EndVertical();
            
        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");

        OnlyExportMap = EditorGUILayout.Toggle("Only Show Map Code", OnlyExportMap);
        if(OnlyExportMap != temp1) {
            temp1 = OnlyExportMap;
            GenerateMap(OnlyExportMap, false);
        }

        UseStartingOffset = EditorGUILayout.Toggle("Use Map Origin Offset", UseStartingOffset);
        if(UseStartingOffset != temp2) {
            temp2 = UseStartingOffset;
            GenerateMap(OnlyExportMap, false);
        }

        if(UseStartingOffset) 
        {
            DisableStartingOffsetString = EditorGUILayout.Toggle("    Hide Starting Vector", DisableStartingOffsetString);
            if(DisableStartingOffsetString != temp3) {
                temp3 = DisableStartingOffsetString;
                GenerateMap(OnlyExportMap, false);
            }
        }

        GUILayout.EndVertical();

        if (GUILayout.Button("Copy"))
            GenerateMap(OnlyExportMap, true);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void DataTableGUI()
    {
        if (GUILayout.Button("Copy"))
            GenerateDataTable(true);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void PrecacheGUI()
    {
        if (GUILayout.Button("Copy"))
            GeneratePrecacheCode(true);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void EntCodeGUI()
    {
        if(finalcount < Helper.greenPropCount)
            GUI.contentColor = Color.green;
        else if(finalcount < Helper.yellowPropCount)
            GUI.contentColor = Color.yellow;
        else
            GUI.contentColor = Color.red;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Entity Count: " + finalcount);

        if(finalcount < Helper.greenPropCount)
            GUILayout.Label("Entity Status: Safe");
        else if((finalcount < Helper.yellowPropCount)) 
            GUILayout.Label("Entity Status: Safe");
        else
            GUILayout.Label("Entity Status: Warning! Game might crash!");
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        if (GUILayout.Button("Copy Code"))
            GenerateEntCode(true);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void SoundEntGUI()
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Sound");
        int finalcount = PropObjects.Length;

        if(finalcount < Helper.greenPropCount)
            GUI.contentColor = Color.green;
        else if(finalcount < Helper.yellowPropCount)
            GUI.contentColor = Color.yellow;
        else
            GUI.contentColor = Color.red;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Entity Count: " + finalcount);

        if(finalcount < Helper.greenPropCount)
            GUILayout.Label("Entity Status: Safe");
        else if((finalcount < Helper.yellowPropCount)) 
            GUILayout.Label("Entity Status: Safe");
        else
            GUILayout.Label("Entity Status: Warning! Game might crash!");
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        UseOriginOffset = EditorGUILayout.Toggle("Add a origin offset", UseOriginOffset);
        if(UseOriginOffset) OriginOffset = EditorGUILayout.Vector3Field("Origin Offset", OriginOffset);
        GUILayout.EndVertical();

        if (GUILayout.Button("Copy Code"))
            GenerateEntCode(true);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
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

        text = mapcode;
        mapcode = "";
    }

    void GenerateDataTable(bool copytext)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string tableCode = Build.Props(Build.BuildType.DataTable);

        if (copytext) {
            GUIUtility.systemCopyBuffer = tableCode;
            tableCode = "";
            return;
        }
        
        text = tableCode;
        tableCode = "";
    }

    void GeneratePrecacheCode(bool copytext)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string precacheCode = Build.Props(Build.BuildType.Precache);
     
        if(copytext) {
            GUIUtility.systemCopyBuffer = precacheCode;
            precacheCode = "";
            return;
        }

        text = precacheCode;
        precacheCode = "";
    }

    void GenerateEntCode(bool copytext)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build.Props(Build.BuildType.Ent);

        if(copytext) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        text = entCode;
        entCode = "";
    }

    void GenerateSoundEntCode(bool copytext)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build.Sounds();

        if(copytext) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        text = entCode;
        entCode = "";
    }
}