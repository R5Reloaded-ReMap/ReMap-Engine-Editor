
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
        internal static Vector2 scroll;

        internal static AdditionalCode additionalCode;
        private static string entry = "";
        private static string emptyContentStr = "Empty Code";

        public static void Init()
        {
            additionalCode = FindAdditionalCode();

            Refresh();

            windowInstance = ( AdditionalCodeWindow ) GetWindow( typeof( AdditionalCodeWindow ), false, "Additional Code" );
            windowInstance.Show();
        }

        private void OnEnable()
        {
            additionalCode = FindAdditionalCode();

            Refresh();
        }

        private void EditorSceneManager_sceneSaved( UnityEngine.SceneManagement.Scene arg0 )
        {
            additionalCode = FindAdditionalCode();

            Refresh();
        }

        private void EditorSceneManager_sceneOpened( UnityEngine.SceneManagement.Scene arg0, OpenSceneMode mode )
        {
            additionalCode = FindAdditionalCode();

            Refresh();
        }

        void OnGUI()
        {
            bool isEmptyCode = true;

            if ( additionalCode.Content[ tabIdx ].Name != emptyContentStr ) isEmptyCode = false;

            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal();
                    tabIdx = GUILayout.Toolbar( tabIdx, toolbarTab );
                    CreateButton( "Refresh", "Refresh Window", () => Refresh( true ), 100, 20 );
                GUILayout.EndHorizontal();

                if( tabIdx != tabIdxTemp )
                {
                    Refresh();
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
                    additionalCode.Content[ tabIdx ].Code = EditorGUILayout.TextArea( additionalCode.Content[ tabIdx ].Code, GUILayout.ExpandHeight( true ) );
                EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        internal static void AddNewCode()
        {
            additionalCode.Content.Add( NewAdditionalCodeContent() );

            tabIdx = toolbarTab.Length;

            SaveJson();
        }

        internal static void DeleteCode()
        {
            if ( !LibrarySorter.LibrarySorterWindow.CheckDialog( "Delete Code", "Are you sure you want delete this code ?" ) ) return;

            additionalCode.Content.Remove( additionalCode.Content[ tabIdx ] );

            tabIdx = toolbarTab.Length - 2;

            SaveJson();
        }

        internal static void RenameTab()
        {
            additionalCode.Content[ tabIdx ].Name = entry;

            entry = "";

            SaveJson();
        }

        public static AdditionalCode FindAdditionalCode()
        {
            if ( !File.Exists( relativePathAdditionalCode ) )
            {
                CreateNewJsonAdditionalCode();
            }

            string json = System.IO.File.ReadAllText( relativePathAdditionalCode );
            additionalCode = JsonUtility.FromJson< AdditionalCode >( json );

            return additionalCode;
        }

        internal static void CreateNewJsonAdditionalCode()
        {
            additionalCode = new AdditionalCode();
            additionalCode.Content = new List< AdditionalCodeContent >();

            AdditionalCodeContent emptyContent = NewAdditionalCodeContent();
            emptyContent.Name = emptyContentStr;

            additionalCode.Content.Add( emptyContent );

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
            string json = JsonUtility.ToJson( additionalCode );
            System.IO.File.WriteAllText( relativePathAdditionalCode, json );

            Refresh();
        }

        internal static void Refresh( bool refreshCodeView = false )
        {
            UnityInfo.SortListByKey( additionalCode.Content, x => x.Name );

            int targetIndex = -1;

            List< string > pages = new List< string >();

            for ( int i = 0; i < additionalCode.Content.Count; i++ )
            {
                AdditionalCodeContent content = additionalCode.Content[ i ];
                pages.Add( content.Name );

                if ( content.Name == emptyContentStr )
                {
                    targetIndex = i;
                }
            }

            if ( targetIndex != -1 )
            {
                AdditionalCodeContent targetContent = additionalCode.Content[ targetIndex ];
                additionalCode.Content.RemoveAt( targetIndex );
                additionalCode.Content.Insert( 0, targetContent );

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
