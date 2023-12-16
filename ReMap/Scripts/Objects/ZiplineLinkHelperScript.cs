using UnityEngine;

[AddComponentMenu("ReMap/Zipline Link Helper", 0)]
public class ZiplineLinkHelperScript : MonoBehaviour
{
    public Transform helper;
    public Transform zipline;
    public bool origin = true;
    public bool angles = true;

    void OnDrawGizmos()
    {
        if (zipline == null) return;

        if (origin) zipline.position = helper.position;

        if (angles) zipline.eulerAngles = helper.eulerAngles;
    }
}