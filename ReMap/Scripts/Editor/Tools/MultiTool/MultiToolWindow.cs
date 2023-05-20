
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
            SelectionTool
        }
        private static MultiToolWindow windowInstance;
        private static ToolType toolTypeSelection = ToolType.DistanceMeter;

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
                
            ShowTool( toolTypeSelection );
        }

        private static void MenuSelector()
        {
            GUILayout.BeginHorizontal();
                CreateButton( "Distance Meter", "", () => ChangeToolType( ToolType.DistanceMeter ), 120, 20 );
                CreateButton( "Selection", "", () => ChangeToolType( ToolType.SelectionTool ), 120, 20 );
                GetEditorWindowSize( windowInstance );
            GUILayout.EndHorizontal();
        }

        private static void ShowTool( ToolType toolType )
        {
            switch ( toolTypeSelection )
            {
                case ToolType.DistanceMeter: DistanceMeter.OnGUI(); break;
                case ToolType.SelectionTool: SelectionTool.OnGUI(); break;
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
                    break;

                case ToolType.SelectionTool:
                    //windowInstance.minSize = new Vector2( 600, 240 );
                    //windowInstance.maxSize = new Vector2( 600, 240 );
                break;
            }
        }
    }
}
