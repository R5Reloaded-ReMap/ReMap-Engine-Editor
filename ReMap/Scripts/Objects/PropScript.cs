using System.Collections.Generic;
using UnityEngine;

public enum PropScriptParameters
{
    PlayerClip, // ent is a invisible wall
    PlayerNoClimb, // Player can't climb
    MakeInvisible, // ent is invisible

    // KV File
    // KV solid
    KvSolidNoCollision, // ent have no collision
    KvSolidBoundingBox, // ent use bounding box
    KvSolidNoFriction, // ent have no friction
    KvSolidUseVPhysics, // ent use vPhysics
    KvSolidHitboxOnly, // ent use hitbox only

    // KV contents
    KvContentsNOGRAPPLE
}

public class PropScript : MonoBehaviour
{
    [Header("Settings:")]
    public bool AllowMantle = true;
    public float FadeDistance = 50000;
    public int RealmID = -1;
    public List<PropScriptParameters> Parameters = new List<PropScriptParameters>();
    public List<string> CustomParameters = new List<string>();
}
