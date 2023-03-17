using System.Collections.Generic;
using UnityEngine;

public enum PropScriptParameters
{
    MakeInvisible,
    KvSolid3
}

public class PropScript : MonoBehaviour
{
    [Header("Settings:")]
    public bool allowMantle = true;
    public float fadeDistance = 50000;
    public int realmID = -1;
    public bool playerClip = false;
    public List<PropScriptParameters> parameters = new List<PropScriptParameters>();
    public bool playerNoClimb = false;
    public bool playerNoCollision = false;
}
