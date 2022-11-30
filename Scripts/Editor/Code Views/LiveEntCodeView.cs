using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LiveEntCodeView : EditorWindow
{
    string text = "";
    bool OverrideTextLimit = false;
    Vector2 scroll;

    [MenuItem("R5Reloaded/script.ent Code View", false, 25)]
    static void Init()
    {
        LiveEntCodeView window = (LiveEntCodeView)GetWindow(typeof(LiveEntCodeView), false, "Live script.ent Code");
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
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
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
            if (GUILayout.Button("Copy Code"))
                GenerateEntCode(true);

            GUI.contentColor = Color.yellow;
            GUILayout.Label("Text area disabled, please use the copy button!");
            GUI.contentColor = Color.white;
        }
        else
        {
            if (GUILayout.Button("Copy Code"))
                GenerateEntCode(true);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            GenerateEntCode(false);
            GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// Generates Ent Code String
    /// </summary>
    /// <param name="copytext">copy text to clipboard</param>
    void GenerateEntCode(bool copytext)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();
        
        string entCode = Build.Props(true, Build.BuildType.Ent);

        if(copytext) {
            GUIUtility.systemCopyBuffer = entCode;
            entCode = "";
            return;
        }
        
        text = entCode;
        entCode = "";
    }
}