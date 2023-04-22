
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildVerticalZipline
    {
        public static async Task< StringBuilder > BuildVerticalZipLineObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code.Append( "    // VerticalZipLines" );
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

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                DrawVerticalZipline script = ( DrawVerticalZipline ) Helper.GetComponentByEnum( obj, ObjectType.VerticalZipLine );
                if ( script == null ) continue;

                int PreserveVelocity = script.PreserveVelocity  ? 1 : 0;
                int DropToBottom = script.DropToBottom ? 1 : 0;
                int PushOffInDirectionX = script.PushOffInDirectionX ? 1 : 0;
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
                        code.Append( $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(script.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, {Helper.BuildOrigin(script.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, true, {Helper.ReplaceComma( script.FadeDistance )}, {Helper.ReplaceComma( script.Scale )}, {Helper.ReplaceComma( script.Width )}, {Helper.ReplaceComma( script.SpeedScale )}, {Helper.ReplaceComma( script.LengthScale )}, {PreserveVelocity}, {DropToBottom}, {Helper.ReplaceComma( script.AutoDetachStart )}, {Helper.ReplaceComma( script.AutoDetachEnd )}, {Helper.BoolToLower( script.RestPoint )}, {PushOffInDirectionX}, {Helper.BoolToLower( script.IsMoving )}, {DetachEndOnSpawn}, {DetachEndOnUse}, {PanelOrigin}, {PanelAngles}, {PanelModels}, {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )" );
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code.Append(  "{" ); PageBreak( ref code );
                        code.Append( $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\"" ); PageBreak( ref code );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true, true )}\"" ); PageBreak( ref code );
                        code.Append( $"\"link_guid\" \"{LinkGuid}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineLengthScale\" \"{Helper.ReplaceComma( script.LengthScale )}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineAutoDetachDistance\" \"{Helper.ReplaceComma( script.AutoDetachEnd )}\"" ); PageBreak( ref code );
                        code.Append( $"\"classname\" \"zipline_end\"" ); PageBreak( ref code );
                        code.Append(  "}" ); PageBreak( ref code );
                        code.Append(  "{" ); PageBreak( ref code );

                        if ( script.RestPoint )
                        {
                            code.Append( $"\"_zipline_rest_point_1\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true )}\"" ); PageBreak( ref code );
                            code.Append( $"\"_zipline_rest_point_0\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true )}\"" ); PageBreak( ref code );
                        }

                        code.Append( $"\"ZiplinePreserveVelocity\" \"{PreserveVelocity}\"" );PageBreak( ref code );
                        code.Append( $"\"ZiplineFadeDistance\" \"{Helper.ReplaceComma( script.FadeDistance )}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineDropToBottom\" \"{DropToBottom}\"" ); PageBreak( ref code );
                        code.Append( $"\"Width\" \"{Helper.ReplaceComma( script.Width )}\"" ); PageBreak( ref code );
                        code.Append( $"\"Material\" \"cable/zipline.vmt\"" ); PageBreak( ref code );
                        code.Append( $"\"gamemode_freedm\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"gamemode_control\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"gamemode_arenas\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"DetachEndOnUse\" \"{DetachEndOnUse}\"" ); PageBreak( ref code );
                        code.Append( $"\"DetachEndOnSpawn\" \"{DetachEndOnSpawn}\"" ); PageBreak( ref code );
                        code.Append( $"\"scale\" \"{Helper.ReplaceComma( script.Scale )}\"" ); PageBreak( ref code );
                        code.Append( $"\"angles\" \"{Helper.BuildAngles( script.rope_start.gameObject, true )}\"" ); PageBreak( ref code );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true, true )}\"" ); PageBreak( ref code );
                        code.Append( $"\"link_to_guid_0\" \"{LinkGuidTo0}\"" ); PageBreak( ref code );
                        code.Append( $"\"link_guid\" \"{LinkGuid}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineVertical\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineVersion\" \"3\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineSpeedScale\" \"{Helper.ReplaceComma( script.SpeedScale )}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineLengthScale\" \"{Helper.ReplaceComma( script.LengthScale )}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineAutoDetachDistance\" \"{Helper.ReplaceComma( script.AutoDetachStart )}\"" ); PageBreak( ref code );
                        code.Append( $"\"gamemode_survival\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"classname\" \"zipline\"" ); PageBreak( ref code );
                        code.Append(  "}" ); PageBreak( ref code );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapSetOrigin( {Helper.BuildOrigin(script.rope_start.gameObject, false, true)}, {Helper.BuildOrigin(script.rope_end.gameObject, false, true)} )" );
                        Helper.DelayInMS();
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapSetAngles( {Helper.BuildAngles(script.rope_start.gameObject)}, {Helper.BuildAngles(script.rope_start.gameObject)} )" );
                        Helper.DelayInMS();
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapSetZiplineVars01( true, {Helper.ReplaceComma( script.FadeDistance )}, {Helper.ReplaceComma( script.Scale )}, {Helper.ReplaceComma( script.Width )}, {Helper.ReplaceComma( script.SpeedScale )}, {Helper.ReplaceComma( script.LengthScale )}, {PreserveVelocity}, {DropToBottom} )" );
                        Helper.DelayInMS();
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapSetZiplineVars02( {Helper.ReplaceComma( script.AutoDetachStart )}, {Helper.ReplaceComma( script.AutoDetachEnd )}, {Helper.BoolToLower( script.RestPoint )}, {PushOffInDirectionX}, {Helper.BoolToLower( script.IsMoving )}, {DetachEndOnSpawn}, {DetachEndOnUse} )" );
                        Helper.DelayInMS();
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapSetRemapArrayVec01( {PanelOrigin} )" );
                        Helper.DelayInMS();
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapSetRemapArrayVec02( {PanelAngles} )" );
                        Helper.DelayInMS();
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapSetZiplinePanelModel( {PanelModels} )" );
                        Helper.DelayInMS();
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapSetZiplinePanelSettings( {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )");
                        Helper.DelayInMS();
                        CodeViewsWindow.LiveMap.SendCommandToApex( $"script ReMapCreateZipline()" );
                        Helper.DelayInMS();
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

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }

        internal static string BuildPanelOriginArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $" {Helper.BuildOrigin( objArray[i], false, true )}";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }

        internal static string BuildPanelAnglesArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $"{Helper.BuildAngles( objArray[i] )}";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }

        internal static string BuildPanelModelsArray( GameObject[] objArray )
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
