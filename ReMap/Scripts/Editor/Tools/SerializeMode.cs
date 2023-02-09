using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Edit window to copy and recreate existing models
/// </summary>
public class SerializeMode : EditorWindow
{
    public enum DirectionType { Top, Bottom, Forward, Backward, Left, Right };

    private static float x_spacing = 0;
    private static float y_spacing = 0;
    private static float z_spacing = 0;
    private static bool StackingMode = false;
    private static bool UnidirectionalMode = false;
    private static bool MirrorMode = false;
    private static Vector3 BoundsMax;
    private static Vector3 BoundsMin;

    /// <summary>
    /// SerializeMode.Init()
    /// </summary>
    [MenuItem("ReMap/Tools/Serialize Mode", false, 100)]
    public static void Init()
    {
        SerializeMode window = (SerializeMode)EditorWindow.GetWindow(typeof(SerializeMode), false, "Serialize Mode");
        window.minSize = new Vector2(360, 300);
        window.maxSize = new Vector2(360, 300);
        window.Show();
    }

    /// <summary>
    /// SerializeMode.OnGUI() : When the window is displayed
    /// </summary>
    void OnGUI()
    {
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
        centeredStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Amount of props selected: " + Selection.count.ToString(), centeredStyle, GUILayout.ExpandWidth(true));

        EditorGUILayout.Space(4);

        GUILayout.BeginHorizontal();
        if(StackingMode = GUILayout.Toggle(StackingMode, "Stacking Mode"))
        {
            x_spacing = 0;
            y_spacing = 0;
            z_spacing = 0;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        GUILayout.BeginHorizontal();
        UnidirectionalMode = GUILayout.Toggle(UnidirectionalMode, "Unidirectional Mode");
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(6);
        
        GUILayout.BeginHorizontal();
        MirrorMode = GUILayout.Toggle(MirrorMode, "Mirror Mode");
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        if(!StackingMode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("T/U Spacing: ");
            float.TryParse(GUILayout.TextField(y_spacing.ToString()), out y_spacing);
            if (GUILayout.Button("Reset T/U values")) y_spacing = 0;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("F/B Spacing: ");
            float.TryParse(GUILayout.TextField(z_spacing.ToString()), out z_spacing);
            if (GUILayout.Button("Reset F/B values")) z_spacing = 0;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("L/R Spacing: ");
            float.TryParse(GUILayout.TextField(x_spacing.ToString()), out x_spacing);
            if (GUILayout.Button("Reset L/R values")) x_spacing = 0;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset all values"))
            {
                z_spacing = 0;
                x_spacing = 0;
                y_spacing = 0;
            }

            EditorGUILayout.Space(4);
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
    /// SerializeMode.DuplicateProp(int) : Duplicates the props in a desired direction
    /// </summary>
    public static void DuplicateProp(int directionType)
    {

        GameObject[] source = Selection.gameObjects;
        GameObject[] newSource = new GameObject[0];

        if(StackingMode)
        {
            Vector3 size = FindBoundsInSelection();
            x_spacing = size.x;
            y_spacing = size.y;
            z_spacing = size.z;
            ReMapConsole.Log($"[Serialize Mode] bounds.size: {size}" , ReMapConsole.LogType.Info);
        }

        foreach ( GameObject go in source )
        {
            string name = go.name;

            if(go == null || !name.Contains("mdl#"))
                continue;

            PropScript script = go.GetComponent<PropScript>();

            UnityEngine.Object loadedPrefabResource = ImportExportJson.FindPrefabFromName(name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Serialize Mode] Couldnt find prefab with name of: {name}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            /* obj.transform.position = */ SetPropOrigin(go, obj, directionType);
            obj.transform.eulerAngles = go.transform.eulerAngles;
            obj.gameObject.transform.localScale = go.transform.lossyScale;

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
    /// SerializeMode.SetPropOrigin(GameObject, GameObject, int) : Duplicates the props in a desired direction
    /// </summary>
    public static void SetPropOrigin(GameObject reference, GameObject newGo, int directionType)
    {
        float x = 0;
        float y = 0;
        float z = 0;

        Transform refTranform = reference.transform;
        Vector3 origin = refTranform.position;

        Transform goTranform = newGo.transform;

        Quaternion referenceRotation = refTranform.rotation;
        refTranform.rotation = Quaternion.Euler(0, 0, 0);
        foreach(Renderer o in reference.GetComponentsInChildren<Renderer>())
        {
            if(o.bounds.size.x > x)
                x = o.bounds.size.x;

            if(o.bounds.size.y > y)
                y = o.bounds.size.y;

            if(o.bounds.size.z > z)
                z = o.bounds.size.z;
        }
        refTranform.rotation = referenceRotation;

        if(x_spacing != 0) x = x_spacing;
        if(y_spacing != 0) y = y_spacing;
        if(z_spacing != 0) z = z_spacing;

        if(MirrorMode)
        {
            x = Vector3.Distance(new Vector3(refTranform.position.x, 0, 0), new Vector3(BoundsMax.x, 0, 0)) * 2;//Mathf.Clamp(refTranform.position.x, BoundsMin.x, BoundsMax.x);
            y = Vector3.Distance(new Vector3(0, refTranform.position.y, 0), new Vector3(0, BoundsMax.y, 0)) * 2;//Mathf.Clamp(refTranform.position.y, BoundsMin.y, BoundsMax.y);
            z = Vector3.Distance(new Vector3(0, 0, refTranform.position.z), new Vector3(0, 0, BoundsMax.z)) * 2;//Mathf.Clamp(refTranform.position.z, BoundsMin.z, BoundsMax.z);
            ReMapConsole.Log($"[Serialize Mode] ({x} {y} {z})", ReMapConsole.LogType.Info);
        }

        switch(directionType)
        {
            case (int)DirectionType.Top:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.up * y);
                }
                else goTranform.Translate(origin + refTranform.up * y);

                break;
            case (int)DirectionType.Bottom:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.up * -y);
                }
                else goTranform.Translate(origin + refTranform.up * -y);

                break;
            case (int)DirectionType.Forward:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.forward * z);
                }
                else goTranform.Translate(origin + refTranform.forward * z);

                break;
            case (int)DirectionType.Backward:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.forward * -z);
                }
                else goTranform.Translate(origin + refTranform.forward * -z);
                
                break;
            case (int)DirectionType.Left:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.right * -x);
                }
                else goTranform.Translate(origin + refTranform.right * -x);
                
                break;
            case (int)DirectionType.Right:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.right * x);
                }
                else goTranform.Translate(origin + refTranform.right * x);

                break;
            
            default: break;
        }
    }

    /// <summary>
    /// SerializeMode.FindBoundsInSelection() : Find the bounds (x, y, z)
    /// </summary>
    public static Vector3 FindBoundsInSelection()
    {
        GameObject[] source = Selection.gameObjects;
        GameObject[] newSource = new GameObject[0];

        GameObject sourceParent = new GameObject();
        sourceParent.transform.position = source[0].transform.position;
        sourceParent.transform.eulerAngles = source[0].transform.eulerAngles;

        Bounds bounds = new Bounds(sourceParent.transform.position, sourceParent.transform.eulerAngles);

        foreach ( GameObject go in source )
        {
            string name = go.name;

            if(go == null || !name.Contains("mdl#"))
                continue;

            UnityEngine.Object loadedPrefabResource = ImportExportJson.FindPrefabFromName(name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Serialize Mode] Couldnt find prefab with name of: {name}" , ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = go.transform.position;
            obj.transform.eulerAngles = go.transform.eulerAngles;
            obj.gameObject.transform.localScale = go.transform.lossyScale;

            obj.transform.SetParent(sourceParent.transform);

            int currentLength = newSource.Length;
            Array.Resize(ref newSource, currentLength + 1);
            newSource[currentLength] = obj;
        }

        sourceParent.transform.eulerAngles = Vector3.zero;

        foreach( GameObject go in newSource )
        {
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        Vector3 size = bounds.size;

        BoundsMax = bounds.max;
        BoundsMin = bounds.min;

        DestroyImmediate(sourceParent as UnityEngine.Object);

        return size;
    }
}
