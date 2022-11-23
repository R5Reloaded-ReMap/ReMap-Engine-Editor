using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetRealmIds : EditorWindow
{
    private GameObject source;
    private int realmID = 0;

    [MenuItem("R5Reloaded/Tools/RealmID Tool", false, 200)]
    static void Init()
    {
        SetRealmIds window = (SetRealmIds)EditorWindow.GetWindow(typeof(SetRealmIds), false, "RealmID Tool");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
        source = EditorGUILayout.ObjectField(source, typeof(Object), true) as GameObject;
        realmID = EditorGUILayout.IntField("RealmID:", realmID);
        GUILayout.EndVertical();

        if (GUILayout.Button("Set Realm IDS"))
            SetID();
    }

    void SetID()
    {
        if(source == null)
            return;

        foreach (Transform child in source.transform) {
            PropScript script = child.gameObject.GetComponent<PropScript>();
            if(script != null)
                script.realmID = realmID;
        }
    }
}
