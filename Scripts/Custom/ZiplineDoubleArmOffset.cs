using UnityEngine;
using UnityEditor;

public class ZiplineDoubleArmOffset : MonoBehaviour
{
    [Header("Dont change transforms:")]
    public Transform support_start;
    public Transform support_end;

    [Header("Arm Parameters:")]
    public float armOffset_start = 160;
    public float armOffset_end = 160;

    void OnDrawGizmos()
    {
        GameObject arm_start = support_start.transform.Find("arm").gameObject;
        arm_start.transform.position = new Vector3( arm_start.transform.position.x, support_start.transform.position.y + armOffset_start, arm_start.transform.position.z );
        if(armOffset_start < 46) armOffset_start = 46;
        if(armOffset_start > 300) armOffset_start = 300;

        GameObject arm_end = support_end.transform.Find("arm").gameObject;
        arm_end.transform.position = new Vector3( arm_end.transform.position.x, support_end.transform.position.y + armOffset_end, arm_end.transform.position.z );
        if(armOffset_end < 46) armOffset_end = 46;
        if(armOffset_end > 300) armOffset_end = 300;
    }
}
