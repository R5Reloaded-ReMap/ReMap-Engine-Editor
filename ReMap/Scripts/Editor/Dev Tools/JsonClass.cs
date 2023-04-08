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
        public List< string > Data;
        public DateTime Update;
    }
}