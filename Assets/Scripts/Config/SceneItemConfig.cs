using FrameWork;
using System;
using UnityEngine;

public class SceneItemConfig : BaseScriptableObject<SceneItemData>
{

}

[Serializable]
public class SceneItemData : BaseConfigData
{
    public enum ItemType
    {
        Weapon,
        HP,
        EXP,
        Life,
        Money,
        Trap,
    }

    public string Name;
    public string AssetName;
    public ItemType Type;
    public Vector2 TriggerSize;
    public Vector2 TriggerOffest;
    public int Value;//消耗品等恢复的数值
    public bool CanDrop;
}
