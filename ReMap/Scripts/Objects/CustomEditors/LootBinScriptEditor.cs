using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[CustomEditor(typeof(LootBinScript))]
public class LootBinScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        LootBinScript myScript = target as LootBinScript;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/Lootbin_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("LootbinSkin"));

        serializedObject.ApplyModifiedProperties();
    }
}
