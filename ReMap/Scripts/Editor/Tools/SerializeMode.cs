using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

public class SerializeMode : EditorWindow
{
    public enum DirectionType { Top, Bottom, Forward, Backward, Left, Right };

    private static int w_spacing = 0;
    private static int d_spacing = 0;
    private static int h_spacing = 0;

    [MenuItem("ReMap/Tools/Serialize Mode", false, 100)]
    public static void Init()
    {
        SerializeMode window = (SerializeMode)EditorWindow.GetWindow(typeof(SerializeMode), false, "Serialize Mode");
        window.minSize = new Vector2(360, 300);
        window.maxSize = new Vector2(360, 300);
        window.Show();
    }

    void OnGUI()
    {
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
        centeredStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Amount of props selected: " + Selection.count.ToString(), centeredStyle, GUILayout.ExpandWidth(true));

        GUILayout.BeginHorizontal();
        GUILayout.Label("T/U Spacing: ");
        int.TryParse(GUILayout.TextField(h_spacing.ToString()), out h_spacing);
        if (GUILayout.Button("Reset T/U values")) h_spacing = 0;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("F/B Spacing: ");
        int.TryParse(GUILayout.TextField(w_spacing.ToString()), out w_spacing);
        if (GUILayout.Button("Reset F/B values")) w_spacing = 0;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("L/R Spacing: ");
        int.TryParse(GUILayout.TextField(d_spacing.ToString()), out d_spacing);
        if (GUILayout.Button("Reset L/R values")) d_spacing = 0;
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset all values"))
        {
            w_spacing = 0;
            d_spacing = 0;
            h_spacing = 0;
        }

        if (GUILayout.Button("Duplicate to TOP"))
            DuplicateProp((int)DirectionType.Top);

        if (GUILayout.Button("Duplicate to UNDER"))
            DuplicateProp((int)DirectionType.Bottom);

        if (GUILayout.Button("Duplicate to FORWARD"))
            DuplicateProp((int)DirectionType.Forward);

        if (GUILayout.Button("Duplicate to BACKWARD"))
            DuplicateProp((int)DirectionType.Backward);

        if (GUILayout.Button("Duplicate to LEFT"))
            DuplicateProp((int)DirectionType.Left);

        if (GUILayout.Button("Duplicate to RIGHT"))
            DuplicateProp((int)DirectionType.Right);
    }

    /// <summary>
    /// Duplicate prop in direction you want
    /// </summary>
    public static void DuplicateProp(int directionType)
    {
        GameObject[] source = Selection.gameObjects;
        GameObject[] newSource = new GameObject[0];

        foreach ( GameObject go in source )
        {
            if(go == null)
                return;

            string name = go.name;
            PropScript script = go.GetComponent<PropScript>();

            if(!name.Contains("mdl#"))
                return;

            UnityEngine.Object loadedPrefabResource = ImportExportJson.FindPrefabFromName(name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Serialize Mode] Couldnt find prefab with name of: {name}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = SetPropOrigin(go, directionType);
            obj.transform.eulerAngles = go.transform.eulerAngles;
            obj.gameObject.transform.localScale = go.transform.localScale;

            PropScript scriptInstance = obj.GetComponent<PropScript>();
            scriptInstance.fadeDistance = script.fadeDistance;
            scriptInstance.allowMantle = script.allowMantle;
            scriptInstance.realmID = script.realmID;

            obj.transform.SetParent(go.transform.parent);

            ReMapConsole.Log("[Serialize Mode] Created " + name, ReMapConsole.LogType.Info);

            int currentLength = newSource.Length;
            Array.Resize(ref newSource, currentLength + 1);
            newSource[currentLength] = obj;
        }

        Selection.objects = newSource;
    }

    /// <summary>
    /// Use for: DuplicateProp(int directionType)
    /// </summary>
    public static Vector3 SetPropOrigin(GameObject go, int directionType)
    {
        float w = 0;
        float d = 0;
        float h = 0;

        Transform goTranform = go.transform;
        Vector3 vector = new Vector3(0, 0, 0);
        Vector3 origin = goTranform.position;
        Vector3 angles = goTranform.eulerAngles;

        Quaternion referenceRotation = goTranform.rotation;
        goTranform.rotation = Quaternion.Euler(0, 0, 0);
        foreach(Renderer o in go.GetComponentsInChildren<Renderer>())
        {
            if(o.bounds.size.z > w)
                w = o.bounds.size.z;

            if(o.bounds.size.x > d)
                d = o.bounds.size.x;

            if(o.bounds.size.y > h)
                h = o.bounds.size.y;
        }
        goTranform.rotation = referenceRotation;

        if(w_spacing != 0) w = w_spacing;
        if(d_spacing != 0) d = d_spacing;
        if(h_spacing != 0) h = h_spacing;

        switch(directionType)
        {
            case (int)DirectionType.Top:
                vector = origin + goTranform.up * h;
                break;
            case (int)DirectionType.Bottom:
                vector = origin + goTranform.up * -h;
                break;
            case (int)DirectionType.Forward:
                vector = origin + goTranform.forward * w;
                break;
            case (int)DirectionType.Backward:
                vector = origin + goTranform.forward * -w;
                break;
            case (int)DirectionType.Left:
                vector = origin + goTranform.right * -d;
                break;
            case (int)DirectionType.Right:
                vector = origin + goTranform.right * d;
            break;
        }

        ReMapConsole.Log("[Serialize Mode] directionType: " + directionType, ReMapConsole.LogType.Info);

        return vector;
    }
}
