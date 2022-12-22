using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ImportExportDataTable
{
    [MenuItem("ReMap/Import/DataTable", false, 50)]
    private async static void ImportDataTable()
    {
        string path = EditorUtility.OpenFilePanel("Datatable Import", "", "csv");
        if (path.Length == 0)
            return;

        string[] splitArray = File.ReadAllText(path).Split(char.Parse("\n"));

        List<String> collectionList = Helper.BuildCollectionList(splitArray);
        foreach (string col in collectionList)
        {
            GameObject objToSpawn = new GameObject(col);
            objToSpawn.name = col;
        }

        //Import datatable
        int i = 0;
        foreach (string item in splitArray)
        {
            string[] itemsplit = item.Replace("\"", "").Split(char.Parse(","));

            if (itemsplit.Length < 12)
                continue;

            EditorUtility.DisplayProgressBar("Importing DataTable", "Importing: " + itemsplit[11], (i + 1) / (float)splitArray.Length);

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

            await Task.Delay(TimeSpan.FromSeconds(0.001));
            i++;
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("ReMap/Export/DataTable", false, 51)]
    private static void ExportDataTable()
    {
        var path = EditorUtility.SaveFilePanel("Datatable Export", "", "mapexport.csv", "csv");
        if (path.Length == 0)
            return;

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        Helper.Is_Using_Starting_Offset = true;

        string tableCode = Build.Props(Build.BuildType.DataTable);

        System.IO.File.WriteAllText(path, tableCode);
    }
}
