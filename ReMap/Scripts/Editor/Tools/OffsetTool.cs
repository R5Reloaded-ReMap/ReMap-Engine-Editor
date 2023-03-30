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
        window.minSize = new Vector2( 300, 75 );
        window.maxSize = new Vector2( 300, 75 );
        window.Show();
    }

    /// <summary>
    /// OffsetTool.OnGUI() : When the window is displayed
    /// </summary>
    void OnGUI()
    {
        GUILayout.BeginVertical("box");
            offset = EditorGUILayout.Vector3Field( "Offset: ", offset );
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
            if ( GUILayout.Button( "Apply Offset" ) )
            {
                foreach ( GameObject go in Selection.gameObjects )
                {
                    go.transform.position = go.transform.position + offset;
                }
            }
        GUILayout.EndVertical();
    }
}
