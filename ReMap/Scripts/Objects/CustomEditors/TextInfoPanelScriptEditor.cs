using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(TextInfoPanelScript))]
public class TextInfoPanelScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        TextInfoPanelScript myScript = target as TextInfoPanelScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/TextPanel_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Title"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showPIN"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Scale"));

        serializedObject.ApplyModifiedProperties();
    }
}
