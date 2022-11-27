using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PrecacheCode : EditorWindow
{
    static bool OverrideTextLimit = false;
    static string text = "";
    static Vector2 scroll;

    [MenuItem("R5Reloaded/Precache Code", false, 25)]
    static void Init()
    {
        PrecacheCode window = (PrecacheCode)GetWindow(typeof(PrecacheCode), false, "Precache Code");
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
        if (GUILayout.Button("Copy"))
            GeneratePrecacheCode(true);

        if (text.Length > 75000)
        {
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Export is to long to show in the textbox, Please copy using the button above!");
            GUI.contentColor = Color.white;
            OverrideTextLimit = EditorGUILayout.Toggle("Override Text Limit", OverrideTextLimit);
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);
        if(text.Length > 75000 && !OverrideTextLimit) {
            GUI.contentColor = Color.yellow;
            GUILayout.TextArea("Output code is to long. You can override this with the toggle above. \nWARNING: MAY CAUSE LAG!", GUILayout.ExpandHeight(true));
            GUI.contentColor = Color.white;
        }
        else
        {
            GeneratePrecacheCode(false);
            GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        }
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

        string saved = "";

        //Generate All Props
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
            
        foreach(GameObject go in PropObjects) {
            string[] splitArray = go.name.Split(char.Parse(" "));
            string finished = splitArray[0].Replace("#", "/") + ".rmdl";

            if(saved.Contains(finished))
                continue;
            
            saved += "    PrecacheModel( $\"" + finished + "\"" + ")" + "\n";
        }
     
        if(copytext) {
            GUIUtility.systemCopyBuffer = saved;

            if(saved.Length > 75000)
                saved = "";
        } else {
            text = saved;
            saved = "";
        }
    }
}