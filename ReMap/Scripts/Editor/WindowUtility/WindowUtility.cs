using UnityEditor;
using UnityEngine;

namespace WindowUtility
{
    internal delegate void FunctionRef();

    public class WindowUtility
    {
        internal static void CreateButton( string text = "button", string tooltip = "", FunctionRef functionRef = null )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginHorizontal();
                if( GUILayout.Button( new GUIContent( text, tooltip ), buttonStyle, GUILayout.Height( 20 ) ) )
                {
                    if ( functionRef != null ) functionRef();
                }
            GUILayout.EndHorizontal();
        }
    }
}
