
using System;
using System.Collections.Generic;
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
}