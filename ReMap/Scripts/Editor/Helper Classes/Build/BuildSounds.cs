using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildSound
    {
        public static async Task< StringBuilder > BuildSoundObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

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

                case BuildType.LiveMap:
                    // Empty
                    break;
            }

            // Build the code
            foreach ( var obj in objectData )
            {
                var script = ( SoundScript ) Helper.GetComponentByEnum( obj, ObjectType.Sound );
                if ( script == null ) continue;

                string isWaveAmbient = script.IsWaveAmbient ? "1" : "0";
                string enabled = script.Enable ? "1" : "0";

                switch ( buildType )
                {
                    case BuildType.Script:
                        // Empty
                        break;

                    case BuildType.EntFile:
                        AppendCode( ref code, "{" );

                        // Polyline segments
                        for ( int i = script.PolylineSegment.Length - 1; i > -1; i-- )
                        {
                            string polylineSegmentEnd = Helper.BuildOrigin( script.PolylineSegment[ i ], true ).Replace( ",", "" );

                            if ( i != 0 )
                            {
                                string polylineSegmentStart = Helper.BuildOrigin( script.PolylineSegment[ i - 1 ], true ).Replace( ",", "" );

                                AppendCode( ref code, $"\"polyline_segment_{i}\" \"({polylineSegmentStart}) ({polylineSegmentEnd})\"" );
                            }
                            else
                            {
                                AppendCode( ref code, $"\"polyline_segment_{i}\" \"(0 0 0) ({polylineSegmentEnd})\"" );
                            }
                        }

                        AppendCode( ref code, "\"radius\" \"0\"" );
                        AppendCode( ref code, "\"model\" \"mdl/dev/editor_ambient_generic_node.rmdl\"" );
                        AppendCode( ref code, $"\"isWaveAmbient\" \"{isWaveAmbient}\"" );
                        AppendCode( ref code, $"\"enabled\" \"{enabled}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"" );
                        AppendCode( ref code, $"\"soundName\" \"{script.SoundName}\"" );
                        AppendCode( ref code, "\"classname\" \"ambient_generic\"" );
                        AppendCode( ref code, "}" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        // Remove 1 to the counter since we don't support this object for live map code
                        Helper.RemoveSendEntityCount();
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

                case BuildType.LiveMap:
                    // Empty
                    break;
            }

            await Helper.Wait();

            return code;
        }
    }
}