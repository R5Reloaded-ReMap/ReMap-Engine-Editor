using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static WindowUtility.WindowUtility;

namespace MultiTool
{
    internal class ObjectInfo
    {
        private static GameObject selectedObject;
        private static Component objectComponent;
        private static ObjectType objectType;
        private static bool objectIsValid;
        private static float w, d, h;

        private static bool showExtention = true;
        private static Vector2 scroll;

        private static readonly ObjectType[] showOnly =
        {
            ObjectType.Prop,
            ObjectType.WeaponRack,
            ObjectType.SingleDoor,
            ObjectType.DoubleDoor,
            ObjectType.HorzDoor,
            ObjectType.VerticalDoor,
            ObjectType.Jumppad,
            ObjectType.LootBin,
            ObjectType.Button
        };

        internal static void OnGUI()
        {
            if ( Helper.IsEmpty( Selection.gameObjects ) )
            {
                GUILayout.BeginVertical( "box" );
                FlexibleSpace();

                CreateTextInfoCentered( "No object selected." );

                FlexibleSpace();
                GUILayout.EndVertical();

                return;
            }

            CheckObject();
            ShortCut();

            GUILayout.BeginVertical( "box" );

            if ( !Helper.IsValid( selectedObject ) || !objectIsValid )
            {
                FlexibleSpace();

                CreateTextInfoCentered( "The selected object is not valid." );

                FlexibleSpace();
                GUILayout.EndVertical();

                return;
            }

            GUILayout.BeginHorizontal();
            CreateToggle( ref showExtention, "Show name extention:", "", 128 );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string unityName = UnityInfo.GetUnityModelName( selectedObject.name, showExtention );
            CreateTextField( ref unityName, "Unity Name:", "", 100 );
            CreateCopyButton( "copy", "", unityName, 100 );
            GUILayout.EndHorizontal();

            if ( Helper.IsObjectFromTag( selectedObject, ObjectType.Prop ) )
            {
                GUILayout.BeginHorizontal();
                string apexName = UnityInfo.GetApexModelName( selectedObject.name, showExtention );
                CreateTextField( ref apexName, "Apex Name:", "", 100 );
                CreateCopyButton( "copy", "", apexName, 100 );
                GUILayout.EndHorizontal();
            }

            Space( 4 );

            GUILayout.BeginHorizontal();
            string unityOrigin = Helper.BuildOrigin( selectedObject, VectorType.Unity );
            CreateTextField( ref unityOrigin, "Unity Origin:", "", 100 );
            CreateCopyButton( "copy", "", unityOrigin, 100 );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string unityAngles = Helper.BuildAngles( selectedObject, VectorType.Unity );
            CreateTextField( ref unityAngles, "Unity Angles:", "", 100 );
            CreateCopyButton( "copy", "", unityAngles, 100 );
            GUILayout.EndHorizontal();

            if ( Helper.IsObjectFromTag( selectedObject, showOnly ) )
            {
                GUILayout.BeginHorizontal();
                string unitySize = $"X: {Helper.ReplaceComma( d ),-12} Y: {Helper.ReplaceComma( h ),-12} Z: {Helper.ReplaceComma( w ),-12}";
                CreateTextField( ref unitySize, "Unity Size:", "X = Depth\nY = Height\nZ = Width", 100 );
                CreateCopyButton( "copy", "", unitySize, 100 );
                GUILayout.EndHorizontal();
            }

            Space( 4 );

            GUILayout.BeginHorizontal();
            string apexOrigin = Helper.BuildOrigin( selectedObject );
            CreateTextField( ref apexOrigin, "Apex Origin:", "", 100 );
            CreateCopyButton( "copy", "", apexOrigin, 100 );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string apexAngles = Helper.BuildAngles( selectedObject );
            CreateTextField( ref apexAngles, "Apex Angles:", "", 100 );
            CreateCopyButton( "copy", "", apexAngles, 100 );
            GUILayout.EndHorizontal();

            if ( Helper.IsObjectFromTag( selectedObject, showOnly ) )
            {
                GUILayout.BeginHorizontal();
                string apexSize = $"X: {Helper.ReplaceComma( w ),-12} Y: {Helper.ReplaceComma( d ),-12} Z: {Helper.ReplaceComma( h ),-12}";
                CreateTextField( ref apexSize, "Apex Size:", "X = Width\nY = Depth\nZ = Height", 100 );
                CreateCopyButton( "copy", "", apexSize, 100 );
                GUILayout.EndHorizontal();
            }

            Space( 4 );

            GUILayout.BeginHorizontal();
            string scale = Helper.ReplaceComma( selectedObject.transform.localScale.x );
            ;
            CreateTextField( ref scale, "Global Scale:", "", 100 );
            CreateCopyButton( "copy", "", scale, 100 );
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical( "box" );

            CreateTextInfo( $"Object Parameters: (Read Only) - Type: {Helper.GetObjNameWithEnum( objectType )}" );

            var fields = objectComponent.GetType().GetFields( BindingFlags.Public | BindingFlags.Instance );

            if ( Helper.IsEmpty( fields ) ) CreateTextInfo( "// None." );

            scroll = EditorGUILayout.BeginScrollView( scroll );

            foreach ( var field in fields )
            {
                if ( Attribute.IsDefined( field, typeof(HideInInspector) ) ) continue;

                GUILayout.BeginHorizontal();
                string value = field.GetValue( objectComponent ).ToString();
                CreateTextField( ref value, $"{field.Name}:", "", 160 );
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private static void ShortCut()
        {
            if ( Input.GetKeyDown( KeyCode.Return ) || Event.current.type == EventType.MouseDown )
                GUI.FocusControl( "" ); // remove focus from current control
        }

        private static void CheckObject()
        {
            var obj = selectedObject;

            selectedObject = Selection.gameObjects[Selection.gameObjects.Length - 1];

            if ( obj != selectedObject )
            {
                objectIsValid = UnityInfo.PrefabExist( selectedObject.name );

                w = d = h = 0;

                if ( objectIsValid )
                {
                    objectType = Helper.GetTypeFromObject( selectedObject );

                    foreach ( var component in selectedObject.GetComponents< Component >() )
                        if ( Helper.IsValid( component ) && component == Helper.GetComponentByEnum( selectedObject, objectType ) )
                        {
                            objectComponent = component;

                            break;
                        }

                    foreach ( var o in selectedObject.GetComponentsInChildren< Renderer >() )
                    {
                        if ( o.bounds.size.z > w ) w = o.bounds.size.z;
                        if ( o.bounds.size.x > d ) d = o.bounds.size.x;
                        if ( o.bounds.size.y > h ) h = o.bounds.size.y;
                    }
                }
            }
        }
    }
}