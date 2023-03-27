
using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildBubbleShield
    {
        public static string BuildBubbleShieldObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Bubble Shield";
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
                BubbleScript script = ( BubbleScript ) Helper.GetComponentByEnum( obj, ObjectType.BubbleShield );
                if ( script == null ) continue;

                string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );
                string scale = Helper.ReplaceComma( obj.transform.localScale.x );

                string ShieldColor = script.ShieldColor.r + " " + script.ShieldColor.g + " " + script.ShieldColor.b;

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_CreateBubbleShieldWithSettings( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {scale}, \"{ShieldColor}\", $\"{model}\" )";
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
