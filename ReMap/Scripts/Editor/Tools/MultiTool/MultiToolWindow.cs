
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
            ObjectInfo,
            ComponentTransfer,
            ModelPosition
        }
        internal protected static MultiToolWindow windowInstance;
        private static ToolType toolTypeSelection = ToolType.DistanceMeter;
        private static string toolInfo = "";

        public static void Init()
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
                    #if RMAPDEV
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
            GUILayout.BeginHorizontal();
                CreateButton( "Component Transfer Tool", "", () => ChangeToolType( ToolType.ComponentTransfer ), 194 );
                CreateButton( "Model Position Tool", "", () => ChangeToolType( ToolType.ModelPosition ), 194 );
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
                case ToolType.ComponentTransfer: ComponentTransfer.OnGUI(); break;
                case ToolType.ModelPosition: ModelPosition.OnGUI(); break;
            }
        }

        private static void ChangeToolType( ToolType toolType )
        {
            toolTypeSelection = toolType;

            if ( !Helper.IsValid( windowInstance ) ) windowInstance = ( MultiToolWindow ) EditorWindow.GetWindow( typeof( MultiToolWindow ), false, "Multi Tool");

            switch ( toolTypeSelection )
            {
                case ToolType.DistanceMeter:
                    windowInstance.minSize = new Vector2( 600, 186 );
                    windowInstance.maxSize = new Vector2( 600, 186 );
                    toolInfo = "Distance Meter Tool:";
                    break;

                case ToolType.SelectionTool:
                    int size = 30 * Mathf.CeilToInt( Helper.GetAllObjectType().Length / 3 );
                    windowInstance.minSize = new Vector2( 600, 130 + size );
                    windowInstance.maxSize = new Vector2( 600, 130 + size );
                    toolInfo = "Selection Tool:";
                    break;
                
                case ToolType.OffsetTool:
                    windowInstance.minSize = new Vector2( 600, 152 );
                    windowInstance.maxSize = new Vector2( 600, 152 );
                    toolInfo = "Offset Tool:";
                    break;

                case ToolType.ModelSwap:
                    windowInstance.minSize = new Vector2( 600, 296 );
                    windowInstance.maxSize = new Vector2( 600, 296 );
                    toolInfo = "Model Swap Tool:";
                    break;
                
                case ToolType.Serialize:
                    SerializeTool.ChangeWindowSize();
                    toolInfo = "Serialize Tool:";
                    break;

                case ToolType.ObjectInfo:
                    windowInstance.minSize = new Vector2( 600, 450 );
                    windowInstance.maxSize = new Vector2( 600, 450 );
                    toolInfo = "Object Info:";
                    break;

                case ToolType.ComponentTransfer:
                    windowInstance.minSize = new Vector2( 600, 360 );
                    windowInstance.maxSize = new Vector2( 600, 360 );
                    toolInfo = "Component Transfer Tool:";
                    break;

                case ToolType.ModelPosition:
                    windowInstance.minSize = new Vector2( 600, 200 );
                    windowInstance.maxSize = new Vector2( 600, 200 );
                    toolInfo = "Model Position Tool:";
                break;
            }
        }
    }
}
