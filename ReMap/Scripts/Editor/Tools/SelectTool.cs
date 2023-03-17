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
        SelectPropScript();
    }


    public static void SelectPropScript()
    {
        GameObject[] array = new GameObject[0];

        foreach ( GameObject go in UnityInfo.GetAllGameObjectInScene() )
        {
            PropScript script = go.GetComponent<PropScript>();

            if ( script != null )
            {
                int currentLength = array.Length;
                Array.Resize(ref array, currentLength + 1);
                array[currentLength] = go;
            }
        }

        Selection.objects = array;
    }
}
