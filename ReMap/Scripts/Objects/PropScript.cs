using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropScript : MonoBehaviour
{
    [ Header( "Settings:" ) ]
    public bool AllowMantle = true;
    public float FadeDistance = 50000;
    public int RealmID = -1;
    public bool ClientSide = false;

    [ TextArea( 14, 40 ) ]
    public string Options = "";
}
