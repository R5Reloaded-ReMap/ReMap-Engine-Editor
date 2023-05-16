
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;
using static ImportExport.Shared.SharedFunction;

namespace Build
{
    public class BuildProp
    {
        public static List< String > PrecacheList;

        private static Dictionary< PropScriptOptions, string > ArrayName = new Dictionary< PropScriptOptions, string >()
        {
            { PropScriptOptions.PlayerClip, "ClipArray" },
            { PropScriptOptions.NoClimb, "NoClimbArray" },
            { PropScriptOptions.MakeInvisible, "InvisibleArray" },
            { PropScriptOptions.NoGrapple, "NoGrappleArray" },
            { PropScriptOptions.PlayerClipInvisibleNoGrappleNoClimb, "ClipInvisibleNoGrappleNoClimbArray" },
            { PropScriptOptions.PlayerClipNoGrappleNoClimb, "ClipNoGrappleNoClimb" },
            { PropScriptOptions.NoCollision, "NoCollisionArray" }
        };

        public static async Task< StringBuilder > BuildPropObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder(); PrecacheList = new List< String >();

            bool ClipArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.PlayerClip );
            bool NoClimbArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.NoClimb );
            bool InvisibleArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.MakeInvisible );
            bool NoGrappleArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.NoGrapple );
            bool ClipInvisibleNoGrappleNoClimb = ObjHavePropScriptOptions( objectData, PropScriptOptions.PlayerClipInvisibleNoGrappleNoClimb );
            bool PlayerClipNoGrappleNoClimb = ObjHavePropScriptOptions( objectData, PropScriptOptions.PlayerClipNoGrappleNoClimb );
            bool NoCollisionArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.NoCollision );

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    if ( ClipArrayBool || NoClimbArrayBool || InvisibleArrayBool || NoGrappleArrayBool || ClipInvisibleNoGrappleNoClimb || PlayerClipNoGrappleNoClimb || NoCollisionArrayBool )
                    {
                        AppendCode( ref code, "    // Props Array" );
                        AppendCode( ref code, "    ", 0 );
                        if ( ClipArrayBool ) AppendCode( ref code, $"array < entity > ClipArray; ", 0 );
                        if ( NoClimbArrayBool ) AppendCode( ref code, $"array < entity > NoClimbArray; ", 0 );
                        if ( InvisibleArrayBool ) AppendCode( ref code, $"array < entity > InvisibleArray; ", 0 );
                        if ( NoGrappleArrayBool ) AppendCode( ref code, $"array < entity > NoGrappleArray; ", 0 );
                        if ( ClipInvisibleNoGrappleNoClimb ) AppendCode( ref code, $"array < entity > ClipInvisibleNoGrappleNoClimbArray; ", 0 );
                        if ( PlayerClipNoGrappleNoClimb ) AppendCode( ref code, $"array < entity > ClipNoGrappleNoClimb; ", 0 );
                        if ( NoCollisionArrayBool ) AppendCode( ref code, $"array < entity > NoCollisionArray; ", 0 );

                        AppendCode( ref code, "", 2 );
                    }

                    AppendCode( ref code, "    // Props" );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;

                case BuildType.DataTable:
                    AppendCode( ref code, "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"" );
                    break;

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                PropScript script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
                if ( script == null ) continue;

                string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );
                string scale = Helper.ReplaceComma( obj.transform.localScale.x );

                string addToArray = script.Option != PropScriptOptions.NoOption ? $"{ArrayName[script.Option]}.append( " : "";
                string endFunction = script.Option != PropScriptOptions.NoOption ? $" )" : "";

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    {addToArray}MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale} ){endFunction}" );
                        break;

                    case BuildType.EntFile:
                        AppendCode( ref code,  "{" );
                        AppendCode( ref code,  "\"StartDisabled\" \"0\"" );
                        AppendCode( ref code,  "\"spawnflags\" \"0\"" );
                        AppendCode( ref code, $"\"fadedist\" \"{Helper.ReplaceComma( script.FadeDistance )}\"" );
                        AppendCode( ref code, $"\"collide_titan\" \"1\"" );
                        AppendCode( ref code, $"\"collide_ai\" \"1\"" );
                        AppendCode( ref code, $"\"scale\" \"{scale}\"" );
                        AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"" );
                        AppendCode( ref code,  "\"targetname\" \"ReMapEditorProp\"" );
                        AppendCode( ref code,  "\"solid\" \"6\"" );
                        AppendCode( ref code, $"\"model\" \"{model}\"" );
                        AppendCode( ref code,  "\"ClientSide\" \"0\"" );
                        AppendCode( ref code,  "\"classname\" \"prop_dynamic\"" );
                        AppendCode( ref code,  "}" );
                        break;

                    case BuildType.Precache:
                        if ( PrecacheList.Contains( model ) )
                            continue;
                        PrecacheList.Add( model );
                        AppendCode( ref code, $"    PrecacheModel( $\"{model}\" )" );
                        break;

                    case BuildType.DataTable:
                        AppendCode( ref code, $"\"prop_dynamic\",\"{Helper.BuildOrigin( obj, false, true )}\",\"{Helper.BuildAngles( obj )}\",{scale},{Helper.ReplaceComma( script.FadeDistance )},{Helper.BoolToLower( script.AllowMantle )},true,\"{model}\",\"{FindPathString( obj )}\"" );
                        break;

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale}, true )" );
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                
                    if ( ClipArrayBool || NoClimbArrayBool || InvisibleArrayBool || NoGrappleArrayBool || ClipInvisibleNoGrappleNoClimb || PlayerClipNoGrappleNoClimb || NoCollisionArrayBool )
                    {
                        AppendCode( ref code );
                        
                        if ( ClipArrayBool )
                        {
                            AppendCode( ref code, "    foreach ( entity ent in ClipArray )" );
                            AppendCode( ref code, "    {" );
                            AppendCode( ref code, "        ent.MakeInvisible()" );
                            AppendCode( ref code, "        ent.kv.solid = 6" );
                            AppendCode( ref code, "        ent.kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER" );
                            AppendCode( ref code, "        ent.kv.contents = CONTENTS_PLAYERCLIP" );
                            AppendCode( ref code, "    }" );
                        }

                        if ( NoClimbArrayBool ) AppendCode( ref code, "    foreach ( entity ent in NoClimbArray ) ent.kv.solid = 3" );

                        if ( InvisibleArrayBool ) AppendCode( ref code, "    foreach ( entity ent in InvisibleArray ) ent.MakeInvisible()" );

                        if ( NoGrappleArrayBool ) AppendCode( ref code, "    foreach ( entity ent in NoGrappleArray ) ent.kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE" );

                        if ( ClipInvisibleNoGrappleNoClimb )
                        {
                            AppendCode( ref code, "    foreach ( entity ent in ClipInvisibleNoGrappleNoClimbArray )" );
                            AppendCode( ref code, "    {" );
                            AppendCode( ref code, "        ent.MakeInvisible()" );
                            AppendCode( ref code, "        ent.kv.solid = 3" );
                            AppendCode( ref code, "        ent.kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER" );
                            AppendCode( ref code, "        ent.kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE" );
                            AppendCode( ref code, "    }" );
                        }

                        if ( PlayerClipNoGrappleNoClimb )
                        {
                            AppendCode( ref code, "    foreach ( entity ent in ClipNoGrappleNoClimb )" );
                            AppendCode( ref code, "    {" );
                            AppendCode( ref code, "        ent.kv.solid = 3" );
                            AppendCode( ref code, "        ent.kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER" );
                            AppendCode( ref code, "        ent.kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE" );
                            AppendCode( ref code, "    }" );
                        }

                        if ( NoCollisionArrayBool ) AppendCode( ref code, "    foreach ( entity ent in NoCollisionArray ) ent.kv.solid = 0" );
                    }

                    AppendCode( ref code );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;
                    
                case BuildType.DataTable:
                    AppendCode( ref code, "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"" );
                    break;

                case BuildType.LiveMap:
                    // Empty
                break;
            }
            
            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }

        private static bool ObjHavePropScriptOptions( GameObject[] objectData, PropScriptOptions option )
        {
            foreach ( GameObject obj in objectData )
            {
                PropScript script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
                if ( script == null ) continue;

                if ( script.Option == option ) return true;
            }

            return false;
        }
    }
}
