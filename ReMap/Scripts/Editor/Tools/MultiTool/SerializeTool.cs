
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;

namespace MultiTool
{
    internal class SerializeTool
    {
        private enum DirectionType
        {
            Top,
            Bottom,
            Forward,
            Backward,
            Left,
            Right
        }

        private enum BoundsInt
        {
            Size,
            Max,
            Min,
            Center,
            Extents
        }

        private static string[] calculationMethod = new string[] {"Bounds only", "Bounds + XYZ entry", "XYZ entry only"};

        private static float x_entry = 0.0f;
        private static float y_entry = 0.0f;
        private static float z_entry = 0.0f;
        private static float x_spacing = 0.0f;
        private static float y_spacing = 0.0f;
        private static float z_spacing = 0.0f;
        private static bool unidirectionalMode = false;
        private static int selectedMethod = 0;
        private static int selectedMethod_temp = 0;

        private static GameObject[] source;
        private static Vector3 BoundsSize;
        private static Vector3 BoundsMax;
        private static Vector3 BoundsMin;
        private static Vector3 BoundsCenter;
        private static Vector3 BoundsExtents;

        internal static void OnGUI()
        {
            //GUIStyle centeredStyle = new GUIStyle( GUI.skin.label );
            //centeredStyle.alignment = TextAnchor.MiddleCenter;
            //centeredStyle.fontSize = 18;

            //GUILayout.Label( "Amount of props selected: " + Selection.count.ToString(), centeredStyle, GUILayout.ExpandWidth( true ) );

            //Space( 6 );

            if ( selectedMethod != selectedMethod_temp )
            {
                ChangeWindowSize();
                selectedMethod_temp = selectedMethod;
            }

            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    CreateToggle( ref unidirectionalMode, "Unidirectional Mode", "", 128 );
                GUILayout.EndHorizontal();

                CreateTextInfo( "Method of calculation:" );
                selectedMethod = GUILayout.Toolbar( selectedMethod, calculationMethod );

                if ( selectedMethod != 0 )
                {
                    Space( 2 );

                    GUILayout.BeginHorizontal();
                        CreateTextInfo( "T/U Spacing: ", "", 120 );
                        float.TryParse( GUILayout.TextField( y_entry.ToString(), GUILayout.Width( 120 ) ), out y_entry );
                        CreateButton( "Reset T/U values", "", () => { y_entry = 0; } );
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                        CreateTextInfo( "F/B Spacing: ", "", 120 );
                        float.TryParse( GUILayout.TextField( z_entry.ToString(), GUILayout.Width( 120 ) ), out z_entry );
                        CreateButton( "Reset F/B values", "", () => { z_entry = 0; } );
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                        CreateTextInfo( "L/R Spacing: ", "", 120 );
                        float.TryParse( GUILayout.TextField( x_entry.ToString(), GUILayout.Width( 120 ) ), out x_entry );
                        CreateButton( "Reset L/R values", "", () => { x_entry = 0; } );
                    GUILayout.EndHorizontal();

                    CreateButton( "Reset all values", "", () => ResetValues() );
                }

                GUILayout.BeginHorizontal();
                    Separator( 584 );
                GUILayout.EndHorizontal();

                    Space( 8 );

                GUILayout.BeginHorizontal();
                    Space( 196 );
                    CreateButton( "Duplicate to FORWARD", "", () => DuplicateInit( DirectionType.Forward ), 193 );
                GUILayout.EndHorizontal();
                    
                GUILayout.BeginHorizontal();
                    CreateButton( "Duplicate to LEFT", "", () => DuplicateInit( DirectionType.Left ), 193 );
                    CreateButton( "Duplicate to BACKWARD", "", () => DuplicateInit( DirectionType.Backward ), 193 );
                    CreateButton( "Duplicate to RIGHT", "", () => DuplicateInit( DirectionType.Right ), 193 );
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    CreateButton( "Duplicate to TOP", "", () => DuplicateInit( DirectionType.Top ), 193 );
                        Space( 196 );
                    CreateButton( "Duplicate to BOTTOM", "", () => DuplicateInit( DirectionType.Bottom ), 193 );
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void DuplicateInit( DirectionType directionType )
        {
            source = Helper.GetAllObjectTypeWithEnum( ObjectType.Prop, true );

            if ( source.Length == 0 )
            {
                UnityInfo.Printt( "No objects to duplicate" );
                return;
            }

            Vector3[] DeterminedBounds = DetermineBoundsInSelection();
            BoundsSize = DeterminedBounds[ ( int ) BoundsInt.Size ];
            BoundsMax = DeterminedBounds[ ( int ) BoundsInt.Max ];
            BoundsMin = DeterminedBounds[ ( int ) BoundsInt.Min ];
            BoundsCenter = DeterminedBounds[ ( int ) BoundsInt.Center ];
            BoundsExtents = DeterminedBounds[ ( int ) BoundsInt.Extents ];

            ReMapConsole.Log($"[Serialize Tool] Bounds Size:    {BoundsSize}", ReMapConsole.LogType.Info);
            ReMapConsole.Log($"[Serialize Tool] Bounds Max:     {BoundsMax}",  ReMapConsole.LogType.Info);
            ReMapConsole.Log($"[Serialize Tool] Bounds Min:     {BoundsMin}",  ReMapConsole.LogType.Info);
            ReMapConsole.Log($"[Serialize Tool] Bounds Center:  {BoundsCenter}",  ReMapConsole.LogType.Info);
            ReMapConsole.Log($"[Serialize Tool] Bounds Extents: {BoundsExtents}",  ReMapConsole.LogType.Info);

            switch( selectedMethod )
            {
                case 0:
                    x_spacing = BoundsSize.x;
                    y_spacing = BoundsSize.y;
                    z_spacing = BoundsSize.z;
                    break;
                case 1:
                    x_spacing = x_entry + BoundsSize.x;
                    y_spacing = y_entry + BoundsSize.y;
                    z_spacing = z_entry + BoundsSize.z;
                    break;
                case 2:
                    x_spacing = x_entry;
                    y_spacing = y_entry;
                    z_spacing = z_entry;
                break;
            }

            DuplicateProps( directionType );
        }

        private static void DuplicateProps( DirectionType directionType )
        {
            GameObject[] newSource = new GameObject[0];

            foreach ( GameObject go in source )
            {
                if ( !Helper.IsValid( go ) ) continue;

                PropScript script = ( PropScript ) Helper.GetComponentByEnum( go, ObjectType.Prop );

                if ( !Helper.IsValid( script ) ) continue;

                GameObject obj = Helper.CreateGameObject( "", go.name, PathType.Name );

                if ( !Helper.IsValid( obj ) ) continue;

                PropScript scriptInstance = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );

                if ( !Helper.IsValid( scriptInstance ) ) continue;

                SetPropOrigin( go, obj, directionType );
                obj.transform.eulerAngles = go.transform.eulerAngles;
                obj.gameObject.transform.localScale = go.transform.lossyScale;

                obj.transform.SetParent( Helper.FindParent( go ) );

                Helper.ApplyComponentScriptData< PropScript >( script, scriptInstance );

                Helper.ArrayAppend( ref newSource, obj );
            }

            Selection.objects = newSource;
        }

