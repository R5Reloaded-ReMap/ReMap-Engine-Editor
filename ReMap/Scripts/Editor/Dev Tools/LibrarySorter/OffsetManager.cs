using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class OffsetManagerWindow : EditorWindow
    {
        private static Vector2 scrollPos = Vector2.zero;
        private static List< PrefabOffset > offsets;
        private static List< PrefabOffset > offsetsSearch;

        private static GameObject prefabToAdd;
        private static string search = "";
        private static int searchLenght;
        private static bool searchEnable;

        // Page
        private static int itemStart;
        private static int itemEnd;
        private int currentPage;
        private readonly int itemsPerPage = 100;
        private int maxPage;

        public static void Init()
        {
            var window = ( OffsetManagerWindow )GetWindow( typeof(OffsetManagerWindow), false, "Prefab Offset Manager" );
            window.minSize = new Vector2( 800, 400 );
            window.Show();
            RefreshPage();
        }

        private void OnEnable()
        {
            RefreshPage();
        }

        private void OnGUI()
        {
            itemStart = currentPage * itemsPerPage;
            itemEnd = itemStart + itemsPerPage;

            searchEnable = search.Length >= 3;

            if ( searchEnable && searchLenght != search.Length )
            {
                searchLenght = search.Length;
                SearchResult();
                RefreshPage( false );
            }

            maxPage = offsets.Count == 0 ? 1 : offsets.Count / itemsPerPage + 1;

            var labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;

            EditorGUILayout.BeginVertical( "box" );

            TabInfo();

            scrollPos = EditorGUILayout.BeginScrollView( scrollPos );

            var offsetToShow = searchEnable ? offsetsSearch : offsets;

            for ( int i = itemStart; i < itemEnd && i < offsetToShow.Count; i++ )
            {
                var offset = offsetToShow[i];

                GUILayout.BeginHorizontal( "box" );
                WindowUtility.WindowUtility.CreateTextInfo( offset.ModelName );

                WindowUtility.WindowUtility.CreateVector3Field( ref offset.Rotation, "", "", 0, 280 );

                if ( WindowUtility.WindowUtility.CreateButton( "Remove", "", () =>
                    {
                        SaveJson();
                        RemoveOffsetFromList( offset );
                    }, 100 ) )
                {
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                    return;
                }

                WindowUtility.WindowUtility.CreateButton( "Fix Prefab", "", () =>
                {
                    SaveJson();
                    LibrarySorterWindow.AwaitTask( TaskType.FixSpecificPrefabData, UnityInfo.GetUnityModelName( offset.ModelName ) );
                }, 100 );

                GUILayout.EndHorizontal();

                if ( Input.GetKeyDown( KeyCode.Return ) || Input.GetKeyDown( KeyCode.Tab ) || Event.current.type == EventType.MouseDown )
                    GUI.FocusControl( "" ); // remove focus from current control
            }

            GUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            prefabToAdd = EditorGUILayout.ObjectField( prefabToAdd, typeof(Object), true ) as GameObject;
            WindowUtility.WindowUtility.CreateButton( "Add Prefab", "", () => AddPrefabToList( prefabToAdd ), 100 );
            EditorGUILayout.EndHorizontal();

            if ( GUILayout.Button( "Save Json" ) ) SaveJson();

            EditorGUILayout.EndVertical();
        }

        private void TabInfo()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            var offsetToShow = searchEnable ? offsetsSearch : offsets;

            int end = itemEnd > offsetToShow.Count ? offsetToShow.Count : itemEnd;

            WindowUtility.WindowUtility.ShowPageInfo( currentPage, maxPage, itemStart, end, $"Total Models: {offsetToShow.Count} | Page" );

            GUILayout.FlexibleSpace();

            WindowUtility.WindowUtility.CreateButton( "Previous Page", "", () =>
            {
                if ( currentPage > 0 ) currentPage--;
            }, 100 );

            WindowUtility.WindowUtility.CreateButton( "Next Page", "", () =>
            {
                if ( itemEnd < offsetToShow.Count ) currentPage++;
            }, 100 );

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            WindowUtility.WindowUtility.CreateTextField( ref search, "Search", "", 50 );
            WindowUtility.WindowUtility.CreateButton( "Refresh", "", () => RefreshPage(), 100 );
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal static void AddPrefabToList( GameObject prefabToAdd )
        {
            if ( !Helper.IsValid( prefabToAdd ) ) return;

            string rpakPath = UnityInfo.GetApexModelName( prefabToAdd.name, true );
            if ( PrefabOffsetExist( rpakPath ) ) return;

            var newOffset = new PrefabOffset();
            newOffset.ModelName = rpakPath;
            newOffset.Rotation = Vector3.zero;
            offsets.Add( newOffset );

            SaveJson();
            RefreshPage();
            FindObjectAndSetScroll( rpakPath );
        }

        internal static void RemoveOffsetFromList( PrefabOffset offset )
        {
            if ( !LibrarySorterWindow.CheckDialog( "Remove PrefabOffset", $"Are you sure you want delete {offset.ModelName} ?" ) ) return;

            offsets.Remove( offset );
            SaveJson();
            RefreshPage();
        }

        internal static void SaveJson()
        {
            PrefabOffsetList newOffsetlist;
            newOffsetlist = new PrefabOffsetList();
            newOffsetlist.List = new List< PrefabOffset >();

            foreach ( var offset in offsets )
            {
                var newOffset = new PrefabOffset();
                newOffset.ModelName = offset.ModelName;
                newOffset.Rotation = offset.Rotation;
                newOffsetlist.List.Add( newOffset );
            }

            string json = JsonUtility.ToJson( newOffsetlist );
            File.WriteAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}", json );
        }

        internal static void FindObjectAndSetScroll( string objectName )
        {
            int index = offsets.FindIndex( o => o.ModelName == objectName );

            if ( index != -1 ) scrollPos = new Vector2( 0, index * EditorGUIUtility.singleLineHeight );
        }

        internal static void RefreshPage( bool save = true )
        {
            offsets = FindPrefabOffsetFile();

            UnityInfo.SortListByKey( offsets, x => x.ModelName );

            if ( save ) SaveJson();
        }

        internal static bool PrefabOffsetExist( string name )
        {
            var prefab = offsets.Find( o => o.ModelName == name );
            if ( prefab != null ) return true;

            return false;
        }

        internal static List< PrefabOffset > FindPrefabOffsetFile()
        {
            return GetPrefabOffset().List;
        }

        internal static PrefabOffsetList GetPrefabOffset()
        {
            return JsonUtility.FromJson< PrefabOffsetList >( ReadOffsetFile() );
        }

        private static string ReadOffsetFile()
        {
            return File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}" );
        }

        private static void SearchResult()
        {
            offsetsSearch = new List< PrefabOffset >();

            foreach ( var offset in offsets )
            {
                string[] terms = search.ToLower().Replace( " ", "" ).Split( ";" );

                bool found = false;

                foreach ( string term in terms )
                {
                    if ( string.IsNullOrEmpty( term ) ) continue;

                    if ( offset.ModelName.ToLower().Contains( term ) )
                    {
                        found = true;
                        break;
                    }
                }

                if ( found ) offsetsSearch.Add( offset );
            }
        }
    }
}