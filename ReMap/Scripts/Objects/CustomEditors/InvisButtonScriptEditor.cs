using System;
using UnityEditor;
using UnityEngine;

[ Serializable ]
[ CustomEditor( typeof( InvisButtonScript ) ) ]
[ CanEditMultipleObjects ]
public class InvisButtonScriptEditor : Editor
{
    private void OnEnable()
    {
        CustomEditorStyle.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var myTexture = Resources.Load< Texture2D >( "CustomEditor/PropSettings_CustomEditor" );
        GUILayout.Label( myTexture );

        EditorGUILayout.LabelField( " Invis Button Settings:", CustomEditorStyle.LabelStyle );
        EditorGUILayout.Space( 5 );

        EditorGUILayout.BeginVertical( CustomEditorStyle.BoxStyle );

        EditorGUILayout.PropertyField( serializedObject.FindProperty( "Up" ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( "Message" ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( "SubMessage" ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( "Type" ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( "Duration" ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( "Token" ) );
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}