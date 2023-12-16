using UnityEngine;

[AddComponentMenu("ReMap/Door", 0)]
public class DoorScript : MonoBehaviour
{
    public Transform DoorLeft;
    public Transform DoorRight;
    public bool GoldDoor = false;
    public bool AppearOpen = false;

    void OnDrawGizmosSelected()
    {
        if (DoorLeft != null)
        {
            if (AppearOpen)
            {
                DoorLeft.localEulerAngles = new Vector3(0, 90, 0);
            }
            else
            {
                DoorLeft.localEulerAngles = new Vector3(0, 180, 0);
            }
        }

        if (DoorRight != null)
        {
            if (AppearOpen)
            {
                DoorRight.localEulerAngles = new Vector3(0, 90, 0);
            }
            else
            {
                DoorRight.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }
}
