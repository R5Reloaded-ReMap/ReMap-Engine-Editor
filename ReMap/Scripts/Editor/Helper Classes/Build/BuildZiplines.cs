
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;
using static ImportExport.SharedFunction;

namespace Build
{
    public class BuildZipline
    {
        public static async Task< StringBuilder > BuildZiplineObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();
            List< String > precacheList = new List< String >();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Ziplines" );
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
                DrawZipline script = ( DrawZipline ) Helper.GetComponentByEnum( obj, ObjectType.ZipLine );
                if ( script == null ) continue;

                //string model = "custom_zipline" );
                string ziplinestart = "";
                string ziplineend = "";

                foreach ( Transform child in obj.transform )
                {
                    if(buildType == BuildType.LiveMap)
                    {
                        if ( child.name == "zipline_start" ) ziplinestart = Helper.BuildOrigin( child.gameObject, false, true );
                        if ( child.name == "zipline_end" ) ziplineend = Helper.BuildOrigin( child.gameObject, false, true );
                    }
                    else
                    {
                        if ( child.name == "zipline_start" ) ziplinestart = Helper.BuildOrigin( child.gameObject );
                        if ( child.name == "zipline_end" ) ziplineend = Helper.BuildOrigin( child.gameObject );
                    }
                }

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_CreateZipline( {ziplinestart + Helper.ShouldAddStartingOrg()}, {ziplineend + Helper.ShouldAddStartingOrg()} )" );
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
                        CodeViewsWindow.LiveMap.AddToGameQueue($"script MapEditor_CreateZipline( {ziplinestart}, {ziplineend}, true )");
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
