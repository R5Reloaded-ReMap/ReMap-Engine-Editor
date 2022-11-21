using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CopyPasteCode : EditorWindow
{
    static bool OnlyExportMap = true;
    static bool UseStartingOffset = false;
    static bool OverrideTextLimit = false;
    static string text = "";
    static Vector2 scroll;

    static string StartFunction;

    [MenuItem("R5Reloaded/Map Code", false, 25)]
    static void Init()
    {
        CopyPasteCode window = (CopyPasteCode)GetWindow(typeof(CopyPasteCode), false, "Live Map Code");
        window.Show();
    }

    void OnEnable()
    {
        EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
    }

    private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode)
    {
        OverrideTextLimit = false;
        text = "";
    }

    void OnGUI()
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

        if(finalcount < 1500)
            GUI.contentColor = Color.green;
        else if((finalcount < 3000)) 
            GUI.contentColor = Color.yellow;
        else
            GUI.contentColor = Color.red;

        GUILayout.Label("Entity Count: " + finalcount);
        if(finalcount < 1500)
            GUILayout.Label("Entity Status: Safe");
        else if((finalcount < 3000)) 
            GUILayout.Label("Entity Status: Safe");
        else
            GUILayout.Label("Entity Status: Warning! Game could crash!");
            
        GUI.contentColor = Color.white;

        OnlyExportMap = EditorGUILayout.Toggle("Only Show Map Code", OnlyExportMap);
        UseStartingOffset = EditorGUILayout.Toggle("Use Map Origin Offset", UseStartingOffset);

        if (GUILayout.Button("Copy Export Code"))
            GenerateMap(OnlyExportMap, true);

        if (text.Length > 75000)
        {
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Export is to long to show in the textbox, Please copy using the button above!");
            GUI.contentColor = Color.white;
            OverrideTextLimit = EditorGUILayout.Toggle("Override Text Limit", OverrideTextLimit);
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);
        if(text.Length > 75000 && !OverrideTextLimit) {
            GUI.contentColor = Color.yellow;
            GUILayout.TextArea("Output code is to long. You can override this with the toggle above. \nWARNING: MAY CAUSE LAG!", GUILayout.ExpandHeight(true));
            GUI.contentColor = Color.white;
        }
        else
        {
            GenerateMap(OnlyExportMap, false);
            GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        }
        EditorGUILayout.EndScrollView();
    }

    void GenerateMap(bool onlymap, bool copytext)
    {
        BuildStartingString();
        SetPropTagsItem();

        EditorSceneManager.SaveOpenScenes();

        string saved = "";

        if(!onlymap)
            saved = Credits + "\n" + StartFunction +  ShouldAddStartingOrg(1);
        else
            saved = ShouldAddStartingOrg(1);

        //Generate All Buttons
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");

        if(ButtonObjects.Length > 0)
            saved += "    //Buttons" + "\n";

        foreach(GameObject go in ButtonObjects) {
            ButtonScripting script = go.GetComponent<ButtonScripting>();
            saved += "    AddCallback_OnUseEntity( CreateFRButton(" + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", \"" + script.UseText + "\"), void function(entity panel, entity user, int input)" + "\n" + "    {" + "\n" + script.OnUseCallback + "\n" + "    })" + "\n";
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
            saved += "    JumpPad_CreatedCallback( MapEditor_CreateProp( $\"mdl/props/octane_jump_pad/octane_jump_pad.rmdl\" , " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", " + script.allowMantle.ToString().ToLower() + ", " + script.fadeDistance + ", " + script.realmID + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ")" + " )" + "\n";
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
            saved += "    MapEditor_CreateBubbleShieldWithSettings( " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ", \"" + shieldColor +"\", $\"" + finished + "\" )" + "\n";
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
            saved += @"    MapEditor_CreateRespawnableWeaponRack( " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", \"" + go.name.Replace("custom_weaponrack_", "mp_weapon_") + "\", " + script.respawnTime + " )" + "\n";
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
            saved += @"    MapEditor_CreateLootBin( " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", " + script.lootbinSkin + " )" + "\n";
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
                    ziplinestart = BuildOrigin(child.gameObject);
                }else if (child.name == "zipline_end") {
                    ziplineend = BuildOrigin(child.gameObject);
                }
            }

            saved += @"    CreateZipline(" + ziplinestart + ShouldAddStartingOrg() + ", " + ziplineend + ShouldAddStartingOrg() + ")" + "\n";
        }

        foreach(GameObject go in LinkedZipLineObjects) {
            bool first = true;
            string nodes = "[ ";

            LinkedZiplineScript script = go.GetComponent<LinkedZiplineScript>();

            foreach (Transform child in go.transform) {
                if(!first)
                    nodes += ", ";

                nodes += BuildOrigin(child.gameObject);

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
            saved += @"    MapEditor_SpawnDoor( " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", eMapEditorDoorType.Single, " + script.goldDoor.ToString().ToLower() + " )" + "\n";
        }

        foreach(GameObject go in DoubleDoorObjects) {
            DoorScript script = go.GetComponent<DoorScript>();
            saved += @"    MapEditor_SpawnDoor( " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", eMapEditorDoorType.Double, " + script.goldDoor.ToString().ToLower() + " )" + "\n";
        }

        foreach(GameObject go in VertDoorObjects)
            saved += @"    MapEditor_SpawnDoor( " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", eMapEditorDoorType.Vertical)" + "\n";

        foreach(GameObject go in HorzDoorObjects)
            saved += @"    MapEditor_SpawnDoor( " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", eMapEditorDoorType.Horizontal)" + "\n";

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
            saved += "    MapEditor_CreateProp( $\"" + finished + "\", " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", " + script.allowMantle.ToString().ToLower() + ", " + script.fadeDistance + ", " + script.realmID + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ")" + "\n";
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
            saved += @"    entity trigger" + triggerid + " = MapEditor_CreateTrigger( " + BuildOrigin(go) + ShouldAddStartingOrg() + ", " + BuildAngles(go) + ", " + go.transform.localScale.x.ToString().Replace(",", ".") + ", " + go.transform.localScale.y.ToString().Replace(",", ".") + ", " + script.Debug.ToString().ToLower() + ")" + "\n";
            
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
     
        if(copytext) {
            GUIUtility.systemCopyBuffer = saved;

            if(saved.Length > 75000)
                saved = "";
        } else {
            text = saved;
            saved = "";
        }
    }

    private static string BuildAngles(GameObject go)
    {
        string x = (-WrapAngle(go.transform.eulerAngles.x)).ToString("F4");
        string y = (-WrapAngle(go.transform.eulerAngles.y)).ToString("F4");
        string z = (WrapAngle(go.transform.eulerAngles.z)).ToString("F4");
                    
        string angles = "< " + x.Replace(",", ".") + ", " + y.Replace(",", ".") + ", " + z.Replace(",", ".") + " >";

        return angles;
    }

    private static float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }

    private static string BuildOrigin(GameObject go)
    {
        string x = (-go.transform.position.z).ToString("F4");
        string y = (go.transform.position.x).ToString("F4");
        string z = (go.transform.position.y).ToString("F4");

        string origin = "< " + x.Replace(",", ".") + ", " + y.Replace(",", ".") + ", " + z.Replace(",", ".") + " >";

        return origin;
    }

    private static string ShouldAddStartingOrg(int type = 0)
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

    void BuildStartingString()
    {
        Scene scene = SceneManager.GetActiveScene();
        StartFunction = @"void function " + scene.name.Replace(" ", "_") + "()" + "\n" + "{" + "\n";
    }

    //Tags Custom Prefabs so users cant retag a item wrong
    private static void SetPropTagsItem()
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

    static string Credits = @"
//Made with Unity Map Editor
//By AyeZee#6969
//With help from Julefox#0050
";
}