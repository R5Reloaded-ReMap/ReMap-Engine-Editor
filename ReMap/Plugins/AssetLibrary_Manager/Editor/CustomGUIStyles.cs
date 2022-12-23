using UnityEngine;

namespace AssetLibraryManager
{
    public class CustomGUIStyles
    {
        //Purple bar on sections
        public static GUIStyle FoldStyle()
        {
            Texture2D tx = new Texture2D(1, 1);

            Color color = new Color(0.5f, 0.35f, 0.8f);
            Color[] textureColor = tx.GetPixels();

            for (var i = 0; i < textureColor.Length; ++i)
            {
                textureColor[i] = color;
            }

            tx.SetPixels(textureColor);

            tx.Apply();

            //---------------------------------------------

            GUIStyle style = new GUIStyle();

            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 16;
            style.fixedHeight = 24;

            style.normal = new GUIStyleState() { background = tx };
            style.normal.textColor = Color.white;

            return style;
        }

        //Black bar at the bottom of the window
        public static GUIStyle FooterStyle()
        {
            Texture2D tx = new Texture2D(1, 1);

            Color color = new Color(0.15f, 0.15f, 0.15f);
            Color[] textureColor = tx.GetPixels();

            for (var i = 0; i < textureColor.Length; ++i)
            {
                textureColor[i] = color;
            }

            tx.SetPixels(textureColor);

            tx.Apply();

            //---------------------------------------------

            GUIStyle style = new GUIStyle();

            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 12;
            style.fixedHeight = PrefabViewer.bottomHeight;

            style.normal = new GUIStyleState() { background = tx };
            style.normal.textColor = Color.grey;

            return style;
        }
    }
}