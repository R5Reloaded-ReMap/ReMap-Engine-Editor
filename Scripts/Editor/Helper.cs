using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;

public class Helper
{
    /// <summary>
    /// Gets Total Prop Count
    /// </summary>
    /// <returns></returns>
    public static int GetPropCount()
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        GameObject[] HorzDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        GameObject[] JumppadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        GameObject[] TriggerObjects = GameObject.FindGameObjectsWithTag("Trigger");
        GameObject[] ShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        int finalcount = PropObjects.Length + SingleDoorObjects.Length + DoubleDoorObjects.Length + ZipLineObjects.Length + LootBinObjects.Length + VertDoorObjects.Length + HorzDoorObjects.Length + WeaponRackObjects.Length + LinkedZipLineObjects.Length + ButtonObjects.Length + JumppadObjects.Length + TriggerObjects.Length + ShieldObjects.Length;

        return finalcount;
    }

    /// <summary>
    /// Should add starting origin to object location
    /// </summary>
    /// <param name="UseStartingOffset"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ShouldAddStartingOrg(bool UseStartingOffset, int type = 0)
    {
        if(UseStartingOffset) {
            if(type == 0)
                return " + startingorg";

            return "    //Starting Origin, Change this to a origin in a map \n    vector startingorg = <0,0,0>" + "\n\n";
        }
        else {
            return "";
        }
    }

    /// <summary>
    /// Builds correct angles from gameobject
    /// </summary>
    /// <param name="go">Prop Object</param>
    /// <returns></returns>
    public static string BuildAngles(GameObject go)
    {
        string x = (-WrapAngle(go.transform.eulerAngles.x)).ToString("F4");
        string y = (-WrapAngle(go.transform.eulerAngles.y)).ToString("F4");
        string z = (WrapAngle(go.transform.eulerAngles.z)).ToString("F4");
                    
        string angles = "< " + x.Replace(",", ".") + ", " + y.Replace(",", ".") + ", " + z.Replace(",", ".") + " >";

        return angles;
    }

    /// <summary>
    /// Wraps Angles that are above 180
    /// </summary>
    /// <param name="angle">Angle to wrap</param>
    /// <returns></returns>
    public static float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }

    /// <summary>
    /// Builds correct ingame origin from gameobject
    /// </summary>
    /// <param name="go">Prop Object</param>
    /// <returns></returns>
    public static string BuildOrigin(GameObject go)
    {
        string x = (-go.transform.position.z).ToString("F4").Replace(",", ".");
        string y = (go.transform.position.x).ToString("F4").Replace(",", ".");
        string z = (go.transform.position.y).ToString("F4").Replace(",", ".");

        string origin = "< " + x + ", " + y + ", " + z + " >";

        return origin;
    }

    /// <summary>
    /// Tags Custom Prefabs so users cant wrongly tag a item
    /// </summary>
    public static void FixPropTags()
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

    /// <summary>
    /// Builds script ent code
    /// </summary>
    /// <param name="model"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string BuildScriptEnt(GameObject go)
    {
        string[] splitArray = go.name.Split(char.Parse(" "));
        string model = splitArray[0].Replace("#", "/") + ".rmdl";

        PropScript script = go.GetComponent<PropScript>();
        
        string buildent = @"{
""StartDisabled"" ""0""
""spawnflags"" ""0""
""fadedist"" """ + script.fadeDistance + @"""
""collide_titan"" ""1""
""collide_ai"" ""1""
""scale"" """ + go.transform.localScale.x.ToString().Replace(",", ".") + @"""
""angles"" """ + BuildAngles(go)  + @"""
""origin"" """ + BuildOrigin(go) + @"""
""targetname"" ""MapEditorProp""
""solid"" ""6""
""model"" """ +  model + @"""
""ClientSide"" ""0""
""classname"" ""prop_dynamic""
}
";
        return buildent;
    }

    public static string Credits = @"
//Made with Unity Map Editor
//By AyeZee#6969
//With help from Julefox#0050
";
}