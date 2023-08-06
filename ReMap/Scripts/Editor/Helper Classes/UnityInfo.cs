using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UnityInfo
{
    public static string ReMapVersion = "Version 1.1.0";
    public static string JsonVersion = "1.0.7";

    // Path Utility
    public static string currentDirectoryPath =                 Directory.GetCurrentDirectory().Replace("\\","/");
    public static string relativePathLods =                     $"Assets/ReMap/Lods - Dont use these";
    public static string relativePathLodsUtility =              $"{relativePathLods}/Utility";
    public static string relativePathEmptyPrefab =              $"{relativePathLodsUtility}/EmptyPrefab.prefab";
    public static string relativePathCubePrefab =               $"{relativePathLodsUtility}/Cube.prefab";
    public static string relativePathModel =                    $"{relativePathLods}/Models";
    public static string relativePathMaterials =                $"{relativePathLods}/Materials";
    public static string relativePathDevLods =                  $"{relativePathLods}/Developer_Lods";
    public static string relativePathPrefabs =                  $"Assets/Prefabs";
    public static string relativePathResources =                $"Assets/ReMap/Resources";
    public static string relativePathAdditionalCode =           $"{relativePathResources}/AdditionalCode";
    public static string relativePathAdditionalCodeJson =       $"{relativePathAdditionalCode}/additionalCode.json";
    public static string relativePathAdditionalCodeInfo =       $"{relativePathAdditionalCode}/additionalCodeInfo.txt";
    public static string relativePathRpakManager =              $"{relativePathResources}/RpakManager";
    public static string relativePathRpakManagerList =          $"{relativePathRpakManager}/rpakManagerList.json";
    public static string relativePathJsonOffset =               $"{relativePathRpakManager}/prefabOffsetList.json";
    public static string relativePathTextureData =              $"{relativePathResources}/TextureData";
    public static string relativePathTextureDataList =          $"{relativePathTextureData}/textureData.json";
    public static string relativePathLegionPlus =               $"Assets/ReMap/LegionPlus";
    public static string relativePathLegionPlusExportedFiles =  $"{relativePathLegionPlus}/exported_files";
    public static string relativePathLegionExecutive =          $"{relativePathLegionPlus}/LegionPlus.exe";
    public static string relativePathNVIDIA =                   $"Assets/ReMap/NVIDIA";
    public static string relativePathNVTTEExecutive =           $"{relativePathNVIDIA}/nvtt_export.exe";
    public static string relativePathR5RPlayerInfo =            "\\platform\\scripts\\player_info.txt";
    public static string relativePathR5RScripts =               "\\platform\\scripts\\vscripts\\mp\\levels\\mp_rr_remap.nut";

    public static string relativeRMAPDEVfolder =                $"{relativePathResources}/DeveloperOnly";


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
        string[] results = AssetDatabase.FindAssets( name, new [] {$"{UnityInfo.relativePathPrefabs}/all_models"} );
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
