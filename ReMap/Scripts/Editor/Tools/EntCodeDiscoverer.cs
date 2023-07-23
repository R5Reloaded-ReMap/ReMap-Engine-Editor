
using System;
using System.Collections.Generic;
using System.IO;
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
            WindowUtility.WindowUtility.CreateButton( "Discover Code", "", () => ConvertCode() );
            WindowUtility.WindowUtility.CreateButton( "Import From File", "", () => ImportCode(), 100 );
            WindowUtility.WindowUtility.CreateButton( "Clear Code", "", () => { Code = ""; }, 100 );
        EditorGUILayout.EndHorizontal();
    }

    private static void ConvertCode()
    {
        string[] codes = Code.Replace( "}", "" ).Split( "{" );

        EntData.EntGlobal = new Dictionary< string, List< EntData > >();
        
        int min = 0; int max = codes.Length; float progress = 0.0f;

        foreach ( string code in codes )
        {
            EditorUtility.DisplayProgressBar( $"Exploring {min}/{max}", $"Processing...", progress );

            EntData entData = new EntData( code );

            progress += 1.0f / max; min++;
        }

        AddEntsToScene();

        //RMAPDEV_GetAllEntType();

        EditorUtility.ClearProgressBar();
    }

    private static void RMAPDEV_GetAllEntType()
    {
        StringBuilder file = new();

        string outputFolder = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativeRMAPDEVfolder}";
        string output = $"{outputFolder}/EntType.txt";

        List< string > editorclass = new();

        foreach ( string classtype in EntData.EntGlobal.Keys )
        {
            Build.Build.AppendCode( ref file, $"// {classtype}" );

            foreach ( EntData entData in EntData.EntGlobal[ classtype ] )
            {
                if ( entData.IsEditorClass() && !editorclass.Contains( entData.EditorClass ) )
                {
                    editorclass.Add( entData.EditorClass );
                    Build.Build.AppendCode( ref file, $"- {entData.EditorClass}" );
                }
            }

            Build.Build.AppendCode( ref file, "" );
        }

        if ( !Directory.Exists( outputFolder ) ) Directory.CreateDirectory( outputFolder );

        File.WriteAllText( output, file.ToString() );
    }

    public static void AddEntsToScene()
    {
        if ( string.IsNullOrEmpty( NamedParent ) ) NamedParent = OutputParent;

        Helper.CreatePath( NamedParent );

        if ( EntData.EntGlobal.ContainsKey( "zipline" ) && EntData.EntGlobal.ContainsKey( "zipline_end" ) )
        {
            EntData.EntGlobal[ "zipline" ].AddRange( EntData.EntGlobal[ "zipline_end" ] );
            EntData.EntGlobal.Remove( "zipline_end" );
        }

        foreach ( string classtype in EntData.EntGlobal.Keys )
        {
            GameObject parent = Helper.CreatePath( $"{NamedParent}/{classtype}" );

            foreach ( EntData entData in EntData.EntGlobal[ classtype ] )
            {
                string skin = SetSkin( entData );
                GameObject obj = Helper.CreateGameObject
                (
                    entData.IsEditorClass() ? $" {entData.EditorClass}" : entData.ClassName, skin, parent
                );
                Vector3 origin = Helper.ConvertApexOriginToUnity( Helper.ExtractVector3( entData.GetValueForKey( "origin" ), true ) );

                if ( !Helper.IsValid( obj ) ) continue;

                Transform transformedObj = obj.transform;
                transformedObj.position = origin;

                if ( skin == UnityInfo.relativePathCubePrefab ) SetColor( obj, entData );

                if ( entData.HasKey( "link_to_guid_0" ) )
                {
                    GameObject newParent = Helper.CreatePath( entData.GetValueForKey( "link_to_guid_0" ) );
                    newParent.transform.SetParent( parent.transform );
                    transformedObj.SetParent( newParent.transform );

                    Dictionary< Transform, Vector3 > savedPos = new Dictionary< Transform, Vector3 >();

                    for ( int i = 0; i < newParent.transform.childCount; i++ )
                    {
                        Transform child = newParent.transform.GetChild( i );
                        savedPos.Add( child, child.position );
                    }

                    if ( Helper.IsValid( parent ) )
                    {
                        newParent.transform.position = origin;
                    }

                    foreach ( var data in savedPos )
                    {
                        data.Key.position = data.Value;
                    }
                }
                else if ( entData.HasKey( "link_guid" ) )
                {
                    GameObject newParent = Helper.CreatePath( entData.GetValueForKey( "link_guid" ) );
                    newParent.transform.SetParent( parent.transform );
                    transformedObj.SetParent( newParent.transform );
                }

                if ( entData.HasKey( "angles" ) && CanBeRotate( classtype ) )
                {
                    Vector3 angles = Helper.ConvertApexAnglesToUnity( Helper.ExtractVector3( entData.GetValueForKey( "angles" ), true ) );
                    GameObject newParent = MoveIntoSubFolder( obj.name, obj );
                    newParent.transform.position = origin;
                    newParent.transform.eulerAngles = angles;
                    transformedObj.position = origin;
                    transformedObj.localEulerAngles = LibrarySorter.LibrarySorterWindow.FindAnglesOffset( entData.GetValueForModelKey() );
                }

                if ( classtype == "zipline" )
                {
                    for ( int i = 0 ; i < 30 ; i++ )
                    {
                        string idx = $"_zipline_rest_point_{i}";
                        if ( entData.HasKey( idx ) )
                        {
                            GameObject restPoint = Helper.CreateGameObject( idx, UnityInfo.relativePathCubePrefab );
                            if ( Helper.IsValid( restPoint ) )
                            {
                                Vector3 restPointOrigin = Helper.ConvertApexOriginToUnity( Helper.ExtractVector3( entData.GetValueForKey( idx ), true ) );
                                restPoint.transform.position = restPointOrigin;
                                restPoint.transform.parent = transformedObj.parent;

                                SetColor( restPoint, entData );
                            }
                        }
                        else break;
                    }
                }
                else if ( classtype == "ambient_generic" )
                {
                    if ( entData.HasKey( "soundName" ) )
                    {
                        obj.name = entData.GetValueForKey( "soundName" );
                    }

                    for ( int i = 0 ; i < 130 ; i++ )
                    {
                        string idx = $"polyline_segment_{i}";
                        GameObject last = obj;
                        if ( entData.HasKey( idx ) )
                        {
                            GameObject polylineSegment = Helper.CreateGameObject( idx, $"{UnityInfo.relativePathModel}/editor_ambient_generic_node_LOD0.fbx" );
                            if ( Helper.IsValid( polylineSegment ) )
                            {
                                string polylineSegmentOriginStr = entData.GetValueForKey( idx ).Split( '(' )[^1].Replace( ")", "" );
                                Vector3 polylineSegmentOrigin = Helper.ConvertApexOriginToUnity( Helper.ExtractVector3( polylineSegmentOriginStr, true ) );
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
                    if ( entData.HasKey( "scale" ) )
                    {
                        float scale = entData.GetValueForKey< float >( "scale" );
                        transformedObj.localScale = new Vector3( scale, scale, scale );
                    }
                }

                if ( entData.IsEditorClass() )
                {
                    float width, height;
                    switch ( entData.EditorClass )
                    {
                        case "info_survival_invalid_end_zone":
                            width = entData.GetValueForKey< float >( "script_radius" );
                            transformedObj.localScale = new Vector3( width, 2000, width );
                            break;
                        
                        case "info_survival_loot_zone":
                            if ( entData.GetValueForKey( "zone_class" ) == "POI_High" )
                            {
                                width = entData.GetValueForKey< float >( "script_radius" );
                                height = entData.GetValueForKey< float >( "script_height" );
                                transformedObj.localScale = new Vector3( width, height, width );
                            }
                            else if ( entData.GetValueForKey( "zone_class" ) == "POI_Sniper" )
                            {
                                width = entData.GetValueForKey< float >( "script_radius" );
                                height = entData.GetValueForKey< float >( "script_height" );
                                transformedObj.localScale = new Vector3( width, height, width );
                            }
                            break;

                        default: break;
                    }
                }
            }
        }
    }

    private static bool CanBeRotate( string classname )
    {
        if ( classname == "zipline" )
            return false;

        return true;
    }

    public static GameObject MoveIntoSubFolder( string name, GameObject obj )
    {
        GameObject subFolder = Helper.CreateGameObject( name, "", obj.transform.parent.gameObject );
        if ( Helper.IsValid( subFolder ) )
        {
            obj.transform.parent = subFolder.transform;
        }
        return subFolder;
    }

    public static string SetSkin( EntData entData )
    {
        string classname = entData.ClassName;
        string editorclass = entData.EditorClass;

        switch ( classname )
        {
            case "prop_dynamic":
            case "prop_door":
                if ( entData.HasModel() )
                {
                    string[] splittedName = entData.GetValueForModelKey().Replace( ".rmdl", "" ).Split( '/' );
                    return $"{UnityInfo.relativePathModel}/{splittedName[^1]}_LOD0.fbx";
                }
                break;

            case "script_ref":
                if ( editorclass == "info_survival_invalid_end_zone" )
                {
                    return $"{UnityInfo.relativePathLodsUtility}/InvalidEndZoneTrigger.prefab";
                }
                else if ( editorclass == "info_survival_loot_zone" )
                {
                    if ( entData.GetValueForKey( "zone_class" ) == "POI_High" )
                    {
                        return $"{UnityInfo.relativePathLodsUtility}/POI_High.prefab";
                    }
                    else if ( entData.GetValueForKey( "zone_class" ) == "POI_Sniper" )
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
                if ( entData.GetValueForKey( "script_name" ) == "apex_screen" )
                {
                    return $"{UnityInfo.relativePathModel}/survival_modular_flexscreens_04_LOD0.fbx";
                }
            break;
        }

        return UnityInfo.relativePathCubePrefab;
    }

    public static void SetColor( GameObject obj, EntData entData )
    {
        MeshRenderer renderer = obj.GetComponent< MeshRenderer >();
        string color;

        switch ( entData.ClassName )
        {
            case "zipline":
            case "zipline_end":
                color = "Yellow";
                break;

            case "script_ref":
                if ( entData.EditorClass == "info_survival_invalid_end_zone" )
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

    private static void ImportCode()
    {
        string path = EditorUtility.OpenFilePanel( "Import Ent File", "", "ent" );
        if ( path.Length == 0 )
            return;

        NamedParent = $"{OutputParent} ({Path.GetFileNameWithoutExtension( path ).Replace( "mp_rr_", "" )})";

        Code = File.ReadAllText( path );

        ConvertCode();

        Code = "";
    }
}

public class EntData
{
    public static Dictionary< string, List< EntData > > EntGlobal { get; set; }

    public Dictionary< string, string > entData { get; set; }

    public string ClassName { get; }
    public string EditorClass { get; }

    public EntData( string codeBlock )
    {
        entData = new Dictionary< string, string >();

        string pattern = "\"([^\"]*)\" \"([^\"]*)\"";
        foreach ( Match match in Regex.Matches( codeBlock, pattern ) )
        {
            entData[ match.Groups[1].Value ] = match.Groups[2].Value;
        }

        if ( IsValid( this ) )
        {
            ClassName = GetValueForKey( "classname" );
            EditorClass = GetValueForKey( "editorclass" );
            if ( !EntGlobal.ContainsKey( ClassName ) )
            {
                EntGlobal.Add( ClassName, new List< EntData >() );
            }
            EntGlobal[ ClassName ].Add( this );
        }
    }

    public bool HasKey( string key )
    {
        return this.entData.ContainsKey( key );
    }

    public bool HasModel()
    {
        return this.entData.ContainsKey( "model" );
    }

    public bool IsEditorClass()
    {
        return this.entData.ContainsKey( "editorclass" );
    }

    public string GetValueForKey( string key )
    {
        return this.entData.TryGetValue( key, out string value ) ? value : "";
    }

    public T GetValueForKey< T >( string key )
    {
        if ( this.entData.TryGetValue( key, out string value ) )
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

    public static bool IsValid( EntData entData )
    {
        return entData.HasKey( "classname" ) && entData.HasKey( "origin" );
    }

    public static bool HaveGuidLink( EntData entData )
    {
        return entData.HasKey( "link_to_guid_0" ) && entData.HasKey( "link_guid" );
    }
}
