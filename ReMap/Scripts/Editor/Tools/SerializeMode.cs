using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

public class SerializeMode : EditorWindow
{
    public enum DirectionType { Top, Bottom, Forward, Backward, Left, Right };

    //public static bool OnGUIActive = false; //Test

    private static float w_spacing = 0;
    private static float d_spacing = 0;
    private static float h_spacing = 0;
    private static bool StackingMode = false;
    private static bool UnidirectionalMode = false;

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
        //OnGUIActive = true; //Test

        GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
        centeredStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Amount of props selected: " + Selection.count.ToString(), centeredStyle, GUILayout.ExpandWidth(true));

        EditorGUILayout.Space(4);

        GUILayout.BeginHorizontal();
        if(StackingMode = GUILayout.Toggle(StackingMode, "Stacking Mode"))
        {
            w_spacing = 0;
            d_spacing = 0;
            h_spacing = 0;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        GUILayout.BeginHorizontal();
        if(UnidirectionalMode = GUILayout.Toggle(UnidirectionalMode, "Unidirectional Mode"))
        {
            //w_spacing = 0;
            //d_spacing = 0;
            //h_spacing = 0;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        if(!StackingMode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("T/U Spacing: ");
            float.TryParse(GUILayout.TextField(h_spacing.ToString()), out h_spacing);
            if (GUILayout.Button("Reset T/U values")) h_spacing = 0;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("F/B Spacing: ");
            float.TryParse(GUILayout.TextField(w_spacing.ToString()), out w_spacing);
            if (GUILayout.Button("Reset F/B values")) w_spacing = 0;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("L/R Spacing: ");
            float.TryParse(GUILayout.TextField(d_spacing.ToString()), out d_spacing);
            if (GUILayout.Button("Reset L/R values")) d_spacing = 0;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset all values"))
            {
                w_spacing = 0;
                d_spacing = 0;
                h_spacing = 0;
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

        //DrawSerializeMode.Update(); //Test
    }

    /// <summary>
    /// Duplicate prop in direction you want
    /// </summary>
    public static void DuplicateProp(int directionType)
    {

        GameObject[] source = Selection.gameObjects;
        GameObject[] newSource = new GameObject[0];

        if(StackingMode)
        {
            Vector3 size = FindSelectionBounds();
            w_spacing = size.z;
            d_spacing = size.x;
            h_spacing = size.y;
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
    /// Use for: DuplicateProp(int directionType)
    /// </summary>
    public static void SetPropOrigin(GameObject reference, GameObject newGo, int directionType)
    {
        float w = 0;
        float d = 0;
        float h = 0;

        Transform refTranform = reference.transform;
        Vector3 origin = refTranform.position;

        Transform goTranform = newGo.transform;

        Quaternion referenceRotation = refTranform.rotation;
        refTranform.rotation = Quaternion.Euler(0, 0, 0);
        foreach(Renderer o in reference.GetComponentsInChildren<Renderer>())
        {
            if(o.bounds.size.z > w)
                w = o.bounds.size.z;

            if(o.bounds.size.x > d)
                d = o.bounds.size.x;

            if(o.bounds.size.y > h)
                h = o.bounds.size.y;
        }
        refTranform.rotation = referenceRotation;

        if(w_spacing != 0) w = w_spacing;
        if(d_spacing != 0) d = d_spacing;
        if(h_spacing != 0) h = h_spacing;

        switch(directionType)
        {
            case (int)DirectionType.Top:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.up * h);
                }
                else goTranform.Translate(origin + refTranform.up * h);

                break;
            case (int)DirectionType.Bottom:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.up * -h);
                }
                else goTranform.Translate(origin + refTranform.up * -h);

                break;
            case (int)DirectionType.Forward:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.forward * w);
                }
                else goTranform.Translate(origin + refTranform.forward * w);

                break;
            case (int)DirectionType.Backward:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.forward * -w);
                }
                else goTranform.Translate(origin + refTranform.forward * -w);
                
                break;
            case (int)DirectionType.Left:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.right * -d);
                }
                else goTranform.Translate(origin + refTranform.right * -d);
                
                break;
            case (int)DirectionType.Right:

                if(UnidirectionalMode)
                {
                    goTranform.Translate(origin + Selection.gameObjects[0].transform.right * d);
                }
                else goTranform.Translate(origin + refTranform.right * d);

                break;
            
            default: break;
        }
    }

    public static Vector3 FindSelectionBounds()
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

        DestroyImmediate(sourceParent as UnityEngine.Object);

        return size;
    }
}
