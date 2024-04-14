using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool( "Random Rotation...", typeof(Transform) )]
internal class RandomRotation : EditorTool
{
    private GUIContent someContent;

    public override GUIContent toolbarIcon => someContent;

    private void OnEnable()
    {
        someContent = new GUIContent
        {
            image = ( Texture )Resources.Load( "Icons/rotation" ),
            tooltip = "Random Rotation..."
        };

        ToolManager.activeToolChanged += OnActiveToolDidChange;
    }

    private void OnDisable()
    {
        ToolManager.activeToolChanged -= OnActiveToolDidChange;
    }

    private void OnActiveToolDidChange()
    {
        if ( ToolManager.IsActiveTool( this ) )
        {
            Undo.RegisterCompleteObjectUndo( Selection.transforms, "UndoRotation" );

            foreach ( var item in Selection.gameObjects )
                item.transform.Rotate( new Vector3( Random.Range( 0, 360 ), Random.Range( 0, 360 ), Random.Range( 0, 360 ) ) );
        }
    }

    public override void OnToolGUI( EditorWindow window )
    {
        Tools.current = Tool.Move;
    }
}