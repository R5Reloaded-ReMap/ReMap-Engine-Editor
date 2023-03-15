using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TriggerScripting : MonoBehaviour
{
    [Header("Settings:")]
    public bool Debug = false;

    [TextArea(15,20)]
    public string EnterCallback = "";

    [TextArea(15,20)]
    public string LeaveCallback = "";

    public Transform playerTeleportationInfo;

    void OnDrawGizmos()
    {
        if ( playerTeleportationInfo != null )
        {
            EnterCallback =   "";

            EnterCallback +=  "if (IsValid(ent)) // ensure the entity is valid\n";
            EnterCallback +=  "{\n";
            EnterCallback +=  "    if (ent.IsPlayer() && ent.GetPhysics() != MOVETYPE_NOCLIP) // Noclip players are not affected by the trigger\n";
            EnterCallback +=  "    {\n";
            EnterCallback += $"         ent.SetOrigin({BuildOrigin( playerTeleportationInfo.gameObject )}) // change tp location\n";
            EnterCallback +=  "    }\n";
            EnterCallback +=  "}\n";
        }
    }

    // Idfk why I can't call Helper.BuildOrigin
    public static string BuildOrigin(GameObject go)
    {
        string x = (-go.transform.position.z).ToString("F4").Replace(",", ".");
        string y = (go.transform.position.x).ToString("F4").Replace(",", ".");
        string z = (go.transform.position.y).ToString("F4").Replace(",", ".");

        if ( x.Contains( ".0000" ) ) x = x.Replace( ".0000", "" );
        if ( y.Contains( ".0000" ) ) y = y.Replace( ".0000", "" );
        if ( z.Contains( ".0000" ) ) z = z.Replace( ".0000", "" );

        return $"< {x}, {y}, {z} >";
    }
}
