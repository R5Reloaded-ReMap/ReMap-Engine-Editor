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

        private bool WindowChange = false;

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
                LoadPageIdx( true );
                CommonChanges();
            }
        }

        public void ShowFunc( int idx = 0 )
        {
            GUIStruct GUIStr = GetGUIStruct();

            if ( Helper.IsValid( GUIStr ) )
            {
                if ( Helper.IsValid( GUIStr.OnGUI[ idx ] ) ) GUIStr.OnGUI[ idx ]();
            }
        }

        private void CommonChanges()
        {
            GUIStruct GUIStr = GetGUIStruct();

            if ( Helper.IsValid( GUIStr ) )
            {
                WindowChange = true;
                if ( Helper.IsValid( GUIStr.OnStartGUI ) ) GUIStr.OnStartGUI();
                WindowChange = false;
            }

            if ( Helper.IsValid( RefreshCallback ) ) RefreshCallback();
        }

        private void SavePageIdx()
        {
            SavedSubTabIdx[ MainTabIdxTemp ] = SubTabIdx;
        }

        private void LoadPageIdx( bool isSubTab = false )
        {
            if ( !isSubTab )
            {
                SubTabIdx = SavedSubTabIdx.ContainsKey( MainTabIdx ) ? SavedSubTabIdx[ MainTabIdx ] : 0;
            }
            SubTabIdxTemp = SubTabIdx;
        }

        private GUIStruct GetGUIStruct( Tuple< int, int > index = null )
        {
            var currentTabIdx = Helper.IsValid( index ) ? index : GetCurrentTabIdx();
            return SubTabGUI.ContainsKey( currentTabIdx ) ? SubTabGUI[ currentTabIdx ] : null;
        }

        public bool OnWindowChange()
        {
            return WindowChange;
        }

        public string GetGUIStructName( Tuple< int, int > index = null )
        {
            GUIStruct GUIStr = GetGUIStruct( index );
            return Helper.IsValid( GUIStr ) ? GUIStr.Name : null;
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
