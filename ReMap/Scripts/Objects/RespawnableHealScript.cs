using UnityEngine;

public class RespawnableHealScript : MonoBehaviour
{
    /* [ HideInInspector ] */ public Transform RespawnableHeal;

    [ Header( "Settings:" ) ]
    public float RespawnTime = 6f;
    public float HealDuration = 5f;
    [ ConditionalHide( "IsSmallHeal", true )  ] public int HealAmount = 25;
    public bool Progressive = true;

    [ HideInInspector ] public bool IsSmallHeal = false;

    private void OnDrawGizmos()
    {
        if ( RespawnableHeal != null )
        {
            IsSmallHeal = RespawnableHeal.gameObject.name == "custom_respawnable_heal_cell" || RespawnableHeal.gameObject.name == "custom_respawnable_heal_seringe";
        }
    }
}
