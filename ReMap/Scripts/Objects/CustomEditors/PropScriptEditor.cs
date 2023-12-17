using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(PropScript))]
public class PropScriptEditor : Editor
{
    bool ShowOptions = false;

    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        PropScript myScript = target as PropScript;
        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/PropSettings_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowMantle"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeDistance"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RealmID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ClientSide"));

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Prop Options: (One Per Line)", CustomEditorStyle.style);
        ShowOptions = EditorGUILayout.Foldout(ShowOptions, "Example Options");
        if (ShowOptions)
        {
            EditorGUILayout.LabelField("    kv.solid = 0", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    kv.VisibilityFlags = ENTITY_VISIBLE_TO_FRIENDLY | ENTITY_VISIBLE_TO_OWNER", EditorStyles.boldLabel);
        }
        EditorGUILayout.Space(5);

        myScript.Options = EditorGUILayout.TextArea(myScript.Options, GUILayout.Height(200));

        serializedObject.ApplyModifiedProperties();
    }
}
