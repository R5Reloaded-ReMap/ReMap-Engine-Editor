using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(ButtonScripting))]
[CanEditMultipleObjects]
public class ButtonScriptingEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }


    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        ButtonScripting myScript = target as ButtonScripting;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Button_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseText"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(" On Use Callback:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUseCallback"), new GUIContent("", ""), GUILayout.Height(200));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
