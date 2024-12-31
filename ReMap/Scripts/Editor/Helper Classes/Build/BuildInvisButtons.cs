using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildInvisButton
    {
        public static async Task< StringBuilder > BuildInvisButtonObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Invis Buttons" );
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
                var script = ( InvisButtonScript ) Helper.GetComponentByEnum( obj, ObjectType.InvisButton );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    Invis_Button( {Helper.BuildOrigin( script.Button.position ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( script.Button.eulerAngles )}, {Helper.BoolToLower( script.Up )}, {Helper.BuildOrigin( script.Destination.position ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( script.Destination.eulerAngles )}, \"{script.Message}\", \"{script.SubMessage}\", {script.Type}, {script.Duration}, \"{script.Token}\" )" );
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
                        LiveMap.AddToGameQueue( $"Invis_Button( {Helper.BuildOrigin( script.Button.position ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( script.Button.eulerAngles )}, {Helper.BoolToLower( script.Up )}, {Helper.BuildOrigin( script.Destination.position ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( script.Destination.eulerAngles )}, \"{script.Message}\", \"{script.SubMessage}\", {script.Type}, {script.Duration}, \"{script.Token}\" )" );
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