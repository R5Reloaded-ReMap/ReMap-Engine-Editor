using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(JumpTowerScript))]
public class JumpTowerScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        JumpTowerScript myScript = target as JumpTowerScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/JumpTower_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField("Unity Settings:", CustomEditorStyle.style);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowZipline"));
        if (myScript.ShowZipline)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowZiplineDistance"));

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Jump Tower Parameters:", CustomEditorStyle.style);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Height"));

        serializedObject.ApplyModifiedProperties();
    }
}
