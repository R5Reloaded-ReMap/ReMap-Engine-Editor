using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Edit window to copy and recreate existing models
/// </summary>
public class SerializeTool : EditorWindow
{
    public enum DirectionType { Top, Bottom, Forward, Backward, Left, Right };
    public enum BoundsInt { Size, Max, Min, Center, Extents };

    private static float x_entry = 0;
    private static float y_entry = 0;
    private static float z_entry = 0;
    private static float x_spacing = 0;
    private static float y_spacing = 0;
    private static float z_spacing = 0;
    private static bool UnidirectionalMode = false;
    private static bool MirrorMode = false;

    // Utility
    private static GameObject[] Source;
    private static Vector3 BoundsSize;
    private static Vector3 BoundsMax;
    private static Vector3 BoundsMin;
    private static Vector3 BoundsExtents;


    /// <summary>
    /// SerializeTool.Init()
    /// </summary>
    [MenuItem("ReMap/Tools/Serialize Tool", false, 100)]
    public static void Init()
    {
        SerializeTool window = (SerializeTool)EditorWindow.GetWindow(typeof(SerializeTool), false, "Serialize Tool");
        window.minSize = new Vector2(360, 300);
        window.maxSize = new Vector2(360, 300);
        window.Show();
    }

    /// <summary>
    /// SerializeTool.OnGUI() : When the window is displayed
    /// </summary>
    void OnGUI()
    {
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
        centeredStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Amount of props selected: " + Selection.count.ToString(), centeredStyle, GUILayout.ExpandWidth(true));

        EditorGUILayout.Space(4);

        EditorGUILayout.Space(6);

        GUILayout.BeginHorizontal();
        UnidirectionalMode = GUILayout.Toggle(UnidirectionalMode, "Unidirectional Mode");
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(6);
        
        GUILayout.BeginHorizontal();
        MirrorMode = GUILayout.Toggle(MirrorMode, "Mirror Mode");
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        GUILayout.BeginHorizontal();
        GUILayout.Label("T/U Spacing: ");
        float.TryParse(GUILayout.TextField(y_entry.ToString()), out y_entry);
        if (GUILayout.Button("Reset T/U values")) y_entry = 0;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("F/B Spacing: ");
        float.TryParse(GUILayout.TextField(z_entry.ToString()), out z_entry);
        if (GUILayout.Button("Reset F/B values")) z_entry = 0;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("L/R Spacing: ");
        float.TryParse(GUILayout.TextField(x_entry.ToString()), out x_entry);
        if (GUILayout.Button("Reset L/R values")) x_entry = 0;
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset all values"))
        {
            z_entry = 0;
            x_entry = 0;
            y_entry = 0;
        }

        EditorGUILayout.Space(4);

        if (GUILayout.Button("Duplicate to TOP"))
            DuplicateInit((int)DirectionType.Top);

        if (GUILayout.Button("Duplicate to UNDER"))
            DuplicateInit((int)DirectionType.Bottom);

        if (GUILayout.Button("Duplicate to FORWARD"))
            DuplicateInit((int)DirectionType.Forward);

        if (GUILayout.Button("Duplicate to BACKWARD"))
            DuplicateInit((int)DirectionType.Backward);

        if (GUILayout.Button("Duplicate to LEFT"))
            DuplicateInit((int)DirectionType.Left);

        if (GUILayout.Button("Duplicate to RIGHT"))
            DuplicateInit((int)DirectionType.Right);
    }

    /// <summary>
    /// SerializeTool.DuplicateInit(int) : Init
    /// </summary>
    public static void DuplicateInit(int directionType)
    {
        Source = Selection.gameObjects;

        Vector3[] DeterminedBounds = DetermineBoundsInSelection();
        BoundsSize = DeterminedBounds[(int)BoundsInt.Size];
        BoundsMax = DeterminedBounds[(int)BoundsInt.Max];
        BoundsMin = DeterminedBounds[(int)BoundsInt.Min];
        BoundsExtents = DeterminedBounds[(int)BoundsInt.Extents];

        ReMapConsole.Log($"[Serialize Tool] Bounds Size: {BoundsSize}", ReMapConsole.LogType.Info);
        ReMapConsole.Log($"[Serialize Tool] Bounds Max:  {BoundsMax}",  ReMapConsole.LogType.Info);
        ReMapConsole.Log($"[Serialize Tool] Bounds Min:  {BoundsMin}",  ReMapConsole.LogType.Info);

        x_spacing = x_entry + BoundsSize.x;
        y_spacing = y_entry + BoundsSize.y;
        z_spacing = z_entry + BoundsSize.z;

        DuplicateProps(directionType);
    }

