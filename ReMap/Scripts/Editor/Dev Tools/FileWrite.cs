
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ReMapDebug
{
    internal static string outputFolder = $"{UnityInfo.currentDirectoryPath}/Assets/ReMap/Resources/DeveloperOnly";
    internal static string output = $"{outputFolder}/WriteFile.txt";

    #if ReMapDev
        [ MenuItem( "ReMap Dev Tools/File Write Test", false, 100 ) ]
        public static async void Debug_FileWrite()
        {
            if ( !Directory.Exists( outputFolder ) ) Directory.CreateDirectory( outputFolder );

            if ( !File.Exists( output ) ) File.Create( output );

            string file = await CodeViews.LiveMap.BuildScriptFile();

            File.WriteAllText( output, file );
        }

        [ MenuItem( "ReMap Dev Tools/Clear Progress Bar", false, 100 ) ]
        public static void Debug_ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }
    #endif
}
