using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SelectTool
{
    [ MenuItem( "ReMap/Selection/Select All Prop", false, 100 ) ]
    public static void SelectPropInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.Prop );
    }

    [ MenuItem( "ReMap/Selection/Select All Zipline ( simple )", false, 100 ) ]
    public static void SelectZiplineInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.ZipLine );
    }

    [ MenuItem( "ReMap/Selection/Select All Linked Zipline", false, 100 ) ]
    public static void SelectLinkedZiplineInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.LinkedZipline );
    }

    [ MenuItem( "ReMap/Selection/Select All Vertical Zipline", false, 100 ) ]
    public static void SelectVerticalZiplineInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.VerticalZipLine );
    }

    [ MenuItem( "ReMap/Selection/Select All Non Vertical Zipline", false, 100 ) ]
    public static void SelectNonVerticalZiplineInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.NonVerticalZipLine );
    }
    
    [ MenuItem( "ReMap/Selection/Select All Ziplines Types", false, 100 ) ]
    public static void SelectAllZiplinesTypesInit()
    {
        GameObject[][] array = new GameObject[][]
        {
            Helper.GetAllObjectTypeWithEnum( ObjectType.ZipLine ),
            Helper.GetAllObjectTypeWithEnum( ObjectType.LinkedZipline ),
            Helper.GetAllObjectTypeWithEnum( ObjectType.VerticalZipLine ),
            Helper.GetAllObjectTypeWithEnum( ObjectType.NonVerticalZipLine )
        };

        Selection.objects = Helper.AppendMultipleObjectType( array );
    }

    [ MenuItem( "ReMap/Selection/Select All Single Doors", false, 100 ) ]
    public static void SelectSingleDoorsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.SingleDoor );
    }

        [ MenuItem( "ReMap/Selection/Select All Double Doors", false, 100 ) ]
    public static void SelectDoubleDoorsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.DoubleDoor );
    }

        [ MenuItem( "ReMap/Selection/Select All Vertical Doors", false, 100 ) ]
    public static void SelectVerticalDoorsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.VerticalDoor );
    }

        [ MenuItem( "ReMap/Selection/Select All Horizontal Doors", false, 100 ) ]
    public static void SelectHorizontalDoorsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.HorzDoor );
    }

    [ MenuItem( "ReMap/Selection/Select All Doors Types", false, 100 ) ]
    public static void SelectAllDoorsTypesInit()
    {
        GameObject[][] array = new GameObject[][]
        {
            Helper.GetAllObjectTypeWithEnum( ObjectType.SingleDoor ),
            Helper.GetAllObjectTypeWithEnum( ObjectType.DoubleDoor ),
            Helper.GetAllObjectTypeWithEnum( ObjectType.VerticalDoor ),
            Helper.GetAllObjectTypeWithEnum( ObjectType.HorzDoor )
        };

        Selection.objects = Helper.AppendMultipleObjectType( array );
    }

    [ MenuItem( "ReMap/Selection/Select All Loot Bins", false, 100 ) ]
    public static void SelectLootBinsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.LootBin );
    }

    [ MenuItem( "ReMap/Selection/Select All Buttons", false, 100 ) ]
    public static void SelectButtonsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.Button );
    }

    [ MenuItem( "ReMap/Selection/Select All Trigger Scripting", false, 100 ) ]
    public static void SelectTriggersInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.Trigger );
    }

    [ MenuItem( "ReMap/Selection/Select All Weapon Racks", false, 100 ) ]
    public static void SelectWeaponRacksInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.WeaponRack );
    }

    [ MenuItem( "ReMap/Selection/Select All Bubble Shields", false, 100 ) ]
    public static void SelectBubbleShieldsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.BubbleShield );
    }

    [ MenuItem( "ReMap/Selection/Select All Sounds", false, 100 ) ]
    public static void SelectSoundsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.Sound );
    }

    [ MenuItem( "ReMap/Selection/Select All New Loc Pairs", false, 100 ) ]
    public static void SelectNewLocPairsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.NewLocPair );
    }

    [ MenuItem( "ReMap/Selection/Select All Info Spawn Points Human", false, 100 ) ]
    public static void SelectSpawnPointsHumanInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.SpawnPoint );
    }

    [ MenuItem( "ReMap/Selection/Select All Text Info Panels", false, 100 ) ]
    public static void SelectTextInfoPanelsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.TextInfoPanel );
    }

    [ MenuItem( "ReMap/Selection/Select All Window Hints", false, 100 ) ]
    public static void SelectWindowHintsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.FuncWindowHint );
    }

    [ MenuItem( "ReMap/Selection/Select All Camera Paths", false, 100 ) ]
    public static void SelectCameraPathsInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.CameraPath );
    }

    [ MenuItem( "ReMap/Selection/Select All Unity Only Player Spawn", false, 100 ) ]
    public static void SelectUOPlayerSpawnInit()
    {
        Selection.objects = Helper.GetAllObjectTypeWithEnum( ObjectType.LiveMapCodePlayerSpawn );
    }

    private static void SelectComponentScript< T >() where T : Component
    {
        GameObject[] array = new GameObject[0];

        foreach ( GameObject go in UnityInfo.GetAllGameObjectInScene() )
        {
            T script = go.GetComponent<T>();

            if ( script != null )
            {
                int currentLength = array.Length;
                Array.Resize(ref array, currentLength + 1);
                array[currentLength] = go;
            }
        }

        Selection.objects = array;
    }

    private static void SelectComponents( params Type[] componentTypes )
    {
        GameObject[] array = new GameObject[0];

        foreach ( GameObject go in UnityInfo.GetAllGameObjectInScene() )
        {
            foreach (Type type in componentTypes)
            {
                Component component = go.GetComponent(type);
                if (component != null)
                {
                    int currentLength = array.Length;
                    Array.Resize(ref array, currentLength + 1);
                    array[currentLength] = go;
                    break;
                }
            }
        }

        Selection.objects = array;
    }
}
