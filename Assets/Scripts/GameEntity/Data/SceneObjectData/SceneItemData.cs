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
    public bool CanDrop { get; set; }
    public int Value { get; set; }

    public override void Clear()
    {
        base.Clear();
        Type = ItemType.None;
        CanDrop = false;
        Value = 0;
    }
}