    /// <summary>
    /// SerializeTool.DuplicateProps(int) : Duplicates the props in a desired direction
    /// </summary>
    public static void DuplicateProps(int directionType)
    {
        GameObject[] newSource = new GameObject[0];

        foreach ( GameObject go in Source )
        {
            string name = go.name;

            if(!IsValidGameObject(go))
                continue;

            PropScript script = go.GetComponent<PropScript>();

            UnityEngine.Object loadedPrefabResource = ImportExportJson.FindPrefabFromName(name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Serialize Tool] Couldnt find prefab with name of: {name}" , ReMapConsole.LogType.Error);
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

            //ReMapConsole.Log("[Serialize Tool] Created " + name, ReMapConsole.LogType.Info);

            int currentLength = newSource.Length;
            Array.Resize(ref newSource, currentLength + 1);
            newSource[currentLength] = obj;
        }

        Selection.objects = newSource;
    }

    /// <summary>
    /// SerializeTool.SetPropOrigin(GameObject, GameObject, int) : Duplicates the props in a desired direction
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

        x = x_spacing;
        y = y_spacing;
        z = z_spacing;

        if(MirrorMode)
        {
            refTranform.rotation = Quaternion.Euler(0, 0, 0);
            Bounds bounds = new Bounds();
            bool firstRenderer = true;
            foreach (Renderer renderer in reference.GetComponentsInChildren<Renderer>())
            {
                if (firstRenderer)
                {
                    bounds = renderer.bounds;
                    firstRenderer = false;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
            refTranform.rotation = referenceRotation;

            ReMapConsole.Log("[Serialize Tool] CenterProp: " + bounds.center, ReMapConsole.LogType.Info);

            switch(directionType)
            {
                case (int)DirectionType.Top:
                case (int)DirectionType.Forward:
                case (int)DirectionType.Right:

                    x = Vector3.Distance(new Vector3(bounds.center.x, 0, 0), new Vector3(BoundsMax.x, 0, 0)) * 2;
                    y = Vector3.Distance(new Vector3(0, bounds.center.y, 0), new Vector3(0, BoundsMax.y, 0)) * 2;
                    z = Vector3.Distance(new Vector3(0, 0, bounds.center.z), new Vector3(0, 0, BoundsMax.z)) * 2;

                    break;

                case (int)DirectionType.Bottom:
                case (int)DirectionType.Backward:
                case (int)DirectionType.Left:

                    x = Vector3.Distance(new Vector3(bounds.center.x, 0, 0), new Vector3(BoundsMin.x, 0, 0)) * 2;
                    y = Vector3.Distance(new Vector3(0, bounds.center.y, 0), new Vector3(0, BoundsMin.y, 0)) * 2;
                    z = Vector3.Distance(new Vector3(0, 0, bounds.center.z), new Vector3(0, 0, BoundsMin.z)) * 2;

                    break;
                
                default: break;
            }

            ReMapConsole.Log($"[Serialize Tool] MirrorMode Vec: ({x} {y} {z})", ReMapConsole.LogType.Info);
        }

        switch(directionType)
        {
            case (int)DirectionType.Top:

                if(UnidirectionalMode)
                {
                    goTranform.position = origin + (Selection.gameObjects[0].transform.up * y);
                }
                else goTranform.position = origin + (refTranform.up * y);

                break;
            case (int)DirectionType.Bottom:

                if(UnidirectionalMode)
                {
                    goTranform.position = origin + (Selection.gameObjects[0].transform.up * -y);
                }
                else goTranform.position = origin + (refTranform.up * -y);

                break;
            case (int)DirectionType.Forward:

                if(UnidirectionalMode)
                {
                    goTranform.position = origin + (Selection.gameObjects[0].transform.forward * z);
                }
                else goTranform.position = origin + (refTranform.forward * z);

                break;
            case (int)DirectionType.Backward:

                if(UnidirectionalMode)
                {
                    goTranform.position = origin + (Selection.gameObjects[0].transform.forward * -z);
                }
                else goTranform.position = origin + (refTranform.forward * -z);
                
                break;
            case (int)DirectionType.Left:

                if(UnidirectionalMode)
                {
                    goTranform.position = origin + (Selection.gameObjects[0].transform.right * -x);
                }
                else goTranform.position = origin + (refTranform.right * -x);
                
                break;
            case (int)DirectionType.Right:

                if(UnidirectionalMode)
                {
                    goTranform.position = origin + (Selection.gameObjects[0].transform.right * x);
                }
                else goTranform.position = origin + (refTranform.right * x);

                break;
            
            default: break;
        }
    }

    /// <summary>
    /// SerializeTool.DetermineBoundsInSelection() : Determine the bounds
    /// </summary>
    public static Vector3[] DetermineBoundsInSelection()
    {
        List<GameObject> newSource = new List<GameObject>();

        GameObject sourceParent = new GameObject();
        sourceParent.transform.position = Source[0].transform.position;
        sourceParent.transform.eulerAngles = Source[0].transform.eulerAngles;

        foreach (GameObject go in Source)
        {
            string name = go.name;

            if(!IsValidGameObject(go))
                continue;

            UnityEngine.Object loadedPrefabResource = ImportExportJson.FindPrefabFromName(name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Serialize Tool] Couldn't find prefab with name of: {name}", ReMapConsole.LogType.Error);
                continue;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = go.transform.position;
            obj.transform.eulerAngles = go.transform.eulerAngles;
            obj.transform.localScale = go.transform.localScale;

            obj.transform.SetParent(sourceParent.transform);

            newSource.Add(obj);
        }

        sourceParent.transform.eulerAngles = Vector3.zero;

        Bounds bounds = new Bounds();
        bool firstRenderer = true;
        foreach (GameObject go in newSource)
        {
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                if (firstRenderer)
                {
                    bounds = renderer.bounds;
                    firstRenderer = false;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }

        Vector3[] boundsArray = new Vector3[5];
        boundsArray[(int)BoundsInt.Size] = bounds.size;
        boundsArray[(int)BoundsInt.Max] = bounds.max;
        boundsArray[(int)BoundsInt.Min] = bounds.min;
        boundsArray[(int)BoundsInt.Center] = bounds.center;
        boundsArray[(int)BoundsInt.Extents] = bounds.extents;

        foreach (GameObject go in newSource)
        {
            GameObject.DestroyImmediate(go);
        }

        GameObject.DestroyImmediate(sourceParent);

        return boundsArray;
    }


    /// <summary>
    /// SerializeTool.DetermineBoundsInProp() : Determine the bounds of a prop
    /// </summary>
    public static Vector3[] DetermineBoundsInProp( GameObject go )
    {
        GameObject sourceParent = new GameObject();
        sourceParent.transform.position = go.transform.position;
        sourceParent.transform.eulerAngles = go.transform.eulerAngles;

        string name = go.name;

        if(!IsValidGameObject(go))
            return new Vector3[4];

        UnityEngine.Object loadedPrefabResource = ImportExportJson.FindPrefabFromName(name);
        if (loadedPrefabResource == null)
        {
            ReMapConsole.Log($"[Serialize Tool] Couldn't find prefab with name of: {name}", ReMapConsole.LogType.Error);
            return new Vector3[4];
        }

        GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
        obj.transform.position = go.transform.position;
        obj.transform.eulerAngles = go.transform.eulerAngles;
        obj.transform.localScale = go.transform.localScale;

        obj.transform.SetParent(sourceParent.transform);

        sourceParent.transform.eulerAngles = Vector3.zero;

        Bounds bounds = new Bounds();
        bool firstRenderer = true;
        foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
        {
            if (firstRenderer)
            {
                bounds = renderer.bounds;
                firstRenderer = false;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        Vector3[] boundsArray = new Vector3[4];
        boundsArray[(int)BoundsInt.Size] = bounds.size;
        boundsArray[(int)BoundsInt.Max] = bounds.max;
        boundsArray[(int)BoundsInt.Min] = bounds.min;
        boundsArray[(int)BoundsInt.Center] = bounds.center;

        GameObject.DestroyImmediate(obj);
        GameObject.DestroyImmediate(sourceParent);

        return boundsArray;
    }

    public static bool IsValidGameObject(GameObject go)
    {
        if(go == null || !go.name.Contains("mdl#"))
            return false;

        return true;
    }
}
