using UnityEditor;

public static class TagHelper
{
    public static void AddTag( string tag )
    {
        var asset = AssetDatabase.LoadAllAssetsAtPath( "ProjectSettings/TagManager.asset" );
        if ( asset != null && asset.Length > 0 )
        {
            var so = new SerializedObject( asset[0] );
            var tags = so.FindProperty( "tags" );

            for ( int i = 0; i < tags.arraySize; ++i )
                if ( tags.GetArrayElementAtIndex( i ).stringValue == tag )
                    return; // Tag already present, nothing to do.

            tags.InsertArrayElementAtIndex( 0 );
            tags.GetArrayElementAtIndex( 0 ).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();
        }
    }

    public static void CheckAndCreateTags()
    {
        foreach ( string tag in Helper.GetAllTags() ) AddTag( tag );
    }
}