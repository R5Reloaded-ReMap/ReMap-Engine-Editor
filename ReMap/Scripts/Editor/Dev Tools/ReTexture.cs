
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ReTexture : EditorWindow
{
    private static UnityEngine.Object obj;

    bool expanded = false;
    private static List<Object> SelectedMats = new List<Object>();
    private static Object NewMat;
    private static Object SelectMat;
    int num = 0;

    public static void Init()
    {
        ReTexture window = (ReTexture)EditorWindow.GetWindow(typeof(ReTexture), false, "Map Helper");
        window.Show();
    }

    void OnGUI()
    {
        CreateObjectField(ref obj, "Object Ref:");

        GUILayout.BeginVertical("box");
        GUILayout.Label("Retexture All Meshes:");
        CreateButton("ReTexture", "", () => OnButtonPressed());
        GUILayout.EndVertical();

        GUILayout.Space(5);

        GUILayout.BeginVertical("box");
        GUILayout.Label("Remove Meshes by Material:");
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Add Material:", GUILayout.Width(100));
        NewMat = EditorGUILayout.ObjectField(NewMat, typeof(Material), true);
        GUILayout.EndHorizontal();
        if (NewMat != null)
        {
            SelectedMats.Add(NewMat);
            NewMat = null;
        }

        for (int i = 0; i < SelectedMats.Count; i++)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(SelectedMats[i], typeof(Material), true);
            CreateButton("Remove", "", () =>
            {
                SelectedMats.RemoveAt(i);
            });
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
        CreateButton("Remove Meshes", "", () => OnButtonPressed2());
        GUILayout.EndVertical();

        GUILayout.Space(5);

        GUILayout.BeginVertical("box");
        GUILayout.Label("Select Meshes by Material:");
        GUILayout.Space(10);
        SelectMat = EditorGUILayout.ObjectField(SelectMat, typeof(Material), true);
        CreateButton("Select All", "", () =>
            {
                List<Object> SelectedObjects = new List<Object>();

                Transform[] transforms = obj.GetComponentsInChildren<Transform>();
                foreach (Transform t in transforms)
                {
                    if (t.GetComponent<Renderer>() != null)
                    {
                        string matname = t.GetComponent<MeshRenderer>().sharedMaterial.name;
                        foreach (Material s in SelectedMats)
                        {
                            if (matname == SelectMat.name)
                            {
                                SelectedObjects.Add(t.gameObject);
                            }
                        }
                    }
                }

                Selection.objects = SelectedObjects.ToArray();
            });
        GUILayout.EndVertical();
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