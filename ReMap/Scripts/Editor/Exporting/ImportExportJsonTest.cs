using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public enum GetSetData
{
    Get = 0,
    Set = 1
}

/// <summary>
/// Class used for import/export using json file
/// </summary>
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
    #if ReMapDev
    [ MenuItem( "ReMap Dev Tools/Import/Json", false, 51 ) ]
    public static async void ImportJson()
    {
        var path = EditorUtility.OpenFilePanel( "Json Import", "", "json" );

        if ( path.Length == 0 ) return;

        EditorUtility.DisplayProgressBar( "Starting Import", "Reading File..." , 0 );
        ReMapConsole.Log( "[Json Import] Reading file: " + path, ReMapConsole.LogType.Warning );

        string json = System.IO.File.ReadAllText( path );
        JsonData jsonData = JsonUtility.FromJson< JsonData >( json );

        // Sort by alphabetical name
        ImportExportJson.SortListByKey( jsonData.Props, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.Ziplines, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.LinkedZiplines, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.VerticalZipLines, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.NonVerticalZipLines, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.SingleDoors, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.DoubleDoors, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.HorzDoors, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.VerticalDoors, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.Buttons, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.Jumppads, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.LootBins, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.WeaponRacks, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.Triggers, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.BubbleShields, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.SpawnPoints, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.TextInfoPanels, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.FuncWindowHints, x => x.PathString );
        ImportExportJson.SortListByKey( jsonData.Sounds, x => x.PathString );


        await ImportObjectsWithEnum( ObjectType.Prop, jsonData.Props );
        await ImportObjectsWithEnum( ObjectType.ZipLine, jsonData.Ziplines );
        await ImportObjectsWithEnum( ObjectType.LinkedZipline, jsonData.LinkedZiplines );
        await ImportObjectsWithEnum( ObjectType.VerticalZipLine, jsonData.VerticalZipLines );
        await ImportObjectsWithEnum( ObjectType.NonVerticalZipLine, jsonData.NonVerticalZipLines );
        await ImportObjectsWithEnum( ObjectType.SingleDoor, jsonData.SingleDoors );
        await ImportObjectsWithEnum( ObjectType.DoubleDoor, jsonData.DoubleDoors );
        await ImportObjectsWithEnum( ObjectType.HorzDoor, jsonData.HorzDoors );
        await ImportObjectsWithEnum( ObjectType.VerticalDoor, jsonData.VerticalDoors );
        await ImportObjectsWithEnum( ObjectType.Button, jsonData.Buttons );
        await ImportObjectsWithEnum( ObjectType.Jumppad, jsonData.Jumppads );
        await ImportObjectsWithEnum( ObjectType.LootBin, jsonData.LootBins );
        await ImportObjectsWithEnum( ObjectType.WeaponRack, jsonData.WeaponRacks );
        await ImportObjectsWithEnum( ObjectType.Trigger, jsonData.Triggers );
        await ImportObjectsWithEnum( ObjectType.BubbleShield, jsonData.BubbleShields );
        await ImportObjectsWithEnum( ObjectType.SpawnPoint, jsonData.SpawnPoints );
        await ImportObjectsWithEnum( ObjectType.TextInfoPanel, jsonData.TextInfoPanels );
        await ImportObjectsWithEnum( ObjectType.FuncWindowHint, jsonData.FuncWindowHints );
        await ImportObjectsWithEnum( ObjectType.Sound, jsonData.Sounds );

        ReMapConsole.Log( "[Json Import] Finished", ReMapConsole.LogType.Success );

        EditorUtility.ClearProgressBar();
    }
    #endif
    private static async Task ImportObjectsWithEnum< T >( ObjectType objectType, List< T > listType ) where T : class
    {
        int i = 0; int j = 1;

        int objectsCount = listType.Count;

        T classData = Activator.CreateInstance( typeof( T ) ) as T; GameObject obj;

        foreach( T objData in listType )
        {
            switch ( classData )
            {
                case PropClassData data: // Props
                    data = ( PropClassData )( object ) objData;
                    obj = ProcessImportClassData( data, data.Name, objectType, i, j, objectsCount );
                    break;

                case ZipLineClassData data: // Ziplines
                    data = ( ZipLineClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_zipline", objectType, i, j, objectsCount );
                    break;

                case LinkedZipLinesClassData data: // Linked Ziplines
                    data = ( LinkedZipLinesClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_linked_zipline", objectType, i, j, objectsCount );
                    break;

                case VerticalZipLineClassData data: // Vertical Ziplines
                    data = ( VerticalZipLineClassData )( object ) objData;
                    obj = ProcessImportClassData( data, data.Name, objectType, i, j, objectsCount );
                    break;

                case NonVerticalZipLineClassData data: // Non Vertical ZipLines
                    data = ( NonVerticalZipLineClassData )( object ) objData;
                    obj = ProcessImportClassData( data, data.Name, objectType, i, j, objectsCount );
                    break;

                case SingleDoorClassData data: // Single Doors
                    data = ( SingleDoorClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_single_door", objectType, i, j, objectsCount );
                    break;

                case DoubleDoorClassData data: // Double Doors
                    data = ( DoubleDoorClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_double_door", objectType, i, j, objectsCount );
                    break;

                case HorzDoorClassData data: // Horizontal Doors
                    data = ( HorzDoorClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_sliding_door", objectType, i, j, objectsCount );
                    break;

                case VerticalDoorClassData data: // Vertical Doors
                    data = ( VerticalDoorClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_vertical_door", objectType, i, j, objectsCount );
                    break;

                case ButtonClassData data: // Bouttons
                    data = ( ButtonClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_button", objectType, i, j, objectsCount );
                    break;

                case JumppadClassData data: // Jumppads
                    data = ( JumppadClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_jumppad", objectType, i, j, objectsCount );
                    break;

                case LootBinClassData data: // Loot Bins
                    data = ( LootBinClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "custom_lootbin", objectType, i, j, objectsCount );
                    break;

                case WeaponRackClassData data: // Weapon Racks
                    data = ( WeaponRackClassData )( object ) objData;
                    obj = ProcessImportClassData( data, data.Name, objectType, i, j, objectsCount );
                    break;

                case TriggerClassData data: // Triggers
                    data = ( TriggerClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "", objectType, i, j, objectsCount );
                    break;

                case BubbleShieldClassData data: // Bubbles Shield
                    data = ( BubbleShieldClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "", objectType, i, j, objectsCount );
                    break;

                case SpawnPointClassData data: // Spawn Points
                    data = ( SpawnPointClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "", objectType, i, j, objectsCount );
                    break;

                case TextInfoPanelClassData data: // Text Info Panels
                    data = ( TextInfoPanelClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "", objectType, i, j, objectsCount );
                    break;

                case FuncWindowHintClassData data: // Window Hints
                    data = ( FuncWindowHintClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "", objectType, i, j, objectsCount );
                    break;

                case SoundClassData data: // Sounds
                    data = ( SoundClassData )( object ) objData;
                    obj = ProcessImportClassData( data, "", objectType, i, j, objectsCount );
                    break;

                default: break;
            }

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) ); i++; j++;
        }
    }


    //  ███████╗██╗  ██╗██████╗  ██████╗ ██████╗ ████████╗
    //  ██╔════╝╚██╗██╔╝██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝
    //  █████╗   ╚███╔╝ ██████╔╝██║   ██║██████╔╝   ██║   
    //  ██╔══╝   ██╔██╗ ██╔═══╝ ██║   ██║██╔══██╗   ██║   
    //  ███████╗██╔╝ ██╗██║     ╚██████╔╝██║  ██║   ██║   
    //  ╚══════╝╚═╝  ╚═╝╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   
    #if ReMapDev
    [ MenuItem( "ReMap Dev Tools/Export/Json", false, 51 ) ]
    public static async void ExportJson()
    {
        Helper.FixPropTags();

        var path = EditorUtility.SaveFilePanel( "Json Export", "", "mapexport.json", "json" );

        if ( path.Length == 0 ) return;

        EditorUtility.DisplayProgressBar( "Starting Export", "" , 0 );

        ResetJsonData();
        await ExportObjectsWithEnum( ObjectType.Prop, jsonData.Props );
        await ExportObjectsWithEnum( ObjectType.ZipLine, jsonData.Ziplines );
        await ExportObjectsWithEnum( ObjectType.LinkedZipline, jsonData.LinkedZiplines );
        await ExportObjectsWithEnum( ObjectType.VerticalZipLine, jsonData.VerticalZipLines );
        await ExportObjectsWithEnum( ObjectType.NonVerticalZipLine, jsonData.NonVerticalZipLines );
        await ExportObjectsWithEnum( ObjectType.SingleDoor, jsonData.SingleDoors );
        await ExportObjectsWithEnum( ObjectType.DoubleDoor, jsonData.DoubleDoors );
        await ExportObjectsWithEnum( ObjectType.HorzDoor, jsonData.HorzDoors );
        await ExportObjectsWithEnum( ObjectType.VerticalDoor, jsonData.VerticalDoors );
        await ExportObjectsWithEnum( ObjectType.Button, jsonData.Buttons );
        await ExportObjectsWithEnum( ObjectType.Jumppad, jsonData.Jumppads );
        await ExportObjectsWithEnum( ObjectType.LootBin, jsonData.LootBins );
        await ExportObjectsWithEnum( ObjectType.WeaponRack, jsonData.WeaponRacks );
        await ExportObjectsWithEnum( ObjectType.Trigger, jsonData.Triggers );
        await ExportObjectsWithEnum( ObjectType.BubbleShield, jsonData.BubbleShields );
        await ExportObjectsWithEnum( ObjectType.SpawnPoint, jsonData.SpawnPoints );
        await ExportObjectsWithEnum( ObjectType.TextInfoPanel, jsonData.TextInfoPanels );
        await ExportObjectsWithEnum( ObjectType.FuncWindowHint, jsonData.FuncWindowHints );
        await ExportObjectsWithEnum( ObjectType.Sound, jsonData.Sounds );

        ReMapConsole.Log( "[Json Export] Writing to file: " + path, ReMapConsole.LogType.Warning );
        string json = JsonUtility.ToJson( jsonData );
        System.IO.File.WriteAllText( path, json );

        ReMapConsole.Log( "[Json Export] Finished.", ReMapConsole.LogType.Success );

        EditorUtility.ClearProgressBar();
    }
    #endif

    private static async Task ExportObjectsWithEnum< T >( ObjectType objectType, List< T > listType ) where T : class
    {
        int i = 0; int j = 1;

        GameObject[] objectsData = Helper.GetObjArrayWithEnum( objectType );

        int objectsCount = objectsData.Length;
        string objType = Helper.GetObjNameWithEnum( objectType );
        string objName;

        foreach( GameObject obj in objectsData )
        {
            objName = obj.name;

            if ( Helper.GetComponentByEnum( obj, objectType ) == null )
            {
                ReMapConsole.Log( $"[Json Export] Missing Component on: " + objName, ReMapConsole.LogType.Error );
                continue;
            }

            string exporting = ""; string objPath = FindPathString( obj );

            if ( string.IsNullOrEmpty( objPath ) )
            {
                exporting = objName;
            } else exporting = $"{objPath}/{objName}";

            ReMapConsole.Log( "[Json Export] Exporting: " + objName, ReMapConsole.LogType.Info );
            EditorUtility.DisplayProgressBar( $"Exporting {objType} {j}/{objectsCount}", $"Exporting: {exporting}", ( i + 1 ) / ( float )objectsCount );

            T classData = Activator.CreateInstance( typeof( T ) ) as T;

            switch ( classData )
            {
                case PropClassData data: // Props
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case ZipLineClassData data: // Ziplines
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case LinkedZipLinesClassData data: // Linked Ziplines
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case VerticalZipLineClassData data: // Vertical Ziplines
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case NonVerticalZipLineClassData data: // Non Vertical ZipLines
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case SingleDoorClassData data: // Single Doors
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case DoubleDoorClassData data: // Double Doors
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case HorzDoorClassData data: // Horizontal Doors
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case VerticalDoorClassData data: // Vertical Doors
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case ButtonClassData data: // Bouttons
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case JumppadClassData data: // Jumppads
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case LootBinClassData data: // Loot Bins
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case WeaponRackClassData data: // Weapon Racks
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case TriggerClassData data: // Triggers
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case BubbleShieldClassData data: // Bubbles Shield
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case SpawnPointClassData data: // Spawn Points
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case TextInfoPanelClassData data: // Text Info Panels
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case FuncWindowHintClassData data: // Window Hints
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;
                case SoundClassData data: // Sounds
                    ProcessExportClassData( data, obj, objPath, objectType );
                    break;

                default: break;
            }

            if ( IsValidPath( objPath ) ) listType.Add( classData );

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) ); i++; j++;
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
        jsonData.Props = new List< PropClassData >();
        jsonData.Ziplines = new List< ZipLineClassData >();
        jsonData.LinkedZiplines = new List< LinkedZipLinesClassData >();
        jsonData.VerticalZipLines = new List< VerticalZipLineClassData >();
        jsonData.NonVerticalZipLines = new List< NonVerticalZipLineClassData >();
        jsonData.SingleDoors = new List< SingleDoorClassData >();
        jsonData.DoubleDoors = new List< DoubleDoorClassData >();
        jsonData.HorzDoors = new List< HorzDoorClassData >();
        jsonData.VerticalDoors = new List< VerticalDoorClassData >();
        jsonData.Buttons = new List< ButtonClassData >();
        jsonData.Jumppads = new List< JumppadClassData >();
        jsonData.LootBins = new List< LootBinClassData >();
        jsonData.WeaponRacks = new List< WeaponRackClassData >();
        jsonData.Triggers = new List< TriggerClassData >();
        jsonData.BubbleShields = new List< BubbleShieldClassData >();
        jsonData.SpawnPoints = new List< SpawnPointClassData >();
        jsonData.TextInfoPanels = new List< TextInfoPanelClassData >();
        jsonData.FuncWindowHints = new List< FuncWindowHintClassData >();
        jsonData.Sounds = new List< SoundClassData >();
    }

    /* Todo objects left:
    Button,
    Jumppad,
    LootBin,
    WeaponRack,
    Trigger,
    BubbleShield,
    SpawnPoint,
    TextInfoPanel,
    FuncWindowHint,
    Sound
    */

    private static TransformData GetSetTransformData( GameObject obj, TransformData data = null )
    {
        if ( data == null ) // if data is null, get the transformation data
        {
            data = new TransformData();
            data.position = obj.transform.position;
            data.eulerAngles = obj.transform.eulerAngles;
            data.localScale = obj.transform.localScale;

            return data;
        }
        else // otherwise, define the transformation data provided
        {
            obj.transform.position = data.position;
            obj.transform.eulerAngles = data.eulerAngles;
            obj.transform.localScale = data.localScale;

            return null;
        }
    }

    private static void GetSetScriptData< T >( GameObject obj, T scriptData, ObjectType dataType, GetSetData getSet ) where T : class
    {
        T classData = Activator.CreateInstance( typeof( T ) ) as T;

        if ( getSet == GetSetData.Get )
        {
            switch ( classData )
            {
                case PropClassData data: // Props
                    data = ( PropClassData )( object ) scriptData;
                    PropScript propScript = ( PropScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( propScript, data );
                    data.Name = GetObjName( obj );
                    break;

                case ZipLineClassData data: // Ziplines
                    data = ( ZipLineClassData )( object ) scriptData;
                    DrawZipline drawZipline = ( DrawZipline ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( drawZipline, data, new[] { "zipline_start", "zipline_end" }.ToList() );
                    data.Zipline_start = drawZipline.zipline_start.position;
                    data.Zipline_end = drawZipline.zipline_end.position;
                    break;

                case LinkedZipLinesClassData data: // Linked Ziplines
                    data = ( LinkedZipLinesClassData )( object ) scriptData;
                    LinkedZiplineScript linkedZiplineScript = ( LinkedZiplineScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( linkedZiplineScript, data );
                    data.Nodes = new List< Vector3 >();
                    foreach ( Transform nodes in obj.transform ) data.Nodes.Add( nodes.gameObject.transform.position );
                    break;

                case VerticalZipLineClassData data: // Vertical Ziplines
                    data = ( VerticalZipLineClassData )( object ) scriptData;
                    DrawVerticalZipline drawVerticalZipline = ( DrawVerticalZipline ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( drawVerticalZipline, data, new[] { "Panels", "Name" }.ToList() );
                    data.Name = GetObjName( obj );
                    data.Panels = new List< VCPanelsClassData >();
                    foreach ( GameObject panel in drawVerticalZipline.Panels )
                    {
                        VCPanelsClassData panelClass = new VCPanelsClassData();
                        panelClass.Model = panel.name;
                        panelClass.TransformData = GetSetTransformData( panel, panelClass.TransformData );
                        panelClass.Path = FindPath( panel );
                        panelClass.PathString = FindPathString( panel );
                        data.Panels.Add( panelClass );
                    }
                    break;

                case NonVerticalZipLineClassData data: // Non Vertical ZipLines
                    data = ( NonVerticalZipLineClassData )( object ) scriptData;
                    DrawNonVerticalZipline drawNonVerticalZipline = ( DrawNonVerticalZipline ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( drawNonVerticalZipline, data, new[] { "Panels", "Name", "ZiplineStart", "ZiplineEnd" }.ToList() );
                    data.Name = GetObjName( obj );
                    data.ZiplineStart = GetSetTransformData( obj.transform.Find( "support_start" ).gameObject, data.ZiplineStart );
                    data.ZiplineEnd = GetSetTransformData( obj.transform.Find( "support_end" ).gameObject, data.ZiplineEnd );
                    data.Panels = new List< VCPanelsClassData >();
                    foreach ( GameObject panel in drawNonVerticalZipline.Panels )
                    {
                        VCPanelsClassData panelClass = new VCPanelsClassData();
                        panelClass.Model = panel.name;
                        panelClass.TransformData = GetSetTransformData( panel, panelClass.TransformData );
                        panelClass.Path = FindPath( panel );
                        panelClass.PathString = FindPathString( panel );
                        data.Panels.Add( panelClass );
                    }
                    break;

                case SingleDoorClassData data: // Single Doors
                    data = ( SingleDoorClassData )( object ) scriptData;
                    DoorScript doorScriptSingle = ( DoorScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( doorScriptSingle, data );
                    break;

                case DoubleDoorClassData data: // Double Doors
                    data = ( DoubleDoorClassData )( object ) scriptData;
                    DoorScript doorScriptDouble = ( DoorScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( doorScriptDouble, data );
                    break;

                case HorzDoorClassData data: // Horizontal Doors
                    data = ( HorzDoorClassData )( object ) scriptData;
                    HorzDoorScript horzDoorScript = ( HorzDoorScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( horzDoorScript, data );
                    break;

                case VerticalDoorClassData data: // Vertical Doors
                    data = ( VerticalDoorClassData )( object ) scriptData;
                    VerticalDoorScript verticalDoorScript = ( VerticalDoorScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( verticalDoorScript, data );
                    break;

                case ButtonClassData data: // Bouttons
                    data = ( ButtonClassData )( object ) scriptData;
                    ButtonScripting buttonScripting = ( ButtonScripting ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( buttonScripting, data );
                    break;

                case JumppadClassData data: // Jumppads
                    data = ( JumppadClassData )( object ) scriptData;
                    PropScript propScriptJumppad = ( PropScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( propScriptJumppad, data );
                    break;

                case LootBinClassData data: // Loot Bins
                    data = ( LootBinClassData )( object ) scriptData;
                    LootBinScript lootBinScript = ( LootBinScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( lootBinScript, data );
                    break;

                case WeaponRackClassData data: // Weapon Racks
                    data = ( WeaponRackClassData )( object ) scriptData;
                    WeaponRackScript weaponRackScript = ( WeaponRackScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( weaponRackScript, data, new[] { "Name" }.ToList() );
                    data.Name = GetObjName( obj );
                    break;

                case TriggerClassData data: // Triggers
                    data = ( TriggerClassData )( object ) scriptData;
                    TriggerScripting triggerScripting = ( TriggerScripting ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( triggerScripting, data );
                    break;

                case BubbleShieldClassData data: // Bubbles Shield
                    data = ( BubbleShieldClassData )( object ) scriptData;
                    BubbleScript bubbleScript = ( BubbleScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( bubbleScript, data );
                    break;

                case SpawnPointClassData data: // Spawn Points
                    data = ( SpawnPointClassData )( object ) scriptData;
                    SpawnPointScript spawnPointScript = ( SpawnPointScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( spawnPointScript, data );
                    break;

                case TextInfoPanelClassData data: // Text Info Panels
                    data = ( TextInfoPanelClassData )( object ) scriptData;
                    TextInfoPanelScript textInfoPanelScript = ( TextInfoPanelScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( textInfoPanelScript, data );
                    break;

                case FuncWindowHintClassData data: // Window Hints
                    data = ( FuncWindowHintClassData )( object ) scriptData;
                    WindowHintScript windowHintScript = ( WindowHintScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( windowHintScript, data );
                    break;

                case SoundClassData data: // Sounds
                    data = (SoundClassData  )( object ) scriptData;
                    SoundScript soundScript = ( SoundScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( soundScript, data );
                    break;

                default: break;
            }
        }
        else
        {
            switch ( classData )
            {
                case PropClassData data: // Props
                    data = ( PropClassData )( object ) scriptData;
                    PropScript propScript = ( PropScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, propScript );
                    break;

                case ZipLineClassData data: // Ziplines
                    data = ( ZipLineClassData )( object ) scriptData;
                    DrawZipline drawZipline = ( DrawZipline ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, drawZipline, new[] { "Zipline_start", "Zipline_end" }.ToList() );
                    drawZipline.zipline_start.position = data.Zipline_start;
                    drawZipline.zipline_end.position = data.Zipline_end;
                    break;

                case LinkedZipLinesClassData data: // Linked Ziplines
                    data = ( LinkedZipLinesClassData )( object ) scriptData;
                    obj.AddComponent< DrawLinkedZipline >();
                    obj.AddComponent< LinkedZiplineScript >();
                    LinkedZiplineScript linkedZiplineScript = ( LinkedZiplineScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, linkedZiplineScript, new[] { "zipline_start", "zipline_end" }.ToList() );
                    foreach ( Vector3 nodesPos in data.Nodes )
                    {
                        GameObject nodes = new GameObject( "zipline_node" );
                        nodes.transform.position = nodesPos;
                        nodes.transform.parent = obj.transform;
                    }
                    break;

                case VerticalZipLineClassData data: // Vertical Ziplines
                    data = ( VerticalZipLineClassData )( object ) scriptData;
                    DrawVerticalZipline drawVerticalZipline = ( DrawVerticalZipline ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, drawVerticalZipline, new[] { "Panels", "ShowDevelopersOptions", "zipline", "fence_post", "arm", "rope_start", "rope_end", "helperPlacement" }.ToList() );

                    foreach ( VCPanelsClassData panelData in data.Panels )
                    {
                        UnityEngine.Object loadedPrefabResourcePanel = ImportExportJson.FindPrefabFromName( "mdl#" + panelData.Model );
                        if ( loadedPrefabResourcePanel == null )
                        {
                            ReMapConsole.Log( $"[Json Import] Couldnt find prefab with name of: {panelData.Model}" , ReMapConsole.LogType.Error );
                            continue;
                        }

                        GameObject panel = PrefabUtility.InstantiatePrefab( loadedPrefabResourcePanel as GameObject ) as GameObject;
                        GetSetTransformData( panel, panelData.TransformData );
                        CreatePath( panelData.Path, panelData.PathString, panel );

                        Array.Resize( ref drawVerticalZipline.Panels, drawVerticalZipline.Panels.Length + 1 );
                        drawVerticalZipline.Panels[ drawVerticalZipline.Panels.Length - 1 ] = panel;
                    }
                    break;

                case NonVerticalZipLineClassData data: // Non Vertical ZipLines
                    data = ( NonVerticalZipLineClassData )( object ) scriptData;
                    DrawNonVerticalZipline drawNonVerticalZipline = ( DrawNonVerticalZipline ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, drawNonVerticalZipline, new[] { "Panels", "ShowDevelopersOptions", "zipline", "fence_post_start", "arm_start", "fence_post_end", "arm_end", "rope_start", "rope_end", "helperPlacement_start", "helperPlacement_end" }.ToList() );
                    GetSetTransformData( obj.transform.Find( "support_start" ).gameObject, data.ZiplineStart );
                    GetSetTransformData( obj.transform.Find( "support_end" ).gameObject, data.ZiplineEnd );

                    foreach ( VCPanelsClassData panelData in data.Panels )
                    {
                        UnityEngine.Object loadedPrefabResourcePanel = ImportExportJson.FindPrefabFromName( "mdl#" + panelData.Model );
                        if ( loadedPrefabResourcePanel == null )
                        {
                            ReMapConsole.Log( $"[Json Import] Couldnt find prefab with name of: {panelData.Model}" , ReMapConsole.LogType.Error );
                            continue;
                        }

                        GameObject panel = PrefabUtility.InstantiatePrefab( loadedPrefabResourcePanel as GameObject ) as GameObject;
                        GetSetTransformData( panel, panelData.TransformData );
                        CreatePath( panelData.Path, panelData.PathString, panel );

                        Array.Resize( ref drawNonVerticalZipline.Panels, drawNonVerticalZipline.Panels.Length + 1 );
                        drawNonVerticalZipline.Panels[ drawNonVerticalZipline.Panels.Length - 1 ] = panel;
                    }
                    break;

                case SingleDoorClassData data: // Single Doors
                    data = ( SingleDoorClassData )( object ) scriptData;
                    DoorScript doorScriptSingle = ( DoorScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, doorScriptSingle );
                    break;

                case DoubleDoorClassData data: // Double Doors
                    data = ( DoubleDoorClassData )( object ) scriptData;
                    DoorScript doorScriptDouble = ( DoorScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, doorScriptDouble );
                    break;

                case HorzDoorClassData data: // Horizontal Doors
                    data = ( HorzDoorClassData )( object ) scriptData;
                    HorzDoorScript horzDoorScript = ( HorzDoorScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, horzDoorScript );
                    break;

                case VerticalDoorClassData data: // Vertical Doors
                    data = ( VerticalDoorClassData )( object ) scriptData;
                    VerticalDoorScript verticalDoorScript = ( VerticalDoorScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, verticalDoorScript );
                    break;

                case ButtonClassData data: // Bouttons
                    data = ( ButtonClassData )( object ) scriptData;
                    ButtonScripting buttonScripting = ( ButtonScripting ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, buttonScripting );
                    break;

                case JumppadClassData data: // Jumppads
                    data = ( JumppadClassData )( object ) scriptData;
                    PropScript propScriptJumppad = ( PropScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, propScriptJumppad );
                    break;

                case LootBinClassData data: // Loot Bins
                    data = ( LootBinClassData )( object ) scriptData;
                    LootBinScript lootBinScript = ( LootBinScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, lootBinScript );
                    break;

                case WeaponRackClassData data: // Weapon Racks
                    data = ( WeaponRackClassData )( object ) scriptData;
                    WeaponRackScript weaponRackScript = ( WeaponRackScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, weaponRackScript, new[] { "Name" }.ToList() );
                    break;

                case TriggerClassData data: // Triggers
                    data = ( TriggerClassData )( object ) scriptData;
                    TriggerScripting triggerScripting = ( TriggerScripting ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, triggerScripting );
                    break;

                case BubbleShieldClassData data: // Bubbles Shield
                    data = ( BubbleShieldClassData )( object ) scriptData;
                    BubbleScript bubbleScript = ( BubbleScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, bubbleScript );
                    break;

                case SpawnPointClassData data: // Spawn Points
                    data = ( SpawnPointClassData )( object ) scriptData;
                    SpawnPointScript spawnPointScript = ( SpawnPointScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, spawnPointScript );
                    break;

                case TextInfoPanelClassData data: // Text Info Panels
                    data = ( TextInfoPanelClassData )( object ) scriptData;
                    TextInfoPanelScript textInfoPanelScript = ( TextInfoPanelScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, textInfoPanelScript );
                    break;

                case FuncWindowHintClassData data: // Window Hints
                    data = ( FuncWindowHintClassData )( object ) scriptData;
                    WindowHintScript windowHintScript = ( WindowHintScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, windowHintScript );
                    break;

                case SoundClassData data: // Sounds
                    data = (SoundClassData  )( object ) scriptData;
                    SoundScript soundScript = ( SoundScript ) Helper.GetComponentByEnum( obj, dataType );
                    TransferDataToClass( data, soundScript );
                    break;

                default: break;
            }
        }
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

        if ( currentParent != obj ) parents.Add( currentParent );

        foreach ( GameObject parent in parents )
        {
            PathClass path = new PathClass();
            path.FolderName = parent.name;
            path.TransformData = GetSetTransformData( parent );
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
        while ( currentParent.transform.parent != null )
        {
            if ( currentParent != obj ) parents.Add( currentParent );
            currentParent = currentParent.transform.parent.gameObject;
        }

        if ( currentParent != obj ) parents.Add( currentParent );

        parents.Reverse();

        foreach ( GameObject parent in parents )
        {
            if ( string.IsNullOrEmpty( path ) )
            {
                path = $"{parent.name}";
            }
            else path = $"{path}/{parent.name}";
        }

        return path;
    }

    private static void CreatePath( List< PathClass > pathList, string pathString, GameObject obj )
    {
        if ( string.IsNullOrEmpty( pathString ) ) return;

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

            TransformData transformData = pathClass.TransformData;
            newFolder.transform.position = transformData.position;
            newFolder.transform.eulerAngles = transformData.eulerAngles;
            newFolder.transform.localScale = transformData.localScale;

            if ( folder != null ) newFolder.transform.SetParent( folder.transform );

            folder = newFolder;
        }

        if ( folder != null ) obj.transform.parent = folder.transform;
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

    private static string GetObjName( GameObject obj )
    {
        return obj.name.Split( char.Parse( " " ) )[0];
    }

    private static void ProcessExportClassData< T >( T classData, GameObject obj, string objPath, ObjectType objectType ) where T : GlobalClassData
    {
        classData.PathString = objPath;
        classData.Path = FindPath( obj );
        classData.TransformData = GetSetTransformData( obj, classData.TransformData );
        GetSetScriptData( obj, classData, objectType, GetSetData.Get );
    }

    private static GameObject ProcessImportClassData< T >( T objData, string objName, ObjectType objectType, int i, int j, int objectsCount ) where T : GlobalClassData
    {
        GameObject obj = null; string importing = "";

        if ( string.IsNullOrEmpty( objData.PathString ) )
        {
            importing = objName;
        } else importing = $"{objData.PathString}/{objName}";

        EditorUtility.DisplayProgressBar( $"Importing {Helper.GetObjNameWithEnum( objectType )} {j}/{objectsCount}", $"Importing: {importing}", ( i + 1 ) / ( float )objectsCount );
        ReMapConsole.Log( "[Json Import] Importing: " + objName, ReMapConsole.LogType.Info );

        if ( objName == "custom_linked_zipline" ) return new GameObject( "custom_linked_zipline" );

        UnityEngine.Object loadedPrefabResource = ImportExportJson.FindPrefabFromName( objName );
        if ( loadedPrefabResource == null )
        {
            ReMapConsole.Log( $"[Json Import] Couldnt find prefab with name of: {objName}" , ReMapConsole.LogType.Error );
            return null;
        } else obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;

        GetSetTransformData( obj, objData.TransformData );
        GetSetScriptData( obj, objData, objectType, GetSetData.Set );
        CreatePath( objData.Path, objData.PathString, obj );

        return obj;
    }

    public static void TransferDataToClass< TSource, TDestination >( TSource source, TDestination destination, List< string > propertiesToRemove = null )
    {
        if ( source == null || destination == null )
        {
            throw new ArgumentNullException( "Source or destination object cannot be null." );
        }

        Type sourceType = typeof( TSource );
        Type destinationType = typeof( TDestination );

        FieldInfo[] sourceFields = sourceType.GetFields( BindingFlags.Public | BindingFlags.Instance );
        FieldInfo[] destinationFields = destinationType.GetFields( BindingFlags.Public | BindingFlags.Instance );

        foreach ( FieldInfo sourceField in sourceFields )
        {
            // Ignore properties that are in the propertiesToRemove list
            if ( propertiesToRemove != null && propertiesToRemove.Contains( sourceField.Name, StringComparer.OrdinalIgnoreCase ) )
            {
                continue;
            }

            FieldInfo destinationField = Array.Find( destinationFields, field => field.Name.Equals( sourceField.Name, StringComparison.OrdinalIgnoreCase ) );

            // Check if the destination field exists and has the same field type as the source field
            if ( destinationField != null && destinationField.FieldType == sourceField.FieldType )
            {
                object value = sourceField.GetValue( source );
                destinationField.SetValue( destination, value );
            }
        }
    }
}
