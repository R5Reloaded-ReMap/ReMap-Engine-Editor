using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetLibraryManager
{
    //[CreateAssetMenu(fileName = "AssetLibrary_Settings", menuName = "Asset Library Settings", order = 1)]
    public class SettingsObject : ScriptableObject
    {
        [Space( 30 )] public List< CreateNewSection > createNewSection = new();

        [Space( 10 )] [Tooltip( "Exclude all prefabs in these folders, particles and canvas have bad or no thumbails, you can remove them here." )]
        public List< ExcludedFolder > excludedFolders = new();

        [Space( 10 )] [Tooltip( "Exclude individual Prefabs not already in Excluded Folders" )]
        public List< Object > excludedPrefabs = new();

        [Space( 30 )] [Tooltip( "Include all prefabs in these folders, if left empty it will search the whole project for prefabs." )]
        public List< IncludedFolder > includedFolders = new();

        [Space( 30 )] [Tooltip( "Include individual Prefabs not already in Included Folders" )]
        public List< Object > includedPrefabs = new();

        [Space( 10 )] public Options options;

        [Space( 10 )] public Placement placement;
    }

    [CustomPropertyDrawer( typeof(IncludedFolder) )]
    public class IncludedFolderDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = -9;


            var someContent = new GUIContent();

            // Calculate rects
            var folderRect = new Rect( position.x + 125, position.y, position.width - 125, position.height );

            if ( property.FindPropertyRelative( "includedFolder" ).objectReferenceValue != null )
            {
                //Get whatever is in the object field
                string path = AssetDatabase.GetAssetPath( property.FindPropertyRelative( "includedFolder" ).objectReferenceValue );
                someContent.text = path.Replace( "Assets/", "" ).Replace( property.FindPropertyRelative( "includedFolder" ).objectReferenceValue.name, "" );
            }

            EditorGUI.PropertyField( folderRect, property.FindPropertyRelative( "includedFolder" ), someContent );


            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
        }
    }

    [CustomPropertyDrawer( typeof(ExcludedFolder) )]
    public class ExcludedFolderDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = -9;


            var someContent = new GUIContent();

            // Calculate rects
            var folderRect = new Rect( position.x + 125, position.y, position.width - 125, position.height );

            if ( property.FindPropertyRelative( "excludedFolder" ).objectReferenceValue != null )
            {
                //Get whatever is in the object field
                string path = AssetDatabase.GetAssetPath( property.FindPropertyRelative( "excludedFolder" ).objectReferenceValue );
                someContent.text = path.Replace( "Assets/", "" ).Replace( property.FindPropertyRelative( "excludedFolder" ).objectReferenceValue.name, "" );
            }

            EditorGUI.PropertyField( folderRect, property.FindPropertyRelative( "excludedFolder" ), someContent );


            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
        }
    }

    public class SelectSettingsObject : EditorWindow
    {
        public static void SelectSettings()
        {
            Selection.activeObject = Resources.Load( "AssetLibrary_Settings" );
        }
    }
}