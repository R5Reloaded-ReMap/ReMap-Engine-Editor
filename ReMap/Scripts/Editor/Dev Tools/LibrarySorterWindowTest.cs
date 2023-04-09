using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class LibrarySorterWindowTest : EditorWindow
    {
        internal static LibraryData libraryData;
        Vector2 scrollPos = Vector2.zero;
        
        #if ReMapDev
            [ MenuItem( "ReMap Dev Tools/Prefabs Management/Windows/Prefab Fix Manager Test", false, 100 ) ]
            public static void Init()
            {
                libraryData = RpakManagerWindow.FindLibraryDataFile();

                LibrarySorterWindowTest window = ( LibrarySorterWindowTest )GetWindow( typeof( LibrarySorterWindowTest ), false, "Prefab Fix Manager" );
                window.minSize = new Vector2( 650, 600 );
                window.Show();
            }
        #endif

        void OnGUI()
        {
            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal();

                    WindowUtility.WindowUtility.CreateButton( "Rpak Manager", "", () => RpakManagerWindow.Init() );
                    WindowUtility.WindowUtility.CreateButton( "Fix Prefabs Tags", "", () => FixPrefabsTags() );

                GUILayout.EndHorizontal();

                scrollPos = EditorGUILayout.BeginScrollView( scrollPos );



                GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        internal static async void FixPrefabsTags()
        {
            string[] prefabs = AssetDatabase.FindAssets("t:prefab", new[] { UnityInfo.relativePathPrefabs });

            int i = 0; int total = prefabs.Length;
            foreach ( string prefab in prefabs )
            {
                string path = AssetDatabase.GUIDToAssetPath( prefab );

                if ( path.Contains( "_custom_prefabs" ) )
                {
                    i++;
                    continue;
                }

                UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath( $"{path}", typeof( UnityEngine.Object ) ) as GameObject;
                if ( loadedPrefabResource == null )
                {
                    ReMapConsole.Log( $"[Library Sorter] Error loading prefab: {path}", ReMapConsole.LogType.Error );
                    i++;
                    continue;
                }

                EditorUtility.DisplayProgressBar( $"Fixing Prefabs Tags {i}/{total}", $"Checking: {path}", ( i + 1 ) / ( float )total );

                if( loadedPrefabResource.tag != Helper.GetObjTagNameWithEnum( ObjectType.Prop ) )
                    loadedPrefabResource.tag = Helper.GetObjTagNameWithEnum( ObjectType.Prop );

                ReMapConsole.Log( $"[Library Sorter] Set {path} tag to: {Helper.GetObjTagNameWithEnum( ObjectType.Prop )}", ReMapConsole.LogType.Info );

                PrefabUtility.SavePrefabAsset(loadedPrefabResource); i++;

                await Task.Delay(TimeSpan.FromSeconds(0.001));
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
