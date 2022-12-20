using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LivePrecacheView : EditorWindow
{
    static string text = "";
    static Vector2 scroll;

    [MenuItem("R5Reloaded/Precache Code View", false, 25)]
    static void Init()
    {
        LivePrecacheView window = (LivePrecacheView)GetWindow(typeof(LivePrecacheView), false, "Precache Code");
        window.Show();
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

        GeneratePrecacheCode(false);
    }

    private void EditorSceneManager_sceneSaved(UnityEngine.SceneManagement.Scene arg0)
    {
        GeneratePrecacheCode(false);
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        GeneratePrecacheCode(false);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Copy"))
            GeneratePrecacheCode(true);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Generates map precache code
    /// </summary>
    /// <param name="copytext">copy to clipboard</param>
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
}