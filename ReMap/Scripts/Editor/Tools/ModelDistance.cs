using UnityEngine;
using UnityEditor;

public class ModelDistance : EditorWindow
{
    bool isValid;
    private Object source;
    private Object target;

    [MenuItem("ReMap/Tools/Measure Distance", false, 100)]
    public static void Init()
    {
        ModelDistance window = (ModelDistance)EditorWindow.GetWindow(typeof(ModelDistance), false, "Measure Distance");
        window.Show();
        window.minSize = new Vector2(375, 140);
        window.maxSize = new Vector2(375, 140);
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        if(isValid)
        {
            Selection.selectionChanged += SourceAndTargetUpdate;

            GameObject sourceObj = source as GameObject;
            GameObject targetObj = target as GameObject;
            float dist = Vector3.Distance(sourceObj.transform.position, targetObj.transform.position);

            GUILayout.BeginVertical("box");
            GUILayout.Label("Click on 2 prefabs to add it automatically or:");
            GUILayout.Label("Add two prefabs to Measure the distance:");
            source = EditorGUILayout.ObjectField(source, typeof(Object), true);
            target = EditorGUILayout.ObjectField(target, typeof(Object), true);
            GUILayout.EndVertical();

            // Distance
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Distance:");
            GUILayout.TextField(dist.ToString(), GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = dist.ToString();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("Click on 2 prefabs to add it automatically or:");
            GUILayout.Label("Add two prefabs to Measure the distance:");
            source = EditorGUILayout.ObjectField(source, typeof(Object), true);
            target = EditorGUILayout.ObjectField(target, typeof(Object), true);
            GUILayout.EndVertical();

            // Distance
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Distance:");
            GUILayout.TextField("0.0", GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = "0.0";
            GUILayout.EndHorizontal();
        }
    }

    void Update()
    {
        if(source != null && target != null)
        {
            isValid = true;
        }
        else if(Selection.count >= 2)
        {
            SourceAndTargetUpdate();
            isValid = true;
        }
        else isValid = false;
    }

    void SourceAndTargetUpdate()
    {
        if(Selection.count >= 2)
        {
            source = Selection.gameObjects[0];
            target = Selection.gameObjects[1];
        }
    }
}