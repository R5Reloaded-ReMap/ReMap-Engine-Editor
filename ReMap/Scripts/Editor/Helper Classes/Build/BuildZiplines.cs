using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildZipline
    {
        public static async Task< StringBuilder > BuildZiplineObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();
            var precacheList = new List< string >();

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
            foreach ( var obj in objectData )
            {
                var script = ( DrawZipline )Helper.GetComponentByEnum( obj, ObjectType.ZipLine );
                if ( script == null ) continue;

                //string model = "custom_zipline" );
                string ziplinestart = "";
                string ziplineend = "";

                foreach ( Transform child in obj.transform )
                    if ( buildType == BuildType.LiveMap )
                    {
                        if ( child.name == "zipline_start" ) ziplinestart = Helper.BuildOrigin( child.gameObject, false, true );
                        if ( child.name == "zipline_end" ) ziplineend = Helper.BuildOrigin( child.gameObject, false, true );
                    }
                    else
                    {
                        if ( child.name == "zipline_start" ) ziplinestart = Helper.BuildOrigin( child.gameObject );
                        if ( child.name == "zipline_end" ) ziplineend = Helper.BuildOrigin( child.gameObject );
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
                        LiveMap.AddToGameQueue( $"MapEditor_CreateZipline( {ziplinestart}, {ziplineend}, true )" );
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