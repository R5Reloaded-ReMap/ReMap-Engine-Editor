using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(SpeedBoostScript))]
[CanEditMultipleObjects]
public class SpeedBoostScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();

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
