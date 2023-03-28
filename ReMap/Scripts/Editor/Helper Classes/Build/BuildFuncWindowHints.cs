
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildFuncWindowHint
    {
        public static string BuildFuncWindowHintObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Func Window Hints";
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
                WindowHintScript script = ( WindowHintScript ) Helper.GetComponentByEnum( obj, ObjectType.FuncWindowHint );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_CreateFuncWindowHint( {Helper.BuildOrigin( obj )}, {Helper.ReplaceComma( script.HalfHeight )}, {Helper.ReplaceComma( script.HalfWidth )}, {Helper.BuildRightVector( script.Right )} )";
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code +=  "{\n";
                        code += $"\"halfheight\" \"{script.HalfHeight}\"\n";
                        code += $"\"halfwidth\" \"{script.HalfWidth}\"\n";
                        code += $"\"right\" \"{Helper.BuildRightVector( script.Right, true )}\"\n";
                        code += $"\"origin\" \"{Helper.BuildOrigin( obj, true )}\"\n";
                        code +=  "\"classname\" \"func_window_hint\"\n";
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
