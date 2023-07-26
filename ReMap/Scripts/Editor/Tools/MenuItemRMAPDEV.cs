
using UnityEngine;
using UnityEditor;

public class MenuItemRMAPDEV
{
    #if RMAPDEV
        // Legion Stuff
        [ MenuItem( "ReMap Dev Tools/Legion/Create All Rpak List", false, 100 ) ]
        public static void CreateAllRpakList()
        {
            LibrarySorter.LegionRpakExporting.RpakListInit();
        }

        // Prefab Fix Manager
        [ MenuItem( "ReMap Dev Tools/Prefab Fix Manager", false, 100 ) ]
        public static void ShowPrefabFixManager()
        {
            LibrarySorter.LibrarySorterWindow.Init();
        }

        // Console
        [ MenuItem( "ReMap Dev Tools/Console", false, 100 ) ]
        public static void ShowConsole()
        {
            ReMapConsole.Init();
        }

        // Maps Utility
        [ MenuItem( "ReMap Dev Tools/Test Tools/Remove Tools Material" ) ]
        public static void RemoveToolsMaterial()
        {
            ReMapDebug.DeleteObjectsWithSpecificMaterial();
        }

        [ MenuItem( "ReMap Dev Tools/Test Tools/Draw On Map End Zone Trigger", false, 100 ) ]
        public static void DrawOnMap()
        {
            EntCodeDiscoverer.RMAPDEV_DrawOnMap();
        }

        // Debug
        [ MenuItem( "ReMap Dev Tools/Debug/File Write Test", false, 100 ) ]
        public static void FileWriteTest()
        {
            ReMapDebug.Debug_FileWrite();
        }

        [ MenuItem( "ReMap Dev Tools/Debug/Clear Progress Bar", false, 100 ) ]
        public static void ClearProgressBar()
        {
            ReMapDebug.Debug_ClearProgressBar();
        }
    #endif
}
