 using UnityEditor;
 using UnityEngine;

 using Build;

[InitializeOnLoad]
 public class SceneViewTools : Editor
 {
    static SceneViewTools()
    {
        //SceneView.duringSceneGui += OnSceneGUI;
    }

    [MenuItem("ReMap/Tools/SceneViewGUI/Enable")]
    public static void Enable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
 
    [MenuItem("ReMap/Tools/SceneViewGUI/Disable")]
    public static void Disable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
 
    private static void OnSceneGUI(SceneView sceneview)
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(Screen.width - 210, Screen.height - 80, 200, 70));
        if (GUILayout.Button("Copy Map Code"))
            CopyMapCode(true,true);
        GUILayout.EndArea();
        
 
        Handles.EndGUI();
    }

    static void CopyMapCode(bool onlyMapCode, bool copyCode)
    {
        Helper.FixPropTags();

        Helper.SetUseStartingOffset( false );
        Helper.SetShowStartingOffset( false );

        string mapcode = "";
        if(onlyMapCode)
            mapcode = Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction );

        Helper.ForceHideBoolToGenerateObjects( new ObjectType[0] );

        //Build Map Code
        mapcode += Helper.BuildMapCode( BuildType.Script, true );

        if(!onlyMapCode)
            mapcode += "}";
     
        if(copyCode) {
            GUIUtility.systemCopyBuffer = mapcode;
            mapcode = "";
            return;
        }

        ReMapConsole.Log("[Code Views] Map Code Generated", ReMapConsole.LogType.Success);
    }
 }