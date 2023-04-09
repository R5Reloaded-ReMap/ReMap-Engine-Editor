
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class OffsetManagerWindow : EditorWindow
    {
        static Vector2 scrollPos = Vector2.zero;
        string json = System.IO.File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}" );
        static PrefabOffsetList offsetlist;
        static PrefabOffsetList newOffsetlist;
        static List< PrefabOffset > offsets;

        static GameObject prefabToAdd = null;
        static string search = "";
        static bool searchEnable = false;

        public static Dictionary<string, Vector3> dictionary = new Dictionary<string, Vector3>();

        public static void Init()
        {
            OffsetManagerWindow window = ( OffsetManagerWindow )GetWindow( typeof( OffsetManagerWindow ), false, "Prefab Offset Manager" );
            window.minSize = new Vector2( 800, 400 ); // new Vector2( 888, 700 );
            //window.maxSize = new Vector2( 888, 700 );
            window.Show(); RefreshPage();
        }

        void OnEnable()
        {
            RefreshPage();
        }

        void OnGUI()
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;

            EditorGUILayout.Space( 2 );

            searchEnable = false;

            EditorGUILayout.BeginHorizontal();
                GUILayout.Label( "Search", GUILayout.Width( 50 ) );
                search = EditorGUILayout.TextField( "", search );
                if ( search.Length >= 3 ) searchEnable = true;
                if ( GUILayout.Button( "Refresh Page", GUILayout.Width( 200 ) ) ) RefreshPage();
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView( scrollPos );

            foreach ( PrefabOffset offset in offsets )
            {
                if ( searchEnable && !offset.ModelName.Contains( search ) ) continue;

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField( $"{offset.ModelName}", labelStyle );
                    offset.Rotation = EditorGUILayout.Vector3Field( "", offset.Rotation, GUILayout.Width( 280 ) );
                    if ( GUILayout.Button( "Remove", GUILayout.Width( 100 ) ) )
                    {
                        RemoveOffsetFromList( offset );
                        EditorGUILayout.EndHorizontal();
                        GUILayout.EndScrollView();
                        return;
                    }
                    if ( GUILayout.Button( "Fix Prefab", GUILayout.Width( 100 ) ) ) LibrarySorterWindow.FixPrefab( offset.ModelName );
                EditorGUILayout.EndHorizontal();

                if ( Input.GetKeyDown( KeyCode.Return ) || Input.GetKeyDown( KeyCode.Tab ) || Event.current.type == EventType.MouseDown )
                {
                    GUI.FocusControl( "" ); // remove focus from current control
                }
            }

            GUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
                prefabToAdd = EditorGUILayout.ObjectField( prefabToAdd, typeof( UnityEngine.Object ), true ) as GameObject;
                if ( GUILayout.Button( "Add Prefab", GUILayout.Width( 100 ) ) && prefabToAdd != null ) AddPrefabToList( prefabToAdd );
            EditorGUILayout.EndHorizontal();

            if ( GUILayout.Button( "Save Json" ) ) SaveJson();
        }

        internal static void AddPrefabToList( GameObject prefabToAdd )
        {
            string rpakPath = prefabToAdd.name.Replace( '#', '/' ) + ".rmdl";
            if ( PrefabOffsetExist( rpakPath ) ) return;

            PrefabOffset newOffset = new PrefabOffset();
            newOffset.ModelName = rpakPath;
            newOffset.Rotation = Vector3.zero;
            offsets.Add( newOffset );

            SaveJson();
            RefreshPage();
            FindObjectAndSetScroll( rpakPath );
        }

        internal static void RemoveOffsetFromList( PrefabOffset offset )
        {
            offsets.Remove( offset );
            SaveJson();
            RefreshPage();
        }

        internal static void SaveJson()
        {
            newOffsetlist = new PrefabOffsetList();
            newOffsetlist.List = new List< PrefabOffset >();

            foreach ( PrefabOffset offset in offsets )
            {
                PrefabOffset newOffset = new PrefabOffset();
                newOffset.ModelName = offset.ModelName;
                newOffset.Rotation = offset.Rotation;
                newOffsetlist.List.Add( newOffset );
            }

            string json = JsonUtility.ToJson( newOffsetlist );
            System.IO.File.WriteAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}", json );
        }

        internal static void FindObjectAndSetScroll( string objectName )
        {
            int index = offsets.FindIndex( o => o.ModelName == objectName );

            if ( index != -1 ) scrollPos = new Vector2( 0, index * EditorGUIUtility.singleLineHeight );
        }

        internal static void RefreshPage()
        {
            string json = System.IO.File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}" );
            offsetlist = JsonUtility.FromJson< PrefabOffsetList >( json );
            offsets = offsetlist.List;

            UnityInfo.SortListByKey(offsets, x => x.ModelName);
        }

        internal static bool PrefabOffsetExist( string name )
        {
            PrefabOffset prefab = offsets.Find( o => o.ModelName == name );
            if ( prefab != null ) return true;

            return false;
        }

        internal static List< PrefabOffset > FindPrefabOffsetFile()
        {
            string json = System.IO.File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}" );
            return JsonUtility.FromJson< PrefabOffsetList >( json ).List;
        }
    }
}