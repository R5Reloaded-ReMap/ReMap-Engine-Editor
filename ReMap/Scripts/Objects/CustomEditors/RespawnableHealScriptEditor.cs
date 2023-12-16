using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(RespawnableHealScript))]
public class RespawnableHealScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        RespawnableHealScript myScript = target as RespawnableHealScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/RespawnableHeal_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("RespawnTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("HealDuration"));
        if (myScript.IsSmallHeal)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("HealAmount"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Progressive"));

        serializedObject.ApplyModifiedProperties();
    }
}