        private static void SetPropOrigin( GameObject reference, GameObject newGo, DirectionType directionType )
        {
            float x = x_spacing;
            float y = y_spacing;
            float z = z_spacing;

            Transform refTranform = reference.transform;
            Vector3 origin = refTranform.position;

            Transform goTranform = newGo.transform;

            Quaternion referenceRotation = refTranform.rotation;
            refTranform.rotation = Quaternion.Euler(0, 0, 0);
            Bounds bounds = new Bounds();
            foreach( Renderer o in reference.GetComponentsInChildren< Renderer >() )
            {
                bounds.Encapsulate( o.bounds );
            }
            refTranform.rotation = referenceRotation;

            switch( directionType )
            {
                case DirectionType.Top:
                    if( unidirectionalMode )
                    {
                        goTranform.position = origin + ( source[0].transform.up * y );
                    }
                    else goTranform.position = origin + ( refTranform.up * y );
                    break;

                case DirectionType.Bottom:
                    if( unidirectionalMode )
                    {
                        goTranform.position = origin + ( source[0].transform.up * -y );
                    }
                    else goTranform.position = origin + ( refTranform.up * -y );
                    break;

                case DirectionType.Forward:
                    if( unidirectionalMode )
                    {
                        goTranform.position = origin + ( source[0].transform.forward * z );
                    }
                    else goTranform.position = origin + ( refTranform.forward * z );
                    break;

                case DirectionType.Backward:
                    if( unidirectionalMode )
                    {
                        goTranform.position = origin + ( source[0].transform.forward * -z );
                    }
                    else goTranform.position = origin + ( refTranform.forward * -z );
                    break;

                case DirectionType.Left:
                    if( unidirectionalMode )
                    {
                        goTranform.position = origin + ( source[0].transform.right * -x );
                    }
                    else goTranform.position = origin + ( refTranform.right * -x );
                    break;

                case DirectionType.Right:
                    if( unidirectionalMode )
                    {
                        goTranform.position = origin + ( source[0].transform.right * x );
                    }
                    else goTranform.position = origin + ( refTranform.right * x );
                    break;

                default: break;
            }
        }

