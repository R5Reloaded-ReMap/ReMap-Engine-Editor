
using UnityEngine;
using UnityEditor;

namespace Sandbox
{
    public class FixOldDoor
    {
        [MenuItem("ReMap/Sandbox/Fix Old Doors", false, 100)]
        public static void FixOldDoorsInit()
        {
            FixOldDoors();
        }

        internal static void FixOldDoors()
        {
            ReplaceDoorsByType( ObjectType.SingleDoor );
            ReplaceDoorsByType( ObjectType.DoubleDoor );
            ReplaceDoorsByType( ObjectType.HorzDoor );
            ReplaceDoorsByType( ObjectType.VerticalDoor );
        }

        internal static void ReplaceDoorsByType( ObjectType objectType )
        {
            foreach ( GameObject go in GameObject.FindGameObjectsWithTag( Helper.GetObjTagNameWithEnum( objectType ) ) )
            {
                GameObject obj = null;
                DoorScript doorScript = null;
                HorzDoorScript horzDoorScript = null;
                VerticalDoorScript verticalDoorScript = null;

                Vector3 position = go.transform.position;
                Vector3 angles = go.transform.eulerAngles;
                Transform parent = go.transform.parent;

                switch ( objectType )
                {
                    case ObjectType.SingleDoor:
                    case ObjectType.DoubleDoor:
                        doorScript = ( DoorScript ) Helper.GetComponentByEnum( go, objectType );
                        if ( doorScript == null ) continue;

                        break;

                    case ObjectType.HorzDoor:
                        horzDoorScript = ( HorzDoorScript ) Helper.GetComponentByEnum( go, objectType );
                        if ( horzDoorScript == null ) continue;

                        break;

                    case ObjectType.VerticalDoor:
                        verticalDoorScript = ( VerticalDoorScript ) Helper.GetComponentByEnum( go, objectType );
                        if ( verticalDoorScript == null ) continue;

                    break;
                }

                UnityEngine.Object loadedPrefabResource = UnityInfo.FindPrefabFromName( Helper.GetObjRefWithEnum( objectType ) );
                if ( loadedPrefabResource != null )
                {
                    obj = PrefabUtility.InstantiatePrefab( loadedPrefabResource as GameObject ) as GameObject;
                }
                else continue;

                obj.transform.position = position;
                obj.transform.eulerAngles = angles;

                if ( parent != null ) obj.transform.parent = parent;

                switch ( objectType )
                {
                    case ObjectType.SingleDoor:
                    case ObjectType.DoubleDoor:
                        DoorScript doorScriptObj = ( DoorScript ) Helper.GetComponentByEnum( obj, objectType );
                        doorScriptObj.GoldDoor = doorScript.GoldDoor;

                        break;

                    case ObjectType.HorzDoor:
                        break;

                    case ObjectType.VerticalDoor:
                    break;
                }

                GameObject.DestroyImmediate( go );
            }
        }
    }
}
