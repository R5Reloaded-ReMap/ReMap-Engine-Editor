using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class SerializeMode : EditorWindow
{
    public enum DirectionType { Top, Bottom, Forward, Backward, Left, Right };

    [MenuItem("ReMap/Tools/Serialize Mode", false, 100)]
    public static void Init()
    {
        SerializeMode window = (SerializeMode)EditorWindow.GetWindow(typeof(SerializeMode), false, "Serialize Mode");
        window.minSize = new Vector2(300, 180);
        window.maxSize = new Vector2(300, 180);
        window.Show();
    }

    void OnGUI()
    {
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
        centeredStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Amount of props selected: " + Selection.count.ToString(), centeredStyle, GUILayout.ExpandWidth(true));



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

            ReMapConsole.Log("[Serialize Mode] Created " + name, ReMapConsole.LogType.Info);

            //Selection.RemoveFromSelection(go);
        }
    }

    /// <summary>
    /// Use for: DuplicateProp(int directionType)
    /// </summary>
    public static Vector3 SetPropOrigin(GameObject go, int directionType)
    {
        float w = 0;
        float d = 0;
        float h = 0;

        Vector3 vector = new Vector3(0, 0, 0);
        Vector3 origin = go.transform.position;
        Vector3 angles = go.transform.eulerAngles;

        foreach(Renderer o in go.GetComponentsInChildren<Renderer>())
        {
            if(o.bounds.size.z > w)
                w = o.bounds.size.z;

            if(o.bounds.size.x > d)
                d = o.bounds.size.x;

            if(o.bounds.size.y > h)
                h = o.bounds.size.y;
        }

        switch(directionType)
        {
            case (int)DirectionType.Top:
                vector = new Vector3(0, h, 0);
                break;
            case (int)DirectionType.Bottom:
                vector = new Vector3(0, -h, 0);
                break;
            case (int)DirectionType.Forward:
                vector = new Vector3(0, 0, w);
                break;
            case (int)DirectionType.Backward:
                vector = new Vector3(0, 0, -w);
                break;
            case (int)DirectionType.Left:
                vector = new Vector3(-d, 0, 0);
                break;
            case (int)DirectionType.Right:
                vector = new Vector3(d, 0, 0);
            break;
        }

        return origin + vector;
    }
}
