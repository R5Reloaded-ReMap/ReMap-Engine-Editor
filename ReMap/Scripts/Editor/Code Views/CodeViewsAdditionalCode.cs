
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
        private static string relativePathInBlockAdditionalCode = UnityInfo.relativePathInBlockAdditionalCode;
        
        internal static AdditionalCodeWindow windowInstance;
        internal static int tabIdx = 0;
        internal static int tabIdxTemp = 0;
        internal static string[] toolbarTab = new string[0];
        internal static Vector2 scroll;

        internal static AdditionalCode inBlockAdditionalCode;
        private static string entry = "";
        private static string emptyContentStr = "Empty Code";

        public static void Init()
        {
            inBlockAdditionalCode = FindAdditionalCode();

            Refresh();

            windowInstance = ( AdditionalCodeWindow ) GetWindow( typeof( AdditionalCodeWindow ), false, "Additional Code" );
            windowInstance.Show();
        }

        private void OnEnable()
        {
            inBlockAdditionalCode = FindAdditionalCode();

            Refresh();
        }

        private void EditorSceneManager_sceneSaved( UnityEngine.SceneManagement.Scene arg0 )
        {
            inBlockAdditionalCode = FindAdditionalCode();

            Refresh();
        }

        private void EditorSceneManager_sceneOpened( UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode )
        {
            inBlockAdditionalCode = FindAdditionalCode();

            Refresh();
        }

        void OnGUI()
        {
            bool isEmptyCode = true;

            if ( inBlockAdditionalCode.Content[ tabIdx ].Name != emptyContentStr ) isEmptyCode = false;

            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal();
                    tabIdx = GUILayout.Toolbar( tabIdx, toolbarTab );
                    CreateButton( "Refresh", "Refresh Window", () => Refresh( true ), 100, 20 );
                GUILayout.EndHorizontal();

                if( tabIdx != tabIdxTemp )
                {
                    Refresh( true );
                    tabIdxTemp = tabIdx;
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
                    inBlockAdditionalCode.Content[ tabIdx ].Code = EditorGUILayout.TextArea( inBlockAdditionalCode.Content[ tabIdx ].Code, GUILayout.ExpandHeight( true ) );
                EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        internal static void AddNewCode()
        {
            inBlockAdditionalCode.Content.Add( NewAdditionalCodeContent() );

            tabIdx = toolbarTab.Length;

            SaveJson();
        }

        internal static void DeleteCode()
        {
            if ( !LibrarySorter.LibrarySorterWindow.CheckDialog( "Delete Code", "Are you sure you want delete this code ?" ) ) return;

            inBlockAdditionalCode.Content.Remove( inBlockAdditionalCode.Content[ tabIdx ] );

            tabIdx = toolbarTab.Length - 2;

            SaveJson();
        }

        internal static void RenameTab()
        {
            inBlockAdditionalCode.Content[ tabIdx ].Name = entry;

            entry = "";

            SaveJson();
        }

        public static AdditionalCode FindAdditionalCode()
        {
            if ( !File.Exists( relativePathInBlockAdditionalCode ) )
            {
                CreateNewJsonAdditionalCode();
            }

            string json = System.IO.File.ReadAllText( relativePathInBlockAdditionalCode );
            inBlockAdditionalCode = JsonUtility.FromJson< AdditionalCode >( json );

            return inBlockAdditionalCode;
        }

        internal static void CreateNewJsonAdditionalCode()
        {
            inBlockAdditionalCode = new AdditionalCode();
            inBlockAdditionalCode.Content = new List< AdditionalCodeContent >();

            AdditionalCodeContent emptyContent = NewAdditionalCodeContent();
            emptyContent.Name = emptyContentStr;

            inBlockAdditionalCode.Content.Add( emptyContent );

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
            string json = JsonUtility.ToJson( inBlockAdditionalCode );
            System.IO.File.WriteAllText( relativePathInBlockAdditionalCode, json );

            Refresh();
        }

        internal static void Refresh( bool refreshCodeView = false )
        {
            UnityInfo.SortListByKey( inBlockAdditionalCode.Content, x => x.Name );

            int targetIndex = -1;

            List< string > pages = new List< string >();

            for ( int i = 0; i < inBlockAdditionalCode.Content.Count; i++ )
            {
                AdditionalCodeContent content = inBlockAdditionalCode.Content[ i ];
                pages.Add( content.Name );

                if ( content.Name == emptyContentStr )
                {
                    targetIndex = i;
                }
            }

            if ( targetIndex != -1 )
            {
                AdditionalCodeContent targetContent = inBlockAdditionalCode.Content[ targetIndex ];
                inBlockAdditionalCode.Content.RemoveAt( targetIndex );
                inBlockAdditionalCode.Content.Insert( 0, targetContent );

                pages.RemoveAt( targetIndex );
                pages.Insert( 0, emptyContentStr );
            }

            if ( CodeViewsWindow.windowInstance != null && refreshCodeView ) CodeViewsWindow.Refresh();

            toolbarTab = pages.ToArray();
        }
    }

    [Serializable]
    public class AdditionalCode
    {
        public List < AdditionalCodeContent > Content;
    }

    [Serializable]
    public class AdditionalCodeContent
    {
        public string Name;
        public string Code;
    }
}
