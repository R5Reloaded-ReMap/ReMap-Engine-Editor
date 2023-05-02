using System.Net;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using WindowUtility;

namespace CodeViewsWindow
{
    public class AdditionalCode : EditorWindow
    {
        internal static AdditionalCode windowInstance;
        internal static int tab = 0;
        internal static string[] toolbarTab;

        public static void Init()
        {
            windowInstance = ( AdditionalCode ) GetWindow( typeof( AdditionalCode ), false, "Additional Code" );
            windowInstance.Show();
        }

        void OnGUI()
        {

        }
    }

    [Serializable]
    public class CodeViewsAdditionalCode
    {
        public CodeViewsAdditionalCodeContent[] Content;
    }

    [Serializable]
    public class CodeViewsAdditionalCodeContent
    {
        public string Name;
        public string Code;
    }
}
