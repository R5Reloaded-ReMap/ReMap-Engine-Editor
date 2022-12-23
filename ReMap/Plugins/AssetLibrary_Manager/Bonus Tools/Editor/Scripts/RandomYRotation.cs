using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Random Y Rotation...", typeof(Transform))]
class RandomYRotation : EditorTool
{
    GUIContent someContent;

    void OnEnable()
    {
        someContent = new GUIContent()
        {
            image = (Texture)Resources.Load("Icons/rotation"),
            tooltip = "Random Y Rotation..."
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
            Undo.RegisterCompleteObjectUndo(Selection.transforms, "UndoRotation");

            foreach (GameObject item in Selection.gameObjects)
            {
                item.transform.Rotate(Vector3.up * Random.Range(0, 360));
            }       
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        Tools.current = Tool.Move;   
    }
}