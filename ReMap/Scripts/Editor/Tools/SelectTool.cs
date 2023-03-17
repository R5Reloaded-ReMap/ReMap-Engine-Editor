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
    [MenuItem("ReMap/Selection/Select All Prop Script", false, 100)]
    public static void SelectPropScriptInit()
    {
        SelectComponentScript<PropScript>();
    }

    [MenuItem("ReMap/Selection/Select All Draw Zipline", false, 100)]
    public static void SelectDrawZiplineInit()
    {
        SelectComponentScript<DrawZipline>();
    }

    [MenuItem("ReMap/Selection/Select All Draw Linked Zipline", false, 100)]
    public static void SelectDrawLinkedZiplineInit()
    {
        SelectComponentScript<DrawLinkedZipline>();
    }

    [MenuItem("ReMap/Selection/Select All Draw Vertical Zipline", false, 100)]
    public static void SelectDrawVerticalZiplineInit()
    {
        SelectComponentScript<DrawVerticalZipline>();
    }

    [MenuItem("ReMap/Selection/Select All Draw Non Vertical Zipline", false, 100)]
    public static void SelectDrawNonVerticalZiplineInit()
    {
        SelectComponentScript<DrawNonVerticalZipline>();
    }
    
    [MenuItem("ReMap/Selection/Select All Ziplines Types", false, 100)]
    public static void SelectAllZiplinesTypesInit()
    {
        SelectComponents( typeof( DrawZipline ), typeof( DrawLinkedZipline ), typeof( DrawVerticalZipline ), typeof( DrawNonVerticalZipline ) );
    }

    [MenuItem("ReMap/Selection/Select All Door Script", false, 100)]
    public static void SelectDoorScriptInit()
    {
        SelectComponentScript<DoorScript>();
    }

    [MenuItem("ReMap/Selection/Select All Loot Bin Script", false, 100)]
    public static void SelectLootBinScriptInit()
    {
        SelectComponentScript<LootBinScript>();
    }

    [MenuItem("ReMap/Selection/Select All Button Scripting", false, 100)]
    public static void SelectButtonScriptingInit()
    {
        SelectComponentScript<ButtonScripting>();
    }

    [MenuItem("ReMap/Selection/Select All Trigger Scripting", false, 100)]
    public static void SelectTriggerScriptingInit()
    {
        SelectComponentScript<TriggerScripting>();
    }

    [MenuItem("ReMap/Selection/Select All Weapon Rack Script", false, 100)]
    public static void SelectWeaponRackScriptInit()
    {
        SelectComponentScript<WeaponRackScript>();
    }

    [MenuItem("ReMap/Selection/Select All Bubble Script", false, 100)]
    public static void SelectBubbleScriptInit()
    {
        SelectComponentScript<BubbleScript>();
    }

    [MenuItem("ReMap/Selection/Select All Sound Script", false, 100)]
    public static void SelectSoundScriptInit()
    {
        SelectComponentScript<SoundScript>();
    }

    public static void SelectComponentScript<T>() where T : Component
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

    public static void SelectComponents(params Type[] componentTypes)
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
