using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ImportExportJsonTest
{
    static JsonData jsonData = new JsonData();

    static string[] protectedModels = { "_vertical_zipline", "_non_vertical_zipline" };

    //  ██╗███╗   ███╗██████╗  ██████╗ ██████╗ ████████╗
    //  ██║████╗ ████║██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝
    //  ██║██╔████╔██║██████╔╝██║   ██║██████╔╝   ██║   
    //  ██║██║╚██╔╝██║██╔═══╝ ██║   ██║██╔══██╗   ██║   
    //  ██║██║ ╚═╝ ██║██║     ╚██████╔╝██║  ██║   ██║   
    //  ╚═╝╚═╝     ╚═╝╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   

    [ MenuItem( "ReMap Dev Tools/Import/Json", false, 51 ) ]
    public static async void ImportJson()
    {
        var path = EditorUtility.OpenFilePanel("Json Import", "", "json");

        if (path.Length == 0) return;

        EditorUtility.DisplayProgressBar( "Starting Import", "Reading File..." , 0 );
        ReMapConsole.Log( "[Json Import] Reading file: " + path, ReMapConsole.LogType.Warning );

        string json = System.IO.File.ReadAllText( path );
        JsonData jsonData = JsonUtility.FromJson< JsonData >( json );

        // Sort by alphabetical name
        ImportExportJson.SortListByKey( jsonData.Props, x => x.PathString );

        await ImportProps( jsonData.Props );

        ReMapConsole.Log("[Json Import] Finished", ReMapConsole.LogType.Success);

        EditorUtility.ClearProgressBar();
    }

    private static async Task ImportProps( List<PropScriptClassData> propScriptClassData )
    {
        int i = 0; int j = 1;

        int propScriptClassDataCount = propScriptClassData.Count;

        foreach( PropScriptClassData propData in propScriptClassData )
        {
            string importing = "";

            if ( string.IsNullOrEmpty( propData.PathString ) )
            {
                importing = propData.Name;
            } else importing = $"{propData.PathString}/{propData.Name}";

            EditorUtility.DisplayProgressBar( $"Importing propScriptClassData {j}/{propScriptClassDataCount}", $"Importing: {importing}", (i + 1) / (float)propScriptClassDataCount );
            ReMapConsole.Log("[Json Import] Importing: " + propData.Name, ReMapConsole.LogType.Info);

            UnityEngine.Object loadedPrefabResource = ImportExportJson.FindPrefabFromName( propData.Name );
            if ( loadedPrefabResource == null )
            {
                ReMapConsole.Log($"[Json Import] Couldnt find prefab with name of: {propData.Name}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
            GetSetTransformData( obj, propData.Transform );
            GetSetPropScriptData( obj.GetComponent<PropScript>(), propData.PropScript );

            if ( propData.PathString != "" )
            obj.transform.parent = CreatePath( propData.Path );

            await Task.Delay(TimeSpan.FromSeconds(0.001)); i++; j++;
        }
    }


    //  ███████╗██╗  ██╗██████╗  ██████╗ ██████╗ ████████╗
    //  ██╔════╝╚██╗██╔╝██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝
    //  █████╗   ╚███╔╝ ██████╔╝██║   ██║██████╔╝   ██║   
    //  ██╔══╝   ██╔██╗ ██╔═══╝ ██║   ██║██╔══██╗   ██║   
    //  ███████╗██╔╝ ██╗██║     ╚██████╔╝██║  ██║   ██║   
    //  ╚══════╝╚═╝  ╚═╝╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   

    [ MenuItem( "ReMap Dev Tools/Export/Json", false, 51 ) ]
    public static async void ExportJson()
    {
        Helper.FixPropTags();

        var path = EditorUtility.SaveFilePanel( "Json Export", "", "mapexport.json", "json" );

        if (path.Length == 0) return;

        EditorUtility.DisplayProgressBar( "Starting Export", "" , 0 );

        ResetJsonData();
        await ExportProps();

        ReMapConsole.Log( "[Json Export] Writing to file: " + path, ReMapConsole.LogType.Warning );
        string json = JsonUtility.ToJson( jsonData );
        System.IO.File.WriteAllText( path, json );

        ReMapConsole.Log( "[Json Export] Finished.", ReMapConsole.LogType.Success );

        EditorUtility.ClearProgressBar();
    }


    private static async Task ExportProps()
    {
        int i = 0; int j = 1;

        GameObject[] objectsData = Helper.GetObjArrayWithEnum( ObjectType.Prop );

        int objectsCount = objectsData.Length;

        foreach( GameObject obj in objectsData )
        {
            PropScript component = obj.GetComponent<PropScript>();

            if ( component == null )
            {
                ReMapConsole.Log( $"[Json Export] Missing {component.GetType().Name} on: " + obj.name, ReMapConsole.LogType.Error );
                continue;
            }

            string exporting = ""; string objPath = FindPathString( obj );

            if ( string.IsNullOrEmpty( objPath ) )
            {
                exporting = obj.name;
            } else exporting = $"{objPath}/{obj.name}";

            ReMapConsole.Log("[Json Export] Exporting: " + obj.name, ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar($"Exporting propScriptClassData {j}/{objectsCount}", $"Exporting: {exporting}", (i + 1) / (float)objectsCount);

            PropScriptClassData propScriptClassData = new PropScriptClassData();
            propScriptClassData.Name = obj.name.Split(char.Parse(" "))[0];
            propScriptClassData.Transform = GetSetTransformData( obj );
            propScriptClassData.PropScript = GetSetPropScriptData( component, propScriptClassData.PropScript );
            propScriptClassData.PathString = objPath;
            propScriptClassData.Path = FindPath( obj );

            if ( IsValidPath( objPath ) ) jsonData.Props.Add( propScriptClassData );

            await Task.Delay(TimeSpan.FromSeconds(0.001)); i++; j++;
        }
    }


    //  ██╗   ██╗████████╗██╗██╗     ██╗████████╗██╗   ██╗
    //  ██║   ██║╚══██╔══╝██║██║     ██║╚══██╔══╝╚██╗ ██╔╝
    //  ██║   ██║   ██║   ██║██║     ██║   ██║    ╚████╔╝ 
    //  ██║   ██║   ██║   ██║██║     ██║   ██║     ╚██╔╝  
    //  ╚██████╔╝   ██║   ██║███████╗██║   ██║      ██║   
    //   ╚═════╝    ╚═╝   ╚═╝╚══════╝╚═╝   ╚═╝      ╚═╝   

    private static void ResetJsonData()
    {
        jsonData = new JsonData();
        jsonData.Props = new List<PropScriptClassData>();
    }

    private static TransformData GetSetTransformData( GameObject obj, TransformData data = null )
    {
        if ( data == null ) // if data is null, get the transformation data
        {
            data = new TransformData();
            data.position = obj.transform.position;
            data.eulerAngles = obj.transform.eulerAngles;
            data.localScale = obj.transform.localScale;
        }
        else // otherwise, define the transformation data provided
        {
            obj.transform.position = data.position;
            obj.transform.eulerAngles = data.eulerAngles;
            obj.transform.localScale = data.localScale;
        }
    
        return data;
    }

    private static PropScriptData GetSetPropScriptData( PropScript component, PropScriptData data = null )
    {
        if ( data == null ) // if data is null, get the propscript data
        {
            data = new PropScriptData();
            data.allowMantle = component.allowMantle;
            data.fadeDistance = component.fadeDistance;
            data.realmID = component.realmID;
            data.parameters = component.parameters;
            data.customParameters = component.customParameters;
        }
        else // otherwise, define the propscript data provided
        {
            component.allowMantle = data.allowMantle;
            component.fadeDistance = data.fadeDistance;
            component.realmID = data.realmID;
            component.parameters = data.parameters;
            component.customParameters = data.customParameters;
        }

        return data;
    }

    private static List< PathClass > FindPath( GameObject obj )
    {
        List< GameObject > parents = new List< GameObject >();
        List< PathClass > pathList = new List< PathClass >();
        GameObject currentParent = obj;

        // find all parented game objects
        while ( currentParent.transform.parent != null )
        {
            if ( currentParent != obj ) parents.Add( currentParent );
            currentParent = currentParent.transform.parent.gameObject;
        }

        if ( currentParent != obj ) parents.Add(currentParent);

        foreach ( GameObject parent in parents )
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
        List< GameObject > parents = new List< GameObject >();
        string path = "";
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
            if ( string.IsNullOrEmpty( path ) )
            {
                path = $"{parent.name}";
            }
            else path = $"{path}/{parent.name}";
        }

        return path;
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
}
