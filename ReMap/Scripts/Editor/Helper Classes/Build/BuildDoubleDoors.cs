
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildDoubleDoor
    {
        public static async Task< StringBuilder > BuildDoubleDoorObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Double Doors" );
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
                DoorScript script = ( DoorScript ) Helper.GetComponentByEnum( obj, ObjectType.DoubleDoor );
                if ( script == null ) continue;

                if ( script.DoorLeft == null || script.DoorRight == null ) continue;

                string LinkGuid = Helper.GetRandomGUIDForEnt();
                string LinkGuidTo0 = Helper.GetRandomGUIDForEnt();

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_SpawnDoor( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Double, {Helper.BoolToLower( script.GoldDoor )}, {Helper.BoolToLower( script.AppearOpen )} )" );
                        break;

                    case BuildType.EntFile:
                        AppendCode( ref code,  "{" );
                        AppendCode( ref code, $"\"only_spawn_in_freelance\" \"0\"" );
                        AppendCode( ref code, $"\"disableshadows\" \"0\"" );
                        AppendCode( ref code, $"\"scale\" \"1\"" );
                        AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( script.DoorLeft.gameObject, true )}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( script.DoorLeft.gameObject, true )}\"" );
                        AppendCode( ref code, $"\"link_to_guid_0\" \"{LinkGuidTo0}\"" );
                        AppendCode( ref code, $"\"link_guid\" \"{LinkGuid}\"" );
                        AppendCode( ref code, $"\"model\" \"mdl/door/canyonlands_door_single_02.rmdl\"" );
                        AppendCode( ref code, $"\"classname\" \"prop_door\"" );
                        AppendCode( ref code,  "}" );
                        AppendCode( ref code,  "{" );
                        AppendCode( ref code, $"\"only_spawn_in_freelance\" \"0\"" );
                        AppendCode( ref code, $"\"disableshadows\" \"0\"" );
                        AppendCode( ref code, $"\"scale\" \"1\"" );
                        AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( script.DoorRight.gameObject, true )}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( script.DoorRight.gameObject, true )}\"" );
                        AppendCode( ref code, $"\"link_guid\" \"{LinkGuidTo0}\"" );
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
                        CodeViews.LiveMap.AddToGameQueue( $"MapEditor_SpawnDoor( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Double, {Helper.BoolToLower( script.GoldDoor )}, {Helper.BoolToLower( script.AppearOpen )}, true )" );
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
