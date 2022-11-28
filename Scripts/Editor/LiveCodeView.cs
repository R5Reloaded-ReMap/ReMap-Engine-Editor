using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CopyPasteCode : EditorWindow
{
    static bool OnlyExportMap = true;
    static bool UseStartingOffset = false;
    static bool OverrideTextLimit = false;
    static string text = "";
    static Vector2 scroll;
    static string StartFunction;

    [MenuItem("R5Reloaded/Map Code", false, 25)]
    static void Init()
    {
        CopyPasteCode window = (CopyPasteCode)GetWindow(typeof(CopyPasteCode), false, "Map Code");
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
        int finalcount = Helper.GetPropCount();

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
            GUILayout.Label("Entity Status: Warning! Game could crash!");
        GUILayout.EndVertical();
            
        GUI.contentColor = Color.white;

        GUILayout.BeginVertical("box");
        OnlyExportMap = EditorGUILayout.Toggle("Only Show Map Code", OnlyExportMap);
        UseStartingOffset = EditorGUILayout.Toggle("Use Map Origin Offset", UseStartingOffset);
        GUILayout.EndVertical();

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
                GenerateMap(OnlyExportMap, true);

            GUI.contentColor = Color.yellow;
            GUILayout.Label("Text area disabled, please use the copy button!");
            GUI.contentColor = Color.white;
        }
        else
        {
            if (GUILayout.Button("Copy"))
                GenerateMap(OnlyExportMap, true);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GenerateMap(OnlyExportMap, false);
            GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// Generates map code
    /// </summary>
    /// <param name="onlymap">code w/o function</param>
    /// <param name="copytext">copy to clipboard</param>
    void GenerateMap(bool onlymap, bool copytext)
    {
        BuildStartingString();
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string mapcode = Helper.Credits + "\n" + StartFunction +  Helper.ShouldAddStartingOrg(UseStartingOffset, 1);

        if(onlymap)
            mapcode = Helper.ShouldAddStartingOrg(UseStartingOffset, 1);

        //Build Map Code
        mapcode += Helper.BuildMapCode(UseStartingOffset);

        if(!onlymap)
            mapcode += "}";
     
        if(copytext) {
            GUIUtility.systemCopyBuffer = mapcode;

            if(mapcode.Length > 75000)
                mapcode = "";

            return;
        }

        text = mapcode;
        mapcode = "";
    }

    /// <summary>
    /// Builds map starting function
    /// </summary>
    void BuildStartingString()
    {
        Scene scene = SceneManager.GetActiveScene();
        StartFunction = @"void function " + scene.name.Replace(" ", "_") + "()" + "\n" + "{" + "\n";
    }
}