using UnityEngine;
using UnityEditor;

namespace AssetLibraryManager
{
    public class Preview_Window : EditorWindow
    {
        GameObject selection;
        Editor gameObjectEditor;

        int totalVertices = 0;
        int totalTris = 0;

        [MenuItem("ReMap/Asset Library Manager/Preview Window...", false, 1068)]
        public static void ShowWindow()
        {
            Preview_Window window = GetWindow<Preview_Window>("Preview Window");
            window.minSize = new Vector2(20, 20);
        }

        void CreateGUI()
        {
            selection = Selection.activeGameObject;

            gameObjectEditor = Editor.CreateEditor(selection);
        }

        void OnGUI()
        {
            if (selection != null)
            {
                gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(0, 2048, 0, 2048), new GUIStyle());
            }

            #region FOOTER
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            if (selection != null)
            {
                GUILayout.Label($"Name: {selection.name}");
            }

            else
            {
                GUILayout.Label($"Name:");
            }

            GUILayout.FlexibleSpace();

            GUILayout.Label($"Vertices: {totalVertices}");

            GUILayout.Label($"Triangles: {totalTris}");

            GUILayout.EndHorizontal();
            #endregion
        }

        private void OnSelectionChange()
        {
            selection = Selection.activeGameObject;

            DestroyImmediate(gameObjectEditor);

            gameObjectEditor = Editor.CreateEditor(selection);

            totalVertices = 0;
            totalTris = 0;

            if (selection != null)
            {
                // check all meshes
                MeshFilter[] meshFilters = selection.GetComponentsInChildren<MeshFilter>();

                for (int i = 0, length = meshFilters.Length; i < length; i++)
                {
                    int verts = meshFilters[i].sharedMesh.vertexCount;
                    totalVertices += verts;

                    totalTris += meshFilters[i].sharedMesh.triangles.Length / 3;
                }

                SkinnedMeshRenderer[] skinMeshRenderer = selection.GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0, length = skinMeshRenderer.Length; i < length; i++)
                {
                    int verts = skinMeshRenderer[i].sharedMesh.vertexCount;
                    totalVertices += verts;

                    totalTris += skinMeshRenderer[i].sharedMesh.triangles.Length / 3;
                }
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}