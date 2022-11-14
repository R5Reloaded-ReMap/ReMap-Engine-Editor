using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScripting : MonoBehaviour
{
    public string UseText = "";

    [TextArea(15,20)]
    public string OnUseCallback = "";
}
