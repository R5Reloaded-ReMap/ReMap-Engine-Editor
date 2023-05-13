
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildCameraPath
    {
        public static async Task< StringBuilder > BuildCameraPathObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code.Append( $"    // Camera Paths" );
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

            int idx = 0;

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                PathScript script = ( PathScript ) Helper.GetComponentByEnum( obj, ObjectType.CameraPath );
                if ( script == null ) continue;

                string idxStr = idx.ToString( "00" ); bool isFirst = true;

                string fov = Helper.ReplaceComma( script.Fov );
                string speed = Helper.ReplaceComma( script.SpeedTransition );

                switch ( buildType )
                {
                    case BuildType.Script:
                        foreach ( Transform node in obj.transform )
                        {
                            if ( node.gameObject.name == "targetRef" ) continue;

                            if ( isFirst )
                            {
                                code.Append( $"    // Path_{idxStr}\n" );
                                code.Append( $"    if ( IsValid( GetLocalClientPlayer() ) ) return\n" );
                                code.Append( $"    float fov_{idxStr} = {fov}\n" );
                                code.Append( $"    float speed_{idxStr} = {speed}\n" );
                                code.Append( $"    vector origin_{idxStr} = {Helper.BuildOrigin( node.gameObject ) + Helper.ShouldAddStartingOrg()}\n" );
                                code.Append( $"    vector angles_{idxStr} = {Helper.BuildAngles( node.gameObject )}\n" );
                                code.Append( $"\n" );
                                code.Append( $"    entity camera_{idxStr} = CreateClientSidePointCamera( origin_{idxStr}, angles_{idxStr}, fov_{idxStr} )\n" );
                                code.Append( $"    entity scriptMover_{idxStr} = CreateClientsideScriptMover( $\"mdl/dev/empty_model.rmdl\", origin_{idxStr}, angles_{idxStr} )\n" );
                                code.Append( $"    camera_{idxStr}.SetParent( scriptMover_{idxStr} )\n" );
                                code.Append( $"    if ( IsValid( GetLocalClientPlayer() ) ) GetLocalClientPlayer().SetMenuCameraEntity( camera_{idxStr} )\n" );
                                code.Append( $"\n" );

                                isFirst = false;
                            }
                            else
                            {
                                code.Append( $"    scriptMover_{idxStr}.NonPhysicsMoveTo( {Helper.BuildOrigin( node.gameObject ) + Helper.ShouldAddStartingOrg()}, speed_{idxStr}, 0, 0)\n" );
                                code.Append( $"    scriptMover_{idxStr}.NonPhysicsRotateTo( {Helper.BuildAngles( node.gameObject )}, speed_{idxStr}, 0, 0)\n" );
                                code.Append( $"    wait speed_{idxStr}\n" );
                                code.Append( $"\n" );
                            }
                        }

                        code.Append( $"    if ( IsValid( GetLocalClientPlayer() ) ) GetLocalClientPlayer().ClearMenuCameraEntity()\n" );
                        code.Append( $"    if ( IsValid( scriptMover_{idxStr} ) ) scriptMover_{idxStr}.Destroy()\n" );
                        code.Append( $"    if ( IsValid( camera_{idxStr} ) ) camera_{idxStr}.Destroy()\n" );
                        code.Append( $"\n" );

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

                idx++;
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
