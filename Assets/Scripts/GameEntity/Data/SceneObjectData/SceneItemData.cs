using GameFrameWork;
using UnityEngine;

public class SceneItemData : BaseSceneObjectData
{
    public enum ItemType
    {
        None,
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
    public bool CanDrop { get; set; }
    public int Value { get; set; }

    public override void Clear()
    {
        base.Clear();
        Type = ItemType.None;
        TriggerOffest = Vector2.zero;
        TriggerSize = Vector2.zero;
        CanDrop = false;
        Value = 0;
    }
}
