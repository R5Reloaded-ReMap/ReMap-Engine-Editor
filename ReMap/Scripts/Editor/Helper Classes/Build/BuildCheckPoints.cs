using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildCheckPoint
    {
        public static async Task< StringBuilder > BuildCheckPointObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();
            int currentCp = 0;

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Check Points" );
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
                var script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
                if ( script == null ) continue;

                //foreach ( Transform child in obj.transform )
                //{
                //    string next = "";
                //    string back = "";
                //    if ( child.name == "next" )
                //        next =
                //            $"Invis_Button({Helper.BuildOrigin( obj.gameObject, false, true )}, {Helper.BuildAngles( obj.gameObject, false, true )}, true, {Helper.BuildOrigin( child.gameObject, false, true )}, {Helper.BuildAngles( child.gameObject, false, true )})";
                //    if ( child.name == "back" )
                //        back =
                //            $"Invis_Button({Helper.BuildOrigin( obj.gameObject, false, true )}, {Helper.BuildAngles( obj.gameObject, false, true )}, false, {Helper.BuildOrigin( child.gameObject, false, true )}, {Helper.BuildAngles( child.gameObject, false, true )})";
                //}

                //++currentCp;

                switch ( buildType )
                {
                    case BuildType.Script:
                        //AppendCode( ref code, $"    //{currentCp}" );
                        //AppendCode( ref code, $"    {next}" );
                        //AppendCode( ref code, $"    {back}" );
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
                        // Remove 1 to the counter since we don't support this object for live map code
                        Helper.RemoveSendEntityCount();
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