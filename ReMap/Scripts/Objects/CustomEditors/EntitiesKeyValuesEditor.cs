using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(EntitiesKeyValues))]
[CanEditMultipleObjects]
public class EntitiesKeyValuesEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        EntitiesKeyValues myScript = target as EntitiesKeyValues;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/KeyValues_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Key Value Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("KeyValues"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
