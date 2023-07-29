
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;

public class ReTexture : EditorWindow
{
    private static UnityEngine.Object obj;

    public static void Init()
    {
        ReTexture window = (ReTexture)EditorWindow.GetWindow(typeof(ReTexture), false, "ReMap Debug Console");
        window.Show();
    }

    void OnGUI()
    {
        CreateObjectField( ref obj, "Object Ref:" );

        CreateButton( "ReTexture", "", () => OnButtonPressed() );
    }

    private static async void OnButtonPressed()
    {
        if ( Helper.IsValid( obj ) )
        {
            await LibrarySorter.LibrarySorterWindow.CheckTextures( ( GameObject ) obj );
        }

        await Helper.Wait();
    }
}