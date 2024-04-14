using UnityEditor;
using UnityEngine;

namespace Sandbox
{
    public class MoveProp
    {
        public static void MovePropInit()
        {
            MoveProps();
        }

        internal static void MoveProps()
        {
            var array = Selection.gameObjects;
            float offsetX = 0;
            float offsetY = 0;
            float offsetY_Temp = 0;

            for ( int i = 0; i < array.Length; i++ )
            {
                var coll = array[i].GetComponent< BoxCollider >();
                if ( coll == null ) continue;

                array[i].transform.position = new Vector3( offsetX, offsetY, 0 );

                offsetX = offsetX + coll.size.x + 200;

                if ( offsetY_Temp < coll.size.y ) offsetY_Temp = coll.size.y + 200;

                if ( offsetX > 16000 )
                {
                    offsetX = 0;
                    offsetY += offsetY_Temp;
                    offsetY_Temp = 0;
                }
            }
        }
    }
}