using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JumpTowerScript : MonoBehaviour
{
    [ HideInInspector ] public Transform ballon_base;
    [ HideInInspector ] public Transform ballon_top;

    [ Header( "Unity Settings:" ) ]
    public bool ShowZipline = true;
    [ ConditionalHide( "ShowZipline", true ) ] public float ShowZiplineDistance = 8000;

    [ Header( "Jump Tower Parameters:" ) ]
    public float Height = 2000;
    
    void OnDrawGizmos()
    {
        if ( ballon_base == null || ballon_top == null ) return;

        ballon_base.position = this.transform.position;
        ballon_top.position = this.transform.position + new Vector3( 2.75f, Height, 2 );

        if ( !ShowZipline ) return;

        float dist = Vector3.Distance( SceneView.currentDrawingSceneView.camera.transform.position, this.transform.position );
        float dist2 = Vector3.Distance( SceneView.currentDrawingSceneView.camera.transform.position, this.transform.position );
        if( dist < ShowZiplineDistance || dist2 < ShowZiplineDistance )
        {
            var startPos = ballon_base.position + new Vector3( 2.75f, 64, 2 );
            var endPos = ballon_top.position;
            var thickness = 3;

            // Draws a line from this transform to the target
            Handles.DrawBezier( startPos, endPos, startPos, endPos, Color.yellow, null, thickness );
        }
    }
}