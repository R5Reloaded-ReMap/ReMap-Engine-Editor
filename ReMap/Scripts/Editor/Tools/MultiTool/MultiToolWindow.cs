
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
            OffsetTool,
            ModelSwap,
            Serialize,
            ObjectInfo
        }
        internal protected static MultiToolWindow windowInstance;
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
                CreateButton( "Distance Meter", "", () => ChangeToolType( ToolType.DistanceMeter ), 194 );
                CreateButton( "Selection Tool", "", () => ChangeToolType( ToolType.SelectionTool ), 194 );
                CreateButton( "Offset Tool", "", () => ChangeToolType( ToolType.OffsetTool ), 194 );
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
                CreateButton( "Model Swap Tool", "", () => ChangeToolType( ToolType.ModelSwap ), 194 );
                CreateButton( "Serialize Tool", "", () => ChangeToolType( ToolType.Serialize ), 194 );
                CreateButton( "Object Info", "", () => ChangeToolType( ToolType.ObjectInfo ), 194 );
            GUILayout.EndHorizontal();
        }

        private static void ShowTool( ToolType toolType )
        {
            switch ( toolTypeSelection )
            {
                case ToolType.DistanceMeter: DistanceMeter.OnGUI(); break;
                case ToolType.SelectionTool: SelectionTool.OnGUI(); break;
                case ToolType.OffsetTool: OffsetTool.OnGUI(); break;
                case ToolType.ModelSwap: ModelSwap.OnGUI(); break;
                case ToolType.Serialize: SerializeTool.OnGUI(); break;
                case ToolType.ObjectInfo: ObjectInfo.OnGUI(); break;
            }
        }

        private static void ChangeToolType( ToolType toolType )
        {
            toolTypeSelection = toolType;

            if ( !Helper.IsValid( windowInstance ) ) windowInstance = ( MultiToolWindow ) EditorWindow.GetWindow( typeof( MultiToolWindow ), false, "Multi Tool");

            switch ( toolTypeSelection )
            {
                case ToolType.DistanceMeter:
                    windowInstance.minSize = new Vector2( 600, 166 );
                    windowInstance.maxSize = new Vector2( 600, 166 );
                    toolInfo = "Distance Meter Tool:";
                    break;

                case ToolType.SelectionTool:
                    windowInstance.minSize = new Vector2( 600, 322 );
                    windowInstance.maxSize = new Vector2( 600, 322 );
                    toolInfo = "Selection Tool:";
                    break;
                
                case ToolType.OffsetTool:
                    windowInstance.minSize = new Vector2( 600, 132 );
                    windowInstance.maxSize = new Vector2( 600, 132 );
                    toolInfo = "Offset Tool:";
                    break;

                case ToolType.ModelSwap:
                    windowInstance.minSize = new Vector2( 600, 276 );
                    windowInstance.maxSize = new Vector2( 600, 276 );
                    toolInfo = "Model Swap Tool:";
                    break;
                
                case ToolType.Serialize:
                    SerializeTool.ChangeWindowSize();
                    toolInfo = "Serialize Tool:";
                    break;

                case ToolType.ObjectInfo:
                    windowInstance.minSize = new Vector2( 600, 430 );
                    windowInstance.maxSize = new Vector2( 600, 430 );
                    toolInfo = "Object Info:";
                break;
            }
        }
    }
}
