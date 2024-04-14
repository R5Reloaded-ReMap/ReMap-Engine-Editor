using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ThemesPlugin
{
    public static class ThemesUtility
    {
        public static readonly string CustomThemesPath = Application.dataPath + "/ReMap/Plugins/EditorThemes/Editor/Themes/";
        public static readonly string UssFilePath = Application.dataPath + "/ReMap/Plugins/EditorThemes/Editor/StyleSheets/Extensions/";
        public static readonly string PresetsPath = Application.dataPath + "/ReMap/Plugins/EditorThemes/Editor/CreatePresets/";
        public static readonly string Version = "v0.65";
        public static readonly string Enc = ".json";
        public static string currentTheme;

        public static Color HtmlToRgb( string s )
        {
            var c = Color.black;
            ColorUtility.TryParseHtmlString( s, out c );
            return c;
        }

        public static void OpenEditTheme( CustomTheme ct )
        {
            EditThemeWindow.ct = ct;
            var window = ( EditThemeWindow )EditorWindow.GetWindow( typeof(EditThemeWindow), false, "Edit Theme" );

            window.Show();
        }

        public static CustomTheme GetCustomThemeFromJson( string Path )
        {
            string json = File.ReadAllText( Path );

            return JsonUtility.FromJson< CustomTheme >( json );
        }

        public static string GetPathForTheme( string Name )
        {
            return CustomThemesPath + Name + Enc;
        }

        public static void DeleteFileWithMeta( string Path )
        {
            if ( File.Exists( Path ) )
            {
                File.Delete( Path );
                File.Delete( Path + ".meta" );
            }
            else
            {
                Debug.LogWarning( "Path: " + Path + " does not exsit" );
            }
        }

        public static string GenerateUssString( CustomTheme c )
        {
            string ussText = "";
            ussText += "/* ========== Editor Themes Plugin ==========*/";
            ussText += "\n";
            ussText += "/*            Auto Generated Code            */";
            ussText += "\n";
            ussText += "/*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*/";
            ussText += "\n";
            ussText += "/*" + Version + "*/";

            foreach ( var I in c.Items )
                ussText += UssBlock( I.Name, I.Color );

            return ussText;
        }

        public static string UssBlock( string Name, Color Color )
        {
            Color32 color32 = Color;

            string a = Color.a + "";
            a = a.Replace( ",", "." );

            string Colors = "rgba(" + color32.r + ", " + color32.g + ", " + color32.b + ", " + a + ")"; // Generate colors for later
            string s = "\n" + "\n"; //add two empty lines

            s += "." + Name + "\n"; //add name
            s += "{" + "\n" + "\t" + "background-color: " + Colors + ";" + "\n" + "}"; //add color

            return s;
        }

        public static void SaveJsonFileForTheme( CustomTheme t )
        {
            t.Version = Version;
            string NewJson = JsonUtility.ToJson( t );
            string Path = GetPathForTheme( t.Name );

            if ( File.Exists( Path ) )
                File.Delete( Path );

            File.WriteAllText( Path, NewJson );
            LoadUssFileForTheme( t.Name );
        }

        public static void LoadUssFileForTheme( string Name )
        {
            LoadUssFileForThemeUsingPath( GetPathForTheme( Name ) );
        }

        public static void LoadUssFileForThemeUsingPath( string Path )
        {
            var t = GetCustomThemeFromJson( Path );

            if ( ( EditorGUIUtility.isProSkin && t.unityTheme == CustomTheme.UnityTheme.Light ) || ( !EditorGUIUtility.isProSkin && t.unityTheme == CustomTheme.UnityTheme.Dark ) ||
                 ( !EditorGUIUtility.isProSkin && t.unityTheme == CustomTheme.UnityTheme.Remap ) )
                InternalEditorUtility.SwitchSkinAndRepaintAllViews();

            string ussText = GenerateUssString( t );
            WriteUss( ussText );
            currentTheme = Path;
        }

        public static void WriteUss( string ussText )
        {
            string Path = UssFilePath + "/dark.uss";
            DeleteFileWithMeta( Path );
            File.WriteAllText( Path, ussText );
            string Path2 = Application.dataPath + "/ReMap/Plugins/EditorThemes/Editor/StyleSheets/Extensions/light.uss";
            DeleteFileWithMeta( Path2 );
            File.WriteAllText( Path2, ussText );
            AssetDatabase.Refresh();
        }

        public static List< string > GetColorListByInt( int i )
        {
            var colorList = new List< string >();
            switch ( i )
            {
                case 0: //base
                    colorList.Add( "TabWindowBackground" );
                    colorList.Add( "ScrollViewAlt" );
                    colorList.Add( "label" );
                    colorList.Add( "ProjectBrowserTopBarBg" );
                    colorList.Add( "ProjectBrowserBottomBarBg" );
                    break;
                case 1: //accent
                    colorList.Add( "dockHeader" );
                    colorList.Add( "TV LineBold" );
                    break;
                case 2: //secondery
                    colorList.Add( "ToolbarDropDownToogleRight" );
                    colorList.Add( "ToolbarPopupLeft" );
                    colorList.Add( "ToolbarPopup" );
                    colorList.Add( "toolbarbutton" );
                    colorList.Add( "PreToolbar" );
                    colorList.Add( "AppToolbar" );
                    colorList.Add( "GameViewBackground" );
                    colorList.Add( "CN EntryInfoSmall" );
                    colorList.Add( "Toolbar" );
                    colorList.Add( "toolbarbutton" );
                    colorList.Add( "toolbarbuttonRight" );
                    colorList.Add( "ProjectBrowserIconAreaBg" );
                    break;
                case 3: //Tab
                    colorList.Add( "dragtab-label" ); //changing this color has overriten dragTab and dragtab first so removed
                    break;
                case 4: //button
                    colorList.Add( "AppCommandLeft" );
                    colorList.Add( "AppCommandMid" );
                    colorList.Add( "AppCommand" );
                    colorList.Add( "AppToolbarButtonLeft" );
                    colorList.Add( "AppToolbarButtonRight" );
                    colorList.Add( "DropDown" );
                    break;
                case 5:
                    colorList.Add( "SceneTopBarBg" );
                    colorList.Add( "MiniPopup" );
                    colorList.Add( "TV Selection" );
                    colorList.Add( "ExposablePopupMenu" );
                    colorList.Add( "minibutton" );
                    colorList.Add( " ToolbarSearchTextField" );
                    break;
            }

            return colorList;
        }
    }
}