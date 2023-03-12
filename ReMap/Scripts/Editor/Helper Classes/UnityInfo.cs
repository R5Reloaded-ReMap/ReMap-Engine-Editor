using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class UnityInfo
{
    public static string currentDirectory = LibrarySorterWindow.currentDirectory;
    public static string relativeRpakFile = LibrarySorterWindow.relativeRpakFile;


    /// <summary>
    /// Gets total GameObject in scene
    /// </summary>
    /// <returns></returns>
    public static GameObject[] GetAllGameObjectInScene()
    {
        return UnityEngine.Object.FindObjectsOfType<GameObject>();
    }

    /// <summary>
    /// Gets Total Count of all objects in scene
    /// </summary>
    /// <returns></returns>
    public static int GetAllCount()
    {
        int objectCount = 0;

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects) {
            foreach (string key in Helper.ObjectToTag.Keys)
                if (go.name.Contains(key))
                    objectCount++;
        }

        return objectCount;
    }

    /// <summary>
    /// Gets Total Count of all props in scene
    /// </summary>
    /// <returns></returns>
    public static int GetPropCount()
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Prop");
        return PropObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all zipLines in scene
    /// </summary>
    /// <returns></returns>
    public static int GetZipLineCount()
    {
        GameObject[] ZipLineObjects = GameObject.FindGameObjectsWithTag("ZipLine");
        return ZipLineObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all vertical zipLines in scene
    /// </summary>
    /// <returns></returns>
    public static int GetVerticalZipLineCount()
    {
        GameObject[] VerticalZipLineObjects = GameObject.FindGameObjectsWithTag("VerticalZipLine");
        return VerticalZipLineObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all non vertical zipLines in scene
    /// </summary>
    /// <returns></returns>
    public static int GetNonVerticalZipLineCount()
    {
        GameObject[] NonVerticalZipLineObjects = GameObject.FindGameObjectsWithTag("NonVerticalZipLine");
        return NonVerticalZipLineObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all linked zipLines in scene
    /// </summary>
    /// <returns></returns>
    public static int GetLinkedZiplineCount()
    {
        GameObject[] LinkedZiplineObjects = GameObject.FindGameObjectsWithTag("LinkedZipline");
        return LinkedZiplineObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all lootbins in scene
    /// </summary>
    /// <returns></returns>
    public static int GetLootBinCount()
    {
        GameObject[] LootBinObjects = GameObject.FindGameObjectsWithTag("LootBin");
        return LootBinObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all weapon racks in scene
    /// </summary>
    /// <returns></returns>
    public static int GetWeaponRackCount()
    {
        GameObject[] WeaponRackObjects = GameObject.FindGameObjectsWithTag("WeaponRack");
        return WeaponRackObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all jump pads in scene
    /// </summary>
    /// <returns></returns>
    public static int GetJumppadCount()
    {
        GameObject[] JumppadObjects = GameObject.FindGameObjectsWithTag("Jumppad");
        return JumppadObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all bubble shields in scene
    /// </summary>
    /// <returns></returns>
    public static int GetBubbleShieldCount()
    {
        GameObject[] BubbleShieldObjects = GameObject.FindGameObjectsWithTag("BubbleShield");
        return BubbleShieldObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all triggers in scene
    /// </summary>
    /// <returns></returns>
    public static int GetTriggerCount()
    {
        GameObject[] TriggerObjects = GameObject.FindGameObjectsWithTag("Trigger");
        return TriggerObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all single doors in scene
    /// </summary>
    /// <returns></returns>
    public static int GetSingleDoorCount()
    {
        GameObject[] SingleDoorObjects = GameObject.FindGameObjectsWithTag("SingleDoor");
        return SingleDoorObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all double doors in scene
    /// </summary>
    /// <returns></returns>
    public static int GetDoubleDoorCount()
    {
        GameObject[] DoubleDoorObjects = GameObject.FindGameObjectsWithTag("DoubleDoor");
        return DoubleDoorObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all vertical doors in scene
    /// </summary>
    /// <returns></returns>
    public static int GetVerticalDoorCount()
    {
        GameObject[] VerticalDoorObjects = GameObject.FindGameObjectsWithTag("VerticalDoor");
        return VerticalDoorObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all horizontal doors in scene
    /// </summary>
    /// <returns></returns>
    public static int GetHorzDoorCount()
    {
        GameObject[] HorzDoorObjects = GameObject.FindGameObjectsWithTag("HorzDoor");
        return HorzDoorObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all buttons in scene
    /// </summary>
    /// <returns></returns>
    public static int GetButtonCount()
    {
        GameObject[] ButtonObjects = GameObject.FindGameObjectsWithTag("Button");
        return ButtonObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all sounds in scene
    /// </summary>
    /// <returns></returns>
    public static int GetSoundCount()
    {
        GameObject[] PropObjects = GameObject.FindGameObjectsWithTag("Sound");
        return PropObjects.Length;
    }

    /// <summary>
    /// Gets Total Count of all spawn points in scene
    /// </summary>
    /// <returns></returns>
    public static int GetSpawnPointCount()
    {
        GameObject[] SpawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        return SpawnPointObjects.Length;
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

        if ( !returnFileName )
        {
            return filePaths;
        }
        else
        {
            List<string> fileNames = new List<string>();

            foreach ( string filePath in filePaths )
            {
                fileNames.Add( Path.GetFileNameWithoutExtension( filePath ) );
            }

            return fileNames.ToArray();
        }
    }

    private static bool IsNotExcludedFile( string filePath, bool includeAllModelFile )
    {
        string fileName = Path.GetFileName(filePath);
        string[] excludedFiles;

        if ( includeAllModelFile )
        {   excludedFiles = new string[] { "modelAnglesOffset.txt", "lastestFolderUpdate.txt", "all_models.txt" }; }
        else
        { excludedFiles = new string[] { "modelAnglesOffset.txt", "lastestFolderUpdate.txt" }; }

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
