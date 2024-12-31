using UnityEngine;

[ AddComponentMenu( "ReMap/Invis Button", 0 ) ]
public class InvisButtonScript : MonoBehaviour
{
    public Transform Button;
    public Transform Destination;

    public bool Up = true;
    public string Message = "";
    public string SubMessage = "";
    public int Type = 4;
    public float Duration = 5.0f;
    public string Token = "#FS_STRING_VAR";
}