using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static WindowUtility.WindowUtility;
using Object = UnityEngine.Object;

namespace MultiTool
{
    internal class ComponentTransfer
    {
        private static Object source;
        private static Component objectComponent;
        private static ObjectType objectType;

        private static Vector2 scroll;

        internal static void OnGUI()
        {
            CheckObject();

            GUILayout.BeginVertical( "box" );
            GUILayout.BeginHorizontal();
            CreateObjectField( ref source, "Component Source:", "", 120, 20 );
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if ( source == null || objectComponent == null )
            {
                GUILayout.BeginVertical( "box" );
                FlexibleSpace();
                CreateTextInfoCentered( "Source is not valid" );
                FlexibleSpace();
                GUILayout.EndVertical();

                return;
            }

            var fields = objectComponent.GetType().GetFields( BindingFlags.Public | BindingFlags.Instance );

            string objName = Helper.GetObjNameWithEnum( objectType );

            if ( Helper.IsEmpty( fields ) )
            {
                GUILayout.BeginVertical( "box" );
                FlexibleSpace();
                CreateTextInfoCentered( $"{objName} Objects does not contain any parameters" );
                FlexibleSpace();
                GUILayout.EndVertical();

                return;
            }

            GUILayout.BeginVertical( "box" );

            CreateTextInfo( $"Object Parameters: (Read Only) - Type: {objName}" );

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

            var selection = Helper.GetAllObjectTypeWithEnum( objectType, true ).Where( obj => obj != source ).ToArray();

            GUILayout.BeginVertical( "box" );
            if ( Helper.IsEmpty( selection ) )
                CreateTextInfoCentered( $"Select at least 1 type object: {objName}" );
            else
                CreateButton( $"Apply Component Data To {selection.Length} Objects", "", () => TransfertComponentData( selection ) );
            GUILayout.EndVertical();
        }

        private static void TransfertComponentData( GameObject[] array )
        {
            foreach ( var obj in array )
            foreach ( var component in obj.GetComponents< Component >() )
                if ( Helper.IsValid( component ) && component == Helper.GetComponentByEnum( obj, objectType ) )
                {
                    var fields = objectComponent.GetType().GetFields( BindingFlags.Public | BindingFlags.Instance );

                    foreach ( var field in fields )
                    {
                        object value = field.GetValue( objectComponent );
                        field.SetValue( component, value );
                    }

                    break;
                }
        }

        private static void CheckObject()
        {
            if ( source == null ) return;

            var obj = ( GameObject )source;
            objectComponent = null;

            objectType = Helper.GetTypeFromObject( obj );

            foreach ( var component in obj.GetComponents< Component >() )
                if ( Helper.IsValid( component ) && component == Helper.GetComponentByEnum( obj, objectType ) )
                {
                    objectComponent = component;

                    break;
                }
        }
    }
}