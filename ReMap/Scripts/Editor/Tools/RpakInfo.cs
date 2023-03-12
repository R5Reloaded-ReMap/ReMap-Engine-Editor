using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class RpakInfo : EditorWindow
{
    static string usedRpak = "";
    static string currentDirectory = LibrarySorterWindow.currentDirectory;
    static string relativeRpakFile = LibrarySorterWindow.relativeRpakFile;

    Vector2 scrollPosition;

    [MenuItem("ReMap/Tools/Rpak Info", false, 100)]
    public static void Init()
    {
        RpakInfo window = (RpakInfo)EditorWindow.GetWindow(typeof(RpakInfo), false, "Rpak Info");
        window.Show();
        //window.minSize = new Vector2(375, 140);
        //window.maxSize = new Vector2(375, 140);
        GetUsedRpak();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        GUILayout.Label("Models Used:");

        scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, GUILayout.Height( 300 ) );

        GUILayout.TextField( usedRpak );

        EditorGUILayout.EndScrollView();
    }

    void Update()
    {
        
    }

    static void GetUsedRpak()
    {
        foreach ( string models in UnityInfo.GetModelsListInScene() )
        {
            usedRpak += $"{models}\n";
        }
    }
}