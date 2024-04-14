using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class CodeImport : EditorWindow
{
    private static string text = "";
    private static Vector2 scroll;
    private readonly List< string > BubbleSheilds = new();
    private readonly List< string > Buttons = new();
    private readonly List< string > Doors = new();
    private readonly List< string > JumpPads = new();
    private readonly List< string > LinkedZiplines = new();
    private readonly List< string > LootBins = new();

    private readonly List< string > Props = new();
    private readonly List< string > Triggers = new();
    private readonly List< string > WeaponRacks = new();
    private readonly List< string > Ziplines = new();

    public static void Init()
    {
        var window = ( CodeImport )GetWindow( typeof(CodeImport), false, "Import Map Code" );
        window.Show();
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView( scroll );
        text = GUILayout.TextArea( text, GUILayout.ExpandHeight( true ) );
        EditorGUILayout.EndScrollView();
        if ( GUILayout.Button( "Import Map Code" ) )
            ImportMapCode();
    }

    private async void ImportMapCode()
    {
        ReMapConsole.Log( "[Code Import] Starting Import", ReMapConsole.LogType.Info );
        Props.Clear();
        JumpPads.Clear();
        BubbleSheilds.Clear();
        WeaponRacks.Clear();
        LootBins.Clear();
        Ziplines.Clear();
        LinkedZiplines.Clear();
        Doors.Clear();
        Triggers.Clear();
        Buttons.Clear();

        await BuildImportList();
        await ImportProps();
        await ImportButtons();
        await ImportJumpPads();
        await ImportBubbleSheilds();
        await ImportWeaponRacks();
        await ImportLootBins();
        await ImportZiplines();
        await ImportDoors();
        await ImportTriggers();

        ReMapConsole.Log( "[Code Import] Finished", ReMapConsole.LogType.Success );

        EditorUtility.ClearProgressBar();
    }

    private async Task BuildImportList()
    {
        EditorUtility.DisplayProgressBar( "Building Import List", "Building...", 0.0f );

        ReMapConsole.Log( "[Code Import] Building Import List", ReMapConsole.LogType.Info );

        string[] lines = text.Replace( "+ startingorg", "" ).Split( char.Parse( "\n" ) );
        foreach ( string line in lines )
            if ( line.Contains( "JumpPad_CreatedCallback(" ) )
                JumpPads.Add( line.Replace( "JumpPad_CreatedCallback( MapEditor_CreateProp(", "" ).Replace( " ) )", "" ).Replace( " ", "" ) );
            else if ( line.Contains( "MapEditor_CreateProp(" ) )
                Props.Add( line.Replace( "MapEditor_CreateProp(", "" ).Replace( " )", "" ).Replace( " ", "" ) );
            else if ( line.Contains( "MapEditor_CreateBubbleShieldWithSettings(" ) )
                BubbleSheilds.Add( line.Replace( "MapEditor_CreateBubbleShieldWithSettings(", "" ).Replace( " )", "" ) );
            else if ( line.Contains( "MapEditor_CreateRespawnableWeaponRack(" ) )
                WeaponRacks.Add( line.Replace( "MapEditor_CreateRespawnableWeaponRack(", "" ).Replace( " )", "" ).Replace( " ", "" ) );
            else if ( line.Contains( "MapEditor_CreateLootBin(" ) )
                LootBins.Add( line.Replace( "MapEditor_CreateLootBin(", "" ).Replace( " )", "" ).Replace( " ", "" ) );
            else if ( line.Contains( "CreateZipline(" ) )
                Ziplines.Add( line.Replace( "CreateZipline(", "" ).Replace( " )", "" ).Replace( " ", "" ) );
            else if ( line.Contains( "MapEditor_CreateLinkedZipline(" ) )
                LinkedZiplines.Add( line.Replace( "MapEditor_CreateLinkedZipline(", "" ).Replace( " )", "" ).Replace( " ", "" ) );
            else if ( line.Contains( "MapEditor_SpawnDoor(" ) )
                Doors.Add( line.Replace( "MapEditor_SpawnDoor(", "" ).Replace( " )", "" ).Replace( " ", "" ) );
            else if ( line.Contains( "MapEditor_CreateTrigger(" ) )
                Triggers.Add( line.Replace( "MapEditor_CreateTrigger(", "" ).Replace( " )", "" ).Replace( " ", "" ) );
            else if ( line.Contains( "AddCallback_OnUseEntity( CreateFRButton(" ) )
                Buttons.Add( line.Replace( "AddCallback_OnUseEntity( CreateFRButton(", "" ).Replace( "), void function(entity panel, entity user, int input)", "" ).Replace( ", ", "," ) );

        await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
    }

    private async Task ImportProps()
    {
        if ( Props.Count == 0 )
            return;

        var find = GameObject.Find( "Props" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "Props" );
            objToSpawn.name = "Props";
        }

        int i = 0;
        foreach ( string prop in Props )
        {
            string[] split = prop.Split( char.Parse( "," ) );
            if ( split.Length < 11 )
                continue;

            EditorUtility.DisplayProgressBar( "Importing Props", "Importing: " + split[0], ( i + 1 ) / ( float )Props.Count );


            string Model = split[0].Replace( "/", "#" ).Replace( ".rmdl", "" ).Replace( "\"", "" ).Replace( "\n", "" ).Replace( "\r", "" ).Replace( "$", "" );

            ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( Model );
            if ( results.Length == 0 )
            {
                ReMapConsole.Log( "[Code Import] Prefab not found for: " + Model, ReMapConsole.LogType.Error );
                continue;
            }

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            obj.transform.position = new Vector3( float.Parse( split[2] ), float.Parse( split[3].Replace( ">", "" ) ), -float.Parse( split[1].Replace( "<", "" ) ) );
            obj.transform.eulerAngles = new Vector3( -float.Parse( split[4].Replace( "<", "" ) ), -float.Parse( split[5] ), float.Parse( split[6].Replace( ">", "" ) ) );
            obj.name = Model;
            //obj.gameObject.transform.localScale = new Vector3(float.Parse(split[10]), float.Parse(split[10]), float.Parse(split[10]));
            //
            var script = obj.GetComponent< PropScript >();
            script.FadeDistance = int.Parse( split[8] );
            script.AllowMantle = bool.Parse( split[7] );
            script.RealmID = int.Parse( split[9] );

            var parent = GameObject.Find( "Props" );
            if ( parent != null )
                obj.gameObject.transform.parent = parent.transform;

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }
    }

    private async Task ImportButtons()
    {
        if ( Buttons.Count == 0 )
            return;

        var find = GameObject.Find( "Buttons" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "Buttons" );
            objToSpawn.name = "Buttons";
        }

        int i = 0;
        foreach ( string button in Buttons )
        {
            string[] split = button.Split( char.Parse( "," ) );
            if ( split.Length < 3 )
                continue;

            EditorUtility.DisplayProgressBar( "Importing Buttons", "Importing: Button " + i, ( i + 1 ) / ( float )Buttons.Count );

            string Model = "custom_button";

            ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( Model );
            if ( results.Length == 0 )
                continue;

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            obj.transform.position = new Vector3( float.Parse( split[1] ), float.Parse( split[2].Replace( ">", "" ).Replace( " ", "" ) ), -float.Parse( split[0].Replace( "<", "" ).Replace( " ", "" ) ) );
            obj.transform.eulerAngles = new Vector3( -float.Parse( split[3].Replace( "<", "" ).Replace( " ", "" ) ), -float.Parse( split[4] ), float.Parse( split[5].Replace( ">", "" ).Replace( " ", "" ) ) );
            obj.name = Model;

            var script = obj.GetComponent< ButtonScripting >();
            script.UseText = split[6].Replace( "\"", "" );

            var parent = GameObject.Find( "Buttons" );
            if ( parent != null )
                obj.gameObject.transform.parent = parent.transform;

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }
    }

    private async Task ImportBubbleSheilds()
    {
        if ( BubbleSheilds.Count == 0 )
            return;

        var find = GameObject.Find( "BubbleSheilds" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "BubbleSheilds" );
            objToSpawn.name = "BubbleSheilds";
        }

        int i = 0;
        foreach ( string bsheild in BubbleSheilds )
        {
            string[] split = bsheild.Split( char.Parse( "," ) );
            if ( split.Length < 9 )
                continue;

            EditorUtility.DisplayProgressBar( "Importing BubbleSheilds", "Importing: BubbleSheild" + i, ( i + 1 ) / ( float )BubbleSheilds.Count );

            string Model = split[8].Replace( "/", "#" ).Replace( ".rmdl", "" ).Replace( "\"", "" ).Replace( "\n", "" ).Replace( "\r", "" ).Replace( "$", "" ).Replace( " ", "" );

            ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( Model );
            if ( results.Length == 0 )
                continue;

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            obj.transform.position = new Vector3( float.Parse( split[1].Replace( " ", "" ) ), float.Parse( split[2].Replace( ">", "" ).Replace( " ", "" ) ), -float.Parse( split[0].Replace( "<", "" ).Replace( " ", "" ) ) );
            obj.transform.eulerAngles = new Vector3( -float.Parse( split[3].Replace( "<", "" ).Replace( " ", "" ) ), -float.Parse( split[4].Replace( " ", "" ) ), float.Parse( split[5].Replace( ">", "" ).Replace( " ", "" ) ) );
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3( float.Parse( split[6].Replace( " ", "" ) ), float.Parse( split[6].Replace( " ", "" ) ), float.Parse( split[6].Replace( " ", "" ) ) );

            string[] colorsplit = split[7].Split( char.Parse( " " ) );
            var script = obj.GetComponent< BubbleScript >();
            script.ShieldColor.r = byte.Parse( colorsplit[1].Replace( "\"", "" ) );
            script.ShieldColor.g = byte.Parse( colorsplit[2].Replace( "\"", "" ) );
            script.ShieldColor.b = byte.Parse( colorsplit[3].Replace( "\"", "" ) );

            var parent = GameObject.Find( "BubbleSheilds" );
            if ( parent != null )
                obj.gameObject.transform.parent = parent.transform;

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }
    }

    private async Task ImportJumpPads()
    {
        if ( JumpPads.Count == 0 )
            return;

        var find = GameObject.Find( "JumpPads" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "JumpPads" );
            objToSpawn.name = "JumpPads";
        }

        int i = 0;
        foreach ( string jumppad in JumpPads )
        {
            string[] split = jumppad.Split( char.Parse( "," ) );
            if ( split.Length < 7 )
                continue;

            EditorUtility.DisplayProgressBar( "Importing Jumppads", "Importing: Jumppad" + i, ( i + 1 ) / ( float )JumpPads.Count );

            string Model = "custom_jumppad";

            ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( Model );
            if ( results.Length == 0 )
                continue;

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            obj.transform.position = new Vector3( float.Parse( split[2] ), float.Parse( split[3].Replace( ">", "" ) ), -float.Parse( split[1].Replace( "<", "" ) ) );
            obj.transform.eulerAngles = new Vector3( -float.Parse( split[4].Replace( "<", "" ) ), -float.Parse( split[5] ), float.Parse( split[6].Replace( ">", "" ) ) );
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3( float.Parse( split[10] ), float.Parse( split[10] ), float.Parse( split[10] ) );

            var script = obj.GetComponent< PropScript >();
            script.FadeDistance = int.Parse( split[8] );
            script.AllowMantle = bool.Parse( split[7] );
            script.RealmID = int.Parse( split[9] );

            var parent = GameObject.Find( "JumpPads" );
            if ( parent != null )
                obj.gameObject.transform.parent = parent.transform;

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }
    }

    private async Task ImportLootBins()
    {
        if ( LootBins.Count == 0 )
            return;

        var find = GameObject.Find( "LootBins" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "LootBins" );
            objToSpawn.name = "LootBins";
        }

        int i = 0;
        foreach ( string bin in LootBins )
        {
            string[] split = bin.Split( char.Parse( "," ) );
            if ( split.Length < 7 )
                continue;

            EditorUtility.DisplayProgressBar( "Importing Lootbins", "Importing: Lootbin" + i, ( i + 1 ) / ( float )LootBins.Count );

            string Model = "custom_lootbin";

            ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( Model );
            if ( results.Length == 0 )
                continue;

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            obj.transform.position = new Vector3( float.Parse( split[1] ), float.Parse( split[2].Replace( ">", "" ) ), -float.Parse( split[0].Replace( "<", "" ) ) );
            obj.transform.eulerAngles = new Vector3( -float.Parse( split[3].Replace( "<", "" ) ), -float.Parse( split[4] ), float.Parse( split[5].Replace( ">", "" ) ) );
            obj.name = Model;

            var script = obj.GetComponent< LootBinScript >();
            script.LootbinSkin = ( LootBinSkinType )int.Parse( split[6] );

            var parent = GameObject.Find( "LootBins" );
            if ( parent != null )
                obj.gameObject.transform.parent = parent.transform;

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }
    }

    private async Task ImportWeaponRacks()
    {
        if ( WeaponRacks.Count == 0 )
            return;

        var find = GameObject.Find( "WeaponRacks" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "WeaponRacks" );
            objToSpawn.name = "WeaponRacks";
        }

        int i = 0;
        foreach ( string wrack in WeaponRacks )
        {
            string[] split = wrack.Split( char.Parse( "," ) );
            if ( split.Length < 8 )
                continue;

            string Model = split[6].Replace( "\"", "" ).Replace( "mp_weapon_", "custom_weaponrack_" );

            ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

            EditorUtility.DisplayProgressBar( "Importing WeaponRacks", "Importing: " + Model, ( i + 1 ) / ( float )WeaponRacks.Count );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( Model );
            if ( results.Length == 0 )
                continue;

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            obj.transform.position = new Vector3( float.Parse( split[1] ), float.Parse( split[2].Replace( ">", "" ) ), -float.Parse( split[0].Replace( "<", "" ) ) );
            obj.transform.eulerAngles = new Vector3( -float.Parse( split[3].Replace( "<", "" ) ), -float.Parse( split[4] ), float.Parse( split[5].Replace( ">", "" ) ) );
            obj.name = Model;

            var script = obj.GetComponent< WeaponRackScript >();
            script.WeaponRespawnTime = int.Parse( split[7] );

            var parent = GameObject.Find( "WeaponRacks" );
            if ( parent != null )
                obj.gameObject.transform.parent = parent.transform;

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }
    }

    private async Task ImportZiplines()
    {
        if ( Ziplines.Count == 0 && LinkedZiplines.Count == 0 )
            return;

        var find = GameObject.Find( "Ziplines" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "Ziplines" );
            objToSpawn.name = "Ziplines";
        }

        if ( Ziplines.Count != 0 )
        {
            int i = 0;
            foreach ( string zip in Ziplines )
            {
                string[] split = zip.Split( char.Parse( "," ) );
                if ( split.Length < 6 )
                    continue;

                EditorUtility.DisplayProgressBar( "Importing Ziplines", "Importing: custom_zipline " + i, ( i + 1 ) / ( float )Ziplines.Count );

                string Model = "custom_zipline";

                ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

                //Find Model GUID in Assets
                string[] results = AssetDatabase.FindAssets( Model );
                if ( results.Length == 0 )
                    continue;

                //Get model path from guid and load it
                Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
                if ( loadedPrefabResource == null )
                    continue;

                var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
                foreach ( Transform child in obj.transform )
                    if ( child.name == "zipline_start" )
                        child.transform.position = new Vector3( float.Parse( split[1] ), float.Parse( split[2].Replace( ">", "" ) ), -float.Parse( split[0].Replace( "<", "" ) ) );
                    else if ( child.name == "zipline_end" )
                        child.transform.position = new Vector3( float.Parse( split[4] ), float.Parse( split[5].Replace( ">", "" ) ), -float.Parse( split[3].Replace( "<", "" ) ) );

                var parent = GameObject.Find( "Ziplines" );
                if ( parent != null )
                    obj.gameObject.transform.parent = parent.transform;

                await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
                i++;
            }
        }

        if ( LinkedZiplines.Count != 0 )
        {
            int f = 0;
            foreach ( string zip in LinkedZiplines )
            {
                bool isSmoothed = false;
                bool smoothingType = false;
                int SmoothAmount = 0;

                if ( zip.Contains( "GetAllPointsOnBezier" ) )
                {
                    isSmoothed = true;
                    smoothingType = true;
                }
                else if ( zip.Contains( "GetBezierOfPath" ) )
                {
                    isSmoothed = true;
                    smoothingType = false;
                }

                string finishedzip = zip.Replace( "GetAllPointsOnBezier(", "" ).Replace( "GetBezierOfPath(", "" ).Replace( ")", "" ).Replace( "[", "" ).Replace( "]", "" ).Replace( ">,<", ":" );
                string[] split = finishedzip.Split( char.Parse( ":" ) );

                ReMapConsole.Log( "[Code Import] Importing: custom_linked_zipline", ReMapConsole.LogType.Info );

                EditorUtility.DisplayProgressBar( "Importing Linked Ziplines", "Importing: custom_linked_zipline " + f, ( f + 1 ) / ( float )LinkedZiplines.Count );

                var obj = new GameObject( "custom_linked_zipline" );
                obj.AddComponent< LinkedZiplineScript >();

                int i = 0;
                foreach ( string s in split )
                {
                    string[] split2 = s.Split( char.Parse( "," ) );

                    if ( i == split.Length - 1 && isSmoothed )
                        SmoothAmount = int.Parse( split2[3] );

                    var child = new GameObject( "zipline_node" );
                    child.transform.position = new Vector3( float.Parse( split2[1] ), float.Parse( split2[2].Replace( ">", "" ) ), -float.Parse( split2[0].Replace( "<", "" ) ) );
                    child.transform.parent = obj.transform;

                    i++;
                }

                var script = obj.GetComponent< LinkedZiplineScript >();
                script.EnableSmoothing = isSmoothed;
                script.SmoothType = smoothingType;
                script.SmoothAmount = SmoothAmount;

                var parent = GameObject.Find( "Ziplines" );
                if ( parent != null )
                    obj.gameObject.transform.parent = parent.transform;

                await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
                f++;
            }
        }
    }

    private async Task ImportDoors()
    {
        if ( Doors.Count == 0 )
            return;

        var find = GameObject.Find( "Doors" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "Doors" );
            objToSpawn.name = "Doors";
        }

        int i = 0;
        foreach ( string door in Doors )
        {
            string[] split = door.Split( char.Parse( "," ) );

            string Model = "custom_single_door";
            bool IsSingleOrDouble = false;
            switch ( split[6] )
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

            ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

            EditorUtility.DisplayProgressBar( "Importing Doors", "Importing: " + Model, ( i + 1 ) / ( float )Doors.Count );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( Model );
            if ( results.Length == 0 )
                continue;

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            obj.transform.position = new Vector3( float.Parse( split[1] ), float.Parse( split[2].Replace( ">", "" ) ), -float.Parse( split[0].Replace( "<", "" ) ) );
            obj.transform.eulerAngles = new Vector3( -float.Parse( split[3].Replace( "<", "" ) ), -float.Parse( split[4] ), float.Parse( split[5].Replace( ">", "" ) ) );
            obj.name = Model;

            if ( IsSingleOrDouble )
            {
                var script = obj.GetComponent< DoorScript >();
                script.GoldDoor = bool.Parse( split[7] );
            }

            var parent = GameObject.Find( "Doors" );
            if ( parent != null )
                obj.gameObject.transform.parent = parent.transform;

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }
    }

    private async Task ImportTriggers()
    {
        if ( Triggers.Count == 0 )
            return;

        var find = GameObject.Find( "Triggers" );
        if ( find == null )
        {
            var objToSpawn = new GameObject( "Triggers" );
            objToSpawn.name = "Triggers";
        }

        int i = 0;
        foreach ( string trigger in Triggers )
        {
            string[] split1 = trigger.Split( char.Parse( "=" ) );
            string[] split = split1[1].Split( char.Parse( "," ) );

            string Model = "trigger_cylinder";

            ReMapConsole.Log( "[Code Import] Importing: " + Model, ReMapConsole.LogType.Info );

            EditorUtility.DisplayProgressBar( "Importing Doors", "Importing: trigger_cylinder " + i, ( i + 1 ) / ( float )Triggers.Count );

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets( Model );
            if ( results.Length == 0 )
                continue;

            //Get model path from guid and load it
            Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof(Object) ) as GameObject;
            if ( loadedPrefabResource == null )
                continue;

            var obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            obj.transform.position = new Vector3( float.Parse( split[1] ), float.Parse( split[2].Replace( ">", "" ) ), -float.Parse( split[0].Replace( "<", "" ) ) );
            obj.transform.eulerAngles = new Vector3( -float.Parse( split[3].Replace( "<", "" ) ), -float.Parse( split[4] ), float.Parse( split[5].Replace( ">", "" ) ) );
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3( float.Parse( split[6] ), float.Parse( split[7] ), float.Parse( split[6] ) );

            var script = obj.GetComponent< TriggerScripting >();
            script.Debug = bool.Parse( split[8] );

            var parent = GameObject.Find( "Triggers" );
            if ( parent != null )
                obj.gameObject.transform.parent = parent.transform;

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );
            i++;
        }
    }
}