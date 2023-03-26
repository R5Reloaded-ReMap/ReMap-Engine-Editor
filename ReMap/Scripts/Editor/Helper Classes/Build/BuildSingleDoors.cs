
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildSingleDoor
    {
        public static string BuildSingleDoorObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Single Doors";
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
                DoorScript script = ( DoorScript ) Helper.GetComponentByEnum( obj, ObjectType.SingleDoor );
                if ( script == null ) continue;

                if ( script.DoorLeft == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Single, { Helper.BoolToLower( script.GoldDoor )} )";
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code +=  "{\n";
                        code += $"\"only_spawn_in_freelance\" \"0\"\n";
                        code += $"\"disableshadows\" \"0\"\n";
                        code += $"\"scale\" \"1\"\n";
                        code += $"\"angles\" \"{Helper.BuildAngles( script.DoorLeft.gameObject, true )}\"\n";
                        code += $"\"origin\" \"{Helper.BuildOrigin( script.DoorLeft.gameObject, true )}\"\n";
                        code += $"\"model\" \"mdl/door/canyonlands_door_single_02.rmdl\"\n";
                        code += $"\"classname\" \"prop_door\"\n";
                        code +=  "}\n";
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
