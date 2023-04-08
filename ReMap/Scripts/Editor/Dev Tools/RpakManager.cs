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
    public class RpakManagerWindow : EditorWindow
    {
        Vector2 scrollPos = Vector2.zero;

        #if ReMapDev
            [ MenuItem( "ReMap Dev Tools/Prefabs Management/Windows/Rpak Manager", false, 100 ) ]
            public static void Init()
            {
                RpakManagerWindow window = ( RpakManagerWindow )GetWindow( typeof( RpakManagerWindow ), false, "Rpak Manager" );
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
