using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool( "Drop to Ground...", typeof(Transform) )]
internal class DropToGroundTool : EditorTool
{
    private GUIContent someContent;

    public override GUIContent toolbarIcon => someContent;

    private void OnEnable()
    {
        someContent = new GUIContent
        {
            image = ( Texture )Resources.Load( "Icons/dropToGround" ),
            tooltip = "Drop to Ground..."
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
            foreach ( var go in Selection.gameObjects )
            {
                RaycastHit hit;
                if ( Physics.Raycast( go.transform.position, Vector3.down, out hit, 20000 ) )
                {
                    go.transform.position = hit.point;

                    var rotation = Quaternion.FromToRotation( Vector3.up, hit.normal );
                    rotation *= go.transform.rotation;
                    go.transform.rotation = rotation;
                }
            }
    }

    public override void OnToolGUI( EditorWindow window )
    {
        Tools.current = Tool.Move;
    }
}