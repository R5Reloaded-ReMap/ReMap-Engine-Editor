using UnityEngine;
using UnityEditor;

public class DrawVerticalZipline : MonoBehaviour
{
    [Header("Dont change start and end transforms:")]
    public Transform zipline;
    public Transform rope_start;
    public Transform rope_end;

    [Header("Settings:")]
    public bool ShowZipline = true;
    public float ShowZiplineDistance = 8000;
    public bool ShowAutoDetachDistance = true;

    [Header("Zipline Parameters:")]
    public Vector3 ziplinePosition;
    public Vector3 ziplineAngles;
    public float heightOffset = 0;
    public float anglesOffset = 0;
    public float fadeDistance = -1;
    public float scale = 1;
    public float width = 2;
    public float speedScale = 1;
    public float lengthScale = 1;
    public bool preserveVelocity = false;
    public bool disableDropToBottom = true;
    public float autoDetachStart = 100;
    public float autoDetachEnd = 50;
    public bool restPoint = false;
    public bool pushOffInDirectionX = true;
    public bool isMoving = false;


    void OnDrawGizmos()
    {
        // Origin && Angles
        rope_end.position = new Vector3( rope_start.position.x, heightOffset, rope_start.position.z );
        rope_start.eulerAngles = new Vector3( 0, anglesOffset, 0 );
        rope_end.eulerAngles = rope_start.eulerAngles;
        zipline.position = ziplinePosition;
        zipline.eulerAngles = ziplineAngles;
        anglesOffset = anglesOffset % 360;

        // Show / Hide: Arrow
        GameObject arrow = rope_start.transform.Find("arrow").gameObject;
        arrow.SetActive(true);
        if(!pushOffInDirectionX) arrow.SetActive(false);

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
