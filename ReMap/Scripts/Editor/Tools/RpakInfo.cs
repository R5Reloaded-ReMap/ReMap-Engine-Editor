using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using DevLibrarySorter;

public class RpakInfo : EditorWindow
{
    static string usedRpak = "";
    static string currentDirectory = LSRelativePath.currentDirectory;
    static string relativeRpakFile = LSRelativePath.relativeRpakFile;

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
        GUILayout.Label("Rpak Used: ");

        GUILayout.TextField(usedRpak/* , GUILayout.Width(200) */);
    }

    void Update()
    {
        
    }

    static void GetUsedRpak()
    {
        string[] files = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where(f => IsNotExcludedFile(f)).ToArray();

        foreach ( GameObject go in UnityInfo.GetAllGameObjectInScene() )
        {
            foreach ( string path in files )
            {
                string fileName = Path.GetFileNameWithoutExtension(path);

                if ( IsContainsModelName( path, go.name ) && !usedRpak.Contains( fileName ) ) usedRpak += $"{fileName}\n";
            }
        }
    }

    private static bool IsContainsModelName( string filePath, string modelName )
    {
        return System.IO.File.ReadAllText(filePath).Contains( modelName.Replace( "#", "/" ) );
    }

    private static bool IsNotExcludedFile(string filePath)
    {
        string fileName = Path.GetFileName(filePath);
        string[] excludedFiles = { "modelAnglesOffset.txt", "lastestFolderUpdate.txt", "all_models.txt" };

        return !excludedFiles.Contains(fileName);
    }
}