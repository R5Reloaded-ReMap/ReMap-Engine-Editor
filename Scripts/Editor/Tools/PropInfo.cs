using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropInfo : EditorWindow
{
    GameObject SelectedObject = null;

    [MenuItem("R5Reloaded/Tools/Prop Info", false, 200)]
    static void Init()
    {
        PropInfo window = (PropInfo)EditorWindow.GetWindow(typeof(PropInfo), false, "Prop Info");
        window.Show();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        if(SelectedObject) {
            string[] splitArray = SelectedObject.name.Split(char.Parse(" "));
            string finished = splitArray[0].Replace("#", "/") + ".rmdl";

            //Asset
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Asset:");
            GUILayout.TextField("$\"" + finished + "\"", GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = "$\"" + finished + "\"";
            GUILayout.EndHorizontal();


            //Origin
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Origin:");
            GUILayout.TextField(Helper.BuildOrigin(SelectedObject), GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = Helper.BuildOrigin(SelectedObject);
            GUILayout.EndHorizontal();

            //Angles
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Angle:");
            GUILayout.TextField(Helper.BuildAngles(SelectedObject), GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = Helper.BuildAngles(SelectedObject);
            GUILayout.EndHorizontal();

            float w = 0;
            float d = 0;
            float h = 0;

            foreach(Renderer o in SelectedObject.GetComponentsInChildren<Renderer>()) {
                if(o.bounds.size.z > w)
                    w = o.bounds.size.z;

                if(o.bounds.size.x > d)
                    d = o.bounds.size.x;

                if(o.bounds.size.y > h)
                    h = o.bounds.size.y;
            }

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Size:");
            GUILayout.TextField("W: " + w.ToString().Replace(",", ".") + "   D: " + d.ToString().Replace(",", ".") + "   H: " + h.ToString().Replace(",", "."), GUILayout.Width(267));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Scale:");
            GUILayout.TextField(SelectedObject.transform.localScale.x.ToString().Replace(",", "."), GUILayout.Width(267));
            GUILayout.EndHorizontal();
        }
        else
        {
            //Asset
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Asset:");
            GUILayout.TextField("$\"\"", GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = "";
            GUILayout.EndHorizontal();

            //Origin
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Origin:");
            GUILayout.TextField("<0,0,0>", GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = "<0,0,0>";
            GUILayout.EndHorizontal();

            //Angles
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Angle:");
            GUILayout.TextField("<0,0,0>", GUILayout.Width(200));
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
                GUIUtility.systemCopyBuffer = "<0,0,0>";
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Size:");
            GUILayout.TextField("W: 0   D: 0   H: 0", GUILayout.Width(267));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Scale:");
            GUILayout.TextField("0", GUILayout.Width(267));
            GUILayout.EndHorizontal();
        }
    }

    void Update()
    {
        if(Selection.activeTransform)
            SelectedObject = Selection.activeTransform.gameObject;
        else
            SelectedObject = null;
    }
}