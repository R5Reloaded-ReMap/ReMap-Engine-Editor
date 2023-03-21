using TMPro;
using UnityEngine;

public class TextInfoPanelScript : MonoBehaviour
{
    [Header("Settings:")]
    [HideInInspector] public TextMeshProUGUI textMeshTitle;
    [HideInInspector] public TextMeshProUGUI textMeshDescription;
    [SerializeField] public string title;
    [SerializeField] public string description;
    public bool showPIN = false;
    public float Scale = 1;

    void OnDrawGizmos()
    {
        textMeshTitle.text = title;
        textMeshDescription.text = description;

        this.transform.localScale = new Vector3( Scale, Scale, Scale );
    }
}
