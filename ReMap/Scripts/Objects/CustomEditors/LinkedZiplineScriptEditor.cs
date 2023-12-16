using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(LinkedZiplineScript))]
public class LinkedZiplineScriptEditor : Editor
{
    int _selected = 0;
    string[] _options = new string[2] { "GetAllPointsOnBezier", "GetBezierOfPath" };
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        LinkedZiplineScript myScript = target as LinkedZiplineScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/LinkedZipline_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField("Unity Settings.", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowZipline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowSmoothedZipline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowZiplineDistance"));

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Zipline Settings.", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("EnableSmoothing"));

        if (myScript.EnableSmoothing)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SmoothAmount"));
            this._selected = EditorGUILayout.Popup("Smooth Type", _selected, _options);

            if (EditorGUI.EndChangeCheck())
            {
                myScript.SmoothType = this._selected == 0 ? true : false;
            }
        }

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Warning: Smoothing will slightly change location of nodes.", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Smoothing previews will be shown in blue, GetBezierOfPath is a rough estimate of the path", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Try not to go to crazy on the smooth amount.", EditorStyles.boldLabel);

        serializedObject.ApplyModifiedProperties();
    }
}
