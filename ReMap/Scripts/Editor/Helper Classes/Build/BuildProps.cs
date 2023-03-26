
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
        public static List< String > PrecacheList;

        private static Dictionary< PropScriptOptions, string > ArrayName = new Dictionary< PropScriptOptions, string >()
        {
            { PropScriptOptions.PlayerClip, "PlayerClipArray" },
            { PropScriptOptions.PlayerNoClimb, "NoClimbArray" },
            { PropScriptOptions.MakeInvisible, "InvisibleArray" },
            { PropScriptOptions.KvContentsNOGRAPPLE, "NoGrappleArray" }
        };

        public static string BuildPropObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = ""; PrecacheList = new List< String >();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Props Array";
                    PageBreak( ref code );
                    code += "    ";
                    foreach ( string arrayType in ArrayName.Values )
                    {
                        code += $"array < entity > {arrayType}; ";
                    }
                    PageBreak( ref code ); PageBreak( ref code );
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
                string scale = Helper.ReplaceComma( obj.transform.localScale.x );

                string addToArray = script.Option != PropScriptOptions.NoOption ? $"{ArrayName[script.Option]}.append( " : "";
                string endFunction = script.Option != PropScriptOptions.NoOption ? $" )" : "";

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    {addToArray}MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale} ){endFunction}";
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code +=  "{\n";
                        code +=  "\"StartDisabled\" \"0\"\n";
                        code +=  "\"spawnflags\" \"0\"\n";
                        code += $"\"fadedist\" \"{Helper.ReplaceComma( script.FadeDistance )}\"\n";
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
                        code += $"\"prop_dynamic\",\"{Helper.BuildOrigin( obj, false, true )}\",\"{Helper.BuildAngles( obj )}\",{scale},{Helper.ReplaceComma( script.FadeDistance )},{Helper.BoolToLower( script.AllowMantle )},true,\"{model}\",\"{FindPathString( obj )}\"";
                        PageBreak( ref code );
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    PageBreak( ref code );

                    code += "    foreach ( entity ent in PlayerClipArray )\n";
                    code += "    {\n";
                    code += "        ent.MakeInvisible()\n";
                    code += "        ent.kv.solid = 6\n";
                    code += "        ent.kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER\n";
                    code += "        ent.kv.contents = CONTENTS_PLAYERCLIP\n";
                    code += "    }\n";

                    code += "    foreach ( entity ent in NoClimbArray ) ent.kv.solid = 3\n";

                    code += "    foreach ( entity ent in InvisibleArray ) ent.MakeInvisible()\n";

                    code += "    foreach ( entity ent in NoGrappleArray ) ent.kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE\n";

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
