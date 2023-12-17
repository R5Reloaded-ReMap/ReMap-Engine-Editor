using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(Trigger))]
public class TriggerEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        Trigger myScript = target as Trigger;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Trigger_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("numSides"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("color"));

        serializedObject.ApplyModifiedProperties();
    }
}
