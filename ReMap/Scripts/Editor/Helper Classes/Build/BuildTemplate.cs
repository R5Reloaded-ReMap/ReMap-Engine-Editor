
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
                    break;
                case BuildType.EntFile:
                    break;
                case BuildType.Precache:
                    break;
                case BuildType.DataTable:
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
                        break;
                    case BuildType.EntFile:
                        break;
                    case BuildType.Precache:
                        break;
                    case BuildType.DataTable:
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    break;
                case BuildType.EntFile:
                    break;
                case BuildType.Precache:
                    break;
                case BuildType.DataTable:
                break;
            }

            return code;
        }
    }
}
