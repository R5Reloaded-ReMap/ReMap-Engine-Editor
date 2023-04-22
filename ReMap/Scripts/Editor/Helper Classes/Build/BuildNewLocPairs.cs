
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildNewLocPair
    {
        public static async Task< StringBuilder > BuildNewLocPairObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code.Append( "    // New Loc Pair" );
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
                NewLocPairScript script = ( NewLocPairScript ) Helper.GetComponentByEnum( obj, ObjectType.NewLocPair );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        code.Append( $"    NewLocPair( {Helper.BuildOrigin( obj )}, {Helper.BuildAngles( obj )} )" );
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
                        // Remove 1 to the counter since we don't support this object for live map code
                        CodeViewsWindow.CodeViewsWindow.SendedEntityCount -= 1;
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
    }
}
