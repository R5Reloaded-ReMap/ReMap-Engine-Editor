using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(NewLocPairScript))]
[CanEditMultipleObjects]
public class NewLocPairScriptEditor : Editor
{
    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/LocPair_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField(" No Settings", CustomEditorStyle.LabelStyle);

        serializedObject.ApplyModifiedProperties();
    }
}
