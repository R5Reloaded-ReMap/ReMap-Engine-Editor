using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UnityInfo
{
    public static string ReMapVersion = "Version 1.0";
    public static string currentDirectory = LibrarySorterWindow.currentDirectory;
    public static string relativeRpakFile = LibrarySorterWindow.relativeRpakFile;


    /// <summary>
    /// Gets total GameObject in scene
    /// </summary>
    /// <returns></returns>
    public static GameObject[] GetAllGameObjectInScene()
    {
        return UnityEngine.Object.FindObjectsOfType< GameObject >();
    }

    /// <summary>
    /// Gets Total Count of all objects in scene
    /// </summary>
    /// <returns></returns>
    public static int GetAllCount()
    {
        int objectCount = 0;

        foreach (GameObject go in GetAllGameObjectInScene())
        {
            foreach ( string key in Helper.ObjectToTag.Keys )
            {
                if ( go.name.Contains( key ) ) objectCount++;
            }
        }

        return objectCount;
    }

    /// <summary>
    /// Gets total count of a specific object in scene
    /// </summary>
    /// <returns></returns>
    public static int GetSpecificObjectCount( ObjectType objectType )
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag( Helper.GetObjTagNameWithEnum( objectType ) );

        if ( objectType == ObjectType.ZipLine || objectType == ObjectType.LinkedZipline || objectType == ObjectType.VerticalZipLine || objectType == ObjectType.NonVerticalZipLine )
            return PropObjects.Length * 2;

        return PropObjects.Length;
    }

    /// <summary>
    /// Get all the models name in the active scene
    /// </summary>
    /// <returns></returns>
    public static string[] GetModelsListInScene()
    {
        List<string> modelsInScene = new List<string>();

        foreach ( GameObject go in GetAllGameObjectInScene() )
        {
            if ( go.name.Contains( "mdl#" ) && !modelsInScene.Contains( go.name ) )
                modelsInScene.Add( go.name );
        }

        modelsInScene.Sort();

        return modelsInScene.ToArray();
    }

    /// <summary>
    /// Get all rpak list in /ReMap/Resources/rpakModelFile
    /// </summary>
    /// <returns></returns>
    public static string[] GetAllRpakModelsFile( bool includeAllModelFile = false, bool returnFileName = false )
    {
        string[] filePaths = Directory.GetFiles($"{currentDirectory}/{relativeRpakFile}", "*.txt", SearchOption.TopDirectoryOnly).Where( f => IsNotExcludedFile( f, includeAllModelFile ) ).ToArray();

        // Return path
        if ( !returnFileName ) return filePaths;

        // Return file name
        List<string> fileNames = new List<string>();

        foreach ( string filePath in filePaths )
        {
            fileNames.Add( Path.GetFileNameWithoutExtension( filePath ) );
        }

        return fileNames.ToArray();
    }

    /// <summary>
    /// Returns the model name as a prefab
    /// </summary>
    /// <returns></returns>
    public static string GetUnityModelName( string modelName, bool extension = false )
    {
        string ext = extension ? ".prefab" : "";
        modelName = modelName.Replace( '#', '/' ).Replace( ".rmdl", "" ).Replace( ".prefab", "" );
        return modelName.Substring( modelName.IndexOf( "mdl/" ) ).Replace( '/', '#' ) + ext;
    }

    /// <summary>
    /// Returns the model name as a Apex path
    /// </summary>
    /// <returns></returns>
    public static string GetApexModelName( string modelName, bool extension = false )
    {
        string ext = extension ? ".rmdl" : "";
        modelName = modelName.Replace( '#', '/' ).Replace( ".rmdl", "" ).Replace( ".prefab", "" );
        if ( modelName.IndexOf( "mdl/" ) == -1 ) modelName = "mdl/" + modelName;
        return modelName.Substring( modelName.IndexOf( "mdl/" ) ) + ext;
    }

    /// <summary>
    /// Printt a string in editor console
    /// </summary>
    /// <returns></returns>
    public static void Printt( string str )
    {
        UnityEngine.Debug.Log( str );
    }

    public static string GetObjName( GameObject obj )
    {
        return obj.name.Split( char.Parse( " " ) )[0];
    }

    public static void SortListByKey<T, TKey>(List<T> list, Func<T, TKey> keySelector) where TKey : IComparable
    {
        list.Sort((x, y) => keySelector(x).CompareTo(keySelector(y)));
    }

    public static UnityEngine.Object FindPrefabFromName(string name)
    {
        // Hack so that the models named at the end with "(number)" still work
        if(name.Contains(" "))
            name = name.Split(" ")[0];

        //Find Model GUID in Assets
        string[] results = AssetDatabase.FindAssets(name);
        if (results.Length == 0)
            return null;

        //Get model path from guid and load it
        UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
        return loadedPrefabResource;
    }

    private static bool IsNotExcludedFile( string filePath, bool includeAllModelFile )
    {
        string fileName = Path.GetFileName(filePath);
        string[] excludedFiles;

        if ( includeAllModelFile )
        {   excludedFiles = new string[] { "lastestFolderUpdate.txt", "all_models.txt" }; }
        else
        { excludedFiles = new string[] { "lastestFolderUpdate.txt" }; }

        return !excludedFiles.Contains(fileName);
    }

    /// <summary>
    /// Example
    /// </summary>
    /// <returns></returns>
    //public static int GetCount()
    //{
    //    GameObject[] Objects = GameObject.FindGameObjectsWithTag("");
    //    return Objects.Length;
    //}
}
