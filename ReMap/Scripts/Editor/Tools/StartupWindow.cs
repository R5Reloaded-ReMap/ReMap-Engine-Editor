using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class StartupWindow : EditorWindow {

    static string currentDirectory = Directory.GetCurrentDirectory();
    static string relativeConfigFile = $"Assets/ReMap/Resources/startupconfig.json";

    public static void Init()
    {
        StartupWindow window = (StartupWindow)EditorWindow.GetWindow(typeof(StartupWindow), false, "ReMap Info");
        window.minSize = new Vector2(300, 360);
        window.maxSize = new Vector2(300, 360);//
        window.Show();
    }

    void OnGUI()
    {
        GUI.contentColor = Color.white;
        GUILayout.BeginVertical("box");
        GUILayout.Label("Startup Window", EditorStyles.boldLabel);
        GUILayout.Label("Work in progress...", EditorStyles.boldLabel);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Close"))
            EditorWindow.GetWindow(typeof(StartupWindow)).Close();
        if (GUILayout.Button("Close and dont show again"))
            CloseAndDontShow();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("ReMap Version: 1.0");
        GUILayout.EndVertical();
    }

    static void CloseAndDontShow()
    {
        if (File.Exists(currentDirectory + "/" + relativeConfigFile))
        {
            string json = System.IO.File.ReadAllText(currentDirectory + "/" + relativeConfigFile);
            if (json != null)
            {
                Root myObject = JsonUtility.FromJson<Root>(json);
                myObject.ShowStartupWindow = false;
                string newJson = JsonUtility.ToJson(myObject);
                System.IO.File.WriteAllText(currentDirectory + "/" + relativeConfigFile, newJson);
            }
        }
        EditorWindow.GetWindow(typeof(StartupWindow)).Close();
    }

    public class Root
    {
        public bool ShowStartupWindow;
    }
}