using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

using WindowUtility;

namespace CodeViewsWindow
{
    public class AdditionalCodeWindow : EditorWindow
    {
        private static string relativePathAdditionalCode = UnityInfo.relativePathAdditionalCode;
        internal static AdditionalCodeWindow windowInstance;
        internal static int tab = 0;
        internal static string[] toolbarTab = new string[0];

        internal static AdditionalCode additionalCode;
        private static string emptyContentStr = "Empty Code";

        public static void Init()
        {
            additionalCode = FindAdditionalCode();

            windowInstance = ( AdditionalCodeWindow ) GetWindow( typeof( AdditionalCodeWindow ), false, "Additional Code" );
            windowInstance.Show();
        }

        void OnGUI()
        {

        }

        public static AdditionalCode FindAdditionalCode()
        {
            if ( !File.Exists( relativePathAdditionalCode ) )
            {
                CreateNewJsonAdditionalCode();
            }

            string json = System.IO.File.ReadAllText( relativePathAdditionalCode );
            additionalCode = JsonUtility.FromJson< AdditionalCode >( json );

            return additionalCode;
        }

        internal static void CreateNewJsonAdditionalCode()
        {
            additionalCode = new AdditionalCode();
            additionalCode.Content = new List< AdditionalCodeContent >();

            AdditionalCodeContent emptyContent = NewAdditionalCodeContent();
            emptyContent.Name = emptyContentStr;

            additionalCode.Content.Add( emptyContent );

            SaveJson();
        }

        internal static AdditionalCodeContent NewAdditionalCodeContent()
        {
            AdditionalCodeContent content = new AdditionalCodeContent();

            content.Name = "unnamed";
            content.Code = "";

            return content;
        }

        internal static void SaveJson()
        {
            string json = JsonUtility.ToJson( additionalCode );
            System.IO.File.WriteAllText( relativePathAdditionalCode, json );

            //Refresh();
        }
    }

    [Serializable]
    public class AdditionalCode
    {
        public List < AdditionalCodeContent > Content;
    }

    [Serializable]
    public class AdditionalCodeContent
    {
        public string Name;
        public string Code;
    }
}
