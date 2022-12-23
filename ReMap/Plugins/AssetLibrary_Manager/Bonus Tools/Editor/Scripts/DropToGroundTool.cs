using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Drop to Ground...", typeof(Transform))]
class DropToGroundTool : EditorTool
{
    GUIContent someContent;

    void OnEnable()
    {
        someContent = new GUIContent()
        {
            image = (Texture)Resources.Load("Icons/dropToGround"),
            tooltip = "Drop to Ground..."
        };

        ToolManager.activeToolChanged += OnActiveToolDidChange;
    }

    void OnDisable()
    {
        ToolManager.activeToolChanged -= OnActiveToolDidChange;
    }

    public override GUIContent toolbarIcon
    {
        get { return someContent; }
    }

    void OnActiveToolDidChange()
    {
        if (ToolManager.IsActiveTool(this))
        {
            Undo.RecordObjects(Selection.transforms, "Drop Objects");

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                GameObject go = Selection.transforms[i].gameObject;

                Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
                var combinedBounds = renderers[0].bounds;

                foreach (Renderer render in renderers)
                {
                    combinedBounds.Encapsulate(render.bounds);
                }

                float diff = go.transform.position.y - combinedBounds.center.y;

                go.transform.position = new Vector3(go.transform.position.x, combinedBounds.extents.y + diff, go.transform.position.z);
            }    
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        Tools.current = Tool.Move;   
    }
}