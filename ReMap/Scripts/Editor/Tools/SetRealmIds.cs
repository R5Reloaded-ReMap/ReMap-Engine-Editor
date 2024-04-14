using UnityEditor;
using UnityEngine;

public class SetRealmIds : EditorWindow
{
    private int RealmID;
    private GameObject source;

    public static void Init()
    {
        var window = ( SetRealmIds )GetWindow( typeof(SetRealmIds), false, "RealmID Tool" );
        window.Show();
        window.minSize = new Vector2( 375, 70 );
        window.maxSize = new Vector2( 375, 70 );
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical( "box" );
        source = EditorGUILayout.ObjectField( source, typeof(Object), true ) as GameObject;
        RealmID = EditorGUILayout.IntField( "RealmID:", RealmID );
        GUILayout.EndVertical();

        if ( GUILayout.Button( "Set Realm IDS" ) )
            SetID();
    }

    private void SetID()
    {
        if ( source == null )
            return;

        foreach ( Transform child in source.transform )
        {
            var script = child.gameObject.GetComponent< PropScript >();
            if ( script != null )
                script.RealmID = RealmID;
        }
    }
}