using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool( "Random Scale...", typeof(Transform) )]
internal class RandomScale : EditorTool
{
    private GUIContent someContent;

    public override GUIContent toolbarIcon => someContent;

    private void OnEnable()
    {
        someContent = new GUIContent
        {
            image = ( Texture )Resources.Load( "Icons/scale" ),
            tooltip = "Random Scale..."
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
            Undo.RegisterCompleteObjectUndo( Selection.transforms, "UndoScale" );

            foreach ( var item in Selection.gameObjects )
            {
                float randomScale = Random.Range( 0.9f, 1.1f );


                var itemScale = item.transform.localScale;

                itemScale = new Vector3( itemScale.x * randomScale, itemScale.y * randomScale, itemScale.z * randomScale );

                item.transform.localScale = itemScale;
            }
        }
    }

    public override void OnToolGUI( EditorWindow window )
    {
        Tools.current = Tool.Move;
    }
}