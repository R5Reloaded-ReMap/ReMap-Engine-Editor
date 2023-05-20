
using UnityEngine;
using UnityEditor;

using static WindowUtility.WindowUtility;

namespace MultiTool
{
    internal class DistanceMeter
    {
        private static Object source;
        private static Object target;
        private static Vector3 sourceOrigin;
        private static Vector3 targetOrigin;
        private static bool axisX = true;
        private static bool axisY = false;
        private static bool axisZ = false;
        private static string distance = "";

        internal static void OnGUI()
        {
            sourceOrigin = new Vector3( 0, 0, 0 );
            targetOrigin = new Vector3( 0, 0, 0 );

            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical( "box" );
                    CreateTextInfo( "Select Axis:", "", 120, 20 );
                    GUILayout.BeginHorizontal();
                        CreateToggle( ref axisX, "X", "", 20, 20 );
                        CreateToggle( ref axisY, "Y", "", 20, 20 );
                        CreateToggle( ref axisZ, "Z", "", 20, 20 );
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.BeginVertical( "box" );
                    GUILayout.BeginHorizontal();
                        CreateObjectField( ref source, "Object 1", "", 60, 20 );
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                        CreateObjectField( ref target, "Object 2", "", 60, 20 );
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical( "box" );
                GUILayout.BeginHorizontal();
                    CreateTextField( ref distance, "Distance", "", 60, 0, 20 );
                    CreateCopyButton( "Copy", "", distance, 60, 20 );
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if ( ObjectsAreValid() )
            {
                Selection.selectionChanged += ObjectsUpdate;

                GameObject sourceObj = source as GameObject;
                GameObject targetObj = target as GameObject;

                if( axisX )
                {
                    sourceOrigin.x = sourceObj.transform.position.x;
                    targetOrigin.x = targetObj.transform.position.x;
                }

                if( axisY )
                {
                    sourceOrigin.y = sourceObj.transform.position.y;
                    targetOrigin.y = targetObj.transform.position.y;
                }

                if( axisZ )
                {
                    sourceOrigin.z = sourceObj.transform.position.z;
                    targetOrigin.z = targetObj.transform.position.z;
                }

                distance = Helper.ReplaceComma( Vector3.Distance( sourceOrigin, targetOrigin ) );
            }
            else
            {
                distance = "0.0";
            }
        }

        internal static bool ObjectsAreValid()
        {
            if ( Helper.IsValid( source ) && Helper.IsValid( target ) )
            {
                return true;
            }
            else if ( Selection.count >= 2 )
            {
                ObjectsUpdate();
                return true;
            }

            return false;
        }

        internal static void ObjectsUpdate()
        {
            if( Selection.count >= 2 )
            {
                source = Selection.gameObjects[0];
                target = Selection.gameObjects[1];
            }
        }
    }
}
