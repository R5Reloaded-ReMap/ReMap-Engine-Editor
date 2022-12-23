using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AssetLibraryManager
{
    public class SearchLabels : EditorWindow
    {
        public static List<string> selectedLabels = new List<string>();

        public static List<string> cacheGuids = new List<string>();
        public static List<Texture2D> cacheThumbs = new List<Texture2D>();

        public static List<Object> selectedPrefabs = new List<Object>();

        public static void GetGUIDs()
        {
            //Get SciptableObject if null
            if (PrefabLabels.settings == null)
            {
                PrefabLabels.settings = Resources.Load("AssetLibrary_Settings") as SettingsObject;
            }

            //Clear includedFolders if all is null
            int countNulls = 0;

            for (int i = 0; i < PrefabLabels.settings.includedFolders.Count; i++)
            {
                if (PrefabLabels.settings.includedFolders[i].includedFolder == null)
                {
                    countNulls++;
                }

                if (PrefabLabels.settings.includedFolders.Count == countNulls)
                {
                    PrefabLabels.settings.includedFolders.Clear();
                }
            }

            //Get Guids only if the list is empty
            if (cacheGuids.Count == 0)
            {
                //Show all prefabs if no folders are present
                if (PrefabLabels.settings.includedFolders.Count == 0)
                {
                    cacheGuids.AddRange(AssetDatabase.FindAssets("t:prefab", new string[] { "Assets" }).ToList());
                }
                else
                {
                    //Get a list of all GUID to include
                    for (int i = 0; i < PrefabLabels.settings.includedFolders.Count; i++)
                    {
                        string path = AssetDatabase.GetAssetPath(PrefabLabels.settings.includedFolders[i].includedFolder);

                        if (path != string.Empty)
                        {
                            //Add all GUIDs in included folders to a list
                            cacheGuids.AddRange(AssetDatabase.FindAssets("t:prefab", new string[] { path }).ToList());
                        }
                    }
                }

                //Remove duplicates
                cacheGuids = cacheGuids.Distinct().ToList();

                //Get a list of GUID to exclude
                List<string> guidsToRemove = new List<string>();

                for (int i = 0; i < PrefabLabels.settings.excludedFolders.Count; i++)
                {
                    string path = AssetDatabase.GetAssetPath(PrefabLabels.settings.excludedFolders[i].excludedFolder);

                    if (path != string.Empty)
                    {
                        guidsToRemove.AddRange(AssetDatabase.FindAssets("t:prefab", new string[] { path }).ToList());
                    }

                    //Remove the ones in excluded folders
                    foreach (var item in guidsToRemove)
                    {
                        cacheGuids.Remove(item);
                    }
                }

                for (int i = 0; i < PrefabLabels.settings.includedPrefabs.Count; i++)
                {
                    string path = AssetDatabase.GetAssetPath(PrefabLabels.settings.includedPrefabs[i]);
                    string guid = AssetDatabase.AssetPathToGUID(path);

                    if (PrefabLabels.settings.includedPrefabs[i] != null)
                    {
                        cacheGuids.Add(guid);
                    }
                }

                //Remove duplicates
                cacheGuids = cacheGuids.Distinct().ToList();

                for (int i = 0; i < PrefabLabels.settings.excludedPrefabs.Count; i++)
                {
                    string path = AssetDatabase.GetAssetPath(PrefabLabels.settings.excludedPrefabs[i]);
                    string guid = AssetDatabase.AssetPathToGUID(path);

                    if (PrefabLabels.settings.excludedPrefabs[i] != null)
                    {
                        cacheGuids.Remove(guid);
                    }
                }

                //Debug.Log($"GetGUIDs");
            }
        }

        public static void Search()
        {
            selectedPrefabs.Clear();

            //If labels are selected
            if (selectedLabels.Count != 0)
            {
                //Search Method "ANY"
                if (PrefabLabels.searchMethod == 0)
                {
                    for (int i = 0; i < selectedLabels.Count; i++)
                    {
                        for (int a = 0; a < cacheGuids.Count; a++)
                        {
                            GUID assetGUID = new GUID(cacheGuids[a]);

                            string getPath = AssetDatabase.GUIDToAssetPath(assetGUID);

                            List<string> getLabels = AssetDatabase.GetLabels(assetGUID).ToList();

                            //Check if getLabels contains at least one of the labels.
                            if (getLabels.Contains(selectedLabels[i]))
                            {
                                Object prefab = AssetDatabase.LoadAssetAtPath<Object>(getPath);

                                selectedPrefabs.Add(prefab);
                            }
                        }
                    }
                }

                //Search Method "ALL"
                else
                {
                    for (int i = 0; i < cacheGuids.Count; i++)
                    {
                        GUID assetGUID = new GUID(cacheGuids[i]);

                        string getPath = AssetDatabase.GUIDToAssetPath(assetGUID);

                        List<string> getLabels = AssetDatabase.GetLabels(assetGUID).ToList();

                        for (int a = 0; a < selectedLabels.Count; a++)
                        {
                            //Check if all selectedLabels are in getLabels
                            if (selectedLabels.All(x => getLabels.Any(y => y == x)))
                            {
                                Object prefab = AssetDatabase.LoadAssetAtPath<Object>(getPath);

                                selectedPrefabs.Add(prefab);
                            }
                        }
                    }
                }
            }

            //Remove duplicates
            selectedPrefabs = selectedPrefabs.Distinct().ToList();

            GetThumbnails();

            //Debug.Log($"Search");
        }

        public static void LoadAllPrefabs()
        {
            if (HasOpenInstances<PrefabLabels>())
            {
                PrefabLabels.Clear_Labels();
            }

            selectedPrefabs.Clear();

            for (int i = 0; i < cacheGuids.Count; i++)
            {
                GUID guid = new GUID(cacheGuids[i]);

                string getPath = AssetDatabase.GUIDToAssetPath(guid);

                Object prefab = AssetDatabase.LoadAssetAtPath<Object>(getPath);

                selectedPrefabs.Add(prefab);
            }

            GetThumbnails();
        }

        public static void GetThumbnails()
        {
            AssetPreview.SetPreviewTextureCacheSize(131072);

            cacheThumbs.Clear();

            for (int i = 0; i < selectedPrefabs.Count; i++)
            {
                cacheThumbs.Add(AssetPreview.GetAssetPreview(selectedPrefabs[i]));
            }

            if (selectedPrefabs.Count != 0)
            {
                //Fix grid Beginning messed Up
                selectedPrefabs.Insert(0, null);
                cacheThumbs.Insert(0, null);
            }

            //Debug.Log($"GetThumbnails");
        }

        public static void Clear_CachePrefabs()
        {
            cacheGuids.Clear();
            cacheThumbs.Clear();

            selectedPrefabs.Clear();

            //Debug.Log($"Clear_CachePrefabs");
        }
    }
}
