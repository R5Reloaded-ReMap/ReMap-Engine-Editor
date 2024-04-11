using UnityEngine;

[AddComponentMenu("ReMap/Button", 0)]
public class ButtonScripting : MonoBehaviour
{
    public string UseText = "";
    [TextArea] public string OnUseCallback = "";
}
