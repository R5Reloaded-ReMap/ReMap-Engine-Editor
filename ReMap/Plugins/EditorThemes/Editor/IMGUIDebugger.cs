using System;
using UnityEditor;

public static class IMGUIDebugger
{
    private static readonly Type type = Type.GetType( "UnityEditor.GUIViewDebuggerWindow,UnityEditor" );

    [MenuItem( "Window/IMGUI Debugger" )]
    public static void Open()
    {
        EditorWindow.GetWindow( type ).Show();
    }
}