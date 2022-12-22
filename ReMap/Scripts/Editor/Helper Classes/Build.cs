using UnityEngine;

public class Build
{
    public enum BuildType {
        Map, 
        Ent, 
        Precache,
        DataTable
    };

    public static string Buttons()
    {
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        if (ButtonObjects.Length < 1)
            return "";
        
        string code = "    //Buttons \n";

        foreach (GameObject go in ButtonObjects)
        {
            ButtonScripting script = go.GetComponent<ButtonScripting>();
            code += $"    AddCallback_OnUseEntity( CreateFRButton({Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, \"{script.UseText}\"), void function(entity panel, entity user, int input)" + "\n    {\n" + script.OnUseCallback + "\n    })" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string Jumpads()
    {
        GameObject[] JumppadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        if (JumppadObjects.Length < 1)
            return "";

        string code = "    //Jumppads \n";

        foreach (GameObject go in JumppadObjects)
        {
            PropScript script = go.GetComponent<PropScript>();
            code += $"    JumpPad_CreatedCallback( MapEditor_CreateProp( $\"mdl/props/octane_jump_pad/octane_jump_pad.rmdl\", {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {script.allowMantle.ToString().ToLower()}, {script.fadeDistance}, {script.realmID}, {go.transform.localScale.x.ToString().Replace(",", ".")} ) )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string BubbleShields()
    {
        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        if (BubbleShieldObjects.Length < 1)
            return "";

        string code = "    //BubbleShields \n";

        foreach (GameObject go in BubbleShieldObjects)
        {
            string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
            BubbleScript script = go.GetComponent<BubbleScript>();
            string shieldColor = script.shieldColor.r + " " + script.shieldColor.g + " " + script.shieldColor.b;
                
            code += $"    MapEditor_CreateBubbleShieldWithSettings( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {go.transform.localScale.x.ToString().Replace(",", ".")}, \"{shieldColor}\", $\"{model}\" )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string WeaponRacks()
    {
        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        if (WeaponRackObjects.Length < 1)
            return "";

        string code = "    //Weapon Racks \n";

        foreach (GameObject go in WeaponRackObjects)
        {
            WeaponRackScript script = go.GetComponent<WeaponRackScript>();
            code += $"    MapEditor_CreateRespawnableWeaponRack( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, \"{go.name.Replace("custom_weaponrack_", "mp_weapon_")}\", {script.respawnTime} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string LootBins()
    {
        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        if (LootBinObjects.Length < 1)
            return "";

        string code = "    //LootBins \n";

        foreach (GameObject go in LootBinObjects)
        {
            LootBinScript script = go.GetComponent<LootBinScript>();
            code += $"    MapEditor_CreateLootBin( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {script.lootbinSkin} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string ZipLines()
    {
        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        if (ZipLineObjects.Length < 1)
            return "";

        string code = "    //ZipLines \n";

        foreach (GameObject go in ZipLineObjects)
        {
            string ziplinestart = "";
            string ziplineend = "";

            foreach (Transform child in go.transform)
            {
                if (child.name == "zipline_start")
                    ziplinestart = Helper.BuildOrigin(child.gameObject);
                else if (child.name == "zipline_end")
                    ziplineend = Helper.BuildOrigin(child.gameObject);
            }

            code += $"    CreateZipline( {ziplinestart + Helper.ShouldAddStartingOrg()}, {ziplineend + Helper.ShouldAddStartingOrg()} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string LinkedZipLines()
    {
        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        
        if(LinkedZipLineObjects.Length < 1)
            return "";

        string code = "    //LinkedZipLines \n";

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

            string smoothType = script.smoothType ? "GetAllPointsOnBezier" : "GetBezierOfPath";

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

        return code;
    }

    public static string VerticalZipLines()
    {
        GameObject[] VerticalZipLineObjects = GameObject.FindGameObjectsWithTag("VerticalZipLine");

        if (VerticalZipLineObjects.Length < 1)
            return "";

        string code = "    //VerticalZipLines \n";

        foreach (GameObject go in VerticalZipLineObjects)
        {
            DrawVerticalZipline ziplineScript = go.GetComponent<DrawVerticalZipline>();
            if (ziplineScript == null)
                continue;

            int preservevelocity = ziplineScript.preserveVelocity ? 1 : 0;
            int dropToBottom = ziplineScript.dropToBottom ? 1 : 0;
            string restpoint = ziplineScript.restPoint.ToString().ToLower();
            int pushoffindirectionx = ziplineScript.pushOffInDirectionX ? 1 : 0;
            string ismoving = ziplineScript.isMoving.ToString().ToLower();
            int detachEndOnSpawn = ziplineScript.detachEndOnSpawn ? 1 : 0;
            int detachEndOnUse = ziplineScript.detachEndOnUse ? 1 : 0;
            float panelTimerMin = ziplineScript.panelTimerMin;
            float panelTimerMax = ziplineScript.panelTimerMax;
            int panelMaxUse = ziplineScript.panelMaxUse;

            string panelOrigin = "[";
            string panelAngles = "[";
            string panelModel = "[";

            for(int i = 0; i < ziplineScript.panels.Length; i++ )
            {
                panelOrigin += " " + Helper.BuildOrigin(ziplineScript.panels[i]) + Helper.ShouldAddStartingOrg();
                panelAngles += " " + Helper.BuildAngles(ziplineScript.panels[i]);
                panelModel += " $\"mdl/" + ziplineScript.panels[i].name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl\"";

                if (i != ziplineScript.panels.Length - 1)
                {
                    panelOrigin += ",";
                    panelAngles += ",";
                    panelModel += ",";
                }
            }

            panelOrigin += " ]";
            panelAngles += " ]";
            panelModel += " ]";

            code += $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(ziplineScript.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(ziplineScript.rope_start.gameObject)}, {Helper.BuildOrigin(ziplineScript.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(ziplineScript.rope_start.gameObject)}, true, {ziplineScript.fadeDistance.ToString().Replace(",", ".")}, {ziplineScript.scale.ToString().Replace(",", ".")}, {ziplineScript.width.ToString().Replace(",", ".")}, {ziplineScript.speedScale.ToString().Replace(",", ".")}, {ziplineScript.lengthScale.ToString().Replace(",", ".")}, {preservevelocity}, {dropToBottom}, {ziplineScript.autoDetachStart.ToString().Replace(",", ".")}, {ziplineScript.autoDetachEnd.ToString().Replace(",", ".")}, {restpoint}, {pushoffindirectionx}, {ismoving}, {detachEndOnSpawn}, {detachEndOnUse}, {panelOrigin}, {panelAngles}, {panelModel}, {panelTimerMin}, {panelTimerMax}, {panelMaxUse} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string NonVerticalZipLines()
    {
        GameObject[] NonVerticalZipLineObjects = GameObject.FindGameObjectsWithTag("NonVerticalZipLine");

        if (NonVerticalZipLineObjects.Length < 1)
            return "";

        string code = "    //NonVerticalZipLines \n";

        foreach (GameObject go in NonVerticalZipLineObjects)
        {
            DrawNonVerticalZipline ziplineScript = go.GetComponent<DrawNonVerticalZipline>();
            if (ziplineScript == null)
                continue;

            int preservevelocity = ziplineScript.preserveVelocity ? 1 : 0;
            int dropToBottom = ziplineScript.dropToBottom ? 1 : 0;
            string restpoint = ziplineScript.restPoint.ToString().ToLower();
            int pushoffindirectionx = ziplineScript.pushOffInDirectionX ? 1 : 0;
            string ismoving = ziplineScript.isMoving.ToString().ToLower();
            int detachEndOnSpawn = ziplineScript.detachEndOnSpawn ? 1 : 0;
            int detachEndOnUse = ziplineScript.detachEndOnUse ? 1 : 0;
            float panelTimerMin = ziplineScript.panelTimerMin;
            float panelTimerMax = ziplineScript.panelTimerMax;
            int panelMaxUse = ziplineScript.panelMaxUse;

            string panelOrigin = "[";
            string panelAngles = "[";
            string panelModel = "[";

            for (int i = 0; i < ziplineScript.panels.Length; i++)
            {
                panelOrigin += " " + Helper.BuildOrigin(ziplineScript.panels[i]) + Helper.ShouldAddStartingOrg();
                panelAngles += " " + Helper.BuildAngles(ziplineScript.panels[i]);
                panelModel += " $\"mdl/" + ziplineScript.panels[i].name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl\"";

                if (i != ziplineScript.panels.Length - 1)
                {
                    panelOrigin += ",";
                    panelAngles += ",";
                    panelModel += ",";
                }
            }

            panelOrigin += " ]";
            panelAngles += " ]";
            panelModel += " ]";

            code += $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(ziplineScript.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(ziplineScript.rope_start.gameObject)}, {Helper.BuildOrigin(ziplineScript.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(ziplineScript.rope_start.gameObject)}, false, {ziplineScript.fadeDistance.ToString().Replace(",", ".")}, {ziplineScript.scale.ToString().Replace(",", ".")}, {ziplineScript.width.ToString().Replace(",", ".")}, {ziplineScript.speedScale.ToString().Replace(",", ".")}, {ziplineScript.lengthScale.ToString().Replace(",", ".")}, {preservevelocity}, {dropToBottom}, {ziplineScript.autoDetachStart.ToString().Replace(",", ".")}, {ziplineScript.autoDetachEnd.ToString().Replace(",", ".")}, {restpoint}, {pushoffindirectionx}, {ismoving}, {detachEndOnSpawn}, {detachEndOnUse}, {panelOrigin}, {panelAngles}, {panelModel}, {panelTimerMin}, {panelTimerMax}, {panelMaxUse} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string SingleDoors()
    {
        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        if (SingleDoorObjects.Length < 1)
            return "";

        string code = "    //Single Doors \n";

        foreach (GameObject go in SingleDoorObjects) {
            DoorScript script = go.GetComponent<DoorScript>();
            code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Single, {script.goldDoor.ToString().ToLower()} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string DoubleDoors()
    {
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        if (DoubleDoorObjects.Length < 1)
            return "";

        string code = "    //Double Doors \n";

        foreach (GameObject go in DoubleDoorObjects) {
            DoorScript script = go.GetComponent<DoorScript>();
            code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Double, {script.goldDoor.ToString().ToLower()} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string VertDoors()
    {
        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        if (VertDoorObjects.Length < 1)
            return "";

        string code = "    //Vertical Doors \n";

        foreach (GameObject go in VertDoorObjects)
            code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Vertical )" + "\n";

        code += "\n";

        return code;
    }

    public static string HorizontalDoors()
    {
        GameObject[] HorzDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        if (HorzDoorObjects.Length < 1)
            return "";

        string code = "    //Horizontal Doors \n";

        foreach (GameObject go in HorzDoorObjects)
            code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Horizontal )" + "\n";

        code += "\n";

        return code;
    }

    public static string Props(BuildType type = BuildType.Map)
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        if (PropObjects.Length < 1)
            return "";

        string code = "";

        switch(type) {
            case BuildType.Map:
                code += "    //Props \n";
                break;
            case BuildType.DataTable:
                code += "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"" + "\n";
                break;
            //case BuildType.Ent:
                //code += $"ENTITIES02 num_models={PropObjects.Length}\n";
                //break;
        }

        foreach (GameObject go in PropObjects)
        {
            string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
            PropScript script = go.GetComponent<PropScript>();

            switch (type) {
                case BuildType.Ent:
                    code += BuildScriptEntItem(go);
                    continue;
                case BuildType.DataTable:
                    code += BuildDataTableItem(go);
                    continue;
                case BuildType.Precache:
                    code += $"    PrecacheModel( $\"{model}\" )" + "\n";
                    continue;
                case BuildType.Map:
                    code += $"    MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {script.allowMantle.ToString().ToLower()}, {script.fadeDistance}, {script.realmID}, {go.transform.localScale.x.ToString().Replace(",", ".")} )" + "\n";
                    continue;
            }
        }

        switch(type) {
            case BuildType.Map:
                code += "\n";
                break;
            case BuildType.DataTable:
                code += "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"";
                break;
        }

        return code;
    }


    public static string Sounds()
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Sound");
        if (PropObjects.Length < 1)
            return "";

        string code = ""; //= $"ENTITIES02 num_models={PropObjects.Length}\n";

        foreach (GameObject go in PropObjects)
            code += BuildSoundEntItem(go);

        return code;
    }


    public static string Triggers()
    {
        GameObject[] TriggerObjects = GameObject.FindGameObjectsWithTag("Trigger");
        if (TriggerObjects.Length < 1)
            return "";

        string code = "    //Triggers \n";

        int triggerid = 0;
        foreach (GameObject go in TriggerObjects)
        {
            TriggerScripting script = go.GetComponent<TriggerScripting>();
            code += $"    entity trigger" + triggerid + $" = MapEditor_CreateTrigger( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {go.transform.localScale.x.ToString().Replace(",", ".")}, {go.transform.localScale.y.ToString().Replace(",", ".")}, {script.Debug.ToString().ToLower()} )" + "\n";

            if (script.EnterCallback != "")
                code += $"    trigger{triggerid}.SetEnterCallback( void function(entity trigger , entity ent)" + "{\n" + script.EnterCallback + "\n    })" + "\n";

            if (script.LeaveCallback != "")
                code += $"    trigger{triggerid}.SetLeaveCallback( void function(entity trigger , entity ent)" + "{\n" + script.LeaveCallback + "\n    })" + "\n";

            code += $"    DispatchSpawn( trigger{triggerid} )" + "\n";
            triggerid++;
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

        string buildent = "{\n";
        buildent += "\"StartDisabled\" \"0\"\n";
        buildent += "\"spawnflags\" \"0\"\n";
        buildent += $"\"fadedist\" \"{script.fadeDistance}\"\n";
        buildent += $"\"collide_titan\" \"1\"\n";
        buildent += $"\"collide_ai\" \"1\"\n";
        buildent += $"\"scale\" \"{go.transform.localScale.x.ToString().Replace(",", ".")}\"\n";
        buildent += $"\"angles\" \"{Helper.BuildAngles(go, true)}\"\n";
        buildent += $"\"origin\" \"{Helper.BuildOrigin(go, true)}\"\n";
        buildent += "\"targetname\" \"MapEditorProp\"\n";
        buildent += "\"solid\" \"6\"\n";
        buildent += $"\"model\" \"{model}\"\n";
        buildent += "\"ClientSide\" \"0\"\n";
        buildent += "\"classname\" \"prop_dynamic\"\n";
        buildent += "}\n";

        return buildent;
    }

    public static string BuildSoundEntItem(GameObject go)
    {
        SoundScript script = go.GetComponent<SoundScript>();

        int isWaveAmbient = script.isWaveAmbient ? 1 : 0;
        int enable = script.enable ? 1 : 0;
        string origin = Helper.BuildOrigin(go, true).Replace(",", "");

        string buildent = "";

        buildent += "{\n";

        // Polyline segments
        for ( int i = script.polylineSegment.Length - 1 ; i > -1 ; i-- )
        {
            string polylineSegmentEnd = Helper.BuildOriginVector( script.polylineSegment[i], true ).ToString().Replace(",", "");

            if ( i != 0 )
            {
                string polylineSegmentStart = Helper.BuildOriginVector( script.polylineSegment[i-1], true ).ToString().Replace(",", "");

                buildent += $"\"polyline_segment_{i}\" \"{polylineSegmentStart} {polylineSegmentEnd}\"\n";
            }
            else
            {
                buildent += $"\"polyline_segment_{i}\" \"(0 0 0) {polylineSegmentEnd}\"\n";
            }
        }

        buildent += $"\"radius\" \"{script.radius}\"\n";
        buildent += $"\"model\" \"mdl/dev/editor_ambient_generic_node.rmdl\"\n"; // don't change this
        buildent += $"\"isWaveAmbient\" \"{isWaveAmbient}\"\n";
        buildent += $"\"enabled\" \"{enable}\"\n";
        buildent += $"\"origin\" \"{origin}\"\n";
        buildent += $"\"soundName\" \"{script.soundName}\"\n";
        buildent += $"\"classname\" \"" + "ambient_generic" + "\"\n"; // don't change this
        buildent += "}\n";
        
        return buildent;
    }
}