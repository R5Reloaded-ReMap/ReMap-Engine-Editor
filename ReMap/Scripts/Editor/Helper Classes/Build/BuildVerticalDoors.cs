
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildVerticalDoor
    {
        public static async Task< StringBuilder > BuildVerticalDoorObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Vertical Doors" );
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
                VerticalDoorScript script = ( VerticalDoorScript ) Helper.GetComponentByEnum( obj, ObjectType.VerticalDoor );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, eMapEditorDoorType.Vertical, false, {Helper.BoolToLower( script.AppearOpen )} )" );
                        break;

                    case BuildType.EntFile:
                            AppendCode( ref code,  "{" );
                            AppendCode( ref code, $"\"SuppressAnimSounds\" \"0\"" );
                            AppendCode( ref code, $"\"StartDisabled\" \"0\"" );
                            AppendCode( ref code, $"\"spawnflags\" \"0\"" );
                            AppendCode( ref code, $"\"solid\" \"6\"" );
                            AppendCode( ref code, $"\"skin\" \"0\"" );
                            AppendCode( ref code, $"\"SetBodyGroup\" \"0\"" );
                            AppendCode( ref code, $"\"rendermode\" \"0\"" );
                            AppendCode( ref code, $"\"renderfx\" \"0\"" );
                            AppendCode( ref code, $"\"rendercolor\" \"255 255 255\"" );
                            AppendCode( ref code, $"\"renderamt\" \"255\"" );
                            AppendCode( ref code, $"\"RandomAnimation\" \"0\"" );
                            AppendCode( ref code, $"\"pressuredelay\" \"0\"" );
                            AppendCode( ref code, $"\"PerformanceMode\" \"0\"" );
                            AppendCode( ref code, $"\"mingpulevel\" \"0\"" );
                            AppendCode( ref code, $"\"mincpulevel\" \"0\"" );
                            AppendCode( ref code, $"\"MinAnimTime\" \"5\"" );
                            AppendCode( ref code, $"\"maxgpulevel\" \"0\"" );
                            AppendCode( ref code, $"\"maxcpulevel\" \"0\"" );
                            AppendCode( ref code, $"\"MaxAnimTime\" \"10\"" );
                            AppendCode( ref code, $"\"HoldAnimation\" \"0\"" );
                            AppendCode( ref code, $"\"gamemode_tdm\" \"1\"" );
                            AppendCode( ref code, $"\"gamemode_sur\" \"1\"" );
                            AppendCode( ref code, $"\"gamemode_lts\" \"1\"" );
                            AppendCode( ref code, $"\"gamemode_lh\" \"1\"" );
                            AppendCode( ref code, $"\"gamemode_fd\" \"1\"" );
                            AppendCode( ref code, $"\"gamemode_ctf\" \"1\"" );
                            AppendCode( ref code, $"\"gamemode_cp\" \"1\"" );
                            AppendCode( ref code, $"\"fadedist\" \"-1\"" );
                            AppendCode( ref code, $"\"ExplodeRadius\" \"0\"" );
                            AppendCode( ref code, $"\"ExplodeDamage\" \"0\"" );
                            AppendCode( ref code, $"\"disableX360\" \"0\"" );
                            AppendCode( ref code, $"\"disableshadows\" \"0\"" );
                            AppendCode( ref code, $"\"disablereceiveshadows\" \"0\"" );
                            AppendCode( ref code, $"\"DisableBoneFollowers\" \"0\"" );
                            AppendCode( ref code, $"\"DefaultCycle\" \"0\"" );
                            AppendCode( ref code, $"\"collide_titan\" \"1\"" );
                            AppendCode( ref code, $"\"collide_ai\" \"1\"" );
                            AppendCode( ref code, $"\"ClientSide\" \"0\"" );
                            AppendCode( ref code, $"\"AnimateInStaticShadow\" \"0\"" );
                            AppendCode( ref code, $"\"scale\" \"1\"" );
                            AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"" );
                            AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( obj, true )}\"" );
                            AppendCode( ref code, $"\"script_name\" \"survival_door_plain\"" );
                            AppendCode( ref code, $"\"model\" \"mdl/door/door_canyonlands_large_01_animated.rmdl\"" );
                            AppendCode( ref code, $"\"classname\" \"prop_dynamic\"" );
                            AppendCode( ref code,  "}" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        CodeViews.LiveMap.AddToGameQueue( $"MapEditor_SpawnDoor( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles(obj)}, eMapEditorDoorType.Vertical, false, {Helper.BoolToLower( script.AppearOpen )}, true )" );
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
