using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class RealmParent : MonoBehaviour
{
    public int realmId = -1;
    void Awake()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        OnHierarchyChanged();
    }

    void OnHierarchyChanged()
    {
        foreach (PropScript a in gameObject.GetComponentsInChildren<PropScript>())
            a.realmID = realmId;
    }

    private void OnValidate()
    {
        OnHierarchyChanged();
    }
}