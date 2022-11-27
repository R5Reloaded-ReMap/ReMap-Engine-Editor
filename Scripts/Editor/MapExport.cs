using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class MapExport
{
    [MenuItem("R5Reloaded/Text File/Export With Origin Offset/Whole Script", false, 100)]
    private static void ExportMapScript()
    {
        ExportMap(false, true);
    }

    [MenuItem("R5Reloaded/Text File/Export With Origin Offset/Map Only", false, 100)]
    private static void ExportOnlyMap()
    {
        ExportMap(true, true);
    }

    [MenuItem("R5Reloaded/Text File/Export Without Origin Offset/Whole Script", false, 100)]
    private static void ExportMapScript2()
    {
        ExportMap(false, false);
    }

    [MenuItem("R5Reloaded/Text File/Export Without Origin Offset/Map Only", false, 100)]
    private static void ExportOnlyMap2()
    {
        ExportMap(true, false);
    }

    [MenuItem("R5Reloaded/Text File/Export script.ent Code", false, 100)]
    private static void ExportScriptEntCode()
    {
        ExportScriptEnt();
    }

    static bool UseStartingOffset = false;

    static string StartFunction = "";

    /// <summary>
    /// Exports script ent code
    /// </summary>
    static void ExportScriptEnt()
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        var path = EditorUtility.SaveFilePanel( "Map Export", "", "scriptentexport.txt", "txt");
        if (path.Length != 0)
        {
            string saved = "";

            //Generate All Props
            GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
            foreach(GameObject go in PropObjects) {
                if (!go.activeInHierarchy)
                    continue;
                    
                saved += Helper.BuildScriptEnt(go);
            }
            
            System.IO.File.WriteAllText(path, saved);
        }
        else
        {
            Debug.Log("Failed: Didnt pick a save path!");
        }
    }


    /// <summary>
    /// Exports map code
    /// </summary>
    /// <param name="onlymap"></param>
    /// <param name="offset"></param>
    private static void ExportMap(bool onlymap, bool offset = false)
    {
        UseStartingOffset = offset;

        BuildStartingString();
        Helper.FixPropTags();

        EditorSceneManager.SaveOpenScenes();

        var path = EditorUtility.SaveFilePanel( "Map Export", "", "mapexport.txt", "txt");
        if (path.Length != 0)
        {
            string saved = "";

            if(!onlymap)
                saved = Helper.Credits + "\n" + StartFunction +  Helper.ShouldAddStartingOrg(UseStartingOffset, 1);
            else
                saved = Helper.ShouldAddStartingOrg(UseStartingOffset, 1);

            //Generate All Buttons
            GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");

            if(ButtonObjects.Length > 0)
                saved += "    //Buttons" + "\n";

            foreach(GameObject go in ButtonObjects) {
                ButtonScripting script = go.GetComponent<ButtonScripting>();
                saved += "    AddCallback_OnUseEntity( CreateFRButton(" + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", \"" + script.UseText + "\"), void function(entity panel, entity user, int input)" + "\n" + "    {" + "\n" + script.OnUseCallback + "\n" + "    })" + "\n";
            }

            if(ButtonObjects.Length > 0)
                saved += "\n";
            //End of Buttons

            //Generate All JumpPads
            GameObject[] JumppadObjects = GameObject.FindGameObjectsWithTag("Jumppad");

            if(JumppadObjects.Length > 0)
                saved += "    //Jumppads" + "\n";

            foreach(GameObject go in JumppadObjects) {
                PropScript script = go.GetComponent<PropScript>();
                saved += "    JumpPad_CreatedCallback( MapEditor_CreateProp( $\"mdl/props/octane_jump_pad/octane_jump_pad.rmdl\" , " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + script.allowMantle.ToString().ToLower() + ", " + script.fadeDistance + ", " + script.realmID + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ")" + " )" + "\n";
            }

            if(JumppadObjects.Length > 0)
                saved += "\n";
            //End of JumpPads

            //Generate All BubbleShields
            GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");

            if(BubbleShieldObjects.Length > 0)
                saved += "    //Bubble Shields" + "\n";

            foreach(GameObject go in BubbleShieldObjects) {
                string[] splitArray = go.name.Split(char.Parse(" "));
                string finished = splitArray[0].Replace("#", "/") + ".rmdl";
                BubbleScript script = go.GetComponent<BubbleScript>();
                string shieldColor = script.shieldColor.r + " " + script.shieldColor.g + " " + script.shieldColor.b;
                saved += "    MapEditor_CreateBubbleShieldWithSettings( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ", \"" + shieldColor +"\", $\"" + finished + "\" )" + "\n";
            }

            if(BubbleShieldObjects.Length > 0)
                saved += "\n";
            //End of BubbleShields

            //Generate All WeaponRacks
            GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");

            if(WeaponRackObjects.Length > 0)
                saved += "    //Weapon Racks" + "\n";

            foreach(GameObject go in WeaponRackObjects) {
                WeaponRackScript script = go.GetComponent<WeaponRackScript>();
                saved += @"    MapEditor_CreateRespawnableWeaponRack( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", \"" + go.name.Replace("custom_weaponrack_", "mp_weapon_") + "\", " + script.respawnTime + " )" + "\n";
            }

            if(WeaponRackObjects.Length > 0)
                saved += "\n";
            //End of WeaponRacks

            //Generate All LootBins
            GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");

            if(LootBinObjects.Length > 0)
                saved += "    //LootBins" + "\n";

            foreach(GameObject go in LootBinObjects) {
                LootBinScript script = go.GetComponent<LootBinScript>();
                saved += @"    MapEditor_CreateLootBin( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + script.lootbinSkin + " )" + "\n";
            }

            if(LootBinObjects.Length > 0)
                saved += "\n";
            //End of LootBins

            //Generate All ZipLines
            GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
            GameObject[] LinkedZipLineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");

            if(ZipLineObjects.Length > 0)
                saved += "    //ZipLines" + "\n";

            foreach(GameObject go in ZipLineObjects) {
                string ziplinestart = "";
                string ziplineend = "";

                foreach (Transform child in go.transform) {
                    if (child.name == "zipline_start") {
                        ziplinestart = Helper.BuildOrigin(child.gameObject);
                    }else if (child.name == "zipline_end") {
                        ziplineend = Helper.BuildOrigin(child.gameObject);
                    }
                }

                saved += @"    CreateZipline(" + ziplinestart + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + ziplineend + Helper.ShouldAddStartingOrg(UseStartingOffset) + ")" + "\n";
            }

            foreach(GameObject go in LinkedZipLineObjects) {
                bool first = true;
                string nodes = "[ ";

                LinkedZiplineScript script = go.GetComponent<LinkedZiplineScript>();

                foreach (Transform child in go.transform) {
                    if(!first)
                        nodes += ", ";

                    nodes += Helper.BuildOrigin(child.gameObject);

                    first = false;
                }

                string smoothType = "GetAllPointsOnBezier";
                if(!script.smoothType)
                    smoothType = "GetBezierOfPath";

                nodes += " ]";

                saved += @"    MapEditor_CreateLinkedZipline( ";
                if(script.enableSmoothing) saved += smoothType + "( ";
                saved += nodes;
                if(script.enableSmoothing) saved += ", " + script.smoothAmount;
                saved += " )";
                if(script.enableSmoothing) saved += " )";
                saved += "\n";
            }

            if(ZipLineObjects.Length > 0 || LinkedZipLineObjects.Length > 0)
                saved += "\n";
            //End of ziplines

            //Generate All Doors
            GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
            GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
            GameObject[] VertDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
            GameObject[] HorzDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");

            if(SingleDoorObjects.Length > 0 || DoubleDoorObjects.Length > 0 || VertDoorObjects.Length > 0 || HorzDoorObjects.Length > 0)
                saved += "    //Doors" + "\n";

            foreach(GameObject go in SingleDoorObjects) {
                DoorScript script = go.GetComponent<DoorScript>();
                saved += @"    MapEditor_SpawnDoor( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", eMapEditorDoorType.Single, " + script.goldDoor.ToString().ToLower() + " )" + "\n";
            }

            foreach(GameObject go in DoubleDoorObjects) {
                DoorScript script = go.GetComponent<DoorScript>();
                saved += @"    MapEditor_SpawnDoor( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", eMapEditorDoorType.Double, " + script.goldDoor.ToString().ToLower() + " )" + "\n";
            }

            foreach(GameObject go in VertDoorObjects)
                saved += @"    MapEditor_SpawnDoor( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", eMapEditorDoorType.Vertical)" + "\n";

            foreach(GameObject go in HorzDoorObjects)
                saved += @"    MapEditor_SpawnDoor( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", eMapEditorDoorType.Horizontal)" + "\n";

            if(SingleDoorObjects.Length > 0 || DoubleDoorObjects.Length > 0 || VertDoorObjects.Length > 0 || HorzDoorObjects.Length > 0)
                saved += "\n";
            //End of Doors

            //Generate All Props
            GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");

            if(PropObjects.Length > 0)
                saved += "    //Props" + "\n";
                
            foreach(GameObject go in PropObjects) {
                string[] splitArray = go.name.Split(char.Parse(" "));
                string finished = splitArray[0].Replace("#", "/") + ".rmdl";
                PropScript script = go.GetComponent<PropScript>();
                saved += "    MapEditor_CreateProp( $\"" + finished + "\", " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + script.allowMantle.ToString().ToLower() + ", " + script.fadeDistance + ", " + script.realmID + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ")" + "\n";
            }

            if(PropObjects.Length > 0)
                saved += "\n";

            //Generate All Triggers
            GameObject[] TriggerObjects = GameObject.FindGameObjectsWithTag("Trigger");

            if(TriggerObjects.Length > 0)
                saved += "    //Triggers" + "\n";

            int triggerid = 0;
            foreach(GameObject go in TriggerObjects) {
                TriggerScripting script = go.GetComponent<TriggerScripting>();
                saved += @"    entity trigger" + triggerid + " = MapEditor_CreateTrigger( " + Helper.BuildOrigin(go) + Helper.ShouldAddStartingOrg(UseStartingOffset) + ", " + Helper.BuildAngles(go) + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ", " + go.transform.localScale.y.ToString().Replace(",", ".") + ", " + script.Debug.ToString().ToLower() + ")" + "\n";
                
                if(script.EnterCallback != "")
                    saved += @"    trigger" + triggerid + ".SetEnterCallback( void function(entity trigger , entity ent) {" + "\n" + script.EnterCallback + "\n" + "    })" + "\n";
                
                if(script.LeaveCallback != "")
                    saved += @"    trigger" + triggerid + ".SetLeaveCallback( void function(entity trigger , entity ent) {" + "\n" + script.LeaveCallback + "\n" + "    })" + "\n";

                saved += @"    DispatchSpawn( trigger" + triggerid + " )" + "\n";
                triggerid++;
            }
            //End of Triggers

            if(!onlymap)
                saved += "}";

            System.IO.File.WriteAllText(path, saved);
        }
        else
        {
            Debug.Log("Failed: Didnt pick a save path!");
        }
    }

    private static void BuildStartingString()
    {
        Scene scene = SceneManager.GetActiveScene();
        StartFunction = @"void function " + scene.name.Replace(" ", "_") + "()" + "\n" + "{" + "\n";
    }
} 