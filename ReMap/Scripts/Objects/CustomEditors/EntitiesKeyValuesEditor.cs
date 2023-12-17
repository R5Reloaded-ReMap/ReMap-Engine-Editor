using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(EntitiesKeyValues))]
public class EntitiesKeyValuesEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        EntitiesKeyValues myScript = target as EntitiesKeyValues;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/KeyValues_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("KeyValues"));

        serializedObject.ApplyModifiedProperties();
    }
}
