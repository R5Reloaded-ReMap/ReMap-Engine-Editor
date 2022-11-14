using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapCords : EditorWindow
{
    GameObject SelectedObject = null;

    [MenuItem("R5Reloaded/Tools/Game Origin And Angle", false, 200)]
    static void Init()
    {
        MapCords window = (MapCords)EditorWindow.GetWindow(typeof(MapCords), false, "Game Origin And Angles");
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

            GUILayout.BeginHorizontal();
            GUILayout.Label("Asset:");
            GUILayout.TextField("$\"" + finished + "\"", GUILayout.Width(360));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Origin:");
            GUILayout.Label("X",GUILayout.ExpandWidth(false));
            GUILayout.TextField(GetOrgX(SelectedObject).ToString(), GUILayout.Width(100));
            GUILayout.Label("Y",GUILayout.ExpandWidth(false));
            GUILayout.TextField(GetOrgY(SelectedObject).ToString(), GUILayout.Width(100));
            GUILayout.Label("Z",GUILayout.ExpandWidth(false));
            GUILayout.TextField(GetOrgZ(SelectedObject).ToString(), GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle:");
            GUILayout.Label("X",GUILayout.ExpandWidth(false));
            GUILayout.TextField((-WrapAngle(SelectedObject.transform.eulerAngles.x)).ToString(), GUILayout.Width(100));
            GUILayout.Label("Y",GUILayout.ExpandWidth(false));
            GUILayout.TextField((-WrapAngle(SelectedObject.transform.eulerAngles.y)).ToString(), GUILayout.Width(100));
            GUILayout.Label("Z",GUILayout.ExpandWidth(false));
            GUILayout.TextField((WrapAngle(SelectedObject.transform.eulerAngles.z)).ToString(), GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scale:");
            GUILayout.TextField(SelectedObject.transform.localScale.x.ToString(), GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Asset:");
            GUILayout.TextField("-", GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Origin:");
            GUILayout.Label("X",GUILayout.ExpandWidth(false));
            GUILayout.TextField("-", GUILayout.Width(100));
            GUILayout.Label("Y",GUILayout.ExpandWidth(false));
            GUILayout.TextField("-", GUILayout.Width(100));
            GUILayout.Label("Z",GUILayout.ExpandWidth(false));
            GUILayout.TextField("-", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle:");
            GUILayout.Label("X",GUILayout.ExpandWidth(false));
            GUILayout.TextField("-", GUILayout.Width(100));
            GUILayout.Label("Y",GUILayout.ExpandWidth(false));
            GUILayout.TextField("-", GUILayout.Width(100));
            GUILayout.Label("Z",GUILayout.ExpandWidth(false));
            GUILayout.TextField("-", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scale:");
            GUILayout.TextField("-", GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }
    }

    private static float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }

    private static float GetOrgX(GameObject go)
    {
        float orgx = -go.transform.position.z;

        return orgx;
    }

    private static float GetOrgY(GameObject go)
    {
        float orgy = go.transform.position.x;

        return orgy;
    }

    private static float GetOrgZ(GameObject go)
    {
        float orgz = go.transform.position.y;

        return orgz;
    }

    void Update()
    {
        if(Selection.activeTransform)
            SelectedObject = Selection.activeTransform.gameObject;
        else
            SelectedObject = null;
    }
}