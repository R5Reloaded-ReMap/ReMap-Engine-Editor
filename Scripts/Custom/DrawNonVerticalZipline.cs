using UnityEngine;
using UnityEditor;

public class DrawNonVerticalZipline : MonoBehaviour
{
    [Header("Dont change start and end transforms:")]
    public Transform zipline_start;
    public Transform zipline_end;

    [Header("Settings:")]
    public bool ShowZipline = true;
    public float ShowZiplineDistance = 8000;

    [Header("Zipline Parameters:")]
    public float fadeDistance = -1;
    public float scale = 1;
    public float width = 2;
    public float speedScale = 1;
    public float lengthScale = 1;
    public bool preserveVelocity = false;
    public bool disableDropToBottom = true;
    public float autoDetachStart = 150;
    public float autoDetachEnd = 150;
    public bool restPoint = false;
    public bool pushOffInDirectionX = false;
    public bool isMoving = false;


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
