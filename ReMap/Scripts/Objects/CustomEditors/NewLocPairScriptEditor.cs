using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(NewLocPairScript))]
public class NewLocPairScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        NewLocPairScript myScript = target as NewLocPairScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/LocPair_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" No Settings", CustomEditorStyle.LabelStyle);

        serializedObject.ApplyModifiedProperties();
    }
}
