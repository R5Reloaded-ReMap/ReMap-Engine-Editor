using UnityEditor;
using UnityEngine;

namespace WindowUtility
{
    internal delegate void FunctionRef();

    public class WindowUtility
    {
        internal static bool CreateButton( string text = "button", string tooltip = "", FunctionRef functionRef = null, int? size = null, int height = 20 )
        {
            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            if ( size != null )
            {
                if( GUILayout.Button( new GUIContent( text, tooltip ), buttonStyle, GUILayout.Height( height ), GUILayout.Width( size.Value ) ) )
                {
                    if ( functionRef != null ) functionRef();
                    return true;
                }
            }
            else
            {
                if( GUILayout.Button( new GUIContent( text, tooltip ), buttonStyle, GUILayout.Height( height ) ) )
                {
                    if ( functionRef != null ) functionRef();
                    return true;
                }
            }

            return false;
        }
    }
}
