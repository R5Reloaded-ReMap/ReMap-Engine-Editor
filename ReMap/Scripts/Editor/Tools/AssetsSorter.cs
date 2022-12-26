using UnityEngine;
using UnityEditor;

public class AssetsSorter : EditorWindow
{
    [MenuItem("ReMap/Asset Library Sorter/Sort Labels", false, 100)]
    public static async void SetModelLabelsInit()
    {
        await AssetLibrarySorter.SetModelLabels();
    }

    [MenuItem("ReMap/Asset Library Sorter/Check Models Files", false, 100)]
    public static void LibrarySorterInit()
    {
        AssetLibrarySorter.LibrarySorter();
    }
}