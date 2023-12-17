using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomEditorStyle : Editor
{
    public static GUIStyle style;
    public static void OnEnable()
    {
        style = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            fontSize = 15,
            normal = new GUIStyleState()
            {
                textColor = Color.white
            }
        };
    }
}
