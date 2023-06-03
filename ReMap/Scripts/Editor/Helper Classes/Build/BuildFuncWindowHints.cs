
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildFuncWindowHint
    {
        public static async Task< StringBuilder > BuildFuncWindowHintObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Func Window Hints" );
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
                WindowHintScript script = ( WindowHintScript ) Helper.GetComponentByEnum( obj, ObjectType.FuncWindowHint );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_CreateFuncWindowHint( {Helper.BuildOrigin( obj )}, {Helper.ReplaceComma( script.HalfHeight )}, {Helper.ReplaceComma( script.HalfWidth )}, {Helper.BuildRightVector( script.Right )} )" );
                        break;

                    case BuildType.EntFile:
                        AppendCode( ref code,  "{" );
                        AppendCode( ref code, $"\"halfheight\" \"{script.HalfHeight}\"" );
                        AppendCode( ref code, $"\"halfwidth\" \"{script.HalfWidth}\"" );
                        AppendCode( ref code, $"\"right\" \"{Helper.BuildRightVector( script.Right, true )}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( obj, true )}\"" );
                        AppendCode( ref code,  "\"classname\" \"func_window_hint\"" );
                        AppendCode( ref code,  "}" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        CodeViews.LiveMap.AddToGameQueue($"script MapEditor_CreateFuncWindowHint( {Helper.BuildOrigin( obj, false, true )}, {Helper.ReplaceComma( script.HalfHeight )}, {Helper.ReplaceComma( script.HalfWidth )}, {Helper.BuildRightVector( script.Right )}, true )");
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code );
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
