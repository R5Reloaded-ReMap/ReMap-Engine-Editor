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

        Helper.UseStartingOffset = false;
        Helper.ShowStartingOffset = false;

        string mapcode = "";
        if(onlyMapCode)
            mapcode = Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction );

        //Build Map Code
        mapcode += Helper.BuildMapCode( BuildType.Script, true,
            Helper.GetBoolFromGenerateObjects( ObjectType.Prop ), Helper.GetBoolFromGenerateObjects( ObjectType.ZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.LinkedZipline ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalZipLine ), Helper.GetBoolFromGenerateObjects( ObjectType.NonVerticalZipLine ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SingleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.DoubleDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.HorzDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.VerticalDoor ), Helper.GetBoolFromGenerateObjects( ObjectType.Button ),
            Helper.GetBoolFromGenerateObjects( ObjectType.Jumppad ), Helper.GetBoolFromGenerateObjects( ObjectType.LootBin ), Helper.GetBoolFromGenerateObjects( ObjectType.WeaponRack ), Helper.GetBoolFromGenerateObjects( ObjectType.Trigger ), Helper.GetBoolFromGenerateObjects( ObjectType.BubbleShield ),
            Helper.GetBoolFromGenerateObjects( ObjectType.SpawnPoint ), Helper.GetBoolFromGenerateObjects( ObjectType.TextInfoPanel ), Helper.GetBoolFromGenerateObjects( ObjectType.FuncWindowHint ), Helper.GetBoolFromGenerateObjects( ObjectType.Sound )
        );

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