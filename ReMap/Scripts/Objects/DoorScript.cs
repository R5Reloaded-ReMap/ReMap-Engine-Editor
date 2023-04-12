using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [ HideInInspector ] public Transform DoorLeft;
    [ HideInInspector ] public Transform DoorRight;

    [Header("Settings:")]
    public bool GoldDoor = false;
    public bool AppearOpen = false;

    void OnDrawGizmosSelected()
    {
        if ( DoorLeft != null )
        {
            if ( AppearOpen )
            {
                DoorLeft.eulerAngles = this.transform.eulerAngles + new Vector3( 0, 90, 0 );
            }
            else
            {
                DoorLeft.eulerAngles = this.transform.eulerAngles + new Vector3( 0, 180, 0 );
            }
        }

        if ( DoorRight != null )
        {
            if ( AppearOpen )
            {
                DoorRight.eulerAngles = this.transform.eulerAngles + new Vector3( 0, 90, 0 );
            }
            else
            {
                DoorRight.eulerAngles = this.transform.eulerAngles;
            }
        }
    }
}
