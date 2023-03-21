using TMPro;
using UnityEngine;

public class TextInfoPanelScript : MonoBehaviour
{
    [Header("Settings:")]
    [HideInInspector] public TextMeshProUGUI TextMeshTitle;
    [HideInInspector] public TextMeshProUGUI TextMeshDescription;
    [SerializeField] public string Title;
    [SerializeField] public string Description;
    public bool showPIN = false;
    public float Scale = 1;

    void OnDrawGizmos()
    {
        TextMeshTitle.text = Title;
        TextMeshDescription.text = Description;

        this.transform.localScale = new Vector3( Scale, Scale, Scale );
    }
}
