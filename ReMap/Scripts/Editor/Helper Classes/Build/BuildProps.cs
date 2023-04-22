
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
            { PropScriptOptions.PlayerNoClimb, "NoClimbArray" },
            { PropScriptOptions.MakeInvisible, "InvisibleArray" },
            { PropScriptOptions.PlayerNoGrapple, "NoGrappleArray" },
            { PropScriptOptions.PlayerNoGrappleNoClimb, "NoGrappleNoClimbArray" },
            { PropScriptOptions.NoCollision, "NoCollisionArray" }
        };

        public static async Task< StringBuilder > BuildPropObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder(); PrecacheList = new List< String >();

            bool ClipArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.PlayerClip );
            bool NoClimbArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.PlayerNoClimb );
            bool InvisibleArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.MakeInvisible );
            bool NoGrappleArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.PlayerNoGrapple );
            bool NoGrappleNoClimbArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.PlayerNoGrappleNoClimb );
            bool NoCollisionArrayBool = ObjHavePropScriptOptions( objectData, PropScriptOptions.NoCollision );

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    if ( ClipArrayBool || NoClimbArrayBool || InvisibleArrayBool || NoGrappleArrayBool || NoGrappleNoClimbArrayBool || NoCollisionArrayBool )
                    {
                        code.Append( "    // Props Array" ); PageBreak( ref code );
                        code.Append( "    " );
                        if ( ClipArrayBool ) code.Append( $"array < entity > ClipArray; " );
                        if ( NoClimbArrayBool ) code.Append( $"array < entity > NoClimbArray; " );
                        if ( InvisibleArrayBool ) code.Append( $"array < entity > InvisibleArray; " );
                        if ( NoGrappleArrayBool ) code.Append( $"array < entity > NoGrappleArray; " );
                        if ( NoGrappleNoClimbArrayBool ) code.Append( $"array < entity > NoGrappleNoClimbArray; " );
                        if ( NoCollisionArrayBool ) code.Append( $"array < entity > NoCollisionArray; " );
                        PageBreak( ref code ); PageBreak( ref code );
                    }

                    code.Append( "    // Props" );
                    PageBreak( ref code );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;

                case BuildType.DataTable:
                    code.Append( "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"" );
                    PageBreak( ref code );
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
                        code.Append( $"    {addToArray}MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale} ){endFunction}" );
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code.Append(  "{\n" );
                        code.Append(  "\"StartDisabled\" \"0\"\n" );
                        code.Append(  "\"spawnflags\" \"0\"\n" );
                        code.Append( $"\"fadedist\" \"{Helper.ReplaceComma( script.FadeDistance )}\"\n" );
                        code.Append( $"\"collide_titan\" \"1\"\n" );
                        code.Append( $"\"collide_ai\" \"1\"\n" );
                        code.Append( $"\"scale\" \"{scale}\"\n" );
                        code.Append( $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"\n" );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( obj, true, true )}\"\n" );
                        code.Append(  "\"targetname\" \"ReMapEditorProp\"\n" );
                        code.Append(  "\"solid\" \"6\"\n" );
                        code.Append( $"\"model\" \"{model}\"\n" );
                        code.Append(  "\"ClientSide\" \"0\"\n" );
                        code.Append(  "\"classname\" \"prop_dynamic\"\n" );
                        code.Append(  "}\n" );
                        break;

                    case BuildType.Precache:
                        if ( PrecacheList.Contains( model ) )
                            continue;
                        PrecacheList.Add( model );
                        code.Append( $"    PrecacheModel( $\"{model}\" )" );
                        PageBreak( ref code );
                        break;

                    case BuildType.DataTable:
                        code.Append( $"\"prop_dynamic\",\"{Helper.BuildOrigin( obj, false, true )}\",\"{Helper.BuildAngles( obj )}\",{scale},{Helper.ReplaceComma( script.FadeDistance )},{Helper.BoolToLower( script.AllowMantle )},true,\"{model}\",\"{FindPathString( obj )}\"" );
                        PageBreak( ref code );
                        break;

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.SendCommandToApex($"script MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale}, true )");
                        Helper.DelayInMS();
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                
                    if ( ClipArrayBool || NoClimbArrayBool || InvisibleArrayBool || NoGrappleArrayBool || NoGrappleNoClimbArrayBool || NoCollisionArrayBool )
                    {
                        PageBreak( ref code );
                        if ( ClipArrayBool )
                        {
                            code.Append( "    foreach ( entity ent in ClipArray )\n" );
                            code.Append( "    {\n" );
                            code.Append( "        ent.MakeInvisible()\n" );
                            code.Append( "        ent.kv.solid = 6\n" );
                            code.Append( "        ent.kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER\n" );
                            code.Append( "        ent.kv.contents = CONTENTS_PLAYERCLIP\n" );
                            code.Append( "    }\n" );
                        }

                        if ( NoClimbArrayBool ) code.Append( "    foreach ( entity ent in NoClimbArray ) ent.kv.solid = 3\n" );

                        if ( InvisibleArrayBool ) code.Append( "    foreach ( entity ent in InvisibleArray ) ent.MakeInvisible()\n" );

                        if ( NoGrappleArrayBool ) code.Append( "    foreach ( entity ent in NoGrappleArray ) ent.kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE\n" );

                        if ( NoGrappleNoClimbArrayBool )
                        {
                            code.Append( "    foreach ( entity ent in NoGrappleNoClimbArray )\n" );
                            code.Append( "    {\n" );
                            code.Append( "        ent.MakeInvisible()\n" );
                            code.Append( "        ent.kv.solid = 3\n" );
                            code.Append( "        ent.kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER\n" );
                            code.Append( "        ent.kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE\n" );
                            code.Append( "    }\n" );
                        }

                        if ( NoCollisionArrayBool ) code.Append( "    foreach ( entity ent in NoCollisionArray ) ent.kv.solid = 0\n" );
                    }

                    PageBreak( ref code );
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;
                    
                case BuildType.DataTable:
                    code.Append( "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"" );
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
