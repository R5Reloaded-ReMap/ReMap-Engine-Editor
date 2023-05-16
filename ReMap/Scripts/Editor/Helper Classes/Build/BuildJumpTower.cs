
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildJumpTower
    {
        public static async Task< StringBuilder > BuildJumpTowerObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code.Append( "    // Jump Towers" );
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

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                JumpTowerScript script = ( JumpTowerScript ) Helper.GetComponentByEnum( obj, ObjectType.JumpTower );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        code.Append( $"    ReMapCreateJumpTower( {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {Helper.ReplaceComma( script.Height )} )" );
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

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.AddToGameQueue($"script ReMapCreateJumpTower({Helper.BuildOrigin(obj, false, true)},{Helper.BuildAngles(obj)},{Helper.ReplaceComma( script.Height )},true)");
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

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }
    }
}
