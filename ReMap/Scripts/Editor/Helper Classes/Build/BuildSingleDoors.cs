
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildSingleDoor
    {
        public static async Task< StringBuilder > BuildSingleDoorObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Single Doors" );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;

                case BuildType.DataTable:
                    // Empty
                    break;

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                DoorScript script = ( DoorScript ) Helper.GetComponentByEnum( obj, ObjectType.SingleDoor );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_SpawnDoor( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Single, {Helper.BoolToLower( script.GoldDoor )}, {Helper.BoolToLower( script.AppearOpen )} )" );
                        break;

                    case BuildType.EntFile:
                        AppendCode( ref code,  "{" );
                        AppendCode( ref code, $"\"only_spawn_in_freelance\" \"0\"" );
                        AppendCode( ref code, $"\"disableshadows\" \"0\"" );
                        AppendCode( ref code, $"\"scale\" \"1\"" );
                        AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( obj, true )}\"" );
                        AppendCode( ref code, $"\"model\" \"mdl/door/canyonlands_door_single_02.rmdl\"" );
                        AppendCode( ref code, $"\"classname\" \"prop_door\"" );
                        AppendCode( ref code,  "}" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        CodeViews.LiveMap.AddToGameQueue( $"MapEditor_SpawnDoor( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Single, {Helper.BoolToLower( script.GoldDoor )}, {Helper.BoolToLower( script.AppearOpen )}, true )" );
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;
                    
                case BuildType.DataTable:
                    // Empty
                    break;

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }
    }
}
