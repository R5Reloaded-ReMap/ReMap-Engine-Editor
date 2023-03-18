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
    public bool allowMantle = true;
    public float fadeDistance = 50000;
    public int realmID = -1;
    public List<PropScriptParameters> parameters = new List<PropScriptParameters>();
    public List<string> customParameters = new List<string>();
}
