using UnityEngine;
using UnityEditor;

public class SoundScript : MonoBehaviour
{

    [Header("Developer options, do not modify:")]
    public bool ShowDevelopersOptions = false;
    [ConditionalHide("ShowDevelopersOptions", true)] public Transform soundModel;

    [Header("Sound Parameters:")]
    public float radius = 0;
    public bool isWaveAmbient = false;
    public bool enable = true;
    public string soundName = "";
    public Vector3[] polylineSegment;
    [HideInInspector] public Vector3[] polylineSegmentTransformed;


    void OnDrawGizmos()
    {
        var startPos = soundModel.position;
        var thickness = 3;

        GameObject go = soundModel.gameObject;

        // Remove "mdl#" so that the props script does not find it
        if (go.name.Contains("mdl#")) go.name = go.name.Split(char.Parse(" "))[0].Replace("mdl#", "");

        // Transforms the vectors to follow the entity
        polylineSegmentTransformed = new Vector3[polylineSegment.Length];

        for (int i = 0; i < polylineSegment.Length; i++)
        {
            polylineSegmentTransformed[i] = transform.TransformPoint(polylineSegment[i]);
        }


        // Draw all polyline segments
        for ( int i = 0 ; i < polylineSegment.Length ; i++ )
        {
            if ( i == 0 )
                Handles.DrawBezier(startPos, polylineSegmentTransformed[i], startPos, polylineSegmentTransformed[i], Color.green, null, thickness);
            else
                Handles.DrawBezier(polylineSegmentTransformed[i], polylineSegmentTransformed[i-1], polylineSegmentTransformed[i], polylineSegmentTransformed[i-1], Color.green, null, thickness);
        }

    }

}
