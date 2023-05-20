
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;

namespace MultiTool
{
    internal class OffsetTool
    {
        private static Vector3 offset;
        internal static void OnGUI()
        {
            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    CreateVector3Field( ref offset, "Offset:", "", 96, 300, 20 );
                GUILayout.EndHorizontal();
 
                CreateButton( "Apply Offset", "", () => ApplyOffset(), 400, 20 );
            GUILayout.EndVertical();
        }

        private static void ApplyOffset()
        {
            foreach ( GameObject go in Selection.gameObjects )
            {
                go.transform.position = go.transform.position + offset;
            }
        }
    }
}
