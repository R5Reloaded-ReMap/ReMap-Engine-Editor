
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
                    AppendCode( ref code, $"    // Camera Paths" );
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
                                AppendCode( ref code, $"    // Path_{idxStr}" );
                                AppendCode( ref code, $"    if ( !IsValid( GetLocalClientPlayer() ) ) return" );
                                AppendCode( ref code, $"    float fov_{idxStr} = {fov}" );
                                AppendCode( ref code, $"    float speed_{idxStr} = {speed}" );
                                AppendCode( ref code, $"    vector origin_{idxStr} = {Helper.BuildOrigin( node.gameObject ) + Helper.ShouldAddStartingOrg()}" );
                                AppendCode( ref code, $"    vector angles_{idxStr} = {Helper.BuildAngles( node.gameObject )}" );
                                AppendCode( ref code );
                                AppendCode( ref code, $"    entity camera_{idxStr} = CreateClientSidePointCamera( origin_{idxStr}, angles_{idxStr}, fov_{idxStr} )" );
                                AppendCode( ref code, $"    entity scriptMover_{idxStr} = CreateClientsideScriptMover( $\"mdl/dev/empty_model.rmdl\", origin_{idxStr}, angles_{idxStr} )" );
                                AppendCode( ref code, $"    camera_{idxStr}.SetParent( scriptMover_{idxStr} )" );
                                AppendCode( ref code, $"    if ( IsValid( GetLocalClientPlayer() ) ) GetLocalClientPlayer().SetMenuCameraEntity( camera_{idxStr} )" );
                                AppendCode( ref code );

                                isFirst = false;
                            }
                            else
                            {
                                AppendCode( ref code, $"    scriptMover_{idxStr}.NonPhysicsMoveTo( {Helper.BuildOrigin( node.gameObject ) + Helper.ShouldAddStartingOrg()}, speed_{idxStr}, 0, 0)" );
                                AppendCode( ref code, $"    scriptMover_{idxStr}.NonPhysicsRotateTo( {Helper.BuildAngles( node.gameObject )}, speed_{idxStr}, 0, 0)" );
                                AppendCode( ref code, $"    wait speed_{idxStr}" );
                                AppendCode( ref code );
                            }
                        }

                        AppendCode( ref code, $"    if ( IsValid( GetLocalClientPlayer() ) ) GetLocalClientPlayer().ClearMenuCameraEntity()" );
                        AppendCode( ref code, $"    if ( IsValid( scriptMover_{idxStr} ) ) scriptMover_{idxStr}.Destroy()" );
                        AppendCode( ref code, $"    if ( IsValid( camera_{idxStr} ) ) camera_{idxStr}.Destroy()" );
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
                        // Remove 1 to the counter since we don't support this object for live map code
                        Helper.RemoveSendedEntityCount();
                    break;
                }

                idx++;
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
