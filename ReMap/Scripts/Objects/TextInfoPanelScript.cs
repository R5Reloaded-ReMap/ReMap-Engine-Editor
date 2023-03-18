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
    public float scale = 1;

    void OnDrawGizmosSelected()
    {
        textMeshTitle.text = title;
        textMeshDescription.text = description;
    }
}
