
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildSound
    {
        public static StringBuilder BuildSoundObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

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
                        code.Append(  "{\n" );

                        // Polyline segments
                        for ( int i = script.PolylineSegment.Length - 1 ; i > -1 ; i-- )
                        {
                            string polylineSegmentEnd = Helper.BuildOriginVector( script.PolylineSegment[i], true ).ToString().Replace(",", "");

                            if ( i != 0 )
                            {
                                string polylineSegmentStart = Helper.BuildOriginVector( script.PolylineSegment[i-1], true ).ToString().Replace(",", "");

                                code.Append( $"\"polyline_segment_{i}\" \"({polylineSegmentStart}) ({polylineSegmentEnd})\"" );
                            }
                            else code.Append( $"\"polyline_segment_{i}\" \"(0 0 0) ({polylineSegmentEnd})\"" );

                            PageBreak( ref code );
                        }

                        code.Append(  "\"radius\" \"0\"\n" );
                        code.Append(  "\"model\" \"mdl/dev/editor_ambient_generic_node.rmdl\"\n" );
                        code.Append( $"\"isWaveAmbient\" \"{isWaveAmbient}\"\n" );
                        code.Append( $"\"enabled\" \"{enabled}\"\n" );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"\n" );
                        code.Append( $"\"soundName\" \"{script.SoundName}\"\n" );
                        code.Append(  "\"classname\" \"ambient_generic\"\n" );
                        code.Append(  "}\n" );
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
