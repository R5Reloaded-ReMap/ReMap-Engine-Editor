
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildSpawnPoint
    {
        public static async Task< StringBuilder > BuildSpawnPointObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

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
                        code.Append(  "{\n" );
                        code.Append( $"\"teamnumber\" \"0\"\n" );
                        code.Append( $"\"phase_9\" \"0\"\n" );
                        code.Append( $"\"phase_8\" \"0\"\n" );
                        code.Append( $"\"phase_7\" \"0\"\n" );
                        code.Append( $"\"phase_6\" \"0\"\n" );
                        code.Append( $"\"phase_5\" \"0\"\n" );
                        code.Append( $"\"phase_4\" \"0\"\n" );
                        code.Append( $"\"phase_3\" \"0\"\n" );
                        code.Append( $"\"phase_2\" \"0\"\n" );
                        code.Append( $"\"phase_1\" \"0\"\n" );
                        code.Append( $"\"model\" \"mdl/dev/mp_spawn.rmdl\"\n" );
                        code.Append( $"\"gamemode_tdm\" \"1\"\n" );
                        code.Append( $"\"gamemode_fw\" \"0\"\n" );
                        code.Append( $"\"gamemode_freelance\" \"0\"\n" );
                        code.Append( $"\"gamemode_ffa\" \"1\"\n" );
                        code.Append( $"\"gamemode_fd\" \"1\"\n" );
                        code.Append( $"\"gamemode_ctf\" \"1\"\n" );
                        code.Append( $"\"gamemode_cp\" \"1\"\n" );
                        code.Append( $"\"gamemode_at\" \"1\"\n" );
                        code.Append( $"\"control_teamnumber\" \"-1\"\n" );
                        code.Append( $"\"scale\" \"1\"\n" );
                        code.Append( $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"\n" );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"\n" );
                        code.Append( $"\"link_guid\" \"{Helper.GetRandomGUIDForEnt()}\"\n" );
                        code.Append( $"\"classname\" \"info_spawnpoint_human\"\n" );
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

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }
    }
}
