using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace AssetLibraryManager
{
    public class PrefabLabels : EditorWindow
    {
        public static SettingsObject settings;

        Vector2 scrollPosition;

        bool customFold = true;

        public static List<string> customLabels = new List<string>();
        public static List<string> isPressed = new List<string>();

        public static int searchMethod, oldSearchMethod;
        string[] searchMode = { "ANY", "ALL" };

        public static void ShowWindow()
        {
            GetWindow<PrefabLabels>("Prefab Labels");
        }

        void CreateGUI()
        {
            settings = Resources.Load("AssetLibrary_Settings") as SettingsObject;

            //Sort all labels and capitalize first character.
            for (int i = 0; i < settings.createNewSection.Count; i++)
            {
                for (int l = 0; l < settings.createNewSection[i].sectionLabels.Count; l++)
                {
                    settings.createNewSection[i].sectionLabels.Sort();

                    settings.createNewSection[i].sectionLabels[l] = FirstCharToUpper(settings.createNewSection[i].sectionLabels[l]);
                }
            }

            SetUp_Labels();
        }

        string FirstCharToUpper(string s)
        {
            // Check for empty string.  
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            // Return char and concat substring.  
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private void OnEnable()
        {
            if (!HasOpenInstances<PrefabViewer>())
            {
                SceneView.duringSceneGui += PrefabViewer.SceneGUI;
            }
        }

        void OnDisable()
        {
            if (!HasOpenInstances<PrefabViewer>())
            {
                Clear_Labels();

                SearchLabels.Clear_CachePrefabs();

                SceneView.duringSceneGui -= PrefabViewer.SceneGUI;
            }

            //Debug.Log($"Disable");
        }

        void OnGUI()
        {
            GUILayout.Space(10);

            #region SEARCH_METHOD
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Search Method:");
            searchMethod = GUILayout.SelectionGrid(searchMethod, searchMode, 2);

            if (searchMethod != oldSearchMethod)
            {
                oldSearchMethod = searchMethod;

                Clear_Labels();
                SetUp_Labels();

                if (settings.options.displayAllPrefabs == true)
                {
                    SearchLabels.LoadAllPrefabs();
                }
                else
                {
                    SearchLabels.Search();
                }
            }

            EditorGUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);

            CreateSection(CustomGUIStyles.FoldStyle());

            Custom_Labels_btn(CustomGUIStyles.FoldStyle());

            EditorGUILayout.EndScrollView();



            GUILayout.FlexibleSpace();

            #region FOOTER
            GUILayout.BeginHorizontal();

            GUIContent content = new GUIContent();

            content.image = Resources.Load("icons/alm/clear") as Texture2D;
            
            if (GUILayout.Button(content, CustomGUIStyles.FooterStyle()))
            {
                Clear_Labels();
                SetUp_Labels();
                SearchLabels.Search();

                if (settings.options.displayAllPrefabs == true)
                {
                    SearchLabels.LoadAllPrefabs();
                }

                Debug.Log($"Filters Cleared");
            }

            GUILayout.Label("Clear Filter");


            GUILayout.FlexibleSpace();


            content.tooltip = "Label Manager";
            content.image = Resources.Load("icons/alm/labels") as Texture2D;
            
            if (GUILayout.Button(content, CustomGUIStyles.FooterStyle()))
            {
                LabelsManager.ShowWindow();
            }


            content.tooltip = "Settings";
            content.image = Resources.Load("icons/alm/settings") as Texture2D;

            if (GUILayout.Button(content, CustomGUIStyles.FooterStyle()))
            {
                SelectSettingsObject.SelectSettings();
            }

            GUILayout.EndHorizontal();
            #endregion
        }

        void CreateSection(GUIStyle guiStyle)
        {
            for (int i = 0; i < settings.createNewSection.Count; i++)
            {
                //Get each sections
                CreateNewSection createNewTab = settings.createNewSection[i];

                //Show or Hide Section
                if (createNewTab.hideSection == false && createNewTab.sectionLabels.Count != 0)
                {
                    //Fold sections
                    createNewTab.fold = EditorGUILayout.BeginFoldoutHeaderGroup(createNewTab.fold, createNewTab.sectionName, guiStyle);

                    if (createNewTab.fold == true)
                    {
                        //Create a grid witch each createNewTab.sectionLabels
                        createNewTab.labelIndex = GUILayout.SelectionGrid(createNewTab.labelIndex, createNewTab.sectionLabels.ToArray(), 1);

                        //On label clicked
                        if (createNewTab.labelIndex != createNewTab.oldLabelIndex)
                        {
                            createNewTab.oldLabelIndex = createNewTab.labelIndex;

                            for (int a = 0; a < createNewTab.sectionLabels.Count; a++)
                            {
                                //Clear all section labels in selectedLabels
                                SearchLabels.selectedLabels.Remove(createNewTab.sectionLabels[a]);

                                if (createNewTab.sectionLabels[createNewTab.labelIndex] == createNewTab.sectionLabels[a])
                                {
                                    //Add only selected label except if it is "Clear"
                                    if (createNewTab.sectionLabels[createNewTab.labelIndex] != "Clear")
                                    {
                                        SearchLabels.selectedLabels.Add(createNewTab.sectionLabels[createNewTab.labelIndex]);
                                    }

                                    //Debug.Log(createNewTab.sectionLabels[createNewTab.labelIndex]);
                                }
                            }

                            if (searchMethod == 1)
                            {
                                //Reset selection when changing section labels.
                                foreach (var label in isPressed)
                                {
                                    SearchLabels.selectedLabels.Remove(label.Replace("|", ""));
                                }

                                isPressed.Clear();
                                isPressed.Add("Clear|");

                                SetUp_Labels();
                            }

                            if (createNewTab.sectionLabels[createNewTab.labelIndex] != "Clear")
                            {
                                PrefabViewer.ShowWindow();
                            }

                            SearchLabels.Search();

                            //Load all prefabs when nothing is selected
                            if (SearchLabels.selectedLabels.Count == 0)
                            {
                                if (settings.options.displayAllPrefabs == true)
                                {
                                    SearchLabels.LoadAllPrefabs();
                                }                              
                            }
                        }
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();

                    GUILayout.Space(10);
                }
            }
        }

        void Custom_Labels_btn(GUIStyle guiStyle)
        {
            customFold = EditorGUILayout.BeginFoldoutHeaderGroup(customFold, "Labels", guiStyle);

            if (customFold == true)
            {
                //Check all labels
                for (int i = 0; i < customLabels.Count; i++)
                {
                    //Button is not Pressed yet
                    if (!isPressed.Contains(customLabels[i] + "|"))
                    {
                        //Create button
                        if (GUILayout.Button(customLabels[i]))
                        {
                            isPressed.Add(customLabels[i] + "|");
                            //Debug.Log($"Add: {customLabels[i]}");

                            //Clear is pressed
                            if (customLabels[i] == "Clear")
                            {
                                //Remove all isPressed from SearchLabels.selectedLabels
                                foreach (var label in isPressed)
                                {
                                    SearchLabels.selectedLabels.Remove(label.Replace("|", ""));
                                }

                                isPressed.Clear();
                                isPressed.Add("Clear|");
                            }

                            else
                            {
                                isPressed.Remove("Clear|");

                                //Add single label
                                SearchLabels.selectedLabels.Add(customLabels[i]);

                                PrefabViewer.ShowWindow();
                            }


                            if (searchMethod == 1)
                            {
                                SetUp_Labels();
                            }

                            SearchLabels.Search();

                            //Load all prefabs when nothing is selected
                            if (SearchLabels.selectedLabels.Count == 0)
                            {
                                if (settings.options.displayAllPrefabs == true)
                                {
                                    SearchLabels.LoadAllPrefabs();
                                }
                            }
                        }
                    }
                    //Button is pressed down
                    else
                    {
                        var oldColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(0.65f, 0.65f, 0.65f);

                        if (GUILayout.Button(customLabels[i]))
                        {
                            isPressed.Remove(customLabels[i] + "|");
                            //Debug.Log($"Remove: {customLabels[i] + "|"}");

                            //Remove single label
                            SearchLabels.selectedLabels.Remove(customLabels[i]);

                            SearchLabels.Search();
                            PrefabViewer.ShowWindow();

                            //Reset
                            if (isPressed.Count == 0)
                            {
                                isPressed.Add("Clear|");
                            }

                            if (searchMethod == 1)
                            {
                                SetUp_Labels();
                            }
                        }

                        GUI.backgroundColor = oldColor;
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            GUILayout.Space(10);
        }

        public static void SetUp_Labels()
        {
            customLabels.Clear();

            //If first launch, cacheGuids and turn "Clear" On.
            SearchLabels.GetGUIDs();

            if (SearchLabels.cacheGuids.Count == 0)
            {
                if (!isPressed.Contains("Clear|"))
                {
                    isPressed.Add("Clear|");
                }
            }      

            //Get labels from guids
            foreach (var id in SearchLabels.cacheGuids)
            {
                GUID guid = new GUID(id);

                List<string> getLabels = AssetDatabase.GetLabels(guid).ToList();

                //Search Method "ANY"
                if (searchMethod == 0)
                {
                    //Add all Labels to list
                    for (int i = 0; i < getLabels.Count; i++)
                    {
                        //Add all project labels to a list
                        customLabels.Add(getLabels[i]);
                    }
                }
                //Search Method "ALL"
                else
                {
                    //If nothing is selected add all
                    if (SearchLabels.selectedLabels.Count == 0)
                    {
                        //Add all Labels to list
                        for (int i = 0; i < getLabels.Count; i++)
                        {
                            customLabels.Add(getLabels[i]);
                        }
                    }
                    else
                    {
                        //Add labels if all selectedLabels are in getLabels
                        for (int i = 0; i < SearchLabels.selectedLabels.Count; i++)
                        {
                            if (SearchLabels.selectedLabels.All(x => getLabels.Any(y => y == x)))
                            {
                                //Add all Labels
                                for (int la = 0; la < getLabels.Count; la++)
                                {
                                    customLabels.Add(getLabels[la]);
                                }
                            }
                        }
                    }
                }
            }

            //Remove duplicates
            customLabels = customLabels.Distinct().ToList();

            //Remove Section from results
            for (int i = 0; i < settings.createNewSection.Count; i++)
            {
                for (int a = 0; a < settings.createNewSection[i].sectionLabels.Count; a++)
                {
                    if (settings.createNewSection[i].hideFromLabels == true)
                    {
                        customLabels.Remove(settings.createNewSection[i].sectionLabels[a]);
                    }
                }
            }


            customLabels.Sort();


            //Add Clear at the beginning of each section
            for (int i = 0; i < settings.createNewSection.Count; i++)
            {
                for (int a = 0; a < settings.createNewSection[i].sectionLabels.Count; a++)
                {
                    if (settings.createNewSection[i].sectionLabels.Count != 0 && settings.createNewSection[i].sectionLabels[0] != "Clear")
                    {
                        settings.createNewSection[i].sectionLabels.Insert(0, "Clear");

                        //Fail safe - Fix unity bug for not saving scriptable objects properly.
                        settings.createNewSection[i].sectionLabels = settings.createNewSection[i].sectionLabels.Distinct().ToList();
                    }
                }
            }

            if (customLabels.Count == 0)
            {
                customLabels.Add("Clear");
            }
            else if (customLabels[0] != "Clear")
            {
                customLabels.Insert(0, "Clear");
            }

            //Debug.Log($"Set Labels");
        }

        public static void Clear_Labels()
        {
            for (int i = 0; i < settings.createNewSection.Count; i++)
            {
                settings.createNewSection[i].labelIndex = 0;
                settings.createNewSection[i].oldLabelIndex = 0;
            }

            isPressed.Clear();
            isPressed.Add("Clear|");

            SearchLabels.selectedLabels.Clear();

            //Debug.Log($"Labels Cleared");
        }


        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}