using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class DrawNonVerticalZipline : MonoBehaviour
{
    /*
    [Header("Developer options, do not modify:")]
    [ HideInInspector ] public bool ShowDevelopersOptions = false;
    */
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform zipline;
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform fence_post_start;
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform arm_start;
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform fence_post_end;
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform arm_end;
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform rope_start;
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform rope_end;
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform helperPlacement_start;
    /* [ConditionalHide("ShowDevelopersOptions", true)] */ [ HideInInspector ] public Transform helperPlacement_end;

    [Header("Unity Settings:")]
    public bool ShowZipline = true;
    [ConditionalHide("ShowZipline", true)] public float ShowZiplineDistance = 8000;
    [ConditionalHide("ShowZipline", true)] public bool ShowAutoDetachDistance = true;

    [Header("Zipline Parameters:")]
    [ConditionalHide("ShowArmOffsetStart", true)] public float ArmOffsetStart = 160;
    [ConditionalHide("ShowArmOffsetEnd", true)] public float ArmOffsetEnd = 160;
    public float FadeDistance = -1;
    public float Scale = 1;
    public float Width = 2;
    public float SpeedScale = 1;
    public float LengthScale = 1;
    public bool PreserveVelocity  = false;
    public bool DropToBottom = false;
    public float AutoDetachStart = 150;
    public float AutoDetachEnd = 150;
    public bool RestPoint = false;
    public bool PushOffInDirectionX = false;
    public bool IsMoving = false;
    public bool DetachEndOnSpawn = false;
    public bool DetachEndOnUse = false;

    [Header("Panels:")]
    public GameObject[] Panels;
    public float PanelTimerMin = 32;
    public float PanelTimerMax = 60;
    public int PanelMaxUse = 0;

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
            arm_start.transform.localPosition = new Vector3((float)0.8, ArmOffsetStart, 1);
            arm_start.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_start.SetParent(arm_start.transform);
            rope_start.localPosition = new Vector3(55, -12, 4);

            if(ArmOffsetStart < 46) ArmOffsetStart = 46;
            if(ArmOffsetStart > 300) ArmOffsetStart = 300;
        }

        if( fence_post_start == null && arm_start != null )
        {
            arm_start.transform.SetParent(support_start.transform);
            arm_start.transform.localPosition = new Vector3(0, 0, 0);
            arm_start.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_start.SetParent(arm_start.transform);
            rope_start.localPosition = new Vector3(55, -12, 4);
            ArmOffsetStart = 0;
}

        if( arm_start == null )
        {
            rope_start.SetParent(support_start.transform);
            rope_start.localPosition = new Vector3(0, 0, 0);
            ArmOffsetStart = 0;
        }

        // End
        if( fence_post_end != null )
        {
            ShowArmOffsetEnd = true;

            fence_post_end.transform.SetParent(support_end.transform);
            fence_post_end.transform.localPosition = new Vector3(0, 0, 0);
            fence_post_end.transform.localEulerAngles = new Vector3(0, 0, 0);
            arm_end.transform.SetParent(support_end.transform);
            arm_end.transform.localPosition = new Vector3((float)0.8, ArmOffsetEnd, 1);
            arm_end.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_end.SetParent(arm_end.transform);
            rope_end.localPosition = new Vector3(55, -12, 4);

            if(ArmOffsetEnd < 46) ArmOffsetEnd = 46;
            if(ArmOffsetEnd > 300) ArmOffsetEnd = 300;
        }

        if( fence_post_end == null && arm_end != null )
        {
            arm_end.transform.SetParent(support_end.transform);
            arm_end.transform.localPosition = new Vector3(0, 0, 0);
            arm_end.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_end.SetParent(arm_end.transform);
            rope_end.localPosition = new Vector3(55, -12, 4);
            ArmOffsetEnd = 0;
        }

        if( arm_end == null )
        {
            rope_end.SetParent(support_end.transform);
            rope_end.localPosition = new Vector3(0, 0, 0);
            ArmOffsetEnd = 0;
        }

        foreach (GameObject go in Panels)
        {
            if ( go == null ) continue;

            if ( go.name.Contains( "mdl#" ) )
            {
                go.name = go.name.Split( char.Parse( " " ) )[0].Replace( "mdl#", "" );
            }
        }

        if (!ShowZipline)
            return;

        float dist = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, rope_start.position);
        float dist2 = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, rope_end.position);
        if(dist < ShowZiplineDistance || dist2 < ShowZiplineDistance)
        {
            var startPos = rope_start.position;
            var endPos = rope_end.position;
            float autoDetachLenght = AutoDetachStart + AutoDetachEnd;

            if (rope_start != null && rope_end != null && Vector3.Distance(startPos, endPos) > 10)
            {
                // Draws a line from this transform to the target
                Handles.color = Color.yellow;
                Handles.DrawPolyLine(new Vector3[]{startPos, endPos});

                if(ShowAutoDetachDistance && Vector3.Distance(startPos, endPos) > autoDetachLenght)
                {
                    // Draw Start Auto Detach
                    var startDir = rope_end.position - rope_start.position;
                    var startDistance = startDir.magnitude;
                    var startDirection = startDir / startDistance;
                    if(AutoDetachStart < 0) AutoDetachStart = 0;

                    if(AutoDetachStart != 0)
                    {
                        Handles.color = Color.red;
                        Handles.DrawPolyLine(new Vector3[]{startPos, startPos + startDirection * AutoDetachStart});
                    }

                    // Draw End Auto Detach
                    var endDir = rope_start.position - rope_end.position;
                    var endDistance = endDir.magnitude;
                    var endDirection = endDir / endDistance;
                    if(AutoDetachEnd < 0) AutoDetachEnd = 0;

                    if(AutoDetachEnd != 0)
                    {
                        Handles.color = Color.red;
                        Handles.DrawPolyLine(new Vector3[]{endPos, endPos + endDirection * AutoDetachEnd});
                    }
                }
            }
        }
    }
}
