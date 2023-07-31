
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
    }

    [Serializable]
    public class RpakData
    {
        public string Name;
        public List< string > Data;
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
    }

    [Serializable]
    public class MaterialClass
    {
        public string Name;
        public string Path;
    }
}