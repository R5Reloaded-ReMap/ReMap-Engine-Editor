using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Collections.Generic;

public class ImportExportDataTable
{
    [MenuItem("R5Reloaded/DataTable/Import DataTable", false, 100)]
    private static void ImportDataTable()
    {
        string path = EditorUtility.OpenFilePanel("Datatable Import", "", "csv");
        if (path.Length != 0)
            return;

        string[] splitArray = File.ReadAllText(path).Split(char.Parse("\n"));

        List<String> collectionList = Helper.BuildCollectionList(splitArray);
        foreach (string col in collectionList)
        {
            GameObject objToSpawn = new GameObject(col);
            objToSpawn.name = col;
        }

        //Import datatable
        foreach (string item in splitArray)
        {
            string[] itemsplit = item.Replace("\"", "").Split(char.Parse(","));

            if (itemsplit.Length < 12)
                continue;

            //Build DataTable
            Helper.NewDataTable dt = Helper.BuildDataTable(item);

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets(dt.Model);
            if (results.Length == 0)
                continue;

            //Get model path from guid and load it
            UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
                continue;

            //Create new model in scene
            Helper.CreateDataTableItem(dt, loadedPrefabResource);
        }
    }

    [MenuItem("R5Reloaded/DataTable/Export DataTable", false, 100)]
    private static void ExportDataTable()
    {
        var path = EditorUtility.SaveFilePanel("Datatable Export", "", "mapexport.csv", "csv");
        if (path.Length != 0)
            return;

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        Helper.Is_Using_Starting_Offset = true;

        string tableCode = Build.Props(Build.BuildType.DataTable);

        System.IO.File.WriteAllText(path, tableCode);
    }
}
