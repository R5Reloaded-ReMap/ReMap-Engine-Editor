using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(DrawZipline))]
public class DrawZiplineEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawZipline myScript = target as DrawZipline;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Draw_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("zipline_start"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("zipline_end"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showZipline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showZiplineDistance"));

        serializedObject.ApplyModifiedProperties();
    }
}
