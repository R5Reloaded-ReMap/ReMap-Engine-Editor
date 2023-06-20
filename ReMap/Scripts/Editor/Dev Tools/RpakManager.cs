using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using WindowUtility;

namespace LibrarySorter
{
    public class RpakManagerWindow : EditorWindow
    {
        private static RpakManagerWindow windowInstance;

        internal static string rpakManagerPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathRpakManagerList}";
        internal static string entry = "";
        internal static string allModelsDataName = "all_models";
        internal static LibraryData libraryData;
        internal static string[][] rpakTab = new string[0][];
        internal static int tabIdx = 0;
        internal static int tabIdxTemp = 0;
        internal static int modelCount = 0;
        Vector2 scrollPos = Vector2.zero;

        List< string > dataContent = new List< string >();

        private static bool isAllModels = true;

        private static bool searchMode = false;

        // Tab
        private static int maxTabPerLine = 5;

        // Page
        private static int itemStart = 0;
        private static int itemEnd = 0;
        private int itemsPerPage = 100;
        private int currentPage = 0;
        private int maxPage = 0;

        public static void Init()
        {
            libraryData = FindLibraryDataFile();

            Refresh();

            windowInstance = ( RpakManagerWindow ) GetWindow( typeof( RpakManagerWindow ), false, "Rpak Manager" );
            windowInstance.minSize = new Vector2( 650, 600 );
            windowInstance.Show();
        }

        private void OnEnable()
        {
            libraryData = FindLibraryDataFile();

            Refresh();
        }

