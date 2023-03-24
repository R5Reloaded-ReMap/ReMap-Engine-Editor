
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildVerticalZipline
    {
        public static string BuildVerticalZipLineObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    // VerticalZipLines";
                    PageBreak( ref code );
                    break;
                case BuildType.EntFile:
                    break;
                case BuildType.Precache:
                    break;
                case BuildType.DataTable:
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                DrawVerticalZipline script = ( DrawVerticalZipline ) Helper.GetComponentByEnum( obj, ObjectType.VerticalZipLine );
                if ( script == null ) continue;

                int PreserveVelocity = script.PreserveVelocity  ? 1 : 0;
                int DropToBottom = script.DropToBottom ? 1 : 0;
                string RestPoint = script.RestPoint.ToString().ToLower();
                int PushOffInDirectionX = script.PushOffInDirectionX ? 1 : 0;
                string IsMoving = script.IsMoving.ToString().ToLower();
                int DetachEndOnSpawn = script.DetachEndOnSpawn ? 1 : 0;
                int DetachEndOnUse = script.DetachEndOnUse ? 1 : 0;
                float PanelTimerMin = script.PanelTimerMin;
                float PanelTimerMax = script.PanelTimerMax;
                int PanelMaxUse = script.PanelMaxUse;

                string PanelOrigin = BuildPanelOriginArray( script.Panels );
                string PanelAngles = BuildPanelAnglesArray( script.Panels );
                string PanelModels = BuildPanelModelsArray( script.Panels );

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += PanelModels;
                        break;
                    case BuildType.EntFile:
                        break;
                    case BuildType.Precache:
                        break;
                    case BuildType.DataTable:
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    PageBreak( ref code );
                    break;
                case BuildType.EntFile:
                    break;
                case BuildType.Precache:
                    break;
                case BuildType.DataTable:
                break;
            }

            return code;
        }

        private static string BuildPanelOriginArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $" {Helper.BuildOrigin( objArray[i] )}{Helper.ShouldAddStartingOrg()}";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }

        private static string BuildPanelAnglesArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $"{ Helper.BuildAngles( objArray[i] )}";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }

        private static string BuildPanelModelsArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $"$\"{UnityInfo.GetApexModelName( "mdl/" + objArray[i].name, true )}\"";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }
    }
}
