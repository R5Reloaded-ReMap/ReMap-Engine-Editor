using UnityEditor;
using UnityEngine;

public class CustomEditorStyle : Editor
{
    public static GUIStyle LabelStyle;
    public static GUIStyle BoxStyle;
    public static void OnEnable()
    {
        SetLabelStyle();
        SetBoxStyle();
    }

    static void SetLabelStyle()
    {
        LabelStyle = new GUIStyle()
        {
            font = Resources.Load<Font>("CustomEditor/ApexMk2-Regular") as Font,
            fontStyle = FontStyle.Bold,
            fontSize = 15,
            normal = new GUIStyleState()
            {
                textColor = Color.white,
                background = Resources.Load<Texture2D>("CustomEditor/labelbackground") as Texture2D,
            },
            fixedHeight = 25,
            alignment = TextAnchor.MiddleLeft,

        };
    }

    static void SetBoxStyle()
    {
        BoxStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = Resources.Load<Texture2D>("CustomEditor/labelbackground") as Texture2D,
            },
        };
    }
}
