
using UnityEngine;
using UnityEditor;

namespace Sandbox
{
    public class MoveProp
    {
        [MenuItem("ReMap/Sandbox/Move Prop", false, 100)]
        public static void MovePropInit()
        {
            MoveProps();
        }

        internal static void MoveProps()
        {
            GameObject[] array = Selection.gameObjects;
            float offsetX = 0;
            float offsetY = 0;
            float offsetY_Temp = 0;

            for ( int i = 0 ; i < array.Length ; i++ )
            {
                BoxCollider coll = array[i].GetComponent<BoxCollider>();
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
