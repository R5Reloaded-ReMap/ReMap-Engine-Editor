
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

using static Build.Build;
using static ImportExport.SharedFunction;
using static LibrarySorter.RpakManagerWindow;

namespace Build
{
    public class BuildProp
    {
        public static List< String > PrecacheList;

        public static async Task< StringBuilder > BuildPropObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder(); PrecacheList = new List< String >();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Props" );
                    AppendCode( ref code, "    entity prop" );
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
                if ( script == null || script.ClientSide ) continue;

                string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );
                string scale = Helper.ReplaceComma( obj.transform.localScale.x );

                if ( !libraryData.IsR5ReloadedModels( model ) )
                {
                    CodeViews.CodeViewsWindow.NotExitingModel++;
                    continue;
                }

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    prop = MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale} )" );

                        if ( script.Options.Length != 0 )
                        {
                            AppendCode(ref code, "    ", 0);
                            string[] lines = script.Options.Split('\n');
                            for (int i = 0; i < lines.Length; i++)
                            {
                                string suffix = i < lines.Length - 1 ? "; " : "";
                                AppendCode(ref code, $"prop.{lines[i].Replace("\n", "")}{suffix}", 0);
                            }
                            AppendCode(ref code, "");
                        }
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
                        CodeViews.LiveMap.AddToGameQueue( $"MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles(obj)}, {Helper.BoolToLower( script.AllowMantle )}, {Helper.ReplaceComma( script.FadeDistance )}, {script.RealmID}, {scale}, true )" );
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
                    AppendCode( ref code, "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"" );
                    break;

                case BuildType.LiveMap:
                    // Empty
                break;
            }
            
            await Helper.Wait();

            return code;
        }

        public static async Task< StringBuilder > BuildClientPropObjects( bool selection )
        {
            GameObject[] objectData = Helper.GetAllObjectTypeWithEnum( ObjectType.Prop, selection );

            StringBuilder code = new StringBuilder();

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                PropScript script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
                if ( script == null || !script.ClientSide ) continue;

                string model = UnityInfo.GetApexModelName( UnityInfo.GetObjName( obj ), true );

                if ( !libraryData.IsR5ReloadedModels( model ) )
                {
                    CodeViews.CodeViewsWindow.NotExitingModel++;
                    continue;
                }

                AppendCode( ref code, $"    CreateClientSidePropDynamic( {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, $\"{model}\" )" );
            }

            CodeViews.CodeViewsWindow.EntityCount = objectData.Where( o => Helper.GetComponentByEnum( o, ObjectType.Prop ) != null ).Select( o => Helper.GetComponentByEnum( o, ObjectType.Prop ) ).Where( s => ( ( PropScript )s ).ClientSide ).Count();

            await Helper.Wait();

            return code;
        }

        private static string BuildPropertiesArray( List< string > args )
        {
            string array = "[ ";
            for( int i = 0 ; i < args.Count ; i++ )
            {
                array += args[i];

                if ( i != args.Count - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }
    }
}
