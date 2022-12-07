using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class MapExport
{

    [MenuItem("R5Reloaded/Export Map/Export With Origin Offset/Whole Script", false, 100)]
    private static void ExportWholeScriptOffset()
    {
        ExportMap(Helper.ExportType.WholeScriptOffset);
    }

    [MenuItem("R5Reloaded/Export Map/Export With Origin Offset/Map Only", false, 100)]
    private static void ExportMapOnlyOffset()
    {
        ExportMap(Helper.ExportType.MapOnlyOffset);
    }

    [MenuItem("R5Reloaded/Export Map/Export Without Origin Offset/Whole Script", false, 100)]
    private static void ExportWholeScript()
    {
        ExportMap(Helper.ExportType.WholeScript);
    }

    [MenuItem("R5Reloaded/Export Map/Export Without Origin Offset/Map Only", false, 100)]
    private static void ExportOnlyMap()
    {
        ExportMap(Helper.ExportType.MapOnly);
    }

    [MenuItem("R5Reloaded/Export Map/Export script.ent Code", false, 100)]
    private static void ExportScriptEntCode()
    {
        var path = EditorUtility.SaveFilePanel("Map Export", "", "scriptentexport.txt", "txt");
        if (path.Length == 0)
            return;

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        Helper.Is_Using_Starting_Offset = true;
        string mapcode = Build.Props(Build.BuildType.Ent);

        System.IO.File.WriteAllText(path, mapcode);

    }


    /// <summary>
    /// Exports map code
    /// </summary>
    private static void ExportMap(Helper.ExportType type)
    {
        var path = EditorUtility.SaveFilePanel("Map Export", "", "mapexport.txt", "txt");
        if (path.Length == 0)
            return;

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        Helper.Is_Using_Starting_Offset = false;
        if (type == Helper.ExportType.MapOnlyOffset || type == Helper.ExportType.WholeScriptOffset)
            Helper.Is_Using_Starting_Offset = true;

        string mapcode = Helper.Credits + "\n" + $"void function {SceneManager.GetActiveScene().name.Replace(" ", "_")}()" + "\n{\n" + Helper.ShouldAddStartingOrg(1);

        if (type == Helper.ExportType.MapOnly || type == Helper.ExportType.MapOnlyOffset)
            mapcode = Helper.ShouldAddStartingOrg(1);

        //Build Map Code
        mapcode += Helper.BuildMapCode();

        if (type == Helper.ExportType.WholeScript || type == Helper.ExportType.WholeScriptOffset)
            mapcode += "}";

        System.IO.File.WriteAllText(path, mapcode);
    }
} 