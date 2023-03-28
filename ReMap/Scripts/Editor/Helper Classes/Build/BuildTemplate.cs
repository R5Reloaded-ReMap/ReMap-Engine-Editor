
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildTemplate
    {
        public static string BuildObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Template";
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
                PropScript script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
                if ( script == null ) continue;

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
