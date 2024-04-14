using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThemesPlugin
{
    [Serializable]
    public class CustomTheme
    {
        public enum UnityTheme
        {
            Dark,
            Light,
            Both,
            Remap
        }

        public bool IsUnDeletable;
        public bool IsUnEditable;
        public List< UIItem > Items;
        public string Name;
        public UnityTheme unityTheme;
        public string Version;

        [Serializable]
        public class UIItem
        {
            public Color Color;
            public string Name;
        }
    }
}