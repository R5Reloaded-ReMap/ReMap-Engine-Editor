using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DevLibrarySorter
{
    public class LSRelativePath
    {
        public static string currentDirectory = Directory.GetCurrentDirectory().Replace("\\","/");
        public static string relativeEmptyPrefab = $"Assets/ReMap/Lods - Dont use these/EmptyPrefab.prefab";
        public static string relativeLods = $"Assets/ReMap/Lods - Dont use these";
        public static string relativeModel = $"Assets/ReMap/Lods - Dont use these/Models";
        public static string relativeMaterials = $"Assets/ReMap/Lods - Dont use these/Materials";
        public static string relativePrefabs = $"Assets/Prefabs";
        public static string relativeRpakFile = $"Assets/ReMap/Resources/rpakModelFile";
    }

    public class LSUtility
    {
        
    }
}
