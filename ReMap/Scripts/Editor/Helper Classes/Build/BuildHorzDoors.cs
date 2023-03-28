
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildHorzDoor
    {
        public static string BuildHorzDoorObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Horizontal Doors";
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
                HorzDoorScript script = ( HorzDoorScript ) Helper.GetComponentByEnum( obj, ObjectType.HorzDoor );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, eMapEditorDoorType.Horizontal )";
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                            code +=  "{\n";
                            code += $"\"SuppressAnimSounds\" \"0\"\n";
                            code += $"\"StartDisabled\" \"0\"\n";
                            code += $"\"spawnflags\" \"0\"\n";
                            code += $"\"solid\" \"6\"\n";
                            code += $"\"skin\" \"0\"\n";
                            code += $"\"SetBodyGroup\" \"0\"\n";
                            code += $"\"rendermode\" \"0\"\n";
                            code += $"\"renderfx\" \"0\"\n";
                            code += $"\"rendercolor\" \"255 255 255\"\n";
                            code += $"\"renderamt\" \"255\"\n";
                            code += $"\"RandomAnimation\" \"0\"\n";
                            code += $"\"pressuredelay\" \"0\"\n";
                            code += $"\"PerformanceMode\" \"0\"\n";
                            code += $"\"mingpulevel\" \"0\"\n";
                            code += $"\"mincpulevel\" \"0\"\n";
                            code += $"\"MinAnimTime\" \"5\"\n";
                            code += $"\"maxgpulevel\" \"0\"\n";
                            code += $"\"maxcpulevel\" \"0\"\n";
                            code += $"\"MaxAnimTime\" \"10\"\n";
                            code += $"\"HoldAnimation\" \"0\"\n";
                            code += $"\"gamemode_tdm\" \"1\"\n";
                            code += $"\"gamemode_sur\" \"1\"\n";
                            code += $"\"gamemode_lts\" \"1\"\n";
                            code += $"\"gamemode_lh\" \"1\"\n";
                            code += $"\"gamemode_fd\" \"1\"\n";
                            code += $"\"gamemode_ctf\" \"1\"\n";
                            code += $"\"gamemode_cp\" \"1\"\n";
                            code += $"\"fadedist\" \"-1\"\n";
                            code += $"\"ExplodeRadius\" \"0\"\n";
                            code += $"\"ExplodeDamage\" \"0\"\n";
                            code += $"\"disableX360\" \"0\"\n";
                            code += $"\"disableshadows\" \"0\"\n";
                            code += $"\"disablereceiveshadows\" \"0\"\n";
                            code += $"\"DisableBoneFollowers\" \"0\"\n";
                            code += $"\"DefaultCycle\" \"0\"\n";
                            code += $"\"collide_titan\" \"1\"\n";
                            code += $"\"collide_ai\" \"1\"\n";
                            code += $"\"ClientSide\" \"0\"\n";
                            code += $"\"AnimateInStaticShadow\" \"0\"\n";
                            code += $"\"scale\" \"1\"\n";
                            code += $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"\n";
                            code += $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"\n";
                            code += $"\"script_name\" \"survival_door_plain\"\n";
                            code += $"\"model\" \"mdl/door/door_256x256x8_elevatorstyle02_animated.rmdl\"\n";
                            code += $"\"classname\" \"prop_dynamic\"\n";
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
