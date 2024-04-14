using UnityEditor;
using UnityEngine;
using static WindowUtility.WindowUtility;
using Object = UnityEngine.Object;

namespace MultiTool
{
    internal class ModelSwap
    {
        private static GameObject[] prefabs = new GameObject[1];
        private static GameObject[] activeSelection;
        private static GameObject[] newSelection;
        private static bool randomlyChanges = true;
        private static Vector2 scroll;

        internal static void OnGUI()
        {
            var labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;

            if ( prefabs.Length == 0 ) Helper.ArrayResize( ref prefabs, 1 );

            GUILayout.BeginVertical( "box" );
            EditorGUILayout.BeginHorizontal();
            CreateToggle( ref randomlyChanges, "Change randomly the selection: ", "", 190 );
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical( "box" );
            scroll = EditorGUILayout.BeginScrollView( scroll );

            for ( int i = 0; i < prefabs.Length; i++ )
            {
                int idx = i + 1;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( $"Prefab {idx.ToString( "00" )}: ", labelStyle, GUILayout.Width( 60 ) );
                prefabs[i] = EditorGUILayout.ObjectField( prefabs[i], typeof(GameObject), true ) as GameObject;
                CreateButton( $"Clear Prefab {idx.ToString( "00" )}", "", () => RemoveGameObject( i ), 120 );
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            CreateButton( "Remove 1 Row", "", () => Helper.ArrayResize( ref prefabs, -1 ) );
            CreateButton( "Add 1 Row", "", () => Helper.ArrayResize( ref prefabs, 1 ) );
            CreateButton( "Remove Null Prefabs", "", () => Helper.RemoveNull( ref prefabs ), 140 );
            EditorGUILayout.EndHorizontal();

            CreateButton( "Change Selection", "", () => ChangeSelection() );
            GUILayout.EndVertical();
        }

        private static void ChangeSelection()
        {
            activeSelection = Helper.GetAllObjectTypeWithEnum( ObjectType.Prop, true );
            newSelection = activeSelection;

            CheckArray();

            if ( !Helper.IsValid( prefabs ) ) return;

            foreach ( var obj in activeSelection )
                if ( !randomlyChanges || Helper.CoinFlip() )
                    SwapModel( obj );

            Helper.RemoveNull( ref newSelection );
            Selection.objects = newSelection;
        }

        private static void SwapModel( GameObject obj )
        {
            var script = ( PropScript )Helper.GetComponentByEnum( obj, ObjectType.Prop );

            if ( !Helper.IsValid( script ) ) return;

            GameObject chosenObject;
            do
            {
                chosenObject = prefabs[Random.Range( 0, prefabs.Length )];
                if ( chosenObject.name == obj.name && prefabs.Length > 1 )
                    chosenObject = null;
            } while (chosenObject == null && prefabs.Length > 1);

            var newObj = Helper.CreateGameObject( "", chosenObject.name, PathType.Name );
            if ( !Helper.IsValid( newObj ) ) return;

            Helper.ApplyTransformData( obj, newObj );

            var newScript = ( PropScript )Helper.GetComponentByEnum( newObj, ObjectType.Prop );

            if ( !Helper.IsValid( newScript ) ) return;

            Helper.ApplyComponentScriptData( script, newScript );

            if ( Helper.IsValid( obj ) ) Object.DestroyImmediate( obj );

            Helper.ArrayAppend( ref newSelection, newObj );
        }

        private static void CheckArray()
        {
            for ( int i = 0; i < prefabs.Length; i++ )
                if ( !prefabs[i].CompareTag( Helper.GetObjTagNameWithEnum( ObjectType.Prop ) ) )
                    prefabs[i] = null;
        }

        private static void RemoveGameObject( int i )
        {
            prefabs[i] = null;
        }
    }
}