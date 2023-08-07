
using UnityEngine;
using UnityEditor;

public class MenuItemRMAPDEV
{
    #if RMAPDEV
        // Legion Stuff
        [ MenuItem( "Dev Menu/Legion/Create All Rpak List", false, 100 ) ]
        public static void CreateAllRpakList()
        {
            LibrarySorter.LegionExporting.RpakListInit();
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

        [ MenuItem( "Dev Menu/Test Tools/Loot Repartition", false, 100 ) ]
        public static void ShowLootRepartitionWindow()
        {
            LootRepartitionWindow.Init();
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

        [ MenuItem( "Dev Menu/Test Tools/ReTexture Window", false, 100 ) ]
        public static void ShowReTextureWindow()
        {
            ReTexture.Init();
        }

        [ MenuItem( "Dev Menu/Test Tools/Delete Multiple Formats", false, 100 ) ]
        public static void DeleteMultipleFormats()
        {
            ReMapDebug.DeleteMultipleFormats();
        }

        [ MenuItem( "Dev Menu/Material Manager", false, 100 ) ]
        public static void OpenMaterialManagerWindow()
        {
            LibrarySorter.MaterialManagerWindow.Init();
        }

        [ MenuItem( "Dev Menu/Material Selector", false, 100 ) ]
        public static void ShowMaterialWindowSelectorWindow()
        {
            LibrarySorter.MaterialWindowSelector.Init();
        }

        [ MenuItem( "Dev Menu/Test Tools/Resize all Textures", false, 100 ) ]
        public static void ResizeAllTextures()
        {
            ReMapDebug.ResizeAllTextures();
        }
    #endif
}
