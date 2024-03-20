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

    public int itemType { get; set; }
    public bool canDrop { get; set; }
    public int value { get; set; }

    public static SceneItemData Create()
    {
        return ReferencePool.Acquire<SceneItemData>();
    }

    public override void Clear()
    {
        base.Clear();
        itemType = 0;
        canDrop = false;
        value = 0;
    }
}
