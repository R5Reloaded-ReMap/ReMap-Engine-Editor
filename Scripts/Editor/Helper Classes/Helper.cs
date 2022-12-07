using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Helper
{
    public static int maxBuildLength = 75000;
    public static int greenPropCount = 1500;
    public static int yellowPropCount = 1500;
    public static bool Is_Using_Starting_Offset = false;
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
        "_vertical_zipline", //VerticalZipline
        "_non_vertical_zipline", //NonVerticalZipLine
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
        "VerticalZipLine", // _vertical_zipline
        "NonVerticalZipLine", // _non_vertical_zipline
    };

    public enum ExportType
    {
        WholeScriptOffset,
        MapOnlyOffset,
        WholeScript,
        MapOnly
    }

    public struct NewDataTable {
        public string Type;
        public Vector3 Origin;
        public Vector3 Angles;
        public float Scale;
        public string fadeDistance;
        public string canMantle;
        public string isVisible;
        public string Model;
        public string Collection;
    }

    /// <summary>
    /// Gets Total Prop Count
    /// </summary>
    /// <returns></returns>
    public static int GetPropCount()
    {
        int objectCount = 0;

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            for (int i = 0; i < ObjectNames.Length - 1; i++)
                if (go.name.Contains(ObjectNames[i]))
                    objectCount++;
        }

        return objectCount;
    }

    /// <summary>
    /// Should add starting origin to object location
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ShouldAddStartingOrg(int type = 0)
    {
        if(!Is_Using_Starting_Offset)
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
        foreach (GameObject go in allObjects)
        {
            go.tag = "Untagged";

            for (int i = 0; i < ObjectNames.Length; i++)
                if (go.name.Contains(ObjectNames[i]))
                    go.tag = TagNames[i];
        }
    }

    public static NewDataTable BuildDataTable(string item)
    {
        string[] items = item.Replace("\"", "").Split(char.Parse(","));

        NewDataTable dt = new NewDataTable();
        dt.Type = items[0];
        dt.Origin = new Vector3(float.Parse(items[2]), float.Parse(items[3].Replace(">", "")), -(float.Parse(items[1].Replace("<", ""))));
        dt.Angles = new Vector3(-(float.Parse(items[4].Replace("<", ""))), -(float.Parse(items[5])), float.Parse(items[6].Replace(">", "")));
        dt.Scale = float.Parse(items[7]);
        dt.fadeDistance = items[8];
        dt.canMantle = items[9];
        dt.isVisible = items[10];
        dt.Model = items[11].Replace("/", "#").Replace(".rmdl", "").Replace("\"", "").Replace("\n", "").Replace("\r", "");
        dt.Collection = items[12].Replace("\"", "");

        return dt;
    }

    public static void CreateDataTableItem(NewDataTable dt, UnityEngine.Object loadedPrefabResource)
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
        obj.transform.position = dt.Origin;
        obj.transform.eulerAngles = dt.Angles;
        obj.name = dt.Model;
        obj.gameObject.transform.localScale = new Vector3(dt.Scale, dt.Scale, dt.Scale);
        obj.SetActive(dt.isVisible == "true");

        PropScript script = obj.GetComponent<PropScript>();
        script.fadeDistance = float.Parse(dt.fadeDistance);
        script.allowMantle = dt.canMantle == "true";

        if (dt.Collection == "")
            return;

        GameObject parent = GameObject.Find(dt.Collection);
        if (parent != null)
            obj.gameObject.transform.parent = parent.transform;
    }

    public static List<String> BuildCollectionList(string[] items)
    {
        List<String> collectionList = new List<String>();
        foreach (string item in items)
        {
            string[] itemsplit = item.Replace("\"", "").Split(char.Parse(","));

            if (itemsplit.Length < 12)
                continue;

            string collection = itemsplit[12].Replace("\"", "");

            if (collection == "")
                continue;

            if (!collectionList.Contains(collection))
                collectionList.Add(collection);
        }

        return collectionList;
    }

    /// <summary>
    /// Builds Map Code
    /// </summary>
    /// <returns>built map code string</returns>
    public static string BuildMapCode()
    {
        string code = Build.Buttons();
        code += Build.Jumpads();
        code += Build.BubbleShields();
        code += Build.WeaponRacks();
        code += Build.LootBins();
        code += Build.ZipLines();
        code += Build.LinkedZipLines();
        code += Build.VerticalZipLines();
        code += Build.NonVerticalZipLines();
        code += Build.SingleDoors();
        code += Build.DoubleDoors();
        code += Build.VertDoors();
        code += Build.HorizontalDoors();
        code += Build.Props(Build.BuildType.Map);
        code += Build.Triggers();
        return code;
    }

    public static string Credits = @"
//Made with Unity Map Editor
//By AyeZee#6969
//With help from Julefox#0050
";
}