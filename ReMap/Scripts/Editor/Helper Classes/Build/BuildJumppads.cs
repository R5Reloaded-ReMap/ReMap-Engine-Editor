using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildJumppad
    {
        public static async Task< StringBuilder > BuildJumppadObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // JumpPads" );
                    AppendCode( ref code, "    entity jumppad" );
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
                var script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Jumppad );
                if ( script == null ) continue;

                string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );
                string scale = Helper.ReplaceComma( obj.transform.localScale.x );

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code,
                            $"    jumppad = MapEditor_CreateJumpPad( MapEditor_CreateProp( $\"mdl/props/octane_jump_pad/octane_jump_pad.rmdl\", {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale} ) )" );

                        if ( script.Options.Length != 0 )
                        {
                            AppendCode( ref code, "    ", 0 );
                            string[] lines = script.Options.Split( '\n' );
                            for ( int i = 0; i < lines.Length; i++ )
                            {
                                string suffix = i < lines.Length - 1 ? "; " : "";
                                AppendCode( ref code, $"jumppad.{lines[ i ].Replace( "\n", "" )}{suffix}", 0 );
                            }
                            AppendCode( ref code );
                        }
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
                            $"MapEditor_CreateJumpPad( MapEditor_CreateProp( $\"mdl/props/octane_jump_pad/octane_jump_pad.rmdl\", {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles( obj )}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale} ), true )" );
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