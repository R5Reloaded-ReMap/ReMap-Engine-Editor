
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

// Internal
using ImportExport.Shared;
using static ImportExport.Shared.SharedFunction;
using static ImportExport.Json.JsonImport;
using static ImportExport.Json.JsonExport;

namespace ImportExport.Json
{
    public enum ExportType
    {
        All = 0,
        Selection = 1,
    }

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
        internal static void GetSetScriptData< T >( GameObject obj, T scriptData, ObjectType dataType, GetSetData getSet ) where T : class
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
                        data.Name = UnityInfo.GetObjName( obj );
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
                        data.Name = UnityInfo.GetObjName( obj );
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
                        data.Name = UnityInfo.GetObjName( obj );
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

                    case JumpTowerClassData data: // Jump Towers
                        data = ( JumpTowerClassData )( object ) scriptData;
                        JumpTowerScript jumpTowerScript = ( JumpTowerScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( jumpTowerScript, data );
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
                        data.Name = UnityInfo.GetObjName( obj );
                        break;

                    case TriggerClassData data: // Triggers
                        data = ( TriggerClassData )( object ) scriptData;
                        TriggerScripting triggerScripting = ( TriggerScripting ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( triggerScripting, data, new[] { "HelperData" }.ToList() );
                        data.HelperData = GetSetTransformData( triggerScripting.Helper.gameObject, data.HelperData );
                        break;

                    case BubbleShieldClassData data: // Bubbles Shield
                        data = ( BubbleShieldClassData )( object ) scriptData;
                        BubbleScript bubbleScript = ( BubbleScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( bubbleScript, data, new[] { "Name" }.ToList() );
                        data.Name = UnityInfo.GetObjName( obj );
                        break;

                    case SpawnPointClassData data: // Spawn Points
                        data = ( SpawnPointClassData )( object ) scriptData;
                        SpawnPointScript spawnPointScript = ( SpawnPointScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( spawnPointScript, data );
                        break;

                    case TextInfoPanelClassData data: // Text Info Panels
                        data = ( TextInfoPanelClassData )( object ) scriptData;
                        TextInfoPanelScript textInfoPanelScript = ( TextInfoPanelScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( textInfoPanelScript, data, new[] { "TextMeshTitle", "TextMeshDescription" }.ToList() );
                        break;

                    case FuncWindowHintClassData data: // Window Hints
                        data = ( FuncWindowHintClassData )( object ) scriptData;
                        WindowHintScript windowHintScript = ( WindowHintScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( windowHintScript, data );
                        break;

                    case SoundClassData data: // Sounds
                        data = ( SoundClassData )( object ) scriptData;
                        SoundScript soundScript = ( SoundScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( soundScript, data, new[] { "ShowDevelopersOptions", "soundModel" }.ToList() );
                        break;

                    case CameraPathClassData data: // Camera Paths
                        data = ( CameraPathClassData )( object ) scriptData;
                        PathScript pathScript = ( PathScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( pathScript, data, new[] { "TargetRef", "PathNode" }.ToList() );
                        data.TargetRef = GetSetTransformData( pathScript.targetRef );
                        data.PathNode = new List< TransformData >();
                        foreach ( Transform node in obj.transform )
                        {
                            if ( node.gameObject.name == "targetRef" ) continue;
                            
                            data.PathNode.Add( GetSetTransformData( node.gameObject ) );
                        }
                        break;
                    
                    case UOPlayerSpawnClassData data: // Unity Only Player Spawn
                        data = ( UOPlayerSpawnClassData )( object ) scriptData;
                        EmptyScript emptyScript = ( EmptyScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( emptyScript, data );
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
                            GameObject node = Helper.CreateGameObject( "path_point" );
                            if ( !Helper.IsValid( node ) ) continue;
                            node.transform.position = nodesPos;
                            node.transform.parent = obj.transform;
                        }
                        break;

                    case VerticalZipLineClassData data: // Vertical Ziplines
                        data = ( VerticalZipLineClassData )( object ) scriptData;
                        DrawVerticalZipline drawVerticalZipline = ( DrawVerticalZipline ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, drawVerticalZipline, new[] { "Panels", "ShowDevelopersOptions", "zipline", "fence_post", "arm", "rope_start", "rope_end", "helperPlacement" }.ToList() );

                        foreach ( VCPanelsClassData panelData in data.Panels )
                        {
                            GameObject panel = Helper.CreateGameObject( "", $"mdl#{panelData.Model}", PathType.Name );
                            if ( !Helper.IsValid( panel ) ) continue;
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
                            GameObject panel = Helper.CreateGameObject( "", $"mdl#{panelData.Model}", PathType.Name );
                            if ( !Helper.IsValid( panel ) ) continue;
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

                    case JumpTowerClassData data: // Jump Towers
                        data = ( JumpTowerClassData )( object ) scriptData;
                        JumpTowerScript jumpTowerScript = ( JumpTowerScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, jumpTowerScript );
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
                        TransferDataToClass( data, triggerScripting, new[] { "HelperData" }.ToList() );
                        GetSetTransformData( triggerScripting.Helper.gameObject, data.HelperData );
                        break;

                    case BubbleShieldClassData data: // Bubbles Shield
                        data = ( BubbleShieldClassData )( object ) scriptData;
                        BubbleScript bubbleScript = ( BubbleScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, bubbleScript, new[] { "Name" }.ToList() );
                        break;

                    case SpawnPointClassData data: // Spawn Points
                        data = ( SpawnPointClassData )( object ) scriptData;
                        SpawnPointScript spawnPointScript = ( SpawnPointScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, spawnPointScript );
                        break;

                    case TextInfoPanelClassData data: // Text Info Panels
                        data = ( TextInfoPanelClassData )( object ) scriptData;
                        TextInfoPanelScript textInfoPanelScript = ( TextInfoPanelScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, textInfoPanelScript, new[] { "TextMeshTitle", "TextMeshDescription" }.ToList() );
                        break;

                    case FuncWindowHintClassData data: // Window Hints
                        data = ( FuncWindowHintClassData )( object ) scriptData;
                        WindowHintScript windowHintScript = ( WindowHintScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, windowHintScript );
                        break;

                    case SoundClassData data: // Sounds
                        data = ( SoundClassData )( object ) scriptData;
                        SoundScript soundScript = ( SoundScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, soundScript, new[] { "ShowDevelopersOptions", "soundModel" }.ToList() );
                        break;

                    case CameraPathClassData data: // Camera Paths
                        data = ( CameraPathClassData )( object ) scriptData;
                        obj.AddComponent< PathScript >();
                        PathScript pathScript = ( PathScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, pathScript );
                        pathScript.targetRef = Helper.CreateGameObject( "targetRef", UnityInfo.relativePathCubePrefab );
                        GetSetTransformData( pathScript.targetRef, data.TargetRef );
                        pathScript.targetRef.transform.parent = obj.transform;
                        foreach ( TransformData nodeData in data.PathNode )
                        {
                            GameObject node = Helper.CreateGameObject( "path_point", $"{UnityInfo.relativePathLodsUtility}/Camera.prefab" );
                            if ( !Helper.IsValid( node ) ) continue;
                            GetSetTransformData( node, nodeData );
                            node.transform.parent = obj.transform;
                        }
                        break;

                    case UOPlayerSpawnClassData data: // Unity Only Player Spawn
                        data = ( UOPlayerSpawnClassData )( object ) scriptData;
                        EmptyScript emptyScript = ( EmptyScript ) Helper.GetComponentByEnum( obj, dataType );
                        TransferDataToClass( data, emptyScript );
                        break;

                    default: break;
                }
            }
        }
        
        public static async Task ExecuteJson( ObjectType objectType, ExecuteType executeType, ExportType exportType = ExportType.All )
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
                            await ExportObjectsWithEnum( objectType, jsonData.BubbleShields, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.Buttons, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.CameraPaths, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.DoubleDoors, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.FuncWindowHints, exportType );
                        break;
                    }
                    break;
                case ObjectType.HorzDoor:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.HorzDoors, x => x.PathString );
                            break;

                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.HorzDoors );
                            break;

                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.HorzDoors, exportType );
                        break;
                    }
                    break;
                case ObjectType.Jumppad:
                    switch ( executeType )
                    {
                        case ExecuteType.SortList:
                            UnityInfo.SortListByKey( jsonData.Jumppads, x => x.PathString );
                            break;

                        case ExecuteType.Import:
                            await ImportObjectsWithEnum( objectType, jsonData.Jumppads );
                            break;

                        case ExecuteType.Export:
                            await ExportObjectsWithEnum( objectType, jsonData.Jumppads, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.JumpTowers, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.LinkedZiplines, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.LootBins, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.NewLocPairs, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.NonVerticalZipLines, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.Props, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.SingleDoors, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.Sounds, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.SpawnPoints, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.TextInfoPanels, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.Triggers, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.VerticalDoors, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.VerticalZipLines, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.WeaponRacks, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.Ziplines, exportType );
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
                            await ExportObjectsWithEnum( objectType, jsonData.PlayerSpawns, exportType );
                        break;
                    }
                    break;

                default: return;
            }
        }

        internal static bool IsValidPath( string path )
        {
            foreach ( string protectedModel in protectedModels )
            {
                if ( path.Contains( protectedModel ) )
                    return false;
            }

            return true;
        }
    }
}
