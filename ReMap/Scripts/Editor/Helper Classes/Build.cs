using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class Build_
{
    public static int entityIdx = 0;
    public enum BuildType {
        Map, 
        Ent, 
        Precache,
        DataTable
    };

    public static string Buttons( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.Button );
        if ( ObjectsArray.Length < 1 )
            return "";
        
        string code = "    //Buttons \n";

        foreach ( GameObject go in ObjectsArray )
        {
            ButtonScripting script = go.GetComponent<ButtonScripting>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing ButtonScripting on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }
            code += $"    AddCallback_OnUseEntity( CreateFRButton({Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, \"{script.UseText}\"), void function(entity panel, entity user, int input)" + "\n    {\n" + script.OnUseCallback + "\n    })" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string Jumpads( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.Jumppad );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //Jumppads \n";

        foreach ( GameObject go in ObjectsArray )
        {
            PropScript script = go.GetComponent<PropScript>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing PropScript on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }
            code += $"    JumpPad_CreatedCallback( MapEditor_CreateProp( $\"mdl/props/octane_jump_pad/octane_jump_pad.rmdl\", {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {script.AllowMantle.ToString().ToLower()}, {script.FadeDistance}, {script.RealmID}, {go.transform.localScale.x.ToString().Replace(",", ".")} ) )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string BubbleShields( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.BubbleShield );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //BubbleShields \n";

        foreach ( GameObject go in ObjectsArray )
        {
            string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
            BubbleScript script = go.GetComponent<BubbleScript>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing BubbleScript on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }
            string ShieldColor = script.ShieldColor.r + " " + script.ShieldColor.g + " " + script.ShieldColor.b;
                
            code += $"    MapEditor_CreateBubbleShieldWithSettings( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {go.transform.localScale.x.ToString().Replace(",", ".")}, \"{ShieldColor}\", $\"{model}\" )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string WeaponRacks( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.WeaponRack );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //Weapon Racks \n";

        foreach ( GameObject go in ObjectsArray )
        {
            WeaponRackScript script = go.GetComponent<WeaponRackScript>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing WeaponRackScript on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }
            code += $"    MapEditor_CreateRespawnableWeaponRack( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, \"{go.name.Replace("custom_weaponrack_", "mp_weapon_")}\", {script.RespawnTime} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string LootBins( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.LootBin );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //LootBins \n";

        foreach ( GameObject go in ObjectsArray )
        {
            LootBinScript script = go.GetComponent<LootBinScript>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing LootBinScript on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }
            code += $"    MapEditor_CreateLootBin( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {script.LootbinSkin} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string ZipLines( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.ZipLine );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //ZipLines \n";

        foreach ( GameObject go in ObjectsArray )
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

    public static string LinkedZipLines( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.LinkedZipline );
        
        if( ObjectsArray.Length < 1 )
            return "";

        string code = "    //LinkedZipLines \n";

        foreach ( GameObject go in ObjectsArray )
        {
            bool first = true;
            string nodes = "[ ";

            LinkedZiplineScript script = go.GetComponent<LinkedZiplineScript>();

            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing LinkedZiplineScript on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }

            foreach (Transform child in go.transform)
            {
                if (!first)
                    nodes += ", ";

                nodes += Helper.BuildOrigin(child.gameObject);

                first = false;
            }

            string SmoothType = script.SmoothType ? "GetAllPointsOnBezier" : "GetBezierOfPath";

            nodes += " ]";

            code += @"    MapEditor_CreateLinkedZipline( ";
            if (script.EnableSmoothing) code += $"{SmoothType}( ";
            code += nodes;
            if (script.EnableSmoothing) code += $", {script.SmoothAmount}";
            code += " )";
            if (script.EnableSmoothing) code += " )";
            code += "\n";
        }

        code += "\n";

        return code;
    }

    public static string VerticalZipLines( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.VerticalZipLine );

        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //VerticalZipLines \n";

        foreach ( GameObject go in ObjectsArray )
        {
            DrawVerticalZipline ziplineScript = go.GetComponent<DrawVerticalZipline>();
            if (ziplineScript == null) {
                ReMapConsole.Log("[Map Export] Missing DrawVerticalZipline on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }

            int preservevelocity = ziplineScript.PreserveVelocity  ? 1 : 0;
            int DropToBottom = ziplineScript.DropToBottom ? 1 : 0;
            string restpoint = ziplineScript.RestPoint.ToString().ToLower();
            int pushoffindirectionx = ziplineScript.PushOffInDirectionX ? 1 : 0;
            string ismoving = ziplineScript.IsMoving.ToString().ToLower();
            int DetachEndOnSpawn = ziplineScript.DetachEndOnSpawn ? 1 : 0;
            int DetachEndOnUse = ziplineScript.DetachEndOnUse ? 1 : 0;
            float PanelTimerMin = ziplineScript.PanelTimerMin;
            float PanelTimerMax = ziplineScript.PanelTimerMax;
            int PanelMaxUse = ziplineScript.PanelMaxUse;

            string panelOrigin = "[";
            string panelAngles = "[";
            string panelModel = "[";

            for(int i = 0; i < ziplineScript.Panels.Length; i++ )
            {
                panelOrigin += " " + Helper.BuildOrigin(ziplineScript.Panels[i]) + Helper.ShouldAddStartingOrg();
                panelAngles += " " + Helper.BuildAngles(ziplineScript.Panels[i]);
                panelModel += " $\"mdl/" + ziplineScript.Panels[i].name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl\"";

                if (i != ziplineScript.Panels.Length - 1)
                {
                    panelOrigin += ",";
                    panelAngles += ",";
                    panelModel += ",";
                }
            }

            panelOrigin += " ]";
            panelAngles += " ]";
            panelModel += " ]";

            code += $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(ziplineScript.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(ziplineScript.rope_start.gameObject)}, {Helper.BuildOrigin(ziplineScript.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(ziplineScript.rope_start.gameObject)}, true, {ziplineScript.FadeDistance.ToString().Replace(",", ".")}, {ziplineScript.Scale.ToString().Replace(",", ".")}, {ziplineScript.Width.ToString().Replace(",", ".")}, {ziplineScript.SpeedScale.ToString().Replace(",", ".")}, {ziplineScript.LengthScale.ToString().Replace(",", ".")}, {preservevelocity}, {DropToBottom}, {ziplineScript.AutoDetachStart.ToString().Replace(",", ".")}, {ziplineScript.AutoDetachEnd.ToString().Replace(",", ".")}, {restpoint}, {pushoffindirectionx}, {ismoving}, {DetachEndOnSpawn}, {DetachEndOnUse}, {panelOrigin}, {panelAngles}, {panelModel}, {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string NonVerticalZipLines( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.NonVerticalZipLine );

        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //NonVerticalZipLines \n";

        foreach ( GameObject go in ObjectsArray )
        {
            DrawNonVerticalZipline ziplineScript = go.GetComponent<DrawNonVerticalZipline>();
            if (ziplineScript == null) {
                ReMapConsole.Log("[Map Export] Missing DrawNonVerticalZipline on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }

            int preservevelocity = ziplineScript.PreserveVelocity  ? 1 : 0;
            int DropToBottom = ziplineScript.DropToBottom ? 1 : 0;
            string restpoint = ziplineScript.RestPoint.ToString().ToLower();
            int pushoffindirectionx = ziplineScript.PushOffInDirectionX ? 1 : 0;
            string ismoving = ziplineScript.IsMoving.ToString().ToLower();
            int DetachEndOnSpawn = ziplineScript.DetachEndOnSpawn ? 1 : 0;
            int DetachEndOnUse = ziplineScript.DetachEndOnUse ? 1 : 0;
            float PanelTimerMin = ziplineScript.PanelTimerMin;
            float PanelTimerMax = ziplineScript.PanelTimerMax;
            int PanelMaxUse = ziplineScript.PanelMaxUse;

            string panelOrigin = "[";
            string panelAngles = "[";
            string panelModel = "[";

            for (int i = 0; i < ziplineScript.Panels.Length; i++)
            {
                panelOrigin += " " + Helper.BuildOrigin(ziplineScript.Panels[i]) + Helper.ShouldAddStartingOrg();
                panelAngles += " " + Helper.BuildAngles(ziplineScript.Panels[i]);
                panelModel += " $\"mdl/" + ziplineScript.Panels[i].name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl\"";

                if (i != ziplineScript.Panels.Length - 1)
                {
                    panelOrigin += ",";
                    panelAngles += ",";
                    panelModel += ",";
                }
            }

            panelOrigin += " ]";
            panelAngles += " ]";
            panelModel += " ]";

            code += $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(ziplineScript.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(ziplineScript.rope_start.gameObject)}, {Helper.BuildOrigin(ziplineScript.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(ziplineScript.rope_start.gameObject)}, false, {ziplineScript.FadeDistance.ToString().Replace(",", ".")}, {ziplineScript.Scale.ToString().Replace(",", ".")}, {ziplineScript.Width.ToString().Replace(",", ".")}, {ziplineScript.SpeedScale.ToString().Replace(",", ".")}, {ziplineScript.LengthScale.ToString().Replace(",", ".")}, {preservevelocity}, {DropToBottom}, {ziplineScript.AutoDetachStart.ToString().Replace(",", ".")}, {ziplineScript.AutoDetachEnd.ToString().Replace(",", ".")}, {restpoint}, {pushoffindirectionx}, {ismoving}, {DetachEndOnSpawn}, {DetachEndOnUse}, {panelOrigin}, {panelAngles}, {panelModel}, {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string SingleDoors( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.SingleDoor );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //Single Doors \n";

        foreach ( GameObject go in ObjectsArray ) {
            DoorScript script = go.GetComponent<DoorScript>();

            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing DoorScript on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }

            code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Single, {script.GoldDoor.ToString().ToLower()} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string DoubleDoors( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.DoubleDoor );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //Double Doors \n";

        foreach ( GameObject go in ObjectsArray ) {
            DoorScript script = go.GetComponent<DoorScript>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing DoorScript on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }
            code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Double, {script.GoldDoor.ToString().ToLower()} )" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string VertDoors( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.VerticalDoor );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //Vertical Doors \n";

        foreach ( GameObject go in ObjectsArray )
            code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Vertical )" + "\n";

        code += "\n";

        return code;
    }

    public static string HorizontalDoors( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.HorzDoor );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //Horizontal Doors \n";

        foreach ( GameObject go in ObjectsArray )
            code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, eMapEditorDoorType.Horizontal )" + "\n";

        code += "\n";

        return code;
    }

    public static string Props( GameObject[] ObjectsArray = null, BuildType type = BuildType.Map, bool isexport = false )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.Prop );

        if ( ObjectsArray.Length < 1 ) return "";

        List<String> precacheList = new List<String>();

        entityIdx = 0;

        string code = "";

        switch(type)
        {
            case BuildType.Map:
                code += "    //Props \n";
                break;
            case BuildType.DataTable:
                code += "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"" + "\n";
                break;
            //case BuildType.Ent:
                //code += $"ENTITIES02 num_models={ObjectsArray.Length}\n";
                //break;
        }

        foreach ( GameObject go in ObjectsArray )
        {
            string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
            PropScript script = go.GetComponent<PropScript>();

            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing PropScript on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }

            switch (type) {
                case BuildType.Ent:
                    code += BuildScriptEntItem(go, isexport);
                    continue;
                case BuildType.DataTable:
                    code += BuildDataTableItem(go, isexport);
                    continue;
                case BuildType.Precache:
                    if (precacheList.Contains(model))
                        continue;

                    precacheList.Add(model);
                    code += $"    PrecacheModel( $\"{model}\" )" + "\n";
                    continue;
                case BuildType.Map:
                    if (isexport)
                        ReMapConsole.Log("[Map Export] Exporting: " + model, ReMapConsole.LogType.Info);

                    string idx = "";
                    if ( script.Parameters.Count != 0 || script.CustomParameters.Count != 0 )
                    {
                        idx = $"prop{CreateEntityIxd()}";
                        code += $"    entity {idx} = ";
                    } else code += $"    ";
                    
                    code += $"MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {script.AllowMantle.ToString().ToLower()}, {script.FadeDistance}, {script.RealmID}, {go.transform.localScale.x.ToString().Replace(",", ".")} )" + "\n";

                    if ( script.Parameters.Count != 0 || script.CustomParameters.Count != 0 )
                    {
                        foreach ( PropScriptParameters param in script.Parameters )
                        {
                            code += GetPropScriptParamValue( idx, param );
                        }

                        foreach ( string param in script.CustomParameters )
                        {
                            code += SetPropScriptParamValue( idx, param );
                        }
                    }
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


    public static string Sounds( GameObject[] ObjectsArray = null, bool isexport = false)
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.Sound );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = ""; //= $"ENTITIES02 num_models={ObjectsArray.Length}\n";

        foreach ( GameObject go in ObjectsArray )
            code += BuildSoundEntItem(go, isexport);

        return code;
    }


    public static string Triggers( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.Trigger );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    //Triggers \n";

        int triggerid = 0;
        foreach ( GameObject go in ObjectsArray )
        {
            TriggerScripting script = go.GetComponent<TriggerScripting>();
            code += $"    entity trigger" + triggerid + $" = MapEditor_CreateTrigger( {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {go.transform.localScale.x.ToString().Replace(",", ".")}, {go.transform.localScale.y.ToString().Replace(",", ".")}, {script.Debug.ToString().ToLower()} )" + "\n";

            string ReplacedEnterCallback = script.EnterCallback;
            string ReplacedLeaveCallback = script.LeaveCallback;

            Transform teleportationInfo = script.PlayerTeleportationInfo;

            if ( teleportationInfo != null )
            {
                ReplacedEnterCallback = ChangeTriggerLocalization( ReplacedEnterCallback, "#TPORIGIN", Helper.BuildOrigin( teleportationInfo.gameObject ), "< 0, 0, 0 >", teleportationInfo.gameObject );
                ReplacedEnterCallback = ChangeTriggerLocalization( ReplacedEnterCallback, "#TPANGLES", Helper.BuildAngles( teleportationInfo.gameObject ), "< 0, 0, 0 >", teleportationInfo.gameObject );
                ReplacedEnterCallback = ChangeTriggerLocalization( ReplacedEnterCallback, "#OFFSET", "+ startingorg", "", teleportationInfo.gameObject );
                ReplacedLeaveCallback = ChangeTriggerLocalization( ReplacedLeaveCallback, "#TPORIGIN", Helper.BuildOrigin( teleportationInfo.gameObject ), "< 0, 0, 0 >", teleportationInfo.gameObject );
                ReplacedLeaveCallback = ChangeTriggerLocalization( ReplacedLeaveCallback, "#TPANGLES", Helper.BuildAngles( teleportationInfo.gameObject ), "< 0, 0, 0 >", teleportationInfo.gameObject );
                ReplacedLeaveCallback = ChangeTriggerLocalization( ReplacedLeaveCallback, "#OFFSET", "+ startingorg", "", teleportationInfo.gameObject );
            }

            if (ReplacedEnterCallback != "")
                code += $"    trigger{triggerid}.SetEnterCallback( void function(entity trigger , entity ent)" + "{\n" + ReplacedEnterCallback + "\n    })" + "\n";

            if (ReplacedLeaveCallback != "")
                code += $"    trigger{triggerid}.SetLeaveCallback( void function(entity trigger , entity ent)" + "{\n" + ReplacedLeaveCallback + "\n    })" + "\n";

            code += $"    DispatchSpawn( trigger{triggerid} )" + "\n";
            triggerid++;
        }
        code += "\n";

        return code;
    }

    public static string NewLocPair( GameObject[] ObjectsArray = null , bool isexport = false)
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.SpawnPoint );
        if ( ObjectsArray.Length < 1 )
            return "";

        string code = "    // NewLocPair\n\n";

        code += Helper.ShouldAddStartingOrg( StartingOriginType.SquirrelFunction );

        foreach ( GameObject go in ObjectsArray )
            code += BuildNewLocPairItem(go, isexport);

        return code;
    }

    public static string TextInfoPanel( GameObject[] ObjectsArray = null )
    {
        if ( ObjectsArray == null ) ObjectsArray = Helper.GetObjArrayWithEnum( ObjectType.TextInfoPanel );
        if ( ObjectsArray.Length < 1 )
            return "";
        
        string code = "    //TextInfoPanels \n";

        foreach ( GameObject go in ObjectsArray )
        {
            TextInfoPanelScript script = go.GetComponent<TextInfoPanelScript>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing ButtonScripting on: " + go.name, ReMapConsole.LogType.Error);
                continue;
            }
            code += $"    MapEditor_CreateTextInfoPanel( \"{script.Title}\", \"{script.Description}\", {Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)}, {script.showPIN.ToString().ToLower()}, {script.Scale})" + "\n";
        }

        code += "\n";

        return code;
    }

    public static string BuildDataTableItem(GameObject go, bool isexport)
    {
        string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
        PropScript script = go.GetComponent<PropScript>();

        if (isexport)
            ReMapConsole.Log("[Datatable Export] Exporting: " + model, ReMapConsole.LogType.Info);

        string type = "\"dynamic_prop\",";
        string origin = "\"" + Helper.BuildOrigin(go).Replace(" ", "") + "\",";
        string angles = "\"" + Helper.BuildAngles(go).Replace(" ", "") + "\",";
        string scale = go.transform.localScale.x.ToString().Replace(",", ".") + ",";
        string fade = script.FadeDistance.ToString() + ",";
        string mantle = script.AllowMantle.ToString().ToLower() + ",";
        string visible = "true,";
        string mdl = "\"" + model + "\",";
        string collection = "\"\"";

        if (go.transform.parent != null) {
            GameObject parent = go.transform.parent.gameObject;
            collection = "\"" + parent.name.Replace("\r", "").Replace("\n", "") + "\"";
        }

        return type + origin + angles + scale + fade + mantle + visible + mdl + collection + "\n";
    }

    public static string BuildScriptEntItem(GameObject go, bool isexport)
    {
        string model = go.name.Split(char.Parse(" "))[0].Replace("#", "/") + ".rmdl";
        PropScript script = go.GetComponent<PropScript>();

        if (isexport)
            ReMapConsole.Log("[Script.ent Export] Exporting: " + model, ReMapConsole.LogType.Info);

        string buildent = "{\n";
        buildent += "\"StartDisabled\" \"0\"\n";
        buildent += "\"spawnflags\" \"0\"\n";
        buildent += $"\"fadedist\" \"{script.FadeDistance}\"\n";
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

    public static string BuildSoundEntItem(GameObject go, bool isexport)
    {
        SoundScript script = go.GetComponent<SoundScript>();

        int IsWaveAmbient = script.IsWaveAmbient ? 1 : 0;
        int Enable = script.Enable ? 1 : 0;
        string origin = Helper.BuildOrigin(go, true).Replace(",", "");

        if (isexport)
            ReMapConsole.Log("[Sound.ent Export] Exporting: " + script.SoundName, ReMapConsole.LogType.Info);

        string buildent = "";

        buildent += "{\n";

        // Polyline segments
        for ( int i = script.PolylineSegment.Length - 1 ; i > -1 ; i-- )
        {
            string polylineSegmentEnd = Helper.BuildOriginVector( script.PolylineSegment[i], true ).ToString().Replace(",", "");

            if ( i != 0 )
            {
                string polylineSegmentStart = Helper.BuildOriginVector( script.PolylineSegment[i-1], true ).ToString().Replace(",", "");

                buildent += $"\"polyline_segment_{i}\" \"{polylineSegmentStart} {polylineSegmentEnd}\"\n";
            }
            else
            {
                buildent += $"\"polyline_segment_{i}\" \"(0 0 0) {polylineSegmentEnd}\"\n";
            }
        }

        buildent += $"\"radius\" \"{script.Radius}\"\n";
        buildent += $"\"model\" \"mdl/dev/editor_ambient_generic_node.rmdl\"\n"; // don't change this
        buildent += $"\"isWaveAmbient\" \"{IsWaveAmbient}\"\n";
        buildent += $"\"enabled\" \"{Enable}\"\n";
        buildent += $"\"origin\" \"{origin}\"\n";
        buildent += $"\"soundName\" \"{script.SoundName}\"\n";
        buildent += $"\"classname\" \"" + "ambient_generic" + "\"\n"; // don't change this
        buildent += "}\n";
        
        return buildent;
    }

    public static string CreateEntityIxd()
    {
        return $"{entityIdx++.ToString( "000" )}";
    }

    public static string GetPropScriptParamValue( string name, PropScriptParameters param )
    {
        string paramStr = "";

        switch ( param )
        {
            case PropScriptParameters.PlayerClip:
                paramStr += $"    {name}.MakeInvisible()" + "\n";
                paramStr += $"    {name}.kv.solid = 6" + "\n";
                paramStr += $"    {name}.kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER" + "\n";
                paramStr += $"    {name}.kv.contents = CONTENTS_PLAYERCLIP" + "\n";
                break;
            case PropScriptParameters.PlayerNoClimb:       paramStr += $"    {name}.kv.solid = 3" + "\n"; break;
            case PropScriptParameters.MakeInvisible:       paramStr += $"    {name}.MakeInvisible()" + "\n"; break;

            case PropScriptParameters.KvSolidNoCollision:  paramStr += $"    {name}.kv.solid = 0" + "\n"; break;
            case PropScriptParameters.KvSolidBoundingBox:  paramStr += $"    {name}.kv.solid = 2" + "\n"; break;
            case PropScriptParameters.KvSolidNoFriction:   paramStr += $"    {name}.kv.solid = 3" + "\n"; break;
            case PropScriptParameters.KvSolidUseVPhysics:  paramStr += $"    {name}.kv.solid = 6" + "\n"; break;
            case PropScriptParameters.KvSolidHitboxOnly:   paramStr += $"    {name}.kv.solid = 8" + "\n"; break;

            case PropScriptParameters.KvContentsNOGRAPPLE: paramStr += $"    {name}.kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE" + "\n"; break;


            default: break;
        }

        return paramStr;
    }

    public static string SetPropScriptParamValue( string name, string param )
    {
        return $"    {name}{param}" + "\n";
    }

    public static string BuildNewLocPairItem(GameObject go, bool isexport)
    {
        string buildCode = "";

        buildCode += $"    NewLocPair({Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(go)})\n";
        
        return buildCode;
    }

    private static string ChangeTriggerLocalization( string callback, string searchTerm, string replacedString, string ifInvalidInfo, GameObject infoObject )
    {
        int index = callback.IndexOf(searchTerm);
        while (index >= 0)
        {
            callback = callback.Substring(0, index) + replacedString + callback.Substring(index + searchTerm.Length);
            index = callback.IndexOf(searchTerm, index + replacedString.Length);
        }
        return callback;
    }
}