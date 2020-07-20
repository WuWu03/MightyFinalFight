using UnityEngine;

public class ItemData : BaseSceneObjectData
{
    public enum ItemType
    {
        Weapon,
        HP,
        EXP,
        Life,
        Money,
    }

    public ItemType Type { get; set; }
    public Vector2 TriggerOffest { get; set; }
    public Vector2 TriggerSize { get; set; }
    public int Value;
}
