using UnityEngine;

public class ButtonScripting : MonoBehaviour
{
    [Header("Settings:")]
    public string UseText = "";

    [TextArea(15,20)]
    public string OnUseCallback = "";
}
