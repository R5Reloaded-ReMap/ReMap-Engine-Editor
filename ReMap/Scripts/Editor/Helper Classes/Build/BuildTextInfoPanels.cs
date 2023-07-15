
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildTextInfoPanel
    {
        public static async Task< StringBuilder > BuildTextInfoPanelObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Text Info Panels" );
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
                TextInfoPanelScript script = ( TextInfoPanelScript ) Helper.GetComponentByEnum( obj, ObjectType.TextInfoPanel );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_CreateTextInfoPanel( \"{script.Title}\", \"{script.Description}\", {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {Helper.BoolToLower( script.showPIN )}, {Helper.ReplaceComma( script.Scale )} )" + "\n" );
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
                        CodeViews.LiveMap.AddToGameQueue( $"MapEditor_CreateTextInfoPanel(\"{script.Title}\",\"{script.Description}\",{Helper.BuildOrigin( obj, false, true )},{Helper.BuildAngles( obj )},{Helper.BoolToLower( script.showPIN )},{Helper.ReplaceComma( script.Scale )})" );
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
    }
}
