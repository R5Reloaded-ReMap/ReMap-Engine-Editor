
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using LibrarySorter;

public class UnityInfo
{
    public static readonly string ReMapVersion = "Version 1.1.0";
    public static readonly string JsonVersion = "1.0.7";

    // Path Utility
    public static readonly string currentDirectoryPath =                 Directory.GetCurrentDirectory().Replace("\\","/");
    public static readonly string relativePathLods =                     $"Assets/ReMap/Lods - Dont use these";
    public static readonly string relativePathLodsUtility =              $"{relativePathLods}/Utility";
    public static readonly string relativePathEmptyPrefab =              $"{relativePathLodsUtility}/EmptyPrefab.prefab";
    public static readonly string relativePathCubePrefab =               $"{relativePathLodsUtility}/Cube.prefab";
    public static readonly string relativePathModel =                    $"{relativePathLods}/Models";
    public static readonly string relativePathMaterials =                $"{relativePathLods}/Materials";
    public static readonly string relativePathDevLods =                  $"{relativePathLods}/Developer_Lods";
    public static readonly string relativePathPrefabs =                  $"Assets/Prefabs";
    public static readonly string relativePathResources =                $"Assets/ReMap/Resources";
    public static readonly string relativePathAdditionalCode =           $"{relativePathResources}/AdditionalCode";
    public static readonly string relativePathAdditionalCodeJson =       $"{relativePathAdditionalCode}/additionalCode.json";
    public static readonly string relativePathAdditionalCodeInfo =       $"{relativePathAdditionalCode}/additionalCodeInfo.txt";
    public static readonly string relativePathRpakManager =              $"{relativePathResources}/RpakManager";
    public static readonly string relativePathRpakManagerList =          $"{relativePathRpakManager}/rpakManagerList.json";
    public static readonly string relativePathJsonOffset =               $"{relativePathRpakManager}/prefabOffsetList.json";
    public static readonly string relativePathTextureData =              $"{relativePathResources}/TextureData";
    public static readonly string relativePathTextureDataList =          $"{relativePathTextureData}/textureData.json";
    public static readonly string relativePathLegionPlus =               $"Assets/ReMap/LegionPlus";
    public static readonly string relativePathLegionPlusExportedFiles =  $"{relativePathLegionPlus}/exported_files";
    public static readonly string relativePathLegionExecutive =          $"{relativePathLegionPlus}/LegionPlus.exe";
    public static readonly string relativePathNVIDIA =                   $"Assets/ReMap/NVIDIA";
    public static readonly string relativePathNVTTEExecutive =           $"{relativePathNVIDIA}/nvtt_export.exe";
    public static readonly string relativePathTexConv =                  $"{relativePathNVIDIA}/texconv.exe";
    public static readonly string relativePathR5RPlayerInfo =            "\\platform\\scripts\\player_info.txt";
    public static readonly string relativePathR5RScripts =               "\\platform\\scripts\\vscripts\\mp\\levels\\mp_rr_remap.nut";

    public static readonly string relativeRMAPDEVfolder =                $"{relativePathResources}/DeveloperOnly";


    /// <summary>
    /// Gets total GameObject in scene
    /// </summary>
    /// <returns></returns>
    public static GameObject[] GetAllGameObjectInScene( bool selection = false )
    {
        return selection ? Selection.gameObjects : UnityEngine.Object.FindObjectsOfType< GameObject >();
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
    /// Returns the model name as a prefab
    /// </summary>
    /// <returns></returns>
    public static string GetUnityModelName( string modelName, bool extension = false )
    {
        string ext = extension ? ".prefab" : "";
        modelName = modelName.Replace( '#', '/' ).Replace( ".rmdl", "" ).Replace( ".prefab", "" );
        if ( modelName.IndexOf( "mdl/" ) != -1 ) modelName = modelName.Substring( modelName.IndexOf( "mdl/" ) );
        return Helper.DeleteNewLine( modelName.Replace( '/', '#' ) ) + ext;
    }

    public static string GetUnityModelName( GameObject go, bool extension = false )
    {
        return GetUnityModelName( go.name, extension );
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
        return Helper.DeleteNewLine( modelName.Substring( modelName.IndexOf( "mdl/" ) ) ) + ext;
    }

    public static string GetApexModelName( GameObject go, bool extension = false )
    {
        return GetApexModelName( go.name, extension );
    }

    /// <summary>
    /// Printt a string in editor console
    /// </summary>
    /// <returns></returns>
    public static void Printt( string str )
    {
        UnityEngine.Debug.Log( str );
    }

    public static string GetObjName( string name )
    {
        return name.Split( char.Parse( " " ) )[0];
    }

    public static string GetObjName( GameObject obj )
    {
        return obj.name.Split( char.Parse( " " ) )[0];
    }

    public static void SortListByKey< T, TKey >( List< T > list, Func< T, TKey > keySelector ) where TKey : IComparable
    {
        list.Sort( ( x, y ) => keySelector( x ).CompareTo( keySelector( y ) ) );
    }

    public static UnityEngine.Object FindPrefabFromName( string name )
    {
        // Hack so that the models named at the end with "(number)" still work
        if( name.Contains( " " ) ) name = name.Split( " " )[0];

        //Find Model GUID in Assets
        string[] results = AssetDatabase.FindAssets( name, new [] { $"{UnityInfo.relativePathPrefabs}/{RpakManagerWindow.allModelsDataName}", $"{UnityInfo.relativePathPrefabs}/{RpakManagerWindow.allModelsRetailDataName}" } );
        if ( results.Length == 0 ) return null;

        //Get model path from guid and load it
        UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof( UnityEngine.Object ) ) as GameObject;
        return loadedPrefabResource;
    }

    public static bool PrefabExist( string name )
    {
        return AssetDatabase.FindAssets( name, new [] {$"{UnityInfo.relativePathPrefabs}"} ).Any( guid => AssetDatabase.GUIDToAssetPath( guid ).EndsWith( name + ".prefab" ) );
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
