using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[AddComponentMenu("ReMap/Draw Zipline", 0)]
public class DrawZipline : MonoBehaviour
{
    public Transform zipline_start;
    public Transform zipline_end;
    public bool showZipline = true;
    public float showZiplineDistance = 15000;

    void OnDrawGizmos()
    {
        if (!showZipline)
            return;

        List<Vector3> drawpoints = new List<Vector3>();

        float dist = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, zipline_start.position);
        float dist2 = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, zipline_end.position);
        if (dist < showZiplineDistance || dist2 < showZiplineDistance)
        {
            if (zipline_start != null && zipline_end != null)
            {
                drawpoints.Add(zipline_start.position);
                drawpoints.Add(zipline_end.position);
            }
        }

        if (drawpoints.Count < 2)
            return;

        Handles.color = Color.yellow;
        Handles.DrawPolyLine(drawpoints.ToArray());
    }
}