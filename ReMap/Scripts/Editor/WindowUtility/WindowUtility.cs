using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WindowUtility
{
    public delegate void FunctionRef();
    public delegate Task FunctionRefAsync();
    public delegate Task< string > FunctionRefAsyncString();

    public class WindowUtility
    {
        private static Color GUI_SettingsColor = new Color( 255f, 255f, 255f );

        public static bool CreateButton( string text = "button", string tooltip = "", float width = 0, float height = 0 )
        {
            return CreateButton( text, tooltip, ( FunctionRef[] ) null, width, height );
        }

        public static bool CreateButton( string text = "button", string tooltip = "", FunctionRef functionRef = null, float width = 0, float height = 0 )
        {
            return CreateButton( text, tooltip, new FunctionRef[] { functionRef }, width, height );
        }

        public static bool CreateButton( string text = "button", string tooltip = "", FunctionRef[] functionRefs = null, float width = 0, float height = 0 )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            if( GUILayout.Button( new GUIContent( text, tooltip ), buttonStyle, SizeOptions( width, height ) ) )
            {
                if ( Helper.IsValid( functionRefs ) )
                {
                    foreach ( var functionRef in functionRefs ) functionRef();
                }

                GUI.FocusControl( null );

                return true;
            }

            return false;
        }

        public static bool CreateCopyButton( string text = "button", string tooltip = "", string copy = "", float width = 0, float height = 0 )
        {
            return CreateButton( text, tooltip, () => CopyText( copy ), width, height );
        }

        private static void CopyText( string text )
        {
            GUIUtility.systemCopyBuffer = text;
        }


        public static string CreateTextField( ref string reference, float fieldWidth, float height = 0 )
        {
            return CreateTextField( ref reference, "", "", 0, fieldWidth, height, true );
        }

        public static string CreateTextField( ref string reference, string text = "text field", string tooltip = "", float labelWidth = 0, float fieldWidth = 0, float height = 0, bool fieldOnly = false )
        {
            if ( !fieldOnly ) EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            return reference = EditorGUILayout.TextField( new GUIContent( "", tooltip ), reference, SizeOptions( fieldWidth, height ) );
        }

        public static void CreateToggle( ref bool reference, string text = "toggle", string tooltip = "", float labelWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            reference = EditorGUILayout.Toggle( "", reference, GUILayout.MaxWidth( 20 ) );
        }

        public static void CreateIntField( ref int reference, string text = "int field", string tooltip = "", float labelWidth = 0, float fieldWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            reference = EditorGUILayout.IntField( "", reference, SizeOptions( fieldWidth, height ) );
        }

        public static Vector3 CreateVector3Field( ref Vector3 reference, string text = "vector3 field", string tooltip = "", float labelWidth = 0, float fieldWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            return reference = EditorGUILayout.Vector3Field( "", reference, SizeOptions( fieldWidth, height ) );
        }

        public static void CreateTextInfo( string text = "text", string tooltip = "", float labelWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
        }

        public static void CreateTextInfoCentered( string text = "text", string tooltip = "", float labelWidth = 0, float height = 0 )
        {
            GUIStyle labelStyle = new GUIStyle( EditorStyles.label );
            labelStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), labelStyle, SizeOptions( labelWidth, height ) );
        }

        public static void CreateObjectField( ref UnityEngine.Object obj, string text = "text", string tooltip = "", float labelWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            obj = EditorGUILayout.ObjectField( obj, typeof( UnityEngine.Object ), true );
        }

        public static void CreateObjectField( ref GameObject obj, string text = "text", string tooltip = "", float labelWidth = 0, float height = 0 )
        {
            UnityEngine.Object uobj = ( UnityEngine.Object ) obj;
            CreateObjectField( ref uobj, text, tooltip, labelWidth, height );
        }

        public static void Space( float value )
        {
            GUILayout.Space( value );
        }

        internal static void SeparatorAutoWidth( EditorWindow editorWindow, float width = 0, float height = 4, Color color = default )
        {
            Separator( ( float ) editorWindow.position.width + width, height, color );
        }

        internal static void Separator( float width = 0, float height = 4, Color color = default )
        {
            GUI.backgroundColor = color != default ? color : GUI_SettingsColor;
            GUILayout.Box( "", SizeOptions( width, height ) );
            GUI.backgroundColor = Color.white;
        }

        public static void FlexibleSpace()
        {
            GUILayout.FlexibleSpace();
        }

        public static void GetEditorWindowSize( EditorWindow editorWindow )
        {
            if ( Helper.IsValid( editorWindow ) )
            {
                CreateTextInfo( $"Window Size: {Helper.ReplaceComma( editorWindow.position.width )} x {Helper.ReplaceComma( editorWindow.position.height )}", "", 152 );
            }
        }

        public static void ShowPageInfo( int currentPage, int maxPage, int itemStart, int itemEnd, string info = "Page", string type = "" )
        {
            GUILayout.Label( $"{info} {currentPage + 1} / {maxPage} ( {itemStart} - {itemEnd} {type} )", EditorStyles.boldLabel, GUILayout.Width( 400 ) );
        }

        public static void GetScrollSize( Vector2 scroll )
        {
            CreateTextInfo( $"Scroll Position: {Helper.ReplaceComma( scroll.x )} x {Helper.ReplaceComma( scroll.y )}", "", 152 );
        }

        private static GUILayoutOption HeightOption( float value )
        {
            GUILayoutOption heightOption = value != 0 ? GUILayout.Height( value ) : default( GUILayoutOption );

            return heightOption;
        }

        private static GUILayoutOption[] SizeOptions( float width, float height )
        {
            List< GUILayoutOption > options = new List< GUILayoutOption >();

            if ( width != 0 ) options.Add( GUILayout.Width( width ) );
            if ( height != 0 ) options.Add( GUILayout.Height( height ) );

            return options.ToArray();
        }
    }
}
