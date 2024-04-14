using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildJumpTower
    {
        public static async Task< StringBuilder > BuildJumpTowerObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Jump Towers" );
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
            foreach ( var obj in objectData )
            {
                var script = ( JumpTowerScript )Helper.GetComponentByEnum( obj, ObjectType.JumpTower );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    ReMapCreateJumpTower( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {Helper.ReplaceComma( script.Height )} )" );
                        break;

                    case BuildType.EntFile:
                        //AppendCode( ref code, BuildJumpTowerEntCode( obj, script ) );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        LiveMap.AddToGameQueue( $"ReMapCreateJumpTower({Helper.BuildOrigin( obj, false, true )},{Helper.BuildAngles( obj )},{Helper.ReplaceComma( script.Height )},true)" );
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

            await Helper.Wait();

            return code;
        }

        private static string BuildJumpTowerEntCode( GameObject obj, JumpTowerScript script )
        {
            var code = new StringBuilder();

            string link_guid_balloon = Helper.GetRandomGUIDForEnt();
            string link_guid_balloon_0 = Helper.GetRandomGUIDForEnt();
            string link_guid_zipline = Helper.GetRandomGUIDForEnt();
            string link_guid_zipline_0 = Helper.GetRandomGUIDForEnt();

            AppendCode( ref code, "{" );
            AppendCode( ref code, "\"SuppressAnimSounds\" \"0\"" );
            AppendCode( ref code, "\"StartDisabled\" \"0\"" );
            AppendCode( ref code, "\"spawnflags\" \"0\"" );
            AppendCode( ref code, "\"solid\" \"6\"" );
            AppendCode( ref code, "\"skin\" \"0\"" );
            AppendCode( ref code, "\"SetBodyGroup\" \"0\"" );
            AppendCode( ref code, "\"rendermode\" \"0\"" );
            AppendCode( ref code, "\"renderfx\" \"0\"" );
            AppendCode( ref code, "\"rendercolor\" \"255 255 255\"" );
            AppendCode( ref code, "\"renderamt\" \"255\"" );
            AppendCode( ref code, "\"RandomAnimation\" \"0\"" );
            AppendCode( ref code, "\"pressuredelay\" \"0\"" );
            AppendCode( ref code, "\"PerformanceMode\" \"0\"" );
            AppendCode( ref code, "\"mingpulevel\" \"0\"" );
            AppendCode( ref code, "\"mincpulevel\" \"0\"" );
            AppendCode( ref code, "\"MinAnimTime\" \"5\"" );
            AppendCode( ref code, "\"maxgpulevel\" \"0\"" );
            AppendCode( ref code, "\"maxcpulevel\" \"0\"" );
            AppendCode( ref code, "\"MaxAnimTime\" \"10\"" );
            AppendCode( ref code, "\"HoldAnimation\" \"0\"" );
            AppendCode( ref code, "\"gamemode_tdm\" \"1\"" );
            AppendCode( ref code, "\"gamemode_sur\" \"1\"" );
            AppendCode( ref code, "\"gamemode_lts\" \"1\"" );
            AppendCode( ref code, "\"gamemode_lh\" \"1\"" );
            AppendCode( ref code, "\"gamemode_fd\" \"1\"" );
            AppendCode( ref code, "\"gamemode_ctf\" \"1\"" );
            AppendCode( ref code, "\"gamemode_cp\" \"1\"" );
            AppendCode( ref code, "\"fadedist\" \"-1\"" );
            AppendCode( ref code, "\"ExplodeRadius\" \"0\"" );
            AppendCode( ref code, "\"ExplodeDamage\" \"0\"" );
            AppendCode( ref code, "\"disableX360\" \"0\"" );
            AppendCode( ref code, "\"disableshadows\" \"0\"" );
            AppendCode( ref code, "\"disablereceiveshadows\" \"0\"" );
            AppendCode( ref code, "\"DisableBoneFollowers\" \"0\"" );
            AppendCode( ref code, "\"DefaultCycle\" \"0\"" );
            AppendCode( ref code, "\"collide_titan\" \"1\"" );
            AppendCode( ref code, "\"collide_ai\" \"1\"" );
            AppendCode( ref code, "\"ClientSide\" \"0\"" );
            AppendCode( ref code, "\"AnimateInStaticShadow\" \"0\"" );
            AppendCode( ref code, "\"scale\" \"1\"" );
            AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( script.ballon_base.gameObject, true )}\"" );
            AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( script.ballon_base.gameObject, true, true )}\"" );
            AppendCode( ref code, $"\"link_to_guid_0\" \"{link_guid_balloon_0}\"" );
            AppendCode( ref code, $"\"link_guid\" \"{link_guid_balloon}\"" );
            AppendCode( ref code, "\"script_name\" \"jump_tower\"" );
            AppendCode( ref code, "\"model\" \"mdl/props/zipline_balloon/zipline_balloon_base.rmdl\"" );
            AppendCode( ref code, "\"classname\" \"prop_dynamic\"" );
            AppendCode( ref code, "}" );
            AppendCode( ref code, "{" );
            AppendCode( ref code, "\"SuppressAnimSounds\" \"0\"" );
            AppendCode( ref code, "\"StartDisabled\" \"0\"" );
            AppendCode( ref code, "\"spawnflags\" \"0\"" );
            AppendCode( ref code, "\"solid\" \"6\"" );
            AppendCode( ref code, "\"skin\" \"0\"" );
            AppendCode( ref code, "\"SetBodyGroup\" \"0\"" );
            AppendCode( ref code, "\"rendermode\" \"0\"" );
            AppendCode( ref code, "\"renderfx\" \"0\"" );
            AppendCode( ref code, "\"rendercolor\" \"255 255 255\"" );
            AppendCode( ref code, "\"renderamt\" \"255\"" );
            AppendCode( ref code, "\"RandomAnimation\" \"0\"" );
            AppendCode( ref code, "\"pressuredelay\" \"0\"" );
            AppendCode( ref code, "\"PerformanceMode\" \"0\"" );
            AppendCode( ref code, "\"mingpulevel\" \"0\"" );
            AppendCode( ref code, "\"mincpulevel\" \"0\"" );
            AppendCode( ref code, "\"MinAnimTime\" \"5\"" );
            AppendCode( ref code, "\"maxgpulevel\" \"0\"" );
            AppendCode( ref code, "\"maxcpulevel\" \"0\"" );
            AppendCode( ref code, "\"MaxAnimTime\" \"10\"" );
            AppendCode( ref code, "\"HoldAnimation\" \"0\"" );
            AppendCode( ref code, "\"gamemode_tdm\" \"1\"" );
            AppendCode( ref code, "\"gamemode_sur\" \"1\"" );
            AppendCode( ref code, "\"gamemode_lts\" \"1\"" );
            AppendCode( ref code, "\"gamemode_lh\" \"1\"" );
            AppendCode( ref code, "\"gamemode_fd\" \"1\"" );
            AppendCode( ref code, "\"gamemode_ctf\" \"1\"" );
            AppendCode( ref code, "\"gamemode_cp\" \"1\"" );
            AppendCode( ref code, "\"fadedist\" \"-1\"" );
            AppendCode( ref code, "\"ExplodeRadius\" \"0\"" );
            AppendCode( ref code, "\"ExplodeDamage\" \"0\"" );
            AppendCode( ref code, "\"disableX360\" \"0\"" );
            AppendCode( ref code, "\"disableshadows\" \"0\"" );
            AppendCode( ref code, "\"disablereceiveshadows\" \"0\"" );
            AppendCode( ref code, "\"DisableBoneFollowers\" \"0\"" );
            AppendCode( ref code, "\"DefaultCycle\" \"0\"" );
            AppendCode( ref code, "\"collide_titan\" \"1\"" );
            AppendCode( ref code, "\"collide_ai\" \"1\"" );
            AppendCode( ref code, "\"ClientSide\" \"0\"" );
            AppendCode( ref code, "\"AnimateInStaticShadow\" \"0\"" );
            AppendCode( ref code, "\"scale\" \"1\"" );
            AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( script.ballon_top.gameObject, true )}\"" );
            AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( script.ballon_top.gameObject, true, true )}\"" );
            AppendCode( ref code, $"\"link_guid\" \"{link_guid_balloon_0}\"" );
            AppendCode( ref code, "\"script_name\" \"jump_tower\"" );
            AppendCode( ref code, "\"model\" \"mdl/props/zipline_balloon/zipline_balloon.rmdl\"" );
            AppendCode( ref code, "\"classname\" \"prop_dynamic\"" );
            AppendCode( ref code, "}" );
            AppendCode( ref code, "{" );
            AppendCode( ref code, $"\"_zipline_rest_point_1\" \"{Helper.BuildOrigin( script.ballon_top.gameObject, true, true )}\"" );
            AppendCode( ref code, $"\"_zipline_rest_point_0\" \"{Helper.BuildOrigin( script.ballon_base.position + new Vector3( 2.75f, 64, 2 ), true, true )}\"" );
            AppendCode( ref code, "\"ZiplineSpeedScale\" \"1\"" );
            AppendCode( ref code, "\"ZiplinePushOffInDirectionX\" \"0\"" );
            AppendCode( ref code, "\"ZiplineLengthScale\" \"1\"" );
            AppendCode( ref code, "\"ZiplineDropToBottom\" \"1\"" );
            AppendCode( ref code, "\"ZiplineAutoDetachDistance\" \"350\"" );
            AppendCode( ref code, "\"Width\" \"2\"" );
            AppendCode( ref code, "\"useAutoDetachSpeed\" \"0\"" );
            AppendCode( ref code, "\"Material\" \"cable/zipline.vmt\"" );
            AppendCode( ref code, "\"gamemode_survival\" \"1\"" );
            AppendCode( ref code, "\"gamemode_freedm\" \"1\"" );
            AppendCode( ref code, "\"gamemode_control\" \"1\"" );
            AppendCode( ref code, "\"gamemode_arenas\" \"1\"" );
            AppendCode( ref code, "\"DetachEndOnUse\" \"0\"" );
            AppendCode( ref code, "\"DetachEndOnSpawn\" \"0\"" );
            AppendCode( ref code, "\"scale\" \"1\"" );
            AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( script.ballon_base.gameObject, true )}\"" );
            AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( script.ballon_base.position + new Vector3( 2.75f, 64, 2 ), true, true )}\"" );
            AppendCode( ref code, $"\"link_to_guid_0\" \"{link_guid_zipline_0}\"" );
            AppendCode( ref code, $"\"link_guid\" \"{link_guid_zipline}\"" );
            AppendCode( ref code, "\"ZiplineVertical\" \"1\"" );
            AppendCode( ref code, "\"ZiplineVersion\" \"3\"" );
            AppendCode( ref code, "\"ZiplinePreserveVelocity\" \"1\"" );
            AppendCode( ref code, "\"ZiplineFadeDistance\" \"20000\"" );
            AppendCode( ref code, "\"script_name\" \"skydive_tower\"" );
            AppendCode( ref code, "\"classname\" \"zipline\"" );
            AppendCode( ref code, "}" );
            AppendCode( ref code, "{" );
            AppendCode( ref code, "\"ZiplineSpeedScale\" \"1\"" );
            AppendCode( ref code, "\"ZiplinePushOffInDirectionX\" \"0\"" );
            AppendCode( ref code, "\"ZiplineLengthScale\" \"1\"" );
            AppendCode( ref code, "\"ZiplineDropToBottom\" \"1\"" );
            AppendCode( ref code, "\"Width\" \"2\"" );
            AppendCode( ref code, "\"useAutoDetachSpeed\" \"0\"" );
            AppendCode( ref code, "\"Material\" \"cable/zipline.vmt\"" );
            AppendCode( ref code, "\"gamemode_survival\" \"1\"" );
            AppendCode( ref code, "\"gamemode_freedm\" \"1\"" );
            AppendCode( ref code, "\"gamemode_control\" \"1\"" );
            AppendCode( ref code, "\"gamemode_arenas\" \"1\"" );
            AppendCode( ref code, "\"DetachEndOnUse\" \"0\"" );
            AppendCode( ref code, "\"DetachEndOnSpawn\" \"0\"" );
            AppendCode( ref code, "\"scale\" \"1\"" );
            AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( script.ballon_top.gameObject, true )}\"" );
            AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( script.ballon_top.gameObject, true, true )}\"" );
            AppendCode( ref code, $"\"link_guid\" \"{link_guid_zipline_0}\"" );
            AppendCode( ref code, "\"ZiplineVertical\" \"1\"" );
            AppendCode( ref code, "\"ZiplinePreserveVelocity\" \"1\"" );
            AppendCode( ref code, "\"ZiplineFadeDistance\" \"20000\"" );
            AppendCode( ref code, "\"ZiplineAutoDetachDistance\" \"100\"" );
            AppendCode( ref code, "\"script_name\" \"skydive_tower\"" );
            AppendCode( ref code, "\"classname\" \"zipline\"" );
            AppendCode( ref code, "}", 0 );
            //AppendCode( ref code,  "{" );

            return code.ToString();
        }
    }
}