using System.Collections.Generic;
using UnityEngine;

public enum PropScriptOptions
{
    NoOption, // null
    PlayerClip, // ent is a invisible wall
    PlayerNoClimb, // player can't climb this ent
    MakeInvisible, // ent is invisible
    KvContentsNOGRAPPLE // player can't grapple this ent
}

public class PropScript : MonoBehaviour
{
    [Header("Settings:")]
    public bool AllowMantle = true;
    public float FadeDistance = 50000;
    public int RealmID = -1;
    public PropScriptOptions Option = PropScriptOptions.NoOption;
}
