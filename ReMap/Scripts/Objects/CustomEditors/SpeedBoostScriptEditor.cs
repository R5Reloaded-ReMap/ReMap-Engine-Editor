using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(SpeedBoostScript))]
public class SpeedBoostScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        SpeedBoostScript myScript = target as SpeedBoostScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/SpeedBoost_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Speedboost Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Color"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RespawnTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Strengh"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeTime"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
