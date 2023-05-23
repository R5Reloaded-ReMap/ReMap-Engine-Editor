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
        internal static string rpakManagerPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathRpakManagerList}";
        internal static string entry = "";
        internal static string allModelsDataName = "all_models";
        internal static LibraryData libraryData;
        internal static string[] rpakTab = new string[0];
        internal static int tabIdx = 0;
        internal static int tabIdxTemp = 0;
        internal static int modelCount = 0;
        Vector2 scrollPos = Vector2.zero;

        public static void Init()
        {
            libraryData = FindLibraryDataFile();

            Refresh();

            RpakManagerWindow window = ( RpakManagerWindow )GetWindow( typeof( RpakManagerWindow ), false, "Rpak Manager" );
            window.minSize = new Vector2( 650, 600 );
            window.Show();
        }

        private void OnEnable()
        {
            libraryData = FindLibraryDataFile();

            Refresh();
        }

        void OnGUI()
        {
            bool isAllModels = true;

            if ( libraryData.RpakList[ tabIdx ].Name != allModelsDataName ) isAllModels = false;

            GUILayout.BeginVertical( "box" );
                tabIdx = GUILayout.Toolbar( tabIdx, rpakTab );

                if( tabIdx != tabIdxTemp )
                {
                    Refresh();
                    tabIdxTemp = tabIdx;
                }

                GUILayout.BeginHorizontal();
                    GUILayout.Label( $"Models: {modelCount}" );
                    GUILayout.FlexibleSpace();
                    WindowUtility.WindowUtility.CreateButton( "Add Rpak", "", () => AddNewRpakList(), 100 );
                    if ( !isAllModels ) WindowUtility.WindowUtility.CreateButton( "Remove Rpak", "", () => DeleteRpakList(), 100 );
                    if ( !isAllModels ) WindowUtility.WindowUtility.CreateButton( "Delete List", "", () => DeleteList(), 100 );
                    WindowUtility.WindowUtility.CreateButton( "Build all_models", "", () => BuildAllModels(), 100 );
                    WindowUtility.WindowUtility.CreateButton( "Save", "", () => SaveJson(), 100 );
                GUILayout.EndHorizontal();

                scrollPos = EditorGUILayout.BeginScrollView( scrollPos );

                GUILayout.Space( 6 );

                foreach ( string model in libraryData.RpakList[ tabIdx ].Data )
                {
                    GUILayout.BeginHorizontal( "box" );
                        GUILayout.Label( model );
                        if( !isAllModels )
                        if( WindowUtility.WindowUtility.CreateButton( "Remove", "", () => RemoveModel( model ), 100 ) )
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.EndScrollView();
                            GUILayout.EndVertical();
                            return;
                        }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();

                if ( !isAllModels )
                {
                    GUILayout.BeginHorizontal();
                        entry = EditorGUILayout.TextField( "", entry );
                        WindowUtility.WindowUtility.CreateButton( "Add Model", "", () => AddModel(), 100 );
                        WindowUtility.WindowUtility.CreateButton( "Rename Folder", "", () => RenameTab(), 100 );
                    GUILayout.EndHorizontal();
                }
            GUILayout.EndVertical();
        }

        internal static void AddNewRpakList()
        {
            libraryData.RpakList.Add( NewRpakData() );

            tabIdx = rpakTab.Length;

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
            string[] models = entry.Replace( " ", "" ).Split( ";" );

            foreach ( string model in models )
            {
                if ( libraryData.RpakList[ tabIdx ].Data.Contains( model ) || model == "" ) continue;

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

            List< string > pages = new List< string >();

            foreach ( RpakData data in libraryData.RpakList )
            {
                pages.Add( data.Name );
            }

            rpakTab = pages.ToArray();

            modelCount = libraryData.RpakList[ tabIdx ].Data.Count;
        }
    }
}
