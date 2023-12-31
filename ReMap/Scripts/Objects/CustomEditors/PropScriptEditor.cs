using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PropScript))]
[CanEditMultipleObjects]
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

        EditorGUILayout.LabelField(" Prop Settings:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowMantle"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeDistance"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RealmID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ClientSide"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(" Prop Options: (One Per Line)", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(CustomEditorStyle.BoxStyle);
        ShowOptions = EditorGUILayout.Foldout(ShowOptions, "Example Options");
        if (ShowOptions)
        {
            EditorGUILayout.LabelField("    kv.solid = 0", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    kv.CollisionGroup = TRACE_COLLISION_GROUP_PLAYER", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    kv.contents = CONTENTS_SOLID | CONTENTS_NOGRAPPLE", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    kv.VisibilityFlags = ENTITY_VISIBLE_TO_FRIENDLY | ENTITY_VISIBLE_TO_OWNER", EditorStyles.boldLabel);
        }
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Options"), new GUIContent("", ""), GUILayout.Height(200));
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
