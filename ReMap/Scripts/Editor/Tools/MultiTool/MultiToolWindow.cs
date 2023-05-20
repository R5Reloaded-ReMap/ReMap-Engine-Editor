
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;

namespace MultiTool
{
    public class MultiToolWindow : EditorWindow
    {
        private enum ToolType
        {
            DistanceMeter,
            SelectionTool,
            OffsetTool
        }
        private static MultiToolWindow windowInstance;
        private static ToolType toolTypeSelection = ToolType.DistanceMeter;
        private static string toolInfo = "";

        [ MenuItem( "ReMap/Tools/Multi Tool", false, 0 ) ]
        private static void Init()
        {
            windowInstance = ( MultiToolWindow ) EditorWindow.GetWindow( typeof( MultiToolWindow ), false, "Multi Tool");
            windowInstance.Show();

            ChangeToolType( toolTypeSelection );
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical( "box" );
                MenuSelector();
            GUILayout.EndVertical();

            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    CreateTextInfo( toolInfo );
                    #if ReMapDev
                        FlexibleSpace();
                        GetEditorWindowSize( windowInstance );
                    #endif
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
                
            ShowTool( toolTypeSelection );
        }

        private static void MenuSelector()
        {
            GUILayout.BeginHorizontal();
                CreateButton( "Distance Meter", "", () => ChangeToolType( ToolType.DistanceMeter ), 97, 20 );
                CreateButton( "Selection Tool", "", () => ChangeToolType( ToolType.SelectionTool ), 97, 20 );
                CreateButton( "Offset Tool", "", () => ChangeToolType( ToolType.OffsetTool ), 97, 20 );
            GUILayout.EndHorizontal();
        }

        private static void ShowTool( ToolType toolType )
        {
            switch ( toolTypeSelection )
            {
                case ToolType.DistanceMeter: DistanceMeter.OnGUI(); break;
                case ToolType.SelectionTool: SelectionTool.OnGUI(); break;
                case ToolType.OffsetTool: OffsetTool.OnGUI(); break;
            }
        }

        private static void ChangeToolType( ToolType toolType )
        {
            toolTypeSelection = toolType;

            if ( !Helper.IsValid( windowInstance ) ) windowInstance = ( MultiToolWindow ) EditorWindow.GetWindow( typeof( MultiToolWindow ), false, "Multi Tool");

            switch ( toolTypeSelection )
            {
                case ToolType.DistanceMeter:
                    //windowInstance.minSize = new Vector2( 540, 116 );
                    //windowInstance.maxSize = new Vector2( 540, 116 );
                    toolInfo = "Distance Meter Tool:";
                    break;

                case ToolType.SelectionTool:
                    //windowInstance.minSize = new Vector2( 600, 240 );
                    //windowInstance.maxSize = new Vector2( 600, 240 );
                    toolInfo = "Selection Tool:";
                    break;
                
                case ToolType.OffsetTool:
                    //windowInstance.minSize = new Vector2( 600, 240 );
                    //windowInstance.maxSize = new Vector2( 600, 240 );
                    toolInfo = "Offset Tool:";
                    break;
            }
        }
    }
}
