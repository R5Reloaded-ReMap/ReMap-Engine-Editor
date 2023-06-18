using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WindowUtility
{
    public class WindowStruct
    {
        public string[] MainTab { get; set; }
        public Dictionary< int, string[] > SubTab { get; set; }
        public Dictionary< Tuple< int, int >, GUIStruct > SubTabGUI { get; set; }
        public FunctionRef MainTabCallback { get; set; }
        public FunctionRef SubTabCallback { get; set; }
        public FunctionRef RefreshCallback { get; set; }

        public int MainTabIdx = 0;
        public int SubTabIdx = 0;
        private int MainTabIdxTemp = 0;
        private int SubTabIdxTemp = 0;

        private Dictionary< int, int > SavedSubTabIdx = new Dictionary< int, int >();

        public void ShowTab()
        {
            if ( !Helper.IsValid( MainTab ) || Helper.IsEmpty( MainTab ) ) return;

            GUILayout.BeginHorizontal();
                MainTabIdx = GUILayout.Toolbar ( MainTabIdx, MainTab );
                if ( Helper.IsValid( MainTabCallback ) )
                {
                    MainTabCallback();
                }
            GUILayout.EndHorizontal();

            if ( SubTab.ContainsKey( MainTabIdx ) )
            {
                if ( !Helper.IsValid( SubTab[ MainTabIdx ] ) || !Helper.IsEmpty( SubTab[ MainTabIdx ] ) )
                {
                    GUILayout.BeginHorizontal();
                        SubTabIdx = GUILayout.Toolbar ( SubTabIdx, SubTab[ MainTabIdx ] );
                        if ( Helper.IsValid( SubTabCallback ) )
                        {
                            SubTabCallback();
                        }
                    GUILayout.EndHorizontal();
                }
            }

            if ( MainTabIdx != MainTabIdxTemp )
            {
                SavePageIdx();
                MainTabIdxTemp = MainTabIdx;
                LoadPageIdx();
                CommonChanges();
            }

            if ( SubTabIdx != SubTabIdxTemp )
            {
                CommonChanges();
            }
        }

        public void ShowFunc( int idx = 0 )
        {
            Tuple< int, int > index = GetCurrentTabIdx();

            if ( !SubTabGUI.ContainsKey( index ) ) return;

            GUIStruct str = SubTabGUI[ index ];

            if ( Helper.IsValid( str ) )
            {
                if ( Helper.IsValid( str.OnGUI[ idx ] ) ) str.OnGUI[ idx ]();
            }
        }

        private void CommonChanges()
        {
            Tuple< int, int > index = GetCurrentTabIdx();

            if ( !SubTabGUI.ContainsKey( index ) ) return;

            GUIStruct str = SubTabGUI[ index ];

            if ( Helper.IsValid( str ) )
            {
                if ( Helper.IsValid( str.OnStartGUI ) ) str.OnStartGUI();
            }

            if ( Helper.IsValid( RefreshCallback ) ) RefreshCallback();
        }

        private void SavePageIdx()
        {
            SavedSubTabIdx[ MainTabIdxTemp ] = SubTabIdx;
        }

        private void LoadPageIdx()
        {
            SubTabIdx = SavedSubTabIdx.ContainsKey( MainTabIdx ) ? SavedSubTabIdx[ MainTabIdx ] : 0;
            SubTabIdxTemp = SubTabIdx;
        }

        public Tuple< int, int > GetCurrentTabIdx()
        {
            return NewTuple( MainTabIdx, SubTabIdx );
        }

        public static Tuple< int, int > NewTuple( int i, int j )
        {
            return Tuple.Create( i, j );
        }
    }

    public class GUIStruct
    {
        public string Name { get; set; }
        public FunctionRef[] OnGUI { get; set; }
        public FunctionRef OnStartGUI { get; set; }
    }
}
