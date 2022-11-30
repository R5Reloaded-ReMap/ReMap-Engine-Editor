using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LiveDataTableView : EditorWindow
{
    static bool OverrideTextLimit = false;
    static string text = "";
    static Vector2 scroll;

    [MenuItem("R5Reloaded/DataTable View", false, 25)]
    static void Init()
    {
        LiveDataTableView window = (LiveDataTableView)GetWindow(typeof(LiveDataTableView), false, "Datatable View");
        window.Show();
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        OverrideTextLimit = false;
        text = "";
    }

    void OnGUI()
    {
        if (text.Length > Helper.maxBuildLength)
        {
            GUILayout.BeginVertical("box");
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Output code is longer then the text limit. You can override this with the toggle above. \nWARNING: MAY CAUSE LAG!");
            GUI.contentColor = Color.white;
            OverrideTextLimit = EditorGUILayout.Toggle("Override Text Limit", OverrideTextLimit);
            GUILayout.EndVertical();
        }
        
        if(text.Length > Helper.maxBuildLength && !OverrideTextLimit) {
            if (GUILayout.Button("Copy"))
                GenerateDataTable(true);

            GUI.contentColor = Color.yellow;
            GUILayout.Label("Text area disabled, please use the copy button!");
            GUI.contentColor = Color.white;
        }
        else
        {
            if (GUILayout.Button("Copy"))
                GenerateDataTable(true);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            GenerateDataTable(false);
            GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// Generates datatable export
    /// </summary>
    /// <param name="copytext">copy to clipboard</param>
    void GenerateDataTable(bool copytext)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string tableCode = Build.Props(true, Build.BuildType.DataTable);

        if (copytext) {
            GUIUtility.systemCopyBuffer = tableCode;
            tableCode = "";
            return;
        }
        
        text = tableCode;
        tableCode = "";
    }
}