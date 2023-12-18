
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ReTexture : EditorWindow
{
    private static UnityEngine.Object obj;

    bool expanded = false;
    private static Object[] SelectedMats = new Object[0];
    int num = 0;

    public static void Init()
    {
        ReTexture window = (ReTexture)EditorWindow.GetWindow(typeof(ReTexture), false, "ReMap Debug Console");
        window.Show();
    }

    void OnGUI()
    {
        CreateObjectField(ref obj, "Object Ref:");

        CreateButton("ReTexture", "", () => OnButtonPressed());

        // "target" can be any class derrived from ScriptableObject 
        // (could be EditorWindow, MonoBehaviour, etc)
        CreateButton("Add", "", () =>
        {
            Object[] old = SelectedMats;
            SelectedMats = new Object[old.Length + 1];
            for (int i = 0; i < old.Length; i++)
            {
                SelectedMats[i] = old[i];
            }
        });

        CreateButton("Remove", "", () =>
        {
            if (SelectedMats.Length > 0)
            {
                Object[] old = SelectedMats;
                SelectedMats = new Object[old.Length - 1];
                for (int i = 0; i < old.Length - 1; i++)
                {
                    SelectedMats[i] = old[i];
                }
            }
        });

        for (int i = 0; i < SelectedMats.Length; i++)
        {
            SelectedMats[i] = EditorGUILayout.ObjectField(SelectedMats[i], typeof(Material), true);
        }

        CreateButton("Remove Bolt Decals", "", () => OnButtonPressed2());
    }

    private static async void OnButtonPressed()
    {
        if (Helper.IsValid(obj))
        {
            await LibrarySorter.Materials.SetMaterialsToObject((GameObject)obj);
        }

        await Helper.Wait();
    }

    private static async void OnButtonPressed2()
    {
        Transform[] transforms = obj.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            if (t.GetComponent<Renderer>() != null)
            {
                string matname = t.GetComponent<MeshRenderer>().sharedMaterial.name;
                foreach (Material s in SelectedMats)
                {
                    if (matname == s.name)
                    {
                        if (t != null)
                            DestroyImmediate(t.gameObject);
                    }
                }
            }
        }

        await Helper.Wait();
    }
}