using UnityEngine;
using UnityEditor;

public class InfoUI : EditorWindow
{
    [MenuItem("ReMap/Info", false, 0)]
    static void Init()
    {
        InfoUI window = (InfoUI)EditorWindow.GetWindow(typeof(InfoUI), false, "ReMap Info");
        window.minSize = new Vector2(678, 290);
        window.maxSize = new Vector2(678, 290);
        window.Show();
    }

    private Texture2D m_Logo = null;
    void OnEnable()
    {
        m_Logo = (Texture2D)Resources.Load("Images/logo",typeof(Texture2D));
    }

    void OnGUI()
    {
        //Logo
        GUILayout.Label(m_Logo, GUILayout.Height(50));

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

        GUILayout.BeginVertical("box");
            GUILayout.Label("Links:", EditorStyles.boldLabel);
            //Docs
            GUILayout.BeginHorizontal();
                GUILayout.Label("R5Reloaded Docs:", EditorStyles.boldLabel, GUILayout.Width(120));
                if (GUILayout.Button("docs.r5reloaded.com"))
                    Application.OpenURL("https://docs.r5reloaded.com/");
            GUILayout.EndHorizontal();
            //Github
            GUILayout.BeginHorizontal();
                GUILayout.Label("Github:", EditorStyles.boldLabel, GUILayout.Width(120));
                if (GUILayout.Button("github.com/R5Reloaded-ReMap/"))
                    Application.OpenURL("https://github.com/R5Reloaded-ReMap/");
            GUILayout.EndHorizontal();
            //Discord
            GUILayout.BeginHorizontal();
                GUILayout.Label("Discord:", EditorStyles.boldLabel, GUILayout.Width(120));
                if (GUILayout.Button("discord.gg/snsyDVa2fn"))
                    Application.OpenURL("https://discord.gg/snsyDVa2fn");
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        //Version
        GUILayout.BeginVertical("box");
            GUILayout.Label($"{UnityInfo.ReMapVersion}");
        GUILayout.EndVertical();
    }
}