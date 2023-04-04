
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildHorzDoor
    {
        public static async Task< StringBuilder > BuildHorzDoorObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code.Append( "    // Horizontal Doors" );
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
                        code.Append( $"    MapEditor_SpawnDoor( {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, eMapEditorDoorType.Horizontal )" );
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                            code.Append(  "{\n" );
                            code.Append( $"\"SuppressAnimSounds\" \"0\"\n" );
                            code.Append( $"\"StartDisabled\" \"0\"\n" );
                            code.Append( $"\"spawnflags\" \"0\"\n" );
                            code.Append( $"\"solid\" \"6\"\n" );
                            code.Append( $"\"skin\" \"0\"\n" );
                            code.Append( $"\"SetBodyGroup\" \"0\"\n" );
                            code.Append( $"\"rendermode\" \"0\"\n" );
                            code.Append( $"\"renderfx\" \"0\"\n" );
                            code.Append( $"\"rendercolor\" \"255 255 255\"\n" );
                            code.Append( $"\"renderamt\" \"255\"\n" );
                            code.Append( $"\"RandomAnimation\" \"0\"\n" );
                            code.Append( $"\"pressuredelay\" \"0\"\n" );
                            code.Append( $"\"PerformanceMode\" \"0\"\n" );
                            code.Append( $"\"mingpulevel\" \"0\"\n" );
                            code.Append( $"\"mincpulevel\" \"0\"\n" );
                            code.Append( $"\"MinAnimTime\" \"5\"\n" );
                            code.Append( $"\"maxgpulevel\" \"0\"\n" );
                            code.Append( $"\"maxcpulevel\" \"0\"\n" );
                            code.Append( $"\"MaxAnimTime\" \"10\"\n" );
                            code.Append( $"\"HoldAnimation\" \"0\"\n" );
                            code.Append( $"\"gamemode_tdm\" \"1\"\n" );
                            code.Append( $"\"gamemode_sur\" \"1\"\n" );
                            code.Append( $"\"gamemode_lts\" \"1\"\n" );
                            code.Append( $"\"gamemode_lh\" \"1\"\n" );
                            code.Append( $"\"gamemode_fd\" \"1\"\n" );
                            code.Append( $"\"gamemode_ctf\" \"1\"\n" );
                            code.Append( $"\"gamemode_cp\" \"1\"\n" );
                            code.Append( $"\"fadedist\" \"-1\"\n" );
                            code.Append( $"\"ExplodeRadius\" \"0\"\n" );
                            code.Append( $"\"ExplodeDamage\" \"0\"\n" );
                            code.Append( $"\"disableX360\" \"0\"\n" );
                            code.Append( $"\"disableshadows\" \"0\"\n" );
                            code.Append( $"\"disablereceiveshadows\" \"0\"\n" );
                            code.Append( $"\"DisableBoneFollowers\" \"0\"\n" );
                            code.Append( $"\"DefaultCycle\" \"0\"\n" );
                            code.Append( $"\"collide_titan\" \"1\"\n" );
                            code.Append( $"\"collide_ai\" \"1\"\n" );
                            code.Append( $"\"ClientSide\" \"0\"\n" );
                            code.Append( $"\"AnimateInStaticShadow\" \"0\"\n" );
                            code.Append( $"\"scale\" \"1\"\n" );
                            code.Append( $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"\n" );
                            code.Append( $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"\n" );
                            code.Append( $"\"script_name\" \"survival_door_plain\"\n" );
                            code.Append( $"\"model\" \"mdl/door/door_256x256x8_elevatorstyle02_animated.rmdl\"\n" );
                            code.Append( $"\"classname\" \"prop_dynamic\"\n" );
                            code.Append(  "}\n" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                    break;

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.SendCommandToApex($"script MapEditor_SpawnDoor( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles(obj)}, eMapEditorDoorType.Horizontal, false, true )");
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
            }

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }
    }
}
