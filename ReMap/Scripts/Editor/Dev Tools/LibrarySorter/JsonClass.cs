
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
            return this.RpakList.FirstOrDefault( data => data.Name == name );
        }

        public List< string > GetAllModelsList()
        {
            return this.AllModels().Data.Union( this.AllModelsRetail().Data ).ToList();
        }

        public bool IsR5ReloadedModels( string model )
        {
            return this.R5ReloadedList().Data.Contains( model );
        }

        public List< RpakData > GetSpecialData()
        {
            return this.RpakList.Where( r => r.IsSpecial ).ToList();
        }

        public List< RpakData > GetR5ReloadedData()
        {
            return this.RpakList.Where( r => !r.IsRetail && !r.IsSpecial ).ToList();
        }

        public List< RpakData > GetRetailData()
        {
            return this.RpakList.Where( r => r.IsRetail && !r.IsSpecial ).ToList();
        }

        public List< RpakData > GetVisibleData()
        {
            return this.RpakList.Where( r => !r.IsHidden ).ToList();
        }
    }

    [Serializable]
    public class RpakData
    {
        public string Name;
        public List< string > Data;
        public bool IsRetail;
        public bool IsHidden;
        public bool IsSpecial;
        public string Update;
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
            return this.MaterialList.Any( material => material.Name == name );
        }

        public string GetPath( string name )
        {
            MaterialClass material = this.MaterialList.FirstOrDefault( m => m.Name == name );
            return material != null ? material.Path : null;
        }

        public void RemoveMaterial( string name )
        {
            if ( this.ContainsName( name ) )
            {
                this.MaterialList.RemoveAll( material => material.Name == name );
            }
        }

        public void Add( MaterialClass materialClass )
        {
            if ( !this.ContainsName( materialClass.Name ) )
            {
                this.MaterialList.Add( materialClass );
            }
        }
    }

    [Serializable]
    public class MaterialClass
    {
        public string Name;
        public string Path;
    }
}