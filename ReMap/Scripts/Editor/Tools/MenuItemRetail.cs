using AssetLibraryManager;
using CodeViews;
using ImportExport.Json;
using MultiTool;
using Sandbox;
using ThemesPlugin;
using UnityEditor;

public class MenuItemRetail
{
    // AssetLibraryManager
    [MenuItem( "ReMap/Asset Library Manager/Labels Manager...", false, 1080 )]
    public static void ShowLabelsManagerWindow()
    {
        LabelsManager.ShowWindow();
    }

    [MenuItem( "ReMap/Asset Library Manager/Prefab Labels...", false, 1080 )]
    public static void ShowPrefabLabelsWindow()
    {
        PrefabLabels.ShowWindow();
    }

    [MenuItem( "ReMap/Asset Library Manager/Prefab Viewer...", false, 1080 )]
    public static void ShowPrefabViewerWindow()
    {
        PrefabViewer.ShowWindow();
    }

    [MenuItem( "ReMap/Asset Library Manager/Preview Window...", false, 1080 )]
    public static void ShowPreviewWindow()
    {
        Preview_Window.ShowWindow();
    }

    [MenuItem( "ReMap/Asset Library Manager/Settings...", false, 1100 )]
    public static void ShowAssetLibraryManagerSettingsWindow()
    {
        SelectSettingsObject.SelectSettings();
    }

    // Themes
    [MenuItem( "ReMap/Themes", false, 1080 )]
    public static void ShowThemes()
    {
        ThemeSettings.ShowWindow();
    }

    // Import Export
    [MenuItem( "ReMap/Export/Json", false, 50 )]
    public static void ExportJson()
    {
        JsonExport.ExportJson();
    }

    [MenuItem( "ReMap/Export Selection/Json", false, 50 )]
    public static void ExportSelectionJson()
    {
        JsonExport.ExportSelectionJson();
    }

    [MenuItem( "ReMap/Import/Json", false, 50 )]
    public static void ImportJson()
    {
        JsonImport.ImportJson();
    }

    [MenuItem( "ReMap/Import/Map Code", false, 50 )]
    public static void ImportMapCode()
    {
        CodeImport.Init();
    }

    [MenuItem( "ReMap/Import/DataTable", false, 50 )]
    public static void ImportDataTable()
    {
        ImportExportDataTable.ImportDataTable();
    }

    // Popout Menu
    [MenuItem( "ReMap/Popout Menu", false, 1 )]
    public static void ShowQuickMenuWindow()
    {
        QuickMenu.Init();
    }

    // Code Views
    [MenuItem( "ReMap/Code Views", false, 25 )]
    public static void ShowCodeViewsWindow()
    {
        CodeViewsWindow.Init();
    }

    // Tools
    [MenuItem( "ReMap/Tools/Prop Info", false, 100 )]
    public static void ShowPropInfoWindow()
    {
        PropInfo.Init();
    }

    [MenuItem( "ReMap/Tools/Multi Tool", false, 100 )]
    public static void ShowMultiToolWindow()
    {
        MultiToolWindow.Init();
    }

    [MenuItem( "ReMap/Tools/Ent Code Discoverer", false, 100 )]
    public static void ShowEntCodeDiscovererWindow()
    {
        EntCodeDiscoverer.Init();
    }

    //[ MenuItem( "ReMap/Tools/Loot Repartition", false, 100 ) ]
    //public static void ShowLootRepartitionWindow()
    //{
    //    LootRepartitionWindow.Init();
    //}

    [MenuItem( "ReMap/Tools/Grid Tool", false, 100 )]
    public static void ShowGridTool()
    {
        GridTool.Init();
    }

    [MenuItem( "ReMap/Tools/RealmID Tool", false, 100 )]
    public static void Init()
    {
        SetRealmIds.Init();
    }

    [MenuItem( "ReMap/Tools/SceneViewGUI/Enable" )]
    public static void SceneViewGUIEnable()
    {
        SceneViewTools.Enable();
    }

    [MenuItem( "ReMap/Tools/SceneViewGUI/Disable" )]
    public static void SceneViewGUIDisable()
    {
        SceneViewTools.Disable();
    }

    // Sandbox
    [MenuItem( "ReMap/Sandbox/Fix Old Doors", false, 100 )]
    public static void SBFixOldDoors()
    {
        FixOldDoor.FixOldDoors();
    }

    [MenuItem( "ReMap/Sandbox/Fix Prefabs", false, 100 )]
    public static void SBFixPrefabs()
    {
        FixPrefabs.FixPrefabsInScene();
    }

    [MenuItem( "ReMap/Sandbox/Move Prop", false, 100 )]
    public static void SBMoveProp()
    {
        MoveProp.MoveProps();
    }

    // Infos
    [MenuItem( "ReMap/Infos", false, 0 )]
    public static void ShowInfo()
    {
        InfoUI.Init();
    }
}