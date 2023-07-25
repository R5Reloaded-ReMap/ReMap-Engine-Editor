
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class EntCodeDiscoverer : EditorWindow
{
    private static string Code = "";

    private static string OutputParent = "Discovered_Code";
    private static string NamedParent = "";

    private static Vector2 Scroll = Vector2.zero;

    [MenuItem( "ReMap/Tools/Ent Code Discoverer", false, 100 )]
    public static void Init()
    {
        EntCodeDiscoverer window = ( EntCodeDiscoverer ) EditorWindow.GetWindow( typeof( EntCodeDiscoverer ), false, "Ent Code Discoverer");
        //window.minSize = new Vector2(300, 290);
        //window.maxSize = new Vector2(300, 290);
        window.Show();
    }

    void OnGUI()
    {
        Scroll = EditorGUILayout.BeginScrollView( Scroll );
        Code = GUILayout.TextArea( Code, GUILayout.ExpandHeight( true ) );
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
            WindowUtility.WindowUtility.CreateButton( "Discover Code", "", () => { ConvertCode(); AddEntsToScene(); } );
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
    /// Convert all code in file to EntitiesData
    /// </summary>
    private static void ConvertCode()
    {
        string[] codes = Code.Replace( "}", "" ).Split( "{" );

        EntitiesData.EntitiesGlobal = new ();
        
        int min = 0; int max = codes.Length; float progress = 0.0f;

        foreach ( string code in codes )
        {
            EditorUtility.DisplayProgressBar( $"Exploring Code", $"Processing... ({min}/{max})", progress );

            new EntitiesData( code );

            progress += 1.0f / max; min++;
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
            {
                zipline.AddRange( ziplineEnd );
            }
            else EntitiesData.EntitiesGlobal[ "zipline" ] = ziplineEnd;

            EntitiesData.EntitiesGlobal.Remove( "zipline_end" );
        }

        int entListGlobalCountStart = 1; int entListGlobalCount = EntitiesData.EntitiesGlobal.Keys.Count;
        int entGlobalCountStart = 1; int entGlobalCount = EntitiesData.GetTotalEntitiesData();

        foreach ( string classtype in EntitiesData.EntitiesGlobal.Keys )
        {
            GameObject parent = Helper.CreatePath( $"{NamedParent}/{classtype} class" );

            int min = 0; int max = EntitiesData.EntitiesGlobal[ classtype ].Count; float progress = 0.0f;

            foreach ( EntitiesData entity in EntitiesData.EntitiesGlobal[ classtype ] )
            {
                EditorUtility.DisplayProgressBar
                (
                    $"Adding Entities To Scene ({entListGlobalCountStart}/{entListGlobalCount})",
                    $"Processing... ( {classtype} => {min}/{max} ) ( Total => {entGlobalCountStart}/{entGlobalCount} )", progress
                );
                
                progress += 1.0f / max; min++;

                string skin = SetSkin( entity );
                GameObject obj = Helper.CreateGameObject
                (
                    entity.HasEditorClass() ? $" {entity.EditorClass}" : entity.ClassName, skin, parent
                );

                Vector3 origin = Helper.ConvertApexOriginToUnity( entity.Origin );

                if ( !Helper.IsValid( obj ) )
                {
                    entGlobalCountStart++;
                    continue;
                }

                Transform transformedObj = obj.transform;
                transformedObj.position = origin;

                if ( skin == UnityInfo.relativePathCubePrefab )
                    SetColor( obj, entity );

                GameObject newParent = obj;

                // Angles
                if ( entity.HasKey( "angles" ) && CanBeRotate( classtype ) )
                {
                    Vector3 angles = Helper.ConvertApexAnglesToUnity( entity.Angles );

                    // Idk why apex screen need to ba at ( 0, 0, 0 )
                    if ( entity.ScriptName == "apex_screen" )
                    {
                        transformedObj.eulerAngles = angles;
                    }
                    else
                    {
                        string model = entity.Model;
                        if ( model == "mdl/creatures/flyer/flyer_kingscanyon_animated.rmdl" ) model = model.Replace( "/creatures/", "/Creatures/" );

                        newParent = Helper.MoveGameObject( obj.name, obj, true );
                        newParent.transform.position = origin;
                        newParent.transform.eulerAngles = angles;
                        transformedObj.localPosition = Vector3.zero;
                        transformedObj.localEulerAngles = LibrarySorter.LibrarySorterWindow.FindAnglesOffset( model );
                    }
                }

                // Guid Link
                if ( entity.HasKey( "link_to_guid_0" ) )
                {
                    GameObject folder = Helper.MoveGameObject( newParent, $"{Helper.CreatePathString( parent, true )}/{entity.GetValueForKey( "link_to_guid_0" )}" );

                    Dictionary< Transform, Vector3 > savedPos = new ();

                    for ( int i = 0; i < newParent.transform.childCount; i++ )
                    {
                        Transform child = newParent.transform.GetChild( i );
                        savedPos.Add( child, child.position );
                    }

                    if ( Helper.IsValid( folder ) )
                    {
                        Helper.SetOriginAndAngles( folder, origin );
                        newParent.transform.position = origin;
                    }

                    foreach ( var data in savedPos )
                    {
                        GameObject restorePos = data.Key.gameObject;
                        if ( restorePos.name != obj.name )
                        {
                            Helper.SetOriginAndAngles( restorePos, data.Value );
                        }
                    }
                }
                else if ( entity.HasKey( "link_guid" ) )
                {
                    Helper.CreatePath( $"{Helper.CreatePathString( parent, true )}/{entity.GetValueForKey( "link_guid" )}", newParent );
                }

                if ( classtype == "zipline" )
                {
                    for ( int i = 0 ; i < 30 ; i++ )
                    {
                        string idx = $"_zipline_rest_point_{i}";
                        if ( entity.HasKey( idx ) )
                        {
                            GameObject restPoint = Helper.CreateGameObject( idx, UnityInfo.relativePathCubePrefab );
                            if ( Helper.IsValid( restPoint ) )
                            {
                                Vector3 restPointOrigin = Helper.ConvertApexOriginToUnity( Helper.StringToVector3( entity.GetValueForKey( idx ), true ) );
                                restPoint.transform.position = restPointOrigin;
                                restPoint.transform.parent = transformedObj.parent;

                                SetColor( restPoint, entity );
                            }
                        }
                        else break;
                    }
                }
                else if ( classtype == "ambient_generic" )
                {
                    if ( entity.HasKey( "soundName" ) )
                    {
                        obj.name = entity.GetValueForKey( "soundName" );
                    }

                    for ( int i = 0 ; i < 130 ; i++ )
                    {
                        string idx = $"polyline_segment_{i}";
                        GameObject last = obj;
                        if ( entity.HasKey( idx ) )
                        {
                            GameObject polylineSegment = Helper.CreateGameObject( idx, $"{UnityInfo.relativePathModel}/editor_ambient_generic_node_LOD0.fbx" );
                            if ( Helper.IsValid( polylineSegment ) )
                            {
                                string polylineSegmentOriginStr = entity.GetValueForKey( idx ).Split( '(' )[^1].Replace( ")", "" );
                                Vector3 polylineSegmentOrigin = Helper.ConvertApexOriginToUnity( Helper.StringToVector3( polylineSegmentOriginStr, true ) );
                                polylineSegment.transform.position = last.transform.position + polylineSegmentOrigin;
                                polylineSegment.transform.parent = transformedObj.parent;
                                last = polylineSegment;
                            }
                        }
                        else break;
                    }
                }
                else if ( classtype == "info_target" )
                {
                    if ( entity.HasKey( "scale" ) )
                    {
                        float scale = entity.GetValueForKey< float >( "scale" );
                        transformedObj.localScale = new Vector3( scale, scale, scale );
                    }
                }

                float width, height;
                switch ( entity.EditorClass )
                {
                    case "info_survival_invalid_end_zone":
                        width = entity.GetValueForKey< float >( "script_radius" );
                        transformedObj.localScale = new Vector3( width, 2000, width );
                        break;
                        
                    case "info_survival_loot_zone":
                        if ( entity.GetValueForKey( "zone_class" ) == "POI_High" )
                        {
                            width = entity.GetValueForKey< float >( "script_radius" );
                            height = entity.GetValueForKey< float >( "script_height" );
                            transformedObj.localScale = new Vector3( width, height, width );
                        }
                        else if ( entity.GetValueForKey( "zone_class" ) == "POI_Sniper" )
                        {
                            width = entity.GetValueForKey< float >( "script_radius" );
                            height = entity.GetValueForKey< float >( "script_height" );
                            transformedObj.localScale = new Vector3( width, height, width );
                        }
                        break;

                    default: break;
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

    public static string SetSkin( EntitiesData entity )
    {
        switch ( entity.ClassName )
        {
            case "prop_dynamic":
            case "prop_door":
                if ( !string.IsNullOrEmpty( entity.Model ) )
                {
                    string[] splittedName = entity.Model.Replace( ".rmdl", "" ).Split( '/' );
                    return $"{UnityInfo.relativePathModel}/{splittedName[^1]}_LOD0.fbx";
                }
                break;

            case "script_ref":
                if ( entity.EditorClass == "info_survival_invalid_end_zone" )
                {
                    return $"{UnityInfo.relativePathLodsUtility}/InvalidEndZoneTrigger.prefab";
                }
                else if ( entity.EditorClass == "info_survival_loot_zone" )
                {
                    string zoneClass = entity.GetValueForKey( "zone_class" );

                    if ( zoneClass == "POI_High" )
                    {
                        return $"{UnityInfo.relativePathLodsUtility}/POI_High.prefab";
                    }
                    else if ( zoneClass == "POI_sniper" )
                    {
                        return $"{UnityInfo.relativePathLodsUtility}/POI_Sniper.prefab";
                    }
                }
                break;

            case "ambient_generic":
            case "soundscape_floor":
            case "trigger_soundscape":
                return $"{UnityInfo.relativePathModel}/editor_ambient_generic_node_LOD0.fbx";

            case "traverse":
                return $"{UnityInfo.relativePathModel}/editor_traverse_LOD0.fbx";

            case "info_spawnpoint_human":
            case "info_spawnpoint_human_start":
                return $"{UnityInfo.relativePathModel}/mp_spawn_LOD0.fbx";

            case "info_target":
                if ( entity.ScriptName == "apex_screen" )
                {
                    return $"{UnityInfo.relativePathModel}/survival_modular_flexscreens_04_LOD0.fbx";
                }
            break;
        }

        return UnityInfo.relativePathCubePrefab;
    }

    public static void SetColor( GameObject obj, EntitiesData entity )
    {
        MeshRenderer renderer = obj.GetComponent< MeshRenderer >(); string color;

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

        Material newMaterial = AssetDatabase.LoadAssetAtPath< Material >( $"{UnityInfo.relativePathLodsUtility}/Materials/{color}.mat" );

        if ( Helper.IsValid( newMaterial ) ) renderer.material = newMaterial;
    }

    //  ██████╗ ███╗   ███╗ █████╗ ██████╗ ██████╗ ███████╗██╗   ██╗
    //  ██╔══██╗████╗ ████║██╔══██╗██╔══██╗██╔══██╗██╔════╝██║   ██║
    //  ██████╔╝██╔████╔██║███████║██████╔╝██║  ██║█████╗  ██║   ██║
    //  ██╔══██╗██║╚██╔╝██║██╔══██║██╔═══╝ ██║  ██║██╔══╝  ╚██╗ ██╔╝
    //  ██║  ██║██║ ╚═╝ ██║██║  ██║██║     ██████╔╝███████╗ ╚████╔╝ 
    //  ╚═╝  ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝╚═╝     ╚═════╝ ╚══════╝  ╚═══╝  

    #if ReMapDev
    [MenuItem( "ReMap/Tools/Draw On Map", false, 100 )]
    #endif
    private static void RMAPDEV_DrawOnMap()
    {
        string path = EditorUtility.OpenFilePanel( "Import Ent File", "", "ent" );
        if ( string.IsNullOrEmpty( path ) )
            return;

        Code = File.ReadAllText( path );

        ConvertCode();

        GameObject parent = Helper.CreatePath( "info_survival_invalid_end_zone" );

        foreach ( string classtype in EntitiesData.EntitiesGlobal.Keys )
        {
            if ( classtype != "script_ref" )
                continue;

            foreach ( EntitiesData entity in EntitiesData.EntitiesGlobal[ classtype ] )
            {
                if ( entity.EditorClass != "info_survival_invalid_end_zone" )
                    continue;

                GameObject obj = Helper.CreateGameObject( "info_survival_invalid_end_zone", $"{UnityInfo.relativePathLodsUtility}/InvalidEndZoneTrigger.prefab", parent );

                if ( !Helper.IsValid( obj ) ) continue;

                Vector3 origin = Helper.ConvertApexOriginToUnity( entity.Origin );
                origin.y = 0.0f;

                Transform transformedObj = obj.transform;
                transformedObj.position = origin;

                float width = entity.GetValueForKey< float >( "script_radius" );
                transformedObj.localScale = new Vector3( width, 2000, width );
            }
        }

        Code = "";
    }

    private static void RMAPDEV_GetAllEntType()
    {
        StringBuilder file = new ();

        string outputFolder = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativeRMAPDEVfolder}";
        string output = $"{outputFolder}/EntType.txt";

        List< string > editorclass = new();

        foreach ( string classtype in EntitiesData.EntitiesGlobal.Keys )
        {
            Build.Build.AppendCode( ref file, $"// {classtype}" );

            foreach ( EntitiesData entity in EntitiesData.EntitiesGlobal[ classtype ] )
            {
                if ( entity.HasEditorClass() && !editorclass.Contains( entity.EditorClass ) )
                {
                    editorclass.Add( entity.EditorClass );
                    Build.Build.AppendCode( ref file, $"- {entity.EditorClass}" );
                }
            }

            Build.Build.AppendCode( ref file, "" );
        }

        if ( !Directory.Exists( outputFolder ) ) Directory.CreateDirectory( outputFolder );

        File.WriteAllText( output, file.ToString() );
    }
}

/// <summary>
/// Use to save all entities found in apex .ent files
/// </summary>
public class EntitiesData
{
    // Contains all entities of type
    public static Dictionary< string, List< EntitiesData > > EntitiesGlobal { get; set; }

    // Contains all KeyValues from one entity
    public Dictionary< string, string > KeyValues { get; set; }

    public string ClassName { get; }
    public string EditorClass { get; }
    public Vector3 Origin { get; }
    public Vector3 Angles { get; }
    public string ScriptName { get; }
    public string Model { get; }

    public EntitiesData( string codeBlock )
    {
        KeyValues = new Dictionary< string, string >();

        // Find all occurences of type => "key" "value"
        string pattern = "\"([^\"]*)\" \"([^\"]*)\"";
        foreach ( Match match in Regex.Matches( codeBlock, pattern ) )
        {
            KeyValues [ match.Groups[1].Value ] = match.Groups[2].Value;
        }

        if ( IsValid( this ) )
        {
            ClassName = GetValueForKey( "classname" );
            EditorClass = GetValueForKey( "editorclass" );
            Origin = Helper.StringToVector3( GetValueForKey( "origin" ), true );
            Angles = Helper.StringToVector3( GetValueForKey( "angles" ), true );
            ScriptName = GetValueForKey( "script_name" );
            Model = GetValueForModelKey();

            if ( !EntitiesGlobal.ContainsKey( ClassName ) )
            {
                EntitiesGlobal.Add( ClassName, new List< EntitiesData >() );
            }

            EntitiesGlobal[ ClassName ].Add( this );
        }
    }

    public bool HasKey( string key )
    {
        return this.KeyValues.ContainsKey( key );
    }

    public bool HasEditorClass()
    {
        return this.KeyValues.ContainsKey( "editorclass" );
    }

    public string GetValueForKey( string key )
    {
        return this.KeyValues.TryGetValue( key, out string value ) ? value : "";
    }

    public T GetValueForKey< T >( string key )
    {
        if ( this.KeyValues.TryGetValue( key, out string value ) )
        {
            try
            {
                if ( typeof( T ) == typeof( float ) )
                {
                    return ( T ) ( object ) float.Parse( value, System.Globalization.CultureInfo.InvariantCulture );
                }

                return ( T ) Convert.ChangeType( value, typeof( T ) );
            }
            catch
            {
                return default( T );
            }
        }
    
        return default( T );
    }

    public string GetValueForModelKey()
    {
        return GetValueForKey( "model" );
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
