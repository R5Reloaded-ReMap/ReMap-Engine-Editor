
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildNewLocPair
    {
        public static string BuildNewLocPairObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // New Loc Pair";
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
                NewLocPairScript script = ( NewLocPairScript ) Helper.GetComponentByEnum( obj, ObjectType.NewLocPair );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    NewLocPair( {Helper.BuildOrigin( obj )}, {Helper.BuildAngles( obj )} )";
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
