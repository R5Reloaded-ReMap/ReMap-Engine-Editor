using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Random Scale...", typeof(Transform))]
class RandomScale : EditorTool
{
    GUIContent someContent;

    void OnEnable()
    {
        someContent = new GUIContent()
        {
            image = (Texture)Resources.Load("Icons/scale"),
            tooltip = "Random Scale..."
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
            Undo.RegisterCompleteObjectUndo(Selection.transforms, "UndoScale");

            foreach (GameObject item in Selection.gameObjects)
            {
                float randomScale = Random.Range(0.9f, 1.1f);


                Vector3 itemScale = item.transform.localScale;
             
                itemScale = new Vector3(itemScale.x * randomScale, itemScale.y * randomScale, itemScale.z * randomScale);

                item.transform.localScale = itemScale;

            }        
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        Tools.current = Tool.Move;   
    }
}