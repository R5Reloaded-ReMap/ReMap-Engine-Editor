using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(VerticalDoorScript))]
public class VerticalDoorScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        VerticalDoorScript myScript = target as VerticalDoorScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/VerticalDoor_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("AppearOpen"));

        serializedObject.ApplyModifiedProperties();
    }
}
