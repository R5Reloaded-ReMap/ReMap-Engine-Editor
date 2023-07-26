using UnityEngine;
using UnityEditor;

public class InfoUI : EditorWindow
{
    public static void Init()
    {
        InfoUI window = (InfoUI)EditorWindow.GetWindow(typeof(InfoUI), false, "ReMap Info");
        window.minSize = new Vector2(678, 310);
        window.maxSize = new Vector2(678, 310);
        window.Show();
    }

    private Texture2D m_Logo = null;
    void OnEnable()
    {
        m_Logo = (Texture2D)Resources.Load("Images/logo",typeof(Texture2D));
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

            GUILayout.BeginVertical("box", GUILayout.Height(150), GUILayout.Width(150));
                GUILayout.Label(m_Logo, GUILayout.Height(140), GUILayout.Width(140));
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box", GUILayout.Height(150));
                //Docs
                GUILayout.BeginHorizontal();
                    GUILayout.Label("ReMap Docs:", EditorStyles.boldLabel, GUILayout.Width(120));
                    if (GUILayout.Button("remap.ayezee.app"))
                        Application.OpenURL("https://remap.ayezee.app/");
                GUILayout.EndHorizontal();
                //Github
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Github:", EditorStyles.boldLabel, GUILayout.Width(120));
                    if (GUILayout.Button("github.com/R5Reloaded-ReMap"))
                        Application.OpenURL("https://github.com/R5Reloaded-ReMap/");
                GUILayout.EndHorizontal();
                //Discord
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Discord:", EditorStyles.boldLabel, GUILayout.Width(120));
                    if (GUILayout.Button("discord.gg/snsyDVa2fn"))
                        Application.OpenURL("https://discord.gg/snsyDVa2fn");
                GUILayout.EndHorizontal();
                //Twitter
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Twitter:", EditorStyles.boldLabel, GUILayout.Width(120));
                    if (GUILayout.Button("twitter.com/ReMapR5R"))
                        Application.OpenURL("https://twitter.com/ReMapR5R");
                GUILayout.EndHorizontal();
                //Base Scripts
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Base Scripts:", EditorStyles.boldLabel, GUILayout.Width(120));
                    if (GUILayout.Button("github.com/R5Reloaded-ReMap/scripts_r5"))
                        Application.OpenURL("hhttps://github.com/R5Reloaded-ReMap/scripts_r5");
                GUILayout.EndHorizontal();
                //Flowstate Scripts
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Flowstate Scripts:", EditorStyles.boldLabel, GUILayout.Width(120));
                    if (GUILayout.Button("github.com/ColombianGuy/r5_flowstate"))
                        Application.OpenURL("https://github.com/ColombianGuy/r5_flowstate");
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUI.contentColor = Color.white;
        GUILayout.BeginHorizontal();
            //Credits
            GUILayout.BeginVertical("box", GUILayout.Height(120));
                GUILayout.Label("Credits:", EditorStyles.boldLabel);
                GUILayout.Label("Unity Scripts/Export Code/Custom Prefabs:", EditorStyles.boldLabel);
                GUILayout.Label("  AyeZee#6969 \n");
                GUILayout.Label("Unity Scripts/Export Code/Importing Models:", EditorStyles.boldLabel);
                GUILayout.Label("  Julefox#0050");
            GUILayout.EndVertical();
            //Donate
            GUILayout.BeginVertical("box", GUILayout.Height(120));
                GUILayout.Label("Donate:", EditorStyles.boldLabel);
                GUILayout.Label("If you would like to donate to show support\nfor the project. :)");
                GUILayout.Label("");
                if (GUILayout.Button("paypal.me/AyeZeeBB"))
                    Application.OpenURL("https://www.paypal.com/paypalme/AyeZeeBB");
                if (GUILayout.Button("paypal.me/Julefx"))
                    Application.OpenURL("https://www.paypal.com/paypalme/Julefx");
            GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        //Version
        GUILayout.BeginVertical("box");
            GUILayout.Label($"{UnityInfo.ReMapVersion}");
        GUILayout.EndVertical();
    }
}