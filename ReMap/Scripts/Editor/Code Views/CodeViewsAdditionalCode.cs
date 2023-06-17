
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WindowUtility;
using static WindowUtility.WindowUtility;

namespace CodeViews
{
    public class AdditionalCodeWindow : EditorWindow
    {
        internal static AdditionalCodeWindow windowInstance;
        private static Vector2 scroll;

        // Info
        private static bool showInfo = false;
        private static string textInfo = File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathAdditionalCodeInfo}" );
        private static string textInfoTemp = textInfo;

        internal static AdditionalCode additionalCode;
        internal static AdditionalCodeClass[] additionalCodeArray;
        private static AdditionalCodeClass activeCode;
        private static string entry = "";
        internal static string emptyContentStr = "Empty Code";

        internal static string[] contentType = new[] { "HeadContent", "InBlockContent", "BelowContent" };

        public static void Init()
        {
            AdditionalCodeInit();

            windowInstance = ( AdditionalCodeWindow ) GetWindow( typeof( AdditionalCodeWindow ), false, "Additional Code" );
            windowInstance.minSize = new Vector2( 1000, 500 );
            windowInstance.Show();
        }

        private void OnEnable()
        {
            AdditionalCodeInit();
        }

        private void EditorSceneManager_sceneSaved( UnityEngine.SceneManagement.Scene arg0 )
        {
            AdditionalCodeInit();
        }

        private void EditorSceneManager_sceneOpened( UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode )
        {
            AdditionalCodeInit();
        }

        void OnGUI()
        {
            if ( !Helper.IsValid( additionalCode ) ) return;

            bool isEmptyCode = true;

            if ( activeCode.Content[ activeCode.tabCodeIdx ].Name != emptyContentStr ) isEmptyCode = false;

            CodeViewsWindow.ShortCut();

            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal();
                    additionalCode.tabIdx = GUILayout.Toolbar( additionalCode.tabIdx, additionalCode.toolbarTab );
                    CreateButton( "Refresh", "Refresh Window", () => Refresh( true ), 100 );
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    activeCode.tabCodeIdx = GUILayout.Toolbar( activeCode.tabCodeIdx, activeCode.toolbarCodeTab );
                    CreateButton( "Info", "", () => { showInfo = !showInfo; }, 100 );
                GUILayout.EndHorizontal();

                if( additionalCode.tabIdx != additionalCode.tabIdxTemp || activeCode.tabCodeIdx != activeCode.tabCodeIdxTemp )
                {
                    additionalCode.tabIdxTemp = additionalCode.tabIdx;
                    activeCode.tabCodeIdxTemp = activeCode.tabCodeIdx;
                    GUI.FocusControl( null );
                    showInfo = false;
                    Refresh( true );
                }

                GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

                GUILayout.BeginHorizontal();
                    if ( !isEmptyCode ) CreateTextField( ref entry, "", "", 1, 200, 20, true );
                    if ( !isEmptyCode ) CreateButton( "Rename", "", () => RenameTab(), 100, 20 );
                    GUILayout.FlexibleSpace();
                    CreateButton( "Add Code", "", () => AddNewCode(), 100, 20 );
                    if ( !isEmptyCode ) CreateButton( "Remove Code", "", () => DeleteCode(), 100, 20 );
                    CreateButton( "Save", "", () => SaveJson(), 100, 20 );
                GUILayout.EndHorizontal();

                CodeViewsMenu.Space( 4 );

                scroll = EditorGUILayout.BeginScrollView( scroll );
                    if ( showInfo )
                    {
                        textInfoTemp = textInfo;
                        EditorGUILayout.TextArea( textInfoTemp, GUILayout.ExpandHeight( true ) );
                    }
                    else if ( !isEmptyCode )
                    {
                        activeCode.Content[ activeCode.tabCodeIdx ].Code = EditorGUILayout.TextArea( activeCode.Content[ activeCode.tabCodeIdx ].Code, GUILayout.ExpandHeight( true ) );
                    }
                EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        internal static void AddNewCode()
        {
            AdditionalCodeContent newContent = NewAdditionalCodeContent();

            activeCode.Content.Add( newContent );

            int position = activeCode.Content.IndexOf( newContent ) - 1;

            if ( position != -1 ) activeCode.tabCodeIdx = position;

            SaveJson();
        }

        internal static void DeleteCode()
        {
            if ( !LibrarySorter.LibrarySorterWindow.CheckDialog( "Delete Code", "Are you sure you want delete this code ?" ) ) return;

            activeCode.Content.Remove( activeCode.Content[ activeCode.tabCodeIdx ] );

            activeCode.tabCodeIdx = activeCode.toolbarCodeTab.Length - 1;

            SaveJson();
        }

        internal static void RenameTab()
        {
            if ( string.IsNullOrEmpty( entry ) ) return;

            string newEntry = FindUniqueName( entry );

            activeCode.Content[ activeCode.tabCodeIdx ].Name = newEntry;

            SaveJson();

            for ( int i = 0; i < activeCode.Content.Count - 1; i++ )
            {
                if ( activeCode.Content[ i ].Name == newEntry )
                {
                    activeCode.tabCodeIdx = i;
                    activeCode.tabCodeIdxTemp = i;
                    break;
                }
            }

            entry = "";
        }

        private static string FindUniqueName( string name )
        {
            if ( !Helper.IsValid( activeCode ) ) return "";

            string newEntry = name; int idx = 1; bool nameExists;

            do
            {
                nameExists = false;
                foreach ( AdditionalCodeContent content in activeCode.Content )
                {
                    if ( content.Name == newEntry )
                    {
                        nameExists = true;
                        newEntry = $"{name} ({++idx})";
                        break;
                    }
                }
            } while ( nameExists );

            return newEntry;
        }

        public static AdditionalCode FindAdditionalCode()
        {
            if ( !File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathAdditionalCodeJson}" ) )
            {
                CreateNewJsonAdditionalCode();
            }
            else
            {
                string json = System.IO.File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathAdditionalCodeJson}" );
                additionalCode = JsonUtility.FromJson< AdditionalCode >( json );
            }

            return additionalCode;
        }

