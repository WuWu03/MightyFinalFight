using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consume : BaseSceneItem
{
    public override void InitData(BaseSceneObjectData data)
    {
        base.InitData(data);
        m_ConsumeData = data as ItemData;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        owner.AddHealth(m_ConsumeData.Value);
        Release();
    }

    public override void Release()
    {
        base.Release();
        m_ConsumeData = null;
    }

    private ItemData m_ConsumeData = null;
}
