using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildSingleDoor
    {
        public static async Task< StringBuilder > BuildSingleDoorObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Single Doors" );
                    AppendCode( ref code, "    array < entity > singleDoorEnts" );
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
                var script = ( DoorScript )Helper.GetComponentByEnum( obj, ObjectType.SingleDoor );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code,
                            $"    singleDoorEnts = MapEditor_SpawnDoor( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Single, {Helper.BoolToLower( script.GoldDoor )}, {Helper.BoolToLower( script.AppearOpen )} )" );
                            if ( !string.IsNullOrEmpty( script.Options ) )
                            {
                                AppendCode( ref code, "    foreach ( ent in singleDoorEnts )" );
                                AppendCode( ref code, "    {" );
                                string[] lines = script.Options.Split( '\n' );
                                for ( int i = 0; i < lines.Length; i++ )
                                {
                                    string line = lines[ i ].Replace( "\n", "" );
                                    if ( string.IsNullOrEmpty( line ) )
                                        continue;
                                    string suffix = i < lines.Length - 1 ? "; " : "";
                                    AppendCode( ref code, $"        ent.{line}{suffix}", 0 );
                                }
                                AppendCode( ref code );
                                AppendCode( ref code, "    }" );
                            }
                        break;

                    case BuildType.EntFile:
                        AppendCode( ref code, "{" );
                        AppendCode( ref code, "\"only_spawn_in_freelance\" \"0\"" );
                        AppendCode( ref code, "\"disableshadows\" \"0\"" );
                        AppendCode( ref code, "\"scale\" \"1\"" );
                        AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( obj, true )}\"" );
                        AppendCode( ref code, "\"model\" \"mdl/door/canyonlands_door_single_02.rmdl\"" );
                        AppendCode( ref code, "\"classname\" \"prop_door\"" );
                        AppendCode( ref code, "}" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        LiveMap.AddToGameQueue(
                            $"MapEditor_SpawnDoor( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles( obj )}, eMapEditorDoorType.Single, {Helper.BoolToLower( script.GoldDoor )}, {Helper.BoolToLower( script.AppearOpen )}, true )" );
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