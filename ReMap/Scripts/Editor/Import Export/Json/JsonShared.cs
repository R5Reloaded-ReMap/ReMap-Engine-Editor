
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

namespace ImportExport.Json
{
    public class JsonShared
    {
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
                        data = (SoundClassData  )( object ) scriptData;
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
                        data = (SoundClassData  )( object ) scriptData;
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

                    default: break;
                }
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
