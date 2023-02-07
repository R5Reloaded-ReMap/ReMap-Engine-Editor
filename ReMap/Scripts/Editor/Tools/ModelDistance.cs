using UnityEngine;
using UnityEditor;

public class ModelDistance : EditorWindow
{
    bool isValid;
    private Object source;
    private Object target;
    private Vector3 sourceOrigin;
    private Vector3 targetOrigin;
    private bool axisX = true;
    private bool axisY = false;
    private bool axisZ = false;
    //private bool edgeToEdge = false;

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
        sourceOrigin = new Vector3(0, 0, 0);
        targetOrigin = new Vector3(0, 0, 0);

        GUILayout.Label("Select Axis: ");

        EditorGUILayout.Space(4);

        GUILayout.BeginHorizontal();
        axisX = GUILayout.Toggle(axisX, "X");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        axisY = GUILayout.Toggle(axisY, "Y");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        axisZ = GUILayout.Toggle(axisZ, "Z");
        GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //edgeToEdge = GUILayout.Toggle(edgeToEdge, "Edge To Edge Mode");
        //GUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        if(isValid)
        {
            Selection.selectionChanged += SourceAndTargetUpdate;

            GameObject sourceObj = source as GameObject;
            GameObject targetObj = target as GameObject;

            if(axisX)
            {
                sourceOrigin.x = sourceObj.transform.position.x;
                targetOrigin.x = targetObj.transform.position.x;
            }

            if(axisY)
            {
                sourceOrigin.y = sourceObj.transform.position.y;
                targetOrigin.y = targetObj.transform.position.y;
            }

            if(axisZ)
            {
                sourceOrigin.z = sourceObj.transform.position.z;
                targetOrigin.z = targetObj.transform.position.z;
            }

            //if(edgeToEdge)
            //{
            //    
            //}


            float dist = Vector3.Distance(sourceOrigin, targetOrigin);

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
            GUILayout.Label("Select two prefabs to add it automatically or\nadd two prefabs to measure the distance:");
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