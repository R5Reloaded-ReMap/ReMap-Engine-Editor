using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WindowUtility
{
    public delegate void FunctionRef();

    public class WindowUtility
    {
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
                if ( functionRefs != null )
                {
                    foreach ( var functionRef in functionRefs ) functionRef();
                }

                return true;
            }

            return false;
        }


        public static void CreateTextField( ref string reference, string text = "button", string tooltip = "", float labelWidth = 0, float fieldWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            reference = EditorGUILayout.TextField( new GUIContent( "", tooltip ), reference, SizeOptions( fieldWidth, height ) );
        }

        public static void CreateToggle( ref bool reference, string text = "toggle", string tooltip = "", float labelWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            if ( EditorGUILayout.Toggle( "", reference, GUILayout.MaxWidth( 0 ), HeightOption( height ) ) )
            {
                reference = !reference;
            }
        }

        public static void CreateIntField( ref int reference, string text = "int field", string tooltip = "", float labelWidth = 0, float fieldWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            reference = EditorGUILayout.IntField( "", reference, SizeOptions( fieldWidth, height ) );
        }

        public static void CreateVector3Field( ref Vector3 reference, string text = "vector3 field", string tooltip = "", float labelWidth = 0, float fieldWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
            reference = EditorGUILayout.Vector3Field( "", reference, SizeOptions( fieldWidth, height ) );
        }

        public static void CreateTextInfo( string text = "text", string tooltip = "", float labelWidth = 0, float height = 0 )
        {
            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), SizeOptions( labelWidth, height ) );
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
