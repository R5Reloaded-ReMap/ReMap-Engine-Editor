using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;

public class Helper
{
    public static int maxBuildLength = 75000;
    public static int greenPropCount = 1500;
    public static int yellowPropCount = 1500;
    static string[] ObjectNames = new string[] {
        "custom_lootbin", //LootBin
        "custom_zipline", //ZipLine
        "custom_jumppad", //Jumppad
        "custom_linked_zipline", //LinkedZipline
        "custom_single_door", //SingleDoor
        "custom_double_door", //DoubleDoor
        "custom_vertical_door", //VerticalDoor
        "custom_sliding_door", //HorzDoor
        "custom_weaponrack", //WeaponRack
        "custom_button", //Button
        "trigger_cylinder", //Trigger
        "mdl", //Prop
        "mdl#fx#bb_shield", //BubbleShield
    };

     static string[] TagNames = new string[] {
        "LootBin", //custom_lootbin
        "ZipLine", //custom_zipline
        "Jumppad", //custom_jumppad
        "LinkedZipline", //custom_linked_zipline
        "SingleDoor", //custom_single_door
        "DoubleDoor", //custom_double_door
        "VerticalDoor", //custom_vertical_door
        "HorzDoor", //custom_sliding_door
        "WeaponRack", //custom_weaponrack
        "Button", //custom_button
        "Trigger", //trigger_cylinder
        "Prop", //mdl
        "BubbleShield", //#bb_shield
    };


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
        if(!UseStartingOffset)
            return "";

        if(type == 0)
            return " + startingorg";

        return "    //Starting Origin, Change this to a origin in a map \n    vector startingorg = <0,0,0>" + "\n\n";
    }

    /// <summary>
    /// Builds correct angles from gameobject
    /// </summary>
    /// <param name="go">Prop Object</param>
    /// <returns></returns>
    public static string BuildAngles(GameObject go)
    {
        string x = (-WrapAngle(go.transform.eulerAngles.x)).ToString("F4").Replace(",", ".");
        string y = (-WrapAngle(go.transform.eulerAngles.y)).ToString("F4").Replace(",", ".");
        string z = (WrapAngle(go.transform.eulerAngles.z)).ToString("F4").Replace(",", ".");
                    
        string angles = $"< {x}, {y}, {z} >";

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

        string origin = $"< {x}, {y}, {z} >";

        return origin;
    }

    /// <summary>
    /// Tags Custom Prefabs so users cant wrongly tag a item
    /// </summary>
    public static void FixPropTags()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        //Retag All Objects
        foreach (GameObject go in allObjects) {
            go.tag = "Untagged";

            for (int i = 0; i < ObjectNames.Length; i++)
                if (go.name.Contains(ObjectNames[i]))
                    go.tag = TagNames[i];
        }
    }

    /// <summary>
    /// Builds Map Code
    /// </summary>
    /// <param name="UseStartingOffset">want to use starting offset</param>
    /// <returns>built map code string</returns>
    public static string BuildMapCode(bool UseStartingOffset)
    {
        string code = "";

        code += Build.Buttons(UseStartingOffset);
        code += Build.Jumpads(UseStartingOffset);
        code += Build.BubbleShields(UseStartingOffset);
        code += Build.WeaponRacks(UseStartingOffset);
        code += Build.LootBins(UseStartingOffset);
        code += Build.ZipLines(UseStartingOffset);
        code += Build.Doors(UseStartingOffset);
        code += Build.Props(UseStartingOffset, Build.BuildType.Map);
        code += Build.Triggers(UseStartingOffset);

        return code;
    }

    public static string Credits = @"
//Made with Unity Map Editor
//By AyeZee#6969
//With help from Julefox#0050
";
}