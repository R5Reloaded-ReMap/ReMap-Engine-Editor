using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class AssetLibrarySorter
{
    public static void LibrarySorter()
    {
        int totalFiles = 0;
        Array[] totalArrayMap = new Array[10];
        string[] files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), @"Assets\Scripts\Editor\Helper Classes", "TextFile"), "*.txt");

        foreach (string file in files)
        {
            string[] arrayMap = new string[2000];

            string mapPath = file;

            int row = 0;

            using (StreamReader reader = new StreamReader(mapPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    arrayMap[row] = line;
                    row++;
                }
            }

            totalArrayMap[totalFiles] = arrayMap;
            totalFiles++;

            //if ( !Directory.Exists( Path.Combine(Directory.GetCurrentDirectory(), "Assets/Prefabs", file.Replace(".txt", "")) ) )
                Directory.CreateDirectory( Path.Combine(Directory.GetCurrentDirectory(), "Assets/Prefabs", Path.GetFileName(file) ) );
                
        }

        //foreach ()
    }
}