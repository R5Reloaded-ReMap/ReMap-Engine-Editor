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

    Object source;
    Vector2 scroll;

    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        GUI.contentColor = Color.white;
        GUILayout.Label(" ", EditorStyles.boldLabel);
        GUILayout.Label("Credits:", EditorStyles.boldLabel);
        GuiLine(1);
        GUILayout.Label("   Unity Scripts/Map Export Code/Custom Prefabs:", EditorStyles.boldLabel);
        GUILayout.Label("       AyeZee#6969 \n");
        GUILayout.Label("   Importing/Organizing All Models:", EditorStyles.boldLabel);
        GUILayout.Label("       Julefox#0050 \n");
        GUILayout.Label("Map Editor Docs:", EditorStyles.boldLabel);
        GuiLine(1);
        GUILayout.TextArea("https://docs.r5reloaded.com/");
        GUILayout.Label(" ");
        GUILayout.Label("Feel free to donate and show support!", EditorStyles.boldLabel);
        GuiLine(1);
        GUILayout.Label("   Donations are not required by anymeans, \n   just a way to show support for a project. :)");
        GUILayout.Label(" ", EditorStyles.boldLabel);
        GUILayout.TextArea("https://ko-fi.com/ayezee");
        GUILayout.Label("\n\n\n\n");
        GUILayout.Label("Version: 1.0");
        EditorGUILayout.EndScrollView();
    }

    void GuiLine( int i_height = 1 )
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height );

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );

    }
}