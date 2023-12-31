using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(RespawnableHealScript))]
[CanEditMultipleObjects]
public class RespawnableHealScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        RespawnableHealScript myScript = target as RespawnableHealScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/RespawnableHeal_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Respawnable Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RespawnTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("HealDuration"));
        if (myScript.IsSmallHeal)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("HealAmount"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Progressive"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
