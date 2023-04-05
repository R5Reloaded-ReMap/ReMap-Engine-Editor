
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
                    code.Append( "    // Single Doors" );
                    PageBreak( ref code );
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

                if ( script.DoorLeft == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        code.Append( $"    MapEditor_SpawnDoor( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Single, { Helper.BoolToLower( script.GoldDoor )} )" );
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code.Append(  "{\n" );
                        code.Append( $"\"only_spawn_in_freelance\" \"0\"\n" );
                        code.Append( $"\"disableshadows\" \"0\"\n" );
                        code.Append( $"\"scale\" \"1\"\n" );
                        code.Append( $"\"angles\" \"{Helper.BuildAngles( script.DoorLeft.gameObject, true )}\"\n" );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( script.DoorLeft.gameObject, true )}\"\n" );
                        code.Append( $"\"model\" \"mdl/door/canyonlands_door_single_02.rmdl\"\n" );
                        code.Append( $"\"classname\" \"prop_door\"\n" );
                        code.Append(  "}\n" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.SendCommandToApex($"script MapEditor_SpawnDoor( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Single, { Helper.BoolToLower( script.GoldDoor )}, true )");
                        Helper.DelayInMS(CodeViewsWindow.LiveMap.BuildWaitMS);
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    PageBreak( ref code );
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
