using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ImportExportJson
{
    static SaveJson save = new SaveJson();

    static string[] protectedModels =
    {
        "_vertical_zipline",
        "_non_vertical_zipline"
    };

    [MenuItem("ReMap/Import/Json", false, 51)]
    public static async void ImportJson()
    {
        var path = EditorUtility.OpenFilePanel("Json Import", "", "json");
        if (path.Length == 0)
            return;

        ReMapConsole.Log("[Json Import] Reading file: " + path, ReMapConsole.LogType.Warning);
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

        ReMapConsole.Log("[Json Import] Finished", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ImportProps(List<PropsClass> Props)
    {
        int i = 0;
        foreach(PropsClass prop in Props)
        {
            ReMapConsole.Log("[Json Import] Importing: " + prop.Name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Props", "Importing: " + prop.Name, (i + 1) / (float)Props.Count);
            i++;

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(prop.Name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {prop.Name}" , ReMapConsole.LogType.Error);
                continue;
            }

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

            List<string> parentsList = new List<string>();
            int startIndex = 0;
            while (true)
            {
                int slashIndex = prop.Collection.IndexOf("/", startIndex);
                if (slashIndex < 0)
                {
                    parentsList.Add(prop.Collection.Substring(startIndex));
                    break;
                }
                else
                {
                    parentsList.Add(prop.Collection.Substring(startIndex, slashIndex - startIndex));
                    startIndex = slashIndex + 1;
                }
            }
            string[] parents = parentsList.ToArray();

            GameObject folder;
            folder = GameObject.Find(parents[0].Split("|")[0]);
                if(folder == null)
            folder = new GameObject(parents[0].Split("|")[0]);

            string[] partsPosF = parents[0].Split("|")[1].Replace("(", "").Replace(")", "").Replace(" ", "").Split(",");
            string[] partsAngF = parents[0].Split("|")[2].Replace("(", "").Replace(")", "").Replace(" ", "").Split(",");

            float xPosF = float.Parse(partsPosF[0].Replace(".", ","));
            float yPosF = float.Parse(partsPosF[1].Replace(".", ","));
            float zPosF = float.Parse(partsPosF[2].Replace(".", ","));

            float xAngF = float.Parse(partsAngF[0].Replace(".", ","));
            float yAngF = float.Parse(partsAngF[1].Replace(".", ","));
            float zAngF = float.Parse(partsAngF[2].Replace(".", ","));

            folder.transform.position = new Vector3( xPosF, yPosF, zPosF );
            folder.transform.eulerAngles = new Vector3( xAngF, yAngF, zAngF );

            int folderNum = parents.Length;

            string path = parents[0].Split("|")[0];

            if ( folderNum >= 2 )
            {
                for ( int j = 1 ; j < folderNum ; j++ )
                {
                    string parentName = parents[j].Split("|")[0];
                    string parentPosString = parents[j].Split("|")[1];
                    string parentAngString = parents[j].Split("|")[2];

                    string[] partsPos = parentPosString.Replace("(", "").Replace(")", "").Replace(" ", "").Split(",");
                    string[] partsAng = parentAngString.Replace("(", "").Replace(")", "").Replace(" ", "").Split(",");

                    path = path + "/" + parentName;
                    GameObject newFolder;
                    newFolder = GameObject.Find(path);
                        if(newFolder == null)
                    newFolder = new GameObject(parentName);

                    float xPos = float.Parse(partsPos[0].Replace(".", ","));
                    float yPos = float.Parse(partsPos[1].Replace(".", ","));
                    float zPos = float.Parse(partsPos[2].Replace(".", ","));

                    float xAng = float.Parse(partsAng[0].Replace(".", ","));
                    float yAng = float.Parse(partsAng[1].Replace(".", ","));
                    float zAng = float.Parse(partsAng[2].Replace(".", ","));

                    newFolder.transform.position = new Vector3( xPos, yPos, zPos );
                    newFolder.transform.eulerAngles = new Vector3( xAng, yAng, zAng );

                    newFolder.transform.SetParent(folder.transform);

                    folder = newFolder;
                }
            }

            obj.gameObject.transform.parent = folder.transform;

            await Task.Delay(TimeSpan.FromSeconds(0.001));
        }
    }

    private static async Task ImportJumppads(List<JumpPadsClass> JumpPads)
    {
        int i = 0;
        foreach(JumpPadsClass jumppad in JumpPads)
        {
            ReMapConsole.Log("[Json Import] Importing: custom_jumppad", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Jumppads", "Importing: custom_jumppad " + i, (i + 1) / (float)JumpPads.Count);

            string Model = "custom_jumppad";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

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
            ReMapConsole.Log("[Json Import] Importing: custom_button", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Buttons", "Importing: custom_button " + i, (i + 1) / (float)Buttons.Count);

            string Model = "custom_button";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

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
            ReMapConsole.Log("[Json Import] Importing: " + sheild.Model, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing BubbleShields", "Importing: " + sheild.Model, (i + 1) / (float)BSheilds.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(sheild.Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {sheild.Model}" , ReMapConsole.LogType.Error);
                continue;
            }

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
            ReMapConsole.Log("[Json Import] Importing: " + weaponrack.Weapon, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing WeaponRacks", "Importing: " + weaponrack.Weapon, (i + 1) / (float)WeaponRacks.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(weaponrack.Weapon);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {weaponrack.Weapon}" , ReMapConsole.LogType.Error);
                continue;
            }

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
            ReMapConsole.Log("[Json Import] Importing: custom_lootbin", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing LootBins", "Importing: custom_lootbin " + i, (i + 1) / (float)LootBins.Count);

            string Model = "custom_lootbin";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

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
            ReMapConsole.Log("[Json Import] Importing: custom_zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Ziplines", "Importing: custom_zipline " + i, (i + 1) / (float)Ziplines.Count);

            string Model = "custom_zipline";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

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
            ReMapConsole.Log("[Json Import] Importing: Linked Zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing LinkedZiplines", "Importing: Linked Zipline " + i, (i + 1) / (float)Ziplines.Count);

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

            ReMapConsole.Log("[Json Import] Importing: " + Model, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Doors", "Importing: " + Model, (i + 1) / (float)Doors.Count);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

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
            ReMapConsole.Log("[Json Import] Importing: Trigger", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing Triggers", "Importing: Triggers" + i, (i + 1) / (float)Triggers.Count);

            string Model = "trigger_cylinder";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

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
        Helper.FixPropTags();

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

        ReMapConsole.Log("[Json Export] Writing to file: " + path, ReMapConsole.LogType.Warning);
        string json = JsonUtility.ToJson(save);
        System.IO.File.WriteAllText(path, json);

        ReMapConsole.Log("[Json Export] Finished.", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ExportProps()
    {
        int i = 0;
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        foreach(GameObject obj in PropObjects)
        {
            PropScript script = obj.GetComponent<PropScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing PropScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Props", "Exporting: " + obj.name, (i + 1) / (float)PropObjects.Length);
            
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

            prop.Collection = FindCollectionPath( obj );

            if ( !prop.Collection.Contains(protectedModels[0]) && !prop.Collection.Contains(protectedModels[1]) )
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
            PropScript script = obj.GetComponent<PropScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing PropScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Jumppads", "Exporting: " + obj.name, (i + 1) / (float)JumpPadObjects.Length);

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
            ButtonScripting script = obj.GetComponent<ButtonScripting>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing ButtonScripting on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Buttons", "Exporting: " + obj.name, (i + 1) / (float)ButtonObjects.Length);

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
            BubbleScript script = obj.GetComponent<BubbleScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing BubbleScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting BubbleShields", "Exporting: " + obj.name, (i + 1) / (float)BubbleShieldObjects.Length);

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
            WeaponRackScript script = obj.GetComponent<WeaponRackScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing WeaponRackScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting LootBins", "Exporting: " + obj.name, (i + 1) / (float)WeaponRackObjects.Length);

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
            LootBinScript script = obj.GetComponent<LootBinScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing LootBinScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting LootBins", "Exporting: " + obj.name, (i + 1) / (float)LootBinObjects.Length);

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
            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
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
            LinkedZiplineScript script = obj.GetComponent<LinkedZiplineScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing LinkedZiplineScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Ziplines", "Exporting: " + obj.name, (i + 1) / (float)LinkedZipLineObjects.Length);

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

        i = 0;
        GameObject[] VerticalZipLineObjects = GameObject.FindGameObjectsWithTag("VerticalZipLine");
        foreach (GameObject obj in VerticalZipLineObjects)
        {
            DrawVerticalZipline script = obj.GetComponent<DrawVerticalZipline>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing DrawVerticalZipline on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Ziplines", "Exporting: " + obj.name, (i + 1) / (float)VerticalZipLineObjects.Length);

            VerticalZipLinesClass verticalZipLine = new VerticalZipLinesClass();

            verticalZipLine.ZiplinePosition = script.zipline.position;
            verticalZipLine.ZiplineAngles = script.zipline.eulerAngles;
            verticalZipLine.ArmOffset = script.armOffset;
            verticalZipLine.HeightOffset = script.heightOffset;
            verticalZipLine.AnglesOffset = script.anglesOffset;
            verticalZipLine.FadeDistance = script.fadeDistance;
            verticalZipLine.Scale = script.scale;
            verticalZipLine.Width = script.width;
            verticalZipLine.SpeedScale = script.speedScale;
            verticalZipLine.LengthScale = script.lengthScale;
            verticalZipLine.PreserveVelocity = script.preserveVelocity;
            verticalZipLine.DropToBottom = script.dropToBottom;
            verticalZipLine.AutoDetachStart = script.autoDetachStart;
            verticalZipLine.AutoDetachEnd = script.autoDetachEnd;
            verticalZipLine.RestPoint = script.restPoint;
            verticalZipLine.PushOffInDirectionX = script.pushOffInDirectionX;
            verticalZipLine.IsMoving = script.isMoving;
            verticalZipLine.DetachEndOnSpawn = script.detachEndOnSpawn;
            verticalZipLine.DetachEndOnUse = script.detachEndOnUse;
            //public GameObject[] Panels; // How to convert each game objects ( model / position / angles )
            verticalZipLine.PanelTimerMin = script.panelTimerMin;
            verticalZipLine.PanelTimerMax = script.panelTimerMax;
            verticalZipLine.PanelMaxUse = script.panelMaxUse;

            verticalZipLine.Collection = FindCollectionPath( obj );

            save.VerticalZipLines.Add(verticalZipLine);

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
            DoorScript script = obj.GetComponent<DoorScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing DoorScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)SingleDoorObjects.Length);

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
            DoorScript script = obj.GetComponent<DoorScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing DoorScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Doors", "Exporting: " + obj.name, (i + 1) / (float)DoubleDoorObjects.Length);

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
            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
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
            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
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
            TriggerScripting script = obj.GetComponent<TriggerScripting>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing TriggerScripting on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Exporting Triggers", "Exporting: " + obj.name, (i + 1) / (float)Triggers.Length);

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
        save.VerticalZipLines = new List<VerticalZipLinesClass>();
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

    private static string FindCollectionPath( GameObject obj )
    {
        List<GameObject> parents = new List<GameObject>();
        GameObject currentParent = obj;
        string collectionPath = "";

        while (currentParent.transform.parent != null)
        {
            if ( currentParent != obj ) parents.Add(currentParent);
            currentParent = currentParent.transform.parent.gameObject;
        }

        if ( currentParent != obj ) parents.Add(currentParent);

        foreach (GameObject parent in parents)
        {
            Vector3 pos = parent.transform.position;
            Vector3 ang = parent.transform.eulerAngles;
            collectionPath = $"{parent.name}|{parent.transform.position}|{parent.transform.eulerAngles}/{collectionPath}";
        }

        int lastSlashIndex = collectionPath.LastIndexOf("/");
        if (lastSlashIndex >= 0)
        {
            collectionPath = collectionPath.Remove(lastSlashIndex, collectionPath.Length - lastSlashIndex);
        }

        return collectionPath.Replace("\r", "").Replace("\n", "");
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
    public List<VerticalZipLinesClass> VerticalZipLines;
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
public class VerticalZipLinesClass
{
    public Vector3  ZiplinePosition;
    public Vector3 ZiplineAngles;
    public float ArmOffset;
    public float HeightOffset;
    public float AnglesOffset;
    public float FadeDistance;
    public float Scale;
    public float Width;
    public float SpeedScale;
    public float LengthScale;
    public bool PreserveVelocity;
    public bool DropToBottom;
    public float AutoDetachStart;
    public float AutoDetachEnd;
    public bool RestPoint;
    public bool PushOffInDirectionX;
    public bool IsMoving;
    public bool DetachEndOnSpawn;
    public bool DetachEndOnUse;
    public GameObject[] Panels; // How to convert each game objects ( model / position / angles )
    public float PanelTimerMin;
    public float PanelTimerMax;
    public int PanelMaxUse;
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
