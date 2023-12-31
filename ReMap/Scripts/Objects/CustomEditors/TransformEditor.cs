using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform), true), CanEditMultipleObjects]
public class TransformEditor : Editor
{
    GameObject SelectedObject = null;//
    private static readonly Type transformInspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TransformInspector");
    private UnityEditor.Editor transformEditor = null;

    void OnEnable()
    {
        CustomEditorStyle.OnEnable();
        transformEditor = CreateEditor(serializedObject.targetObjects, transformInspectorType);
    }

    public override void OnInspectorGUI()
    {
        if (transformEditor == null)
            return;

        SelectedObject = null;
        if (Selection.activeTransform && Selection.gameObjects.Length == 1)
            SelectedObject = Selection.activeTransform.gameObject;

        EditorGUILayout.LabelField(" Unity Transforms:", CustomEditorStyle.LabelStyle);
        EditorGUILayout.Space(5);
        transformEditor.OnInspectorGUI();

        if (SelectedObject)
        {
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField(" Apex Info:", CustomEditorStyle.LabelStyle);
            EditorGUILayout.Space(5);

            string[] splitArray = SelectedObject.name.Split(char.Parse(" "));
            string model = "$\"" + splitArray[0].Replace("#", "/") + ".rmdl" + "\"";
            if (!splitArray[0].Contains("mdl#"))
                model = "(Not a model)";

            //Asset
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Asset:");
            GUILayout.TextField(model, GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = model;
            GUILayout.EndHorizontal();


            //Origin
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Origin:");
            GUILayout.TextField(BuildOrigin(SelectedObject), GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = BuildOrigin(SelectedObject);
            GUILayout.EndHorizontal();

            //Angles
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Angle:");
            GUILayout.TextField(BuildAngles(SelectedObject), GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = BuildAngles(SelectedObject);
            GUILayout.EndHorizontal();

            float w = 0;
            float d = 0;
            float h = 0;

            foreach (Renderer o in SelectedObject.GetComponentsInChildren<Renderer>())
            {
                if (o.bounds.size.z > w)
                    w = o.bounds.size.z;

                if (o.bounds.size.x > d)
                    d = o.bounds.size.x;

                if (o.bounds.size.y > h)
                    h = o.bounds.size.y;
            }

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Size (Unity Vector):");
            GUILayout.TextField("X: " + d.ToString().Replace(",", ".") + "   Y: " + h.ToString().Replace(",", ".") + "   Z: " + w.ToString().Replace(",", "."), GUILayout.Width(267));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Size (Apex Vector):");
            GUILayout.TextField("X: " + w.ToString().Replace(",", ".") + "   Y: " + d.ToString().Replace(",", ".") + "   Z: " + h.ToString().Replace(",", "."), GUILayout.Width(267));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Scale:");
            GUILayout.TextField(SelectedObject.transform.localScale.x.ToString().Replace(",", "."), GUILayout.Width(267));
            GUILayout.EndHorizontal();
        }
    }

    public static string BuildOrigin(GameObject go)
    {
        Vector3 vec = go.transform.position;
        float x = -vec.z;
        float y = vec.x;
        float z = vec.y;

        return $"< {x}, {y}, {z} >";
    }

    public static string BuildAngles(GameObject go)
    {
        Vector3 vec = go.transform.rotation.eulerAngles;

        float x = -WrapAngle(vec.x);
        float y = -WrapAngle(vec.y);
        float z = WrapAngle(vec.z);

        return $"< {x}, {y}, {z} >";
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360;

        if (angle > 180) return angle - 360;

        return angle;
    }
}
