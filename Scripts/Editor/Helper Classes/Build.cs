using UnityEngine;

public class Build
{
    public enum BuildType {
        Map, 
        Ent, 
        Precache,
        DataTable
    };

    public static string Buttons(bool UseStartingOffset)
    {
        string code = "";

        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        if (ButtonObjects.Length > 0)
        {
            code += "    //Buttons \n";

            foreach (GameObject go in ButtonObjects)
            {
                ButtonScripting script = go.GetComponent<ButtonScripting>();
                code += $"    AddCallback_OnUseEntity( CreateFRButton({Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, \"{script.UseText}\"), void function(entity panel, entity user, int input)" + "\n    {\n" + script.OnUseCallback + "\n    })" + "\n";
            }

            code += "\n";
        }

        return code;
    }

    public static string Jumpads(bool UseStartingOffset)
    {
        string code = "";

        GameObject[] JumppadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        if (JumppadObjects.Length > 0)
        {
            code += "    //Jumppads \n";

            foreach (GameObject go in JumppadObjects)
            {
                PropScript script = go.GetComponent<PropScript>();
                code += $"    JumpPad_CreatedCallback( MapEditor_CreateProp( $\"mdl/props/octane_jump_pad/octane_jump_pad.rmdl\", {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, {script.allowMantle.ToString().ToLower()}, {script.fadeDistance}, {script.realmID}, {go.transform.localScale.x.ToString().Replace(",", ".")} ) )" + "\n";
            }

            code += "\n";
        }

        return code;
    }

    public static string BubbleShields(bool UseStartingOffset)
    {
        string code = "";

        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        if (BubbleShieldObjects.Length > 0)
        {
            code += "    //BubbleShields \n";

            foreach (GameObject go in BubbleShieldObjects)
            {
                string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
                BubbleScript script = go.GetComponent<BubbleScript>();
                string shieldColor = script.shieldColor.r + " " + script.shieldColor.g + " " + script.shieldColor.b;
                
                code += $"    MapEditor_CreateBubbleShieldWithSettings( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, {go.transform.localScale.x.ToString().Replace(",", ".")}, \"{shieldColor}\", $\"{model}\" )" + "\n";
            }

            code += "\n";
        }

        return code;
    }

    public static string WeaponRacks(bool UseStartingOffset)
    {
        string code = "";

        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        if (WeaponRackObjects.Length > 0)
        {
            code += "    //Weapon Racks \n";

            foreach (GameObject go in WeaponRackObjects)
            {
                WeaponRackScript script = go.GetComponent<WeaponRackScript>();
                code += $"    MapEditor_CreateRespawnableWeaponRack( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, \"{go.name.Replace("custom_weaponrack_", "mp_weapon_")}\", {script.respawnTime} )" + "\n";
            }

            code += "\n";
        }

        return code;
    }

    public static string LootBins(bool UseStartingOffset)
    {
        string code = "";

        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        if (LootBinObjects.Length > 0)
        {
            code += "    //LootBins \n";

            foreach (GameObject go in LootBinObjects)
            {
                LootBinScript script = go.GetComponent<LootBinScript>();
                code += $"    MapEditor_CreateLootBin( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, {script.lootbinSkin} )" + "\n";
            }

            code += "\n";
        }

        return code;
    }

    public static string ZipLines(bool UseStartingOffset)
    {
        string code = "";

        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        if (ZipLineObjects.Length > 0 || LinkedZipLineObjects.Length > 0)
        {
            code += "    //ZipLines \n";

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

                code += $"    CreateZipline( {ziplinestart + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {ziplineend + Helper.ShouldAddStartingOrg(UseStartingOffset)} )" + "\n";
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
                if (script.enableSmoothing) code += $"{smoothType}( ";
                code += nodes;
                if (script.enableSmoothing) code += $", {script.smoothAmount}";
                code += " )";
                if (script.enableSmoothing) code += " )";
                code += "\n";
            }

            code += "\n";
        }

