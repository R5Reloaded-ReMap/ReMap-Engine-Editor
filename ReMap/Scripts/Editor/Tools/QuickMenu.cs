using UnityEngine;
using UnityEditor;
using ThemesPlugin;
using AssetLibraryManager;

public class QuickMenu : EditorWindow
{
    GameObject SelectedObject = null;

    bool utilfold = true;
    bool importfold = true;
    bool exportfold = true;
    bool exportfold2 = false;
    bool exportfold3 = false;
    bool toolsfold = true;
    bool otherfold = true;
    bool otherfold2 = false;

    Vector2 scroll;

    [MenuItem("ReMap/Popout Menu", false, 1)]
    static void Init()
    {
        QuickMenu window = (QuickMenu)EditorWindow.GetWindow(typeof(QuickMenu), false, "Popout Menu");
        window.Show();
    }

    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUILayout.BeginVertical("box");
        utilfold = EditorGUILayout.BeginFoldoutHeaderGroup(utilfold, "Utilities");
        if (utilfold)
        {
            if (GUILayout.Button("Code Views", GUILayout.ExpandWidth(true)))
                CodeViews.Init();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        importfold = EditorGUILayout.BeginFoldoutHeaderGroup(importfold, "Import");
        if (importfold)
        {
            if (GUILayout.Button("Map Code", GUILayout.ExpandWidth(true)))
                CodeImport.Init();
            if (GUILayout.Button("Datatable", GUILayout.ExpandWidth(true)))
                ImportExportDataTable.ImportDataTable();
            if (GUILayout.Button("Json", GUILayout.ExpandWidth(true)))
                ImportExportJson.ImportJson();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        exportfold = EditorGUILayout.BeginFoldoutHeaderGroup(exportfold, "Export");
        if (exportfold)
        {
            exportfold2 = EditorGUILayout.Foldout(exportfold2, "Map with origin offset");
            if(exportfold2)
            {
                if (GUILayout.Button("Whole Script", GUILayout.ExpandWidth(true)))
                    MapExport.ExportWholeScriptOffset();
                if (GUILayout.Button("Map Only", GUILayout.ExpandWidth(true)))
                    MapExport.ExportMapOnlyOffset();
            }
            exportfold3 = EditorGUILayout.Foldout(exportfold3, "Map without origin offset");
            if(exportfold3)
            {
                if (GUILayout.Button("Whole Script", GUILayout.ExpandWidth(true)))
                    MapExport.ExportWholeScript();
                if (GUILayout.Button("Map Only", GUILayout.ExpandWidth(true)))
                    MapExport.ExportOnlyMap();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Script.ent", GUILayout.ExpandWidth(true)))
                MapExport.ExportScriptEntCode();
            if (GUILayout.Button("Sound.end", GUILayout.ExpandWidth(true)))
                MapExport.ExportSoundEntCode();
            if (GUILayout.Button("Datatable", GUILayout.ExpandWidth(true)))
                ImportExportDataTable.ExportDataTable();
            if (GUILayout.Button("Json", GUILayout.ExpandWidth(true)))
                ImportExportJson.ExportJson();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        toolsfold = EditorGUILayout.BeginFoldoutHeaderGroup(toolsfold, "Tools");
        if (toolsfold)
        {
            if (GUILayout.Button("Prop Info", GUILayout.ExpandWidth(true)))
                PropInfo.Init();
            if (GUILayout.Button("Grid Tool", GUILayout.ExpandWidth(true)))
                GridTool.Init();
            if (GUILayout.Button("Realm ID Tool", GUILayout.ExpandWidth(true)))
                SetRealmIds.Init();
            if (GUILayout.Button("Serialize Tool", GUILayout.ExpandWidth(true)))
                SerializeTool.Init();
            if (GUILayout.Button("Measure Distance Tool", GUILayout.ExpandWidth(true)))
                ModelDistance.Init();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        otherfold = EditorGUILayout.BeginFoldoutHeaderGroup(otherfold, "Other");
        if (otherfold)
        {
            if (GUILayout.Button("Themes", GUILayout.ExpandWidth(true)))
                ThemeSettings.ShowWindow();
            otherfold2 = EditorGUILayout.Foldout(otherfold2, "Asset Library Manager");
            if(otherfold2)
            {
                if (GUILayout.Button("Prefab Labels", GUILayout.ExpandWidth(true)))
                    PrefabLabels.ShowWindow();
                if (GUILayout.Button("Prefab Viewer", GUILayout.ExpandWidth(true)))
                    PrefabViewer.ShowWindow();
                if (GUILayout.Button("Preview Window", GUILayout.ExpandWidth(true)))
                    Preview_Window.ShowWindow();
                if (GUILayout.Button("Labels Manager", GUILayout.ExpandWidth(true)))
                    LabelsManager.ShowWindow();
                if (GUILayout.Button("Settings", GUILayout.ExpandWidth(true)))
                    SelectSettingsObject.SelectSettings();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void Update()
    {
        if(Selection.activeTransform)
            SelectedObject = Selection.activeTransform.gameObject;
        else
            SelectedObject = null;
    }
}