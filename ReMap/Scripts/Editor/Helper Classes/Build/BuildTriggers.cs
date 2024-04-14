using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildTrigger
    {
        public static async Task< StringBuilder > BuildTriggerObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();
            int idx = 0;

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Triggers" );
                    AppendCode( ref code, "    entity trigger" );
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
                var script = ( TriggerScripting )Helper.GetComponentByEnum( obj, ObjectType.Trigger );
                if ( script == null ) continue;

                string TEnterCallback = script.EnterCallback;
                string TLeaveCallback = script.LeaveCallback;
                string scale = Helper.ReplaceComma( obj.transform.localScale.x );

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code,
                            $"    trigger = MapEditor_CreateTrigger( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {Helper.ReplaceComma( script.Width / 2 )}, {Helper.ReplaceComma( script.Height )}, {Helper.BoolToLower( script.Debug )} )" );

                        var helper = script.Helper.gameObject;

                        if ( TEnterCallback != "" )
                        {
                            ChangeLocalization( ref TEnterCallback, helper );
                            AppendCode( ref code, "    trigger.SetEnterCallback( void function( entity trigger, entity ent )" );
                            AppendCode( ref code, "    {" );
                            AppendCode( ref code, $"    {TEnterCallback}" );
                            AppendCode( ref code, "    })" );
                        }

                        if ( TLeaveCallback != "" )
                        {
                            ChangeLocalization( ref TLeaveCallback, helper );
                            AppendCode( ref code, "    trigger.SetLeaveCallback( void function( entity trigger, entity ent )" );
                            AppendCode( ref code, "    {" );
                            AppendCode( ref code, $"    {TLeaveCallback}" );
                            AppendCode( ref code, "    })" );
                        }
                        AppendCode( ref code, "    DispatchSpawn( trigger )" );
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

        private static void ChangeLocalization( ref string callback, GameObject obj )
        {
            foreach ( string key in Helper.LocalizedStringTrigger.Keys )
                if ( Helper.LocalizedStringTrigger.TryGetValue( key, out var mapping ) )
                {
                    string searchTerm = mapping.SearchTerm;
                    string replacedString = mapping.ReplacementFunc( obj );

                    int index = callback.IndexOf( searchTerm );

                    while (index >= 0)
                    {
                        callback = callback.Substring( 0, index ) + replacedString + callback.Substring( index + searchTerm.Length );
                        index = callback.IndexOf( searchTerm, index + replacedString.Length );
                    }
                }
        }
    }
}