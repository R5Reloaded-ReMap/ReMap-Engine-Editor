using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(BubbleScript))]
[CanEditMultipleObjects]
public class BubbleScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        BubbleScript myScript = target as BubbleScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/BubbleShield_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Bubble Shield Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShieldColor"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
