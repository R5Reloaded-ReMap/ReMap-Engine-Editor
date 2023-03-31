using UnityEngine;
using UnityEditor;

public class DrawZipline : MonoBehaviour
{
    [Header("Dont change start and end transforms:")]
    public Transform zipline_start;
    public Transform zipline_end;

    [Header("Settings:")]
    public bool showZipline = true;
    public float showZiplineDistance = 8000;

    void OnDrawGizmos()
    {
        if(!showZipline)
            return;

        float dist = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, zipline_start.position);
        float dist2 = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, zipline_end.position);
        if(dist < showZiplineDistance || dist2 < showZiplineDistance)
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