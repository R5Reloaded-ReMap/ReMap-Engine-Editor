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
            foreach ( GameObject go in Selection.gameObjects )
            {
                RaycastHit hit;
                if (Physics.Raycast(go.transform.position, Vector3.down, out hit, 20000))
                {
                    go.transform.position = hit.point;

                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    rotation *= go.transform.rotation;
                    go.transform.rotation = rotation;
                }
            }
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        Tools.current = Tool.Move;   
    }
}