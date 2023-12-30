using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(DrawNonVerticalZipline))]
[CanEditMultipleObjects]
public class DrawNonVerticalZiplineEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawNonVerticalZipline myScript = target as DrawNonVerticalZipline;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/NonVerticalZipline_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Unity Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowZipline"));
        if (myScript.ShowZipline)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowZiplineDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowAutoDetachDistance"));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(" Zipline Parameters:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        if (myScript.ShowArmOffsetStart)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ArmOffsetStart"));
        if (myScript.ShowArmOffsetEnd)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ArmOffsetEnd"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeDistance"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Scale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Width"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("SpeedScale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("LengthScale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PreserveVelocity"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DropToBottom"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoDetachStart"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoDetachEnd"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RestPoint"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PushOffInDirectionX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsMoving"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DetachEndOnSpawn"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DetachEndOnUse"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(" Panel Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Panels"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PanelTimerMin"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PanelTimerMax"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PanelMaxUse"));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
