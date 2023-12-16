using UnityEngine;

[AddComponentMenu("ReMap/Vertical Door", 0)]
public class VerticalDoorScript : MonoBehaviour
{
    public Transform Door;
    public bool AppearOpen = false;

    void OnDrawGizmosSelected()
    {
        if (Door != null)
        {
            if (AppearOpen)
            {
                Door.position = this.transform.position + new Vector3(0, 128, 0);
            }
            else
            {
                Door.position = this.transform.position;
            }
        }
    }
}
