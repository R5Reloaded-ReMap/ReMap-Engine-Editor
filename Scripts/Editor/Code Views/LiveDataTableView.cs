using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LiveDataTableView : EditorWindow
{
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
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

        GenerateDataTable(false);
    }

    private void EditorSceneManager_sceneSaved(UnityEngine.SceneManagement.Scene arg0)
    {
        GenerateDataTable(false);
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        GenerateDataTable(false);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Copy"))
            GenerateDataTable(true);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Generates datatable export
    /// </summary>
    /// <param name="copytext">copy to clipboard</param>
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
}