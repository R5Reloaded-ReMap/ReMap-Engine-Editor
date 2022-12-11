#define CONDITION_NAME

using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class DrawNonVerticalZipline : MonoBehaviour
{
    [Header("Developer options, do not modify:")]
    public bool ShowDevelopersOptions = false;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform zipline;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform fence_post_start;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform arm_start;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform fence_post_end;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform arm_end;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform rope_start;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform rope_end;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform helperPlacement_start;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform helperPlacement_end;

    [Header("Unity Settings:")]
    public bool ShowZipline = true;
    [ConditionalHide("ShowZipline", true)] public float ShowZiplineDistance = 8000;
    [ConditionalHide("ShowZipline", true)] public bool ShowAutoDetachDistance = true;

    [Header("Zipline Parameters:")]
    [ConditionalHide("ShowArmOffsetStart", true)] public float armOffsetStart = 160;
    [ConditionalHide("ShowArmOffsetEnd", true)] public float armOffsetEnd = 160;
    public float fadeDistance = -1;
    public float scale = 1;
    public float width = 2;
    public float speedScale = 1;
    public float lengthScale = 1;
    public bool preserveVelocity = false;
    public bool dropToBottom = false;
    public float autoDetachStart = 150;
    public float autoDetachEnd = 150;
    public bool restPoint = false;
    public bool pushOffInDirectionX = false;
    public bool isMoving = false;
    public bool detachEndOnSpawn = false;
    public bool detachEndOnUse = false;

    // If true show the param
    [HideInInspector] public bool ShowArmOffsetStart = false;
    [HideInInspector] public bool ShowArmOffsetEnd = false;


    void OnDrawGizmos()
    {
        // Origin && Angles
        // Start
        GameObject support_start = zipline.transform.Find("support_start").gameObject;
        GameObject support_end = zipline.transform.Find("support_end").gameObject;

        if (helperPlacement_start != null) support_start.transform.position = helperPlacement_start.position;
        if(helperPlacement_end != null) support_end.transform.position = helperPlacement_end.position;

        if( fence_post_start != null )
        {
            ShowArmOffsetStart = true;

            fence_post_start.transform.SetParent(support_start.transform);
            fence_post_start.transform.localPosition = new Vector3(0, 0, 0);
            fence_post_start.transform.localEulerAngles = new Vector3(0, 0, 0);
            arm_start.transform.SetParent(support_start.transform);
            arm_start.transform.localPosition = new Vector3((float)0.8, armOffsetStart, 1);
            arm_start.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_start.SetParent(arm_start.transform);
            rope_start.localPosition = new Vector3(55, -12, 4);

            if(armOffsetStart < 46) armOffsetStart = 46;
            if(armOffsetStart > 300) armOffsetStart = 300;
        }

        if( fence_post_start == null && arm_start != null )
        {
            arm_start.transform.SetParent(support_start.transform);
            arm_start.transform.localPosition = new Vector3(0, 0, 0);
            arm_start.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_start.SetParent(arm_start.transform);
            rope_start.localPosition = new Vector3(55, -12, 4);
            armOffsetStart = 0;
}

        if( arm_start == null )
        {
            rope_start.SetParent(support_start.transform);
            rope_start.localPosition = new Vector3(0, 0, 0);
            armOffsetStart = 0;
        }

        // End
        if( fence_post_end != null )
        {
            ShowArmOffsetEnd = true;

            fence_post_end.transform.SetParent(support_end.transform);
            fence_post_end.transform.localPosition = new Vector3(0, 0, 0);
            fence_post_end.transform.localEulerAngles = new Vector3(0, 0, 0);
            arm_end.transform.SetParent(support_end.transform);
            arm_end.transform.localPosition = new Vector3((float)0.8, armOffsetEnd, 1);
            arm_end.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_end.SetParent(arm_end.transform);
            rope_end.localPosition = new Vector3(55, -12, 4);

            if(armOffsetEnd < 46) armOffsetEnd = 46;
            if(armOffsetEnd > 300) armOffsetEnd = 300;
        }

        if( fence_post_end == null && arm_end != null )
        {
            arm_end.transform.SetParent(support_end.transform);
            arm_end.transform.localPosition = new Vector3(0, 0, 0);
            arm_end.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_end.SetParent(arm_end.transform);
            rope_end.localPosition = new Vector3(55, -12, 4);
            armOffsetEnd = 0;
        }

        if( arm_end == null )
        {
            rope_end.SetParent(support_end.transform);
            rope_end.localPosition = new Vector3(0, 0, 0);
            armOffsetEnd = 0;
        }


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

                if(ShowAutoDetachDistance && Vector3.Distance(startPos, endPos) > 200)
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
