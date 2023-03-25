
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildNonVerticalZipline
    {
        public static string BuildNonVerticalZipLineObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // NonVerticalZipLines";
                    PageBreak( ref code );
                    break;
                    
                case BuildType.EntFile:
                    break;

                case BuildType.Precache:
                    break;

                case BuildType.DataTable:
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                DrawNonVerticalZipline script = ( DrawNonVerticalZipline ) Helper.GetComponentByEnum( obj, ObjectType.NonVerticalZipLine );
                if ( script == null ) continue;

                int PreserveVelocity = script.PreserveVelocity  ? 1 : 0;
                int DropToBottom = script.DropToBottom ? 1 : 0;
                string RestPoint = script.RestPoint.ToString().ToLower();
                int PushOffInDirectionX = script.PushOffInDirectionX ? 1 : 0;
                string IsMoving = script.IsMoving.ToString().ToLower();
                int DetachEndOnSpawn = script.DetachEndOnSpawn ? 1 : 0;
                int DetachEndOnUse = script.DetachEndOnUse ? 1 : 0;
                float PanelTimerMin = script.PanelTimerMin;
                float PanelTimerMax = script.PanelTimerMax;
                int PanelMaxUse = script.PanelMaxUse;

                string PanelOrigin = BuildPanelOriginArray( script.Panels );
                string PanelAngles = BuildPanelAnglesArray( script.Panels );
                string PanelModels = BuildPanelModelsArray( script.Panels );
                string LinkGuid = Helper.GetRandomGUIDForEnt();
                string LinkGuidTo0 = Helper.GetRandomGUIDForEnt();

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(script.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, {Helper.BuildOrigin(script.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, false, {script.FadeDistance.ToString().Replace(",", ".")}, {script.Scale.ToString().Replace(",", ".")}, {script.Width.ToString().Replace(",", ".")}, {script.SpeedScale.ToString().Replace(",", ".")}, {script.LengthScale.ToString().Replace(",", ".")}, {PreserveVelocity}, {DropToBottom}, {script.AutoDetachStart.ToString().Replace(",", ".")}, {script.AutoDetachEnd.ToString().Replace(",", ".")}, {RestPoint}, {PushOffInDirectionX}, {IsMoving}, {DetachEndOnSpawn}, {DetachEndOnUse}, {PanelOrigin}, {PanelAngles}, {PanelModels}, {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )" + "\n";
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code +=  "{"; PageBreak( ref code );
                        code += $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\""; PageBreak( ref code );
                        code += $"\"origin\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true )}\""; PageBreak( ref code );
                        code += $"\"link_guid\" \"{LinkGuid}\""; PageBreak( ref code );
                        code += $"\"ZiplineLengthScale\" \"{script.LengthScale}\""; PageBreak( ref code );
                        code += $"\"ZiplineAutoDetachDistance\" \"{script.AutoDetachEnd}\""; PageBreak( ref code );
                        code += $"\"classname\" \"zipline_end\""; PageBreak( ref code );
                        code +=  "}"; PageBreak( ref code );
                        code +=  "{"; PageBreak( ref code );

                        if ( script.RestPoint )
                        {
                            code += $"\"_zipline_rest_point_1\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true )}\""; PageBreak( ref code );
                            code += $"\"_zipline_rest_point_0\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true )}\""; PageBreak( ref code );
                        }

                        code += $"\"ZiplinePreserveVelocity\" \"{PreserveVelocity}\"";PageBreak( ref code );
                        code += $"\"ZiplineFadeDistance\" \"{script.FadeDistance}\""; PageBreak( ref code );
                        code += $"\"ZiplineDropToBottom\" \"{DropToBottom}\""; PageBreak( ref code );
                        code += $"\"Width\" \"{script.Width}\""; PageBreak( ref code );
                        code += $"\"Material\" \"cable/zipline.vmt\""; PageBreak( ref code );
                        code += $"\"gamemode_freedm\" \"1\""; PageBreak( ref code );
                        code += $"\"gamemode_control\" \"1\""; PageBreak( ref code );
                        code += $"\"gamemode_arenas\" \"1\""; PageBreak( ref code );
                        code += $"\"DetachEndOnUse\" \"{DetachEndOnUse}\""; PageBreak( ref code );
                        code += $"\"DetachEndOnSpawn\" \"{DetachEndOnSpawn}\""; PageBreak( ref code );
                        code += $"\"scale\" \"{script.Scale}\""; PageBreak( ref code );
                        code += $"\"angles\" \"{Helper.BuildAngles( script.rope_start.gameObject, true )}\""; PageBreak( ref code );
                        code += $"\"origin\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true, true )}\""; PageBreak( ref code );
                        code += $"\"link_to_guid_0\" \"{LinkGuidTo0}\""; PageBreak( ref code );
                        code += $"\"link_guid\" \"{LinkGuid}\""; PageBreak( ref code );
                        code += $"\"ZiplineVertical\" \"0\""; PageBreak( ref code );
                        code += $"\"ZiplineVersion\" \"3\""; PageBreak( ref code );
                        code += $"\"ZiplineSpeedScale\" \"{script.SpeedScale}\""; PageBreak( ref code );
                        code += $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\""; PageBreak( ref code );
                        code += $"\"ZiplineLengthScale\" \"{script.LengthScale}\""; PageBreak( ref code );
                        code += $"\"ZiplineAutoDetachDistance\" \"{script.AutoDetachStart}\""; PageBreak( ref code );
                        code += $"\"gamemode_survival\" \"1\""; PageBreak( ref code );
                        code += $"\"classname\" \"zipline\""; PageBreak( ref code );
                        code +=  "}"; PageBreak( ref code );
                        break;

                    case BuildType.Precache:
                        break;

                    case BuildType.DataTable:
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
                    break;

                case BuildType.Precache:
                    break;

                case BuildType.DataTable:
                break;
            }

            return code;
        }

        private static string BuildPanelOriginArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $" {Helper.BuildOrigin( objArray[i] )}{Helper.ShouldAddStartingOrg()}";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }

        private static string BuildPanelAnglesArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $"{ Helper.BuildAngles( objArray[i] )}";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }

        private static string BuildPanelModelsArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $"$\"{UnityInfo.GetApexModelName( "mdl/" + objArray[i].name, true )}\"";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }
    }
}
