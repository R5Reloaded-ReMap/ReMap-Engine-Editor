using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(AnimatedCameraScript))]
[CanEditMultipleObjects]
public class AnimatedCameraScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        GUI.skin = Resources.Load("CustomEditor/skin") as GUISkin;

        serializedObject.Update();

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/AnimatedCamera_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Camera Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AngleOffset"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxLeft"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxRight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RotationTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionTime"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
