
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

using static WindowUtility.WindowUtility;

namespace CodeViews
{
    public class CodeViewsSearchWindow : EditorWindow
    {
        private static CodeViewsSearchWindow windowInstance;

        private static Vector2 Scroll;
        private static string Entry = "";
        private static string SearchEntry = "";

        private static List< string > Result = new List< string >();

        private static int ResultIdx = 0;
        
        public static void Init()
        {
            windowInstance = ( CodeViewsSearchWindow ) GetWindow( typeof( CodeViewsSearchWindow ), false, "Code Search" );
            windowInstance.minSize = new Vector2( 1130, 400 );
            windowInstance.minSize = new Vector2( 800, 400 );
            windowInstance.Show();
        }

        void OnGUI()
        {
            if ( !Helper.IsValid( windowInstance ) )
            {
                windowInstance = ( CodeViewsSearchWindow ) GetWindow( typeof( CodeViewsSearchWindow ), false, "Code Search" );
            }

            EditorGUILayout.BeginHorizontal();
                CreateTextInfo( $"Results: {ResultIdx}", "" );
                #if ReMapDev
                    GetEditorWindowSize( windowInstance );
                #endif
            EditorGUILayout.EndHorizontal();

            CreateTextField( ref SearchEntry, ( float ) windowInstance.position.width - 6, 20 );
            SeparatorAutoWidth( windowInstance, -8 );

            Space( 2 );

            if ( SearchEntry.Length >= 3 )
            {
                if ( SearchEntry != Entry )
                {
                    Entry = SearchEntry;
                    SearchResult( Entry );
                }

                if ( Result.Count != 0 )
                {
                    Scroll = EditorGUILayout.BeginScrollView( Scroll );
                    foreach ( string line in Result )
                    {
                        EditorGUILayout.BeginHorizontal();
                            CreateTextInfo( line, "", windowInstance.position.width - 122 );
                            CreateButton( "Copy", "", () => CreateCopyButton( line ), 100, 20 );
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    FlexibleSpace();
                        CreateTextInfoCentered( "No results.", "" );
                    FlexibleSpace();
                }
            }
            else
            {
                FlexibleSpace();
                    CreateTextInfoCentered( "Search must be at least 3 characters long.", "" );
                FlexibleSpace();

                ResultIdx = 0;
            }

            Space( 2 );
        }

        public static void Refresh()
        {
            SearchResult( Entry );
        }

        private static void SearchResult( string search )
        {
            Result = new List< string >();

            foreach( string line in CodeViewsWindow.codeSplit )
            {
                if ( line.Contains( SearchEntry ) )
                {
                    Result.Add( line );
                }
            }

            ResultIdx = Result.Count;
        }
    }
}