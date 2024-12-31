// Internal

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static ImportExport.SharedFunction;
using static ImportExport.Json.JsonShared;

namespace ImportExport.Json
{
    public class JsonExport
    {
        public static async void ExportJson()
        {
            Helper.FixPropTags();

            string path = EditorUtility.SaveFilePanel( "Json Export", "", "mapexport.json", "json" );

            if ( path.Length == 0 ) return;

            EditorUtility.DisplayProgressBar( "Starting Export", "", 0 );

            ResetJsonData();

            foreach ( var objectType in Helper.GetAllObjectType() )
                await ExecuteJson( objectType, ExecuteType.Export );

            ReMapConsole.Log( "[Json Export] Writing to file: " + path, ReMapConsole.LogType.Warning );
            string json = JsonUtility.ToJson( jsonData );
            File.WriteAllText( path, json );

            ReMapConsole.Log( "[Json Export] Finished.", ReMapConsole.LogType.Success );

            EditorUtility.ClearProgressBar();
        }

        public static async void ExportSelectionJson()
        {
            Helper.FixPropTags();

            string path = EditorUtility.SaveFilePanel( "Json Export", "", "mapexport.json", "json" );

            if ( path.Length == 0 ) return;

            EditorUtility.DisplayProgressBar( "Starting Export", "", 0 );

            ResetJsonData();

            foreach ( var objectType in Helper.GetAllObjectType() )
                await ExecuteJson( objectType, ExecuteType.Export, true );

            ReMapConsole.Log( "[Json Export] Writing to file: " + path, ReMapConsole.LogType.Warning );
            string json = JsonUtility.ToJson( jsonData );
            File.WriteAllText( path, json );

            ReMapConsole.Log( "[Json Export] Finished.", ReMapConsole.LogType.Success );

            EditorUtility.ClearProgressBar();
        }


        internal static async Task ExportObjectsWithEnum<T>( ObjectType objectType, List< T > listType, bool selectionOnly = false ) where T : GlobalClassData
        {
            int i = 0;
            int j = 1;

            var objectsData = Helper.GetAllObjectTypeWithEnum( objectType, selectionOnly );

            int objectsCount = objectsData.Length;
            string objType = Helper.GetObjNameWithEnum( objectType );
            string objName;

            foreach ( var obj in objectsData )
            {
                objName = obj.name;

                if ( Helper.GetComponentByEnum( obj, objectType ) == null )
                {
                    ReMapConsole.Log( "[Json Export] Missing Component on: " + objName, ReMapConsole.LogType.Error );
                    continue;
                }

                string exporting = "";
                string objPath = FindPathString( obj );

                if ( string.IsNullOrEmpty( objPath ) )
                    exporting = objName;
                else exporting = $"{objPath}/{objName}";

                ReMapConsole.Log( "[Json Export] Exporting: " + objName, ReMapConsole.LogType.Info );
                EditorUtility.DisplayProgressBar( $"Exporting {objType} {j}/{objectsCount}", $"Exporting: {exporting}", ( i + 1 ) / ( float ) objectsCount );

                var classData = Activator.CreateInstance( typeof( T ) ) as T;

                ProcessExportClassData( classData, obj, objPath, objectType );

                if ( IsValidPath( objPath ) ) listType.Add( classData );

                await Helper.Wait();
                i++;
                j++;
            }
        }

        private static void ProcessExportClassData<T>( T classData, GameObject obj, string objPath, ObjectType objectType ) where T : GlobalClassData
        {
            classData.PathString = objPath;
            classData.Path = FindPath( obj );
            classData.TransformData = GetSetTransformData( obj, classData.TransformData );
            GetSetScriptData( objectType, obj, classData, GetSetData.Get );
        }

        /// <summary>
        ///     Instantiate a new JsonData class
        /// </summary>
        private static void ResetJsonData()
        {
            jsonData = new JsonData();
            jsonData.Version = UnityInfo.JsonVersion;
            jsonData.Props = new List< PropClassData >();
            jsonData.Ziplines = new List< ZipLineClassData >();
            jsonData.LinkedZiplines = new List< LinkedZipLinesClassData >();
            jsonData.VerticalZipLines = new List< VerticalZipLineClassData >();
            jsonData.NonVerticalZipLines = new List< NonVerticalZipLineClassData >();
            jsonData.SingleDoors = new List< SingleDoorClassData >();
            jsonData.DoubleDoors = new List< DoubleDoorClassData >();
            jsonData.HorizontalDoors = new List< HorzDoorClassData >();
            jsonData.VerticalDoors = new List< VerticalDoorClassData >();
            jsonData.JumpTowers = new List< JumpTowerClassData >();
            jsonData.Buttons = new List< ButtonClassData >();
            jsonData.JumpPads = new List< JumppadClassData >();
            jsonData.LootBins = new List< LootBinClassData >();
            jsonData.WeaponRacks = new List< WeaponRackClassData >();
            jsonData.Triggers = new List< TriggerClassData >();
            jsonData.BubbleShields = new List< BubbleShieldClassData >();
            jsonData.SpawnPoints = new List< SpawnPointClassData >();
            jsonData.NewLocPairs = new List< NewLocPairClassData >();
            jsonData.TextInfoPanels = new List< TextInfoPanelClassData >();
            jsonData.FuncWindowHints = new List< FuncWindowHintClassData >();
            jsonData.Sounds = new List< SoundClassData >();
            jsonData.CameraPaths = new List< CameraPathClassData >();
            jsonData.PlayerSpawns = new List< UOPlayerSpawnClassData >();
            jsonData.RespawnableHeals = new List< RespawnableHealClassData >();
            jsonData.SpeedBoosts = new List< SpeedBoostClassData >();
            jsonData.AnimatedCameras = new List< AnimatedCameraClassData >();
            jsonData.InvisButtons = new List< InvisButtonClassData >();
        }
    }
}