using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridTool : EditorWindow
{
    private Object source;
    private float gridX = 10.0f;
    private float gridY = 10.0f;
    private float spacing = 256f;

    private float rotx = 0.0f;
    private float roty = 0.0f;
    private float rotz = 0.0f;
    private bool isfloor = true;

    [MenuItem("R5Reloaded/Tools/Grid Tool", false, 200)]
    static void Init()
    {
        GridTool window = (GridTool)EditorWindow.GetWindow(typeof(GridTool), false, "Grid Tool");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab to use:");
        source = EditorGUILayout.ObjectField(source, typeof(Object), true);

        GUILayout.Label("\nRotation:");
        rotx = EditorGUILayout.FloatField("X:", rotx);
        roty = EditorGUILayout.FloatField("Y:", roty);
        rotz = EditorGUILayout.FloatField("Z:", rotz);

        GUILayout.Label("\nChecked: Floor / Unchecked: Wall");
        isfloor = EditorGUILayout.Toggle("", isfloor);
        
        GUILayout.Label("\nGrid Settings:");
        gridX = EditorGUILayout.FloatField("Grid X:", gridX);
        gridY = EditorGUILayout.FloatField("Grid Y:", gridY);
        spacing = EditorGUILayout.FloatField("Spacing:", spacing);

        if (GUILayout.Button("Create Grid"))
            CreateGrid();
    }

    void CreateGrid()
    {
        if(source == null)
            return;

        if(!source.name.Contains("mdl#"))
            return;

        string name = "Grid (" + Random.Range(0, 100) + ")";

        GameObject objToSpawn = new GameObject(name);
        objToSpawn.name = name;

        GameObject parent = GameObject.Find(name);

        for (int y = 0; y < gridY; y++) 
        {
            for (int x = 0; x < gridX; x++)
            {
                Vector3 pos = new Vector3(x, 0, y) * spacing;
                if(!isfloor)
                    pos = new Vector3(x, y, 0) * spacing;

                GameObject obj = PrefabUtility.InstantiatePrefab(source as GameObject) as GameObject;
                obj.transform.position = pos;
                obj.transform.eulerAngles = new Vector3(rotx,roty,rotz);

                if(parent != null) //If its not null set it as a child
                    obj.gameObject.transform.parent = parent.transform;
            }
        }
    }
}
