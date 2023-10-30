using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DrawLinkedZipline : MonoBehaviour
{
    [Header("Settings:")]
    public bool ShowZipline = true;
    public bool ShowSmoothedZipline = true;
    public float ShowZiplineDistance = 8000;
    
    void OnDrawGizmos()
    {
        if(!ShowZipline)
            return;

        float dist = Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, gameObject.transform.position);
        if(dist > ShowZiplineDistance)
            return;
        
        List<GameObject> Children = new List<GameObject>();

        foreach (Transform child in gameObject.transform)
        {
            Children.Add(child.gameObject);
        }

        LinkedZiplineScript script = gameObject.GetComponent<LinkedZiplineScript>();
        List<Vector3> points = new List<Vector3>();

        int i = 0;
        foreach (Transform child in gameObject.transform)
        {
            if(i < Children.Count - 1)
            {
                var startPos = Children[i].transform.position;
                var endPos = Children[i + 1].transform.position;
                
                points.Add(startPos);

                if(i == Children.Count - 2)
                    points.Add(endPos);

                Handles.color = Color.yellow;
                Handles.DrawPolyLine(new Vector3[]{startPos, endPos});
            }

            i++;
        }

        if(!script.EnableSmoothing)
            return;

        if(script.SmoothType)
            GetAllPointsOnBezier(points.ToArray(), script.SmoothAmount);
        else
            GetBezierOfPath(points.ToArray(), script.SmoothAmount);
    }

    Vector3[] GetAllPointsOnBezier( Vector3[] points, int numSegments)
    {
        if(points.Length < 3)
            return points;

        List<Vector3> newPoints = new List<Vector3>();

        for ( int i = 0; i < numSegments; i++ )
        {
            float t = i / ( numSegments - 1.0f );
            newPoints.Add( GetSinglePointOnBezier( points, t ) );
        }
        
        if(ShowSmoothedZipline)
        {
            for ( int i = 0; i < newPoints.Count - 1; i++ )
            {
                Handles.DrawBezier(newPoints[i], newPoints[i + 1], newPoints[i], newPoints[i + 1], Color.blue, null, 3);
            }
        }

        return newPoints.ToArray();
    }

    Vector3 GetSinglePointOnBezier( Vector3[] points, float t )
    {
        // evaluate a point on a bezier-curve. t goes from 0 to 1.0

        Vector3[] lastPoints = points;
        for ( ;; )
        {
            List<Vector3> newPoints = new List<Vector3>();
            for ( int i = 0; i < lastPoints.Length - 1; i++ )
                newPoints.Add( lastPoints[i] + ( lastPoints[i+1] - lastPoints[i] ) * t );

            if ( newPoints.Count == 1 )
                return newPoints[0];

            lastPoints = newPoints.ToArray();
        }
    }

    void GetBezierOfPath( Vector3[] path, int numSegments )
    {
        if(path.Length < 3)
            return;

        for (int idx_cur = 0 ; idx_cur < path.Length - 2; idx_cur++ )
        {
            Vector3[] a = {
            path[ idx_cur ], 
            path[ idx_cur + 1 ],
            path[ idx_cur + 2 ]
            };
            Vector3[] bezierPoints = GetAllPointsOnBezier( a, numSegments );

            if(ShowSmoothedZipline)
                for ( int i = 0; i < bezierPoints.Length - 1; i++ )
                    Handles.DrawBezier(bezierPoints[i], bezierPoints[i + 1], bezierPoints[i], bezierPoints[i + 1], Color.blue, null, 3);
        }
    }
}