using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ImportExportJson
{
    [MenuItem("ReMap/Import/Json", false, 51)]
    private static void ImportJson()
    {
        var path = EditorUtility.OpenFilePanel("Json Export", "", "json");
        if (path.Length == 0)
            return;

        string json = System.IO.File.ReadAllText(path);
        SaveJson myObject = JsonUtility.FromJson<SaveJson>(json);

        List<PropsClass> props = myObject.Props;
        foreach(PropsClass prop in props)
        {
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(prop.Name);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = prop.Postion;
            obj.transform.eulerAngles = prop.Rotation;
            obj.name = prop.Name;
            obj.gameObject.transform.localScale = prop.Scale;

            PropScript script = obj.GetComponent<PropScript>();
            PropScriptClass propScript = prop.script;
            script.fadeDistance = propScript.FadeDistance;
            script.allowMantle = propScript.AllowMantle;
            script.realmID = propScript.RealmID;

            if (prop.Collection == "")
                continue;

            GameObject parent = GameObject.Find(prop.Collection);
            if(parent == null)
                parent = new GameObject(prop.Collection);

            obj.gameObject.transform.parent = parent.transform;
        }
    }
    [MenuItem("ReMap/Export/Json", false, 51)]
    private static void ExportJson()
    {
        var path = EditorUtility.SaveFilePanel("Json Export", "", "mapexport.json", "json");
        if (path.Length == 0)
            return;

        SaveJson save = new SaveJson();
        save.Props = new List<PropsClass>();
        save.JumpPads = new List<JumpPadsClass>();
        save.Buttons = new List<ButtonsClass>();
        save.BubbleShields = new List<BubbleShieldsClass>();
        save.WeaponRacks = new List<WeaponRacksClass>();
        save.LootBins = new List<LootBinsClass>();
        save.ZipLines = new List<ZipLinesClass>();
        save.LinkedZipLines = new List<LinkedZipLinesClass>();
        save.Doors = new List<DoorsClass>();
        save.Triggers = new List<TriggersClass>();

        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        foreach(GameObject obj in PropObjects)
        {
            PropScript script = obj.GetComponent<PropScript>();
            if (script == null)
                continue;
            
            PropsClass prop = new PropsClass();
            PropScriptClass propScript = new PropScriptClass();
            prop.Name = obj.name.Split(char.Parse(" "))[0];
            prop.Postion = obj.transform.position;
            prop.Rotation = obj.transform.rotation.eulerAngles;
            prop.Scale = obj.transform.localScale;
            propScript.AllowMantle = script.allowMantle;
            propScript.FadeDistance = script.fadeDistance;
            propScript.RealmID = script.realmID;
            prop.script = propScript;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                prop.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.Props.Add(prop);
        }

        GameObject[] JumpPadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        foreach (GameObject obj in JumpPadObjects)
        {
            PropScript script = obj.GetComponent<PropScript>();
            if (script == null)
                continue;

            JumpPadsClass jumpPad = new JumpPadsClass();
            PropScriptClass propScript = new PropScriptClass();
            jumpPad.Postion = obj.transform.position;
            jumpPad.Rotation = obj.transform.rotation.eulerAngles;
            jumpPad.Scale = obj.transform.localScale;
            propScript.AllowMantle = script.allowMantle;
            propScript.FadeDistance = script.fadeDistance;
            propScript.RealmID = script.realmID;
            jumpPad.script = propScript;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                jumpPad.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.JumpPads.Add(jumpPad);
        }

        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject obj in ButtonObjects)
        {
            ButtonScripting script = obj.GetComponent<ButtonScripting>();
            if (script == null)
                continue;

            ButtonsClass button = new ButtonsClass();
            button.Postion = obj.transform.position;
            button.Rotation = obj.transform.rotation.eulerAngles;
            button.UseText = script.UseText;
            button.OnUseCallback = script.OnUseCallback;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                button.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.Buttons.Add(button);
        }

        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        foreach (GameObject obj in BubbleShieldObjects)
        {
            BubbleScript script = obj.GetComponent<BubbleScript>();
            if (script == null)
                continue;

            BubbleShieldsClass bubbleShield = new BubbleShieldsClass();
            bubbleShield.Postion = obj.transform.position;
            bubbleShield.Rotation = obj.transform.rotation.eulerAngles;
            bubbleShield.Scale = obj.transform.localScale;
            bubbleShield.Color = script.shieldColor.r + " " + script.shieldColor.g + " " + script.shieldColor.b;
            bubbleShield.Model = obj.name.Split(char.Parse(" "))[0];

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                bubbleShield.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.BubbleShields.Add(bubbleShield);
        }

        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        foreach (GameObject obj in WeaponRackObjects)
        {
            WeaponRackScript script = obj.GetComponent<WeaponRackScript>();
            if (script == null)
                continue;

            WeaponRacksClass weaponRack = new WeaponRacksClass();
            weaponRack.Postion = obj.transform.position;
            weaponRack.Rotation = obj.transform.rotation.eulerAngles;
            weaponRack.Weapon = obj.name.Split(char.Parse(" "))[0];
            weaponRack.RespawnTime = script.respawnTime;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                weaponRack.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.WeaponRacks.Add(weaponRack);
        }

        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        foreach (GameObject obj in LootBinObjects)
        {
            LootBinScript script = obj.GetComponent<LootBinScript>();
            if (script == null)
                continue;

            LootBinsClass lootBin = new LootBinsClass();
            lootBin.Postion = obj.transform.position;
            lootBin.Rotation = obj.transform.rotation.eulerAngles;
            lootBin.Skin = script.lootbinSkin;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                lootBin.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.LootBins.Add(lootBin);
        }

        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        foreach (GameObject obj in ZipLineObjects)
        {
            ZipLinesClass zipLine = new ZipLinesClass();
            foreach (Transform child in obj.transform)
            {
                if (child.name == "zipline_start")
                    zipLine.Start = child.gameObject.transform.position;
                else if (child.name == "zipline_end")
                    zipLine.End = child.gameObject.transform.position;
            }

            if(zipLine.Start == null || zipLine.End == null)
                continue;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                zipLine.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.ZipLines.Add(zipLine);
        }

        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        foreach (GameObject obj in LinkedZipLineObjects)
        {
            LinkedZiplineScript script = obj.GetComponent<LinkedZiplineScript>();
            if (script == null)
                continue;

            List<Vector3> nodes = new List<Vector3>();
            LinkedZipLinesClass linkedZipLine = new LinkedZipLinesClass();
            foreach (Transform child in obj.transform)
                nodes.Add(child.gameObject.transform.position);

            linkedZipLine.Nodes = nodes;
            linkedZipLine.IsSmoothed = script.enableSmoothing;
            linkedZipLine.SmoothType = script.smoothType;
            linkedZipLine.SmoothAmount = script.smoothAmount;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                linkedZipLine.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.LinkedZipLines.Add(linkedZipLine);
        }

        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        foreach (GameObject obj in SingleDoorObjects)
        {
            DoorScript script = obj.GetComponent<DoorScript>();
            if (script == null)
                continue;

            DoorsClass singleDoor = new DoorsClass();
            singleDoor.Postion = obj.transform.position;
            singleDoor.Rotation = obj.transform.rotation.eulerAngles;
            singleDoor.Type = "eMapEditorDoorType.Single";
            singleDoor.Gold = script.goldDoor;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                singleDoor.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.Doors.Add(singleDoor);
        }

        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        foreach (GameObject obj in DoubleDoorObjects)
        {
            DoorScript script = obj.GetComponent<DoorScript>();
            if (script == null)
                continue;

            DoorsClass doubleDoor = new DoorsClass();
            doubleDoor.Postion = obj.transform.position;
            doubleDoor.Rotation = obj.transform.rotation.eulerAngles;
            doubleDoor.Type = "eMapEditorDoorType.Double";
            doubleDoor.Gold = script.goldDoor;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                doubleDoor.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.Doors.Add(doubleDoor);
        }

        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        foreach (GameObject obj in VertDoorObjects)
        {
            DoorsClass vertDoor = new DoorsClass();
            vertDoor.Postion = obj.transform.position;
            vertDoor.Rotation = obj.transform.rotation.eulerAngles;
            vertDoor.Type = "eMapEditorDoorType.Vertical";
            vertDoor.Gold = false;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                vertDoor.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.Doors.Add(vertDoor);
        }

        GameObject[] HorDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        foreach (GameObject obj in HorDoorObjects)
        {
            DoorsClass horDoor = new DoorsClass();
            horDoor.Postion = obj.transform.position;
            horDoor.Rotation = obj.transform.rotation.eulerAngles;
            horDoor.Type = "eMapEditorDoorType.Horizontal";
            horDoor.Gold = false;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                horDoor.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.Doors.Add(horDoor);
        }

        GameObject[] Triggers = GameObject.FindGameObjectsWithTag("Trigger");
        foreach (GameObject obj in Triggers)
        {
            TriggerScripting script = obj.GetComponent<TriggerScripting>();
            if (script == null)
                continue;

            TriggersClass trigger = new TriggersClass();
            trigger.Postion = obj.transform.position;
            trigger.Rotation = obj.transform.rotation.eulerAngles;
            trigger.Radius = obj.transform.localScale.x;
            trigger.Height = obj.transform.localScale.y;
            trigger.EnterCallback = script.EnterCallback;
            trigger.ExitCallback = script.LeaveCallback;
            trigger.Debug = script.Debug;

            if (obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                trigger.Collection = parent.name.Replace("\r", "").Replace("\n", "");
            }

            save.Triggers.Add(trigger);
        }

        string json = JsonUtility.ToJson(save);

        System.IO.File.WriteAllText(path, json);
    }

    private static UnityEngine.Object FindPrefabFromName(string name)
    {
        //Find Model GUID in Assets
        string[] results = AssetDatabase.FindAssets(name);
        if (results.Length == 0)
            return null;

        //Get model path from guid and load it
        UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
        return loadedPrefabResource;
    }
}

