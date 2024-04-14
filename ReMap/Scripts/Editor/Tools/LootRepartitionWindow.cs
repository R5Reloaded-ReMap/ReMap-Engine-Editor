using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using static WindowUtility.WindowUtility;

public class LootRepartitionWindow : EditorWindow
{
    private static LootRepartitionWindow windowInstance;

    private static readonly string lootRepartitionPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathResources}/LootRepartition";
    private static Vector2 Scroll = Vector2.zero;

    private static LootData LootData;

    public static void Init()
    {
        windowInstance = ( LootRepartitionWindow )GetWindow( typeof(LootRepartitionWindow), false, "Loot Repartition" );
        //windowInstance.minSize = new Vector2( 300, 290 );
        windowInstance.Show();
    }

    private void OnEnable()
    {
        string json = File.ReadAllText( $"{lootRepartitionPath}/LootRepartition.json" );
        LootData = JsonUtility.FromJson< LootData >( json );
    }

    private void OnGUI()
    {
        if ( !Helper.IsValid( LootData ) )
        {
            FlexibleSpace();
            CreateTextInfoCentered( "LootData file not found." );
            FlexibleSpace();
            return;
        }

        Scroll = EditorGUILayout.BeginScrollView( Scroll );

        foreach ( var group in LootData.Groups )
        {
            CreateTextInfo( $"{group.GroupRef}" );

            foreach ( var entry in group.Entries )
                CreateTextInfo( $"{entry.EntryRef} {entry.EntryDistribution} {entry.GroupTier} {entry.Priority}" );
        }

        EditorGUILayout.EndScrollView();
    }

    public static void WriteLootRepartitionFile()
    {
        string filePath = $"{lootRepartitionPath}/0xe7c3cc7cffc408f2.csv";

        var lootData = new LootData();

        lootData.Groups = new List< Group >();

        using (var reader = new StreamReader( filePath ))
        {
            // Ignore 2 first line
            reader.ReadLine();
            reader.ReadLine();

            string line;
            while (( line = reader.ReadLine() ) != null)
            {
                if ( line == "\"string\",\"string\",\"string\",\"float\",\"int\",\"int\"" || line.Contains( "\u000F" ) )
                    continue;

                string[] fields = line.Split( ',' );

                string featureFlag = fields[0].Trim( '"' );
                string groupRef = fields[1].Trim( '"' );
                string entryRef = fields[2].Trim( '"' );
                float entryDistribution = float.Parse( fields[3], NumberStyles.Float, CultureInfo.InvariantCulture );
                int groupTier = int.Parse( fields[4] );
                int priority = int.Parse( fields[5] );

                var group = lootData.Groups.Find( g => g.GroupRef == groupRef );

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
                } );
            }
        }

        string json = JsonUtility.ToJson( lootData );

        string path = $"{lootRepartitionPath}/LootRepartition.json";

        if ( File.Exists( path ) ) File.Delete( path );

        File.WriteAllText( path, json );
    }
}

[Serializable]
public class Entry
{
    public float EntryDistribution;
    public string EntryRef;
    public int GroupTier;
    public int Priority;
}

[Serializable]
public class Group
{
    public List< Entry > Entries;
    public string FeatureFlag;
    public string GroupRef;
}

[Serializable]
public class LootData
{
    public List< Group > Groups;
}