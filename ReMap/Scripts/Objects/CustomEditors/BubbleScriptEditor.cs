using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(BubbleScript))]
public class BubbleScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        BubbleScript myScript = target as BubbleScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/BubbleShield_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShieldColor"));

        serializedObject.ApplyModifiedProperties();
    }
}
