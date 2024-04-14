using UnityEngine;

namespace Sandbox
{
    public class FixOldDoor
    {
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
            foreach ( var go in GameObject.FindGameObjectsWithTag( Helper.GetObjTagNameWithEnum( objectType ) ) )
            {
                GameObject obj = null;
                DoorScript doorScript = null;
                HorzDoorScript horzDoorScript = null;
                VerticalDoorScript verticalDoorScript = null;

                var position = go.transform.position;
                var angles = go.transform.eulerAngles;
                var parent = go.transform.parent;

                switch ( objectType )
                {
                    case ObjectType.SingleDoor:
                    case ObjectType.DoubleDoor:
                        doorScript = ( DoorScript )Helper.GetComponentByEnum( go, objectType );
                        if ( doorScript == null ) continue;

                        break;

                    case ObjectType.HorzDoor:
                        horzDoorScript = ( HorzDoorScript )Helper.GetComponentByEnum( go, objectType );
                        if ( horzDoorScript == null ) continue;

                        break;

                    case ObjectType.VerticalDoor:
                        verticalDoorScript = ( VerticalDoorScript )Helper.GetComponentByEnum( go, objectType );
                        if ( verticalDoorScript == null ) continue;

                        break;
                }

                obj = Helper.CreateGameObject( "", Helper.GetObjRefWithEnum( objectType ), PathType.Name );
                if ( !Helper.IsValid( obj ) ) continue;

                obj.transform.position = position;
                obj.transform.eulerAngles = angles;

                if ( parent != null ) obj.transform.parent = parent;

                switch ( objectType )
                {
                    case ObjectType.SingleDoor:
                    case ObjectType.DoubleDoor:
                        var doorScriptObj = ( DoorScript )Helper.GetComponentByEnum( obj, objectType );
                        doorScriptObj.GoldDoor = doorScript.GoldDoor;

                        break;

                    case ObjectType.HorzDoor:
                        break;

                    case ObjectType.VerticalDoor:
                        break;
                }

                Object.DestroyImmediate( go );
            }
        }
    }
}