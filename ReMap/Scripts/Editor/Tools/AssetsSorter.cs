using UnityEngine;
using UnityEditor;

public class AssetsSorter : EditorWindow
{
    [MenuItem("ReMap/Asset Library Sorter/Sort Labels", false, 100)]
    public static async void Init()
    {
        await AssetLibrarySorter.SetModelLabels();
    }
}