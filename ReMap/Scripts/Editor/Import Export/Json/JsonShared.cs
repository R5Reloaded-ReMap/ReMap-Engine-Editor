
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

// Internal
using static ImportExport.SharedFunction;
using static ImportExport.Json.JsonImport;
using static ImportExport.Json.JsonExport;

namespace ImportExport.Json
{
    public enum ExecuteType
    {
        SortList,
        Import,
        Export
    }

    public class JsonShared
    {
        internal static JsonData jsonData = new JsonData();

        internal static string[] protectedModels = { "_vertical_zipline", "_non_vertical_zipline" };

        /// <summary>
        /// Get or Set the values contained in GameObject component
        /// </summary>
        internal static void GetSetScriptData< T >( ObjectType objectType, GameObject obj, T scriptData, GetSetData getSet ) where T : class
        {
            switch ( objectType )
            {
                case ObjectType.BubbleShield:
                    BubbleShieldClassData bubbleShieldData = ( BubbleShieldClassData )( object ) scriptData;
                    BubbleScript bubbleScript = ( BubbleScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( bubbleScript, bubbleShieldData );
                        bubbleShieldData.Name = UnityInfo.GetObjName( obj );
                    }
                    else TransferDataToClass( bubbleShieldData, bubbleScript );
                    break;

                case ObjectType.Button:
                    ButtonClassData buttonData = ( ButtonClassData )( object ) scriptData;
                    ButtonScripting buttonScripting = ( ButtonScripting ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( buttonScripting, buttonData );
                    }
                    else TransferDataToClass( buttonData, buttonScripting );
                    break;

                case ObjectType.CameraPath:
                    CameraPathClassData cameraPathData = ( CameraPathClassData )( object ) scriptData;
                    PathScript cameraPathScript = ( PathScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( cameraPathScript, cameraPathData );
                        cameraPathData.TargetRef = GetSetTransformData( cameraPathScript.targetRef );
                        cameraPathData.PathNode = new List< TransformData >();
                        foreach ( Transform node in obj.transform )
                        {
                            if ( node.gameObject.name == "targetRef" ) continue;
                         
                            cameraPathData.PathNode.Add( GetSetTransformData( node.gameObject ) );
                        }
                    }
                    else
                    {
                        List< Transform > path_point = new List< Transform >();
                        for ( int i = 0; i < obj.transform.childCount; i++ )
                        {
                            path_point.Add( obj.transform.GetChild( i ) );
                        }
                        for ( int i = 0; i < path_point.Count; i++ )
                        {
                            if ( Helper.IsValid( path_point[i].gameObject ) ) GameObject.DestroyImmediate( path_point[i].gameObject );
                        }
                        TransferDataToClass( cameraPathData, cameraPathScript );
                        cameraPathScript.targetRef = Helper.CreateGameObject( "targetRef", UnityInfo.relativePathCubePrefab );
                        GetSetTransformData( cameraPathScript.targetRef, cameraPathData.TargetRef );
                        cameraPathScript.targetRef.transform.parent = obj.transform;
                        foreach ( TransformData nodeData in cameraPathData.PathNode )
                        {
                            GameObject node = Helper.CreateGameObject( "path_point", $"{UnityInfo.relativePathLodsUtility}/Camera.prefab" );
                            if ( !Helper.IsValid( node ) ) continue;
                            GetSetTransformData( node, nodeData );
                            node.transform.parent = obj.transform;
                        }
                    }
                    break;

                case ObjectType.DoubleDoor:
                    DoubleDoorClassData doubleDoorData = ( DoubleDoorClassData )( object ) scriptData;
                    DoorScript doorScriptDouble = ( DoorScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( doorScriptDouble, doubleDoorData );
                    }
                    else TransferDataToClass( doubleDoorData, doorScriptDouble );
                    break;

                case ObjectType.FuncWindowHint:
                    FuncWindowHintClassData windowHintData = ( FuncWindowHintClassData )( object ) scriptData;
                    WindowHintScript windowHintScript = ( WindowHintScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( windowHintScript, windowHintData );
                    }
                    else TransferDataToClass( windowHintData, windowHintScript );
                    break;

                case ObjectType.HorzDoor:
                    HorzDoorClassData horzDoorData = ( HorzDoorClassData )( object ) scriptData;
                    HorzDoorScript horzDoorScript = ( HorzDoorScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( horzDoorScript, horzDoorData );
                    }
                    else TransferDataToClass( horzDoorData, horzDoorScript );
                    break;

                case ObjectType.Jumppad:
                    JumppadClassData jumppadData = ( JumppadClassData )( object ) scriptData;
                    PropScript propScriptJumppad = ( PropScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( propScriptJumppad, jumppadData );
                    }
                    else TransferDataToClass( jumppadData, propScriptJumppad );
                    break;

                case ObjectType.JumpTower:
                    JumpTowerClassData jumpTowerData = ( JumpTowerClassData )( object ) scriptData;
                    JumpTowerScript jumpTowerScript = ( JumpTowerScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( jumpTowerScript, jumpTowerData );
                    }
                    else TransferDataToClass( jumpTowerData, jumpTowerScript );
                    break;

                case ObjectType.LinkedZipline:
                    LinkedZipLinesClassData linkedZiplineData = ( LinkedZipLinesClassData )( object ) scriptData;
                    LinkedZiplineScript linkedZiplineScript = ( LinkedZiplineScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( linkedZiplineScript, linkedZiplineData );
                        linkedZiplineData.Nodes = new List< Vector3 >();
                        foreach ( Transform nodes in obj.transform ) linkedZiplineData.Nodes.Add( nodes.gameObject.transform.position );
                    }
                    else
                    {
                        List< Transform > nodes = new List< Transform >();
                        for ( int i = 0; i < obj.transform.childCount; i++ )
                        {
                            nodes.Add( obj.transform.GetChild( i ) );
                        }
                        for ( int i = 0; i < nodes.Count; i++ )
                        {
                            if ( Helper.IsValid( nodes[i].gameObject ) ) GameObject.DestroyImmediate( nodes[i].gameObject );
                        }
                        TransferDataToClass( linkedZiplineData, linkedZiplineScript );
                        foreach ( Vector3 nodesPos in linkedZiplineData.Nodes )
                        {
                            GameObject node = Helper.CreateGameObject( "zipline_node", UnityInfo.relativePathCubePrefab );
                            if ( !Helper.IsValid( node ) ) continue;
                            node.transform.position = nodesPos;
                            node.transform.parent = obj.transform;
                        }
                    }
                    break;

                case ObjectType.LootBin:
                    LootBinClassData lootBinData = ( LootBinClassData )( object ) scriptData;
                    LootBinScript lootBinScript = ( LootBinScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( lootBinScript, lootBinData );
                    }
                    else TransferDataToClass( lootBinData, lootBinScript );
                    break;

                case ObjectType.NewLocPair:
                    NewLocPairClassData newLocPairData = ( NewLocPairClassData )( object ) scriptData;
                    NewLocPairScript newLocPairScript = ( NewLocPairScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( newLocPairScript, newLocPairData );
                    }
                    else TransferDataToClass( newLocPairData, newLocPairScript );
                    break;

                case ObjectType.NonVerticalZipLine:
                    NonVerticalZipLineClassData nonVerticalZipLineData = ( NonVerticalZipLineClassData )( object ) scriptData;
                    DrawNonVerticalZipline drawNonVerticalZipline = ( DrawNonVerticalZipline ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( drawNonVerticalZipline, nonVerticalZipLineData );
                        nonVerticalZipLineData.Name = UnityInfo.GetObjName( obj );
                        nonVerticalZipLineData.ZiplineStart = GetSetTransformData( obj.transform.Find( "support_start" ).gameObject, nonVerticalZipLineData.ZiplineStart );
                        nonVerticalZipLineData.ZiplineEnd = GetSetTransformData( obj.transform.Find( "support_end" ).gameObject, nonVerticalZipLineData.ZiplineEnd );
                        nonVerticalZipLineData.Panels = new List< VCPanelsClassData >();
                        foreach ( GameObject panel in drawNonVerticalZipline.Panels )
                        {
                            VCPanelsClassData panelClass = new VCPanelsClassData();
                            panelClass.Model = panel.name;
                            panelClass.TransformData = GetSetTransformData( panel, panelClass.TransformData );
                            panelClass.Path = FindPath( panel );
                            panelClass.PathString = FindPathString( panel );
                            nonVerticalZipLineData.Panels.Add( panelClass );
                        }
                    }
                    else
                    {
                        TransferDataToClass( nonVerticalZipLineData, drawNonVerticalZipline );
                        GetSetTransformData( obj.transform.Find( "support_start" ).gameObject, nonVerticalZipLineData.ZiplineStart );
                        GetSetTransformData( obj.transform.Find( "support_end" ).gameObject, nonVerticalZipLineData.ZiplineEnd );

                        foreach ( VCPanelsClassData panelData in nonVerticalZipLineData.Panels )
                        {
                            GameObject panel = Helper.CreateGameObject( "", $"mdl#{panelData.Model}", PathType.Name );
                            if ( !Helper.IsValid( panel ) ) continue;
                            GetSetTransformData( panel, panelData.TransformData );
                            CreatePath( panelData.Path, panelData.PathString, panel );

                            Array.Resize( ref drawNonVerticalZipline.Panels, drawNonVerticalZipline.Panels.Length + 1 );
                            drawNonVerticalZipline.Panels[ drawNonVerticalZipline.Panels.Length - 1 ] = panel;
                        }
                    }
                    break;

                case ObjectType.Prop:
                    PropClassData propData = ( PropClassData )( object ) scriptData;
                    PropScript propScript = ( PropScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( propScript, propData );
                        propData.Name = UnityInfo.GetObjName( obj );
                    }
                    else TransferDataToClass( propData, propScript );
                    break;

                case ObjectType.SingleDoor:
                    SingleDoorClassData singleDoorData = ( SingleDoorClassData )( object ) scriptData;
                    DoorScript doorScriptSingle = ( DoorScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( doorScriptSingle, singleDoorData );
                    }
                    else TransferDataToClass( singleDoorData, doorScriptSingle );
                    break;

                case ObjectType.Sound:
                    SoundClassData soundData = ( SoundClassData )( object ) scriptData;
                    SoundScript soundScript = ( SoundScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( soundScript, soundData );
                    }
                    else TransferDataToClass( soundData, soundScript );
                    break;

                case ObjectType.SpawnPoint:
                    SpawnPointClassData spawnPointData = ( SpawnPointClassData )( object ) scriptData;
                    SpawnPointScript spawnPointScript = ( SpawnPointScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( spawnPointScript, spawnPointData );
                    }
                    else TransferDataToClass( spawnPointData, spawnPointScript );
                    break;

                case ObjectType.TextInfoPanel:
                    TextInfoPanelClassData textInfoPanelData = ( TextInfoPanelClassData )( object ) scriptData;
                    TextInfoPanelScript textInfoPanelScript = ( TextInfoPanelScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( textInfoPanelScript, textInfoPanelData );
                    }
                    else TransferDataToClass( textInfoPanelData, textInfoPanelScript );
                    break;

                case ObjectType.Trigger:
                    TriggerClassData triggerData = ( TriggerClassData )( object ) scriptData;
                    TriggerScripting triggerScripting = ( TriggerScripting ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( triggerScripting, triggerData );
                        triggerData.HelperData = GetSetTransformData( triggerScripting.Helper.gameObject, triggerData.HelperData );
                    }
                    else
                    {
                        TransferDataToClass( triggerData, triggerScripting );
                        GetSetTransformData( triggerScripting.Helper.gameObject, triggerData.HelperData );
                    }
                    break;

                case ObjectType.VerticalDoor:
                    VerticalDoorClassData verticalDoorData = ( VerticalDoorClassData )( object ) scriptData;
                    VerticalDoorScript verticalDoorScript = ( VerticalDoorScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( verticalDoorScript, verticalDoorData );
                    }
                    else TransferDataToClass( verticalDoorData, verticalDoorScript );
                    break;

                case ObjectType.VerticalZipLine:
                    VerticalZipLineClassData verticalZipLineData = ( VerticalZipLineClassData )( object ) scriptData;
                    DrawVerticalZipline drawVerticalZipline = ( DrawVerticalZipline ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( drawVerticalZipline, verticalZipLineData );
                        verticalZipLineData.Name = UnityInfo.GetObjName( obj );
                        verticalZipLineData.Panels = new List< VCPanelsClassData >();
                        foreach ( GameObject panel in drawVerticalZipline.Panels )
                        {
                            VCPanelsClassData panelClass = new VCPanelsClassData();
                            panelClass.Model = panel.name;
                            panelClass.TransformData = GetSetTransformData( panel, panelClass.TransformData );
                            panelClass.Path = FindPath( panel );
                            panelClass.PathString = FindPathString( panel );
                            verticalZipLineData.Panels.Add( panelClass );
                        }
                    }
                    else
                    {
                        TransferDataToClass( verticalZipLineData, drawVerticalZipline );
                        foreach ( VCPanelsClassData panelData in verticalZipLineData.Panels )
                        {
                            GameObject panel = Helper.CreateGameObject( "", $"mdl#{panelData.Model}", PathType.Name );
                            if ( !Helper.IsValid( panel ) ) continue;
                            GetSetTransformData( panel, panelData.TransformData );
                            CreatePath( panelData.Path, panelData.PathString, panel );

                            Array.Resize( ref drawVerticalZipline.Panels, drawVerticalZipline.Panels.Length + 1 );
                            drawVerticalZipline.Panels[ drawVerticalZipline.Panels.Length - 1 ] = panel;
                        }
                    }
                    break;

                case ObjectType.WeaponRack:
                    WeaponRackClassData weaponRackData = ( WeaponRackClassData )( object ) scriptData;
                    WeaponRackScript weaponRackScript = ( WeaponRackScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( weaponRackScript, weaponRackData );
                        weaponRackData.Name = UnityInfo.GetObjName( obj );
                    }
                    else TransferDataToClass( weaponRackData, weaponRackScript );
                    break;

                case ObjectType.ZipLine:
                    ZipLineClassData ziplineData = ( ZipLineClassData )( object ) scriptData;
                    DrawZipline drawZipline = ( DrawZipline ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( drawZipline, ziplineData );
                        ziplineData.Zipline_start = drawZipline.zipline_start.position;
                        ziplineData.Zipline_end = drawZipline.zipline_end.position;
                    }
                    else
                    {
                        TransferDataToClass( ziplineData, drawZipline );
                        drawZipline.zipline_start.position = ziplineData.Zipline_start;
                        drawZipline.zipline_end.position = ziplineData.Zipline_end;
                    }
                    break;

                case ObjectType.LiveMapCodePlayerSpawn:
                    UOPlayerSpawnClassData UOPlayerSpawnData = ( UOPlayerSpawnClassData )( object ) scriptData;
                    EmptyScript UOPlayerSpawnScript = ( EmptyScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( UOPlayerSpawnScript, UOPlayerSpawnData );
                    }
                    else TransferDataToClass( UOPlayerSpawnData, UOPlayerSpawnScript );
                    break;

                default: return;
            }
        }

