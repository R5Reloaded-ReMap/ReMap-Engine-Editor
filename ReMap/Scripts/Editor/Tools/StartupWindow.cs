using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class StartupWindow : EditorWindow
{
    private static readonly string currentDirectory = Directory.GetCurrentDirectory();
    private static readonly string relativeConfigFile = "Assets/ReMap/Resources/startupconfig.json";

    public static void Init()
    {
        var window = ( StartupWindow )GetWindow( typeof(StartupWindow), false, "ReMap Info" );
        window.minSize = new Vector2( 300, 360 );
        window.maxSize = new Vector2( 300, 360 ); //
        window.Show();
    }

    private void OnGUI()
    {
        GUI.contentColor = Color.white;
        GUILayout.BeginVertical( "box" );
        GUILayout.Label( "Startup Window", EditorStyles.boldLabel );
        GUILayout.Label( "Work in progress...", EditorStyles.boldLabel );
        GUILayout.EndVertical();

        GUILayout.BeginVertical( "box" );
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button( "Close" ) )
            GetWindow( typeof(StartupWindow) ).Close();
        if ( GUILayout.Button( "Close and dont show again" ) )
            CloseAndDontShow();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical( "box" );
        GUILayout.Label( "ReMap Version: 1.0" );
        GUILayout.EndVertical();
    }

    private static void CloseAndDontShow()
    {
        if ( File.Exists( currentDirectory + "/" + relativeConfigFile ) )
        {
            string json = File.ReadAllText( currentDirectory + "/" + relativeConfigFile );
            if ( json != null )
            {
                var myObject = JsonUtility.FromJson< Root >( json );
                myObject.ShowStartupWindow = false;
                string newJson = JsonUtility.ToJson( myObject );
                File.WriteAllText( currentDirectory + "/" + relativeConfigFile, newJson );
            }
        }
        GetWindow( typeof(StartupWindow) ).Close();
    }

    public class Root
    {
        public bool ShowStartupWindow;
    }
}