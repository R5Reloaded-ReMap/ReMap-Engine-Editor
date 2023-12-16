using UnityEngine;

[AddComponentMenu("ReMap/Speed Boost", 0)]
public class SpeedBoostScript : MonoBehaviour
{
    public Transform Boost;
    public Transform BoostBase;
    public Color32 Color = new Color32(255, 255, 255, 255);
    public float RespawnTime = 5f;
    public float Strengh = 0.35f;
    public float Duration = 3f;
    public float FadeTime = 0f;

    void OnDrawGizmos()
    {
        if (FadeTime > Duration) FadeTime = Duration;

        if (Boost != null)
        {
            Material[] boostMat = Boost.GetComponent<Renderer>().sharedMaterials;
            boostMat[0].SetColor("_EmissionColor", Color);
        }

        if (BoostBase != null)
        {
            Material[] mymat = BoostBase.GetComponent<Renderer>().sharedMaterials;
            mymat[0].SetColor("_EmissionColor", Color);
        }
    }
}