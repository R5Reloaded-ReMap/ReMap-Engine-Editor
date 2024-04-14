using UnityEditor;
using UnityEngine;

public class FirstTimeWindow : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem( "ReMap/Startup Window", false, 1080 )]
    public static void OpenWindow()
    {
        var window = GetWindow< FirstTimeWindow >();
        window.titleContent = new GUIContent( "ReMap Startup" );
        window.minSize = new Vector2( 420, 600 ); // Set minimum width and height for the window
        window.maxSize = new Vector2( 420, 600 ); // Set maximum width and height for the window
        window.Show();
    }

    private void OnGUI()
    {
        float scrollViewWidth = 420f; // Width of the scroll view
        GUILayout.BeginArea( new Rect( ( Screen.width - scrollViewWidth ) / 2, 20, scrollViewWidth, Screen.height - 40 ) );
        scrollPosition = GUILayout.BeginScrollView( scrollPosition, GUILayout.Width( scrollViewWidth ) );


        GUILayout.BeginVertical();
        GUILayout.Label( Resources.Load< Texture2D >( "CustomEditor/Welcome" ), GUILayout.Width( 400 ), GUILayout.Height( 100 ) );

        // Header Label
        var headerStyle = new GUIStyle( GUI.skin.label );
        headerStyle.fontSize = 18;
        headerStyle.fontStyle = FontStyle.Bold;
        var subHeaderStyle = new GUIStyle( GUI.skin.label );
        subHeaderStyle.fontSize = 16;
        subHeaderStyle.fontStyle = FontStyle.Bold;

        // Description Label
        var descriptionStyle = new GUIStyle( GUI.skin.label );
        descriptionStyle.wordWrap = true; // Allow wrapping for the description
        GUILayout.Label( "Seamless Prop-Based Design:", headerStyle, GUILayout.Width( 400 ) );
        GUILayout.Label(
            "Remap revolutionizes map creation by exclusively focusing on the power of props. Say goodbye to traditional terrain limitations and hello to a world where every structure, detail, and element is meticulously placed using an extensive library of props. Immerse players in a completely fresh Apex Legends experience with your custom-crafted environments.",
            descriptionStyle, GUILayout.Width( 400 ) );
        GUILayout.Space( 10 );
        GUILayout.Label( "Tutorials, Support & Documentation", headerStyle, GUILayout.Width( 400 ) );
        GUILayout.Space( 5 );
        GUILayout.Label( "Documentation", subHeaderStyle, GUILayout.Width( 400 ) );
        GUILayout.Label( "Remap documentation can be found at https://docs.ayezee.app/", descriptionStyle, GUILayout.Width( 400 ) );
        if ( GUILayout.Button( "Visit Documentation" ) )
            Application.OpenURL( "https://docs.ayezee.app/" );
        GUILayout.Space( 5 );
        GUILayout.Label( "Support", subHeaderStyle, GUILayout.Width( 400 ) );
        GUILayout.Label( "Get access to support on our Discord Server!", descriptionStyle, GUILayout.Width( 400 ) );
        GUILayout.Space( 5 );
        if ( GUILayout.Button( "Join Discord Server" ) )
            Application.OpenURL( "https://discord.gg/3f72WZJN6Z" );

        GUILayout.Space( 20 );
        GUILayout.Label( "First Steps", headerStyle, GUILayout.Width( 400 ) );
        GUILayout.Space( 5 );
        GUILayout.Label( "We highly recommend you to navigate to 'Levels/Example Level', where you will find a premade example level/", descriptionStyle, GUILayout.Width( 400 ) );
        GUILayout.Label( "Feel free to reach out on Discord if you need help!", descriptionStyle, GUILayout.Width( 400 ) );
        GUILayout.EndVertical();

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}