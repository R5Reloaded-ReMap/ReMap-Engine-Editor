using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class MapExport
{
    static bool UseStartingOffset = false;
    static string StartFunction = "";

    [MenuItem("R5Reloaded/Text File/Export With Origin Offset/Whole Script", false, 100)]
    private static void ExportMapScript()
    {
        ExportMap(false, true);
    }

    [MenuItem("R5Reloaded/Text File/Export With Origin Offset/Map Only", false, 100)]
    private static void ExportOnlyMap()
    {
        ExportMap(true, true);
    }

    [MenuItem("R5Reloaded/Text File/Export Without Origin Offset/Whole Script", false, 100)]
    private static void ExportMapScript2()
    {
        ExportMap(false, false);
    }

    [MenuItem("R5Reloaded/Text File/Export Without Origin Offset/Map Only", false, 100)]
    private static void ExportOnlyMap2()
    {
        ExportMap(true, false);
    }

    [MenuItem("R5Reloaded/Text File/Export script.ent Code", false, 100)]
    private static void ExportScriptEntCode()
    {
        ExportScriptEnt();
    }

    /// <summary>
    /// Exports script ent code
    /// </summary>
    static void ExportScriptEnt()
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        var path = EditorUtility.SaveFilePanel( "Map Export", "", "scriptentexport.txt", "txt");
        if (path.Length != 0)
        {
            string mapcode = "";

            //Generate All Props
            GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
            foreach(GameObject go in PropObjects) {
                if (!go.activeInHierarchy)
                    continue;

                mapcode += Helper.BuildScriptEnt(go);
            }
            
            System.IO.File.WriteAllText(path, mapcode);
        }
        else
        {
            Debug.Log("Failed: Didnt pick a save path!");
        }
    }


    /// <summary>
    /// Exports map code
    /// </summary>
    /// <param name="onlymap"></param>
    /// <param name="offset"></param>
    private static void ExportMap(bool onlymap, bool offset = false)
    {
        UseStartingOffset = offset;

        BuildStartingString();
        Helper.FixPropTags();

        EditorSceneManager.SaveOpenScenes();

        var path = EditorUtility.SaveFilePanel( "Map Export", "", "mapexport.txt", "txt");
        if (path.Length != 0)
        {
            string mapcode = Helper.Credits + "\n" + StartFunction +  Helper.ShouldAddStartingOrg(UseStartingOffset, 1);

            if(onlymap)
                mapcode = Helper.ShouldAddStartingOrg(UseStartingOffset, 1);

            //Build Map Code
            mapcode += Helper.BuildMapCode(UseStartingOffset);

            if(!onlymap)
                mapcode += "}";

            System.IO.File.WriteAllText(path, mapcode);
        }
        else
        {
            Debug.Log("Failed: Didnt pick a save path!");
        }
    }

    /// <summary>
    /// Builds map starting function
    /// </summary>
    private static void BuildStartingString()
    {
        Scene scene = SceneManager.GetActiveScene();
        StartFunction = @"void function " + scene.name.Replace(" ", "_") + "()" + "\n" + "{" + "\n";
    }
} 