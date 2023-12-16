using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(PathScript))]
public class PathScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        PathScript myScript = target as PathScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Path_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField("Unity Settings:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowPath"));
        if (myScript.ShowPath)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowPathDistance"));

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Path Parameters:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("SpeedTransition"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Fov"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TrackTarget"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("EnableSpacing"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Spacing"));

        serializedObject.ApplyModifiedProperties();
    }
}
