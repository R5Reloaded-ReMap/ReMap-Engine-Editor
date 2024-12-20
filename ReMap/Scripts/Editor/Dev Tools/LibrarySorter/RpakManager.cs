using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class RpakManagerWindow : EditorWindow
    {
        private static RpakManagerWindow windowInstance;

        internal static readonly string rpakManagerPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathRpakManagerList}";
        internal static string entry = "";
        public static readonly string allModelsDataName = "all_models";
        public static readonly string r5reloadedModelsDataName = "r5reloaded_list";
        public static readonly string allModelsRetailDataName = "all_models_retail";

        internal static LibraryData libraryData = FindLibraryDataFile();

        internal static string[][] rpakTab = new string[0][];
        internal static RpakData rpakData = libraryData.RpakList[0];
        internal static RpakData rpakDataTemp = libraryData.RpakList[0];
        internal static int modelCount;

        private static readonly string[] rpakTypes = { "special", "r5reloaded", "retail" };
        private static string rpakType = rpakTypes[1];

        private static Texture2D visibleIcon;
        private static Texture2D hiddenIcon;

        private static bool searchMode;

        // Tab
        private static readonly int maxTabPerLine = 5;

        // Page
        private static int itemStart;
        private static int itemEnd;
        private static readonly int itemsPerPage = 100;
        private static int currentPage;
        private static int maxPage;

        private List< string > dataContent = new();
        private Vector2 scrollPos = Vector2.zero;

        public static void Init()
        {
            libraryData = FindLibraryDataFile();

            Refresh();

            GetWindow( typeof(RpakManagerWindow), false, "Rpak Manager" );
            windowInstance.minSize = new Vector2( 650, 600 );
            windowInstance.Show();
        }

        private void OnEnable()
        {
            windowInstance = ( RpakManagerWindow )GetWindow( typeof(RpakManagerWindow), false, "Rpak Manager" );

            libraryData = FindLibraryDataFile();

            visibleIcon = Resources.Load( "icons/codeViewEnable" ) as Texture2D;
            hiddenIcon = Resources.Load( "icons/codeViewDisable" ) as Texture2D;

            Refresh();
        }

        private void OnGUI()
        {
            itemStart = currentPage * itemsPerPage;
            itemEnd = itemStart + itemsPerPage;

            dataContent = searchMode ? GetSearchResult() : rpakData.Data;

            maxPage = modelCount == 0 ? 1 : dataContent.Count / itemsPerPage + 1;

            if ( modelCount != dataContent.Count ) GetModelCount();

            GUILayout.BeginVertical( "box" );

            ShowToolBar();

            WindowUtility.WindowUtility.SeparatorAutoWidth( windowInstance, -16 );

            ShowRpakData();

            if ( rpakData != rpakDataTemp )
            {
                Refresh();
                rpakDataTemp = rpakData;
                currentPage = 0;
            }

            if ( itemEnd > dataContent.Count ) currentPage = dataContent.Count / itemsPerPage;

            WindowUtility.WindowUtility.SeparatorAutoWidth( windowInstance, -16 );

            WindowUtility.WindowUtility.Space( 2 );

            TabInfo();

            WindowUtility.WindowUtility.Space( 2 );

            WindowUtility.WindowUtility.SeparatorAutoWidth( windowInstance, -16 );

            RpakUtility();

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
                if ( searchMode )
                    WindowUtility.WindowUtility.CreateTextInfoCentered( "No results." );
                else
                    WindowUtility.WindowUtility.CreateTextInfoCentered( "List is empty." );
                WindowUtility.WindowUtility.FlexibleSpace();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                return;
            }

            for ( int i = itemStart; i < itemEnd && i < dataContent.Count; i++ )
            {
                string model = dataContent[i];

                if ( searchMode && entry.Length >= 3 )
                {
                    string[] terms = entry.Replace( " ", "" ).Split( ";" );

                    bool found = false;

                    foreach ( string term in terms )
                        if ( model.Contains( term ) )
                        {
                            found = true;
                            break;
                        }

                    if ( !found ) continue;
                }

                GUILayout.BeginHorizontal( "box" );
                GUILayout.Label( model, GUILayout.Height( 20 ) );
                WindowUtility.WindowUtility.CreateCopyButton( "Copy", "", model, 100, 20 );
                if ( !rpakData.IsSpecial )
                    if ( WindowUtility.WindowUtility.CreateButton( "Remove", "", () => RemoveModel( model ), 100, 20 ) )
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
            int idx;
            int totalButtons;
            int tabPerLine;

            var special = libraryData.GetSpecialData();
            var r5reloaded = libraryData.GetR5ReloadedData();
            var retail = libraryData.GetRetailData();

            GUILayout.BeginVertical();
            idx = 0;
            totalButtons = special.Count;
            // Calculate how many buttons should be in each line
            tabPerLine = totalButtons > maxTabPerLine ? Mathf.CeilToInt( ( float )totalButtons / Mathf.CeilToInt( ( float )totalButtons / maxTabPerLine ) ) : totalButtons;

            GUILayout.Label( "Special:", EditorStyles.boldLabel, GUILayout.Width( 400 ), GUILayout.Height( 20 ) );

            GUILayout.BeginHorizontal();
            foreach ( var data in special )
            {
                WindowUtility.WindowUtility.CreateButton( $"{data.Name}", "", () =>
                {
                    rpakData = data;
                    rpakType = rpakTypes[0];
                } );
                idx++;
                if ( idx % tabPerLine == 0 && idx != 0 )
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            WindowUtility.WindowUtility.SeparatorAutoWidth( windowInstance, -16 );

            GUILayout.BeginVertical();
            idx = 0;
            totalButtons = r5reloaded.Count;
            // Calculate how many buttons should be in each line
            tabPerLine = totalButtons > maxTabPerLine ? Mathf.CeilToInt( ( float )totalButtons / Mathf.CeilToInt( ( float )totalButtons / maxTabPerLine ) ) : totalButtons;

            GUILayout.BeginHorizontal();
            GUILayout.Label( "R5Reloaded:", EditorStyles.boldLabel, GUILayout.Width( 400 ), GUILayout.Height( 20 ) );
            WindowUtility.WindowUtility.FlexibleSpace();
            WindowUtility.WindowUtility.CreateButton( "Add R5Reloaded Rpak", "", () => AddNewRpakList( RpakType.R5Reloaded ), 200, 20 );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            foreach ( var data in r5reloaded )
            {
                WindowUtility.WindowUtility.CreateButton( $"{data.Name}", "", () =>
                {
                    rpakData = data;
                    rpakType = rpakTypes[1];
                } );
                idx++;
                if ( idx % tabPerLine == 0 && idx != 0 )
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            WindowUtility.WindowUtility.SeparatorAutoWidth( windowInstance, -16 );

            GUILayout.BeginVertical();
            idx = 0;
            totalButtons = retail.Count;
            // Calculate how many buttons should be in each line
            tabPerLine = totalButtons > maxTabPerLine ? Mathf.CeilToInt( ( float )totalButtons / Mathf.CeilToInt( ( float )totalButtons / maxTabPerLine ) ) : totalButtons;

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Retail:", EditorStyles.boldLabel, GUILayout.Width( 400 ), GUILayout.Height( 20 ) );
            WindowUtility.WindowUtility.FlexibleSpace();
            WindowUtility.WindowUtility.CreateButton( "Add Retail Rpak", "", () => AddNewRpakList( RpakType.Retail ), 200, 20 );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            foreach ( var data in retail )
            {
                WindowUtility.WindowUtility.CreateButton( $"{data.Name}", "", () =>
                {
                    rpakData = data;
                    rpakType = rpakTypes[2];
                } );
                idx++;
                if ( idx % tabPerLine == 0 && idx != 0 )
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void ShowToolBar()
        {
            GUILayout.BeginHorizontal();
            WindowUtility.WindowUtility.CreateButton( "Build all_models", "", () => BuildAllModels(), 140, 20 );
            WindowUtility.WindowUtility.CreateButton( "Build r5reloaded list", "", () => BuildR5ReloadedList(), 140, 20 );
            WindowUtility.WindowUtility.CreateButton( "Build retail list", "", () => BuildRetailList(), 140, 20 );

#if RMAPDEV
            WindowUtility.WindowUtility.GetEditorWindowSize( windowInstance );
#endif

            GUILayout.FlexibleSpace();

            WindowUtility.WindowUtility.CreateButton( "Refresh", "", () => Refresh(), 100, 20 );
            WindowUtility.WindowUtility.CreateButton( "Save", "", () => SaveJson(), 100, 20 );
            GUILayout.EndHorizontal();
        }

        private void TabInfo()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            int end = itemEnd > dataContent.Count ? dataContent.Count : itemEnd;

            string countShow = rpakData.Data.Count == modelCount ? $"{modelCount}" : $"{modelCount} / {rpakData.Data.Count}";

            WindowUtility.WindowUtility.ShowPageInfo( currentPage, maxPage, itemStart, end, $"Total Models: {countShow} | Page" );

            GUILayout.FlexibleSpace();

            WindowUtility.WindowUtility.CreateButton( "Previous Page", "", () =>
            {
                if ( currentPage > 0 ) currentPage--;
            }, 100, 20 );

            WindowUtility.WindowUtility.CreateButton( "Next Page", "", () =>
            {
                if ( itemEnd < dataContent.Count ) currentPage++;
            }, 100, 20 );

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            WindowUtility.WindowUtility.CreateToggle( ref searchMode, "Search Mode", "", 80 );
            WindowUtility.WindowUtility.CreateTextField( ref entry, "", "", 0, 0, 0, true );
            if ( !rpakData.IsSpecial ) WindowUtility.WindowUtility.CreateButton( "Add Model", "To add several models, the script separates\nthe entry from all \";\"", () => AddModel(), 100, 20 );
            if ( !rpakData.IsSpecial ) WindowUtility.WindowUtility.CreateButton( "Rename Folder", "", () => RenameTab(), 100, 20 );
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void RpakUtility()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label( $"Rpak Open => '{rpakType}/{rpakData.Name}'", EditorStyles.boldLabel, GUILayout.Width( 20 ), GUILayout.Width( 400 ) );

            GUILayout.FlexibleSpace();

            if ( !rpakData.IsSpecial ) WindowUtility.WindowUtility.CreateButton( "Remove Rpak", "", () => DeleteRpakList(), 100, 20 );
            if ( !rpakData.IsSpecial ) WindowUtility.WindowUtility.CreateButton( "Delete List", "", () => DeleteList(), 100, 20 );

            var buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleLeft;

            var buttonContentInfo = new GUIContent( rpakData.IsHidden ? "Hidden" : "Visible", rpakData.IsHidden ? hiddenIcon : visibleIcon );

            if ( GUILayout.Button( buttonContentInfo, buttonStyle, GUILayout.Height( 20 ), GUILayout.Width( 76 ) ) )
                rpakData.IsHidden = !rpakData.IsHidden;
            GUILayout.EndHorizontal();
        }

        private static void AddNewRpakList( RpakType rpakType )
        {
            var data = NewRpakData();

            switch ( rpakType )
            {
                case RpakType.Special:
                    data.IsSpecial = true;
                    break;

                case RpakType.Retail:
                    data.IsRetail = true;
                    break;

                default: // R5Reloaded
                    data.IsRetail = false;
                    break;
            }

            libraryData.RpakList.Add( data );

            rpakData = data;

            SaveJson();
        }

        internal static void DeleteRpakList()
        {
            if ( !LibrarySorterWindow.CheckDialog( "Delete Rpak List", $"Are you sure you want delete this rpak list ?\n\n'{rpakType}/{rpakData.Name}'" ) ) return;

            libraryData.RpakList.Remove( rpakData );

            rpakData = libraryData.RpakList[libraryData.RpakList.Count - 2];

            BuildAllModels();

            SaveJson();
        }

        internal static void AddModel()
        {
            if ( string.IsNullOrEmpty( entry ) ) return;

            string[] models = entry.Replace( " ", "" ).Split( ";" );

            foreach ( string model in models )
            {
                if ( rpakData.Data.Contains( model ) )
                    continue;

                if ( string.IsNullOrEmpty( model ) )
                    continue;

                if ( !model.Contains( "mdl/" ) )
                    continue;

                if ( !model.Contains( ".rmdl" ) )
                    continue;

                rpakData.Data.Add( model );
            }

            rpakData.Data.Sort();

            entry = "";

            SaveJson();
        }

        internal static void DeleteList()
        {
            if ( !LibrarySorterWindow.CheckDialog( "Delete Rpak Content", $"Are you sure you want delete this rpak content ?\n\n'{rpakType}/{rpakData.Name}'" ) ) return;

            rpakData.Data = new List< string >();

            SaveJson();
        }

        internal static void RemoveModel( string model )
        {
            if ( !LibrarySorterWindow.CheckDialog( "Remove Model", $"Are you sure you want remove this model ?\n\n'{model}'" ) ) return;

            rpakData.Data.Remove( model );

            SaveJson();
        }

        internal static void RenameTab()
        {
            if ( string.IsNullOrEmpty( entry ) ) return;

            string newEntry = entry;
            int idx = 1;
            bool nameExists;

            do
            {
                nameExists = false;
                foreach ( var content in libraryData.RpakList )
                    if ( content.Name == newEntry )
                    {
                        nameExists = true;
                        newEntry = $"{entry} ({++idx})";
                        break;
                    }
            } while (nameExists);

            if ( !LibrarySorterWindow.CheckDialog( "Rename Tab", $"Are you sure you want rename this tab ?\n\n'{rpakType}/{rpakData.Name}' => '{rpakType}/{newEntry}'" ) ) return;

            rpakData.Name = newEntry;

            entry = "";

            SaveJson();
        }

        internal static void SaveJson()
        {
            string json = JsonUtility.ToJson( libraryData );
            File.WriteAllText( rpakManagerPath, json );

            Refresh();
        }

        internal static void BuildAllModels()
        {
            var allModels = new List< string >();

            var allModelsData = libraryData.AllModels();

            foreach ( var data in libraryData.GetR5ReloadedData() )
            foreach ( string model in data.Data )
                if ( !allModels.Contains( model ) )
                    allModels.Add( model );

            if ( !Helper.IsValid( allModelsData ) )
            {
                allModelsData = NewRpakData();
                allModelsData.Name = allModelsDataName;
                allModelsData.IsSpecial = true;

                libraryData.RpakList.Add( allModelsData );
            }

            allModelsData.Data = allModels;
            allModelsData.UpdateTime();

            SaveJson();
        }

        internal static void BuildR5ReloadedList()
        {
            var r5rList = new List< string >();

            var r5rModelsData = libraryData.R5ReloadedList();

            foreach ( var data in libraryData.GetR5ReloadedData() )
            foreach ( string model in data.Data )
                if ( !r5rList.Contains( model ) )
                    r5rList.Add( model );

            if ( !Helper.IsValid( r5rModelsData ) )
            {
                r5rModelsData = NewRpakData();
                r5rModelsData.Name = r5reloadedModelsDataName;
                r5rModelsData.IsSpecial = true;
                r5rModelsData.IsHidden = true;

                libraryData.RpakList.Add( r5rModelsData );
            }

            r5rModelsData.Data = r5rList;
            r5rModelsData.UpdateTime();

            SaveJson();
        }

        internal static void BuildRetailList()
        {
            var retailList = new List< string >();

            var retailListModelsData = libraryData.AllModelsRetail();

            foreach ( var data in libraryData.GetRetailData() )
            foreach ( string model in data.Data )
                if ( !retailList.Contains( model ) )
                    retailList.Add( model );

            if ( !Helper.IsValid( retailListModelsData ) )
            {
                retailListModelsData = NewRpakData();
                retailListModelsData.Name = allModelsRetailDataName;
                retailListModelsData.IsSpecial = true;
                retailListModelsData.IsHidden = true;

                libraryData.RpakList.Add( retailListModelsData );
            }

            retailListModelsData.Data = retailList;
            retailListModelsData.UpdateTime();

            SaveJson();
        }

        internal static void CreateNewJsonLibraryData()
        {
            libraryData = new LibraryData();
            libraryData.RpakList = new List< RpakData >();

            var allModelsData = NewRpakData();
            allModelsData.Name = allModelsDataName;

            libraryData.RpakList.Add( allModelsData );

            string json = JsonUtility.ToJson( libraryData );
            File.WriteAllText( rpakManagerPath, json );

            SaveJson();
        }

        internal static LibraryData FindLibraryDataFile()
        {
            if ( !File.Exists( rpakManagerPath ) )
                CreateNewJsonLibraryData();

            string json = File.ReadAllText( rpakManagerPath );
            libraryData = JsonUtility.FromJson< LibraryData >( json );

            return libraryData;
        }

        private static RpakData NewRpakData()
        {
            var rpak = new RpakData();
            rpak.Name = "unnamed";
            rpak.Data = new List< string >();
            rpak.UpdateTime();

            return rpak;
        }

        private static void Refresh()
        {
            // Sort by priorities
            libraryData.RpakList = libraryData.RpakList
                .OrderByDescending( x => x.Name == allModelsDataName )
                .ThenByDescending( x => x.Name == r5reloadedModelsDataName )
                .ThenByDescending( x => x.Name == allModelsRetailDataName )
                .ThenBy( x => x.Name )
                .ToList();

            var arrays = new List< List< string > >();

            var tab = new List< string >();

            foreach ( var data in libraryData.RpakList )
            {
                if ( tab.Count == maxTabPerLine )
                {
                    arrays.Add( tab );
                    tab = new List< string >();
                }

                tab.Add( data.Name );
            }

            if ( tab.Count > 0 )
                arrays.Add( tab );

            rpakTab = arrays.Select( a => a.ToArray() ).ToArray();
        }

        private static void GetModelCount()
        {
            if ( searchMode && entry.Length < 3 )
            {
                modelCount = 0;
                return;
            }
            if ( searchMode && entry.Length >= 3 )
            {
                modelCount = 0;

                foreach ( string model in rpakData.Data )
                {
                    string[] terms = entry.Replace( " ", "" ).Split( ";" );

                    bool found = false;

                    foreach ( string term in terms )
                        if ( model.Contains( term ) )
                        {
                            found = true;
                            break;
                        }

                    if ( found ) modelCount++;
                }
            }
            else
            {
                modelCount = rpakData.Data.Count;
            }
        }

        private static List< string > GetSearchResult()
        {
            var list = new List< string >();

            foreach ( string model in rpakData.Data )
            {
                string[] terms = entry.Replace( " ", "" ).Split( ";" );

                bool found = false;

                foreach ( string term in terms )
                {
                    if ( string.IsNullOrEmpty( term ) ) continue;

                    if ( model.Contains( term ) )
                    {
                        found = true;
                        break;
                    }
                }

                if ( found ) list.Add( model );
            }

            return list;
        }

        private enum RpakType
        {
            Special,
            R5Reloaded,
            Retail
        }
    }
}