
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildRespawnableHeal
    {
        public static async Task< StringBuilder > BuildRespawnableHealObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Respawnable Heals" );
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
                RespawnableHealScript script = ( RespawnableHealScript ) Helper.GetComponentByEnum( obj, ObjectType.RespawnableHeal );
                if ( script == null ) continue;

                string healType = DetermineHealType( obj );

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    ReMapCreateRespawnableHeal( {Helper.BuildOrigin( obj )}, {healType}, {Helper.ReplaceComma( script.RespawnTime )}, {Helper.ReplaceComma( script.HealDuration )}, {Helper.ReplaceComma( script.HealAmount )}, {Helper.BoolToLower( script.Progressive )} )" );
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
                        CodeViews.LiveMap.AddToGameQueue( $"ReMapCreateRespawnableHeal( {Helper.BuildOrigin( obj, false, true )}, {healType}, {Helper.ReplaceComma( script.RespawnTime )}, {Helper.ReplaceComma( script.HealDuration )}, {Helper.ReplaceComma( script.HealAmount )}, {Helper.BoolToLower( script.Progressive )}, true )" );
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

        private static string DetermineHealType( GameObject obj )
        {
            switch ( UnityInfo.GetObjName( obj.name ) )
            {
                case "custom_respawnable_heal_medkit":  return "eReMapHealType.Heal";
                case "custom_respawnable_heal_seringe": return "eReMapHealType.HealSmall";
                case "custom_respawnable_heal_battery": return "eReMapHealType.Shield";
                case "custom_respawnable_heal_cell":    return "eReMapHealType.ShieldSmall";
                case "custom_respawnable_heal_phoenix": return "eReMapHealType.Both";
            }

            return "eReMapHealType.Both";
        }
    }
}
