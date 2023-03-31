using UnityEngine;

public enum LootBinSkinType
{
    Default = 0,
    Blue = 1,
    Gold = 2,
    Yellow = 3
}

public class LootBinScript : MonoBehaviour
{
    public LootBinSkinType LootbinSkin = LootBinSkinType.Default;
}
