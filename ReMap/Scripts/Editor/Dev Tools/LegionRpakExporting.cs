using DevLibrarySorter;
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
    static string relativeRpakFile = LSRelativePath.relativeRpakFile;

    #if ReMapDev
    [MenuItem("ReMap Dev Tools/Asset Library Sorter/Legion/Create All Rpak List", false, 100)]
    public static async void RpakListInit()
    {
        await CreateAllRpakList();
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
}
