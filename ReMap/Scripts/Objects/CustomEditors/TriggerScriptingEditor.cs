using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(TriggerScripting))]
public class TriggerScriptingEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        TriggerScripting myScript = target as TriggerScripting;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Trigger_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField("Trigger Visual Look:", CustomEditorStyle.style);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useWireMesh"));
        if (myScript.useWireMesh)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wireMeshSides"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("color"));

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Settings:", CustomEditorStyle.style);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Debug"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Height"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Width"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseHelperForTP"));

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Enter Callback:", CustomEditorStyle.style);
        myScript.EnterCallback = EditorGUILayout.TextArea(myScript.EnterCallback, GUILayout.Height(200));

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Leave Callback:", CustomEditorStyle.style);
        myScript.LeaveCallback = EditorGUILayout.TextArea(myScript.LeaveCallback, GUILayout.Height(200));

        serializedObject.ApplyModifiedProperties();
    }
}
