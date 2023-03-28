
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static string BuildTriggerObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = ""; int idx = 0;

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // Triggers";
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
                        code += $"    entity trigger_{idx} = MapEditor_CreateTrigger( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, {Helper.ReplaceComma( script.Width )}, {Helper.ReplaceComma( script.Height )}, {Helper.BoolToLower( script.Debug )} )";
                        PageBreak( ref code );

                        GameObject helper = script.Helper.gameObject;

                        if ( TEnterCallback != "" )
                        {
                            ChangeLocalization( ref TEnterCallback, helper, LocalizationType.Origin );
                            ChangeLocalization( ref TEnterCallback, helper, LocalizationType.Angles );
                            ChangeLocalization( ref TEnterCallback, helper, LocalizationType.Offset );
                            code += $"    trigger_{idx}.SetEnterCallback( void function(entity trigger , entity ent)\n";
                            code +=  "    {\n";
                            code += $"    {TEnterCallback}\n";
                            code +=  "    }\n";
                        }

                        if ( TLeaveCallback != "" )
                        {
                            ChangeLocalization( ref TLeaveCallback, helper, LocalizationType.Origin );
                            ChangeLocalization( ref TLeaveCallback, helper, LocalizationType.Angles );
                            ChangeLocalization( ref TLeaveCallback, helper, LocalizationType.Offset );
                            code += $"    trigger_{idx}.SetLeaveCallback( void function(entity trigger , entity ent)\n";
                            code +=  "    {\n";
                            code += $"    {TLeaveCallback}\n";
                            code +=  "    }\n";
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
            }

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
