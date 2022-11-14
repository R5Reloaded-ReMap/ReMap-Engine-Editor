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

    [MenuItem("R5Reloaded/Live script.ent Code", false, 25)]
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

        GUILayout.Label("Entity Count: " + finalcount);

        if(finalcount < 1500)
            GUILayout.Label("Entity Status: Safe");
        else if((finalcount < 3000)) 
            GUILayout.Label("Entity Status: Safe");
        else
            GUILayout.Label("Entity Status: Warning! Game might crash!");

        GUI.contentColor = Color.white;

        if (GUILayout.Button("Copy Code"))
            GenerateMap(true);

        if (text.Length > 75000)
        {
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Text is to long to show in the textbox, Please copy using the button above!");
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

    private static float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }

    private static float GetOrgX(GameObject go)
    {
        float orgx = -go.transform.position.z;

        return orgx;
    }

    private static float GetOrgY(GameObject go)
    {
        float orgy = go.transform.position.x;

        return orgy;
    }

    private static float GetOrgZ(GameObject go)
    {
        float orgz = go.transform.position.y;

        return orgz;
    }

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
            } else if(go.name.Contains("custom_single_door")) {
                go.tag = "SingleDoor";
            } else if(go.name.Contains("custom_double_door")) {
                go.tag = "DoubleDoor";
            } else if(go.name.Contains("mdl")) {
                go.tag = "Prop";
            }
        }
    }

    private static string BuildScriptEnt(string model, GameObject go)
    {
        string scale = go.transform.localScale.x.ToString();
        scale = scale.Replace(",", ".");

        string orgx = GetOrgX(go).ToString("F4");
        string orgy = GetOrgY(go).ToString("F4");
        string orgz = GetOrgZ(go).ToString("F4");

        string angx = (-WrapAngle(go.transform.eulerAngles.x)).ToString("F4");
        string angy = (-WrapAngle(go.transform.eulerAngles.y)).ToString("F4");
        string angz = (WrapAngle(go.transform.eulerAngles.z)).ToString("F4");
                    
        string origin = "< " + orgx.Replace(",", ".") + ", " + orgy.Replace(",", ".") + ", " + orgz.Replace(",", ".") + " >";
        string angles = "< " + angx.Replace(",", ".") + ", " + angy.Replace(",", ".") + ", " + angz.Replace(",", ".") + " >";

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
""angles"" """ + angles + @"""
""origin"" """ + origin + @"""
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