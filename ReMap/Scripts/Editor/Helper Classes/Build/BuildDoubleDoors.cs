
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildDoubleDoor
    {
        public static StringBuilder BuildDoubleDoorObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code.Append( "    // Double Doors" );
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
                        code.Append( $"    MapEditor_SpawnDoor( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Double, { Helper.BoolToLower( script.GoldDoor )} )" );
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code.Append(  "{\n" );
                        code.Append( $"\"only_spawn_in_freelance\" \"0\"\n" );
                        code.Append( $"\"disableshadows\" \"0\"\n" );
                        code.Append( $"\"scale\" \"1\"\n" );
                        code.Append( $"\"angles\" \"{Helper.BuildAngles( script.DoorLeft.gameObject, true )}\"\n" );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( script.DoorLeft.gameObject, true )}\"\n" );
                        code.Append( $"\"link_to_guid_0\" \"{LinkGuidTo0}\"\n" );
                        code.Append( $"\"link_guid\" \"{LinkGuid}\"\n" );
                        code.Append( $"\"model\" \"mdl/door/canyonlands_door_single_02.rmdl\"\n" );
                        code.Append( $"\"classname\" \"prop_door\"\n" );
                        code.Append(  "}\n" );
                        code.Append(  "{\n" );
                        code.Append( $"\"only_spawn_in_freelance\" \"0\"\n" );
                        code.Append( $"\"disableshadows\" \"0\"\n" );
                        code.Append( $"\"scale\" \"1\"\n" );
                        code.Append( $"\"angles\" \"{Helper.BuildAngles( script.DoorRight.gameObject, true )}\"\n" );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( script.DoorRight.gameObject, true )}\"\n" );
                        code.Append( $"\"link_guid\" \"{LinkGuidTo0}\"\n" );
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
            }

            return code;
        }
    }
}
