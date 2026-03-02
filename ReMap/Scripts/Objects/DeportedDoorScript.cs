using UnityEngine;

[AddComponentMenu("ReMap/Decorated Door", 0)]
public class DeportedDoorScript : MonoBehaviour
{
    public DoorScript doorScript;
    public bool GoldDoor = false;
    public bool AppearOpen = false;
    [ TextArea ] public string Options = "";

    void OnDrawGizmosSelected()
    {
        if (doorScript == null) return;

        doorScript.GoldDoor = GoldDoor;

        doorScript.AppearOpen = AppearOpen;
        
        doorScript.Options = Options;
    }
}
