using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;

public class EntCodeView : EditorWindow
{
    string text = "";
    bool OverrideTextLimit = false;
    Vector2 scroll;

    [MenuItem("R5Reloaded/script.ent Code", false, 25)]
    static void Init()
    {
        EntCodeView window = (EntCodeView)GetWindow(typeof(EntCodeView), false, "Live script.ent Code");
        window.Show();
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        OverrideTextLimit = false;
        text = "";
    }

    void OnGUI()
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        int finalcount = PropObjects.Length;

        if(finalcount < 1500)
            GUI.contentColor = Color.green;
        else if((finalcount < 3000)) 
            GUI.contentColor = Color.yellow;
        else
            GUI.contentColor = Color.red;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Entity Count: " + finalcount);

        if(finalcount < 1500)
            GUILayout.Label("Entity Status: Safe");
        else if((finalcount < 3000)) 
            GUILayout.Label("Entity Status: Safe");
        else
            GUILayout.Label("Entity Status: Warning! Game might crash!");
        GUILayout.EndVertical();

        GUI.contentColor = Color.white;

        if (text.Length > 75000)
        {
            GUILayout.BeginVertical("box");
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Output code is longer then the text limit. You can override this with the toggle above. \nWARNING: MAY CAUSE LAG!");
            GUI.contentColor = Color.white;
            OverrideTextLimit = EditorGUILayout.Toggle("Override Text Limit", OverrideTextLimit);
            GUILayout.EndVertical();
        }

        if(text.Length > 75000 && !OverrideTextLimit) {
            if (GUILayout.Button("Copy Code"))
                GenerateMap(true);

            GUI.contentColor = Color.yellow;
            GUILayout.Label("Text area disabled, please use the copy button!");
            GUI.contentColor = Color.white;
        }
        else
        {
            if (GUILayout.Button("Copy Code"))
                GenerateMap(true);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            GenerateMap(false);
            GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    void GenerateMap(bool copytext)
    {
        SetPropTagsItem();
        EditorSceneManager.SaveOpenScenes();
        string saved = "";

        //Generate All Props
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        foreach(GameObject go in PropObjects)
        {
            if (!go.activeInHierarchy)
                continue;

            string[] splitArray = go.name.Split(char.Parse(" "));
            string finished = splitArray[0].Replace("#", "/") + ".rmdl";

            saved += BuildScriptEnt(finished, go);
        }

        if(copytext)
        {
            GUIUtility.systemCopyBuffer = saved;

            if(saved.Length > 50000)
                saved = "";
        }
        else
        {
            text = saved;
        }
    }

    private static string BuildAngles(GameObject go)
    {
        string x = (-WrapAngle(go.transform.eulerAngles.x)).ToString("F4");
        string y = (-WrapAngle(go.transform.eulerAngles.y)).ToString("F4");
        string z = (WrapAngle(go.transform.eulerAngles.z)).ToString("F4");
                    
        string angles = "< " + x.Replace(",", ".") + ", " + y.Replace(",", ".") + ", " + z.Replace(",", ".") + " >";

        return angles;
    }

    private static float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }

    /// <summary>
    /// Builds correct ingame origin from gameobject
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private static string BuildOrigin(GameObject go)
    {
        string x = (-go.transform.position.z).ToString("F4");
        string y = (go.transform.position.x).ToString("F4");
        string z = (go.transform.position.y).ToString("F4");

        string origin = "< " + x.Replace(",", ".") + ", " + y.Replace(",", ".") + ", " + z.Replace(",", ".") + " >";

        return origin;
    }

    /// <summary>
    /// Tags Custom Prefabs so users cant retag a item wrong
    /// </summary>
    private static void SetPropTagsItem()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;

        //Untag all objects
        foreach(GameObject go in allObjects) {
            go.tag = "Untagged";
        }

        //Retag All Objects
        foreach(GameObject go in allObjects) {
            if(go.name.Contains("custom_lootbin")) {
                go.tag = "LootBin";
            } else if(go.name.Contains("custom_zipline")) {
                go.tag = "ZipLine";
            } else if(go.name.Contains("custom_jumppad")) {
                go.tag = "Jumppad";
            } else if(go.name.Contains("custom_linked_zipline")) {
                go.tag = "LinkedZipline";
            } else if(go.name.Contains("custom_single_door")) {
                go.tag = "SingleDoor";
            } else if(go.name.Contains("custom_double_door")) {
                go.tag = "DoubleDoor";
            } else if(go.name.Contains("custom_vertical_door")) {
                go.tag = "VerticalDoor";
            } else if(go.name.Contains("custom_sliding_door")) {
                go.tag = "HorzDoor";
            } else if(go.name.Contains("custom_weaponrack")) {
                go.tag = "WeaponRack";
            } else if(go.name.Contains("custom_button")) {
                go.tag = "Button";
            } else if(go.name.Contains("trigger_cylinder")) {
                go.tag = "Trigger";
            } else if(go.name.Contains("#bb_shield")) {
                go.tag = "BubbleShield";
            } else if(go.name.Contains("mdl")) {
                go.tag = "Prop";
            }
        }
    }

    private static string BuildScriptEnt(string model, GameObject go)
    {
        string scale = go.transform.localScale.x.ToString();
        scale = scale.Replace(",", ".");

        int clientside = 0;
        if(go.transform.localScale.x > 1)
            clientside = 1;

        string buildent = @"{
""StartDisabled"" ""0""
""spawnflags"" ""0""
""fadedist"" ""-1""
""collide_titan"" ""1""
""collide_ai"" ""1""
""scale"" """ + scale + @"""
""angles"" """ + BuildAngles(go) + @"""
""origin"" """ + BuildOrigin(go) + @"""
""targetname"" ""MapEditorProp""
""solid"" ""6""
""model"" """ +  model + @"""
""ClientSide"" """ + clientside + @"""
""classname"" ""prop_dynamic""
}
";
        return buildent;
    }
}