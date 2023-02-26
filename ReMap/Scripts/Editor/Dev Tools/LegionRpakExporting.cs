using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LegionRpakExporting : EditorWindow
{
    static string currentDirectory = Directory.GetCurrentDirectory().Replace("\\","/");
    static string relativeLegionPlus = $"Assets/ReMap/LegionPlus";
    static string relativeLegionPlusExportedFiles = $"Assets/ReMap/LegionPlus/exported_files";
    static string relativeRpakFile = AssetLibrarySorter.relativeRpakFile;
    static string relativeModel = AssetLibrarySorter.relativeModel;
    static string relativeMaterials = AssetLibrarySorter.relativeMaterials;

    #if ReMapDev
    [MenuItem("ReMap Dev Tools/Asset Library Sorter/Legion/Create All Rpak List", false, 100)]
    public static async void RpakListInit()
    {
        await CreateAllRpakList();
    }

    [MenuItem("ReMap Dev Tools/Asset Library Sorter/Legion/Export Rpak Models", false, 100)]
    public static async void RpakModelsInit()
    {
        await ExportModels();
        await DeleteUselessModels();
        await MoveModels();
    }
    #endif

    public static async Task CreateAllRpakList()
    {
        var path = EditorUtility.OpenFolderPanel("Rpak Folder", "", "");

        List<string> rpakFiles = new List<string>();

        foreach ( string rpakPath in Directory.GetFiles(path) )
        {
            if ( !IsValidRpakFile( rpakPath ) ) continue;

            rpakFiles.Add( rpakPath );
        }

        string[] rpakFilesValid = rpakFiles.ToArray();


        float progress = 0.0f;

        int fileIdx = 0; int fileTotalIdx = rpakFilesValid.Length;

        foreach (string rpakPath in rpakFilesValid)
        {
            string command = $"{currentDirectory}/{relativeLegionPlus}/LegionPlus.exe";
            string arguments = $"--list \"{rpakPath.Replace("\\","/")}\" --loadmodels --fullpath --nologfile --overwrite";

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = command;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;

            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                process.StartInfo = startInfo;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                // Wait for the process to exit
                await Task.Run(() => process.WaitForExit());

                // Update progress bar
                progress += 1.0f / fileTotalIdx;
                EditorUtility.DisplayProgressBar($"Parsing Rpak Files {fileIdx++}/{fileTotalIdx}", $"Processing {Path.GetFileName(rpakPath)}", progress);
            }

            string exportedFilePath = $"{currentDirectory}/{relativeLegionPlusExportedFiles}/lists/{Path.GetFileNameWithoutExtension(rpakPath)}.txt";

            if ( File.Exists( exportedFilePath ) )
            {
                string exportedFilePathMeta = $"{exportedFilePath}.meta";
                if ( new FileInfo( exportedFilePath ).Length == 0 )
                {
                    File.Delete( exportedFilePath );

                    if ( File.Exists( exportedFilePathMeta ) ) File.Delete( exportedFilePathMeta );
                }
                else
                {
                    // [TODO] merge in one file multiple rpaks ( rpak, rpak(01), rpak(02) ) either by a function
                    // or most simple would be that Legion supports multiple opening by command argument

                    //if ( Path.GetFileNameWithoutExtension(rpakPath).Contains("(") )

                    File.Move( $"{exportedFilePath}", $"{relativeRpakFile}/{Path.GetFileNameWithoutExtension(rpakPath).Replace("mp_rr_", "")}.txt" );
                    if ( File.Exists( exportedFilePathMeta ) ) File.Move( $"{exportedFilePath}.meta", $"{relativeRpakFile}/{Path.GetFileNameWithoutExtension(rpakPath).Replace("mp_rr_", "")}.txt.meta" );
                }
            }
        }

        EditorUtility.ClearProgressBar();
    }

    public static async Task ExportModels()
    {
        var path = EditorUtility.OpenFolderPanel("Rpak Folder", "", "");

        List<string> rpakFiles = new List<string>();

        foreach ( string rpakPath in Directory.GetFiles(path) )
        {
            if ( !IsValidRpakFile( rpakPath ) ) continue;

            rpakFiles.Add( rpakPath );
        }

        string[] rpakFilesValid = rpakFiles.ToArray();


        float progress = 0.0f;

        int fileIdx = 0; int fileTotalIdx = rpakFilesValid.Length;

        foreach (string rpakPath in rpakFilesValid)
        {
            string command = $"{currentDirectory}/{relativeLegionPlus}/LegionPlus.exe";
            string arguments = $"--export \"{rpakPath.Replace("\\","/")}\" --loadmodels --mdlfmt fbx --fullpath --nologfile --overwrite";

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = command;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;

            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                process.StartInfo = startInfo;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                // Wait for the process to exit
                await Task.Run(() => process.WaitForExit());

                // Update progress bar
                progress += 1.0f / fileTotalIdx;
                EditorUtility.DisplayProgressBar($"Extracting Models {fileIdx++}/{fileTotalIdx}", $"Processing {Path.GetFileName(rpakPath)}", progress);
            }
        }
    }

    public static async Task DeleteUselessModels()
    {
        string directorySearch = $"{currentDirectory}/{relativeLegionPlusExportedFiles}/models";

        string[] allFolders = Directory.GetDirectories( directorySearch );

        float progress = 0.0f; int folderIdx = 0; int folderTotalIdx = allFolders.Length;

        foreach ( string modelPath in allFolders )
        {
            string modelPathR = modelPath.Replace("\\", "/");

            if ( !IsValidModelForUnity( modelPathR ) )
            {
                Directory.Delete( modelPathR );

                if ( File.Exists( $"{modelPathR}.meta" ) ) File.Delete( $"{modelPathR}.meta" );
            }

            // Update progress bar
            progress += 1.0f / folderTotalIdx;
            EditorUtility.DisplayProgressBar($"Deleting Useless Models {folderIdx++}/{folderTotalIdx}", $"Checking {Path.GetFileName(modelPathR)}", progress);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
        }
    }

    public static async Task MoveModels()
    {
        string directorySearch = $"{currentDirectory}/{relativeLegionPlusExportedFiles}/models";

        string[] allLods = Directory.GetFiles( directorySearch, "*.fbx", SearchOption.AllDirectories );
        string[] allTextures = Directory.GetFiles( directorySearch, "*.dds", SearchOption.AllDirectories );

        float progress = 0.0f; int filesIdx = 0; int filesTotalIdx = allLods.Length;

        foreach ( string lodsPath in allLods )
        {
            string lodsPathR = lodsPath.Replace("\\", "/");

            File.Move( $"{lodsPathR}", $"{relativeModel}/{Path.GetFileName(lodsPathR)}" );

            // Update progress bar
            progress += 1.0f / filesTotalIdx;
            EditorUtility.DisplayProgressBar($"Deleting Useless Models {filesIdx++}/{filesTotalIdx}", $"Moving {Path.GetFileName(lodsPathR)}", progress);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
        }

        progress = 0.0f; filesIdx = 0; filesTotalIdx = allTextures.Length;

        foreach ( string texturesPath in allTextures )
        {
            string texturesPathR = texturesPath.Replace("\\", "/");

            File.Move( $"{texturesPathR}", $"{relativeMaterials}/{Path.GetFileName(texturesPathR)}" );

            // Update progress bar
            progress += 1.0f / filesTotalIdx;
            EditorUtility.DisplayProgressBar($"Deleting Useless Models {filesIdx++}/{filesTotalIdx}", $"Moving {Path.GetFileName(texturesPathR)}", progress);

            await Task.Delay(TimeSpan.FromSeconds(0.001));
        }
    }

    public static bool IsValidRpakFile( string rpakPath )
    {
        string fileName = Path.GetFileName(rpakPath);

        if ( Path.GetExtension(fileName) != ".rpak" )    return false;

        if ( fileName.Contains( "charm_" ) )             return false;
        if ( fileName.Contains( "gcard_" ) )             return false;
        if ( fileName.Contains( "loadscreen" ) )         return false;
        if ( fileName.Contains( "material_stickers_" ) ) return false;
        if ( fileName.Contains( "subtitles_" ) )         return false;

        return true;
    }

    public static bool IsValidModelForUnity( string modelPath )
    {
        string fileName = Path.GetFileName(modelPath);

        if ( fileName.Contains( "ptpov" ) ) return false;
        if ( fileName.Contains( "pov" ) )   return false;
        if ( fileName.Contains( "_pov" ) )  return false;

        return true;
    }
}
