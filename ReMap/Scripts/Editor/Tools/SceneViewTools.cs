 using UnityEditor;
 using UnityEngine;

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

        Helper.Is_Using_Starting_Offset = false;
        Helper.DisableStartingOffsetString = false;

        string mapcode = "";
        if(onlyMapCode)
            mapcode = Helper.ShouldAddStartingOrg(1);

        //Build Map Code
        mapcode += Helper.BuildMapCode(Helper.GenerateButtons, Helper.GenerateJumppads, Helper.GenerateBubbleShields, Helper.GenerateWeaponRacks, Helper.GenerateLootBins, Helper.GenerateZipLines, Helper.GenerateDoors, Helper.GenerateProps, Helper.GenerateTriggers, Helper.GenerateTextInfoPanel);

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