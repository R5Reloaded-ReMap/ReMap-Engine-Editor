using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class LiveDataTable : EditorWindow
{
    static bool OverrideTextLimit = false;
    static string text = "";
    static Vector2 scroll;

    [MenuItem("R5Reloaded/DataTable View", false, 25)]
    static void Init()
    {
        LiveDataTable window = (LiveDataTable)GetWindow(typeof(LiveDataTable), false, "Datatable View");
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
    /// Generates map precache code
    /// </summary>
    /// <param name="copytext">copy to clipboard</param>
    void GenerateDataTable(bool copytext)
    {
        Helper.FixPropTags();

        EditorSceneManager.SaveOpenScenes();

        //Generate All Props
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");

        string saved = "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"" + "\n";

        foreach (GameObject go in PropObjects)
        {
            saved += Helper.BuildDataTableItem(go);
        }

        saved += "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"";

        if (copytext)
        {
            GUIUtility.systemCopyBuffer = saved;

            if (saved.Length > 75000)
                saved = "";
        }
        else
        {
            text = saved;
            saved = "";
        }
    }
}