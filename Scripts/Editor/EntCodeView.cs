using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;

public class EntCodeView : EditorWindow
{
    string text = "";
    bool OverrideTextLimit = false;
    Vector2 scroll;

    [MenuItem("R5Reloaded/script.ent Code", false, 25)]
    static void Init()
    {
        EntCodeView window = (EntCodeView)GetWindow(typeof(EntCodeView), false, "Live script.ent Code");
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

        if(finalcount < 1500)
            GUI.contentColor = Color.green;
        else if((finalcount < 3000)) 
            GUI.contentColor = Color.yellow;
        else
            GUI.contentColor = Color.red;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Entity Count: " + finalcount);

        if(finalcount < 1500)
            GUILayout.Label("Entity Status: Safe");
        else if((finalcount < 3000)) 
            GUILayout.Label("Entity Status: Safe");
        else
            GUILayout.Label("Entity Status: Warning! Game might crash!");
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        if (text.Length > 75000)
        {
            GUILayout.BeginVertical("box");
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Output code is longer then the text limit. You can override this with the toggle above. \nWARNING: MAY CAUSE LAG!");
            GUI.contentColor = Color.white;
            OverrideTextLimit = EditorGUILayout.Toggle("Override Text Limit", OverrideTextLimit);
            GUILayout.EndVertical();
        }

        if(text.Length > 75000 && !OverrideTextLimit) {
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
        string saved = "";

        //Generate All Props
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        foreach(GameObject go in PropObjects) {
            if (!go.activeInHierarchy)
                continue;

            string[] splitArray = go.name.Split(char.Parse(" "));
            string finished = splitArray[0].Replace("#", "/") + ".rmdl";

            saved += Helper.BuildScriptEnt(go);
        }

        if(copytext) {
            GUIUtility.systemCopyBuffer = saved;

            if(saved.Length > 50000)
                saved = "";

            return;
        }
        
        
        text = saved;
    }
}