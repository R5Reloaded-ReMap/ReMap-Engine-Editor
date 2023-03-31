using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

[Serializable]
public class RpakContentJson
{
    public List<RpakContentClass> List;
}

[Serializable]
public class RpakContentClass
{
    public string modelName;
    public string[] location;
}

public class RpakInfo : EditorWindow
{
    static string usedRpak = "";
    static string currentDirectory = LibrarySorterWindow.currentDirectory;
    static string relativePrefabs = LibrarySorterWindow.relativePrefabs;

    Vector2 scrollPosition;

    //[MenuItem("ReMap/Tools/Rpak Info", false, 100)]
    public static void Init()
    {
        RpakInfo window = (RpakInfo)EditorWindow.GetWindow(typeof(RpakInfo), false, "Rpak Info");
        window.Show();
        //window.minSize = new Vector2(375, 140);
        //window.maxSize = new Vector2(375, 140);
        //GetModelsInScene();
        RpakDetermine();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
            GUILayout.Label( "Models Used:" );
            if ( GUILayout.Button( "Reload List" ) ) Reload();
        EditorGUILayout.EndHorizontal();

        scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, GUILayout.Height( 300 ) );
            GUILayout.TextField( usedRpak );
        EditorGUILayout.EndScrollView();
    }

    void Update()
    {
        
    }

    static void GetModelsInScene()
    {
        foreach ( string models in UnityInfo.GetModelsListInScene() )
        {
            usedRpak += $"{models}\n";
        }
    }

    static void RpakDetermine()
    {
        List<string> modelsInScene = UnityInfo.GetModelsListInScene().ToList();

        List<string> rpakLists = new List<string>( UnityInfo.GetAllRpakModelsFile( true, true ) );

        RemoveFromList( rpakLists, "_custom_models" );

        List< List<string> > rpakFiles = new List< List<string> >();

        RpakContentJson rpakContent = JsonUtility.FromJson<RpakContentJson>( File.ReadAllText( $"{currentDirectory}/{relativePrefabs}/RpakList.json" ) );

        usedRpak += $"The map use {modelsInScene.Count} models\n\n";

        bool newRpak = true;

        foreach (string modelName in modelsInScene)
        {
            RpakContentClass rpakContentClass = rpakContent.List.Find(x => x.modelName == modelName);

            if (rpakContentClass == null) continue;

            if ( rpakContentClass.location.Contains( "common" ) || rpakContentClass.location.Contains( "common_mp" ) || rpakContentClass.location.Contains( "common_sdk" ) )
            {
                if ( newRpak )
                {
                    newRpak = false;
                    rpakFiles.Add(new List<string>());
                }

                rpakFiles[rpakFiles.Count - 1].Add(modelName);
            }
        }

        RemoveFromList( rpakLists, "common" );
        RemoveFromList( rpakLists, "common_mp" );
        RemoveFromList( rpakLists, "common_sdk" );

        newRpak = true;
        if (rpakFiles.Count != 0)
        foreach (string modelName in rpakFiles[rpakFiles.Count - 1])
        {
            if ( newRpak )
            {
                newRpak = false;
                usedRpak += "common && common_mp ↓\n";
            }
            usedRpak += $"- {modelName}\n";
            modelsInScene.Remove( modelName );
            RemoveFromList( modelsInScene, modelName );
        }
        usedRpak += "\n";

        //for ( int i = 0 ; i < 6 ; i++ )
        //{
            //debug.Log(modelsInScene.Count);
            Dictionary<string, int> locationCount = new Dictionary<string, int>();

            foreach (string modelName in modelsInScene)
            {
                RpakContentClass rpakContentClass = rpakContent.List.Find(x => x.modelName == modelName);

                if (rpakContentClass == null) continue;

                foreach (string loc in rpakContentClass.location)
                {
                    if (locationCount.ContainsKey(loc))
                    {
                        locationCount[loc]++;
                    }
                    else
                    {
                        locationCount[loc] = 1;
                    }
                }
            }

            // Search for the location with the highest meter
            string mostCommonLocation = "";
            int highestCount = 0;
            foreach (KeyValuePair<string, int> pair in locationCount)
            {
                if (pair.Value > highestCount)
                {
                    highestCount = pair.Value;
                    mostCommonLocation = pair.Key;
                }
            }

            //Debug.Log("The most common location is : " + mostCommonLocation);

            foreach (string modelName in modelsInScene)
            {
                RpakContentClass rpakContentClass = rpakContent.List.Find(x => x.modelName == modelName);

                if (rpakContentClass == null) continue;

                if ( rpakContentClass.location.Contains( mostCommonLocation ) )
                {
                    if ( newRpak )
                    {
                        newRpak = false;
                        rpakFiles.Add(new List<string>());
                    }

                    rpakFiles[rpakFiles.Count - 1].Add(modelName);
                }
            }

            RemoveFromList( rpakLists, mostCommonLocation );

            newRpak = true;
            if (rpakFiles.Count != 0)
            foreach (string modelName in rpakFiles[rpakFiles.Count - 1])
            {
                if ( newRpak )
                {
                    newRpak = false;
                    usedRpak += $"{mostCommonLocation} ↓\n";
                }
                usedRpak += $"- {modelName}\n";
                modelsInScene.Remove( modelName );
                RemoveFromList( modelsInScene, modelName );
            }
            usedRpak += "\n";
        //}


        



        // Remove after tests
            usedRpak += $"\n\n\n";
            foreach ( string rpakList in rpakLists )
            {
                usedRpak += $"- {rpakList}\n";
            }
        //
    }

    static void RemoveFromList( List<string> list, string reference )
    {
        if ( list.Contains( reference ) ) list.Remove( reference );
    }

    static void ProcessingRpak( List<string> modelsInScene, RpakContentJson rpakContent, List<string> reference )
    {

    }

    static void Reload()
    {
        usedRpak = "";
        //GetModelsInScene();
        RpakDetermine();
    }
}