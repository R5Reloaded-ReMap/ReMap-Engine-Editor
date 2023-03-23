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
    public async static void ImportDataTable()
    {
        string path = EditorUtility.OpenFilePanel("Datatable Import", "", "csv");
        if (path.Length == 0)
            return;

        ReMapConsole.Log("[Datatable Import] Reading file: " + path, ReMapConsole.LogType.Warning);
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

            ReMapConsole.Log("[Datatable Import] Importing: " + itemsplit[11], ReMapConsole.LogType.Info);
            EditorUtility.DisplayProgressBar("Importing DataTable", "Importing: " + itemsplit[11], (i + 1) / (float)splitArray.Length);

            //Build DataTable
            Helper.NewDataTable dt = Helper.BuildDataTable(item);

            ReMapConsole.Log("Origin: " + dt.Origin, ReMapConsole.LogType.Info);
            ReMapConsole.Log("Angles: " + dt.Angles, ReMapConsole.LogType.Info);
            ReMapConsole.Log("Scale: " + dt.Scale, ReMapConsole.LogType.Info);
            ReMapConsole.Log("Type: " + dt.Type, ReMapConsole.LogType.Info);
            ReMapConsole.Log("Collection: " + dt.Collection, ReMapConsole.LogType.Info);

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

        ReMapConsole.Log("[Datatable Import] Finished", ReMapConsole.LogType.Success);
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("ReMap/Export/DataTable", false, 51)]
    public static void ExportDataTable()
    {
        var path = EditorUtility.SaveFilePanel("Datatable Export", "", "mapexport.csv", "csv");
        if (path.Length == 0)
            return;

        ReMapConsole.Log("[Datatable Export] Starting Export", ReMapConsole.LogType.Warning);

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        Helper.Is_Using_Starting_Offset = true;

        string tableCode = Build_.Props( null, Build_.BuildType.DataTable, true );
        ReMapConsole.Log("[Datatable Export] Writing to file: " + path, ReMapConsole.LogType.Warning);

        System.IO.File.WriteAllText(path, tableCode);

        ReMapConsole.Log("[Datatable Export] Finished", ReMapConsole.LogType.Success);
    }
}
