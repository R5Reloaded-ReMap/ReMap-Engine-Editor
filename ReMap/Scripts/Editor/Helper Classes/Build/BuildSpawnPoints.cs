
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

                case BuildType.LiveMap:
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
                        AppendCode( ref code, "{" );
                        AppendCode( ref code, $"\"teamnumber\" \"0\"" );
                        AppendCode( ref code, $"\"phase_9\" \"0\"" );
                        AppendCode( ref code, $"\"phase_8\" \"0\"" );
                        AppendCode( ref code, $"\"phase_7\" \"0\"" );
                        AppendCode( ref code, $"\"phase_6\" \"0\"" );
                        AppendCode( ref code, $"\"phase_5\" \"0\"" );
                        AppendCode( ref code, $"\"phase_4\" \"0\"" );
                        AppendCode( ref code, $"\"phase_3\" \"0\"" );
                        AppendCode( ref code, $"\"phase_2\" \"0\"" );
                        AppendCode( ref code, $"\"phase_1\" \"0\"" );
                        AppendCode( ref code, $"\"model\" \"mdl/dev/mp_spawn.rmdl\"" );
                        AppendCode( ref code, $"\"gamemode_tdm\" \"1\"" );
                        AppendCode( ref code, $"\"gamemode_fw\" \"0\"" );
                        AppendCode( ref code, $"\"gamemode_freelance\" \"0\"" );
                        AppendCode( ref code, $"\"gamemode_ffa\" \"1\"" );
                        AppendCode( ref code, $"\"gamemode_fd\" \"1\"" );
                        AppendCode( ref code, $"\"gamemode_ctf\" \"1\"" );
                        AppendCode( ref code, $"\"gamemode_cp\" \"1\"" );
                        AppendCode( ref code, $"\"gamemode_at\" \"1\"" );
                        AppendCode( ref code, $"\"control_teamnumber\" \"-1\"" );
                        AppendCode( ref code, $"\"scale\" \"1\"" );
                        AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"" );
                        AppendCode( ref code, $"\"link_guid\" \"{Helper.GetRandomGUIDForEnt()}\"" );
                        AppendCode( ref code, $"\"classname\" \"info_spawnpoint_human\"" );
                        AppendCode( ref code,  "}" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;
    
                    case BuildType.LiveMap:
                        // Remove 1 to the counter since we don't support this object for live map code
                        Helper.RemoveSendedEntityCount();
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

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            await Helper.Wait();

            return code;
        }
    }
}
