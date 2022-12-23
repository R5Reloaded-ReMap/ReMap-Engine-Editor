using UnityEngine;
using UnityEditor;

namespace AssetLibraryManager
{
    public class PrefabViewer : EditorWindow
    {
        static SettingsObject settings;

        Vector2 scrollView;

        static int columns = 5;

        int edgePadding = 5;
        int thumbSpacing = 2;

        int totalWidth;
        int totalHeight;

        public static int bottomHeight = 20;

        Texture2D placeHolder;


        [MenuItem("ReMap/Asset Library Manager/Prefab Viewer...", false, 1067)]
        public static void ShowWindow()
        {
            GetWindow<PrefabViewer>("Prefab Viewer");
        }

        void CreateGUI()
        {
            settings = Resources.Load("AssetLibrary_Settings") as SettingsObject;

            placeHolder = Resources.Load("icons/alm/loading") as Texture2D;

            if (!HasOpenInstances<PrefabLabels>())
            {
                if (SearchLabels.selectedLabels.Count == 0 && settings.options.displayAllPrefabs == true)
                {
                    SearchLabels.Clear_CachePrefabs();
                    SearchLabels.GetGUIDs();
                    SearchLabels.LoadAllPrefabs();
                }
            }
        }

        private void OnEnable()
        {
            if (!HasOpenInstances<PrefabLabels>())
            {
                SceneView.duringSceneGui += SceneGUI;
            }
        }

        void OnDisable()
        {
            if (!HasOpenInstances<PrefabLabels>())
            {
                PrefabLabels.Clear_Labels();

                SearchLabels.Clear_CachePrefabs();

                SceneView.duringSceneGui -= SceneGUI;
            }

            //Debug.Log($"Disable");
        }

        public static void SceneGUI(SceneView sceneView)
        {
            if (settings == null)
            {
                settings = Resources.Load("AssetLibrary_Settings") as SettingsObject;
            }

            //Rotate object when ctrl and shift are pressed.
            if (Event.current.control && Event.current.shift)
            {
                Undo.RegisterCompleteObjectUndo(Selection.transforms, "Scale");

                Vector2 delta = Event.current.delta;

                foreach (GameObject item in Selection.gameObjects)
                {
                    if (settings.placement.useScale == true)
                    {
                        if (settings.placement.useScaleSnap == true)
                        {
                            if (Time.frameCount % settings.placement.mouseSensibility == 0)
                            {
                                Vector3 scale = item.transform.localScale;

                                float xyRatio = scale.x / scale.y;
                                float xzRatio = scale.x / scale.z;

                                if (delta.y < -5)
                                {
                                    scale.x += EditorSnapSettings.scale;
                                }

                                if (delta.y > 5)
                                {
                                    scale.x -= EditorSnapSettings.scale;
                                }

                                scale.y = scale.x / xyRatio;
                                scale.z = scale.x / xzRatio;

                                if (scale.x > 0 && scale.y > 0 && scale.z > 0)
                                {
                                    item.transform.localScale = scale;
                                }
                            }
                        }

                        else
                        {
                            Vector3 scale = item.transform.localScale;

                            float xyRatio = scale.x / scale.y;
                            float xzRatio = scale.x / scale.z;

                            scale.x -= delta.y * 0.02f;

                            scale.y = scale.x / xyRatio;
                            scale.z = scale.x / xzRatio;

                            if (scale.x > 0 && scale.y > 0 && scale.z > 0)
                            {
                                item.transform.localScale = scale;
                            }
                        }
                    }

                    if (settings.placement.useRotation == true)
                    {
                        if (settings.placement.useRotationSnap == true)
                        {
                            if (Time.frameCount % settings.placement.mouseSensibility == 0)
                            {
                                float rot = item.transform.localEulerAngles.y;
                                Vector3 angle = item.transform.localEulerAngles;

                                if (delta.x > 1)
                                {
                                    rot += EditorSnapSettings.rotate;
                                }

                                if (delta.x < -1)
                                {
                                    rot -= EditorSnapSettings.rotate;
                                }

                                angle.y = rot;
                                item.transform.localEulerAngles = angle;
                            }
                        }
                        else
                        {
                            item.transform.Rotate(Vector3.up * delta.x);
                        }
                    }
                }
            }

            //Rotate object by snap settings value.
            if (Event.current.keyCode == settings.placement.rotateWith && Event.current.type == EventType.KeyUp)
            {
                Undo.RegisterCompleteObjectUndo(Selection.transforms, "Rotation");

                foreach (GameObject item in Selection.gameObjects)
                {
                    float rot = item.transform.localEulerAngles.y;
                    rot += EditorSnapSettings.rotate;

                    item.transform.localEulerAngles = new Vector3(0, rot, 0);
                }
            }

            sceneView.Repaint();
        }

