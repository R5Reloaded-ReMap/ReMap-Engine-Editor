using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildSpeedBoost
    {
        public static async Task< StringBuilder > BuildSpeedBoostObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Speed Boosts" );
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
            foreach ( var obj in objectData )
            {
                var script = ( SpeedBoostScript )Helper.GetComponentByEnum( obj, ObjectType.SpeedBoost );
                if ( script == null ) continue;

                string BoostColor = $"< {script.Color.r}, {script.Color.g}, {script.Color.b} >";

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code,
                            $"    ReMapCreateSpeedBoost( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {BoostColor}, {Helper.ReplaceComma( script.RespawnTime )}, {Helper.ReplaceComma( script.Strengh )}, {Helper.ReplaceComma( script.Duration )}, {Helper.ReplaceComma( script.FadeTime )} )" );
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
                        LiveMap.AddToGameQueue(
                            $"ReMapCreateSpeedBoost( {Helper.BuildOrigin( obj, false, true )}, {BoostColor}, {Helper.ReplaceComma( script.RespawnTime )}, {Helper.ReplaceComma( script.Strengh )}, {Helper.ReplaceComma( script.Duration )}, {Helper.ReplaceComma( script.FadeTime )}, true )" );
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
    }
}