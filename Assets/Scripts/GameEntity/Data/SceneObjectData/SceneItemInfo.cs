using UnityEngine;

public class SceneItemInfo : BaseSceneObjectInfo
{
    public enum ItemType
    {
        NONE,
        Weapon,
        HP,
        EXP,
        Life,
        Money,
        Trap,
    }

    public ItemType Type { get; set; }
    public Vector2 TriggerOffest { get; set; }
    public Vector2 TriggerSize { get; set; }
    public int Value;
}
