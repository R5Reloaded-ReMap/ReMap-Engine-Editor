// Internal

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static ImportExport.SharedFunction;
using static ImportExport.Json.JsonImport;
using static ImportExport.Json.JsonExport;
using Object = UnityEngine.Object;

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
        internal static JsonData jsonData = new();

        internal static string[] protectedModels = { "_vertical_zipline", "_non_vertical_zipline" };

        /// <summary>
        ///     Get or Set the values contained in GameObject component
        /// </summary>
        internal static void GetSetScriptData<T>( ObjectType objectType, GameObject obj, T scriptData, GetSetData getSet ) where T : class
        {
            switch ( objectType )
            {
                case ObjectType.BubbleShield:
                    var bubbleShieldData = ( BubbleShieldClassData ) ( object ) scriptData;
                    var bubbleScript = ( BubbleScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( bubbleScript, bubbleShieldData );
                        bubbleShieldData.Name = UnityInfo.GetObjName( obj );
                    }
                    else
                    {
                        TransferDataToClass( bubbleShieldData, bubbleScript );
                    }
                    break;

                case ObjectType.Button:
                    var buttonData = ( ButtonClassData ) ( object ) scriptData;
                    var buttonScripting = ( ButtonScripting ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( buttonScripting, buttonData );
                    else TransferDataToClass( buttonData, buttonScripting );
                    break;

                case ObjectType.CameraPath:
                    var cameraPathData = ( CameraPathClassData ) ( object ) scriptData;
                    var cameraPathScript = ( PathScript ) Helper.GetComponentByEnum( obj, objectType );

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
                        var path_point = new List< Transform >();
                        for ( int i = 0; i < obj.transform.childCount; i++ )
                            path_point.Add( obj.transform.GetChild( i ) );
                        for ( int i = 0; i < path_point.Count; i++ )
                            if ( Helper.IsValid( path_point[ i ].gameObject ) )
                                Object.DestroyImmediate( path_point[ i ].gameObject );
                        TransferDataToClass( cameraPathData, cameraPathScript );
                        cameraPathScript.targetRef = Helper.CreateGameObject( "targetRef", UnityInfo.relativePathCubePrefab );
                        GetSetTransformData( cameraPathScript.targetRef, cameraPathData.TargetRef );
                        cameraPathScript.targetRef.transform.parent = obj.transform;
                        foreach ( var nodeData in cameraPathData.PathNode )
                        {
                            var node = Helper.CreateGameObject( "path_point", $"{UnityInfo.relativePathLodsUtility}/Camera.prefab" );
                            if ( !Helper.IsValid( node ) ) continue;
                            GetSetTransformData( node, nodeData );
                            node.transform.parent = obj.transform;
                        }
                    }
                    break;

                case ObjectType.DoubleDoor:
                    var doubleDoorData = ( DoubleDoorClassData ) ( object ) scriptData;
                    var doorScriptDouble = ( DoorScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( doorScriptDouble, doubleDoorData );
                    else TransferDataToClass( doubleDoorData, doorScriptDouble );
                    break;

                case ObjectType.FuncWindowHint:
                    var windowHintData = ( FuncWindowHintClassData ) ( object ) scriptData;
                    var windowHintScript = ( WindowHintScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( windowHintScript, windowHintData );
                    else TransferDataToClass( windowHintData, windowHintScript );
                    break;

                case ObjectType.HorzDoor:
                    var horzDoorData = ( HorzDoorClassData ) ( object ) scriptData;
                    var horzDoorScript = ( HorzDoorScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( horzDoorScript, horzDoorData );
                    else TransferDataToClass( horzDoorData, horzDoorScript );
                    break;

                case ObjectType.Jumppad:
                    var jumppadData = ( JumppadClassData ) ( object ) scriptData;
                    var propScriptJumppad = ( PropScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( propScriptJumppad, jumppadData );
                    else TransferDataToClass( jumppadData, propScriptJumppad );
                    break;

                case ObjectType.JumpTower:
                    var jumpTowerData = ( JumpTowerClassData ) ( object ) scriptData;
                    var jumpTowerScript = ( JumpTowerScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( jumpTowerScript, jumpTowerData );
                    else TransferDataToClass( jumpTowerData, jumpTowerScript );
                    break;

                case ObjectType.LinkedZipline:
                    var linkedZiplineData = ( LinkedZipLinesClassData ) ( object ) scriptData;
                    var linkedZiplineScript = ( LinkedZiplineScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( linkedZiplineScript, linkedZiplineData );
                        linkedZiplineData.Nodes = new List< Vector3 >();
                        foreach ( Transform nodes in obj.transform ) linkedZiplineData.Nodes.Add( nodes.gameObject.transform.position );
                    }
                    else
                    {
                        var nodes = new List< Transform >();
                        for ( int i = 0; i < obj.transform.childCount; i++ )
                            nodes.Add( obj.transform.GetChild( i ) );
                        for ( int i = 0; i < nodes.Count; i++ )
                            if ( Helper.IsValid( nodes[ i ].gameObject ) )
                                Object.DestroyImmediate( nodes[ i ].gameObject );
                        TransferDataToClass( linkedZiplineData, linkedZiplineScript );
                        foreach ( var nodesPos in linkedZiplineData.Nodes )
                        {
                            var node = Helper.CreateGameObject( "zipline_node", UnityInfo.relativePathCubePrefab );
                            if ( !Helper.IsValid( node ) ) continue;
                            node.transform.position = nodesPos;
                            node.transform.parent = obj.transform;
                        }
                    }
                    break;

                case ObjectType.LootBin:
                    var lootBinData = ( LootBinClassData ) ( object ) scriptData;
                    var lootBinScript = ( LootBinScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( lootBinScript, lootBinData );
                    else TransferDataToClass( lootBinData, lootBinScript );
                    break;

                case ObjectType.NewLocPair:
                    var newLocPairData = ( NewLocPairClassData ) ( object ) scriptData;
                    var newLocPairScript = ( NewLocPairScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( newLocPairScript, newLocPairData );
                    else TransferDataToClass( newLocPairData, newLocPairScript );
                    break;

                case ObjectType.NonVerticalZipLine:
                    var nonVerticalZipLineData = ( NonVerticalZipLineClassData ) ( object ) scriptData;
                    var drawNonVerticalZipline = ( DrawNonVerticalZipline ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( drawNonVerticalZipline, nonVerticalZipLineData );
                        nonVerticalZipLineData.Name = UnityInfo.GetObjName( obj );
                        nonVerticalZipLineData.ZiplineStart = GetSetTransformData( obj.transform.Find( "support_start" ).gameObject, nonVerticalZipLineData.ZiplineStart );
                        nonVerticalZipLineData.ZiplineEnd = GetSetTransformData( obj.transform.Find( "support_end" ).gameObject, nonVerticalZipLineData.ZiplineEnd );
                        nonVerticalZipLineData.Panels = new List< VCPanelsClassData >();
                        foreach ( var panel in drawNonVerticalZipline.Panels )
                        {
                            var panelClass = new VCPanelsClassData();
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

                        foreach ( var panelData in nonVerticalZipLineData.Panels )
                        {
                            var panel = Helper.CreateGameObject( "", $"mdl#{panelData.Model}", PathType.Name );
                            if ( !Helper.IsValid( panel ) ) continue;
                            GetSetTransformData( panel, panelData.TransformData );
                            CreatePath( panelData.Path, panelData.PathString, panel );

                            Array.Resize( ref drawNonVerticalZipline.Panels, drawNonVerticalZipline.Panels.Length + 1 );
                            drawNonVerticalZipline.Panels[ drawNonVerticalZipline.Panels.Length - 1 ] = panel;
                        }
                    }
                    break;

                case ObjectType.Prop:
                    var propData = ( PropClassData ) ( object ) scriptData;
                    var propScript = ( PropScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( propScript, propData );
                        propData.Name = UnityInfo.GetObjName( obj );
                    }
                    else
                    {
                        TransferDataToClass( propData, propScript );
                    }
                    break;

                case ObjectType.SingleDoor:
                    var singleDoorData = ( SingleDoorClassData ) ( object ) scriptData;
                    var doorScriptSingle = ( DoorScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( doorScriptSingle, singleDoorData );
                    else TransferDataToClass( singleDoorData, doorScriptSingle );
                    break;

                case ObjectType.Sound:
                    var soundData = ( SoundClassData ) ( object ) scriptData;
                    var soundScript = ( SoundScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( soundScript, soundData );
                    else TransferDataToClass( soundData, soundScript );
                    break;

                case ObjectType.SpawnPoint:
                    var spawnPointData = ( SpawnPointClassData ) ( object ) scriptData;
                    var spawnPointScript = ( SpawnPointScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( spawnPointScript, spawnPointData );
                    else TransferDataToClass( spawnPointData, spawnPointScript );
                    break;

                case ObjectType.TextInfoPanel:
                    var textInfoPanelData = ( TextInfoPanelClassData ) ( object ) scriptData;
                    var textInfoPanelScript = ( TextInfoPanelScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( textInfoPanelScript, textInfoPanelData );
                    else TransferDataToClass( textInfoPanelData, textInfoPanelScript );
                    break;

                case ObjectType.Trigger:
                    var triggerData = ( TriggerClassData ) ( object ) scriptData;
                    var triggerScripting = ( TriggerScripting ) Helper.GetComponentByEnum( obj, objectType );

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
                    var verticalDoorData = ( VerticalDoorClassData ) ( object ) scriptData;
                    var verticalDoorScript = ( VerticalDoorScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( verticalDoorScript, verticalDoorData );
                    else TransferDataToClass( verticalDoorData, verticalDoorScript );
                    break;

                case ObjectType.VerticalZipLine:
                    var verticalZipLineData = ( VerticalZipLineClassData ) ( object ) scriptData;
                    var drawVerticalZipline = ( DrawVerticalZipline ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( drawVerticalZipline, verticalZipLineData );
                        verticalZipLineData.Name = UnityInfo.GetObjName( obj );
                        verticalZipLineData.Panels = new List< VCPanelsClassData >();
                        foreach ( var panel in drawVerticalZipline.Panels )
                        {
                            var panelClass = new VCPanelsClassData();
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
                        foreach ( var panelData in verticalZipLineData.Panels )
                        {
                            var panel = Helper.CreateGameObject( "", $"mdl#{panelData.Model}", PathType.Name );
                            if ( !Helper.IsValid( panel ) ) continue;
                            GetSetTransformData( panel, panelData.TransformData );
                            CreatePath( panelData.Path, panelData.PathString, panel );

                            Array.Resize( ref drawVerticalZipline.Panels, drawVerticalZipline.Panels.Length + 1 );
                            drawVerticalZipline.Panels[ drawVerticalZipline.Panels.Length - 1 ] = panel;
                        }
                    }
                    break;

                case ObjectType.WeaponRack:
                    var weaponRackData = ( WeaponRackClassData ) ( object ) scriptData;
                    var weaponRackScript = ( WeaponRackScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( weaponRackScript, weaponRackData );
                        weaponRackData.Name = UnityInfo.GetObjName( obj );
                    }
                    else
                    {
                        TransferDataToClass( weaponRackData, weaponRackScript );
                    }
                    break;

                case ObjectType.ZipLine:
                    var ziplineData = ( ZipLineClassData ) ( object ) scriptData;
                    var drawZipline = ( DrawZipline ) Helper.GetComponentByEnum( obj, objectType );

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
                    var UOPlayerSpawnData = ( UOPlayerSpawnClassData ) ( object ) scriptData;
                    var UOPlayerSpawnScript = ( EmptyScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( UOPlayerSpawnScript, UOPlayerSpawnData );
                    else TransferDataToClass( UOPlayerSpawnData, UOPlayerSpawnScript );
                    break;

                case ObjectType.RespawnableHeal:
                    var respawnableHealData = ( RespawnableHealClassData ) ( object ) scriptData;
                    var respawnableHealScript = ( RespawnableHealScript ) Helper.GetComponentByEnum( obj, objectType );
                    respawnableHealData.Name = UnityInfo.GetObjName( obj );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( respawnableHealScript, respawnableHealData );
                    else TransferDataToClass( respawnableHealData, respawnableHealScript );
                    break;

                case ObjectType.SpeedBoost:
                    var speedBoostData = ( SpeedBoostClassData ) ( object ) scriptData;
                    var speedBoostScript = ( SpeedBoostScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( speedBoostScript, speedBoostData );
                    else TransferDataToClass( speedBoostData, speedBoostScript );
                    break;

                case ObjectType.AnimatedCamera:
                    var animatedCameraData = ( AnimatedCameraClassData ) ( object ) scriptData;
                    var animatedCameraScript = ( AnimatedCameraScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                        TransferDataToClass( animatedCameraScript, animatedCameraData );
                    else TransferDataToClass( animatedCameraData, animatedCameraScript );
                    break;
                case ObjectType.InvisButton:
                    var invisButtonData = ( InvisButtonClassData ) ( object ) scriptData;
                    var invisButton = ( InvisButtonScript ) Helper.GetComponentByEnum( obj, objectType );

                    if ( getSet == GetSetData.Get )
                    {
                        TransferDataToClass( invisButton, invisButtonData );
                        invisButtonData.ButtonLocalisation = GetSetTransformData( invisButton.Button.gameObject, invisButtonData.ButtonLocalisation );
                        invisButtonData.DestinationLocalisation = GetSetTransformData( invisButton.Destination.gameObject, invisButtonData.DestinationLocalisation );
                    }
                    else
                    {
                        TransferDataToClass( invisButtonData, invisButton );
                        GetSetTransformData( invisButton.Button.gameObject, invisButtonData.ButtonLocalisation );
                        GetSetTransformData( invisButton.Destination.gameObject, invisButtonData.DestinationLocalisation );
                    }
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
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.BubbleShields, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.BubbleShields );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.BubbleShields, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.Button:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.Buttons, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.Buttons );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.Buttons, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.CameraPath:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.CameraPaths, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.CameraPaths );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.CameraPaths, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.DoubleDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.DoubleDoors, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.DoubleDoors );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.DoubleDoors, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.FuncWindowHint:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.FuncWindowHints, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.FuncWindowHints );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.FuncWindowHints, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.HorzDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.HorizontalDoors, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.HorizontalDoors );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.HorizontalDoors, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.InvisButton:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.InvisButtons, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.InvisButtons );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.InvisButtons, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.Jumppad:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.JumpPads, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.JumpPads );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.JumpPads, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.JumpTower:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.JumpTowers, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.JumpTowers );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.JumpTowers, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.LinkedZipline:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.LinkedZiplines, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.LinkedZiplines );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.LinkedZiplines, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.LootBin:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.LootBins, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.LootBins );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.LootBins, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.NewLocPair:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.NewLocPairs, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.NewLocPairs );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.NewLocPairs, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.NonVerticalZipLine:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.NonVerticalZipLines, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.NonVerticalZipLines );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.NonVerticalZipLines, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.Prop:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.Props, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.Props );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.Props, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.SingleDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.SingleDoors, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.SingleDoors );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.SingleDoors, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.Sound:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.Sounds, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.Sounds );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.Sounds, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.SpawnPoint:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.SpawnPoints, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.SpawnPoints );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.SpawnPoints, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.TextInfoPanel:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.TextInfoPanels, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.TextInfoPanels );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.TextInfoPanels, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.Trigger:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.Triggers, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.Triggers );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.Triggers, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.VerticalDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.VerticalDoors, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.VerticalDoors );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.VerticalDoors, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.VerticalZipLine:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.VerticalZipLines, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.VerticalZipLines );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.VerticalZipLines, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.WeaponRack:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.WeaponRacks, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.WeaponRacks );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.WeaponRacks, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.ZipLine:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.Ziplines, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.Ziplines );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.Ziplines, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.LiveMapCodePlayerSpawn:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.PlayerSpawns, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.PlayerSpawns );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.PlayerSpawns, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.RespawnableHeal:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.RespawnableHeals, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.RespawnableHeals );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.RespawnableHeals, selectionOnly );
                            break;
                    }
                    break;
                case ObjectType.SpeedBoost:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.SpeedBoosts, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.SpeedBoosts );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.SpeedBoosts, selectionOnly );
                            break;
                    }
                    break;

                case ObjectType.AnimatedCamera:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.AnimatedCameras, x => x.PathString );
                            break;
                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.AnimatedCameras );
                            break;
                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.AnimatedCameras, selectionOnly );
                            break;
                    }
                    break;

                default: return;
            }
        }

        internal static bool IsValidPath( string path )
        {
            foreach ( string protectedModel in protectedModels )
                if ( path.Contains( protectedModel ) )
                    return false;

            return true;
        }
    }
}