        public static async Task ExecuteJson( ObjectType objectType, ExecuteType executeType, bool selectionOnly = false )
        {
            switch ( objectType )
            {
                case ObjectType.BubbleShield:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.BubbleShields, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.BubbleShields ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.BubbleShields, selectionOnly ); break;
                    }
                    break;
                case ObjectType.Button:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.Buttons, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.Buttons ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.Buttons, selectionOnly ); break;
                    }
                    break;
                case ObjectType.CameraPath:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.CameraPaths, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.CameraPaths ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.CameraPaths, selectionOnly ); break;
                    }
                    break;
                case ObjectType.DoubleDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.DoubleDoors, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.DoubleDoors ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.DoubleDoors, selectionOnly ); break;
                    }
                    break;
                case ObjectType.FuncWindowHint:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.FuncWindowHints, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.FuncWindowHints ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.FuncWindowHints, selectionOnly ); break;
                    }
                    break;
                case ObjectType.HorzDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.HorzDoors, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.HorzDoors ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.HorzDoors, selectionOnly ); break;
                    }
                    break;
                case ObjectType.Jumppad:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.Jumppads, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.Jumppads ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.Jumppads, selectionOnly ); break;
                    }
                    break;
                case ObjectType.JumpTower:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.JumpTowers, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.JumpTowers ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.JumpTowers, selectionOnly ); break;
                    }
                    break;
                case ObjectType.LinkedZipline:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.LinkedZiplines, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.LinkedZiplines ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.LinkedZiplines, selectionOnly ); break;
                    }
                    break;
                case ObjectType.LootBin:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.LootBins, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.LootBins ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.LootBins, selectionOnly ); break;
                    }
                    break;
                case ObjectType.NewLocPair:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.NewLocPairs, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.NewLocPairs ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.NewLocPairs, selectionOnly ); break;
                    }
                    break;
                case ObjectType.NonVerticalZipLine:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.NonVerticalZipLines, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.NonVerticalZipLines ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.NonVerticalZipLines, selectionOnly ); break;
                    }
                    break;
                case ObjectType.Prop:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.Props, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.Props ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.Props, selectionOnly ); break;
                    }
                    break;
                case ObjectType.SingleDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.SingleDoors, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.SingleDoors ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.SingleDoors, selectionOnly ); break;
                    }
                    break;
                case ObjectType.Sound:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.Sounds, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.Sounds ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.Sounds, selectionOnly ); break;
                    }
                    break;
                case ObjectType.SpawnPoint:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.SpawnPoints, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.SpawnPoints ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.SpawnPoints, selectionOnly ); break;
                    }
                    break;
                case ObjectType.TextInfoPanel:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.TextInfoPanels, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.TextInfoPanels ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.TextInfoPanels, selectionOnly ); break;
                    }
                    break;
                case ObjectType.Trigger:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.Triggers, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.Triggers ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.Triggers, selectionOnly ); break;
                    }
                    break;
                case ObjectType.VerticalDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.VerticalDoors, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.VerticalDoors ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.VerticalDoors, selectionOnly ); break;
                    }
                    break;
                case ObjectType.VerticalZipLine:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.VerticalZipLines, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.VerticalZipLines ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.VerticalZipLines, selectionOnly ); break;
                    }
                    break;
                case ObjectType.WeaponRack:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.WeaponRacks, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.WeaponRacks ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.WeaponRacks, selectionOnly ); break;
                    }
                    break;
                case ObjectType.ZipLine:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.Ziplines, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.Ziplines ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.Ziplines, selectionOnly ); break;
                    }
                    break;
                case ObjectType.LiveMapCodePlayerSpawn:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList: UnityInfo.SortListByKey( jsonData.PlayerSpawns, x => x.PathString ); break;
                        case ExecuteType.Import: await ImportObjectsWithEnum( objectType, jsonData.PlayerSpawns ); break;
                        case ExecuteType.Export: await ExportObjectsWithEnum( objectType, jsonData.PlayerSpawns, selectionOnly ); break;
                    }
                    break;

                default: return;
            }
        }

        internal static bool IsValidPath( string path )
        {
            foreach ( string protectedModel in protectedModels )
            {
                if ( path.Contains( protectedModel ) ) return false;
            }

            return true;
        }
    }
}
