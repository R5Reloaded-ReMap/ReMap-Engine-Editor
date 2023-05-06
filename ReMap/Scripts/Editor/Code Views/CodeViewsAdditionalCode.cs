
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

namespace CodeViewsWindow
{
    public class AdditionalCodeWindow : EditorWindow
    {
        private static string relativePathAdditionalCode = UnityInfo.relativePathAdditionalCode;
        
        internal static AdditionalCodeWindow windowInstance;
        internal static int tabIdx = 0;
        internal static int tabIdxTemp = 0;
        internal static string[] toolbarTab = new string[0];
        internal static int tabCodeIdx = 0;
        internal static int tabCodeIdxTemp = 0;
        internal static string[] toolbarCodeTab = new string[0];
        internal static Vector2 scroll;

        internal static AdditionalCode additionalCode;
        internal static AdditionalCodeClass[] additionalCodeArray;
        internal static AdditionalCodeClass activeCode;
        private static string entry = "";
        private static string emptyContentStr = "Empty Code";

        public static void Init()
        {
            AdditionalCodeInit();

            windowInstance = ( AdditionalCodeWindow ) GetWindow( typeof( AdditionalCodeWindow ), false, "Additional Code" );
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
            if ( additionalCode == null ) return;

            bool isEmptyCode = true;

            if ( activeCode.Content[ tabCodeIdx ].Name != emptyContentStr ) isEmptyCode = false;

            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal();
                    tabIdx = GUILayout.Toolbar( tabIdx, toolbarTab );
                    CreateButton( "Refresh", "Refresh Window", () => Refresh( true ), 100, 20 );
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    tabCodeIdx = GUILayout.Toolbar( tabCodeIdx, toolbarCodeTab );
                GUILayout.EndHorizontal();

                if( tabIdx != tabIdxTemp || tabCodeIdx != tabCodeIdxTemp )
                {
                    tabIdxTemp = tabIdx;
                    tabCodeIdxTemp = tabCodeIdx;
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

                if ( isEmptyCode )
                {
                    GUILayout.EndVertical();
                    return;
                }

                scroll = EditorGUILayout.BeginScrollView( scroll );
                    activeCode.Content[ tabCodeIdx ].Code = EditorGUILayout.TextArea( activeCode.Content[ tabCodeIdx ].Code, GUILayout.ExpandHeight( true ) );
                EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        internal static void AddNewCode()
        {
            activeCode.Content.Add( NewAdditionalCodeContent() );

            tabCodeIdx = toolbarCodeTab.Length - 1;

            SaveJson();
        }

        internal static void DeleteCode()
        {
            if ( !LibrarySorter.LibrarySorterWindow.CheckDialog( "Delete Code", "Are you sure you want delete this code ?" ) ) return;

            activeCode.Content.Remove( activeCode.Content[ tabCodeIdx ] );

            tabCodeIdx = toolbarCodeTab.Length - 1;

            SaveJson();
        }

        internal static void RenameTab()
        {
            foreach ( AdditionalCodeContent content in activeCode.Content )
            {
                if ( content.Name == entry ) return;
            }

            if ( entry == "" ) return;

            activeCode.Content[ tabCodeIdx ].Name = entry;

            SaveJson();

            for ( int i = 0; i < activeCode.Content.Count - 1; i++ )
            {
                if ( activeCode.Content[ i ].Name == entry )
                {
                    tabCodeIdx = i;
                    tabCodeIdxTemp = i;
                }
            }

            entry = "";
        }

        public static AdditionalCode FindAdditionalCode()
        {
            if ( !File.Exists( relativePathAdditionalCode ) )
            {
                CreateNewJsonAdditionalCode();
            }
            else
            {
                string json = System.IO.File.ReadAllText( relativePathAdditionalCode );
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

            content.Name = "unnamed";
            content.Code = "";

            return content;
        }

        internal static void SaveJson()
        {
            if ( additionalCode == null ) return;

            string json = JsonUtility.ToJson( additionalCode );
            System.IO.File.WriteAllText( relativePathAdditionalCode, json );

            Refresh();
        }

        internal static void Refresh( bool refreshCodeView = false )
        {
            activeCode = additionalCodeArray[ tabIdx ];

            if ( tabCodeIdx > activeCode.Content.Count - 1 )
            {
                tabCodeIdx = activeCode.Content.Count - 1;
                tabCodeIdxTemp = activeCode.Content.Count - 1;
            }

            List < string > nameList = new List < string >();
            foreach ( AdditionalCodeClass classes in additionalCodeArray )
            {           
                nameList.Add( classes.Name );
            }
            toolbarTab = nameList.ToArray();

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

            toolbarCodeTab = pages.ToArray();
        }

        internal static void AdditionalCodeInit()
        {
            additionalCode = FindAdditionalCode();

            additionalCodeArray = new AdditionalCodeClass[]
            {
                additionalCode.HeadContent,
                additionalCode.InBlockContent,
                additionalCode.BelowContent
            };

            Refresh();
        }
    }

    [Serializable]
    public class AdditionalCode
    {
        public AdditionalCodeClass HeadContent;
        public AdditionalCodeClass InBlockContent;
        public AdditionalCodeClass BelowContent;
    }

    [Serializable]
    public class AdditionalCodeClass
    {
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
