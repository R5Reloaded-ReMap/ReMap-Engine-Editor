using UnityEditor;
using UnityEngine;

public class GridTool : EditorWindow
{
    private float gridX = 10.0f;
    private float gridY = 10.0f;
    private bool isfloor = true;

    private float rotx;
    private float roty;
    private float rotz;
    private Object source;
    private float spacing = 256f;

    public static void Init()
    {
        var window = ( GridTool )GetWindow( typeof(GridTool), false, "Grid Tool" );
        window.minSize = new Vector2( 300, 290 );
        window.maxSize = new Vector2( 300, 290 );
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical( "box" );
        GUILayout.Label( "Prefab to use:" );
        source = EditorGUILayout.ObjectField( source, typeof(Object), true );
        GUILayout.EndVertical();

        GUILayout.BeginVertical( "box" );
        GUILayout.Label( "Checked: Floor / Unchecked: Wall" );
        isfloor = EditorGUILayout.Toggle( "", isfloor );
        GUILayout.EndVertical();

        GUILayout.BeginVertical( "box" );
        GUILayout.Label( "Rotation:" );
        rotx = EditorGUILayout.FloatField( "X:", rotx );
        roty = EditorGUILayout.FloatField( "Y:", roty );
        rotz = EditorGUILayout.FloatField( "Z:", rotz );
        GUILayout.EndVertical();

        GUILayout.BeginVertical( "box" );
        GUILayout.Label( "Grid Settings:" );
        gridX = EditorGUILayout.FloatField( "Grid X:", gridX );
        gridY = EditorGUILayout.FloatField( "Grid Y:", gridY );
        spacing = EditorGUILayout.FloatField( "Spacing:", spacing );
        GUILayout.EndVertical();

        if ( GUILayout.Button( "Create Grid" ) )
            CreatePropGrid();
    }

    /// <summary>
    ///     Creates Gird With Given Settings
    /// </summary>
    private void CreatePropGrid()
    {
        if ( source == null )
            return;

        if ( !source.name.Contains( "mdl#" ) )
            return;

        string name = "Grid";

        var objToSpawn = new GameObject( name );
        objToSpawn.name = name;

        var parent = objToSpawn;

        for ( int y = 0; y < gridY; y++ )
        for ( int x = 0; x < gridX; x++ )
        {
            var pos = new Vector3( x, 0, y ) * spacing;
            if ( !isfloor )
                pos = new Vector3( x, y, 0 ) * spacing;

            var obj = PrefabUtility.InstantiatePrefab( source as GameObject ) as GameObject;
            obj.transform.position = pos;
            obj.transform.eulerAngles = new Vector3( rotx, roty, rotz );

            if ( parent != null ) //If its not null set it as a child
                obj.gameObject.transform.parent = parent.transform;
        }

        ReMapConsole.Log( "[Grid Tool] Created " + gridX + "X" + gridY + " Grid", ReMapConsole.LogType.Info );
    }
}