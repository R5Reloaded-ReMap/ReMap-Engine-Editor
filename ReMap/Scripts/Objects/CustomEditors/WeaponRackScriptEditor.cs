using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(WeaponRackScript))]
public class WeaponRackScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        WeaponRackScript myScript = target as WeaponRackScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/WeaponRack_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.LabelField("Weapon respawn time in seconds", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RespawnTime"));

        serializedObject.ApplyModifiedProperties();
    }
}
