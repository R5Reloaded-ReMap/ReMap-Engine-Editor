
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LootRepartitionWindow : EditorWindow
{
    private static LootRepartitionWindow windowInstance;

    private static string lootRepartitionPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathResources}/LootRepartition";

    public static void Init()
    {
        windowInstance = ( LootRepartitionWindow ) EditorWindow.GetWindow( typeof( LootRepartitionWindow ), false, "Loot Repartition" );
    }

    void OnEnable()
    {
        windowInstance = ( LootRepartitionWindow ) EditorWindow.GetWindow( typeof( LootRepartitionWindow ), false, "Loot Repartition" );
        //windowInstance.minSize = new Vector2( 300, 290 );
        windowInstance.Show();
    }

    void OnGUI()
    {
        
    }

    public static void WriteLootRepartitionFile()
    {
        string filePath = $"{lootRepartitionPath}/0xe7c3cc7cffc408f2.csv";

        LootData lootData = new LootData();

        lootData.Groups = new List< Group >();

        using ( var reader = new StreamReader( filePath ) )
        {
            // Ignore 1 first line
            reader.ReadLine();

            string line;
            while ( ( line = reader.ReadLine() ) != null )
            {
                if ( line == "\"string\",\"string\",\"string\",\"float\",\"int\",\"int\"" )
                    break;

                string[] fields = line.Split( ',' );

                string featureFlag = fields[0].Trim( '"' );
                string groupRef = fields[1].Trim( '"' );
                string entryRef = fields[2].Trim( '"' );
                float entryDistribution = float.Parse( fields[3], NumberStyles.Float, CultureInfo.InvariantCulture );
                int groupTier = int.Parse( fields[4] );
                int priority = int.Parse( fields[5] );

                Group group = lootData.Groups.Find( g => g.GroupRef == groupRef );

                if ( group == null )
                {
                    group = new Group { FeatureFlag = featureFlag, GroupRef = groupRef, Entries = new List< Entry >() };
                    lootData.Groups.Add( group );
                }

                group.Entries.Add( new Entry
                {
                    EntryRef = entryRef,
                    EntryDistribution = entryDistribution,
                    GroupTier = groupTier,
                    Priority = priority
                });
            }
        }

        string json = JsonUtility.ToJson( lootData );
        string path = $"{lootRepartitionPath}/LootRepartition.json";;

        if ( File.Exists( path ) ) File.Delete( path );

        File.WriteAllText( path, json );
    }
}

public class Entry
{
    public string EntryRef { get; set; }
    public float EntryDistribution { get; set; }
    public int GroupTier { get; set; }
    public int Priority { get; set; }
}

public class Group
{
    public string FeatureFlag { get; set; }
    public string GroupRef { get; set; }
    public List< Entry > Entries { get; set; }
}

public class LootData
{
    public List< Group > Groups { get; set; }
}
