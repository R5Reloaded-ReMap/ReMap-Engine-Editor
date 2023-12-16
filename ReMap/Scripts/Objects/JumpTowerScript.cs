using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[AddComponentMenu("ReMap/Jump Tower", 0)]
public class JumpTowerScript : MonoBehaviour
{
    public Transform ballon_base;
    public Transform ballon_top;
    public bool ShowZipline = true;
    public float ShowZiplineDistance = 8000;
    public float Height = 2000;

    void OnDrawGizmos()
    {
        if (ballon_base == null || ballon_top == null) return;

        ballon_base.position = this.transform.position;
        ballon_top.position = this.transform.position + new Vector3(2.75f, Height, 2);

        if (!ShowZipline) return;

        float dist = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, this.transform.position);
        float dist2 = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, this.transform.position);
        if (dist < ShowZiplineDistance || dist2 < ShowZiplineDistance)
        {
            // Draws a line from this transform to the target
            Handles.color = Color.yellow;
            Handles.DrawPolyLine(new Vector3[] { ballon_base.position + new Vector3(2.75f, 64, 2), ballon_top.position });
        }
    }
}