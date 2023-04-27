using System;
using UnityEditor;
using UnityEngine;

namespace WindowUtility
{
    public delegate void FunctionRef();

    public class WindowUtility
    {
        internal static bool CreateButton( string text = "button", string tooltip = "", int? width = null, int height = 20 )
        {
            return CreateButton(text, tooltip, ( FunctionRef ) null, width, height);
        }

        internal static bool CreateButton( string text, string tooltip, FunctionRef functionRef, int? width = null, int height = 20 )
        {
            return CreateButton( text, tooltip, new FunctionRef[] { functionRef }, width, height );
        }

        internal static bool CreateButton( string text = "button", string tooltip = "", FunctionRef[] functionRefs = null, int? width = null, int height = 20 )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUILayoutOption widthOption = width.HasValue ? GUILayout.Width( width.Value ) : null;

            if( GUILayout.Button( new GUIContent( text, tooltip ), buttonStyle, GUILayout.Height( height ), widthOption ) )
            {
                if ( functionRefs != null )
                {
                    foreach ( var functionRef in functionRefs ) functionRef();
                }

                return true;
            }

            return false;
        }


        internal static void CreateTextField( ref string reference, string text = "button", string tooltip = "", int labelWidth = 100, int fieldWidth = 200, int? height = 20 )
        {
            GUILayoutOption heightOption = height.HasValue ? GUILayout.Height( height.Value ) : null;

            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( labelWidth ), heightOption );
            reference = EditorGUILayout.TextField( new GUIContent( "", tooltip ), reference, GUILayout.Width( fieldWidth ), heightOption );
        }

        internal static void CreateToggle( ref bool reference, ref bool referenceTemp, string text = "toggle", string tooltip = "", int labelWidth = 100, int? height = 20 )
        {
            GUILayoutOption heightOption = height.HasValue ? GUILayout.Height( height.Value ) : null;

            EditorGUILayout.LabelField( new GUIContent( text, tooltip ), GUILayout.Width( labelWidth ), heightOption );
            reference = EditorGUILayout.Toggle( "", reference, GUILayout.MaxWidth( 0 ), heightOption );

            if( reference != referenceTemp ) referenceTemp = reference;
        }
    }
}