        void OnGUI()
        {
            isAllModels = libraryData.RpakList[ tabIdx ].Name == allModelsDataName;

            itemStart = currentPage * itemsPerPage;
            itemEnd = itemStart + itemsPerPage;

            dataContent = searchMode ? GetSearchResult() : libraryData.RpakList[ tabIdx ].Data;

            maxPage = modelCount == 0 ? 1 : dataContent.Count / itemsPerPage + 1;

            if ( modelCount != dataContent.Count ) GetModelCount();

            GUILayout.BeginVertical( "box" );

                ShowToolBar();

                WindowUtility.WindowUtility.SeparatorAutoWidth( windowInstance, -16 );

                ShowRpakData();

                if ( tabIdx != tabIdxTemp )
                {
                    Refresh();
                    tabIdxTemp = tabIdx;
                }

                if ( itemEnd > dataContent.Count ) currentPage = dataContent.Count / itemsPerPage;

                WindowUtility.WindowUtility.SeparatorAutoWidth( windowInstance, -16 );

                WindowUtility.WindowUtility.Space( 2 );

                TabInfo();

                WindowUtility.WindowUtility.Space( 2 );

                scrollPos = EditorGUILayout.BeginScrollView( scrollPos );

                WindowUtility.WindowUtility.Space( 6 );

                if ( searchMode && entry.Length < 3 )
                {
                    WindowUtility.WindowUtility.FlexibleSpace();
                        WindowUtility.WindowUtility.CreateTextInfoCentered( "Entry must contain at least 3 characters." );
                    WindowUtility.WindowUtility.FlexibleSpace();
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    return;
                }

                if ( modelCount == 0 )
                {
                    WindowUtility.WindowUtility.FlexibleSpace();
                        WindowUtility.WindowUtility.CreateTextInfoCentered( "No results." );
                    WindowUtility.WindowUtility.FlexibleSpace();
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    return;
                }

                for ( int i = itemStart; i < itemEnd && i < dataContent.Count; i++ )
                {
                    string model = dataContent[ i ];

                    if ( searchMode && entry.Length >= 3 )
                    {
                        string[] terms = entry.Replace( " ", "" ).Split( ";" );

                        bool found = false;

                        foreach ( string term in terms )
                        {
                            if ( model.Contains( term ) )
                            {
                                found = true; break;
                            }
                        }

                        if ( !found ) continue;
                    }

                    GUILayout.BeginHorizontal("box");
                        GUILayout.Label( model );
                        WindowUtility.WindowUtility.CreateCopyButton( "Copy", "", model, 100 );
                        if ( !isAllModels )
                        if ( WindowUtility.WindowUtility.CreateButton( "Remove", "", () => RemoveModel( model ), 100 ) )
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.EndScrollView();
                            GUILayout.EndVertical();
                            return;
                        }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void ShowRpakData()
        {
            int idx = 0;
            int totalButtons = libraryData.RpakList.Count;
            // Calculate how many buttons should be in each line
            int tabPerLine = totalButtons > maxTabPerLine ? Mathf.CeilToInt( ( float ) totalButtons / Mathf.CeilToInt( ( float ) totalButtons / maxTabPerLine ) ) : totalButtons;

            GUILayout.BeginHorizontal();
                foreach ( RpakData data in libraryData.RpakList )
                {
                    WindowUtility.WindowUtility.CreateButton( $"{data.Name}", "", () => { tabIdx = idx; } ); idx++;
                    if ( idx % tabPerLine == 0 && idx != 0 )
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                }
            GUILayout.EndHorizontal();
        }

        private void ShowToolBar()
        {
            GUILayout.BeginHorizontal();
                WindowUtility.WindowUtility.CreateButton( "Build all_models", "", () => BuildAllModels(), 100 );

                    GUILayout.FlexibleSpace();

                WindowUtility.WindowUtility.CreateButton( "Add Rpak", "", () => AddNewRpakList(), 100 );
                if ( !isAllModels ) WindowUtility.WindowUtility.CreateButton( "Remove Rpak", "", () => DeleteRpakList(), 100 );
                if ( !isAllModels ) WindowUtility.WindowUtility.CreateButton( "Delete List", "", () => DeleteList(), 100 );
                WindowUtility.WindowUtility.CreateButton( "Save", "", () => SaveJson(), 100 );
            GUILayout.EndHorizontal();
        }

        private void TabInfo()
        {
            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();

                    int end = itemEnd > dataContent.Count ? dataContent.Count : itemEnd;

                    WindowUtility.WindowUtility.ShowPageInfo( currentPage, maxPage, itemStart, end, $"Total Models: {modelCount} | Page" );

                    GUILayout.FlexibleSpace();

                    WindowUtility.WindowUtility.CreateButton( "Previous Page", "", () => { if ( currentPage > 0 ) currentPage--; }, 100 );

                    WindowUtility.WindowUtility.CreateButton( "Next Page", "", () => { if ( itemEnd < dataContent.Count ) currentPage++; }, 100 );

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    WindowUtility.WindowUtility.CreateToggle( ref searchMode, "Search Mode", "", 80 );
                    entry = EditorGUILayout.TextField( "", entry );
                    if ( !isAllModels ) WindowUtility.WindowUtility.CreateButton( "Add Model", "To add several models, the script separates\nthe entry from all \";\"", () => AddModel(), 100 );
                    if ( !isAllModels ) WindowUtility.WindowUtility.CreateButton( "Rename Folder", "", () => RenameTab(), 100 );
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal static void AddNewRpakList()
        {
            libraryData.RpakList.Add( NewRpakData() );

            tabIdx = rpakTab.Length - 1;

            SaveJson();
        }

        internal static void DeleteRpakList()
        {
            if ( !LibrarySorterWindow.CheckDialog( "Delete Rpak List", "Are you sure you want delete this rpak list ?" ) ) return;

            libraryData.RpakList.Remove( libraryData.RpakList[ tabIdx ] );

            tabIdx = rpakTab.Length - 2;

            BuildAllModels();

            SaveJson();
        }

        internal static void AddModel()
        {
            if ( string.IsNullOrEmpty( entry ) ) return;

            string[] models = entry.Replace( " ", "" ).Split( ";" );

            foreach ( string model in models )
            {
                if ( libraryData.RpakList[ tabIdx ].Data.Contains( model ) )
                    continue;

                if ( string.IsNullOrEmpty( model ) )
                    continue;

                if ( !model.Contains( "mdl/" ) )
                    continue;

                if ( !model.Contains( ".rmdl" ) )
                    continue;

                libraryData.RpakList[ tabIdx ].Data.Add( model );
            }

            libraryData.RpakList[ tabIdx ].Data.Sort();

            entry = "";

            SaveJson();
        }

        internal static void DeleteList()
        {
            if ( !LibrarySorterWindow.CheckDialog( "Delete Rpak Content", "Are you sure you want delete this rpak content ?" ) ) return;

            libraryData.RpakList[ tabIdx ].Data = new List< string >();

            SaveJson();
        }

        internal static void RemoveModel( string model )
        {
            if ( !LibrarySorterWindow.CheckDialog( "Remove Model", $"Are you sure you want remove this model ?\n\n{model}" ) ) return;

            libraryData.RpakList[ tabIdx ].Data.Remove( model );

            SaveJson();
        }

        internal static void RenameTab()
        {
            if ( string.IsNullOrEmpty( entry ) ) return;

            if ( !LibrarySorterWindow.CheckDialog( "Rename Tab", "Are you sure you want rename this tab ?" ) ) return;

            string newEntry = entry; int idx = 1; bool nameExists;

            do
            {
                nameExists = false;
                foreach ( RpakData content in libraryData.RpakList )
                {
                    if ( content.Name == newEntry )
                    {
                        nameExists = true;
                        newEntry = $"{entry} ({++idx})";
                        break;
                    }
                }
            } while ( nameExists );
            
            libraryData.RpakList[ tabIdx ].Name = newEntry;

            entry = "";

            SaveJson();
        }

        internal static void SaveJson()
        {
            string json = JsonUtility.ToJson( libraryData );
            System.IO.File.WriteAllText( rpakManagerPath, json );

            Refresh();
        }

        internal static void BuildAllModels()
        {
            List< string > allModels = new List< string >();

            RpakData allModelsData = null;

            foreach ( RpakData data in libraryData.RpakList )
            {
                if ( data.Name == allModelsDataName )
                {
                    allModelsData = data;
                    continue;
                }

                foreach ( string model in data.Data )
                {
                    if ( !allModels.Contains( model ) ) allModels.Add( model );
                }
            }

            if ( allModelsData == null )
            {
                allModelsData = NewRpakData();
                allModelsData.Name = allModelsDataName;

                libraryData.RpakList.Add( allModelsData );
            }

            allModelsData.Data = new List<string>();

            allModelsData.Data = allModels;

            SaveJson();
        }

        internal static void CreateNewJsonLibraryData()
        {
            libraryData = new LibraryData();
            libraryData.RpakList = new List< RpakData >();

            RpakData allModelsData = NewRpakData();
            allModelsData.Name = allModelsDataName;

            libraryData.RpakList.Add( allModelsData );

            string json = JsonUtility.ToJson( libraryData );
            System.IO.File.WriteAllText( rpakManagerPath, json );

            SaveJson();
        }

        internal static LibraryData FindLibraryDataFile()
        {
            if ( !File.Exists( rpakManagerPath ) )
            {
                CreateNewJsonLibraryData();
            }

            string json = System.IO.File.ReadAllText( rpakManagerPath );
            libraryData = JsonUtility.FromJson< LibraryData >( json );

            return libraryData;
        }

        internal static RpakData FindAllModel()
        {
            foreach ( RpakData data in libraryData.RpakList )
            {
                if ( data.Name == allModelsDataName ) return data;
            }

            return null;
        }

        private static RpakData NewRpakData()
        {
            RpakData data = new RpakData();
            data.Name = "unnamed";
            data.Data = new List< string >();
            data.Update = DateTime.UtcNow.ToString();

            return data;
        }

        private static void Refresh()
        {
            UnityInfo.SortListByKey( libraryData.RpakList, x => x.Name );

            List< List< string > > arrays = new List< List< string > >();

            List< string > tab = new List< string> ();

            foreach ( RpakData data in libraryData.RpakList )
            {
                if ( tab.Count == maxTabPerLine )
                {
                    arrays.Add( tab );
                    tab = new List< string >();
                }

                tab.Add( data.Name );
            }

            if ( tab.Count > 0 )
            {
                arrays.Add( tab );
            }

            rpakTab = arrays.Select( a => a.ToArray() ).ToArray();
        }

        private static void GetModelCount()
        {
            if ( searchMode && entry.Length < 3 )
            {
                modelCount = 0;
            }
            else if ( searchMode && entry.Length >= 3 )
            {
                modelCount = 0;

                foreach ( string model in libraryData.RpakList[ tabIdx ].Data )
                {
                    string[] terms = entry.Replace( " ", "" ).Split( ";" );

                    bool found = false;

                    foreach ( string term in terms )
                    {
                        if ( model.Contains( term ) )
                        {
                            found = true; break;
                        }
                    }

                    if ( found ) modelCount++;
                }
            }
            else modelCount = libraryData.RpakList[ tabIdx ].Data.Count;
        }

        private static List < string > GetSearchResult()
        {
            List < string > list = new List < string >();

            foreach ( string model in libraryData.RpakList[ tabIdx ].Data )
            {
                string[] terms = entry.Replace( " ", "" ).Split( ";" );

                bool found = false;

                foreach ( string term in terms )
                {
                    if ( string.IsNullOrEmpty( term ) ) continue;

                    if ( model.Contains( term ) )
                    {
                        found = true; break;
                    }
                }

                if ( found ) list.Add( model );
            }

            return list;
        }
    }
}