        private void OnGUI()
        {
            //Reload thumbnail if null
            for (int i = 1; i < SearchLabels.cacheThumbs.Count; i++)
            {
                bool isLoading = false;

                if (SearchLabels.cacheThumbs[i] != null)
                {
                    isLoading = AssetPreview.IsLoadingAssetPreview(SearchLabels.cacheThumbs[i].GetInstanceID());
                }

                else if (isLoading == false)
                {
                    SearchLabels.cacheThumbs[i] = AssetPreview.GetAssetPreview(SearchLabels.selectedPrefabs[i]);
                }
            }

            #region PREFAB_GRID_LAYOUT

            float horizontalSpacing = 0;
            float verticalSpacing = 0;

            int rows = 1;

            float scrollRatio = scrollView.y / (totalHeight - 631); //Debug.Log($"{scrollRatio}");

            //float strech = (position.width - totalWidth) / columns;

            scrollView = GUILayout.BeginScrollView(scrollView);
            {
                GUILayout.Label("", GUILayout.Width(totalWidth - 8), GUILayout.Height(totalHeight));

                GUIContent someContent = new GUIContent();

                for (int i = 1; i < SearchLabels.selectedPrefabs.Count; i++)
                {
                    //someContent.tooltip = "";
                    if (SearchLabels.cacheThumbs[i] == null)
                    {
                        someContent.image = placeHolder;
                    }
                    else
                    {
                        someContent.image = SearchLabels.cacheThumbs[i];
                    }

                    GUILayout.BeginArea(new Rect(edgePadding + horizontalSpacing, edgePadding + verticalSpacing, PrefabLabels.settings.options.thumbnailSize, PrefabLabels.settings.options.thumbnailSize), someContent);

                    if (Event.current.type == EventType.MouseDown)
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.StartDrag("Dragging");
                        DragAndDrop.objectReferences = new Object[] { SearchLabels.selectedPrefabs[i] };
                    }

                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (settings.options.onClick == Options.OnSelect.selectPrefab)
                        {
                            Selection.activeObject = SearchLabels.selectedPrefabs[i];
                        }

                        if (settings.options.onClick == Options.OnSelect.pingPrefab)
                        {
                            EditorGUIUtility.PingObject(SearchLabels.selectedPrefabs[i]);
                        }
                    }

                    horizontalSpacing += PrefabLabels.settings.options.thumbnailSize + thumbSpacing; //+ strech;

                    if (i % columns == 0)
                    {
                        horizontalSpacing = 0;

                        verticalSpacing += PrefabLabels.settings.options.thumbnailSize + thumbSpacing;
                    }

                    if ((i - 1) % columns == 0)
                    {
                        rows++;
                    }

                    GUILayout.EndArea();
                }
            }

            rows--;

            GUILayout.EndScrollView();
            #endregion


            #region AUTOMATIC_GRID_LAYOUT
            //             window padding                  number of thumbnails                       thumbnail padding    
            totalWidth = (edgePadding * 2) + (columns * PrefabLabels.settings.options.thumbnailSize) + (thumbSpacing * (columns - 1));
            totalHeight = (edgePadding * 2) + (rows * PrefabLabels.settings.options.thumbnailSize) + (thumbSpacing * (rows - 1));

            if (Event.current.type == EventType.Repaint)
            {
                if (position.width > totalWidth + PrefabLabels.settings.options.thumbnailSize)
                {
                    columns++;
                }

                if (position.width < totalWidth && columns != 1)
                {
                    columns--;
                }
            }
            #endregion

            //GUILayout.Space(edgePadding);

            #region FOOTER
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();


            GUIContent content = new GUIContent();

            int count = SearchLabels.selectedPrefabs.Count - 1;

            if (SearchLabels.selectedPrefabs.Count == 0)
            {
                count = 0;
            }

            content.image = Resources.Load("icons/alm/reload") as Texture2D;

            if (GUILayout.Button(content, CustomGUIStyles.FooterStyle()))
            {
                SearchLabels.Clear_CachePrefabs();
                SearchLabels.GetGUIDs();
                SearchLabels.LoadAllPrefabs();

                PrefabLabels.SetUp_Labels();

                Debug.Log($"Prefabs loaded.");
            }

            GUILayout.Label($"Load All Prefabs: {count}");

            GUILayout.FlexibleSpace();


            content.tooltip = "Select all prefabs in viewer";
            content.image = Resources.Load("icons/alm/select") as Texture2D;

            if (GUILayout.Button(content, CustomGUIStyles.FooterStyle()))
            {
                if (SearchLabels.selectedPrefabs.Count != 0)
                {
                    EditorGUIUtility.PingObject(SearchLabels.selectedPrefabs[0]);
                }

                if (SearchLabels.selectedPrefabs.Count > 500)
                {
                    if (EditorUtility.DisplayDialog("Select Confirmation", $"Selecting {SearchLabels.selectedPrefabs.Count} prefabs.\n\nAre you sure?", "Ok", "Cancel"))
                    {
                        Selection.objects = SearchLabels.selectedPrefabs.ToArray();

                        Debug.Log($"{count} prefabs selected");
                    }
                }
                else
                {
                    Selection.objects = SearchLabels.selectedPrefabs.ToArray();

                    Debug.Log($"{count} prefabs selected");
                }
            }

            content.tooltip = "Open preview window";
            content.image = Resources.Load("icons/alm/preview") as Texture2D;

            if (GUILayout.Button(content, CustomGUIStyles.FooterStyle()))
            {
                Preview_Window.ShowWindow();
            }

            GUILayout.EndHorizontal();
            #endregion
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}