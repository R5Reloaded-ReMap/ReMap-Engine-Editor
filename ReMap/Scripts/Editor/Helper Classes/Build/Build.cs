
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static CodeViewsWindow.CodeViewsWindow;

namespace Build
{
    public enum BuildType
    {
        Script,
        EntFile,
        Precache,
        DataTable,
        LiveMap
    }

    public class Build
    {
        public static bool IgnoreCounter = false;

        public static async Task< string > BuildObjectsWithEnum( ObjectType objectType, BuildType buildType, bool selection = false )
        {
            StringBuilder code = new StringBuilder();

            GameObject[] objectData;

            if ( selection ) objectData = Helper.GetSelectedObjectWithEnum( objectType );
            else objectData = Helper.GetObjArrayWithEnum( objectType );

            // Does not generate if the type of object are flaged hide
            if ( IsHided( objectType ) ) return "";
            
            // Dynamic Counter
            if ( !IgnoreCounter ) IncrementToCounter( objectType, buildType, objectData );

            switch ( objectType )
            {
                case ObjectType.BubbleShield:
                    code.Append( await BuildBubbleShield.BuildBubbleShieldObjects( objectData, buildType ) );
                    break;
                case ObjectType.Button:
                    code.Append( await BuildButton.BuildButtonObjects( objectData, buildType ) );
                    break;
                case ObjectType.DoubleDoor:
                    code.Append( await BuildDoubleDoor.BuildDoubleDoorObjects( objectData, buildType ) );
                    break;
                case ObjectType.FuncWindowHint:
                    code.Append( await BuildFuncWindowHint.BuildFuncWindowHintObjects( objectData, buildType ) );
                    break;
                case ObjectType.HorzDoor:
                    code.Append( await BuildHorzDoor.BuildHorzDoorObjects( objectData, buildType ) );
                    break;
                case ObjectType.Jumppad:
                    code.Append( await BuildJumppad.BuildJumppadObjects( objectData, buildType ) );
                    break;
                case ObjectType.LinkedZipline:
                    code.Append( await BuildLinkedZipline.BuildLinkedZiplineObjects( objectData, buildType ) );
                    break;
                case ObjectType.LootBin:
                    code.Append( await BuildLootBin.BuildLootBinObjects( objectData, buildType ) );
                    break;
                case ObjectType.NonVerticalZipLine:
                    code.Append( await BuildNonVerticalZipline.BuildNonVerticalZipLineObjects( objectData, buildType ) );
                    break;
                case ObjectType.Prop:
                    code.Append( await BuildProp.BuildPropObjects( objectData, buildType ) );
                    break;
                case ObjectType.SingleDoor:
                    code.Append( await BuildSingleDoor.BuildSingleDoorObjects( objectData, buildType ) );
                    break;
                case ObjectType.Sound:
                    code.Append( await BuildSound.BuildSoundObjects( objectData, buildType ) );
                    break;
                case ObjectType.NewLocPair:
                    code.Append( await BuildNewLocPair.BuildNewLocPairObjects( objectData, buildType ) );
                    break;
                case ObjectType.SpawnPoint:
                    code.Append( await BuildSpawnPoint.BuildSpawnPointObjects( objectData, buildType ) );
                    break;
                case ObjectType.TextInfoPanel:
                    code.Append( await BuildTextInfoPanel.BuildTextInfoPanelObjects( objectData, buildType ) );
                    break;
                case ObjectType.Trigger:
                    code.Append( await BuildTrigger.BuildTriggerObjects( objectData, buildType ) );
                    break;
                case ObjectType.VerticalDoor:
                    code.Append( await BuildVerticalDoor.BuildVerticalDoorObjects( objectData, buildType ) );
                    break;
                case ObjectType.VerticalZipLine:
                    code.Append( await BuildVerticalZipline.BuildVerticalZipLineObjects( objectData, buildType ) );
                    break;
                case ObjectType.WeaponRack:
                    code.Append( await BuildWeaponRack.BuildWeaponRackObjects( objectData, buildType ) );
                    break;
                case ObjectType.ZipLine:
                    code.Append( await BuildZipline.BuildZiplineObjects( objectData, buildType ) );
                break;
            }

            return code.ToString();
        }

        internal static void PageBreak( ref StringBuilder code )
        {
            code.Append( Environment.NewLine );
        }

        private static void IncrementToCounter( ObjectType objectType, BuildType buildType, GameObject[] objectData )
        {
            int objectDataLength = objectData.Length;

            switch ( objectType )
            {
                case ObjectType.ZipLine:
                case ObjectType.LinkedZipline:
                case ObjectType.VerticalZipLine:
                case ObjectType.NonVerticalZipLine:
                case ObjectType.DoubleDoor:

                    objectDataLength = objectDataLength * 2;

                    break;

                default: break;
            }

            switch ( buildType )
            {
                case BuildType.Precache:

                    List< String > PrecacheList = new List< String >();
                    foreach ( GameObject obj in objectData )
                    {
                        string model = UnityInfo.GetObjName( obj );
                        if ( PrecacheList.Contains( model ) )
                            continue;
                        PrecacheList.Add( model );
                    }
                    CodeViewsWindow.CodeViewsWindow.EntityCount += PrecacheList.Count;

                    break;
                case BuildType.LiveMap:
                        CodeViewsWindow.CodeViewsWindow.SendedEntityCount += objectDataLength;
                    break;

                default:
                    CodeViewsWindow.CodeViewsWindow.EntityCount += objectDataLength;
                break;
            }
        }
    }
}
