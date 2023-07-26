
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildLinkedZipline
    {
        public static async Task< StringBuilder > BuildLinkedZiplineObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Linked Ziplines" );
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
                LinkedZiplineScript script = ( LinkedZiplineScript ) Helper.GetComponentByEnum( obj, ObjectType.LinkedZipline );
                if ( script == null ) continue;

                string function = "";
                string smoothType = script.SmoothType ? "GetAllPointsOnBezier" : "GetBezierOfPath";
                string nodes = MakeLinkedZiplineNodeArray( obj );

                if ( script.EnableSmoothing ) function = $"{smoothType}( {nodes}, {script.SmoothAmount} )";
                else function = $"{nodes}";

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_CreateLinkedZipline( {function} )" );
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
                        CodeViews.LiveMap.AddToGameQueue( $"MapEditor_CreateLinkedZipline( {function}, true )" );
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

            await Helper.Wait();

            return code;
        }

        private static string MakeLinkedZiplineNodeArray( GameObject obj )
        {
            bool first = true;

            string nodes = "[ ";
            foreach ( Transform child in obj.transform )
            {
                if (!first)
                    nodes += ", ";

                nodes += Helper.BuildOrigin( child.gameObject );

                    first = false;
            }
            nodes += " ]";

            return nodes;
        }
    }
}
