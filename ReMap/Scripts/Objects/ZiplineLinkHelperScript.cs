using UnityEngine;

public class ZiplineLinkHelperScript : MonoBehaviour
{
    [Header("Unity Settings:")]
    [ HideInInspector ] public Transform helper;
    public Transform zipline;
    public bool origin = true;
    public bool angles = true;

    void OnDrawGizmos()
    {
        if ( zipline == null ) return;

        if ( origin ) zipline.position = helper.position;
        
        if ( angles ) zipline.eulerAngles = helper.eulerAngles;
    }
}