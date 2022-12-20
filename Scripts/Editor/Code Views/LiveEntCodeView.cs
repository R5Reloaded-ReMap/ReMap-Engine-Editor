using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LiveEntCodeView : EditorWindow
{
    string text = "";
    Vector2 scroll;

    int finalcount = 0;

    [MenuItem("R5Reloaded/script.ent Code View", false, 25)]
    static void Init()
    {
        LiveEntCodeView window = (LiveEntCodeView)GetWindow(typeof(LiveEntCodeView), false, "Live script.ent Code");
        window.Show();
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        finalcount = PropObjects.Length;
        GenerateEntCode(false);
    }

    private void EditorSceneManager_sceneSaved(UnityEngine.SceneManagement.Scene arg0)
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        finalcount = PropObjects.Length;
        GenerateEntCode(false);
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        finalcount = PropObjects.Length;
        GenerateEntCode(false);
    }

    void OnGUI()
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

    /// <summary>
    /// Generates Ent Code String
    /// </summary>
    /// <param name="copytext">copy text to clipboard</param>
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
}