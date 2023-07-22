
using System;
using System.Collections.Generic;
using System.IO;
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

        EditorUtility.ClearProgressBar();
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
                    entData.IsEditorClass() ? $" {entData.GetValue( "editorclass" )}" : entData.ClassName, skin, parent
                );
                Vector3 origin = Helper.ConvertApexOriginToUnity( Helper.ExtractVector3( entData.GetValue( "origin" ), true ) );

                if ( !Helper.IsValid( obj ) ) continue;

                Transform transformedObj = obj.transform;
                transformedObj.position = origin;

                if ( entData.HasKey( "angles" ) )
                {
                    string model = entData.HasKey( "model" ) ? entData.GetValue( "model" ) : "";
                    Vector3 offset = LibrarySorter.LibrarySorterWindow.FindAnglesOffset( model );
                    Vector3 angles = Helper.ConvertApexAnglesToUnity( Helper.ExtractVector3( entData.GetValue( "angles" ), true ) );
                    
                    transformedObj.eulerAngles = new Vector3( ( angles.x + offset.x ) % 360, ( angles.y + offset.y ) % 360, ( angles.z + offset.z ) % 360 );
                }

                if ( skin == UnityInfo.relativePathCubePrefab ) SetColor( obj, entData );

                if ( entData.HasKey( "link_to_guid_0" ) )
                {
                    GameObject newParent = Helper.CreatePath( entData.GetValue( "link_to_guid_0" ) );
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
                    GameObject newParent = Helper.CreatePath( entData.GetValue( "link_guid" ) );
                    newParent.transform.SetParent( parent.transform );
                    transformedObj.SetParent( newParent.transform );
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
                                Vector3 restPointOrigin = Helper.ConvertApexOriginToUnity( Helper.ExtractVector3( entData.GetValue( idx ), true ) );
                                restPoint.transform.position = restPointOrigin;
                                restPoint.transform.parent = transformedObj.parent;

                                SetColor( restPoint, entData );
                            }
                        }
                        else break;
                    }
                }

                if ( entData.HasKey( "editorclass" ) )
                {
                    if ( entData.GetValue( "editorclass" ) == "info_survival_invalid_end_zone" )
                    {
                        float width = entData.GetValue< float >( "script_radius" );
                        transformedObj.localScale = new Vector3( width, 2000, width );
                    }
                }
            }
        }
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
        string classname = entData.ClassName; string editorclass = "";

        bool hasEditorClass = entData.IsEditorClass();

        if ( hasEditorClass ) editorclass = entData.GetValue( "editorclass" );

        switch ( classname )
        {
            case "prop_dynamic":
            case "prop_door":
                if ( entData.HasKey( "model" ) )
                {
                    string[] splittedName = entData.GetValue( "model" ).Replace( ".rmdl", "" ).Split( '/' );
                    return $"{UnityInfo.relativePathModel}/{splittedName[splittedName.Length - 1]}_LOD0.fbx";
                }
                else return UnityInfo.relativePathCubePrefab;

            case "script_ref":
                if ( hasEditorClass )
                {
                    switch ( editorclass )
                    {
                        case "info_survival_invalid_end_zone":
                            return $"{UnityInfo.relativePathLodsUtility}/InvalidEndZoneTrigger.prefab";

                        default: return UnityInfo.relativePathCubePrefab;
                    }
                }
                else return UnityInfo.relativePathCubePrefab;

            case "ambient_generic":
            case "soundscape_floor":
            case "trigger_soundscape":
                return $"{UnityInfo.relativePathModel}/editor_ambient_generic_node_LOD0.fbx";

            case "traverse":
                return $"{UnityInfo.relativePathModel}/editor_traverse_LOD0.fbx";

            case "info_spawnpoint_human":
            case "info_spawnpoint_human_start":
                return $"{UnityInfo.relativePathModel}/mp_spawn_LOD0.fbx";

            default: return UnityInfo.relativePathCubePrefab;
        }
    }

    public static void SetColor( GameObject obj, EntData entData )
    {
        MeshRenderer renderer = obj.GetComponent< MeshRenderer >();

        string color; string classname = entData.ClassName; string editorclass = "";

        bool hasEditorClass = entData.IsEditorClass();

        if ( hasEditorClass ) editorclass = entData.GetValue( "editorclass" );

        switch ( classname )
        {
            case "zipline":
            case "zipline_end":
                color = "Yellow";
                break;

            case "script_ref":
                if ( hasEditorClass )
                {
                    switch ( editorclass )
                    {
                        case "info_survival_invalid_end_zone":
                            color = "InvalidEndZone";
                            break;

                        default:
                            color = "Grey";
                        break;
                    }
                }
                else color = "Grey";
                break;

            default:
                color = "Grey";
            break;
        }

        Material newMaterial = AssetDatabase.LoadAssetAtPath< Material >( $"Assets/ReMap/Lods - Dont use these/Utility/Materials/{color}.mat" );

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
            ClassName = GetValue( "classname" );
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

    public bool IsEditorClass()
    {
        return this.entData.ContainsKey( "editorclass" );
    }

    public string GetValue( string key )
    {
        return this.entData[ key ];
    }

    public T GetValue< T >( string key )
    {
        if ( this.entData.TryGetValue( key, out string value ) )
        {
            try
            {
                return ( T ) Convert.ChangeType( value, typeof( T ) );
            }
            catch
            {
                return default( T );
            }
        }
    
        return default( T );
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
