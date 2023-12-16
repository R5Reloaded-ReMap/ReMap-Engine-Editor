using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(WindowHintScript))]
public class WindowHintScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        WindowHintScript myScript = target as WindowHintScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/WindowHint_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("HalfHeight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("HalfWidth"));

        serializedObject.ApplyModifiedProperties();
    }
}
