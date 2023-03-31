using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Random Rotation...", typeof(Transform))]
class RandomRotation : EditorTool
{
    GUIContent someContent;

    void OnEnable()
    {
        someContent = new GUIContent()
        {
            image = (Texture)Resources.Load("Icons/rotation"),
            tooltip = "Random Rotation..."
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
                item.transform.Rotate(new Vector3( Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360) ) );
            }       
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        Tools.current = Tool.Move;   
    }
}