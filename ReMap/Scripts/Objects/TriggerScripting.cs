using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TriggerScripting : MonoBehaviour
{
    [ HideInInspector ] public Transform Trigger;
    [ HideInInspector ] public Transform Helper;

    [Header("Trigger Visual Look:")]
    public bool useWireMesh = false;
    [ConditionalHide("useWireMesh", true)] public int wireMeshSides = 16;
    public Color color = Color.white;

    [Header("Settings:")]
    public bool Debug = false;
    public float Height = 50;
    public float Width = 200;
    public bool UseHelperForTP = false;

    [TextArea(15,20)]
    public string EnterCallback = "";

    [TextArea(15,20)]
    public string LeaveCallback = "";

    void OnDrawGizmos()
    {
        if ( Trigger != null )
        {
            Trigger.gameObject.SetActive(!useWireMesh);
            Trigger.localScale = new Vector3( Width, Height, Width );
            Trigger.GetComponent<Renderer>().sharedMaterial.color = new Color(color.r, color.g, color.b, 0.3f);
        }

        if ( Helper != null )
            Helper.gameObject.SetActive( UseHelperForTP );
        
        if(useWireMesh)
        {
            GizmosExtensions.DrawWireCylinder(transform.position, Width / 2, Height * 2, color, wireMeshSides);
        }
    }

}
