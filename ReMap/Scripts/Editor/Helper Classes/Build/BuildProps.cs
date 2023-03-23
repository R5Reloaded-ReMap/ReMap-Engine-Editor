
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildProp
    {
        public static string BuildPropObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    //Props\n";
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

                string model = UnityInfo.GetObjName( obj );

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {script.AllowMantle.ToString().ToLower()}, {script.FadeDistance}, {script.RealmID}, {obj.transform.localScale.x.ToString().Replace(",", ".")} )";
                        code += "\n";
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
                    code += "\n";
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
