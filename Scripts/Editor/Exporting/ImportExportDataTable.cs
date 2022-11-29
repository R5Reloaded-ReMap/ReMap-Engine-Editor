using UnityEngine;
using UnityEditor;
using System.Reflection;
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
        {
            string[] splitArray = File.ReadAllText(path).Split(char.Parse("\n"));

            //First get a list of collections
            List<String> collectionList = new List<String>();
            foreach (string item in splitArray) {
                string[] itemsplit = item.Replace("\"", "").Split(char.Parse(","));

                if (itemsplit.Length < 12)
                    continue;

                string collection = itemsplit[12].Replace("\"", "");

                if (collection == "None")
                    continue;

                if (!collectionList.Contains(collection))
                    collectionList.Add(collection);
            }

            //Create empty game objects for collection
            foreach (string col in collectionList) {
                GameObject objToSpawn = new GameObject(col);
                objToSpawn.name = col;
            }

            //Import datatable
            foreach (string item in splitArray)
            {
                string[] itemsplit = item.Replace("\"", "").Split(char.Parse(","));

                if (itemsplit.Length < 12)
                    continue;

                //Vars
                string Type = itemsplit[0];
                Vector3 Origin = new Vector3(float.Parse(itemsplit[2]), float.Parse(itemsplit[3].Replace(">\"", "")), -(float.Parse(itemsplit[1].Replace("\"<", ""))));
                Vector3 Angles = new Vector3(-(float.Parse(itemsplit[4].Replace("\"<", ""))), -(float.Parse(itemsplit[5])), float.Parse(itemsplit[6].Replace(">\"", "")));
                float Scale = float.Parse(itemsplit[7]);
                string Fade = itemsplit[8];
                string Mantle = itemsplit[9];
                string Visible = itemsplit[10];
                string Model = itemsplit[11].Replace("/", "#").Replace(".rmdl", "").Replace("\"", "").Replace("\n", "").Replace("\r", "");
                string Collection = itemsplit[12].Replace("\"", "");

                //Find Model GUID in Assets
                string[] results = AssetDatabase.FindAssets(Model);

                //If not found dont continue
                if (results.Length == 0)
                    continue;

                //Get model path from guid and load it
                UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;

                //If its null dont continue
                if (loadedPrefabResource == null)
                    continue;

                //Create new model in scene
                GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
                obj.transform.position = Origin;
                obj.transform.eulerAngles = Angles;
                obj.name = Model;
                obj.gameObject.transform.localScale = new Vector3(Scale, Scale, Scale);
                obj.SetActive(Visible == "true");

                PropScript script = obj.GetComponent<PropScript>();
                script.fadeDistance = float.Parse(Fade);
                script.allowMantle = Mantle == "true";

                if (Collection == "None")
                    continue;

                //Get the correct parrent gameobject
                GameObject parent = GameObject.Find(Collection);
                if (parent != null) //If its not null set it as a child
                    obj.gameObject.transform.parent = parent.transform;
            }
        }
    }

    [MenuItem("R5Reloaded/DataTable/Export DataTable", false, 100)]
    private static void ExportDataTable()
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        var path = EditorUtility.SaveFilePanel("Datatable Export", "", "mapexport.csv", "csv");
        if (path.Length != 0)
        {
            string tableCode = Build.Props(true, Build.BuildType.DataTable);

            System.IO.File.WriteAllText(path, tableCode);
        }
    }
}
