using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DevLibrarySorter
{
    /// <summary>
    /// Get relative paths for the editor.
    /// </summary>
    public class LSRelativePath
    {
        /// <summary>
        /// Directory.GetCurrentDirectory().Replace("\\","/");
        /// </summary>
        public static string currentDirectory = Directory.GetCurrentDirectory().Replace("\\","/");

        /// <summary>
        /// $"Assets/ReMap/Lods - Dont use these/EmptyPrefab.prefab";
        /// </summary>
        public static string relativeEmptyPrefab = $"Assets/ReMap/Lods - Dont use these/EmptyPrefab.prefab";

        /// <summary>
        /// $"Assets/ReMap/Lods - Dont use these";
        /// </summary>
        public static string relativeLods = $"Assets/ReMap/Lods - Dont use these";

        /// <summary>
        /// $"Assets/ReMap/Lods - Dont use these/Models";
        /// </summary>
        public static string relativeModel = $"Assets/ReMap/Lods - Dont use these/Models";

        /// <summary>
        /// $"Assets/ReMap/Lods - Dont use these/Materials";
        /// </summary>
        public static string relativeMaterials = $"Assets/ReMap/Lods - Dont use these/Materials";

        /// <summary>
        /// $"Assets/Prefabs";
        /// </summary>
        public static string relativePrefabs = $"Assets/Prefabs";

        /// <summary>
        /// $"Assets/ReMap/Resources/rpakModelFile";
        /// </summary>
        public static string relativeRpakFile = $"Assets/ReMap/Resources/rpakModelFile";
    }

    /// <summary>
    /// Library Sorter Utility function.
    /// </summary>
    public class LSUtility
    {
        static string currentDirectory = LSRelativePath.currentDirectory;
        static string relativeEmptyPrefab = LSRelativePath.relativeEmptyPrefab;
        static string relativeLods = LSRelativePath.relativeLods;
        static string relativeModel = LSRelativePath.relativeModel;
        static string relativeMaterials = LSRelativePath.relativeMaterials;
        static string relativePrefabs = LSRelativePath.relativePrefabs;
        static string relativeRpakFile = LSRelativePath.relativeRpakFile;

        public static string[] protectedFolders = { "_custom_prefabs" };

        
    }
}
