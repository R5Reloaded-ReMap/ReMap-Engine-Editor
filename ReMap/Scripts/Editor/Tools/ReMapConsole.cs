using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ReMapConsole : EditorWindow
{
    [MenuItem("ReMap/Debug/Console", false, 100)]
    public static void Init()
    {
        ReMapConsole window = (ReMapConsole)EditorWindow.GetWindow(typeof(ReMapConsole), false, "ReMap Debug Console");
        window.minSize = new Vector2(300, 360);
        window.Show();
    }

    public enum LogType { Error, Warning, Success, Info };
    public static Dictionary<LogType, string> consolecolors = new Dictionary<LogType, string>(){
        {LogType.Error, "#ff462e"},
        {LogType.Warning, "#ccc92d"},
        {LogType.Success, "#2dcc7f"},
        {LogType.Info, "#e6e6e6"}
    };

    static Vector2 scroll = new Vector2(0, 0);
    static string consolelines = "";

    GUIStyle style = new GUIStyle ();

    bool locktoend = true;

    void OnGUI()
    {
        style.richText = true;
        style.fontSize = 15;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Console", GUILayout.Width(100)))
            consolelines = "";
        locktoend = GUILayout.Toggle(locktoend, "Auto Scroll");
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical("box");
        scroll = GUILayout.BeginScrollView(scroll, true, true);
        GUILayout.TextArea(consolelines, style, GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        if(locktoend) {
            scroll.y = consolelines.Length;
            Repaint();
        }
    }

    public static void Log(string message, LogType color)
    {
        string hexcolor = consolecolors[color];
        consolelines += "<color=" + hexcolor + ">" + message + "</color>\n";
    }
}