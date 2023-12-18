using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(DrawZipline))]
public class DrawZiplineEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawZipline myScript = target as DrawZipline;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Draw_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Zipline Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("zipline_start"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("zipline_end"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showZipline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showZiplineDistance"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
