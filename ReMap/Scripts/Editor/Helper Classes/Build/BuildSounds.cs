
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildSound
    {
        public static string BuildSoundObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    // Empty
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
                SoundScript script = ( SoundScript ) Helper.GetComponentByEnum( obj, ObjectType.Sound );
                if ( script == null ) continue;

                string isWaveAmbient = script.IsWaveAmbient ? "1" : "0";
                string enabled = script.Enable ? "1" : "0";

                switch ( buildType )
                {
                    case BuildType.Script:
                        // Empty
                        break;

                    case BuildType.EntFile:
                        code +=  "{\n";

                        // Polyline segments
                        for ( int i = script.PolylineSegment.Length - 1 ; i > -1 ; i-- )
                        {
                            string polylineSegmentEnd = Helper.BuildOriginVector( script.PolylineSegment[i], true ).ToString().Replace(",", "");

                            if ( i != 0 )
                            {
                                string polylineSegmentStart = Helper.BuildOriginVector( script.PolylineSegment[i-1], true ).ToString().Replace(",", "");

                                code += $"\"polyline_segment_{i}\" \"({polylineSegmentStart}) ({polylineSegmentEnd})\"\n";
                            }
                            else code += $"\"polyline_segment_{i}\" \"(0 0 0) ({polylineSegmentEnd})\"\n";
                        }

                        code +=  "\"radius\" \"0\"\n";
                        code +=  "\"model\" \"mdl/dev/editor_ambient_generic_node.rmdl\"\n";
                        code += $"\"isWaveAmbient\" \"{isWaveAmbient}\"\n";
                        code += $"\"enabled\" \"{enabled}\"\n";
                        code += $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"\n";
                        code += $"\"soundName\" \"{script.SoundName}\"\n";
                        code +=  "\"classname\" \"ambient_generic\"\n";
                        code +=  "}\n";
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
                    // Empty
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
