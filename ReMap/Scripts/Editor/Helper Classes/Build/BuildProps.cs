
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;
using static ImportExport.Shared.SharedFunction;

namespace Build
{
    public class BuildProp
    {
        public static List< String > PrecacheList = new List< String >();

        public static string BuildPropObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Props";
                    PageBreak( ref code );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;

                case BuildType.DataTable:
                    code += "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"";
                    PageBreak( ref code );
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                PropScript script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
                if ( script == null ) continue;

                string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );
                string scale = obj.transform.localScale.x.ToString().Replace(",", ".");

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {script.AllowMantle.ToString().ToLower()}, {script.FadeDistance}, {script.RealmID}, {scale} )";
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code +=  "{\n";
                        code +=  "\"StartDisabled\" \"0\"\n";
                        code +=  "\"spawnflags\" \"0\"\n";
                        code += $"\"fadedist\" \"{script.FadeDistance}\"\n";
                        code += $"\"collide_titan\" \"1\"\n";
                        code += $"\"collide_ai\" \"1\"\n";
                        code += $"\"scale\" \"{scale}\"\n";
                        code += $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"\n";
                        code += $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"\n";
                        code +=  "\"targetname\" \"ReMapEditorProp\"\n";
                        code +=  "\"solid\" \"6\"\n";
                        code += $"\"model\" \"{model}\"\n";
                        code +=  "\"ClientSide\" \"0\"\n";
                        code +=  "\"classname\" \"prop_dynamic\"\n";
                        code +=  "}\n";
                        break;

                    case BuildType.Precache:
                        if ( PrecacheList.Contains( model ) )
                            continue;
                        PrecacheList.Add( model );
                        code += $"    PrecacheModel( $\"{model}\" )";
                        PageBreak( ref code );
                        break;

                    case BuildType.DataTable:
                        code += $"\"prop_dynamic\",\"{Helper.BuildOrigin( obj, false, true )}\",\"{Helper.BuildAngles( obj )}\",{scale},{script.FadeDistance},{script.AllowMantle.ToString().ToLower()},true,\"{model}\",\"{FindPathString( obj )}\"";
                        PageBreak( ref code );
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
                    code += "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"";
                break;
            }

            return code;
        }
    }
}
