
using UnityEngine;
using UnityEditor;

public class MenuItemRMAPDEV
{
    #if RMAPDEV
        // Legion Stuff
        [ MenuItem( "Dev Menu/Legion/Create All Rpak List", false, 100 ) ]
        public static void CreateAllRpakList()
        {
            LibrarySorter.LegionRpakExporting.RpakListInit();
        }

        // Prefab Fix Manager
        [ MenuItem( "Dev Menu/Prefab Fix Manager", false, 100 ) ]
        public static void ShowPrefabFixManager()
        {
            LibrarySorter.LibrarySorterWindow.Init();
        }

        // Console
        [ MenuItem( "Dev Menu/Console", false, 100 ) ]
        public static void ShowConsole()
        {
            ReMapConsole.Init();
        }

        // Maps Utility
        [ MenuItem( "Dev Menu/Test Tools/Remove Tools Material" ) ]
        public static void RemoveToolsMaterial()
        {
            ReMapDebug.DeleteObjectsWithSpecificMaterial();
        }

        [ MenuItem( "Dev Menu/Test Tools/Draw On Map End Zone Trigger", false, 100 ) ]
        public static void DrawOnMap()
        {
            EntCodeDiscoverer.RMAPDEV_DrawOnMap();
        }

        // Utility
        [ MenuItem( "Dev Menu/Utility/Write Loot Repartition File", false, 100 ) ]
        public static void WriteLootRepartitionFile()
        {
            LootRepartitionWindow.WriteLootRepartitionFile();
        }

        // Debug
        [ MenuItem( "Dev Menu/Debug/File Write Test", false, 100 ) ]
        public static void FileWriteTest()
        {
            ReMapDebug.Debug_FileWrite();
        }

        [ MenuItem( "Dev Menu/Debug/Clear Progress Bar", false, 100 ) ]
        public static void ClearProgressBar()
        {
            ReMapDebug.Debug_ClearProgressBar();
        }
    #endif
}
