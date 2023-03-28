using TMPro;
using UnityEngine;

public class TextInfoPanelScript : MonoBehaviour
{
    [Header("Settings:")]
    [HideInInspector] public TextMeshProUGUI TextMeshTitle;
    [HideInInspector] public TextMeshProUGUI TextMeshDescription;
    [HideInInspector] public Transform Panel;
    [HideInInspector] public Transform Pin;
    [SerializeField] public string Title;
    [SerializeField] public string Description;
    public bool showPIN = true;
    public float Scale = 1;

    void OnDrawGizmos()
    {
        TextMeshTitle.text = Title;
        TextMeshDescription.text = Description;

        if ( Panel != null ) Panel.localScale = new Vector3( Scale, Scale, Scale );

        if ( Pin != null ) Pin.gameObject.SetActive( showPIN );
    }
}
