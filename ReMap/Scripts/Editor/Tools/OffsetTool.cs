using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Swap an existing model to an other
/// </summary>
public class OffsetTool : EditorWindow
{
    public Vector3 offset;

    /// <summary>
    /// OffsetTool.Init()
    /// </summary>
    [MenuItem("ReMap/Tools/Offset Tool", false, 100)]
    public static void Init()
    {
        OffsetTool window = ( OffsetTool )EditorWindow.GetWindow( typeof( OffsetTool ), false, "Offset Tool" );
        window.minSize = new Vector2( 300, 70 );
        window.maxSize = new Vector2( 300, 70 );
        window.Show();
    }

    /// <summary>
    /// OffsetTool.OnGUI() : When the window is displayed
    /// </summary>
    void OnGUI()
    {
        EditorGUILayout.Space( 2 );

        offset = EditorGUILayout.Vector3Field( "Offset: ", offset );

        if ( GUILayout.Button( "Apply Offset" ) )
        {
            foreach ( GameObject go in Selection.gameObjects )
            {
                go.transform.position = go.transform.position + offset;
            }
        }
    }
}
