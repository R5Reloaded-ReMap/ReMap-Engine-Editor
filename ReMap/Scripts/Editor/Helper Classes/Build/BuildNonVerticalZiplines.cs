
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

                string PanelOrigin = BuildVerticalZipline.BuildPanelOriginArray( script.Panels );
                string PanelAngles = BuildVerticalZipline.BuildPanelAnglesArray( script.Panels );
                string PanelModels = BuildVerticalZipline.BuildPanelModelsArray( script.Panels );
                string LinkGuid = Helper.GetRandomGUIDForEnt();
                string LinkGuidTo0 = Helper.GetRandomGUIDForEnt();

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(script.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, {Helper.BuildOrigin(script.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, false, {Helper.ReplaceComma( script.FadeDistance )}, {Helper.ReplaceComma( script.Scale )}, {Helper.ReplaceComma( script.Width )}, {Helper.ReplaceComma( script.SpeedScale )}, {Helper.ReplaceComma( script.LengthScale )}, {PreserveVelocity}, {DropToBottom}, {Helper.ReplaceComma( script.AutoDetachStart )}, {Helper.ReplaceComma( script.AutoDetachEnd )}, {RestPoint}, {PushOffInDirectionX}, {IsMoving}, {DetachEndOnSpawn}, {DetachEndOnUse}, {PanelOrigin}, {PanelAngles}, {PanelModels}, {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )";
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code +=  "{"; PageBreak( ref code );
                        code += $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\""; PageBreak( ref code );
                        code += $"\"origin\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true, true )}\""; PageBreak( ref code );
                        code += $"\"link_guid\" \"{LinkGuid}\""; PageBreak( ref code );
                        code += $"\"ZiplineLengthScale\" \"{Helper.ReplaceComma( script.LengthScale )}\""; PageBreak( ref code );
                        code += $"\"ZiplineAutoDetachDistance\" \"{Helper.ReplaceComma( script.AutoDetachEnd )}\""; PageBreak( ref code );
                        code += $"\"classname\" \"zipline_end\""; PageBreak( ref code );
                        code +=  "}"; PageBreak( ref code );
                        code +=  "{"; PageBreak( ref code );

                        if ( script.RestPoint )
                        {
                            code += $"\"_zipline_rest_point_1\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true )}\""; PageBreak( ref code );
                            code += $"\"_zipline_rest_point_0\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true )}\""; PageBreak( ref code );
                        }

                        code += $"\"ZiplinePreserveVelocity\" \"{PreserveVelocity}\"";PageBreak( ref code );
                        code += $"\"ZiplineFadeDistance\" \"{Helper.ReplaceComma( script.FadeDistance )}\""; PageBreak( ref code );
                        code += $"\"ZiplineDropToBottom\" \"{DropToBottom}\""; PageBreak( ref code );
                        code += $"\"Width\" \"{Helper.ReplaceComma( script.Width )}\""; PageBreak( ref code );
                        code += $"\"Material\" \"cable/zipline.vmt\""; PageBreak( ref code );
                        code += $"\"gamemode_freedm\" \"1\""; PageBreak( ref code );
                        code += $"\"gamemode_control\" \"1\""; PageBreak( ref code );
                        code += $"\"gamemode_arenas\" \"1\""; PageBreak( ref code );
                        code += $"\"DetachEndOnUse\" \"{DetachEndOnUse}\""; PageBreak( ref code );
                        code += $"\"DetachEndOnSpawn\" \"{DetachEndOnSpawn}\""; PageBreak( ref code );
                        code += $"\"scale\" \"{Helper.ReplaceComma( script.Scale )}\""; PageBreak( ref code );
                        code += $"\"angles\" \"{Helper.BuildAngles( script.rope_start.gameObject, true )}\""; PageBreak( ref code );
                        code += $"\"origin\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true, true )}\""; PageBreak( ref code );
                        code += $"\"link_to_guid_0\" \"{LinkGuidTo0}\""; PageBreak( ref code );
                        code += $"\"link_guid\" \"{LinkGuid}\""; PageBreak( ref code );
                        code += $"\"ZiplineVertical\" \"0\""; PageBreak( ref code );
                        code += $"\"ZiplineVersion\" \"3\""; PageBreak( ref code );
                        code += $"\"ZiplineSpeedScale\" \"{Helper.ReplaceComma( script.SpeedScale )}\""; PageBreak( ref code );
                        code += $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\""; PageBreak( ref code );
                        code += $"\"ZiplineLengthScale\" \"{Helper.ReplaceComma( script.LengthScale )}\""; PageBreak( ref code );
                        code += $"\"ZiplineAutoDetachDistance\" \"{Helper.ReplaceComma( script.AutoDetachStart )}\""; PageBreak( ref code );
                        code += $"\"gamemode_survival\" \"1\""; PageBreak( ref code );
                        code += $"\"classname\" \"zipline\""; PageBreak( ref code );
                        code +=  "}"; PageBreak( ref code );
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
