using UnityEngine;
using UnityEditor;

public class DrawVerticalZipline : MonoBehaviour
{
    [Header("Developer options, do not modify:")]
    public bool ShowDevelopersOptions = false;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform zipline;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform fence_post;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform arm;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform rope_start;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform rope_end;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform helperPlacement;

    [Header("Unity Settings:")]
    public bool ShowZipline = true;
    [ConditionalHide("ShowZipline", true)] public float ShowZiplineDistance = 8000;
    [ConditionalHide("ShowZipline", true)] public bool ShowAutoDetachDistance = true;
    public bool EnableAutoOffsetDistance = false;

    [Header("Zipline Parameters:")]
    [ConditionalHide("ShowArmOffset", true)] public float ArmOffset = 180;
    public float HeightOffset = 0;
    [ConditionalHide("EnableAutoOffsetDistance", true)] public float GroundOffset = 0;
    [ConditionalHide("PushOffInDirectionX", true)] public float anglesOffset = 0;
    public float FadeDistance = -1;
    public float Scale = 1;
    public float Width = 2;
    public float SpeedScale = 1;
    public float LengthScale = 1;
    public bool PreserveVelocity  = false;
    public bool DropToBottom = true;
    public float AutoDetachStart = 50;
    public float AutoDetachEnd = 25;
    public bool RestPoint = false;
    public bool PushOffInDirectionX = true;
    public bool IsMoving = false;
    public bool DetachEndOnSpawn = false;
    public bool DetachEndOnUse = false;

    [Header("Panels:")]
    public GameObject[] Panels;
    public float PanelTimerMin = 32;
    public float PanelTimerMax = 60;
    public int PanelMaxUse = 0;

    // If true show the param
    [HideInInspector] public bool ShowArmOffset = false;


    void OnDrawGizmos()
    {
        // Origin && Angles
        // Start
        GameObject support_start = zipline.transform.Find("support_start").gameObject;
        GameObject support_end = zipline.transform.Find("support_end").gameObject;

        support_start.transform.position = zipline.position;
        support_start.transform.eulerAngles = zipline.eulerAngles;

        if(helperPlacement != null) zipline.position = helperPlacement.position;

        if( fence_post != null )
        {
            ShowArmOffset = true;

            fence_post.transform.SetParent(support_start.transform);
            fence_post.transform.localPosition = new Vector3(0, 0, 0);
            fence_post.transform.localEulerAngles = new Vector3(0, 0, 0);
            arm.transform.SetParent(support_start.transform);
            arm.transform.localPosition = new Vector3((float)0.8, ArmOffset, 1);
            arm.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_start.SetParent(arm.transform);
            rope_start.localPosition = new Vector3(55, -12, 4);

            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            rope_start.rotation = Quaternion.Euler(targetRotation.x, anglesOffset, targetRotation.z);
            //rope_start.localEulerAngles = new Vector3(0, anglesOffset, 0); // old angle system

            if(ArmOffset < 46) ArmOffset = 46;
            if(ArmOffset > 300) ArmOffset = 300;
        }

        if( fence_post == null && arm != null )
        {
            arm.transform.SetParent(support_start.transform);
            arm.transform.localPosition = new Vector3(0, 0, 0);
            arm.transform.localEulerAngles = new Vector3(0, 90, 0);
            rope_start.SetParent(arm.transform);
            rope_start.localPosition = new Vector3(55, -12, 4);
            rope_start.localEulerAngles = new Vector3(0, anglesOffset, 0);
            ArmOffset = 0;
        }

        if( arm == null )
        {
            rope_start.SetParent(support_start.transform);
            rope_start.localPosition = new Vector3(0, 0, 0);
            rope_start.localEulerAngles = new Vector3(0, anglesOffset, 0);
            ArmOffset = 0;
        }

        // End
        support_end.transform.position = new Vector3( rope_start.position.x, rope_start.position.y + HeightOffset, rope_start.position.z );
        support_end.transform.eulerAngles = rope_start.eulerAngles;

        anglesOffset = anglesOffset % 360;

        // Show / Hide: Arrow
        GameObject arrow = rope_start.transform.Find("arrow").gameObject;
        arrow.SetActive( PushOffInDirectionX );

        foreach ( GameObject go in Panels)
        {
            if ( go == null ) continue;

            if ( go.name.Contains( "mdl#" ) )
            {
                go.name = go.name.Split( char.Parse( " " ) )[0].Replace( "mdl#", "" );
            }
        }

        if ( EnableAutoOffsetDistance )
        {
            RaycastHit hit;
            if (Physics.Raycast(rope_start.transform.position - new Vector3(0, 1, 0), Vector3.down, out hit, 20000))
            {
                HeightOffset = -(hit.distance - GroundOffset);
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
            var thickness = 3;
            float autoDetachLenght = AutoDetachStart + AutoDetachEnd;

            if (rope_start != null && rope_end != null && Vector3.Distance(startPos, endPos) > 10)
            {
                // Draws a line from this transform to the target
                Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.yellow, null, thickness);

                if(ShowAutoDetachDistance && Vector3.Distance(startPos, endPos) > autoDetachLenght)
                {
                    // Draw Start Auto Detach
                    var startDir = rope_end.position - rope_start.position;
                    var startDistance = startDir.magnitude;
                    var startDirection = startDir / startDistance;
                    if(AutoDetachStart < 0) AutoDetachStart = 0;

                    if(AutoDetachStart != 0) Handles.DrawBezier(startPos, startPos + startDirection * AutoDetachStart, startPos, startPos + startDirection * AutoDetachStart, Color.red, null, thickness);

                    // Draw End Auto Detach
                    var endDir = rope_start.position - rope_end.position;
                    var endDistance = endDir.magnitude;
                    var endDirection = endDir / endDistance;
                    if(AutoDetachEnd < 0) AutoDetachEnd = 0;

                    if(AutoDetachEnd != 0) Handles.DrawBezier(endPos, endPos + endDirection * AutoDetachEnd, endPos, endPos + endDirection * AutoDetachEnd, Color.red, null, thickness);
                }
            }
        }
    }
}
