
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
                    code.Append( "    // Func Window Hints" );
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
                        code.Append( $"    MapEditor_CreateFuncWindowHint( {Helper.BuildOrigin( obj )}, {Helper.ReplaceComma( script.HalfHeight )}, {Helper.ReplaceComma( script.HalfWidth )}, {Helper.BuildRightVector( script.Right )} )" );
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code.Append(  "{\n" );
                        code.Append( $"\"halfheight\" \"{script.HalfHeight}\"\n" );
                        code.Append( $"\"halfwidth\" \"{script.HalfWidth}\"\n" );
                        code.Append( $"\"right\" \"{Helper.BuildRightVector( script.Right, true )}\"\n" );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( obj, true )}\"\n" );
                        code.Append(  "\"classname\" \"func_window_hint\"\n" );
                        code.Append(  "}\n" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                    break;

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.SendCommandToApex($"script MapEditor_CreateFuncWindowHint( {Helper.BuildOrigin( obj, false, true )}, {Helper.ReplaceComma( script.HalfHeight )}, {Helper.ReplaceComma( script.HalfWidth )}, {Helper.BuildRightVector( script.Right )}, true )");
                        Helper.DelayInMS(CodeViewsWindow.LiveMap.BuildWaitMS);
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

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }
    }
}
