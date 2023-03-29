
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public static StringBuilder BuildObjectsWithEnum( ObjectType objectType, BuildType buildType, bool selection = false )
        {
            StringBuilder code = new StringBuilder();

            GameObject[] objectData;

            if ( selection ) objectData = Helper.GetSelectedObjectWithEnum( objectType );
            else objectData = Helper.GetObjArrayWithEnum( objectType );

            int objectDataLength = objectData.Length;

            // Does not generate if the type of object are flaged hide
            if ( IsHided( objectType ) ) return new StringBuilder( "" );
            
            // Dynamic Counter
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
                    code.Append( BuildBubbleShield.BuildBubbleShieldObjects( objectData, buildType ) );
                    break;
                case ObjectType.Button:
                    code.Append( BuildButton.BuildButtonObjects( objectData, buildType ) );
                    break;
                case ObjectType.DoubleDoor:
                    code.Append( BuildDoubleDoor.BuildDoubleDoorObjects( objectData, buildType ) );
                    break;
                case ObjectType.FuncWindowHint:
                    code.Append( BuildFuncWindowHint.BuildFuncWindowHintObjects( objectData, buildType ) );
                    break;
                case ObjectType.HorzDoor:
                    code.Append( BuildHorzDoor.BuildHorzDoorObjects( objectData, buildType ) );
                    break;
                case ObjectType.Jumppad:
                    code.Append( BuildJumppad.BuildJumppadObjects( objectData, buildType ) );
                    break;
                case ObjectType.LinkedZipline:
                    code.Append( BuildLinkedZipline.BuildLinkedZiplineObjects( objectData, buildType ) );
                    break;
                case ObjectType.LootBin:
                    code.Append( BuildLootBin.BuildLootBinObjects( objectData, buildType ) );
                    break;
                case ObjectType.NonVerticalZipLine:
                    code.Append( BuildNonVerticalZipline.BuildNonVerticalZipLineObjects( objectData, buildType ) );
                    break;
                case ObjectType.Prop:
                    code.Append( BuildProp.BuildPropObjects( objectData, buildType ) );
                    break;
                case ObjectType.SingleDoor:
                    code.Append( BuildSingleDoor.BuildSingleDoorObjects( objectData, buildType ) );
                    break;
                case ObjectType.Sound:
                    code.Append( BuildSound.BuildSoundObjects( objectData, buildType ) );
                    break;
                case ObjectType.NewLocPair:
                    code.Append( BuildNewLocPair.BuildNewLocPairObjects( objectData, buildType ) );
                    break;
                case ObjectType.SpawnPoint:
                    code.Append( BuildSpawnPoint.BuildSpawnPointObjects( objectData, buildType ) );
                    break;
                case ObjectType.TextInfoPanel:
                    code.Append( BuildTextInfoPanel.BuildTextInfoPanelObjects( objectData, buildType ) );
                    break;
                case ObjectType.Trigger:
                    code.Append( BuildTrigger.BuildTriggerObjects( objectData, buildType ) );
                    break;
                case ObjectType.VerticalDoor:
                    code.Append( BuildVerticalDoor.BuildVerticalDoorObjects( objectData, buildType ) );
                    break;
                case ObjectType.VerticalZipLine:
                    code.Append( BuildVerticalZipline.BuildVerticalZipLineObjects( objectData, buildType ) );
                    break;
                case ObjectType.WeaponRack:
                    code.Append( BuildWeaponRack.BuildWeaponRackObjects( objectData, buildType ) );
                    break;
                case ObjectType.ZipLine:
                    code.Append( BuildZipline.BuildZiplineObjects( objectData, buildType ) );
                break;
            }

            return code;
        }

        internal static void PageBreak( ref StringBuilder code )
        {
            code.Append( Environment.NewLine );
        }
    }
}
