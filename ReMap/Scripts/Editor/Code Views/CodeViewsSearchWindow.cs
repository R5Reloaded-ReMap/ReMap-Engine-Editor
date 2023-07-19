
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

        internal static int SearchedString = -1;

        private static Dictionary< int, string > Result = new Dictionary< int, string >();

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

            string maxReach = Result.Count > 200 ? $"|| The number of result displays is reached ( {Result.Count - 200} more )" : "";

            EditorGUILayout.BeginHorizontal();
                CreateTextInfo( $"Results: {ResultIdx} {maxReach}", "" );
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
                    int resultShowed = 0;
                    Scroll = EditorGUILayout.BeginScrollView( Scroll );
                    foreach ( var result in Result )
                    {
                        EditorGUILayout.BeginHorizontal();
                            CreateTextInfo( result.Value, "", windowInstance.position.width - 226 );
                            CreateButton( "Go To Page", "", () => CodeViewsWindow.GoToPage( result.Key ), 100, 20 );
                            CreateButton( "Copy", "", () => CreateCopyButton( result.Value ), 100, 20 );
                        EditorGUILayout.EndHorizontal();

                        if ( resultShowed++ == 200 ) break;
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
            Result = new Dictionary< int, string >(); int i = 0;

            string[] terms = search.Split( ";" );

            foreach( string line in CodeViewsWindow.codeSplit )
            {
                foreach ( string term in terms )
                {
                    if ( line.Contains( term, StringComparison.OrdinalIgnoreCase ) && !string.IsNullOrEmpty( term ) && term.Length >= 3 && !Result.ContainsKey( i ) )
                    {
                        Result.Add( i, line );
                    }
                }

                i++;
            }

            ResultIdx = Result.Count;
        }
    }
}