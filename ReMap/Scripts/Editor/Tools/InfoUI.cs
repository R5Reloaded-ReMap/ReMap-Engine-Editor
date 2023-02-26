using UnityEngine;
using UnityEditor;

public class InfoUI : EditorWindow
{
    [MenuItem("ReMap/Info", false, 0)]
    static void Init()
    {
        InfoUI window = (InfoUI)EditorWindow.GetWindow(typeof(InfoUI), false, "ReMap Info");
        window.minSize = new Vector2(300, 360);
        window.maxSize = new Vector2(300, 360);
        window.Show();
    }

    void OnGUI()
    {
        GUI.contentColor = Color.white;
        GUILayout.BeginVertical("box");
        GUILayout.Label("Credits:", EditorStyles.boldLabel);
        GUILayout.Label("Unity Scripts/Export Code/Custom Prefabs:", EditorStyles.boldLabel);
        GUILayout.Label("  AyeZee#6969 \n");
        GUILayout.Label("Importing All Models/Scripting Help:", EditorStyles.boldLabel);
        GUILayout.Label("  Julefox#0050");
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Links:", EditorStyles.boldLabel);
        GUILayout.Label("Map Editor Docs:", EditorStyles.boldLabel);
        if (GUILayout.Button("docs.r5reloaded.com"))
            Application.OpenURL("https://docs.r5reloaded.com/");
        GUILayout.Label("Github:", EditorStyles.boldLabel);
        if (GUILayout.Button("github.com/AyeZeeBB/R5R-Unity-Map-Editor"))
            Application.OpenURL("https://github.com/AyeZeeBB/R5R-Unity-Map-Editor");
        GUILayout.Label("Discord:", EditorStyles.boldLabel);
        if (GUILayout.Button("discord.gg/snsyDVa2fn"))
            Application.OpenURL("https://discord.gg/snsyDVa2fn");
        GUILayout.Label("Ko-fi:", EditorStyles.boldLabel);
        GUILayout.Label("If you would like to donate to show support\nfor the project. :)");
        if (GUILayout.Button("ko-fi.com/ayezee"))
            Application.OpenURL("https://ko-fi.com/ayezee");
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Version: 1.0");
        GUILayout.EndVertical();
    }
}