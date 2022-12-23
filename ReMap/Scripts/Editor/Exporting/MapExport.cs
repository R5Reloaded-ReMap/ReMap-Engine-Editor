using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class MapExport
{

    [MenuItem("ReMap/Export/Map With Origin Offset/Whole Script", false, 50)]
    private static void ExportWholeScriptOffset()
    {
        ExportMap(Helper.ExportType.WholeScriptOffset);
    }

    [MenuItem("ReMap/Export/Map With Origin Offset/Map Only", false, 50)]
    private static void ExportMapOnlyOffset()
    {
        ExportMap(Helper.ExportType.MapOnlyOffset);
    }

    [MenuItem("ReMap/Export/Map Without Origin Offset/Whole Script", false, 50)]
    private static void ExportWholeScript()
    {
        ExportMap(Helper.ExportType.WholeScript);
    }

    [MenuItem("ReMap/Export/Map Without Origin Offset/Map Only", false, 50)]
    private static void ExportOnlyMap()
    {
        ExportMap(Helper.ExportType.MapOnly);
    }

    [MenuItem("ReMap/Export/script.ent", false, 50)]
    private static void ExportScriptEntCode()
    {
        var path = EditorUtility.SaveFilePanel("Script.ent", "", "scriptentexport.ent", "ent");
        if (path.Length == 0)
            return;

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        Helper.Is_Using_Starting_Offset = true;
        string entCode = Build.Props(Build.BuildType.Ent);

        System.IO.File.WriteAllText(path, entCode + "\u0000");
    }

    [MenuItem("ReMap/Export/sound.ent", false, 50)]
    private static void ExportSoundEntCode()
    {
        var path = EditorUtility.SaveFilePanel("Sound.ent Export", "", "soundentexport.ent", "ent");
        if (path.Length == 0)
            return;

        Helper.FixPropTags();
        EditorSceneManager.SaveOpenScenes();

        string entCode = Build.Sounds();

        System.IO.File.WriteAllText(path, entCode + "\u0000");
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