using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(ZiplineLinkHelperScript))]
public class ZiplineLinkHelperScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        ZiplineLinkHelperScript myScript = target as ZiplineLinkHelperScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/ZiplineLink_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField("Unity Settings:", CustomEditorStyle.style);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("zipline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("origin"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("angles"));

        serializedObject.ApplyModifiedProperties();
    }
}
