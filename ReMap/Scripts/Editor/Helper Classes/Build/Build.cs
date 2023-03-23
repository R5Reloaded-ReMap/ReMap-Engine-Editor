
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Build
{
    public enum BuildType
    {
        Map,
        Ent,
        Precache,
        DataTable
    }

    public class Build
    {
        public static string BuildObjectsWithEnum( ObjectType objectType, BuildType buildType, bool selection = false )
        {
            string code = "";

            GameObject[] objectData;

            if ( selection ) objectData = Helper.GetSelectedObjectWithEnum( objectType );
            else objectData = Helper.GetObjArrayWithEnum( objectType );

            switch ( objectType )
            {
                case ObjectType.BubbleShield:
                    //code +=  BuildBubbleShieldObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.Button:
                    //code +=  BuildButtonObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.DoubleDoor:
                    //code +=  BuildDoubleDoorObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.FuncWindowHint:
                    //code +=  BuildFuncWindowHintObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.HorzDoor:
                    //code +=  BuildHorzDoorObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.Jumppad:
                    //code +=  BuildJumppadObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.LinkedZipline:
                    //code +=  BuildLinkedZiplineObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.LootBin:
                    //code +=  BuildLootBinObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.NonVerticalZipLine:
                    //code +=  BuildNonVerticalZipLineObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.Prop:
                    //code +=  BuildPropObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.SingleDoor:
                    //code +=  BuildSingleDoorObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.Sound:
                    //code +=  BuildSoundObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.SpawnPoint:
                    //code +=  BuildSpawnPointObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.TextInfoPanel:
                    //code +=  BuildTextInfoPanelObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.Trigger:
                    //code +=  BuildTriggerObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.VerticalDoor:
                    //code +=  BuildVerticalDoorObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.VerticalZipLine:
                    //code +=  BuildVerticalZipLineObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.WeaponRack:
                    //code +=  BuildWeaponRackObjectsWithEnum( objectData, buildType );
                    break;
                case ObjectType.ZipLine:
                    //code +=  BuildZipLineObjectsWithEnum( objectData, buildType );
                break;
            }

            return code;
        }
    }
}
