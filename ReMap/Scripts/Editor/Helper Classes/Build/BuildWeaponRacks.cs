using System.Text;
using System.Threading.Tasks;
using CodeViews;
using UnityEngine;
using static Build.Build;

namespace Build
{
    public class BuildWeaponRack
    {
        public static async Task< StringBuilder > BuildWeaponRackObjects( GameObject[] objectData, BuildType buildType )
        {
            var code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // Weapon Racks" );
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
                var script = ( WeaponRackScript )Helper.GetComponentByEnum( obj, ObjectType.WeaponRack );
                if ( script == null ) continue;

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code,
                            $"    MapEditor_CreateRespawnableWeaponRack( {Helper.BuildOrigin( obj ) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles( obj )}, \"{obj.name.Replace( "custom_weaponrack_", "mp_weapon_" )}\", {Helper.ReplaceComma( script.WeaponRespawnTime )} )" );
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
                        LiveMap.AddToGameQueue(
                            $"MapEditor_CreateRespawnableWeaponRack( {Helper.BuildOrigin( obj, false, true )}, {Helper.BuildAngles( obj )}, \"{obj.name.Replace( "custom_weaponrack_", "mp_weapon_" )}\", {Helper.ReplaceComma( script.WeaponRespawnTime )}, true )" );
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