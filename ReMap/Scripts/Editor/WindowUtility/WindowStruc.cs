using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WindowUtility
{
    public class WindowStruct
    {
        public string[] MainTab { get; set; }
        public Dictionary< int, string[] > SubTab { get; set; }
        public Dictionary< ( int, int ), GUIStruct > SubTabGUI { get; set; }
        public GUIStruct ActiveGUIStruct  { get; set; }
        public FunctionRef MainTabCallback { get; set; }
        public FunctionRef SubTabCallback { get; set; }
        public FunctionRef PostRefreshCallback { get; set; }
        public FunctionRef RefreshCallback { get; set; }
        public FunctionRef InitCallback { get; set; }

        public int MainTabIdx = 0;
        public int SubTabIdx = 0;
        private int MainTabIdxTemp = 0;
        private int SubTabIdxTemp = 0;

        private bool WindowChange = false;
        private bool ForceChange = false;

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

            if ( MainTabIdx != MainTabIdxTemp || SubTabIdx != SubTabIdxTemp )
            {
                if ( Helper.IsValid( PostRefreshCallback ) )
                {
                    PostRefreshCallback();
                }

                if ( MainTabIdx != MainTabIdxTemp )
                {
                    SavePageIdx();
                    MainTabIdxTemp = MainTabIdx;
                    LoadPageIdx();
                    
                }

                if ( SubTabIdx != SubTabIdxTemp )
                {
                    LoadPageIdx( true );
                }

                CommonChanges();
            }
        }

        public void ShowFunc( int idx = 0 )
        {
            if ( Helper.IsValid( ActiveGUIStruct ) && Helper.IsValid( ActiveGUIStruct.OnGUI[ idx ] ) )
            {
                ActiveGUIStruct.OnGUI[ idx ]();
            }
        }

        private void CommonChanges()
        {
            ActiveGUIStruct = GetGUIStruct();

            if ( Helper.IsValid( ActiveGUIStruct ) )
            {
                WindowChange = true;
                if ( Helper.IsValid( ActiveGUIStruct.OnStartGUI ) ) ActiveGUIStruct.OnStartGUI();
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

        private GUIStruct GetGUIStruct()
        {
            return GetGUIStruct( GetCurrentTabIdx() );
        }

        private GUIStruct GetGUIStruct( ( int, int ) index )
        {
            return SubTabGUI.ContainsKey( index ) ? SubTabGUI[ index ] : null;
        }

        public async void Awake()
        {
            await Task.Delay( 200 ); // 200 ms

            ForceChange = true;
                foreach ( var tuple in SubTabGUI.Keys )
                {
                    GUIStruct GUIStr = GetGUIStruct( tuple );
                    if ( Helper.IsValid( GUIStr ) && Helper.IsValid( GUIStr.InitCallback ) )
                    {
                        ActiveGUIStruct = GUIStr;

                        GUIStr.InitCallback();
                    }
                }

                ActiveGUIStruct = SubTabGUI.First().Value;
                
                if ( Helper.IsValid( InitCallback ) ) InitCallback();
            ForceChange = false;

            if ( Helper.IsValid( RefreshCallback ) ) RefreshCallback();
        }

        public void StoreInfo( string name, object value )
        {
            if ( Helper.IsValid( ActiveGUIStruct ) )
            {
                ActiveGUIStruct.StoredInfo[ name ] = value;
            }
        }

        public void StoreInfo( string name, object value, ( int, int ) index )
        {
            GUIStruct GUIStr = GetGUIStruct( index );
            if ( Helper.IsValid( GUIStr ) && GUIStr.StoredInfo.ContainsKey( name ) )
            {
                GUIStr.StoredInfo[ name ] = value;
            }
        }

        public void ReStoreInfo< T >( ref T obj, string name ) where T : class
        {
            if ( Helper.IsValid( ActiveGUIStruct ) && ActiveGUIStruct.StoredInfo.ContainsKey( name ) )
            {
                obj = ActiveGUIStruct.StoredInfo[ name ] as T;
            }
        }

        public void ReStoreInfo< T >( ref T obj, string name, ( int, int ) index ) where T : class
        {
            GUIStruct GUIStr = GetGUIStruct( index );
            if ( Helper.IsValid( GUIStr ) && GUIStr.StoredInfo.ContainsKey( name ) )
            {
                obj = GUIStr.StoredInfo[ name ] as T;
            }
        }

        public void RemoveStoredInfo( string name )
        {
            GUIStruct GUIStr = GetGUIStruct();
            if ( Helper.IsValid( GUIStr ) && GUIStr.StoredInfo.ContainsKey( name ) )
            {
                GUIStr.StoredInfo.Remove( name );
            }
        }

        public void RemoveStoredInfo( string name, ( int, int ) index = default )
        {
            GUIStruct GUIStr = GetGUIStruct( index );
            if ( Helper.IsValid( GUIStr ) && GUIStr.StoredInfo.ContainsKey( name ) )
            {
                GUIStr.StoredInfo.Remove( name );
            }
        }

        public T GetStoredInfo< T >( string name ) where T : class
        {
            return ActiveGUIStruct.StoredInfo[ name ] as T;
        }

        public T GetStoredInfo< T >( string name, ( int, int ) index = default ) where T : class
        {
            GUIStruct GUIStr = GetGUIStruct( index );
            if ( Helper.IsValid( GUIStr ) && GUIStr.StoredInfo.ContainsKey( name ) )
            {
                return GUIStr.StoredInfo[ name ] as T;
            }
            else return default( T );
        }

        public bool OnWindowChange()
        {
            return WindowChange || ForceChange;
        }

        public string GetGUIStructName( ( int, int ) index = default )
        {
            GUIStruct GUIStr = GetGUIStruct( index );
            return Helper.IsValid( GUIStr ) ? GUIStr.Name : "NoName";
        }

        public ( int, int ) GetCurrentTabIdx()
        {
            return ( MainTabIdx, SubTabIdx );
        }

        public ( int, int ) GetCurrentTabIdxTemp()
        {
            return ( MainTabIdxTemp, SubTabIdxTemp );
        }
    }

    public class GUIStruct
    {
        public string Name { get; set; }
        public FunctionRef[] OnGUI { get; set; }
        public FunctionRef OnStartGUI { get; set; }
        public Dictionary< string, object > StoredInfo = new Dictionary< string, object >();

        public FunctionRef InitCallback { get; set; }
    }
}
