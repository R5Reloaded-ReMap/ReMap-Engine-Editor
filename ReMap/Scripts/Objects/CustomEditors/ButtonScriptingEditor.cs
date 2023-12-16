using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(ButtonScripting))]
public class ButtonScriptingEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        ButtonScripting myScript = target as ButtonScripting;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Button_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseText"));
        EditorGUILayout.LabelField("On Use Callback:", EditorStyles.boldLabel);
        myScript.OnUseCallback = EditorGUILayout.TextArea(myScript.OnUseCallback, GUILayout.Height(200));

        serializedObject.ApplyModifiedProperties();
    }
}
