using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    public class LibrarySorterWindowTest : EditorWindow
    {
        Vector2 scrollPos = Vector2.zero;
        
        #if ReMapDev
             [ MenuItem( "ReMap Dev Tools/Prefabs Management/Windows/Prefab Fix Manager Test", false, 100 ) ]
            public static void Init()
            {
                LibrarySorterWindowTest window = ( LibrarySorterWindowTest )GetWindow( typeof( LibrarySorterWindowTest ), false, "Prefab Fix Manager" );
                window.minSize = new Vector2( 650, 600 );
                window.Show();
            }
        #endif

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView( scrollPos );



            GUILayout.EndScrollView();
        }
    }
}
