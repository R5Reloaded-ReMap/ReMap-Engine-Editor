using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Collections.Generic;

public class ImportDataTable
{
    [MenuItem("R5Reloaded/DataTable/Import DataTable", false, 100)]
    private static void ImportScript()
    {
        ImportData();
    }

    [MenuItem("R5Reloaded/DataTable/Export DataTable", false, 100)]
    private static void ExportScript()
    {
        ExportDataTable();
    }

    /// <summary>
    /// Imports datatable
    /// </summary>
    private static void ImportData()
    {
        string path = EditorUtility.OpenFilePanel("Datatable Import", "", "csv");

        if (path.Length != 0)
        {
            string contents = File.ReadAllText(path);

            string[] splitArray = contents.Split(char.Parse("\n"));

            //First get a list of collections
            List<String> collectionList = new List<String>();
            foreach (string item in splitArray)
            {
                string cleaneditem = item.Replace("\"", "");
                string[] itemsplit = item.Split(char.Parse(","));

                if (itemsplit.Length < 12)
                    continue;

                string collection = itemsplit[12].Replace("\"", "");

                if (collection == "None")
                    continue;

                if (!collectionList.Contains(collection))
                    collectionList.Add(collection);
            }

            //Create empty game objects for collection
            foreach (string col in collectionList)
            {
                GameObject objToSpawn = new GameObject(col);
                objToSpawn.name = col;
            }

            //Import datatable
            foreach (string item in splitArray)
            {
                string cleaneditem = item.Replace("\"", "");
                string[] itemsplit = item.Split(char.Parse(","));

                if (itemsplit.Length < 12)
                    continue;

                //Not Used
                string type = itemsplit[0];

                //Origin
                float orgx = float.Parse(itemsplit[1].Replace("\"<", ""));
                float orgy = float.Parse(itemsplit[2]);
                float orgz = float.Parse(itemsplit[3].Replace(">\"", ""));
                Vector3 org = new Vector3(orgy, orgz, -orgx);

                //Angles
                float angx = float.Parse(itemsplit[4].Replace("\"<", ""));
                float angy = float.Parse(itemsplit[5]);
                float angz = float.Parse(itemsplit[6].Replace(">\"", ""));
                Vector3 ang = new Vector3(-angx, -angy, angz);

                //Other
                float scale = float.Parse(itemsplit[7]);
                string fade = itemsplit[8];
                string mantle = itemsplit[9];
                string visible = itemsplit[10];

                //Model
                string mdl = itemsplit[11].Replace("/", "#").Replace(".rmdl", "").Replace("\"", "").Replace("\n", "").Replace("\r", "");

                //Collection
                string collection = itemsplit[12].Replace("\"", "");


                //Find Model GUID in Assets
                string[] results;
                results = AssetDatabase.FindAssets(mdl);

                //If not found dont continue
                if (results.Length == 0)
                    continue;


                //Get model path from guid and load it
                string prefabpath = AssetDatabase.GUIDToAssetPath(results[0]);
                UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(prefabpath, typeof(UnityEngine.Object)) as GameObject;

                //If its null dont continue
                if (loadedPrefabResource == null)
                    continue;

                //Create new model in scene
                GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;

                obj.transform.position = org;
                obj.transform.eulerAngles = ang;
                obj.name = mdl;
                obj.gameObject.transform.localScale = new Vector3(scale, scale, scale);
                obj.SetActive(visible == "true");

                PropScript script = obj.GetComponent<PropScript>();

                script.fadeDistance = float.Parse(fade);
                script.allowMantle = mantle == "true";

                if (collection == "None")
                    continue;

                //Get the correct parrent gameobject
                GameObject parent = GameObject.Find(collection);
                if (parent != null) //If its not null set it as a child
                    obj.gameObject.transform.parent = parent.transform;
            }
        }
    }

    /// <summary>
    /// Exports Datatable
    /// </summary>
    private static void ExportDataTable()
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        var path = EditorUtility.SaveFilePanel("Datatable Export", "", "mapexport.csv", "csv");
        if (path.Length != 0)
        {
            //Generate All Props
            GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");

            string saved = "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"" + "\n";

            foreach (GameObject go in PropObjects)
            {
                saved += Helper.BuildDataTableItem(go);
            }

            saved += "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"";

            System.IO.File.WriteAllText(path, saved);
        }
    }
}
