using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InfoUI : EditorWindow
{
    [MenuItem("R5Reloaded/Info", false, 0)]
    static void Init()
    {
        InfoUI window = (InfoUI)EditorWindow.GetWindow(typeof(InfoUI), false, "R5R Unity Map Editor");
        window.Show();
    }

    void OnGUI()
    {
        GUI.contentColor = Color.white;
        GUILayout.BeginVertical("box");
        GUILayout.Label("Credits:", EditorStyles.boldLabel);
        GUILayout.Label("Unity Scripts/Export Code/Custom Prefabs:", EditorStyles.boldLabel);
        GUILayout.Label("  AyeZee#6969 \n");
        GUILayout.Label("Importing/Organizing All Models:", EditorStyles.boldLabel);
        GUILayout.Label("  Julefox#0050 \n");
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Map Editor Docs:", EditorStyles.boldLabel);
        GUILayout.TextArea("https://docs.r5reloaded.com/");
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Feel free to donate and show support!\n", EditorStyles.boldLabel);
        GUILayout.Label("Donations are not required by anymeans,\njust a way to show support for a project. :)");
        GUILayout.Label(" ", EditorStyles.boldLabel);
        GUILayout.TextArea("https://ko-fi.com/ayezee");
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Version: 1.0");
        GUILayout.EndVertical();
    }
}