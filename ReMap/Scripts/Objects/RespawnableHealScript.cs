using UnityEngine;

[AddComponentMenu("ReMap/Respawnable Heal", 0)]
public class RespawnableHealScript : MonoBehaviour
{
    public Transform RespawnableHeal;
    public float RespawnTime = 6f;
    public float HealDuration = 5f;
    [ConditionalHide("IsSmallHeal", true)] public int HealAmount = 25;
    public bool Progressive = true;
    public bool IsSmallHeal = false;

    private void OnDrawGizmos()
    {
        if (RespawnableHeal != null)
        {
            IsSmallHeal = RespawnableHeal.gameObject.name == "custom_respawnable_heal_cell" || RespawnableHeal.gameObject.name == "custom_respawnable_heal_seringe";
        }
    }
}
