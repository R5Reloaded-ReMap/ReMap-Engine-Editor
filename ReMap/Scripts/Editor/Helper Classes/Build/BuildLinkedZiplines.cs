
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildLinkedZipline
    {
        public static string BuildLinkedZiplineObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Linked Ziplines";
                    PageBreak( ref code );
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
                LinkedZiplineScript script = ( LinkedZiplineScript ) Helper.GetComponentByEnum( obj, ObjectType.LinkedZipline );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        
                        string function = "";
                        string smoothType = script.SmoothType ? "GetAllPointsOnBezier" : "GetBezierOfPath";
                        string nodes = MakeLinkedZiplineNodeArray( obj );

                        if ( script.EnableSmoothing ) function = $"{smoothType}( {nodes}, {script.SmoothAmount} )";
                        else function = $"{nodes}";

                        code += $"    MapEditor_CreateLinkedZipline( {function} )";
                        PageBreak( ref code );
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
                    PageBreak( ref code );
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