        internal static void CreateNewJsonAdditionalCode()
        {
            additionalCode = new AdditionalCode();

            additionalCode.HeadContent = new AdditionalCodeClass();
            additionalCode.HeadContent.Name = "Head Code";
            additionalCode.HeadContent.Content = new List< AdditionalCodeContent >();

            additionalCode.InBlockContent = new AdditionalCodeClass();
            additionalCode.InBlockContent.Name = "In-Block Code";
            additionalCode.InBlockContent.Content = new List< AdditionalCodeContent >();

            additionalCode.BelowContent = new AdditionalCodeClass();
            additionalCode.BelowContent.Name = "Below Code";
            additionalCode.BelowContent.Content = new List< AdditionalCodeContent >();

            AdditionalCodeContent emptyContent = NewAdditionalCodeContent();
            emptyContent.Name = emptyContentStr;

            additionalCode.HeadContent.Content.Add( emptyContent );
            additionalCode.InBlockContent.Content.Add( emptyContent );
            additionalCode.BelowContent.Content.Add( emptyContent );

            SaveJson();
        }

        internal static AdditionalCodeContent NewAdditionalCodeContent()
        {
            AdditionalCodeContent content = new AdditionalCodeContent();

            content.Name = FindUniqueName( "unnamed" );
            content.Code = "";

            return content;
        }

        internal static void SaveJson()
        {
            if ( !Helper.IsValid( additionalCode ) ) return;

            string json = JsonUtility.ToJson( additionalCode );
            System.IO.File.WriteAllText( UnityInfo.relativePathAdditionalCodeJson, json );

            Refresh();
        }

        internal static void Refresh( bool refreshCodeView = false )
        {
            if ( !Helper.IsValid( additionalCode ) ) return;

            activeCode = additionalCodeArray[ additionalCode.tabIdx ];

            if ( activeCode.tabCodeIdx > activeCode.Content.Count - 1 )
            {
                activeCode.tabCodeIdx = activeCode.Content.Count - 1;
                activeCode.tabCodeIdxTemp = activeCode.Content.Count - 1;
            }

            List < string > nameList = new List < string >();
            foreach ( AdditionalCodeClass classes in additionalCodeArray )
            {           
                nameList.Add( classes.Name );
            }
            additionalCode.toolbarTab = nameList.ToArray();

            UnityInfo.SortListByKey( activeCode.Content, x => x.Name );

            int targetIndex = -1;

            List< string > pages = new List< string >();

            for ( int i = 0; i < activeCode.Content.Count; i++ )
            {
                AdditionalCodeContent content = activeCode.Content[ i ];
                pages.Add( content.Name );

                if ( content.Name == emptyContentStr )
                {
                    targetIndex = i;
                }
            }

            if ( targetIndex != -1 )
            {
                AdditionalCodeContent targetContent = activeCode.Content[ targetIndex ];
                activeCode.Content.RemoveAt( targetIndex );
                activeCode.Content.Insert( 0, targetContent );

                pages.RemoveAt( targetIndex );
                pages.Insert( 0, emptyContentStr );
            }

            if ( CodeViewsWindow.windowInstance != null && refreshCodeView ) CodeViewsWindow.Refresh();

            #if ReMapDev
                textInfo = File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathAdditionalCodeInfo}" );
            #endif

            activeCode.toolbarCodeTab = pages.ToArray();
        }

        internal static void AdditionalCodeInit()
        {
            additionalCode = FindAdditionalCode();

            if ( !Helper.IsValid( additionalCode ) ) return;

            additionalCodeArray = new AdditionalCodeClass[]
            {
                additionalCode.HeadContent,
                additionalCode.InBlockContent,
                additionalCode.BelowContent
            };

            activeCode = additionalCode.HeadContent;

            additionalCode.tabIdx = 0;
            additionalCode.tabIdxTemp = 0;

            foreach ( AdditionalCodeClass additionalCodeClass in additionalCodeArray )
            {
                additionalCodeClass.tabCodeIdx = 0;
                additionalCodeClass.tabCodeIdxTemp = 0;
            }

            Refresh();
        }
    }

    [Serializable]
    public class AdditionalCode
    {
        public int tabIdx;
        public int tabIdxTemp;
        public string[] toolbarTab;

        public AdditionalCodeClass HeadContent;
        public AdditionalCodeClass InBlockContent;
        public AdditionalCodeClass BelowContent;
    }

    [Serializable]
    public class AdditionalCodeClass
    {
        public int tabCodeIdx;
        public int tabCodeIdxTemp;
        public string[] toolbarCodeTab;

        public string Name;
        public List < AdditionalCodeContent > Content;
    }

    [Serializable]
    public class AdditionalCodeContent
    {
        public string Name;
        public string Code;
    }
}
