using UnityEngine;
using UnityEditor;

public class DrawNonVerticalZipline : MonoBehaviour
{
    [Header("Dont change start and end transforms:")]
    public Transform zipline_start;
    public Transform zipline_end;
    public Transform rope_start;
    public Transform rope_end;

    [Header("Settings:")]
    public bool ShowZipline = true;
    public float ShowZiplineDistance = 8000;
    public bool ShowAutoDetachDistance = true;

    [Header("Zipline Parameters:")]
    public Vector3 ziplineStartPosition;
    public Vector3 ziplineStartAngles;
    public Vector3 ziplineEndPosition;
    public Vector3 ziplineEndAngles;
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
        zipline_start.position = ziplineStartPosition;
        zipline_start.eulerAngles = ziplineStartAngles;
        zipline_end.position = ziplineEndPosition;
        zipline_end.eulerAngles = ziplineEndAngles;

        if(!ShowZipline)
            return;

        float dist = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, rope_start.position);
        float dist2 = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, rope_end.position);
        if(dist < ShowZiplineDistance || dist2 < ShowZiplineDistance)
        {
            if (rope_start != null && rope_end != null)
            {
                // Draws a line from this transform to the target
                var startPos = rope_start.position;
                var endPos = rope_end.position;
                var thickness = 3;
            
                Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.yellow, null, thickness);
                //float ziplineDistance = Vector3.Distance(rope_start.position, rope_end.position);
                //float lengthScaleCalc = ziplineDistance * lengthScale;
                //Vector3 length = new Vector3( 0, ziplineDistance - lengthScaleCalc - 16, 0 );
                //if(lengthScale < 0.90) length = new Vector3( 0, 0, 0 );
                //Handles.DrawBezier(startPos, endPos, startPos + length, endPos + length, Color.yellow, null, thickness);

                if(ShowAutoDetachDistance)
                {
                    // Draw Start Auto Detach
                    var startDir = rope_end.position - rope_start.position;
                    var startDistance = startDir.magnitude;
                    var startDirection = startDir / startDistance;
                    if(autoDetachStart < 0) autoDetachStart = 0;

                    if(autoDetachStart != 0) Handles.DrawBezier(startPos, startPos + startDirection * autoDetachStart, startPos, startPos + startDirection * autoDetachStart, Color.red, null, thickness);

                    // Draw End Auto Detach
                    var endDir = rope_start.position - rope_end.position;
                    var endDistance = endDir.magnitude;
                    var endDirection = endDir / endDistance;
                    if(autoDetachEnd < 0) autoDetachEnd = 0;

                    if(autoDetachEnd != 0) Handles.DrawBezier(endPos, endPos + endDirection * autoDetachEnd, endPos, endPos + endDirection * autoDetachEnd, Color.red, null, thickness);
                }
            }
        }
    }
}
