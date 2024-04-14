using UnityEditor;
using UnityEngine;

namespace AssetLibraryManager
{
    public class Preview_Window : EditorWindow
    {
        private Editor gameObjectEditor;
        private GameObject selection;
        private int totalTris;

        private int totalVertices;

        public static void ShowWindow()
        {
            var window = GetWindow< Preview_Window >( "Preview Window" );
            window.minSize = new Vector2( 20, 20 );
        }

        private void CreateGUI()
        {
            selection = Selection.activeGameObject;

            gameObjectEditor = Editor.CreateEditor( selection );
        }

        private void OnGUI()
        {
            if ( selection != null )
                gameObjectEditor.OnInteractivePreviewGUI( GUILayoutUtility.GetRect( 0, 2048, 0, 2048 ), new GUIStyle() );

            #region FOOTER

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            if ( selection != null )
                GUILayout.Label( $"Name: {selection.name}" );

            else
                GUILayout.Label( "Name:" );

            GUILayout.FlexibleSpace();

            GUILayout.Label( $"Vertices: {totalVertices}" );

            GUILayout.Label( $"Triangles: {totalTris}" );

            GUILayout.EndHorizontal();

            #endregion
        }

        private void OnSelectionChange()
        {
            selection = Selection.activeGameObject;

            DestroyImmediate( gameObjectEditor );

            gameObjectEditor = Editor.CreateEditor( selection );

            totalVertices = 0;
            totalTris = 0;

            if ( selection != null )
            {
                // check all meshes
                var meshFilters = selection.GetComponentsInChildren< MeshFilter >();

                for ( int i = 0, length = meshFilters.Length; i < length; i++ )
                {
                    int verts = meshFilters[i].sharedMesh.vertexCount;
                    totalVertices += verts;

                    totalTris += meshFilters[i].sharedMesh.triangles.Length / 3;
                }

                var skinMeshRenderer = selection.GetComponentsInChildren< SkinnedMeshRenderer >();

                for ( int i = 0, length = skinMeshRenderer.Length; i < length; i++ )
                {
                    int verts = skinMeshRenderer[i].sharedMesh.vertexCount;
                    totalVertices += verts;

                    totalTris += skinMeshRenderer[i].sharedMesh.triangles.Length / 3;
                }
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}