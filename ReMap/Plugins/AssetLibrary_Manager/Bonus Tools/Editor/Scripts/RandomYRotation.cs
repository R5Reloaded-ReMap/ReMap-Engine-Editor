using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool( "Random Y Rotation...", typeof(Transform) )]
internal class RandomYRotation : EditorTool
{
    private GUIContent someContent;

    public override GUIContent toolbarIcon => someContent;

    private void OnEnable()
    {
        someContent = new GUIContent
        {
            image = ( Texture )Resources.Load( "Icons/rotation" ),
            tooltip = "Random Y Rotation..."
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
                item.transform.Rotate( Vector3.up * Random.Range( 0, 360 ) );
        }
    }

    public override void OnToolGUI( EditorWindow window )
    {
        Tools.current = Tool.Move;
    }
}