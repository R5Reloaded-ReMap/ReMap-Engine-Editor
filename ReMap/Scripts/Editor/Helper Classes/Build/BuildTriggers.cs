
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildTrigger
    {
        private enum LocalizationType
        {
            Origin,
            Angles,
            Offset
        }

        public static async Task< StringBuilder > BuildTriggerObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder(); int idx = 0;

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Triggers" );
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
            foreach ( GameObject obj in objectData )
            {
                TriggerScripting script = ( TriggerScripting ) Helper.GetComponentByEnum( obj, ObjectType.Trigger );
                if ( script == null ) continue;

                string TEnterCallback = script.EnterCallback;
                string TLeaveCallback = script.LeaveCallback;
                string scale = Helper.ReplaceComma( obj.transform.localScale.x );

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    entity trigger_{idx} = MapEditor_CreateTrigger( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {Helper.ReplaceComma( script.Width / 2 )}, {Helper.ReplaceComma( script.Height )}, {Helper.BoolToLower( script.Debug )} )" );

                        GameObject helper = script.Helper.gameObject;

                        if ( TEnterCallback != "" )
                        {
                            ChangeLocalization( ref TEnterCallback, helper, LocalizationType.Origin );
                            ChangeLocalization( ref TEnterCallback, helper, LocalizationType.Angles );
                            ChangeLocalization( ref TEnterCallback, helper, LocalizationType.Offset );
                            AppendCode( ref code, $"    trigger_{idx}.SetEnterCallback( void function(entity trigger , entity ent)" );
                            AppendCode( ref code,  "    {" );
                            AppendCode( ref code, $"    {TEnterCallback}" );
                            AppendCode( ref code,  "    })" );
                        }

                        if ( TLeaveCallback != "" )
                        {
                            ChangeLocalization( ref TLeaveCallback, helper, LocalizationType.Origin );
                            ChangeLocalization( ref TLeaveCallback, helper, LocalizationType.Angles );
                            ChangeLocalization( ref TLeaveCallback, helper, LocalizationType.Offset );
                            AppendCode( ref code, $"    trigger_{idx}.SetLeaveCallback( void function(entity trigger , entity ent)" );
                            AppendCode( ref code,  "    {" );
                            AppendCode( ref code, $"    {TLeaveCallback}" );
                            AppendCode( ref code,  "    })" );
                        }
                        AppendCode( ref code, $"    DispatchSpawn( trigger_{idx} )" );
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
                        Helper.RemoveEntityCount();
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

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }

        private static void ChangeLocalization( ref string callback, GameObject obj, LocalizationType type )
        {
            string searchTerm = "";
            string replacedString = "";

            switch ( type )
            {
                case LocalizationType.Origin:
                    searchTerm = "#HO";
                    replacedString = obj != null && obj.activeSelf ? Helper.BuildOrigin( obj ) : "< 0, 0, 0 >";
                    break;
                case LocalizationType.Angles:
                    searchTerm = "#HA";
                    replacedString = obj != null && obj.activeSelf ? Helper.BuildOrigin( obj ) : "< 0, 0, 0 >";
                    break;
                case LocalizationType.Offset:
                    searchTerm = "#HF";
                    replacedString = "+ startingorg";
                break;
            }

            int index = callback.IndexOf( searchTerm );

            while ( index >= 0 )
            {
                callback = callback.Substring(0, index) + replacedString + callback.Substring(index + searchTerm.Length);
                index = callback.IndexOf(searchTerm, index + replacedString.Length);
            }
        }
    }
}
