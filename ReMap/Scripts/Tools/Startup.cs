using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Startup {
    static Startup()
    {
        //These will run when Unity starts up
        TagHelper.CheckAndCreateTags();
        //AssetLibrarySorter.LibrarySorter();
        AssetLibrarySorter.SetModelLabels();
    }
}