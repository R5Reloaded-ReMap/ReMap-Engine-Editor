using UnityEngine;

public class DeportedDoorScript : MonoBehaviour
{
    [ HideInInspector ] public DoorScript doorScript;

    [Header("Settings:")]
    public bool GoldDoor = false;
    public bool AppearOpen = false;

    void OnDrawGizmosSelected()
    {
        if ( doorScript == null ) return;

        doorScript.GoldDoor = GoldDoor;

        doorScript.AppearOpen = AppearOpen;
    }
}
