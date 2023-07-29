using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class LegionExporting
    {
        internal static string ValidPathArg = "";

        private static readonly string[] ValidRpakFile = new []
        {
            "common_mp", "common_early", "root_lgnd_", "startup", "mp_lobby", "mp_rr_"
        };

        public static async void RpakListInit()
        {
            await CreateAllRpakList();
        }

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

            foreach ( string rpakPath in rpakFilesValid )
            {
                string command = UnityInfo.relativePathLegionExecutive;
                string arguments = $"--list \"{rpakPath.Replace("\\","/")}\" --loadmodels --fullpath --nologfile --overwrite";

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = command;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = false;

                using ( Process process = new Process())
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

                string exportedFilePath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathLegionPlusExportedFiles}/lists/{Path.GetFileNameWithoutExtension(rpakPath)}.txt";

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

                        File.Move( $"{exportedFilePath}", $"{UnityInfo.relativePathRpakManager}/{Path.GetFileNameWithoutExtension(rpakPath).Replace("mp_rr_", "")}.txt" );
                        if ( File.Exists( exportedFilePathMeta ) ) File.Move( $"{exportedFilePath}.meta", $"{UnityInfo.relativePathRpakManager}/{Path.GetFileNameWithoutExtension(rpakPath).Replace("mp_rr_", "")}.txt.meta" );
                    }
                }

                Directory.Delete( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathLegionPlusExportedFiles}" );
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

        public static async Task ExtractModelFromLegion( string assetList )
        {
            if ( string.IsNullOrEmpty( ValidPathArg ) || string.IsNullOrEmpty( assetList ) ) return;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = $"\"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathLegionExecutive}\"";
            startInfo.Arguments = $"--loadPaks \"{ValidPathArg}\" --assetsToExport \"{assetList}\" --loadmodels --loadmaterials --mdlfmt fbx --imgfmt dds --nologfile";

            startInfo.UseShellExecute = false;

            using ( Process process = new Process() )
            {
                process.StartInfo = startInfo;
                process.Start();

                await Task.Run( () => process.WaitForExit() );
            }
        }

        internal static bool GetValidRpakPaths()
        {
            ValidPathArg = "";

            var rpakRetailPath = EditorUtility.OpenFolderPanel( "Rpak Path", "Select rpak folder", "" );

            if ( string.IsNullOrEmpty( rpakRetailPath ) ) return false;

            foreach ( string files in Directory.GetFiles( rpakRetailPath ) )
            {
                string file = Path.GetFileName( files );

                foreach ( string match in ValidRpakFile )
                {
                    if ( file.Contains( match ) && Path.GetExtension( file ) == ".rpak" && !file.Contains( "_loadscreen" ) ) // Ignore loadscreens
                    {
                        ValidPathArg += $"{rpakRetailPath}/{file},";
                    }
                }
            }

            ValidPathArg = ValidPathArg.TrimEnd( ',' );

            return string.IsNullOrEmpty( ValidPathArg );
        }
    }
}
