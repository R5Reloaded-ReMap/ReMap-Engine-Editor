using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DrawZipline : MonoBehaviour
{
    public Transform zipline_start;
    public Transform zipline_end;

    public bool ShowZipline = true;
    public float ShowZiplineDistance = 8000;

    void OnDrawGizmos()
    {
        if(!ShowZipline)
            return;

        float dist = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, zipline_start.position);
        float dist2 = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, zipline_end.position);
        if(dist < ShowZiplineDistance || dist2 < ShowZiplineDistance)
        {
            if (zipline_start != null && zipline_end != null)
            {
                // Draws a line from this transform to the target
                var startPos = zipline_start.position;
                var endPos = zipline_end.position;
                var thickness = 3;
            
                Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.yellow, null, thickness);
            }
        }
    }
}