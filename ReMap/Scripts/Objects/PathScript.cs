using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathScript : MonoBehaviour
{
    [ Header( "Unity Settings:" ) ]
    public bool ShowPath = true;
    public float ShowPathDistance = 8000;

    [ Header( "Path Parameters:" ) ]
    public float SpeedTransition = 8;
    public float Fov = 120;
    public bool TrackTarget = false;

    [ HideInInspector ] public GameObject targetRef;
    
    void OnDrawGizmos()
    {
        if( !ShowPath )
            return;

        float dist = Vector3.Distance( SceneView.currentDrawingSceneView.camera.transform.position, gameObject.transform.position );
        
        if( dist > ShowPathDistance )
            return;
        
        List< GameObject > PathNode = new List< GameObject >();

        foreach ( Transform node in gameObject.transform )
        {
            if ( node.gameObject.name != "targetRef" )
            {
                PathNode.Add( node.gameObject );
            }
            else targetRef = node.gameObject;
        }

        for ( int i = 0; i < PathNode.Count; i++ )
        {
            if ( targetRef != null )
            {
                if ( TrackTarget ) PathNode[ i ].transform.LookAt( targetRef.transform );

                targetRef.SetActive( TrackTarget );
            }

            if ( i < PathNode.Count -1 )
            {
                var startPos = PathNode[ i ].transform.position;
                var endPos = PathNode[ i + 1 ].transform.position;
                var thickness = 3;

                if ( Vector3.Distance( startPos, endPos ) > 10 )
                {
                    Handles.DrawBezier( startPos, endPos, startPos, endPos, Color.yellow, null, thickness );
                }
            }
        }
    }
}