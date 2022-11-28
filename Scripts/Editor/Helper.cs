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

    public static string BuildDataTableItem(GameObject go)
    {
        string[] splitArray = go.name.Split(char.Parse(" "));
        string finished = splitArray[0].Replace("#", "/") + ".rmdl";

        PropScript script = go.GetComponent<PropScript>();

        string type = "\"dynamic_prop\",";
        string origin = "\"" + Helper.BuildOrigin(go) + "\",";
        string angles = "\"" + Helper.BuildAngles(go) + "\",";
        string scale = go.transform.localScale.x.ToString().Replace(",", ".") + ",";
        string fade = script.fadeDistance.ToString() + ",";
        string mantle = script.allowMantle.ToString().ToLower() + ",";
        string visible = "true,";
        string mdl = "\"" + finished + "\",";
        string collection = "\"None\"";

        if (go.transform.parent != null)
        {
            GameObject parent = go.transform.parent.gameObject;
            collection = "\"" + parent.name.Replace("\r", "").Replace("\n", "") + "\"";
        }

        return type + origin + angles + scale + fade + mantle + visible + mdl + collection + "\n";
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
    /// Builds Map Code
    /// </summary>
    /// <param name="UseStartingOffset">want to use starting offset</param>
    /// <returns>built map code string</returns>
    public static string BuildMapCode(bool UseStartingOffset)
    {
        string code = "";

        //Generate All Buttons
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        if (ButtonObjects.Length > 0)
        {
            code += "    //Buttons" + "\n";

            foreach (GameObject go in ButtonObjects)
            {
                ButtonScripting script = go.GetComponent<ButtonScripting>();
                code += "    AddCallback_OnUseEntity( CreateFRButton(" + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", \"" + script.UseText + "\"), void function(entity panel, entity user, int input)" + "\n" + "    {" + "\n" + script.OnUseCallback + "\n" + "    })" + "\n";
            }

            code += "\n";
        }
        //End of Buttons

        //Generate All JumpPads
        GameObject[] JumppadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        if (JumppadObjects.Length > 0)
        {
            code += "    //Jumppads" + "\n";

            foreach (GameObject go in JumppadObjects)
            {
                PropScript script = go.GetComponent<PropScript>();
                code += "    JumpPad_CreatedCallback( MapEditor_CreateProp( $\"mdl/props/octane_jump_pad/octane_jump_pad.rmdl\" , " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + script.allowMantle.ToString().ToLower() + ", " + script.fadeDistance + ", " + script.realmID + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ")" + " )" + "\n";
            }

            code += "\n";
        }
        //End of JumpPads

        //Generate All BubbleShields
        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        if (BubbleShieldObjects.Length > 0)
        {
            code += "    //BubbleShields" + "\n";

            foreach (GameObject go in BubbleShieldObjects)
            {
                string[] splitArray = go.name.Split(char.Parse(" "));
                string finished = splitArray[0].Replace("#", "/") + ".rmdl";
                BubbleScript script = go.GetComponent<BubbleScript>();
                string shieldColor = script.shieldColor.r + " " + script.shieldColor.g + " " + script.shieldColor.b;
                code += "    MapEditor_CreateBubbleShieldWithSettings( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ", \"" + shieldColor + "\", $\"" + finished + "\" )" + "\n";
            }

            code += "\n";
        }
        //End of BubbleShields

        //Generate All WeaponRacks
        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        if (WeaponRackObjects.Length > 0)
        {
            code += "    //Weapon Racks" + "\n";

            foreach (GameObject go in WeaponRackObjects)
            {
                WeaponRackScript script = go.GetComponent<WeaponRackScript>();
                code += @"    MapEditor_CreateRespawnableWeaponRack( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", \"" + go.name.Replace("custom_weaponrack_", "mp_weapon_") + "\", " + script.respawnTime + " )" + "\n";
            }

            code += "\n";
        }
        //End of WeaponRacks

        //Generate All LootBins
        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        if (LootBinObjects.Length > 0)
        {
            code += "    //LootBins" + "\n";

            foreach (GameObject go in LootBinObjects)
            {
                LootBinScript script = go.GetComponent<LootBinScript>();
                code += @"    MapEditor_CreateLootBin( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + script.lootbinSkin + " )" + "\n";
            }

            code += "\n";
        }
        //End of LootBins

        //Generate All ZipLines
        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        if (ZipLineObjects.Length > 0 || LinkedZipLineObjects.Length > 0)
        {
            code += "    //ZipLines" + "\n";

            foreach (GameObject go in ZipLineObjects)
            {
                string ziplinestart = "";
                string ziplineend = "";

                foreach (Transform child in go.transform)
                {
                    if (child.name == "zipline_start")
                    {
                        ziplinestart = Helper.BuildOrigin(child.gameObject);
                    }
                    else if (child.name == "zipline_end")
                    {
                        ziplineend = Helper.BuildOrigin(child.gameObject);
                    }
                }

                code += @"    CreateZipline(" + ziplinestart + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + ziplineend + Helper.ShouldAddStartingOrg(UseStartingOffset) + ")" + "\n";
            }

            foreach (GameObject go in LinkedZipLineObjects)
            {
                bool first = true;
                string nodes = "[ ";

                LinkedZiplineScript script = go.GetComponent<LinkedZiplineScript>();

                foreach (Transform child in go.transform)
                {
                    if (!first)
                        nodes += ", ";

                    nodes += Helper.BuildOrigin(child.gameObject);

                    first = false;
                }

                string smoothType = "GetAllPointsOnBezier";
                if (!script.smoothType)
                    smoothType = "GetBezierOfPath";

                nodes += " ]";

                code += @"    MapEditor_CreateLinkedZipline( ";
                if (script.enableSmoothing) code += smoothType + "( ";
                code += nodes;
                if (script.enableSmoothing) code += ", " + script.smoothAmount;
                code += " )";
                if (script.enableSmoothing) code += " )";
                code += "\n";
            }

            code += "\n";
        }
        //End of ziplines

        //Generate All Doors
        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        GameObject[] HorzDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        if (SingleDoorObjects.Length > 0 || DoubleDoorObjects.Length > 0 || VertDoorObjects.Length > 0 || HorzDoorObjects.Length > 0)
        {
            code += "    //Doors" + "\n";

            foreach (GameObject go in SingleDoorObjects)
            {
                DoorScript script = go.GetComponent<DoorScript>();
                code += @"    MapEditor_SpawnDoor( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", eMapEditorDoorType.Single, " + script.goldDoor.ToString().ToLower() + " )" + "\n";
            }

            foreach (GameObject go in DoubleDoorObjects)
            {
                DoorScript script = go.GetComponent<DoorScript>();
                code += @"    MapEditor_SpawnDoor( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", eMapEditorDoorType.Double, " + script.goldDoor.ToString().ToLower() + " )" + "\n";
            }

            foreach (GameObject go in VertDoorObjects)
                code += @"    MapEditor_SpawnDoor( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", eMapEditorDoorType.Vertical)" + "\n";

            foreach (GameObject go in HorzDoorObjects)
                code += @"    MapEditor_SpawnDoor( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", eMapEditorDoorType.Horizontal)" + "\n";

            code += "\n";
        }
        //End of Doors

        //Generate All Props
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        if (PropObjects.Length > 0)
        {
            code += "    //Props" + "\n";

            foreach (GameObject go in PropObjects)
            {
                string[] splitArray = go.name.Split(char.Parse(" "));
                string finished = splitArray[0].Replace("#", "/") + ".rmdl";
                PropScript script = go.GetComponent<PropScript>();
                code += "    MapEditor_CreateProp( $\"" + finished + "\", " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + script.allowMantle.ToString().ToLower() + ", " + script.fadeDistance + ", " + script.realmID + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ")" + "\n";
            }

            code += "\n";
        }
        //End of Props

        //Generate All Triggers
        GameObject[] TriggerObjects = GameObject.FindGameObjectsWithTag("Trigger");
        if (TriggerObjects.Length > 0)
        {
            code += "    //Triggers" + "\n";

            int triggerid = 0;
            foreach (GameObject go in TriggerObjects)
            {
                TriggerScripting script = go.GetComponent<TriggerScripting>();
                code += @"    entity trigger" + triggerid + " = MapEditor_CreateTrigger( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ", " + go.transform.localScale.y.ToString().Replace(",", ".") + ", " + script.Debug.ToString().ToLower() + ")" + "\n";

                if (script.EnterCallback != "")
                    code += @"    trigger" + triggerid + ".SetEnterCallback( void function(entity trigger , entity ent) {" + "\n" + script.EnterCallback + "\n" + "    })" + "\n";

                if (script.LeaveCallback != "")
                    code += @"    trigger" + triggerid + ".SetLeaveCallback( void function(entity trigger , entity ent) {" + "\n" + script.LeaveCallback + "\n" + "    })" + "\n";

                code += @"    DispatchSpawn( trigger" + triggerid + " )" + "\n";
                triggerid++;
            }
        }
        //End of Triggers

        return code;
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