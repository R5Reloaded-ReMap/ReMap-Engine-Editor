using System.Collections.Generic;
using UnityEngine;

public enum PropScriptOptions
{
    NoOption, // null
    PlayerClip, // ent is a invisible wall
    NoClimb, // player can't climb this ent
    MakeInvisible, // ent is invisible
    NoGrapple, // player can't grapple this ent
    PlayerClipInvisibleNoGrappleNoClimb, // player can't grapple this ent and can't climb && ent is invisible
    PlayerClipNoGrappleNoClimb, // player can't grapple this ent and can't climb
    NoCollision // ent have no collision
}

public class PropScript : MonoBehaviour
{
    [ Header( "Settings:" ) ]
    public bool AllowMantle = true;
    public float FadeDistance = 50000;
    public int RealmID = -1;
    public bool ClientSide = false;
    public PropScriptOptions Option = PropScriptOptions.NoOption;
}
