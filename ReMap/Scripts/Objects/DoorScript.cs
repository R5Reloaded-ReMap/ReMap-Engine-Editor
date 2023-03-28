using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [ HideInInspector ] public Transform DoorLeft;
    [ HideInInspector ] public Transform DoorRight;

    [Header("Settings:")]
    public bool GoldDoor = false;
}
