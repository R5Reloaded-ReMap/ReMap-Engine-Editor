using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScripting : MonoBehaviour
{
    [Header("Settings:")]
    public bool Debug = false;

    [TextArea(15,20)]
    public string EnterCallback = ""; 

    [TextArea(15,20)]
    public string LeaveCallback = ""; 
}
