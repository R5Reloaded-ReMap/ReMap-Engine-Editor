using Build;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SceneViewTools : Editor
{
    public static void Enable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void Disable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private static void OnSceneGUI( SceneView sceneview )
    {
        Handles.BeginGUI();

        GUILayout.BeginArea( new Rect( Screen.width - 210, Screen.height - 80, 200, 70 ) );
        if ( GUILayout.Button( "Copy Map Code" ) )
            CopyMapCode( true, true );
        GUILayout.EndArea();

        // Get camera position
        var cameraPosition = sceneview.camera.transform.position;

        // Set text color to black
        GUI.contentColor = Color.black;

        // Display camera position
        GUI.Label( new Rect( 10, 10, 300, 20 ), "Camera Position: " + Helper.BuildOrigin( cameraPosition ) );

        Handles.EndGUI();
    }

    private static void CopyMapCode( bool onlyMapCode, bool copyCode )
    {
        Helper.FixPropTags();

        Helper.SetUseStartingOffset( false );
        Helper.SetShowStartingOffset( false );

        string mapcode = "";
        if ( onlyMapCode )
            mapcode = Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction );

        Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );

        //Build Map Code
        mapcode += Helper.BuildMapCode( BuildType.Script, true );

        if ( !onlyMapCode )
            mapcode += "}";

        if ( copyCode )
        {
            GUIUtility.systemCopyBuffer = mapcode;
            mapcode = "";
            return;
        }

        ReMapConsole.Log( "[Code Views] Map Code Generated", ReMapConsole.LogType.Success );
    }
}