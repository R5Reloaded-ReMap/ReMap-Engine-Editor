using UnityEngine;
using UnityEditor;

public class AssetsSorter : EditorWindow
{
    [MenuItem("ReMap/Asset Library Sorter/Sort Labels", false, 100)]
    public static void Init()
    {
        AssetLibrarySorter.SetModelLabels();
    }
}