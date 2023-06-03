
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class FileWrite
{
    internal static string outputFolder = $"{UnityInfo.currentDirectoryPath}/Assets/ReMap/Resources/DeveloperOnly";
    internal static string output = $"{outputFolder}/WriteFile.txt";

    #if ReMapDev
        [ MenuItem( "ReMap Dev Tools/File Write Test", false, 100 ) ]
        public static async void Init()
        {
            if ( !Directory.Exists( outputFolder ) ) Directory.CreateDirectory( outputFolder );

            if ( !File.Exists( output ) ) File.Create( output );

            string file = await CodeViews.LiveMap.BuildScriptFile();

            File.WriteAllText( output, file );
        }
    #endif
}
