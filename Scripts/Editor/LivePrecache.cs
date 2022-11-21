using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PrecacheCode : EditorWindow
{
    static bool OverrideTextLimit = false;
    static string text = "";
    static Vector2 scroll;

    [MenuItem("R5Reloaded/Precache Code", false, 25)]
    static void Init()
    {
        PrecacheCode window = (PrecacheCode)GetWindow(typeof(PrecacheCode), false, "Precache Code");
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
        if (GUILayout.Button("Copy"))
            GenerateMap(true);

        if (text.Length > 75000)
        {
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Export is to long to show in the textbox, Please copy using the button above!");
            GUI.contentColor = Color.white;
            OverrideTextLimit = EditorGUILayout.Toggle("Override Text Limit", OverrideTextLimit);
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);
        if(text.Length > 75000 && !OverrideTextLimit) {
            GUI.contentColor = Color.yellow;
            GUILayout.TextArea("Output code is to long. You can override this with the toggle above. \nWARNING: MAY CAUSE LAG!", GUILayout.ExpandHeight(true));
            GUI.contentColor = Color.white;
        }
        else
        {
            GenerateMap(false);
            GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        }
        EditorGUILayout.EndScrollView();
    }

    void GenerateMap(bool copytext)
    {
        SetPropTagsItem();

        EditorSceneManager.SaveOpenScenes();

        string saved = "";

        //Generate All Props
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
            
        foreach(GameObject go in PropObjects) {
            string[] splitArray = go.name.Split(char.Parse(" "));
            string finished = splitArray[0].Replace("#", "/") + ".rmdl";

            if(saved.Contains(finished))
                continue;
            
            saved += "    PrecacheModel( $\"" + finished + "\"" + ")" + "\n";
        }
     
        if(copytext) {
            GUIUtility.systemCopyBuffer = saved;

            if(saved.Length > 75000)
                saved = "";
        } else {
            text = saved;
            saved = "";
        }
    }

    //Tags Custom Prefabs so users cant retag a item wrong
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
}