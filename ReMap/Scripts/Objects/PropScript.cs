using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[AddComponentMenu("ReMap/Prop Settings", 0)]
public class PropScript : MonoBehaviour
{
    public bool AllowMantle = true;
    public float FadeDistance = 50000;
    public int RealmID = -1;
    public bool ClientSide = false;
    [ TextArea ] public string Options = "";
}