        private static Vector3[] DetermineBoundsInSelection()
        {
            List< GameObject > newSource = new List< GameObject >();

            GameObject sourceParent = new GameObject();
            sourceParent.transform.position = source[0].transform.position;
            sourceParent.transform.eulerAngles = source[0].transform.eulerAngles;

            foreach ( GameObject go in source )
            {
                if ( !Helper.IsValid( go ) ) continue;

                GameObject obj = Helper.CreateGameObject( "", go.name, PathType.Name );

                if ( !Helper.IsValid( obj ) ) continue;

                Helper.ApplyTransformData( go, obj );

                obj.transform.SetParent( sourceParent.transform );

                newSource.Add( obj );
            }

            sourceParent.transform.eulerAngles = Vector3.zero;

            Bounds bounds = new Bounds();
            bool firstRenderer = true;
            foreach ( GameObject go in newSource )
            {
                foreach ( Renderer renderer in go.GetComponentsInChildren< Renderer >() )
                {
                    if ( firstRenderer )
                    {
                        bounds = renderer.bounds;
                        firstRenderer = false;
                    }
                    else
                    {
                        bounds.Encapsulate( renderer.bounds );
                    }
                }
            }

            Vector3[] boundsArray = new Vector3[5];
            boundsArray[ ( int ) BoundsInt.Size ] = bounds.size;
            boundsArray[ ( int ) BoundsInt.Max ] = bounds.max;
            boundsArray[ ( int ) BoundsInt.Min ] = bounds.min;
            boundsArray[ ( int ) BoundsInt.Center ] = bounds.center;
            boundsArray[ ( int ) BoundsInt.Extents ] = bounds.extents;

            foreach ( GameObject go in newSource )
            {
                if ( Helper.IsValid( go ) ) GameObject.DestroyImmediate( go );
            }

            if ( Helper.IsValid( sourceParent ) ) GameObject.DestroyImmediate( sourceParent );

            return boundsArray;
        }

        internal static void ChangeWindowSize()
        {
            if ( selectedMethod == 0 )
            {
                MultiToolWindow.windowInstance.minSize = new Vector2( 600, 206 );
                MultiToolWindow.windowInstance.maxSize = new Vector2( 600, 206 );
            }
            else
            {
                MultiToolWindow.windowInstance.minSize = new Vector2( 600, 288 );
                MultiToolWindow.windowInstance.maxSize = new Vector2( 600, 288 );
            }
        }

        private static void ResetValues()
        {
            z_entry = 0.0f;
            x_entry = 0.0f;
            y_entry = 0.0f;
        }


        // Unusued
        private static Vector3[] DetermineBoundsInProp( GameObject go )
        {
            GameObject sourceParent = new GameObject();
            sourceParent.transform.position = go.transform.position;
            sourceParent.transform.eulerAngles = go.transform.eulerAngles;

            string name = go.name;

            //if( !IsValidGameObject( go ) )
            //    return new Vector3[4];

            UnityEngine.Object loadedPrefabResource = UnityInfo.FindPrefabFromName(name);
            if (loadedPrefabResource == null)
            {
                ReMapConsole.Log($"[Serialize Tool] Couldn't find prefab with name of: {name}", ReMapConsole.LogType.Error);
                return new Vector3[4];
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = go.transform.position;
            obj.transform.eulerAngles = go.transform.eulerAngles;
            obj.transform.localScale = go.transform.localScale;

            obj.transform.SetParent(sourceParent.transform);

            sourceParent.transform.eulerAngles = Vector3.zero;

            Bounds bounds = new Bounds();
            bool firstRenderer = true;
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                if (firstRenderer)
                {
                    bounds = renderer.bounds;
                    firstRenderer = false;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            Vector3[] boundsArray = new Vector3[4];
            boundsArray[(int)BoundsInt.Size] = bounds.size;
            boundsArray[(int)BoundsInt.Max] = bounds.max;
            boundsArray[(int)BoundsInt.Min] = bounds.min;
            boundsArray[(int)BoundsInt.Center] = bounds.center;

            GameObject.DestroyImmediate(obj);
            GameObject.DestroyImmediate(sourceParent);

            return boundsArray;
        }
    }
}
