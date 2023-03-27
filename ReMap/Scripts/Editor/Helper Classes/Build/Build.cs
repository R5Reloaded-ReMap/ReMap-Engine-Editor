
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static CodeViewsWindow.CodeViewsWindow;

namespace Build
{
    public enum BuildType
    {
        Script,
        EntFile,
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

            int objectDataLength = objectData.Length;

            if ( objectDataLength == 0 || IsHided( objectType ) ) return "";
            
            if ( objectType == ObjectType.ZipLine || objectType == ObjectType.LinkedZipline || objectType == ObjectType.VerticalZipLine || objectType == ObjectType.NonVerticalZipLine || objectType == ObjectType.DoubleDoor )
            {
                CodeViewsWindow.CodeViewsWindow.EntityCount += objectDataLength * 2;
            }
            else if ( buildType == BuildType.Precache )
            {
                List< String > PrecacheList = new List< String >();
                foreach ( GameObject obj in objectData )
                {
                    string model = UnityInfo.GetObjName( obj );
                    if ( PrecacheList.Contains( model ) )
                        continue;
                    PrecacheList.Add( model );
                }
                CodeViewsWindow.CodeViewsWindow.EntityCount += PrecacheList.Count;
            }
            else CodeViewsWindow.CodeViewsWindow.EntityCount += objectDataLength;
            

            switch ( objectType )
            {
                case ObjectType.BubbleShield:
                    code += BuildBubbleShield.BuildBubbleShieldObjects( objectData, buildType );
                    break;
                case ObjectType.Button:
                    code += BuildButton.BuildButtonObjects( objectData, buildType );
                    break;
                case ObjectType.DoubleDoor:
                    code += BuildDoubleDoor.BuildDoubleDoorObjects( objectData, buildType );
                    break;
                case ObjectType.FuncWindowHint:
                    code += BuildFuncWindowHint.BuildFuncWindowHintObjects( objectData, buildType );
                    break;
                case ObjectType.HorzDoor:
                    code += BuildHorzDoor.BuildHorzDoorObjects( objectData, buildType );
                    break;
                case ObjectType.Jumppad:
                    code += BuildJumppad.BuildJumppadObjects( objectData, buildType );
                    break;
                case ObjectType.LinkedZipline:
                    code += BuildLinkedZipline.BuildLinkedZiplineObjects( objectData, buildType );
                    break;
                case ObjectType.LootBin:
                    code += BuildLootBin.BuildLootBinObjects( objectData, buildType );
                    break;
                case ObjectType.NonVerticalZipLine:
                    code += BuildNonVerticalZipline.BuildNonVerticalZipLineObjects( objectData, buildType );
                    break;
                case ObjectType.Prop:
                    code += BuildProp.BuildPropObjects( objectData, buildType );
                    break;
                case ObjectType.SingleDoor:
                    code += BuildSingleDoor.BuildSingleDoorObjects( objectData, buildType );
                    break;
                case ObjectType.Sound:
                    code += BuildSound.BuildSoundObjects( objectData, buildType );
                    break;
                case ObjectType.SpawnPoint:
                    //code += BuildSpawnPoint.BuildSpawnPointObjects( objectData, buildType );
                    break;
                case ObjectType.TextInfoPanel:
                    code += BuildTextInfoPanel.BuildTextInfoPanelObjects( objectData, buildType );
                    break;
                case ObjectType.Trigger:
                    //code += BuildTrigger.BuildTriggerObjects( objectData, buildType );
                    break;
                case ObjectType.VerticalDoor:
                    code += BuildVerticalDoor.BuildVerticalDoorObjects( objectData, buildType );
                    break;
                case ObjectType.VerticalZipLine:
                    code += BuildVerticalZipline.BuildVerticalZipLineObjects( objectData, buildType );
                    break;
                case ObjectType.WeaponRack:
                    code += BuildWeaponRack.BuildWeaponRackObjects( objectData, buildType );
                    break;
                case ObjectType.ZipLine:
                    code += BuildZipline.BuildZiplineObjects( objectData, buildType );
                break;
            }

            return code;
        }

        internal static void PageBreak( ref string code )
        {
            code += Environment.NewLine;
        }
    }
}
