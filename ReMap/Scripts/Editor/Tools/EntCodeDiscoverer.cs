
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
        Helper.CreatePath( OutputParent );

        foreach ( string classtype in EntData.EntGlobal.Keys )
        {
            Helper.CreatePath( $"{OutputParent}/{classtype}" );
        }



        /* GameObject obj = Helper.CreateGameObject( entData.ClassName, UnityInfo.relativePathCubePrefab );

        Vector3 originToVec = Helper.ConvertApexOriginToUnity( Helper.ExtractVector3( entData.GetValue( "origin" ), true ) );

        if ( Helper.IsValid( obj ) )
        {
            Transform objt = obj.transform;
            objt.position = originToVec;

            Transform parent = FindParent( entData, originToVec );
            objt.parent = parent;

            SetColor( obj, entData.ClassName );

            for ( int i = 0 ; i < 30 ; i++ )
            {
                string idx = $"_zipline_rest_point_{i}";
                if ( entData.HaveKey( idx ) )
                {
                    GameObject restPoint = Helper.CreateGameObject( idx, UnityInfo.relativePathCubePrefab );
                    if ( Helper.IsValid( restPoint ) )
                    {
                        Vector3 restPointOrigin = Helper.ConvertApexOriginToUnity( Helper.ExtractVector3( entData.GetValue( idx ), true ) );
                        restPoint.transform.position = restPointOrigin;
                        restPoint.transform.parent = parent;

                        SetColor( restPoint, "zipline" );
                    }
                }
                else break;
            }
        } */
    }

    public static Transform FindParent( EntData entData, Vector3 origin )
    {
        string parentName = "Discovered_Code";
        GameObject parent = GameObject.Find( parentName );
        if ( !Helper.IsValid( parent ) ) parent = Helper.CreateGameObject( parentName );

        if ( entData.HaveKey( "link_to_guid_0" ) )
        {
            string subParentName = $"{entData.ClassName} {entData.GetValue( "link_to_guid_0" )}";
            GameObject subParent = GameObject.Find( $"{parentName}/{subParentName}" );

            if ( !Helper.IsValid( subParent ) )
            {
                subParent = Helper.CreateGameObject( subParentName );
            }
            else
            {
                subParent.transform.parent = parent.transform;

                Dictionary< Transform, Vector3 > savedPos = new Dictionary< Transform, Vector3 >();

                for ( int i = 0; i < subParent.transform.childCount; i++ )
                {
                    Transform child = subParent.transform.GetChild( i );
                    savedPos.Add( child, child.position );
                }

                if ( Helper.IsValid( parent ) )
                {
                    subParent.transform.position = origin;
                }

                foreach ( var obj in savedPos )
                {
                    obj.Key.position = obj.Value;
                }
            }

            return subParent.transform;
        }
        else if ( entData.HaveKey( "link_guid" ) )
        {
            string subParentName = $"{entData.ClassName} {entData.GetValue( "link_guid" )}";
            GameObject subParent = GameObject.Find( $"{parentName}/{subParentName}" );

            if ( !Helper.IsValid( subParent ) ) subParent = Helper.CreateGameObject( subParentName );
            subParent.transform.parent = parent.transform;
            return subParent.transform;
        }

        return parent.transform;
    }

    public static void SetColor( GameObject obj, string classname )
    {
        MeshRenderer renderer = obj.GetComponent< MeshRenderer >();

        string color;

        switch ( classname )
        {
            case "zipline":
            case "zipline_end":
                color = "Yellow";
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

    public bool HaveKey( string key )
    {
        return this.entData.ContainsKey( key );
    }

    public string GetValue( string key )
    {
        return this.entData[ key ];
    }

    public static bool IsValid( EntData entData )
    {
        return entData.HaveKey( "classname" ) && entData.HaveKey( "origin" );
    }

    public static bool HaveGuidLink( EntData entData )
    {
        return entData.HaveKey( "link_to_guid_0" ) && entData.HaveKey( "link_guid" );
    }
}
