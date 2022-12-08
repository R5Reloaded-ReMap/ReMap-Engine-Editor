using UnityEngine;
using UnityEditor;

public class ZiplineSingleArmOffset : MonoBehaviour
{
    [Header("Dont change transforms:")]
    public Transform support;

    [Header("Arm Parameters:")]
    public float armOffset = 180;

    void OnDrawGizmos()
    {
        GameObject arm = support.transform.Find("arm").gameObject;
        arm.transform.position = new Vector3( arm.transform.position.x, support.transform.position.y + armOffset, arm.transform.position.z );
        if(armOffset < 46) armOffset = 46;
        if(armOffset > 300) armOffset = 300;
    }
}
