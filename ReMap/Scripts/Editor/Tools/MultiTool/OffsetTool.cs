using UnityEditor;
using UnityEngine;
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
            CreateVector3Field( ref offset, "Offset:", "", 120, 0, 20 );
            GUILayout.EndHorizontal();

            CreateButton( "Apply Offset", "", () => ApplyOffset(), 0, 20 );
            GUILayout.EndVertical();
        }

        private static void ApplyOffset()
        {
            foreach ( var go in Selection.gameObjects )
                go.transform.position = go.transform.position + offset;
        }
    }
}