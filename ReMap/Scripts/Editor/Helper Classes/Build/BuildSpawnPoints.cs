
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildSpawnPoint
    {
        public static string BuildSpawnPointObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    // Empty
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
                SpawnPointScript script = ( SpawnPointScript ) Helper.GetComponentByEnum( obj, ObjectType.SpawnPoint );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        // Empty
                        break;

                    case BuildType.EntFile:
                        code +=  "{\n";
                        code += $"\"teamnumber\" \"0\"\n";
                        code += $"\"phase_9\" \"0\"\n";
                        code += $"\"phase_8\" \"0\"\n";
                        code += $"\"phase_7\" \"0\"\n";
                        code += $"\"phase_6\" \"0\"\n";
                        code += $"\"phase_5\" \"0\"\n";
                        code += $"\"phase_4\" \"0\"\n";
                        code += $"\"phase_3\" \"0\"\n";
                        code += $"\"phase_2\" \"0\"\n";
                        code += $"\"phase_1\" \"0\"\n";
                        code += $"\"model\" \"mdl/dev/mp_spawn.rmdl\"\n";
                        code += $"\"gamemode_tdm\" \"1\"\n";
                        code += $"\"gamemode_fw\" \"0\"\n";
                        code += $"\"gamemode_freelance\" \"0\"\n";
                        code += $"\"gamemode_ffa\" \"1\"\n";
                        code += $"\"gamemode_fd\" \"1\"\n";
                        code += $"\"gamemode_ctf\" \"1\"\n";
                        code += $"\"gamemode_cp\" \"1\"\n";
                        code += $"\"gamemode_at\" \"1\"\n";
                        code += $"\"control_teamnumber\" \"-1\"\n";
                        code += $"\"scale\" \"1\"\n";
                        code += $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"\n";
                        code += $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"\n";
                        code += $"\"link_guid\" \"{Helper.GetRandomGUIDForEnt()}\"\n";
                        code += $"\"classname\" \"info_spawnpoint_human\"\n";
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
                    // Empty
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
