using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(SoundScript))]
public class SoundScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        SoundScript myScript = target as SoundScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Sound_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Unity Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowPolylineSegments"));
        if (myScript.ShowPolylineSegments)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowPolylineSegmentsDistance"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(" Sound Parameters:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsWaveAmbient"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Enable"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("SoundName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PolylineSegment"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
