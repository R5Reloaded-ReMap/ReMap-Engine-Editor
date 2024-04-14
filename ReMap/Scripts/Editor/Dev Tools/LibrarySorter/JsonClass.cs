using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LibrarySorter
{
    [Serializable]
    public class LibraryData
    {
        public List< RpakData > RpakList;

        public RpakData AllModels()
        {
            return GetRpakData( RpakManagerWindow.allModelsDataName );
        }

        public RpakData R5ReloadedList()
        {
            return GetRpakData( RpakManagerWindow.r5reloadedModelsDataName );
        }

        public RpakData AllModelsRetail()
        {
            return GetRpakData( RpakManagerWindow.allModelsRetailDataName );
        }

        public RpakData GetRpakData( string name )
        {
            return RpakList.FirstOrDefault( data => data.Name == name );
        }

        public List< string > GetAllModelsList()
        {
            return AllModels().Data.Union( AllModelsRetail().Data ).ToList();
        }

        public bool IsR5ReloadedModels( string model )
        {
            return R5ReloadedList().Data.Contains( model );
        }

        public List< RpakData > GetSpecialData()
        {
            return RpakList.Where( r => r.IsSpecial ).ToList();
        }

        public List< RpakData > GetR5ReloadedData()
        {
            return RpakList.Where( r => !r.IsRetail && !r.IsSpecial ).ToList();
        }

        public List< RpakData > GetRetailData()
        {
            return RpakList.Where( r => r.IsRetail && !r.IsSpecial ).ToList();
        }

        public List< RpakData > GetVisibleData()
        {
            return RpakList.Where( r => !r.IsHidden ).ToList();
        }

        public List< RpakData > RpakContains( string name )
        {
            return GetVisibleData().Where( r => r.Data.Contains( name ) ).ToList();
        }
    }

    [Serializable]
    public class RpakData
    {
        public List< string > Data;
        public bool IsHidden;
        public bool IsRetail;
        public bool IsSpecial;
        public string Name;
        public string Update;

        public bool Contains( string name )
        {
            return Data.Contains( name );
        }

        public void UpdateTime()
        {
            Update = DateTime.UtcNow.ToString();
        }
    }


    [Serializable]
    public class PrefabOffsetList
    {
        public List< PrefabOffset > List;
    }

    [Serializable]
    public class PrefabOffset
    {
        public string ModelName;
        public Vector3 Rotation;
    }

    [Serializable]
    public class MaterialData
    {
        public List< MaterialClass > MaterialList;

        public bool ContainsName( string name )
        {
            return MaterialList.Any( material => material.Name == name );
        }

        public bool ContainsFilePath( string fileName )
        {
            return MaterialList.Any( material => material.Path.Contains( fileName ) );
        }

        public bool ContainsFilePath( string[] fileNames )
        {
            return MaterialList.Any( material => fileNames.Any( fileName => material.Path.Contains( fileName ) ) );
        }

        public string GetPath( string name )
        {
            var material = MaterialList.FirstOrDefault( m => m.Name == name );
            return material != null ? material.Path : null;
        }

        public void RemoveMaterial( string name )
        {
            if ( ContainsName( name ) )
                MaterialList.RemoveAll( material => material.Name == name );
        }

        public void Add( MaterialClass materialClass )
        {
            if ( !ContainsName( materialClass.Name ) )
                MaterialList.Add( materialClass );
        }
    }

    [Serializable]
    public class MaterialClass
    {
        public string Name;
        public string Path;
    }
}