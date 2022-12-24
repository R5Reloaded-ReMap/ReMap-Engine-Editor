using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ImportExportJson
{
    static SaveJson save = new SaveJson();

    [MenuItem("ReMap/Import/Json", false, 51)]
    public static async void ImportJson()
    {
        var path = EditorUtility.OpenFilePanel("Json Import", "", "json");
        if (path.Length == 0)
            return;

        EditorUtility.DisplayProgressBar("Starting Import", "Reading File" , 0);
        string json = System.IO.File.ReadAllText(path);
        SaveJson myObject = JsonUtility.FromJson<SaveJson>(json);

        await ImportProps(myObject.Props);
        await ImportJumppads(myObject.JumpPads);
        await ImportButtons(myObject.Buttons);
        await ImportBubbleSheilds(myObject.BubbleShields);
        await ImportWeaponRacks(myObject.WeaponRacks);
        await ImportLootBins(myObject.LootBins);
        await ImportZipLines(myObject.ZipLines, myObject.LinkedZipLines);
        await ImportDoors(myObject.Doors);
        await ImportTriggers(myObject.Triggers);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ImportProps(List<PropsClass> Props)
    {
        int i = 0;
        foreach(PropsClass prop in Props)
        {
            EditorUtility.DisplayProgressBar("Importing Props", "Importing: " + prop.Name, (i + 1) / (float)Props.Count);
            i++;

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
        }
    }

    private static async Task ImportJumppads(List<JumpPadsClass> JumpPads)
    {
        int i = 0;
        foreach(JumpPadsClass jumppad in JumpPads)
        {
            EditorUtility.DisplayProgressBar("Importing Jumppads", "Importing: custom_jumppad " + i, (i + 1) / (float)JumpPads.Count);

            string Model = "custom_jumppad";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = jumppad.Postion;
            obj.transform.eulerAngles = jumppad.Rotation;
            obj.name = Model;
            obj.gameObject.transform.localScale =  jumppad.Scale;

            PropScript script = obj.GetComponent<PropScript>();
            PropScriptClass propScript = jumppad.script;
            propScript.FadeDistance = script.fadeDistance;
            propScript.AllowMantle = script.allowMantle;
            propScript.RealmID = script.realmID;

            GameObject parent = GameObject.Find(jumppad.Collection);
            if(parent == null)
                parent = new GameObject(jumppad.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportButtons(List<ButtonsClass> Buttons)
    {
        int i = 0;
        foreach(ButtonsClass button in Buttons)
        {
            EditorUtility.DisplayProgressBar("Importing Buttons", "Importing: custom_button " + i, (i + 1) / (float)Buttons.Count);

            string Model = "custom_button";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = button.Postion;
            obj.transform.eulerAngles = button.Rotation;
            obj.name = Model;

            ButtonScripting script = obj.GetComponent<ButtonScripting>();
            script.OnUseCallback = button.OnUseCallback;
            script.UseText = button.UseText;

            GameObject parent = GameObject.Find(button.Collection);
            if(parent == null)
                parent = new GameObject(button.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportBubbleSheilds(List<BubbleShieldsClass> BSheilds)
    {
        int i = 0;
        foreach(BubbleShieldsClass sheild in BSheilds)
        {
            EditorUtility.DisplayProgressBar("Importing BubbleShields", "Importing: " + sheild.Model, (i + 1) / (float)BSheilds.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(sheild.Model);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = sheild.Postion;
            obj.transform.eulerAngles = sheild.Rotation;
            obj.name = sheild.Model;
            obj.gameObject.transform.localScale = sheild.Scale;

            BubbleScript script = obj.GetComponent<BubbleScript>();
            string[] split = sheild.Color.Split(" ");
            script.shieldColor.r = byte.Parse(split[0].Replace("\"", ""));
            script.shieldColor.g = byte.Parse(split[1].Replace("\"", ""));
            script.shieldColor.b = byte.Parse(split[2].Replace("\"", ""));

            GameObject parent = GameObject.Find(sheild.Collection);
            if(parent == null)
                parent = new GameObject(sheild.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportWeaponRacks(List<WeaponRacksClass> WeaponRacks)
    {
        int i = 0;
        foreach(WeaponRacksClass weaponrack in WeaponRacks)
        {
            EditorUtility.DisplayProgressBar("Importing WeaponRacks", "Importing: " + weaponrack.Weapon, (i + 1) / (float)WeaponRacks.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(weaponrack.Weapon);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = weaponrack.Postion;
            obj.transform.eulerAngles = weaponrack.Rotation;
            obj.name = weaponrack.Weapon;

            WeaponRackScript script = obj.GetComponent<WeaponRackScript>();
            script.respawnTime = weaponrack.RespawnTime;

            GameObject parent = GameObject.Find(weaponrack.Collection);
            if(parent == null)
                parent = new GameObject(weaponrack.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportLootBins(List<LootBinsClass> LootBins)
    {
        int i = 0;
        foreach(LootBinsClass lootbin in LootBins)
        {
            EditorUtility.DisplayProgressBar("Importing LootBins", "Importing: custom_lootbin " + i, (i + 1) / (float)LootBins.Count);

            string Model = "custom_lootbin";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = lootbin.Postion;
            obj.transform.eulerAngles = lootbin.Rotation;
            obj.name = Model;

            LootBinScript script = obj.GetComponent<LootBinScript>();
            script.lootbinSkin = lootbin.Skin;

            GameObject parent = GameObject.Find(lootbin.Collection);
            if(parent == null)
                parent = new GameObject(lootbin.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportZipLines(List<ZipLinesClass> Ziplines, List<LinkedZipLinesClass> LinkedZiplines)
    {
        int i = 0;
        foreach(ZipLinesClass zipline in Ziplines)
        {
            EditorUtility.DisplayProgressBar("Importing Ziplines", "Importing: custom_zipline" + i, (i + 1) / (float)Ziplines.Count);

            string Model = "custom_zipline";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            foreach (Transform child in obj.transform)
            {
                if (child.name == "zipline_start")
                    child.transform.position = zipline.Start;
                else if (child.name == "zipline_end")
                    child.transform.position = zipline.End;
            }

            GameObject parent = GameObject.Find(zipline.Collection);
            if(parent == null)
                parent = new GameObject(zipline.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        foreach(LinkedZipLinesClass zipline in LinkedZiplines)
        {
            EditorUtility.DisplayProgressBar("Importing LinkedZiplines", "Importing: BubbleShields" + i, (i + 1) / (float)Ziplines.Count);

            GameObject obj = new GameObject("custom_linked_zipline");
            obj.AddComponent<DrawLinkedZipline>();
            obj.AddComponent<LinkedZiplineScript>();
            
            foreach(Vector3 v in zipline.Nodes)
            {
                GameObject child = new GameObject("zipline_node");
                child.transform.position = v;
                child.transform.parent = obj.transform;
                    
                i++;
            }

            LinkedZiplineScript script = obj.GetComponent<LinkedZiplineScript>();
            script.enableSmoothing = zipline.IsSmoothed;
            script.smoothType = zipline.SmoothType;
            script.smoothAmount = zipline.SmoothAmount;

            GameObject parent = GameObject.Find(zipline.Collection);
            if(parent == null)
                parent = new GameObject(zipline.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportDoors(List<DoorsClass> Doors)
    {
        int i = 0;
        foreach(DoorsClass door in Doors)
        {
            string Model = "custom_single_door";
            bool IsSingleOrDouble = false;
            switch(door.Type)
            {
                case "eMapEditorDoorType.Single":
                    Model = "custom_single_door";
                    IsSingleOrDouble = true;
                    break;
                case "eMapEditorDoorType.Double":
                    Model = "custom_double_door";
                    IsSingleOrDouble = true;
                    break;
                case "eMapEditorDoorType.Vertical":
                    Model = "custom_vertical_door";
                    break;
                case "eMapEditorDoorType.Horizontal":
                    Model = "custom_sliding_door";
                    break;
            }

            EditorUtility.DisplayProgressBar("Importing Doors", "Importing: " + Model, (i + 1) / (float)Doors.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = door.Postion;
            obj.transform.eulerAngles = door.Rotation;
            obj.name = Model;

            if(IsSingleOrDouble)
            {
                DoorScript script = obj.GetComponent<DoorScript>();
                script.goldDoor = door.Gold;
            }

            GameObject parent = GameObject.Find(door.Collection);
            if(parent == null)
                parent = new GameObject(door.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ImportTriggers(List<TriggersClass> Triggers)
    {
        int i = 0;
        foreach(TriggersClass trigger in Triggers)
        {
            EditorUtility.DisplayProgressBar("Importing BubbleShields", "Importing: BubbleShields" + i, (i + 1) / (float)Triggers.Count);

            string Model = "trigger_cylinder";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = trigger.Postion;
            obj.transform.eulerAngles = trigger.Rotation;
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3(trigger.Radius, trigger.Height, trigger.Radius);

            TriggerScripting script = obj.GetComponent<TriggerScripting>();
            script.Debug = trigger.Debug;
            script.EnterCallback = trigger.EnterCallback;
            script.LeaveCallback = trigger.ExitCallback;

            GameObject parent = GameObject.Find(trigger.Collection);
            if(parent == null)
                parent = new GameObject(trigger.Collection);

            obj.gameObject.transform.parent = parent.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    [MenuItem("ReMap/Export/Json", false, 51)]
    public static async void ExportJson()
    {
        var path = EditorUtility.SaveFilePanel("Json Export", "", "mapexport.json", "json");
        if (path.Length == 0)
            return;

        EditorUtility.DisplayProgressBar("Starting Export", "" , 0);

        ResetJsonSave();
        await ExportProps();
        await ExportJumppads();
        await ExportButtons();
        await ExportBubbleSheilds();
        await ExportWeaponRacks();
        await ExportLootBins();
        await ExportZipLines();
        await ExportDoors();
        await ExportTriggers();

        string json = JsonUtility.ToJson(save);
        System.IO.File.WriteAllText(path, json);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ExportProps()
    {
        int i = 0;
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        foreach(GameObject obj in PropObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Props", "Exporting: " + obj.name, (i + 1) / (float)PropObjects.Length);
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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportJumppads()
    {
        int i = 0;
        GameObject[] JumpPadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        foreach (GameObject obj in JumpPadObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Jumppads", "Exporting: " + obj.name, (i + 1) / (float)JumpPadObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportButtons()
    {
        int i = 0;
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject obj in ButtonObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Buttons", "Exporting: " + obj.name, (i + 1) / (float)ButtonObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportBubbleSheilds()
    {
        int i = 0;
        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        foreach (GameObject obj in BubbleShieldObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting BubbleShields", "Exporting: " + obj.name, (i + 1) / (float)BubbleShieldObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportWeaponRacks()
    {
        int i = 0;
        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        foreach (GameObject obj in WeaponRackObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting LootBins", "Exporting: " + obj.name, (i + 1) / (float)WeaponRackObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportLootBins()
    {
        int i = 0;
        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        foreach (GameObject obj in LootBinObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting LootBins", "Exporting: " + obj.name, (i + 1) / (float)LootBinObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportZipLines()
    {
        int i = 0;
        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        foreach (GameObject obj in ZipLineObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Ziplines", "Exporting: " + obj.name, (i + 1) / (float)ZipLineObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        foreach (GameObject obj in LinkedZipLineObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Ziplines", "Exporting: " + obj.name, (i + 1) / (float)LinkedZipLineObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportDoors()
    {
        int i = 0;
        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        foreach (GameObject obj in SingleDoorObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)SingleDoorObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        foreach (GameObject obj in DoubleDoorObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)DoubleDoorObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        i = 0;
        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        foreach (GameObject obj in VertDoorObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)VertDoorObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        GameObject[] HorDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        foreach (GameObject obj in HorDoorObjects)
        {
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)HorDoorObjects.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static async Task ExportTriggers()
    {
        GameObject[] Triggers = GameObject.FindGameObjectsWithTag("Trigger");
        int i = 0;
        foreach (GameObject obj in Triggers)
        {
            EditorUtility.DisplayProgressBar("Exporting Triggers", "Exporting: " + obj.name, (i + 1) / (float)Triggers.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }
    }

    private static void ResetJsonSave()
    {
        save = new SaveJson();
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
