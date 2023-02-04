using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System.Collections.Generic;

public class SerializeMode : EditorWindow
{
    public enum DirectionType { Top, Bottom, Forward, Backward, Left, Right };
    public static Dictionary<DirectionType, Vector3> directions = new Dictionary<DirectionType, Vector3>()
    {
        {DirectionType.Top, new Vector3(0,1,0)},
        {DirectionType.Bottom, new Vector3(0,-1,0)},
        {DirectionType.Forward, new Vector3(0,0,1)},
        {DirectionType.Backward, new Vector3(0,0,-1)},
        {DirectionType.Left, new Vector3(-1,0,0)},
        {DirectionType.Right, new Vector3(1,0,0)}
    };

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
            DuplicateProp(directions[DirectionType.Top]);

        if (GUILayout.Button("Duplicate to UNDER"))
            DuplicateProp(directions[DirectionType.Bottom]);

        if (GUILayout.Button("Duplicate to FORWARD"))
            DuplicateProp(directions[DirectionType.Forward]);

        if (GUILayout.Button("Duplicate to BACKWARD"))
            DuplicateProp(directions[DirectionType.Backward]);

        if (GUILayout.Button("Duplicate to LEFT"))
            DuplicateProp(directions[DirectionType.Left]);

        if (GUILayout.Button("Duplicate to RIGHT"))
            DuplicateProp(directions[DirectionType.Right]);
    }

    /// <summary>
    /// Duplicate prop in direction you want
    /// </summary>
    public static void DuplicateProp(Vector3 vec)
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
            obj.transform.position = SetPropOrigin(go, vec);
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
    /// Use for: DuplicateProp(Vector3 vec)
    /// </summary>
    public static Vector3 SetPropOrigin(GameObject go, Vector3 vec)
    {
        float w = 0;
        float d = 0;
        float h = 0;

        Vector3 vector = new Vector3(0, 0, 0);
        Vector3 origin = go.transform.position;

        foreach(Renderer o in go.GetComponentsInChildren<Renderer>())
        {
            if(o.bounds.size.z > w)
                w = o.bounds.size.z;

            if(o.bounds.size.x > d)
                d = o.bounds.size.x;

            if(o.bounds.size.y > h)
                h = o.bounds.size.y;
        }

        if (vec == directions[DirectionType.Top])
            vector = new Vector3(0, h, 0);

        if (vec == directions[DirectionType.Bottom])
            vector = new Vector3(0, -h, 0);

        if (vec == directions[DirectionType.Forward])
            vector = new Vector3(0, 0, w);

        if (vec == directions[DirectionType.Backward])
            vector = new Vector3(0, 0, -w);

        if (vec == directions[DirectionType.Left])
            vector = new Vector3(-d, 0, 0);

        if (vec == directions[DirectionType.Right])
            vector = new Vector3(d, 0, 0);

        return origin + vector;
    }
}
