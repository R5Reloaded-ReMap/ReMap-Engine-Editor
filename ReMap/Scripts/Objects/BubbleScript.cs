using UnityEngine;

[AddComponentMenu("ReMap/Bubble Shield", 0)]
public class BubbleScript : MonoBehaviour
{
    public Color32 ShieldColor = new Color32(128, 255, 128, 255);
    public Transform Mesh;

    void OnDrawGizmos()
    {
        Material[] mymat = Mesh.GetComponent<Renderer>().sharedMaterials;
        mymat[0].SetColor("_EmissionColor", ShieldColor);
    }
}
