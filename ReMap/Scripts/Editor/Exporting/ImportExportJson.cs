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

        // Sort by alphabetical name
        SortListByKey(myObject.Props, x => x.PathString);
        SortListByKey(myObject.JumpPads, x => x.PathString);
        SortListByKey(myObject.Buttons, x => x.PathString);
        SortListByKey(myObject.BubbleShields, x => x.PathString);
        SortListByKey(myObject.WeaponRacks, x => x.PathString);
        SortListByKey(myObject.LootBins, x => x.PathString);
        SortListByKey(myObject.ZipLines, x => x.PathString);
        SortListByKey(myObject.LinkedZipLines, x => x.PathString);
        SortListByKey(myObject.VerticalZipLines, x => x.PathString);
        SortListByKey(myObject.NonVerticalZipLines, x => x.PathString);
        SortListByKey(myObject.Doors, x => x.PathString);
        SortListByKey(myObject.Triggers, x => x.PathString);
        SortListByKey(myObject.Sounds, x => x.PathString);
        SortListByKey(myObject.TextInfoPanels, x => x.PathString);

        await ImportProps(myObject.Props);
        await ImportJumppads(myObject.JumpPads);
        await ImportButtons(myObject.Buttons);
        await ImportBubbleSheilds(myObject.BubbleShields);
        await ImportWeaponRacks(myObject.WeaponRacks);
        await ImportLootBins(myObject.LootBins);
        await ImportZipLines(myObject.ZipLines, myObject.LinkedZipLines, myObject.VerticalZipLines, myObject.NonVerticalZipLines);
        await ImportDoors(myObject.Doors);
        await ImportTriggers(myObject.Triggers);
        await ImportSounds(myObject.Sounds);
        await ImportTextInfoPanels(myObject.TextInfoPanels);

        ReMapConsole.Log("[Json Import] Finished", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ImportProps(List<PropsClass> Props)
    {
        int i = 0;
        int j = 1;
        int propCount = Props.Count;
        foreach(PropsClass prop in Props)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( prop.PathString ) )
            {
                importing = prop.Name;
            } else importing = $"{prop.PathString}/{prop.Name}";

            ReMapConsole.Log("[Json Import] Importing: " + prop.Name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing Props {j}/{propCount}", $"Importing: {importing}", (i + 1) / (float)propCount);
            i++; j++;

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(prop.Name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {prop.Name}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = prop.Position;
            obj.transform.eulerAngles = prop.Rotation;
            obj.name = prop.Name;
            obj.gameObject.transform.localScale = prop.Scale;

            PropScript script = obj.GetComponent<PropScript>();
            PropScriptClass propScript = prop.script;
            script.FadeDistance = propScript.FadeDistance;
            script.allowMantle = propScript.AllowMantle;
            script.realmID = propScript.RealmID;
            script.parameters = propScript.Parameters;
            script.customParameters = propScript.CustomParameters;

            if ( prop.PathString != "" )
            obj.gameObject.transform.parent = CreatePath( prop.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
        }
    }

    private static async Task ImportJumppads(List<JumpPadsClass> JumpPads)
    {
        int i = 0;
        int j = 1;
        int jumpPadsCount = JumpPads.Count;
        foreach(JumpPadsClass jumppad in JumpPads)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( jumppad.PathString ) )
            {
                importing = "custom_jumppad";
            } else importing = $"{jumppad.PathString}/custom_jumppad";

            ReMapConsole.Log("[Json Import] Importing: custom_jumppad", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing Jumppads {j}/{jumpPadsCount}", $"Importing: {importing}", (i + 1) / (float)jumpPadsCount);

            string Model = "custom_jumppad";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = jumppad.Position;
            obj.transform.eulerAngles = jumppad.Rotation;
            obj.name = Model;
            obj.gameObject.transform.localScale =  jumppad.Scale;

            PropScript script = obj.GetComponent<PropScript>();
            PropScriptClass propScript = jumppad.script;
            propScript.FadeDistance = script.FadeDistance;
            propScript.AllowMantle = script.allowMantle;
            propScript.RealmID = script.realmID;
            propScript.Parameters = script.parameters;
            propScript.CustomParameters = script.customParameters;

            if ( jumppad.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( jumppad.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ImportButtons(List<ButtonsClass> Buttons)
    {
        int i = 0;
        int j = 1;
        int buttonsCount = Buttons.Count;
        foreach(ButtonsClass button in Buttons)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( button.PathString ) )
            {
                importing = "custom_button";
            } else importing = $"{button.PathString}/custom_button";

            ReMapConsole.Log("[Json Import] Importing: custom_button", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing Buttons {j}/{buttonsCount}", $"Importing: {importing}", (i + 1) / (float)buttonsCount);

            string Model = "custom_button";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = button.Position;
            obj.transform.eulerAngles = button.Rotation;
            obj.name = Model;

            ButtonScripting script = obj.GetComponent<ButtonScripting>();
            script.OnUseCallback = button.OnUseCallback;
            script.UseText = button.UseText;

            if ( button.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( button.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ImportBubbleSheilds(List<BubbleShieldsClass> BSheilds)
    {
        int i = 0;
        int j = 1;
        int bSheildsCount = BSheilds.Count;
        foreach(BubbleShieldsClass sheild in BSheilds)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( sheild.PathString ) )
            {
                importing = "bubbleshield";
            } else importing = $"{sheild.PathString}/bubbleshield";

            ReMapConsole.Log("[Json Import] Importing: " + sheild.Model, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing BubbleShields {j}/{bSheildsCount}", $"Importing: {importing}", (i + 1) / (float)bSheildsCount);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(sheild.Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {sheild.Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = sheild.Position;
            obj.transform.eulerAngles = sheild.Rotation;
            obj.name = sheild.Model;
            obj.gameObject.transform.localScale = sheild.Scale;

            BubbleScript script = obj.GetComponent<BubbleScript>();
            string[] split = sheild.Color.Split(" ");
            script.ShieldColor.r = byte.Parse(split[0].Replace("\"", ""));
            script.ShieldColor.g = byte.Parse(split[1].Replace("\"", ""));
            script.ShieldColor.b = byte.Parse(split[2].Replace("\"", ""));

            if ( sheild.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( sheild.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ImportWeaponRacks(List<WeaponRacksClass> WeaponRacks)
    {
        int i = 0;
        int j = 1;
        int weaponRacksCount = WeaponRacks.Count;
        foreach(WeaponRacksClass weaponrack in WeaponRacks)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( weaponrack.PathString ) )
            {
                importing = $"custom_weaponrack {weaponrack.Weapon}";
            } else importing = $"{weaponrack.PathString}/custom_weaponrack {weaponrack.Weapon}";

            ReMapConsole.Log("[Json Import] Importing: " + weaponrack.Weapon, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing WeaponRacks {j}/{weaponRacksCount}", $"Importing: {importing}", (i + 1) / (float)weaponRacksCount);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(weaponrack.Weapon);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {weaponrack.Weapon}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = weaponrack.Position;
            obj.transform.eulerAngles = weaponrack.Rotation;
            obj.name = weaponrack.Weapon;

            WeaponRackScript script = obj.GetComponent<WeaponRackScript>();
            script.respawnTime = weaponrack.RespawnTime;

            if ( weaponrack.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( weaponrack.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ImportLootBins(List<LootBinsClass> LootBins)
    {
        int i = 0;
        int j = 1;
        int lootBinsCount = LootBins.Count;
        foreach(LootBinsClass lootbin in LootBins)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( lootbin.PathString ) )
            {
                importing = "custom_lootbin";
            } else importing = $"{lootbin.PathString}/custom_lootbin";
            
            ReMapConsole.Log("[Json Import] Importing: custom_lootbin", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing LootBins {j}/{lootBinsCount}", $"Importing: {importing}", (i + 1) / (float)lootBinsCount);

            string Model = "custom_lootbin";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = lootbin.Position;
            obj.transform.eulerAngles = lootbin.Rotation;
            obj.name = Model;

            LootBinScript script = obj.GetComponent<LootBinScript>();
            script.LootbinSkin = lootbin.Skin;

            if ( lootbin.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( lootbin.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ImportZipLines(List<ZipLinesClass> Ziplines, List<LinkedZipLinesClass> LinkedZiplines, List<VerticalZipLinesClass> VerticalZiplines, List<NonVerticalZipLinesClass> NonVerticalZiplines)
    {
        int i = 0;
        int j = 1;
        int ziplinesCount = Ziplines.Count;
        int linkedZiplinesCount = LinkedZiplines.Count;
        int verticalZiplinesCount = VerticalZiplines.Count;
        int nonVerticalZiplinesCount = NonVerticalZiplines.Count;

        foreach(ZipLinesClass zipline in Ziplines)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( zipline.PathString ) )
            {
                importing = "custom_zipline";
            } else importing = $"{zipline.PathString}/custom_zipline";
            
            ReMapConsole.Log("[Json Import] Importing: custom_zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing Ziplines {j}/{ziplinesCount}", $"Importing: {importing}", (i + 1) / (float)ziplinesCount);

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

            if ( zipline.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( zipline.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        foreach(LinkedZipLinesClass zipline in LinkedZiplines)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( zipline.PathString ) )
            {
                importing = "custom_linked_zipline";
            } else importing = $"{zipline.PathString}/custom_linked_zipline";
            
            ReMapConsole.Log("[Json Import] Importing: Linked Zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing LinkedZiplines {j}/{linkedZiplinesCount}", $"Importing: {importing}", (i + 1) / (float)linkedZiplinesCount);

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
            script.EnableSmoothing = zipline.IsSmoothed;
            script.SmoothType = zipline.SmoothType;
            script.SmoothAmount = zipline.SmoothAmount;

            if ( zipline.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( zipline.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        foreach(VerticalZipLinesClass zipline in VerticalZiplines)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( zipline.PathString ) )
            {
                importing = "vertical_zipline";
            } else importing = $"{zipline.PathString}/vertical_zipline";
            
            ReMapConsole.Log("[Json Import] Importing: Vertical Zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing VerticalZiplines {j}/{verticalZiplinesCount}", $"Importing: {importing}", (i + 1) / (float)verticalZiplinesCount);

            string Model = zipline.ZiplineType;
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;

            DrawVerticalZipline script = obj.GetComponent<DrawVerticalZipline>();

            obj.transform.position = zipline.ZiplinePosition;
            obj.transform.eulerAngles = zipline.ZiplineAngles;

            script.armOffset = zipline.ArmOffset;
            script.heightOffset = zipline.HeightOffset;
            script.anglesOffset = zipline.AnglesOffset;
            script.FadeDistance = zipline.FadeDistance;
            script.Scale = zipline.Scale;
            script.Width = zipline.Width;
            script.SpeedScale = zipline.SpeedScale;
            script.LengthScale = zipline.LengthScale;
            script.PreserveVelocity  = zipline.PreserveVelocity;
            script.DropToBottom = zipline.DropToBottom;
            script.AutoDetachStart = zipline.AutoDetachStart;
            script.AutoDetachEnd = zipline.AutoDetachEnd;
            script.RestPoint = zipline.RestPoint;
            script.PushOffInDirectionX = zipline.PushOffInDirectionX;
            script.IsMoving = zipline.IsMoving;
            script.DetachEndOnSpawn = zipline.DetachEndOnSpawn;
            script.DetachEndOnUse = zipline.DetachEndOnUse;

            foreach( VCPanelsClass panelInfo in zipline.Panels )
            {
                UnityEngine.Object loadedPrefabResourcePanel = FindPrefabFromName( "mdl#" + panelInfo.Model);
                if (loadedPrefabResourcePanel == null)
                {
                    ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {panelInfo.Model}" , ReMapConsole.LogType.Error);
                    continue;
                }

                GameObject panel = PrefabUtility.InstantiatePrefab(loadedPrefabResourcePanel as GameObject) as GameObject;
                panel.transform.position = panelInfo.Position;
                panel.transform.eulerAngles = panelInfo.Angles;
                panel.transform.parent = CreatePath( panelInfo.Path );
                Array.Resize( ref script.Panels, script.Panels.Length + 1 );
                script.Panels[script.Panels.Length - 1] = panel;
            }

            script.PanelTimerMin = zipline.PanelTimerMin;
            script.PanelTimerMax = zipline.PanelTimerMax;
            script.PanelMaxUse = zipline.PanelMaxUse;

            if ( zipline.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( zipline.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        foreach(NonVerticalZipLinesClass zipline in NonVerticalZiplines)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( zipline.PathString ) )
            {
                importing = "non_vertical_zipline";
            } else importing = $"{zipline.PathString}/non_vertical_zipline";
            
            ReMapConsole.Log("[Json Import] Importing: Non Vertical Zipline", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing NonVerticalZiplines {j}/{nonVerticalZiplinesCount}", $"Importing: {importing}", (i + 1) / (float)nonVerticalZiplinesCount);

            string Model = zipline.ZiplineType;
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;

            DrawNonVerticalZipline script = obj.GetComponent<DrawNonVerticalZipline>();

            obj.transform.position = zipline.ZiplineStartPosition;
            obj.transform.eulerAngles = zipline.ZiplineStartAngles;
            script.zipline.transform.Find("support_end").position = zipline.ZiplineEndPosition;
            script.zipline.transform.Find("support_end").eulerAngles = zipline.ZiplineEndAngles;
            script.ArmOffsetStart = zipline.ArmStartOffset;
            script.ArmOffsetEnd = zipline.ArmEndOffset;
            script.FadeDistance = zipline.FadeDistance;
            script.Scale = zipline.Scale;
            script.Width = zipline.Width;
            script.SpeedScale = zipline.SpeedScale;
            script.LengthScale = zipline.LengthScale;
            script.PreserveVelocity  = zipline.PreserveVelocity;
            script.DropToBottom = zipline.DropToBottom;
            script.AutoDetachStart = zipline.AutoDetachStart;
            script.AutoDetachEnd = zipline.AutoDetachEnd;
            script.RestPoint = zipline.RestPoint;
            script.PushOffInDirectionX = zipline.PushOffInDirectionX;
            script.IsMoving = zipline.IsMoving;
            script.DetachEndOnSpawn = zipline.DetachEndOnSpawn;
            script.DetachEndOnUse = zipline.DetachEndOnUse;

            foreach( VCPanelsClass panelInfo in zipline.Panels )
            {
                UnityEngine.Object loadedPrefabResourcePanel = FindPrefabFromName( "mdl#" + panelInfo.Model);
                if (loadedPrefabResourcePanel == null)
                {
                    ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {panelInfo.Model}" , ReMapConsole.LogType.Error);
                    continue;
                }

                GameObject panel = PrefabUtility.InstantiatePrefab(loadedPrefabResourcePanel as GameObject) as GameObject;
                panel.transform.position = panelInfo.Position;
                panel.transform.eulerAngles = panelInfo.Angles;
                panel.transform.parent = CreatePath( panelInfo.Path );
                Array.Resize( ref script.Panels, script.Panels.Length + 1 );
                script.Panels[script.Panels.Length - 1] = panel;
            }

            script.PanelTimerMin = zipline.PanelTimerMin;
            script.PanelTimerMax = zipline.PanelTimerMax;
            script.PanelMaxUse = zipline.PanelMaxUse;

            if ( zipline.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( zipline.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ImportDoors(List<DoorsClass> Doors)
    {
        int i = 0;
        int j = 1;
        int doorsCount = Doors.Count;
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

            string importing = "";

            if ( string.IsNullOrEmpty( door.PathString ) )
            {
                importing = Model;
            } else importing = $"{door.PathString}/{Model}";

            ReMapConsole.Log("[Json Import] Importing: " + Model, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing Doors {j}/{doorsCount}", $"Importing: {importing}", (i + 1) / (float)doorsCount);

            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = door.Position;
            obj.transform.eulerAngles = door.Rotation;
            obj.name = Model;

            if(IsSingleOrDouble)
            {
                DoorScript script = obj.GetComponent<DoorScript>();
                script.GoldDoor = door.Gold;
            }

            if ( door.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( door.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ImportTriggers(List<TriggersClass> Triggers)
    {
        int i = 0;
        int j = 1;
        int triggersCount = Triggers.Count;
        foreach(TriggersClass trigger in Triggers)
        {
            string importing = "";

            if ( string.IsNullOrEmpty( trigger.PathString ) )
            {
                importing = "trigger_cylinder";
            } else importing = $"{trigger.PathString}/trigger_cylinder";

            ReMapConsole.Log("[Json Import] Importing: Trigger", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing Triggers {j}/{triggersCount}", $"Importing: {importing}", (i + 1) / (float)triggersCount);

            string Model = "trigger_cylinder";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = trigger.Position;
            obj.transform.eulerAngles = trigger.Rotation;
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3(trigger.Radius, trigger.Height, trigger.Radius);

            TriggerScripting script = obj.GetComponent<TriggerScripting>();
            script.Debug = trigger.Debug;
            script.EnterCallback = trigger.EnterCallback;
            script.LeaveCallback = trigger.ExitCallback;

            if ( trigger.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( trigger.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ImportSounds(List<SoundClass> Sounds)
    {
        int i = 0;
        int j = 1;
        int soundsCount = Sounds.Count;
        foreach(SoundClass sound in Sounds)
        {
            
            string importing = "";

            if ( string.IsNullOrEmpty( sound.PathString ) )
            {
                importing = "custom_sound";
            } else importing = $"{sound.PathString}/custom_sound";

            ReMapConsole.Log("[Json Import] Importing: Sound", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing Sounds {j}/{soundsCount}", $"Importing: {importing}", (i + 1) / (float)soundsCount);

            string Model = "custom_sound";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = sound.Position;

            SoundScript script = obj.GetComponent<SoundScript>();
            script.Radius = sound.Radius;
            script.IsWaveAmbient = sound.IsWaveAmbient;
            script.Enable = sound.Enable;
            script.SoundName = sound.SoundName;

            int k = 0;
            foreach ( Vector3 polylineSegments in sound.PolylineSegments )
            {
                Array.Resize( ref script.PolylineSegment, script.PolylineSegment.Length + 1 );
                script.PolylineSegment[k++] = polylineSegments;
            }

            if ( sound.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( sound.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }
    
    private static async Task ImportTextInfoPanels(List<TextInfoPanelClass> TextInfoPanel)
    {
        int i = 0;
        int j = 1;
        int textInfoPanelCount = TextInfoPanel.Count;
        foreach(TextInfoPanelClass textInfoPanel in TextInfoPanel)
        {
            
            string importing = "";

            if ( string.IsNullOrEmpty( textInfoPanel.PathString ) )
            {
                importing = "custom_TextInfoPanel";
            } else importing = $"{textInfoPanel.PathString}/custom_TextInfoPanel";

            ReMapConsole.Log("[Json Import] Importing: Sound", ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Importing Sounds {j}/{textInfoPanelCount}", $"Importing: {importing}", (i + 1) / (float)textInfoPanelCount);

            string Model = "custom_TextInfoPanel";
            UnityEngine.Object loadedPrefabResource = FindPrefabFromName(Model);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {Model}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = textInfoPanel.Position;
            obj.transform.eulerAngles = textInfoPanel.Rotation;

            TextInfoPanelScript script = obj.GetComponent<TextInfoPanelScript>();
            script.title = textInfoPanel.Title;
            script.description = textInfoPanel.Description;
            script.showPIN = textInfoPanel.ShowPIN;
            script.Scale = textInfoPanel.Scale;

            if ( textInfoPanel.Path.Count != 0 )
            obj.gameObject.transform.parent = CreatePath( textInfoPanel.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
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
        await ExportSounds();
        await ExportTextInfoPanels();

        ReMapConsole.Log("[Json Export] Writing to file: " + path, ReMapConsole.LogType.Warning);
        string json = JsonUtility.ToJson(save);
        System.IO.File.WriteAllText(path, json);

        ReMapConsole.Log("[Json Export] Finished.", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ExportProps()
    {
        int i = 0;
        int j = 1;
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        int propObjectsCount = PropObjects.Length;
        foreach(GameObject obj in PropObjects)
        {
            PropScript script = obj.GetComponent<PropScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing PropScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Props {j}/{propObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)propObjectsCount);
            
            PropsClass prop = new PropsClass();
            PropScriptClass propScript = new PropScriptClass();
            prop.Name = obj.name.Split(char.Parse(" "))[0];
            prop.Position = obj.transform.position;
            prop.Rotation = obj.transform.rotation.eulerAngles;
            prop.Scale = obj.transform.localScale;
            propScript.AllowMantle = script.allowMantle;
            propScript.FadeDistance = script.FadeDistance;
            propScript.RealmID = script.realmID;
            propScript.Parameters = script.parameters;
            propScript.CustomParameters = script.customParameters;
            prop.script = propScript;

            prop.Path = FindPath( obj );
            prop.PathString = path;

            if ( IsValidPath( prop.PathString ) ) save.Props.Add(prop);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportJumppads()
    {
        int i = 0;
        int j = 1;
        GameObject[] JumpPadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        int jumpPadObjectsCount = JumpPadObjects.Length;
        foreach (GameObject obj in JumpPadObjects)
        {
            PropScript script = obj.GetComponent<PropScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing PropScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Jumppads {j}/{jumpPadObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)jumpPadObjectsCount);

            JumpPadsClass jumpPad = new JumpPadsClass();
            PropScriptClass propScript = new PropScriptClass();
            jumpPad.Position = obj.transform.position;
            jumpPad.Rotation = obj.transform.rotation.eulerAngles;
            jumpPad.Scale = obj.transform.localScale;
            propScript.AllowMantle = script.allowMantle;
            propScript.FadeDistance = script.FadeDistance;
            propScript.RealmID = script.realmID;
            propScript.Parameters = script.parameters;
            propScript.CustomParameters = script.customParameters;
            jumpPad.script = propScript;

            jumpPad.Path = FindPath( obj );
            jumpPad.PathString = path;

            save.JumpPads.Add(jumpPad);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportButtons()
    {
        int i = 0;
        int j = 1;
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        int buttonObjectsCount = ButtonObjects.Length;
        foreach (GameObject obj in ButtonObjects)
        {
            ButtonScripting script = obj.GetComponent<ButtonScripting>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing ButtonScripting on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }
            
            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Buttons {j}/{buttonObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)buttonObjectsCount);

            ButtonsClass button = new ButtonsClass();
            button.Position = obj.transform.position;
            button.Rotation = obj.transform.rotation.eulerAngles;
            button.UseText = script.UseText;
            button.OnUseCallback = script.OnUseCallback;

            button.Path = FindPath( obj );
            button.PathString = path;

            save.Buttons.Add(button);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportBubbleSheilds()
    {
        int i = 0;
        int j = 1;
        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        int bubbleShieldObjectsCount = BubbleShieldObjects.Length;
        foreach (GameObject obj in BubbleShieldObjects)
        {
            BubbleScript script = obj.GetComponent<BubbleScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing BubbleScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting BubbleShields {j}/{bubbleShieldObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)bubbleShieldObjectsCount);

            BubbleShieldsClass bubbleShield = new BubbleShieldsClass();
            bubbleShield.Position = obj.transform.position;
            bubbleShield.Rotation = obj.transform.rotation.eulerAngles;
            bubbleShield.Scale = obj.transform.localScale;
            bubbleShield.Color = script.ShieldColor.r + " " + script.ShieldColor.g + " " + script.ShieldColor.b;
            bubbleShield.Model = obj.name.Split(char.Parse(" "))[0];

            bubbleShield.Path = FindPath( obj );
            bubbleShield.PathString = path;

            save.BubbleShields.Add(bubbleShield);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportWeaponRacks()
    {
        int i = 0;
        int j = 1;
        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        int weaponRackObjectsCount = WeaponRackObjects.Length;
        foreach (GameObject obj in WeaponRackObjects)
        {
            WeaponRackScript script = obj.GetComponent<WeaponRackScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing WeaponRackScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }
            
            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting WeaponRacks {j}/{weaponRackObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)weaponRackObjectsCount);

            WeaponRacksClass weaponRack = new WeaponRacksClass();
            weaponRack.Position = obj.transform.position;
            weaponRack.Rotation = obj.transform.rotation.eulerAngles;
            weaponRack.Weapon = obj.name.Split(char.Parse(" "))[0];
            weaponRack.RespawnTime = script.respawnTime;

            weaponRack.Path = FindPath( obj );
            weaponRack.PathString = path;

            save.WeaponRacks.Add(weaponRack);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportLootBins()
    {
        int i = 0;
        int j = 1;
        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        int lootBinObjectsCount = LootBinObjects.Length;
        foreach (GameObject obj in LootBinObjects)
        {
            LootBinScript script = obj.GetComponent<LootBinScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing LootBinScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting LootBins {j}/{lootBinObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)lootBinObjectsCount);

            LootBinsClass lootBin = new LootBinsClass();
            lootBin.Position = obj.transform.position;
            lootBin.Rotation = obj.transform.rotation.eulerAngles;
            lootBin.Skin = script.LootbinSkin;

            lootBin.Path = FindPath( obj );
            lootBin.PathString = path;

            save.LootBins.Add(lootBin);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportZipLines()
    {
        int i = 0;
        int j = 1;
        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        int zipLineObjectsCount = ZipLineObjects.Length;
        foreach (GameObject obj in ZipLineObjects)
        {
            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Ziplines {j}/{zipLineObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)zipLineObjectsCount);

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

            zipLine.Path = FindPath( obj );
            zipLine.PathString = path;

            save.ZipLines.Add(zipLine);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        int linkedZipLineObjectsCount = LinkedZipLineObjects.Length;
        foreach (GameObject obj in LinkedZipLineObjects)
        {
            LinkedZiplineScript script = obj.GetComponent<LinkedZiplineScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing LinkedZiplineScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Linked Ziplines {j}/{linkedZipLineObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)linkedZipLineObjectsCount);

            List<Vector3> nodes = new List<Vector3>();
            LinkedZipLinesClass linkedZipLine = new LinkedZipLinesClass();
            foreach (Transform child in obj.transform)
                nodes.Add(child.gameObject.transform.position);

            linkedZipLine.Nodes = nodes;
            linkedZipLine.IsSmoothed = script.EnableSmoothing;
            linkedZipLine.SmoothType = script.SmoothType;
            linkedZipLine.SmoothAmount = script.SmoothAmount;

            linkedZipLine.Path = FindPath( obj );
            linkedZipLine.PathString = path;

            save.LinkedZipLines.Add(linkedZipLine);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        GameObject[] VerticalZipLineObjects = GameObject.FindGameObjectsWithTag("VerticalZipLine");
        int verticalZipLineObjectsCount = VerticalZipLineObjects.Length;
        foreach (GameObject obj in VerticalZipLineObjects)
        {
            DrawVerticalZipline script = obj.GetComponent<DrawVerticalZipline>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing DrawVerticalZipline on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Vertical Ziplines {j}/{verticalZipLineObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)verticalZipLineObjectsCount);

            VerticalZipLinesClass verticalZipLine = new VerticalZipLinesClass();

            verticalZipLine.ZiplineType = obj.name;
            verticalZipLine.ZiplinePosition = script.zipline.position;
            verticalZipLine.ZiplineAngles = script.zipline.eulerAngles;
            verticalZipLine.ArmOffset = script.armOffset;
            verticalZipLine.HeightOffset = script.heightOffset;
            verticalZipLine.AnglesOffset = script.anglesOffset;
            verticalZipLine.FadeDistance = script.FadeDistance;
            verticalZipLine.Scale = script.Scale;
            verticalZipLine.Width = script.Width;
            verticalZipLine.SpeedScale = script.SpeedScale;
            verticalZipLine.LengthScale = script.LengthScale;
            verticalZipLine.PreserveVelocity = script.PreserveVelocity ;
            verticalZipLine.DropToBottom = script.DropToBottom;
            verticalZipLine.AutoDetachStart = script.AutoDetachStart;
            verticalZipLine.AutoDetachEnd = script.AutoDetachEnd;
            verticalZipLine.RestPoint = script.RestPoint;
            verticalZipLine.PushOffInDirectionX = script.PushOffInDirectionX;
            verticalZipLine.IsMoving = script.IsMoving;
            verticalZipLine.DetachEndOnSpawn = script.DetachEndOnSpawn;
            verticalZipLine.DetachEndOnUse = script.DetachEndOnUse;

            List<VCPanelsClass> panels = new List<VCPanelsClass>();
            foreach (GameObject panel in script.Panels)
            {
                VCPanelsClass panelClass = new VCPanelsClass();
                panelClass.Model = panel.name;
                panelClass.Position = panel.transform.position;
                panelClass.Angles = panel.transform.eulerAngles;
                panelClass.Path = FindPath( panel );
                panelClass.PathString = FindPathString( obj );
                panels.Add(panelClass);
            }

            verticalZipLine.Panels = panels.ToArray();

            verticalZipLine.PanelTimerMin = script.PanelTimerMin;
            verticalZipLine.PanelTimerMax = script.PanelTimerMax;
            verticalZipLine.PanelMaxUse = script.PanelMaxUse;

            verticalZipLine.Path = FindPath( obj );
            verticalZipLine.PathString = path;

            save.VerticalZipLines.Add(verticalZipLine);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        GameObject[] NonVerticalZipLineObjects = GameObject.FindGameObjectsWithTag("NonVerticalZipLine");
        int nonVerticalZipLineObjectsCount = NonVerticalZipLineObjects.Length;
        foreach (GameObject obj in NonVerticalZipLineObjects)
        {
            DrawNonVerticalZipline script = obj.GetComponent<DrawNonVerticalZipline>();
            if (script == null) {
                ReMapConsole.Log("[Map Export] Missing DrawNonVerticalZipline on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Non Vertical Ziplines {j}/{nonVerticalZipLineObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)nonVerticalZipLineObjectsCount);

            NonVerticalZipLinesClass nonVerticalZipLine = new NonVerticalZipLinesClass();

            nonVerticalZipLine.ZiplineType = obj.name;
            nonVerticalZipLine.ZiplineStartPosition = script.zipline.transform.Find("support_start").position;
            nonVerticalZipLine.ZiplineStartAngles = script.zipline.transform.Find("support_start").eulerAngles;
            nonVerticalZipLine.ZiplineEndPosition = script.zipline.transform.Find("support_end").position;
            nonVerticalZipLine.ZiplineEndAngles = script.zipline.transform.Find("support_end").eulerAngles;
            nonVerticalZipLine.ArmStartOffset = script.ArmOffsetStart;
            nonVerticalZipLine.ArmEndOffset = script.ArmOffsetEnd;
            nonVerticalZipLine.FadeDistance = script.FadeDistance;
            nonVerticalZipLine.Scale = script.Scale;
            nonVerticalZipLine.Width = script.Width;
            nonVerticalZipLine.SpeedScale = script.SpeedScale;
            nonVerticalZipLine.LengthScale = script.LengthScale;
            nonVerticalZipLine.PreserveVelocity = script.PreserveVelocity ;
            nonVerticalZipLine.DropToBottom = script.DropToBottom;
            nonVerticalZipLine.AutoDetachStart = script.AutoDetachStart;
            nonVerticalZipLine.AutoDetachEnd = script.AutoDetachEnd;
            nonVerticalZipLine.RestPoint = script.RestPoint;
            nonVerticalZipLine.PushOffInDirectionX = script.PushOffInDirectionX;
            nonVerticalZipLine.IsMoving = script.IsMoving;
            nonVerticalZipLine.DetachEndOnSpawn = script.DetachEndOnSpawn;
            nonVerticalZipLine.DetachEndOnUse = script.DetachEndOnUse;

            List<VCPanelsClass> panels = new List<VCPanelsClass>();
            foreach (GameObject panel in script.Panels)
            {
                VCPanelsClass panelClass = new VCPanelsClass();
                panelClass.Model = panel.name;
                panelClass.Position = panel.transform.position;
                panelClass.Angles = panel.transform.eulerAngles;
                panelClass.Path = FindPath( panel );
                panelClass.PathString = FindPathString( obj );
                panels.Add(panelClass);
            }

            nonVerticalZipLine.Panels = panels.ToArray();

            nonVerticalZipLine.PanelTimerMin = script.PanelTimerMin;
            nonVerticalZipLine.PanelTimerMax = script.PanelTimerMax;
            nonVerticalZipLine.PanelMaxUse = script.PanelMaxUse;

            nonVerticalZipLine.Path = FindPath( obj );
            nonVerticalZipLine.PathString = path;

            save.NonVerticalZipLines.Add(nonVerticalZipLine);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportDoors()
    {
        int i = 0;
        int j = 1;
        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        int singleDoorObjectsCount = SingleDoorObjects.Length;
        foreach (GameObject obj in SingleDoorObjects)
        {
            DoorScript script = obj.GetComponent<DoorScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing DoorScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Single Doors {j}/{singleDoorObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)singleDoorObjectsCount);

            DoorsClass singleDoor = new DoorsClass();
            singleDoor.Position = obj.transform.position;
            singleDoor.Rotation = obj.transform.rotation.eulerAngles;
            singleDoor.Type = "eMapEditorDoorType.Single";
            singleDoor.Gold = script.GoldDoor;

            singleDoor.Path = FindPath( obj );
            singleDoor.PathString = path;

            save.Doors.Add(singleDoor);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        int doubleDoorObjectsCount = DoubleDoorObjects.Length;
        foreach (GameObject obj in DoubleDoorObjects)
        {
            DoorScript script = obj.GetComponent<DoorScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing DoorScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }
            
            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Double Doors {j}/{doubleDoorObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)doubleDoorObjectsCount);

            DoorsClass doubleDoor = new DoorsClass();
            doubleDoor.Position = obj.transform.position;
            doubleDoor.Rotation = obj.transform.rotation.eulerAngles;
            doubleDoor.Type = "eMapEditorDoorType.Double";
            doubleDoor.Gold = script.GoldDoor;

            doubleDoor.Path = FindPath( obj );
            doubleDoor.PathString = path;

            save.Doors.Add(doubleDoor);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        int vertDoorObjectsCount = VertDoorObjects.Length;
        foreach (GameObject obj in VertDoorObjects)
        {              
            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Vertical Doors {j}/{vertDoorObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)vertDoorObjectsCount);

            DoorsClass vertDoor = new DoorsClass();
            vertDoor.Position = obj.transform.position;
            vertDoor.Rotation = obj.transform.rotation.eulerAngles;
            vertDoor.Type = "eMapEditorDoorType.Vertical";
            vertDoor.Gold = false;

            vertDoor.Path = FindPath( obj );
            vertDoor.PathString = path;

            save.Doors.Add(vertDoor);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }

        i = 0; j = 1;
        GameObject[] HorDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        int horDoorObjectsCount = HorDoorObjects.Length;
        foreach (GameObject obj in HorDoorObjects)
        {
            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Doors {j}/{horDoorObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)horDoorObjectsCount);

            DoorsClass horDoor = new DoorsClass();
            horDoor.Position = obj.transform.position;
            horDoor.Rotation = obj.transform.rotation.eulerAngles;
            horDoor.Type = "eMapEditorDoorType.Horizontal";
            horDoor.Gold = false;

            horDoor.Path = FindPath( obj );
            horDoor.PathString = path;

            save.Doors.Add(horDoor);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportTriggers()
    {
        int i = 0;
        int j = 1;
        GameObject[] TriggersObjects = GameObject.FindGameObjectsWithTag("Trigger");
        int triggersObjectsCount = TriggersObjects.Length;
        foreach (GameObject obj in TriggersObjects)
        {
            TriggerScripting script = obj.GetComponent<TriggerScripting>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing TriggerScripting on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }

            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Triggers {j}/{triggersObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)triggersObjectsCount);

            TriggersClass trigger = new TriggersClass();
            trigger.Position = obj.transform.position;
            trigger.Rotation = obj.transform.rotation.eulerAngles;
            trigger.Radius = obj.transform.localScale.x;
            trigger.Height = obj.transform.localScale.y;
            trigger.EnterCallback = script.EnterCallback;
            trigger.ExitCallback = script.LeaveCallback;
            trigger.Debug = script.Debug;

            trigger.Path = FindPath( obj );
            trigger.PathString = path;

            save.Triggers.Add(trigger);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportSounds()
    {
        int i = 0;
        int j = 1;
        GameObject[] SoundsObjects = GameObject.FindGameObjectsWithTag("Sound");
        int soundsObjectsCount = SoundsObjects.Length;
        foreach (GameObject obj in SoundsObjects)
        {
            SoundScript script = obj.GetComponent<SoundScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing SoundScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }
            
            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting Sounds {j}/{soundsObjectsCount}", $"Exporting: {exporting}", (i + 1) / (float)soundsObjectsCount);

            SoundClass sound = new SoundClass();
            sound.Position = script.soundModel.transform.position;
            sound.Radius = script.Radius;
            sound.IsWaveAmbient = script.IsWaveAmbient;
            sound.Enable = script.Enable;
            sound.SoundName = script.SoundName;

            List<Vector3> PolylineSegment = new List<Vector3>();
            foreach ( Vector3 polylineSegments in script.PolylineSegment )
            {
                PolylineSegment.Add( polylineSegments );
            }
            sound.PolylineSegments = PolylineSegment;

            sound.Path = FindPath( obj );
            sound.PathString = path;

            save.Sounds.Add(sound);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }

    private static async Task ExportTextInfoPanels()
    {
        int i = 0;
        int j = 1;
        GameObject[] TextInfoPanelsObjects = GameObject.FindGameObjectsWithTag("TextInfoPanel");
        int textInfoPanelsCount = TextInfoPanelsObjects.Length;
        foreach (GameObject obj in TextInfoPanelsObjects)
        {
            TextInfoPanelScript script = obj.GetComponent<TextInfoPanelScript>();
            if (script == null) {
                ReMapConsole.Log("[Json Export] Missing TextInfoPanelsScript on: " + obj.name, ReMapConsole.LogType.Error);
                continue;
            }
            
            string exporting = "";
            string path = FindPathString( obj );

            if ( string.IsNullOrEmpty( path ) )
            {
                exporting = obj.name;
            } else exporting = $"{path}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting TextInfoPanels {j}/{textInfoPanelsCount}", $"Exporting: {exporting}", (i + 1) / (float)textInfoPanelsCount);

            TextInfoPanelClass textInfoPanel = new TextInfoPanelClass();
            textInfoPanel.Position = obj.transform.position;
            textInfoPanel.Rotation = obj.transform.eulerAngles;
            textInfoPanel.Title = script.title;
            textInfoPanel.Description = script.description;
            textInfoPanel.ShowPIN = script.showPIN;
            textInfoPanel.Scale = script.Scale;

            textInfoPanel.Path = FindPath( obj );
            textInfoPanel.PathString = path;

            save.TextInfoPanels.Add( textInfoPanel );

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++; j++;
        }
    }


    [MenuItem("ReMap/Map Fix/ReImport Map", false, 51)]
    public static async void ReImportJson()
    {
        var path = EditorUtility.SaveFilePanel("Json Export", "", "mapexport.json", "json");

        if (path.Length == 0)
            return;

        // Export
        Helper.FixPropTags();

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
        await ExportSounds();

        ReMapConsole.Log("[Json Export] Writing to file: " + path, ReMapConsole.LogType.Warning);
        string jsonExport = JsonUtility.ToJson(save);
        System.IO.File.WriteAllText(path, jsonExport);

        ReMapConsole.Log("[Json Export] Finished.", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();

        // Delete All Prefab
        GameObject[][] Objects = new GameObject[16][];

        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        GameObject[] JumpPadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        GameObject[] VerticalZipLineObjects = GameObject.FindGameObjectsWithTag("VerticalZipLine");
        GameObject[] NonVerticalZipLineObjects = GameObject.FindGameObjectsWithTag("NonVerticalZipLine");
        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        GameObject[] HorDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        GameObject[] TriggersObjects = GameObject.FindGameObjectsWithTag("Trigger");
        GameObject[] SoundsObjects = GameObject.FindGameObjectsWithTag("Sound");

        Objects[0] = PropObjects;
        Objects[1] = JumpPadObjects;
        Objects[2] = ButtonObjects;
        Objects[3] = BubbleShieldObjects;
        Objects[4] = WeaponRackObjects;
        Objects[5] = LootBinObjects;
        Objects[6] = ZipLineObjects;
        Objects[7] = LinkedZipLineObjects;
        Objects[8] = VerticalZipLineObjects;
        Objects[9] = NonVerticalZipLineObjects;
        Objects[10] = SingleDoorObjects;
        Objects[11] = DoubleDoorObjects;
        Objects[12] = VertDoorObjects;
        Objects[13] = HorDoorObjects;
        Objects[14] = TriggersObjects;
        Objects[15] = SoundsObjects;


        foreach (GameObject[] goArray in Objects )
        {
            if ( goArray.Length != 0 )
            {
                for (int i = 0; i < goArray.Length; i++)
                    GameObject.DestroyImmediate(goArray[i]);
            }
        }


        // Import
        ReMapConsole.Log("[Json Import] Reading file: " + path, ReMapConsole.LogType.Warning);
        EditorUtility.DisplayProgressBar("Starting Import", "Reading File" , 0);
        string jsonImport = System.IO.File.ReadAllText(path);
        SaveJson myObject = JsonUtility.FromJson<SaveJson>(jsonImport);

        // Sort by alphabetical name
            SortListByKey(myObject.Props, x => x.PathString);
            SortListByKey(myObject.JumpPads, x => x.PathString);
            SortListByKey(myObject.Buttons, x => x.PathString);
            SortListByKey(myObject.BubbleShields, x => x.PathString);
            SortListByKey(myObject.WeaponRacks, x => x.PathString);
            SortListByKey(myObject.LootBins, x => x.PathString);
            SortListByKey(myObject.ZipLines, x => x.PathString);
            SortListByKey(myObject.LinkedZipLines, x => x.PathString);
            SortListByKey(myObject.VerticalZipLines, x => x.PathString);
            SortListByKey(myObject.NonVerticalZipLines, x => x.PathString);
            SortListByKey(myObject.Doors, x => x.PathString);
            SortListByKey(myObject.Triggers, x => x.PathString);
            SortListByKey(myObject.Sounds, x => x.PathString);
        //

        await ImportProps(myObject.Props);
        await ImportJumppads(myObject.JumpPads);
        await ImportButtons(myObject.Buttons);
        await ImportBubbleSheilds(myObject.BubbleShields);
        await ImportWeaponRacks(myObject.WeaponRacks);
        await ImportLootBins(myObject.LootBins);
        await ImportZipLines(myObject.ZipLines, myObject.LinkedZipLines, myObject.VerticalZipLines, myObject.NonVerticalZipLines);
        await ImportDoors(myObject.Doors);
        await ImportTriggers(myObject.Triggers);
        await ImportSounds(myObject.Sounds);

        ReMapConsole.Log("[Json Import] Finished", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();
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
        save.NonVerticalZipLines = new List<NonVerticalZipLinesClass>();
        save.Doors = new List<DoorsClass>();
        save.Triggers = new List<TriggersClass>();
        save.Sounds = new List<SoundClass>();
        save.TextInfoPanels = new List<TextInfoPanelClass>();
    }

    public static UnityEngine.Object FindPrefabFromName(string name)
    {
        // Hack so that the models named at the end with "(number)" still work
        if(name.Contains(" "))
            name = name.Split(" ")[0];

        //Find Model GUID in Assets
        string[] results = AssetDatabase.FindAssets(name);
        if (results.Length == 0)
            return null;

        //Get model path from guid and load it
        UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
        return loadedPrefabResource;
    }

    private static List<PathClass> FindPath( GameObject obj )
    {
        List<GameObject> parents = new List<GameObject>();
        List<PathClass> pathList = new List<PathClass>();
        GameObject currentParent = obj;

        // find all parented game objects
        while (currentParent.transform.parent != null)
        {
            if ( currentParent != obj ) parents.Add(currentParent);
            currentParent = currentParent.transform.parent.gameObject;
        }

        if ( currentParent != obj ) parents.Add(currentParent);

        foreach (GameObject parent in parents)
        {
            PathClass path = new PathClass();
            path.FolderName = parent.name;
            path.Position = parent.transform.position;
            path.Rotation = parent.transform.eulerAngles;

            pathList.Add( path );
        }

        pathList.Reverse();

        return pathList;
    }

    private static string FindPathString( GameObject obj )
    {
        List<GameObject> parents = new List<GameObject>();
        string pathString = "";
        GameObject currentParent = obj;

        // find all parented game objects
        while (currentParent.transform.parent != null)
        {
            if ( currentParent != obj ) parents.Add(currentParent);
            currentParent = currentParent.transform.parent.gameObject;
        }

        if ( currentParent != obj ) parents.Add(currentParent);

        parents.Reverse();

        foreach (GameObject parent in parents)
        {
            if ( string.IsNullOrEmpty( pathString ) )
            {
                pathString = $"{parent.name}";
            }
            else pathString = $"{pathString}/{parent.name}";
        }

        return pathString;
    }

    private static Transform CreatePath( List<PathClass> pathList )
    {
        GameObject folder = null; string path = "";

        foreach ( PathClass pathClass in pathList )
        {
            if ( string.IsNullOrEmpty( path ) )
            {
                path = $"{pathClass.FolderName}";
            }
            else path = $"{path}/{pathClass.FolderName}";

            GameObject newFolder = GameObject.Find( path );

            if ( newFolder == null ) newFolder = new GameObject( pathClass.FolderName );

            newFolder.transform.position = pathClass.Position;
            newFolder.transform.eulerAngles = pathClass.Rotation;

            if ( folder != null ) newFolder.transform.SetParent(folder.transform);

            folder = newFolder;
        }

        return folder.transform;
    }

    private static bool IsValidPath( string path )
    {
        foreach ( string protectedModel in protectedModels )
        {
            if ( path.Contains( protectedModel ) )
                return false;
        }

        return true;
    }

    public static void SortListByKey<T, TKey>(List<T> list, Func<T, TKey> keySelector) where TKey : IComparable
    {
        list.Sort((x, y) => keySelector(x).CompareTo(keySelector(y)));
    }
}
