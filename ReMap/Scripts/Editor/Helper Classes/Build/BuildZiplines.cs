
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;
using static ImportExport.Shared.SharedFunction;

namespace Build
{
    public class BuildZipline
    {
        public static string BuildZiplineObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";
            List< String > precacheList = new List< String >();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    //Ziplines";
                    code += "\n";
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
                DrawZipline script = ( DrawZipline ) Helper.GetComponentByEnum( obj, ObjectType.ZipLine );
                if ( script == null ) continue;

                //string model = "custom_zipline";
                string ziplinestart = "";
                string ziplineend = "";

                foreach ( Transform child in obj.transform )
                {
                    if ( child.name == "zipline_start" ) ziplinestart = Helper.BuildOrigin( child.gameObject );
                    if ( child.name == "zipline_end" ) ziplineend = Helper.BuildOrigin( child.gameObject );
                }

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    CreateZipline( {ziplinestart + Helper.ShouldAddStartingOrg()}, {ziplineend + Helper.ShouldAddStartingOrg()} )";
                        code += "\n";
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
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "\n";
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
