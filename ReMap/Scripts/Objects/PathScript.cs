using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathScript : MonoBehaviour
{
    [ Header( "Settings:" ) ]
    public bool ShowPath = true;
    public float ShowPathDistance = 8000;
    public float SpeedTransition = 8;
    public float Fov = 120;
    
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
            PathNode.Add( node.gameObject );
        }

        for ( int i = 0; i < PathNode.Count -1; i++ )
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