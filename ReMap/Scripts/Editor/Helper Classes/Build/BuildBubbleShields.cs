using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildBubbleShield
    {
        public static async Task< StringBuilder > BuildBubbleShieldObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Bubbles Shield" );
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
                var script = ( BubbleScript )Helper.GetComponentByEnum( obj, ObjectType.BubbleShield );
                if ( script == null ) continue;

                string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );
                string scale = Helper.ReplaceComma( obj.transform.localScale.x );

                string ShieldColor = script.ShieldColor.r + " " + script.ShieldColor.g + " " + script.ShieldColor.b;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_CreateBubbleShieldWithSettings( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {scale}, \"{ShieldColor}\", $\"{model}\" )" );
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
                        LiveMap.AddToGameQueue( $"MapEditor_CreateBubbleShieldWithSettings( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles( obj )}, {scale}, \"{ShieldColor}\", $\"{model}\", true )" );
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