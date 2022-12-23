using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [Header("Settings:")]
    public Color32 shieldColor = new Color32(128, 255, 128, 255);

    public Transform Mesh;

    void OnDrawGizmos()
    {
        Material[] mymat = Mesh.GetComponent<Renderer>().sharedMaterials;
        mymat[0].SetColor("_EmissionColor", shieldColor);
    }
}
