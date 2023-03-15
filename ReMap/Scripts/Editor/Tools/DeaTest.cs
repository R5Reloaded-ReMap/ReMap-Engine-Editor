using UnityEngine;
using UnityEditor;

public class DeaTest
{

    [MenuItem("ReMap/Tools/Dea Test/Move Prop", false, 100)]
    public static void Init()
    {
        //DeaTest window = (DeaTest)EditorWindow.GetWindow(typeof(DeaTest), false, "Dea Test");
        //window.Show();
        //window.minSize = new Vector2(375, 140);
        //window.maxSize = new Vector2(375, 140);
        MoveProps();
    }

    public static void MoveProps()
    {
        GameObject[] array = Selection.gameObjects;
        float offsetX = 0;
        float offsetY = 0;

        for ( int i = 0 ; i < array.Length ; i++ )
        {
            BoxCollider coll = array[i].GetComponent<BoxCollider>();
            if ( coll == null ) continue;

            array[i].transform.position = new Vector3( offsetX, offsetY, 0 );

            offsetX = offsetX + coll.size.x + 200;

            if ( offsetX > 6000 )
            {
                offsetX = 0;
                offsetY += 1000;
            }
        }
    }
}