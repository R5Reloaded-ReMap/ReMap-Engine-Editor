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

    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
        LinkedZiplineScript myScript = target as LinkedZiplineScript;
        this._selected = myScript.SmoothType ? 0 : 1;
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        LinkedZiplineScript myScript = target as LinkedZiplineScript;

        if (myScript.SmoothAmount < 2)
            myScript.SmoothAmount = 2;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/LinkedZipline_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" Unity Settings.", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowZipline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowSmoothedZipline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowZiplineDistance"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(" Zipline Settings.", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
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
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Warning: Smoothing will slightly change location of nodes.", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Smoothing previews will be shown in blue, GetBezierOfPath is a rough estimate of the path", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Try not to go to crazy on the smooth amount.", EditorStyles.boldLabel);

        serializedObject.ApplyModifiedProperties();
    }
}
