using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(DeportedDoorScript))]
public class DeportedDoorScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        DeportedDoorScript myScript = target as DeportedDoorScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/DeportedDoor_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("GoldDoor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AppearOpen"));

        serializedObject.ApplyModifiedProperties();
    }
}
