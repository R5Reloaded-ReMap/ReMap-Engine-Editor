using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LiveSoundCodeView : EditorWindow
{
    string text = "";
    bool UseOriginOffset = false;
    Vector3 OriginOffset;
    Vector2 scroll;

    int finalcount = 0;

    [MenuItem("R5Reloaded/sound.ent Code View", false, 25)]
    static void Init()
    {
        LiveSoundCodeView window = (LiveSoundCodeView)GetWindow(typeof(LiveSoundCodeView), false, "Live sound.ent Code");
        window.Show();
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;

        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Sound");
        finalcount = PropObjects.Length;
        GenerateEntCode(false);
    }

    private void EditorSceneManager_sceneSaved(UnityEngine.SceneManagement.Scene arg0)
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Sound");
        finalcount = PropObjects.Length;
        GenerateEntCode(false);
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Sound");
        finalcount = PropObjects.Length;
        GenerateEntCode(false);
    }

    void OnGUI()
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

    /// <summary>
    /// Generates Ent Code String
    /// </summary>
    /// <param name="copytext">copy text to clipboard</param>
    void GenerateEntCode(bool copytext)
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