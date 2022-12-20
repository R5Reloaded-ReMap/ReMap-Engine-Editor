using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class CopyPasteCode : EditorWindow
{
    bool OnlyExportMap = true;
    bool temp1 = true;
    bool UseStartingOffset = false;
    bool temp2 = false;
    static bool DisableStartingOffsetString = false;
    bool temp3 = false;
    static string text = "";
    static Vector2 scroll;

    static int finalcount = 0;

    [MenuItem("R5Reloaded/Map Code View", false, 25)]
    static void Init()
    {
        CopyPasteCode window = (CopyPasteCode)GetWindow(typeof(CopyPasteCode), false, "Map Code");
        window.Show();
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

        finalcount = Helper.GetPropCount();
        GenerateMap(OnlyExportMap, false);
    }

    private void EditorSceneManager_sceneSaved(UnityEngine.SceneManagement.Scene arg0)
    {
        finalcount = Helper.GetPropCount();
        GenerateMap(OnlyExportMap, false);
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        finalcount = Helper.GetPropCount();
        GenerateMap(OnlyExportMap, false);
    }

    void OnGUI()
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
            DisableStartingOffsetString = EditorGUILayout.Toggle("Don't Show Startingorg", DisableStartingOffsetString);
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

    /// <summary>
    /// Generates map code
    /// </summary>
    /// <param name="onlyMapCode">code w/o function</param>
    /// <param name="copyCode">copy to clipboard</param>
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
}