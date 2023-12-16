using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(HorzDoorScript))]
public class HorzDoorScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        HorzDoorScript myScript = target as HorzDoorScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/HorizontalDoor_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("AppearOpen"));

        serializedObject.ApplyModifiedProperties();
    }
}
