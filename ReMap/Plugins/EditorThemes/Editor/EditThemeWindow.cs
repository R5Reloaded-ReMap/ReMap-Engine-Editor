using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//to do TextColor
//EditorStyles.label.normal.textColor 

namespace ThemesPlugin
{
    public class EditThemeWindow : EditorWindow
    {
        public static CustomTheme ct;
        private CustomView customView;
        private List< Color > LastSimpleColors = new();
        private string Name;
        private bool Rhold;
        private Vector2 scrollPosition;
        private List< Color > SimpleColors = new();
        private bool STRGHold;

        private void OnDestroy()
        {
            ct = null;
        }

        private void Awake()
        {
            SimpleColors = CreateAverageCoolors();
            LastSimpleColors = CreateAverageCoolors();
            Name = ct.Name;
        }

        private void OnGUI()
        {
            if ( ct == null )
            {
                Close();
                return;
            }

            bool Regenerate = false;

            var e = Event.current;
            if ( e.type == EventType.KeyDown )
                if ( e.keyCode == KeyCode.R )
                    Rhold = true;

            if ( e.type == EventType.KeyUp )
                if ( e.keyCode == KeyCode.R )
                    Rhold = false;

            if ( e.type == EventType.KeyDown )
                if ( e.keyCode == KeyCode.LeftControl )
                    STRGHold = true;

            if ( e.type == EventType.KeyUp )
                if ( e.keyCode == KeyCode.LeftControl )
                    STRGHold = false;

            if ( Rhold && STRGHold )
            {
                Regenerate = true;
                Rhold = false;
                STRGHold = false;
            }

            if ( Regenerate && EditorUtility.DisplayDialog( "Do you want to regenarate this Theme? (Make a Clone first!)",
                    "Regenarating is helpfull when the Theme was made with an older version of the Plugin (but you might loose small amounts of data)", "Continue", "Cancel" ) )
            {
                ct.Items = new List< CustomTheme.UIItem >();
                for ( int i = 0; i < 6; i++ )
                    foreach ( string s in ThemesUtility.GetColorListByInt( i ) )
                    {
                        var uiItem = new CustomTheme.UIItem();
                        uiItem.Name = s;
                        uiItem.Color = SimpleColors[i];

                        ct.Items.Add( uiItem );
                    }
            }

            EditorGUILayout.LabelField( "\n" );
            Name = EditorGUILayout.TextField( Name );
            EditorGUILayout.LabelField( "\n" );

            customView = ( CustomView )EditorGUILayout.EnumPopup( customView, GUILayout.Width( 100 ) );
            if ( customView == CustomView.Advanced )
            {
                EditorGUILayout.LabelField( "" );
                scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition );

                var CTItemsClone = new List< CustomTheme.UIItem >( ct.Items );
                foreach ( var I in CTItemsClone )
                {
                    EditorGUILayout.BeginHorizontal();
                    I.Name = EditorGUILayout.TextField( I.Name, GUILayout.Width( 200 ) );
                    if ( GUILayout.Button( "Del", GUILayout.Width( 50 ) ) )
                        ct.Items.Remove( I );

                    EditorGUILayout.EndHorizontal();
                    I.Color = EditorGUILayout.ColorField( I.Color, GUILayout.Width( 200 ) );
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.LabelField( "" );
                EditorGUILayout.BeginHorizontal();

                if ( GUILayout.Button( "Add", GUILayout.Width( 200 ) ) )
                {
                    var I = new CustomTheme.UIItem();
                    I.Name = "Enter Name";
                    ct.Items.Add( I );
                }

                if ( ct.Items.Count > 0 )
                    ct.Items.RemoveAt( ct.Items.Count - 1 );

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if ( SimpleColors[0] != null )
                {
                    GUILayout.Label( "Base Color:", EditorStyles.boldLabel );
                    SimpleColors[0] = EditorGUILayout.ColorField( SimpleColors[0] );
                }
                if ( SimpleColors[1] != null )
                {
                    GUILayout.Label( "Accent Color:", EditorStyles.boldLabel );
                    SimpleColors[1] = EditorGUILayout.ColorField( SimpleColors[1] );
                }
                if ( SimpleColors[2] != null )
                {
                    GUILayout.Label( "Secondery Base Color:", EditorStyles.boldLabel );
                    SimpleColors[2] = EditorGUILayout.ColorField( SimpleColors[2] );
                }
                if ( SimpleColors[3] != null )
                {
                    GUILayout.Label( "Tab Color:", EditorStyles.boldLabel );
                    SimpleColors[3] = EditorGUILayout.ColorField( SimpleColors[3] );
                }
                if ( SimpleColors[4] != null )
                {
                    GUILayout.Label( "Command Bar Color:", EditorStyles.boldLabel );
                    SimpleColors[4] = EditorGUILayout.ColorField( SimpleColors[4] );
                }
                if ( SimpleColors[5] != null )
                {
                    GUILayout.Label( "Additional Color:", EditorStyles.boldLabel );
                    SimpleColors[5] = EditorGUILayout.ColorField( SimpleColors[5] );
                }

                for ( int i = 0; i < SimpleColors.Count; i++ )
                    if ( SimpleColors[i] != null && SimpleColors[i] != LastSimpleColors[i] )
                        EditColor( i, SimpleColors[i] );
            }

            EditorGUILayout.LabelField( "" );
            EditorGUILayout.LabelField( "Unity Theme:" );
            ct.unityTheme = ( CustomTheme.UnityTheme )EditorGUILayout.EnumPopup( ct.unityTheme, GUILayout.Width( 100 ) );
            EditorGUILayout.LabelField( "" );

            EditorGUILayout.BeginHorizontal();
            if ( GUILayout.Button( "Save", GUILayout.Width( 200 ) ) )
            {
                if ( ct.Name != Name )
                    ThemesUtility.DeleteFileWithMeta( ThemesUtility.GetPathForTheme( ct.Name ) );

                ct.Name = Name;
                ThemesUtility.SaveJsonFileForTheme( ct );
            }
            if ( GUILayout.Button( "Clone", GUILayout.Width( 200 ) ) )
            {
                ct.Name = Name + " - c";
                ThemesUtility.SaveJsonFileForTheme( ct );
            }

            EditorGUILayout.EndHorizontal();
        }

