using System.IO;
using UnityEditor;
using UnityEngine;

//to do TextColor
//EditorStyles.label.normal.textColor 

namespace ThemesPlugin
{
    public class CreateThemeWindow : EditorWindow
    {
        private string Name = "EnterName";
        private UnityTheme unityTheme;

        //[MenuItem("Themes/Create Theme")]
        public static void ShowWindow()
        {
            ThemeSettings.ShowWindow();
            GetWindow< CreateThemeWindow >( "Theme Settings" );
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField( "" );
            Name = EditorGUILayout.TextField( Name, GUILayout.Width( 200 ) );
            EditorGUILayout.LabelField( "" );
            EditorGUILayout.LabelField( "Preset:" );

            unityTheme = ( UnityTheme )EditorGUILayout.EnumPopup( unityTheme, GUILayout.Width( 100 ) );
            string Description = "";
            switch ( unityTheme )
            {
                case UnityTheme.FullDark:
                    Description = "Everything you need for a Dark Theme";
                    break;
                case UnityTheme.FullLight:
                    Description = "Everything you need for a Light Theme";
                    break;
                case UnityTheme.Light:
                    Description = "Minimalistic Preset for a Light Theme";
                    break;
                case UnityTheme.Dark:
                    Description = "Minimalistic Preset for a Dark Theme";
                    break;
                case UnityTheme.Both:
                    Description = "Minimalistic Preset for a Light & Dark Theme";
                    break;
            }

            EditorGUILayout.LabelField( Description );
            EditorGUILayout.LabelField( "" );

            bool create = false;

            var e = Event.current;
            if ( e.type == EventType.KeyDown )
                if ( e.keyCode == KeyCode.Return )
                    create = true;

            if ( GUILayout.Button( "Create Custom Theme", GUILayout.Width( 200 ) ) )
                create = true;


            if ( create )
            {
                string Path = Application.dataPath + "/ReMap/Plugins/EditorThemes/Editor/StyleSheets/Extensions/CustomThemes/" + Name + ".json";
                if ( File.Exists( Path ) )
                    if ( EditorUtility.DisplayDialog( "This Theme already exsists", "Do you want to overide the old Theme?", "Yes", "Cancel" ) == false )
                        return;

                var t = new CustomTheme();
                string PresetName = "";
                switch ( unityTheme )
                {
                    case UnityTheme.FullDark:
                        PresetName = "FullDark";
                        break;
                    case UnityTheme.FullLight:
                        PresetName = "FullLight";
                        break;
                    case UnityTheme.Light:
                        PresetName = "Light";
                        break;
                    case UnityTheme.Dark:
                        PresetName = "Dark";
                        break;
                    case UnityTheme.Both:
                        PresetName = "Both";
                        break;
                }

                t = FetchTheme( PresetName, Name );
                ThemesUtility.SaveJsonFileForTheme( t );
                ThemesUtility.OpenEditTheme( t );

                Close();
            }
        }

        private CustomTheme FetchTheme( string PresetName, string Name )
        {
            var CustomTheme = ThemesUtility.GetCustomThemeFromJson( ThemesUtility.PresetsPath + PresetName + ".json" );

            CustomTheme.Name = Name;

            return CustomTheme;
        }

        private enum UnityTheme
        {
            FullDark,
            FullLight,
            Dark,
            Light,
            Both
        }
    }
}