using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(AnimatedCameraScript))]
public class AnimatedCameraScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        AnimatedCameraScript myScript = target as AnimatedCameraScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/AnimatedCamera_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField("Camera Options:", CustomEditorStyle.style);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AngleOffset"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxLeft"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxRight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RotationTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionTime"));

        serializedObject.ApplyModifiedProperties();
    }
}