[Serializable]
public class PropScriptClass
{
    public bool AllowMantle;
    public float FadeDistance;
    public int RealmID;
}

[Serializable]
public class SaveJson
{
    public List<PropsClass> Props;
    public List<JumpPadsClass> JumpPads;
    public List<ButtonsClass> Buttons;
    public List<BubbleShieldsClass> BubbleShields;
    public List<WeaponRacksClass> WeaponRacks;
    public List<LootBinsClass> LootBins;
    public List<ZipLinesClass> ZipLines;
    public List<LinkedZipLinesClass> LinkedZipLines;
    public List<DoorsClass> Doors;
    public List<TriggersClass> Triggers;
}

[Serializable]
public class PropsClass
{
    public string Name;
    public Vector3 Postion;
    public Vector3 Rotation;
    public Vector3 Scale;
    public PropScriptClass script;
    public string Collection;
}

[Serializable]
public class JumpPadsClass
{
    public Vector3 Postion;
    public Vector3 Rotation;
    public Vector3 Scale;
    public PropScriptClass script;
    public string Collection;
}

[Serializable]
public class ButtonsClass
{
    public Vector3 Postion;
    public Vector3 Rotation;
    public string UseText;
    public string OnUseCallback;
    public string Collection;
}

[Serializable]
public class BubbleShieldsClass
{
    public Vector3 Postion;
    public Vector3 Rotation;
    public Vector3 Scale;
    public string Color;
    public string Model;
    public string Collection;
}

[Serializable]
public class WeaponRacksClass
{
    public Vector3 Postion;
    public Vector3 Rotation;
    public string Weapon;
    public float RespawnTime;
    public string Collection;
}

[Serializable]
public class LootBinsClass
{
    public Vector3 Postion;
    public Vector3 Rotation;
    public int Skin;
    public string Collection;
}

[Serializable]
public class ZipLinesClass
{
    public Vector3 Start;
    public Vector3 End;
    public string Collection;
}

[Serializable]
public class LinkedZipLinesClass
{
    public bool IsSmoothed;
    public bool SmoothType;
    public int SmoothAmount;
    public List<Vector3> Nodes;
    public string Collection;
}

[Serializable]
public class DoorsClass
{
    public Vector3 Postion;
    public Vector3 Rotation;
    public string Type;
    public bool Gold;
    public string Collection;
}

[Serializable]
public class TriggersClass
{
    public Vector3 Postion;
    public Vector3 Rotation;
    public string EnterCallback;
    public string ExitCallback;
    public float Radius;
    public float Height;
    public bool Debug;
    public string Collection;
}
