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

    public ItemType itemType { get; set; }
    public bool canDrop { get; set; }
    public int value { get; set; }

    public override void Clear()
    {
        base.Clear();
        itemType = ItemType.None;
        canDrop = false;
        value = 0;
    }
}
