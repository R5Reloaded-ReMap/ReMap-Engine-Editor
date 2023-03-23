
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ImportExport.Shared
{
    public enum GetSetData
    {
        Get = 0,
        Set = 1
    }

    public class SharedFunction
    {
        /// <summary>
        /// Transfer all TSource values in TDestination
        /// </summary>
        public static void TransferDataToClass< TSource, TDestination >( TSource source, TDestination destination, List< string > propertiesToRemove = null )
        {
            if ( source == null || destination == null )
            {
                throw new ArgumentNullException( "Source or destination object cannot be null." );
            }

            Type sourceType = typeof( TSource );
            Type destinationType = typeof( TDestination );

            FieldInfo[] sourceFields = sourceType.GetFields( BindingFlags.Public | BindingFlags.Instance );
            FieldInfo[] destinationFields = destinationType.GetFields( BindingFlags.Public | BindingFlags.Instance );

            foreach ( FieldInfo sourceField in sourceFields )
            {
                // Ignore properties that are in the propertiesToRemove list
                if ( propertiesToRemove != null && propertiesToRemove.Contains( sourceField.Name, StringComparer.OrdinalIgnoreCase ) )
                {
                    continue;
                }

                FieldInfo destinationField = Array.Find( destinationFields, field => field.Name.Equals( sourceField.Name, StringComparison.OrdinalIgnoreCase ) );

                // Check if the destination field exists and has the same field type as the source field
                if ( destinationField != null && destinationField.FieldType == sourceField.FieldType )
                {
                    object value = sourceField.GetValue( source );
                    destinationField.SetValue( destination, value );
                }
            }
        }
        
        /// <summary>
        /// Get path for a GameObject
        /// </summary>
        public static List< PathClass > FindPath( GameObject obj )
        {
            List< GameObject > parents = new List< GameObject >();
            List< PathClass > pathList = new List< PathClass >();
            GameObject currentParent = obj;

            // find all parented game objects
            while ( currentParent.transform.parent != null )
            {
                if ( currentParent != obj ) parents.Add( currentParent );
                currentParent = currentParent.transform.parent.gameObject;
            }

            if ( currentParent != obj ) parents.Add( currentParent );

            foreach ( GameObject parent in parents )
            {
                PathClass path = new PathClass();
                path.FolderName = parent.name.Replace( "/", "|" );
                path.TransformData = GetSetTransformData( parent );
                pathList.Add( path );
            }

            pathList.Reverse();

            return pathList;
        }

        /// <summary>
        /// Get path string for a GameObject
        /// </summary>
        public static string FindPathString( GameObject obj )
        {
            List< GameObject > parents = new List< GameObject >();
            string path = "";
            GameObject currentParent = obj;

            // find all parented game objects
            while ( currentParent.transform.parent != null )
            {
                if ( currentParent != obj ) parents.Add( currentParent );
                currentParent = currentParent.transform.parent.gameObject;
            }

            if ( currentParent != obj ) parents.Add( currentParent );

            parents.Reverse();

            foreach ( GameObject parent in parents )
            {
                if ( string.IsNullOrEmpty( path ) )
                {
                    path = $"{parent.name}";
                }
                else path = $"{path}/{parent.name}";
            }

            return path;
        }

        /// <summary>
        /// Create path for a GameObject
        /// </summary>
        public static void CreatePath( List< PathClass > pathList, string pathString, GameObject obj )
        {
            if ( string.IsNullOrEmpty( pathString ) ) return;

            GameObject folder = null; string path = "";

            foreach ( PathClass pathClass in pathList )
            {
                if ( string.IsNullOrEmpty( path ) )
                {
                    path = $"{pathClass.FolderName}";
                }
                else path = $"{path}/{pathClass.FolderName}";

                GameObject newFolder = GameObject.Find( path );

                if ( newFolder == null ) newFolder = new GameObject( pathClass.FolderName );

                TransformData transformData = pathClass.TransformData;
                newFolder.transform.position = transformData.position;
                newFolder.transform.eulerAngles = transformData.eulerAngles;
                newFolder.transform.localScale = transformData.localScale;

                if ( folder != null ) newFolder.transform.SetParent( folder.transform );

                folder = newFolder;
            }

            if ( folder != null ) obj.transform.parent = folder.transform;
        }

        /// <summary>
        /// Get or Set the position, eulerAngles, and localScale for a GameObject
        /// </summary>
        public static TransformData GetSetTransformData( GameObject obj, TransformData data = null )
        {
            if ( data == null ) // if data is null, get the transformation data
            {
                data = new TransformData();
                data.position = obj.transform.position;
                data.eulerAngles = obj.transform.eulerAngles;
                data.localScale = obj.transform.localScale;

                return data;
            }
            else // otherwise, define the transformation data provided
            {
                obj.transform.position = data.position;
                obj.transform.eulerAngles = data.eulerAngles;
                obj.transform.localScale = data.localScale;

                return null;
            }
        }
    }
}
