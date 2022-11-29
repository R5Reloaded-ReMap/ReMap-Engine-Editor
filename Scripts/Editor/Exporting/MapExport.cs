using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class MapExport
{
    enum ExportType
    {
        WholeScriptOffset,
        MapOnlyOffset,
        WholeScript,
        MapOnly
    }

    [MenuItem("R5Reloaded/Export Map/Export With Origin Offset/Whole Script", false, 100)]
    private static void ExportWholeScriptOffset()
    {
        ExportMap(ExportType.WholeScriptOffset);
    }

    [MenuItem("R5Reloaded/Export Map/Export With Origin Offset/Map Only", false, 100)]
    private static void ExportMapOnlyOffset()
    {
        ExportMap(ExportType.MapOnlyOffset);
    }

    [MenuItem("R5Reloaded/Export Map/Export Without Origin Offset/Whole Script", false, 100)]
    private static void ExportWholeScript()
    {
        ExportMap(ExportType.WholeScript);
    }

    [MenuItem("R5Reloaded/Export Map/Export Without Origin Offset/Map Only", false, 100)]
    private static void ExportOnlyMap()
    {
        ExportMap(ExportType.MapOnly);
    }

    [MenuItem("R5Reloaded/Export Map/Export script.ent Code", false, 100)]
    private static void ExportScriptEntCode()
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        var path = EditorUtility.SaveFilePanel( "Map Export", "", "scriptentexport.txt", "txt");
        if (path.Length != 0)
        {
            string mapcode = Build.Props(true, Build.BuildType.Ent);

            System.IO.File.WriteAllText(path, mapcode);
        }
    }


    /// <summary>
    /// Exports map code
    /// </summary>
    private static void ExportMap(ExportType type)
    {
        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        bool UseStartingOffset = false;
        if(type == ExportType.MapOnlyOffset || type == ExportType.WholeScriptOffset)
            UseStartingOffset = true;

        var path = EditorUtility.SaveFilePanel( "Map Export", "", "mapexport.txt", "txt");
        if (path.Length != 0)
        {
            string mapcode = Helper.Credits + "\n" + $"void function {SceneManager.GetActiveScene().name.Replace(" ", "_")}()" + "\n{\n" +  Helper.ShouldAddStartingOrg(UseStartingOffset, 1);

            if(type == ExportType.MapOnly || type == ExportType.MapOnlyOffset)
                mapcode = Helper.ShouldAddStartingOrg(UseStartingOffset, 1);

            //Build Map Code
            mapcode += Helper.BuildMapCode(UseStartingOffset);

            if(type == ExportType.WholeScript || type == ExportType.WholeScriptOffset)
                mapcode += "}";

            System.IO.File.WriteAllText(path, mapcode);
        }
    }
} 