        return code;
    }

    public static string Doors(bool UseStartingOffset)
    {
        string code = "";

        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        GameObject[] HorzDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        if (SingleDoorObjects.Length > 0 || DoubleDoorObjects.Length > 0 || VertDoorObjects.Length > 0 || HorzDoorObjects.Length > 0)
        {
            code += "    //Doors \n";

            foreach (GameObject go in SingleDoorObjects) {
                DoorScript script = go.GetComponent<DoorScript>();
                code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Single, {script.goldDoor.ToString().ToLower()} )" + "\n";
            }

            foreach (GameObject go in DoubleDoorObjects) {
                DoorScript script = go.GetComponent<DoorScript>();
                code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Double, {script.goldDoor.ToString().ToLower()} )" + "\n";
            }

            foreach (GameObject go in VertDoorObjects)
                code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Vertical)" + "\n";

            foreach (GameObject go in HorzDoorObjects)
                code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Horizontal)" + "\n";

            code += "\n";
        }

        return code;
    }

    public static string Props(bool UseStartingOffset, BuildType type = BuildType.Map)
    {
        string code = "";

        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        if (PropObjects.Length > 0)
        {
            if(type == BuildType.Map)
                code += "    //Props \n";

            if(type == BuildType.DataTable)
                code += "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"" + "\n";

            foreach (GameObject go in PropObjects) {
                string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
                PropScript script = go.GetComponent<PropScript>();

                if(type == BuildType.Ent) {
                    code += BuildScriptEntItem(go);
                    continue;
                }

                if (type == BuildType.DataTable)
                { 
                    code += BuildDataTableItem(go);
                    continue;
                }

                if(type == BuildType.Precache) {
                    code += $"    PrecacheModel( $\"{model}\" )" + "\n";
                    continue;
                }

                if (type == BuildType.Map) {
                    code += $"    MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, {script.allowMantle.ToString().ToLower()}, {script.fadeDistance}, {script.realmID}, {go.transform.localScale.x.ToString().Replace(",", ".")} )" + "\n";
                }
            }

            if(type == BuildType.Map)
                code += "\n";

            if(type == BuildType.DataTable)
                code += "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"";
        }

        return code;
    }

    public static string Triggers(bool UseStartingOffset)
    {
        string code = "";

        GameObject[] TriggerObjects = GameObject.FindGameObjectsWithTag("Trigger");
        if (TriggerObjects.Length > 0)
        {
            code += "    //Triggers \n";

            int triggerid = 0;
            foreach (GameObject go in TriggerObjects)
            {
                TriggerScripting script = go.GetComponent<TriggerScripting>();
                code += $"    entity trigger" + triggerid + $" = MapEditor_CreateTrigger( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset)}, {Helper.BuildAngles(go)}, {go.transform.localScale.x.ToString().Replace(",", ".")}, {go.transform.localScale.y.ToString().Replace(",", ".")}, {script.Debug.ToString().ToLower()} )" + "\n";

                if (script.EnterCallback != "")
                    code += $"    trigger{triggerid}.SetEnterCallback( void function(entity trigger , entity ent)" + "{\n" + script.EnterCallback + "\n    })" + "\n";

                if (script.LeaveCallback != "")
                    code += $"    trigger{triggerid}.SetLeaveCallback( void function(entity trigger , entity ent)" + "{\n" + script.LeaveCallback + "\n    })" + "\n";

                code += $"    DispatchSpawn( trigger{triggerid} )" + "\n";
                triggerid++;
            }
        }

        return code;
    }

    public static string BuildDataTableItem(GameObject go)
    {
        string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
        PropScript script = go.GetComponent<PropScript>();

        string type = "\"dynamic_prop\",";
        string origin = "\"" + Helper.BuildOrigin(go).Replace(" ", "") + "\",";
        string angles = "\"" + Helper.BuildAngles(go).Replace(" ", "") + "\",";
        string scale = go.transform.localScale.x.ToString().Replace(",", ".") + ",";
        string fade = script.fadeDistance.ToString() + ",";
        string mantle = script.allowMantle.ToString().ToLower() + ",";
        string visible = "true,";
        string mdl = "\"" + model + "\",";
        string collection = "\"\"";

        if (go.transform.parent != null) {
            GameObject parent = go.transform.parent.gameObject;
            collection = "\"" + parent.name.Replace("\r", "").Replace("\n", "") + "\"";
        }

        return type + origin + angles + scale + fade + mantle + visible + mdl + collection + "\n";
    }

    public static string BuildScriptEntItem(GameObject go)
    {
        string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
        PropScript script = go.GetComponent<PropScript>();
        
        string buildent = @"{
""StartDisabled"" ""0""
""spawnflags"" ""0""
""fadedist"" """ + script.fadeDistance + @"""
""collide_titan"" ""1""
""collide_ai"" ""1""
""scale"" """ + go.transform.localScale.x.ToString().Replace(",", ".") + @"""
""angles"" """ + Helper.BuildAngles(go)  + @"""
""origin"" """ + Helper.BuildOrigin(go) + @"""
""targetname"" ""MapEditorProp""
""solid"" ""6""
""model"" """ +  model + @"""
""ClientSide"" ""0""
""classname"" ""prop_dynamic""
}
";
        return buildent;
    }
}