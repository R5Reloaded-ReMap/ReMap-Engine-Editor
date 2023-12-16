using TMPro;
using UnityEngine;

[AddComponentMenu("ReMap/Text Info Panel", 0)]
public class TextInfoPanelScript : MonoBehaviour
{
    public TextMeshProUGUI TextMeshTitle;
    public TextMeshProUGUI TextMeshDescription;
    public Transform Panel;
    public Transform Pin;
    [SerializeField] public string Title;
    [SerializeField] public string Description;
    public bool showPIN = true;
    public float Scale = 1;

    void OnDrawGizmos()
    {
        TextMeshTitle.text = Title;
        TextMeshDescription.text = Description;

        if (Panel != null) Panel.localScale = new Vector3(Scale, Scale, Scale);

        if (Pin != null) Pin.gameObject.SetActive(showPIN);
    }
}
