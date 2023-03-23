
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
using static ImportExport.Json.JsonShared;

namespace ImportExport.Json
{
    public class JsonImport
    {
        [ MenuItem( "ReMap/Import/Json", false, 51 ) ]
        public static async void ImportJson()
        {
            var path = EditorUtility.OpenFilePanel( "Json Import", "", "json" );

            if ( path.Length == 0 ) return;

            EditorUtility.DisplayProgressBar( "Starting Import", "Reading File..." , 0 );
            ReMapConsole.Log( "[Json Import] Reading file: " + path, ReMapConsole.LogType.Warning );

            string json = System.IO.File.ReadAllText( path );
            JsonData jsonData = JsonUtility.FromJson< JsonData >( json );

            // Sort by alphabetical name
            UnityInfo.SortListByKey( jsonData.Props, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.Ziplines, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.LinkedZiplines, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.VerticalZipLines, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.NonVerticalZipLines, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.SingleDoors, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.DoubleDoors, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.HorzDoors, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.VerticalDoors, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.Buttons, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.Jumppads, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.LootBins, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.WeaponRacks, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.Triggers, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.BubbleShields, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.SpawnPoints, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.TextInfoPanels, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.FuncWindowHints, x => x.PathString );
            UnityInfo.SortListByKey( jsonData.Sounds, x => x.PathString );


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
                        obj = ProcessImportClassData( data, "trigger_cylinder", objectType, i, j, objectsCount );
                        break;

                    case BubbleShieldClassData data: // Bubbles Shield
                        data = ( BubbleShieldClassData )( object ) objData;
                        obj = ProcessImportClassData( data, data.Name, objectType, i, j, objectsCount );
                        break;

                    case SpawnPointClassData data: // Spawn Points
                        data = ( SpawnPointClassData )( object ) objData;
                        obj = ProcessImportClassData( data, "custom_info_spawnpoint_human", objectType, i, j, objectsCount );
                        break;

                    case TextInfoPanelClassData data: // Text Info Panels
                        data = ( TextInfoPanelClassData )( object ) objData;
                        obj = ProcessImportClassData( data, "custom_text_info_panel", objectType, i, j, objectsCount );
                        break;

                    case FuncWindowHintClassData data: // Window Hints
                        data = ( FuncWindowHintClassData )( object ) objData;
                        obj = ProcessImportClassData( data, "", objectType, i, j, objectsCount );
                        break;

                    case SoundClassData data: // Sounds
                        data = ( SoundClassData )( object ) objData;
                        obj = ProcessImportClassData( data, "custom_sound", objectType, i, j, objectsCount );
                        break;

                    default: break;
                }

                await Task.Delay( TimeSpan.FromSeconds( 0.001 ) ); i++; j++;
            }
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

            UnityEngine.Object loadedPrefabResource = UnityInfo.FindPrefabFromName( objName );
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
    }
}
