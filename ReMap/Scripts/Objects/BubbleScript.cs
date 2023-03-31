using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [Header("Settings:")]
    public Color32 ShieldColor = new Color32(128, 255, 128, 255);

    [HideInInspector] public Transform Mesh;

    void OnDrawGizmos()
    {
        Material[] mymat = Mesh.GetComponent<Renderer>().sharedMaterials;
        mymat[0].SetColor("_EmissionColor", ShieldColor);
    }
}
