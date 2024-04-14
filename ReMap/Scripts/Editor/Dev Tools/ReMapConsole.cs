using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReMapConsole : EditorWindow
{
    public enum LogType
    {
        Error,
        Warning,
        Success,
        Info
    }

    public static Dictionary< LogType, string > consolecolors = new()
    {
        { LogType.Error, "#ff462e" },
        { LogType.Warning, "#ccc92d" },
        { LogType.Success, "#2dcc7f" },
        { LogType.Info, "#e6e6e6" }
    };

    private static Vector2 scroll = new(0, 0);
    private static string consolelines = "";

    private bool locktoend = true;

    private readonly GUIStyle style = new();

    public static void Init()
    {
        var window = ( ReMapConsole )GetWindow( typeof(ReMapConsole), false, "ReMap Debug Console" );
        window.minSize = new Vector2( 300, 360 );
        window.Show();
    }

    private void OnGUI()
    {
        style.richText = true;
        style.fontSize = 15;

        GUILayout.BeginHorizontal();
        if ( GUILayout.Button( "Clear Console", GUILayout.Width( 100 ) ) )
            consolelines = "";
        locktoend = GUILayout.Toggle( locktoend, "Auto Scroll" );
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical( "box" );
        scroll = GUILayout.BeginScrollView( scroll, true, true );
        GUILayout.TextArea( consolelines, style, GUILayout.ExpandHeight( true ) );
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        if ( locktoend )
        {
            scroll.y = consolelines.Length;
            Repaint();
        }
    }

    public static void Log( string message, LogType color )
    {
        string hexcolor = consolecolors[color];
        consolelines += "<color=" + hexcolor + ">" + message + "</color>\n";
    }
}