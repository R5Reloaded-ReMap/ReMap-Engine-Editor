using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLibraryManager
{
    [Serializable]
    public class CreateNewSection
    {
        [HideInInspector]
        public int labelIndex, oldLabelIndex;

        [HideInInspector]
        public bool fold = false;


        [Space(10)]
        public string sectionName = string.Empty;


        [Space(10)]
        [Tooltip("Hides this section in the Editor Window.")]
        public bool hideSection = false;


        [Tooltip("Hides these labels from the Labels section - Clear filters for the changes to take effect")]
        public bool hideFromLabels = true;


        [Space(10)]
        public List<string> sectionLabels = new List<string>();
    }

    [Serializable]
    public class IncludedFolder
    {
        public UnityEngine.Object includedFolder;
    }

    [Serializable]
    public class ExcludedFolder
    {
        public UnityEngine.Object excludedFolder;
    }

    [Serializable]
    public class Options
    {
        [Space(10)]
        [Range(32, 128)]
        public int thumbnailSize = 128;

        public enum OnSelect { doNothing, pingPrefab, selectPrefab };
        [Space(10)]
        [Tooltip("Choose what to do when you click on a prefab in the Viewer.")]
        public OnSelect onClick = OnSelect.selectPrefab;

        [Space(10)]
        [Tooltip("When nothing is selected in Prefab Labels, the Viewer will display all the prefabs. (Can be slow if you have many prefabs)")]
        public bool displayAllPrefabs = true;
    }

    [Serializable]
    public class Placement
    {
        [Space(10)]
        [Tooltip("After dropping a prefab in the scene, press this key to rotate. (By snap settings amount in Y axis)")]
        public KeyCode rotateWith = KeyCode.Space;

        [Space(10)]
        [Range(1, 20)]
        [Tooltip("Mouse travel distance before a rotation or scale happens, when using Ctrl + Shift. (1 = faster, 20 = slower)")]
        public int mouseSensibility = 3;

        [Space(10)]
        [Tooltip("Allow rotation when using Ctrl + Shift in scene view.")]
        public bool useRotation = true;
        [Tooltip("Use rotation snap setting.")]
        public bool useRotationSnap = true;

        [Space(10)]
        [Tooltip("Allow scale when using Ctrl + Shift in scene view.")]
        public bool useScale = true;
        [Tooltip("Use scale snap setting.")]
        public bool useScaleSnap = true;
    }
}