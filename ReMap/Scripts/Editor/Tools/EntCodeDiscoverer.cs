using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LibrarySorter;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntCodeDiscoverer : EditorWindow
{
    private static string Code = "";

    private static readonly string OutputParent = "Discovered_Code";
    private static string NamedParent = "";

    private static readonly string[] WeaponLocationModel =
    {
        "mdl/weapons_r5/loot/_master/w_loot_cha_shield_upgrade_body_v1.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_cha_shield_upgrade_head_v1.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_ammo_hc.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_ammo_nrg.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_ammo_sc.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_ammo_shg.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_mods_chip.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_mods_mag_energy_v1.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_mods_mag_v1b.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_mods_mag_v2b.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_mods_mag_v3b.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_mods_optic_cq_hcog_r2.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_mods_optic_cq_hcog_r1.rmdl",
        "mdl/weapons_r5/loot/_master/w_loot_wep_mods_suppr_v2b.rmdl",
        "mdl/weapons_r5/loot/w_loot_wep_iso_shield_down_v1.rmdl",
        "mdl/weapons_r5/loot/w_loot_wep_iso_health_main_large.rmdl",
        "mdl/weapons_r5/loot/w_loot_wep_iso_health_main_small.rmdl",
        "mdl/weapons_r5/loot/w_loot_wep_iso_phoenix_kit_v1.rmdl",
        "mdl/weapons_r5/loot/w_loot_wep_iso_shield_battery_large.rmdl",
        "mdl/weapons_r5/loot/w_loot_wep_iso_shield_battery_small.rmdl"
    };

    private static Vector2 Scroll = Vector2.zero;

    public static void Init()
    {
        var window = ( EntCodeDiscoverer )GetWindow( typeof(EntCodeDiscoverer), false, "Ent Code Discoverer" );
        window.minSize = new Vector2( 400, 200 );
        window.Show();
    }

    private void OnGUI()
    {
        Scroll = EditorGUILayout.BeginScrollView( Scroll );
        Code = GUILayout.TextArea( Code, GUILayout.ExpandHeight( true ) );
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        WindowUtility.WindowUtility.CreateButton( "Discover Code", "", () =>
        {
            ConvertCode();
            AddEntsToScene();
        } );
        WindowUtility.WindowUtility.CreateButton( "Import From File", "", () => ImportCode(), 100 );
        WindowUtility.WindowUtility.CreateButton( "Clear Code", "", () => { Code = ""; }, 100 );
        EditorGUILayout.EndHorizontal();
    }

    private static void ImportCode()
    {
        string path = EditorUtility.OpenFilePanel( "Import Ent File", "", "ent" );
        if ( string.IsNullOrEmpty( path ) )
            return;

        NamedParent = $"{OutputParent} ({Path.GetFileNameWithoutExtension( path ).Replace( "mp_rr_", "" )})";

        Code = File.ReadAllText( path );

        ConvertCode();

        AddEntsToScene();

        //RMAPDEV_GetAllEntType();

        Code = "";
    }

    /// <summary>
    ///     Convert all code in file to EntitiesData
    /// </summary>
    private static void ConvertCode()
    {
        string[] codes = Code.Replace( "}", "" ).Split( "{" );

        EntitiesData.EntitiesGlobal = new Dictionary< string, List< EntitiesData > >();

        int min = 0;
        int max = codes.Length;
        float progress = 0.0f;

        foreach ( string code in codes )
        {
            EditorUtility.DisplayProgressBar( "Exploring Code", $"Processing... ({min}/{max})", progress );

            new EntitiesData( code );

            progress += 1.0f / max;
            min++;
        }

        EditorUtility.ClearProgressBar();
    }

    public static void AddEntsToScene()
    {
        if ( string.IsNullOrEmpty( NamedParent ) ) NamedParent = OutputParent;

        Helper.CreatePath( NamedParent );

        if ( EntitiesData.EntitiesGlobal.TryGetValue( "zipline_end", out var ziplineEnd ) )
        {
            if ( EntitiesData.EntitiesGlobal.TryGetValue( "zipline", out var zipline ) )
                zipline.AddRange( ziplineEnd );
            else EntitiesData.EntitiesGlobal["zipline"] = ziplineEnd;

            EntitiesData.EntitiesGlobal.Remove( "zipline_end" );
        }

        int entListGlobalCountStart = 1;
        int entListGlobalCount = EntitiesData.EntitiesGlobal.Keys.Count;
        int entGlobalCountStart = 1;
        int entGlobalCount = EntitiesData.GetTotalEntitiesData();

        foreach ( string classtype in EntitiesData.EntitiesGlobal.Keys )
        {
            var parent = Helper.CreatePath( $"{NamedParent}/{classtype} class" );

            int min = 0;
            int max = EntitiesData.EntitiesGlobal[classtype].Count;
            float progress = 0.0f;

            foreach ( var entity in EntitiesData.EntitiesGlobal[classtype] )
            {
                EditorUtility.DisplayProgressBar
                (
                    $"Adding Entities To Scene ({entListGlobalCountStart}/{entListGlobalCount})",
                    $"Processing... ( {classtype} => {min}/{max} ) ( Total => {entGlobalCountStart}/{entGlobalCount} )", progress
                );

                progress += 1.0f / max;
                min++;

                string skin = SetSkin( entity );
                var obj = Helper.CreateGameObject( SetName( entity ), skin, parent );

                var origin = Helper.ConvertApexOriginToUnity( entity.Origin );

                if ( !Helper.IsValid( obj ) )
                {
                    entGlobalCountStart++;
                    continue;
                }

                var entitiesKeyValues = obj.AddComponent< EntitiesKeyValues >();

                foreach ( var keyval in entity.KeyValues )
                    entitiesKeyValues.KeyValues.Add( $"\"{keyval.Key}\" \"{keyval.Value}\"" );

                var transformedObj = obj.transform;
                transformedObj.position = origin;

                if ( skin == UnityInfo.relativePathCubePrefab )
                    SetColor( obj, entity );

                var newParent = obj;

                // Angles
                if ( entity.HasKey( "angles" ) && CanBeRotate( classtype ) )
                {
                    var angles = Helper.ConvertApexAnglesToUnity( entity.Angles );

                    // Idk why apex screen need to ba at ( 0, 0, 0 )
                    if ( entity.ScriptName == "apex_screen" )
                    {
                        transformedObj.eulerAngles = angles;
                    }
                    else
                    {
                        string model = entity.Model;
                        if ( model == "mdl/creatures/flyer/flyer_kingscanyon_animated.rmdl" ) model = model.Replace( "/creatures/", "/Creatures/" );

                        if ( entity.ScriptName == "static_loot_tick_spawn" ) model = "mdl/robots/drone_frag/drone_frag_loot.rmdl";

                        newParent = Helper.MoveGameObject( obj.name, obj, true );
                        newParent.transform.position = origin;
                        newParent.transform.eulerAngles = angles;
                        transformedObj.localPosition = Vector3.zero;
                        transformedObj.localEulerAngles = Models.FindAnglesOffset( model );
                    }
                }

                // Guid Link
                if ( entity.HasKey( "link_to_guid_0" ) )
                {
                    var folder = Helper.MoveGameObject( newParent, $"{Helper.CreatePathString( parent, true )}/{entity.GetValueForKey( "link_to_guid_0" )}" );

                    Dictionary< Transform, Vector3 > savedPos = new();

                    for ( int i = 0; i < newParent.transform.childCount; i++ )
                    {
                        var child = newParent.transform.GetChild( i );
                        savedPos.Add( child, child.position );
                    }

                    if ( Helper.IsValid( folder ) )
                    {
                        Helper.SetOriginAndAngles( folder, origin );
                        newParent.transform.position = origin;
                    }

                    foreach ( var data in savedPos )
                    {
                        var restorePos = data.Key.gameObject;
                        if ( restorePos.name != obj.name )
                            Helper.SetOriginAndAngles( restorePos, data.Value );
                    }
                }
                else if ( entity.HasKey( "link_guid" ) )
                {
                    Helper.CreatePath( $"{Helper.CreatePathString( parent, true )}/{entity.GetValueForKey( "link_guid" )}", newParent );
                }

                if ( classtype == "zipline" )
                {
                    for ( int i = 0; i < 30; i++ )
                    {
                        string idx = $"_zipline_rest_point_{i}";
                        if ( entity.HasKey( idx ) )
                        {
                            var restPoint = Helper.CreateGameObject( idx, UnityInfo.relativePathCubePrefab );
                            if ( Helper.IsValid( restPoint ) )
                            {
                                var restPointOrigin = Helper.ConvertApexOriginToUnity( Helper.StringToVector3( entity.GetValueForKey( idx ), true ) );
                                restPoint.transform.position = restPointOrigin;
                                restPoint.transform.parent = transformedObj.parent;

                                SetColor( restPoint, entity );
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if ( classtype == "ambient_generic" )
                {
                    if ( entity.HasKey( "soundName" ) )
                        obj.name = entity.GetValueForKey( "soundName" );

                    for ( int i = 0; i < 130; i++ )
                    {
                        string idx = $"polyline_segment_{i}";
                        var last = obj;
                        if ( entity.HasKey( idx ) )
                        {
                            var polylineSegment = Helper.CreateGameObject( idx, $"{UnityInfo.relativePathModel}/editor_ambient_generic_node_LOD0.fbx" );
                            if ( Helper.IsValid( polylineSegment ) )
                            {
                                string polylineSegmentOriginStr = entity.GetValueForKey( idx ).Split( '(' )[^1].Replace( ")", "" );
                                var polylineSegmentOrigin = Helper.ConvertApexOriginToUnity( Helper.StringToVector3( polylineSegmentOriginStr, true ) );
                                polylineSegment.transform.position = last.transform.position + polylineSegmentOrigin;
                                polylineSegment.transform.parent = transformedObj.parent;
                                last = polylineSegment;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if ( classtype == "info_target" )
                {
                    if ( entity.HasKey( "scale" ) && skin != UnityInfo.relativePathCubePrefab )
                    {
                        float scale = entity.GetValueForKey< float >( "scale" );
                        transformedObj.localScale = new Vector3( scale, scale, scale );
                    }
                }

                float width, height;
                switch ( entity.EditorClass )
                {
                    case "info_survival_invalid_end_zone":
                        width = entity.GetValueForKey< float >( "script_radius" ) * 2;
                        transformedObj.localScale = new Vector3( width, 2000, width );
                        break;

                    case "info_survival_loot_zone":
                        switch ( entity.GetValueForKey( "zone_class" ) )
                        {
                            case "zone_low":
                            case "zone_medium":
                            case "zone_high":
                            case "zone_hotzone":
                            case "POI_High":
                            case "POI_Ultra":
                            case "POI_sniper":

                                width = entity.GetValueForKey< float >( "script_radius" ) * 2;
                                height = entity.GetValueForKey< float >( "script_height" );
                                transformedObj.localScale = new Vector3( width, height, width );

                                break;
                        }
                        break;
                }

                entGlobalCountStart++;
            }

            entListGlobalCountStart++;
        }

        EditorUtility.ClearProgressBar();
    }

    private static bool CanBeRotate( string classname )
    {
        if ( classname == "zipline" )
            return false;

        return true;
    }

    public static string SetName( EntitiesData entity )
    {
        if ( !string.IsNullOrEmpty( entity.GetValueForKey( "script_name" ) ) )
            return entity.GetValueForKey( "script_name" );

        if ( !string.IsNullOrEmpty( entity.EditorClass ) )
            return entity.EditorClass;

        return entity.ClassName;
    }

    public static string SetSkin( EntitiesData entity )
    {
        switch ( entity.ClassName )
        {
            case "prop_dynamic":
            case "prop_script":
            case "prop_door":
                if ( !string.IsNullOrEmpty( entity.Model ) )
                {
                    string[] splittedName = entity.Model.Replace( ".rmdl", "" ).Split( '/' );
                    return $"{UnityInfo.relativePathModel}/{splittedName[^1]}_LOD0.fbx";
                }
                break;

            case "script_ref":
                if ( entity.EditorClass == "info_survival_invalid_end_zone" )
                    return $"{UnityInfo.relativePathLodsUtility}/InvalidEndZoneTrigger.prefab";
                if ( entity.EditorClass == "info_survival_loot_zone" )
                    switch ( entity.GetValueForKey( "zone_class" ) )
                    {
                        case "zone_low": return $"{UnityInfo.relativePathLodsUtility}/Zone_Low.prefab";
                        case "zone_medium": return $"{UnityInfo.relativePathLodsUtility}/Zone_Medium.prefab";
                        case "zone_high": return $"{UnityInfo.relativePathLodsUtility}/Zone_High.prefab";
                        case "zone_hotzone": return $"{UnityInfo.relativePathLodsUtility}/Zone_Hotzone.prefab";
                        case "POI_High": return $"{UnityInfo.relativePathLodsUtility}/POI_High.prefab";
                        case "POI_Ultra": return $"{UnityInfo.relativePathLodsUtility}/POI_Ultra.prefab";
                        case "POI_sniper": return $"{UnityInfo.relativePathLodsUtility}/POI_Sniper.prefab";
                    }
                else if ( entity.EditorClass == "info_survival_weapon_location" )
                    return SetRandomWeaponSkin( entity );
                break;

            case "ambient_generic":
            case "soundscape_floor":
            case "trigger_soundscape":
                return $"{UnityInfo.relativePathModel}/editor_ambient_generic_node_LOD0.fbx";

            case "info_target_clientside":
                if ( entity.EditorClass == "info_survival_speaker_location" )
                    return $"{UnityInfo.relativePathModel}/editor_ambient_generic_node_LOD0.fbx";
                break;

            case "traverse":
                return $"{UnityInfo.relativePathModel}/editor_traverse_LOD0.fbx";

            case "info_spawnpoint_human":
            case "info_spawnpoint_human_start":
                return $"{UnityInfo.relativePathModel}/mp_spawn_LOD0.fbx";

            case "info_target":
                switch ( entity.ScriptName )
                {
                    case "apex_screen": return $"{UnityInfo.relativePathModel}/survival_modular_flexscreens_04_LOD0.fbx";
                    case "static_loot_tick_spawn": return $"{UnityInfo.relativePathModel}/drone_frag_loot_LOD0.fbx";
                    case "nfx_loch_point": return $"{UnityInfo.relativePathModel}/nessy_doll_LOD0.fbx";
                }
                break;
        }

        return UnityInfo.relativePathCubePrefab;
    }

    private static string SetRandomWeaponSkin( EntitiesData entity )
    {
        entity.ChangeModelName( WeaponLocationModel[Random.Range( 0, WeaponLocationModel.Length )] );

        string[] splittedName = entity.Model.Replace( ".rmdl", "" ).Split( '/' );
        return $"{UnityInfo.relativePathModel}/{splittedName[^1]}_LOD0.fbx";
    }

    public static void SetColor( GameObject obj, EntitiesData entity )
    {
        var renderer = obj.GetComponent< MeshRenderer >();
        string color;

        switch ( entity.ClassName )
        {
            case "zipline":
            case "zipline_end":
                color = "Yellow";
                break;

            case "script_ref":
                if ( entity.EditorClass == "info_survival_invalid_end_zone" )
                {
                    color = "InvalidEndZone";
                    break;
                }
                goto default;

            default:
                color = "Grey";
                break;
        }

        var newMaterial = AssetDatabase.LoadAssetAtPath< Material >( $"{UnityInfo.relativePathLodsUtility}/Materials/{color}.mat" );

        if ( Helper.IsValid( newMaterial ) ) renderer.material = newMaterial;
    }

    //  ██████╗ ███╗   ███╗ █████╗ ██████╗ ██████╗ ███████╗██╗   ██╗
    //  ██╔══██╗████╗ ████║██╔══██╗██╔══██╗██╔══██╗██╔════╝██║   ██║
    //  ██████╔╝██╔████╔██║███████║██████╔╝██║  ██║█████╗  ██║   ██║
    //  ██╔══██╗██║╚██╔╝██║██╔══██║██╔═══╝ ██║  ██║██╔══╝  ╚██╗ ██╔╝
    //  ██║  ██║██║ ╚═╝ ██║██║  ██║██║     ██████╔╝███████╗ ╚████╔╝ 
    //  ╚═╝  ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝╚═╝     ╚═════╝ ╚══════╝  ╚═══╝  

    public static void RMAPDEV_DrawOnMap()
    {
        string path = EditorUtility.OpenFilePanel( "Import Ent File", "", "ent" );
        if ( string.IsNullOrEmpty( path ) )
            return;

        Code = File.ReadAllText( path );

        ConvertCode();

        var parent = Helper.CreatePath( "info_survival_invalid_end_zone" );

        foreach ( string classtype in EntitiesData.EntitiesGlobal.Keys )
        {
            if ( classtype != "script_ref" )
                continue;

            foreach ( var entity in EntitiesData.EntitiesGlobal[classtype] )
            {
                if ( entity.EditorClass != "info_survival_invalid_end_zone" )
                    continue;

                var obj = Helper.CreateGameObject( "info_survival_invalid_end_zone", $"{UnityInfo.relativePathLodsUtility}/InvalidEndZoneTrigger.prefab", parent );

                if ( !Helper.IsValid( obj ) ) continue;

                var origin = Helper.ConvertApexOriginToUnity( entity.Origin );
                origin.y = 0.0f;

                var transformedObj = obj.transform;
                transformedObj.position = origin;

                float width = entity.GetValueForKey< float >( "script_radius" );
                transformedObj.localScale = new Vector3( width, 2000, width );
            }
        }

        Code = "";
    }

    private static void RMAPDEV_GetAllEntType()
    {
        StringBuilder file = new();

        string outputFolder = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativeRMAPDEVfolder}";
        string output = $"{outputFolder}/EntType.txt";

        List< string > editorclass = new();

        foreach ( string classtype in EntitiesData.EntitiesGlobal.Keys )
        {
            Build.Build.AppendCode( ref file, $"// {classtype}" );

            foreach ( var entity in EntitiesData.EntitiesGlobal[classtype] )
                if ( entity.HasEditorClass() && !editorclass.Contains( entity.EditorClass ) )
                {
                    editorclass.Add( entity.EditorClass );
                    Build.Build.AppendCode( ref file, $"- {entity.EditorClass}" );
                }

            Build.Build.AppendCode( ref file );
        }

        if ( !Directory.Exists( outputFolder ) ) Directory.CreateDirectory( outputFolder );

        File.WriteAllText( output, file.ToString() );
    }
}

/// <summary>
///     Use to save all entities found in apex .ent files
/// </summary>
public class EntitiesData
{
    public EntitiesData( string codeBlock )
    {
        KeyValues = new Dictionary< string, string >();

        // Find all occurences of type => "key" "value"
        string pattern = "\"([^\"]*)\" \"([^\"]*)\"";
        foreach ( Match match in Regex.Matches( codeBlock, pattern ) )
            KeyValues[match.Groups[1].Value] = match.Groups[2].Value;

        if ( IsValid( this ) )
        {
            ClassName = GetValueForKey( "classname" );
            EditorClass = GetValueForKey( "editorclass" );
            Origin = Helper.StringToVector3( GetValueForKey( "origin" ), true );
            Angles = Helper.StringToVector3( GetValueForKey( "angles" ), true );
            ScriptName = GetValueForKey( "script_name" );
            Model = GetValueForModelKey();

            if ( !EntitiesGlobal.ContainsKey( ClassName ) )
                EntitiesGlobal.Add( ClassName, new List< EntitiesData >() );

            EntitiesGlobal[ClassName].Add( this );
        }
    }

    // Contains all entities of type
    public static Dictionary< string, List< EntitiesData > > EntitiesGlobal { get; set; }

    // Contains all KeyValues from one entity
    public Dictionary< string, string > KeyValues { get; set; }

    public string ClassName { get; }
    public string EditorClass { get; }
    public Vector3 Origin { get; }
    public Vector3 Angles { get; }
    public string ScriptName { get; }
    public string Model { get; private set; }

    public bool HasKey( string key )
    {
        return KeyValues.ContainsKey( key );
    }

    public bool HasEditorClass()
    {
        return KeyValues.ContainsKey( "editorclass" );
    }

    public string GetValueForKey( string key )
    {
        return KeyValues.TryGetValue( key, out string value ) ? value : "";
    }

    public T GetValueForKey<T>( string key )
    {
        if ( KeyValues.TryGetValue( key, out string value ) )
            try
            {
                if ( typeof(T) == typeof(float) )
                    return ( T )( object )float.Parse( value, CultureInfo.InvariantCulture );

                return ( T )Convert.ChangeType( value, typeof(T) );
            }
            catch
            {
                return default;
            }

        return default;
    }

    public string GetValueForModelKey()
    {
        return GetValueForKey( "model" );
    }

    public void ChangeModelName( string name )
    {
        Model = name;
    }

    public static bool IsValid( EntitiesData entity )
    {
        return entity.HasKey( "classname" ) && entity.HasKey( "origin" );
    }

    public static int GetTotalEntitiesData()
    {
        return EntitiesGlobal.Values.Sum( list => list.Count );
    }
}