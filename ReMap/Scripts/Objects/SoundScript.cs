using UnityEngine;
using UnityEditor;

[AddComponentMenu("ReMap/Sound", 0)]
public class SoundScript : MonoBehaviour
{
    public Transform soundModel;
    public bool ShowPolylineSegments = true;
    public float ShowPolylineSegmentsDistance = 8000;
    public float Radius = 0;
    public bool IsWaveAmbient = false;
    public bool Enable = true;
    public string SoundName = "";
    public Vector3[] PolylineSegment;

    void OnDrawGizmos()
    {
        if (soundModel == null)
            return;

        var startPos = soundModel.position;
        var thickness = 3;

        GameObject go = soundModel.gameObject;

        soundModel.eulerAngles = new Vector3(0, 0, 0);

        // Remove "mdl#" so that the props script does not find it
        if (go.name.Contains("mdl#")) go.name = go.name.Split(char.Parse(" "))[0].Replace("mdl#", "");

        // Transforms the vectors to follow the entity
        Vector3[] polylineSegmentTransformed = new Vector3[PolylineSegment.Length];

        for (int i = 0; i < PolylineSegment.Length; i++)
        {
            polylineSegmentTransformed[i] = transform.TransformPoint(PolylineSegment[i]);
        }

        if (!ShowPolylineSegments)
            return;

        float dist = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, soundModel.position);
        float dist2 = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, soundModel.position);
        if (dist < ShowPolylineSegmentsDistance || dist2 < ShowPolylineSegmentsDistance)
        {
            // Draw all polyline segments
            for (int i = 0; i < PolylineSegment.Length; i++)
            {
                if (i == 0)
                {
                    if (startPos != polylineSegmentTransformed[i])
                        Handles.DrawBezier(startPos, polylineSegmentTransformed[i], startPos, polylineSegmentTransformed[i], Color.green, null, thickness);
                }
                else if (polylineSegmentTransformed[i] != polylineSegmentTransformed[i - 1])
                {
                    Handles.DrawBezier(polylineSegmentTransformed[i], polylineSegmentTransformed[i - 1], polylineSegmentTransformed[i], polylineSegmentTransformed[i - 1], Color.green, null, thickness);
                }
            }
        }
    }
}
