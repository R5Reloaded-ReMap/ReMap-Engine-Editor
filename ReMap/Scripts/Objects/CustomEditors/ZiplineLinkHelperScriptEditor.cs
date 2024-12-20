using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(ZiplineLinkHelperScript))]
[CanEditMultipleObjects]
public class ZiplineLinkHelperScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/ZiplineLink_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Unity Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("zipline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("origin"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("angles"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