        private CustomTheme.UIItem GeItemByName( string s )
        {
            CustomTheme.UIItem item = null;
            foreach ( var u in ct.Items )
                if ( u.Name == s )
                    item = u;

            return item;
        }

        private List< Color > CreateAverageCoolors()
        {
            var colors = new List< Color >();
            for ( int i = 0; i < 6; i++ )
            {
                var ColorObjects = ThemesUtility.GetColorListByInt( i );
                var AllColors = new List< Color >();

                foreach ( string s in ColorObjects )
                    if ( GeItemByName( s ) != null )
                        AllColors.Add( GeItemByName( s ).Color );

                if ( AllColors.Count > 0 )
                    colors.Add( GetAverage( AllColors ) );
                else
                    colors.Add( ThemesUtility.HtmlToRgb( "#9A7B6E" ) );
            }

            return colors;
        }

        private void EditColor( int i, Color nc )
        {
            var edit = ThemesUtility.GetColorListByInt( i );
            foreach ( string s in edit )
            {
                var Item = GeItemByName( s );
                if ( Item != null )
                    Item.Color = nc;
            }

            LastSimpleColors[i] = SimpleColors[i];
        }

        private Color GetAverage( List< Color > cl )
        {
            float r = 0;
            float g = 0;
            float b = 0;

            int Count = cl.Count;
            foreach ( var c in cl )
            {
                r += c.r;
                g += c.g;
                b += c.b;
            }

            return new Color( r / Count, g / Count, b / Count );
        }

        private enum CustomView
        {
            Simple,
            Advanced
        }
    }
}