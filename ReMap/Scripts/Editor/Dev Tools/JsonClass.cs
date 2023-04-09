using System;
using System.Collections.Generic;

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
}