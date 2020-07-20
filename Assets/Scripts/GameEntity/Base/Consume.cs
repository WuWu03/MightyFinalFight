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
        if (m_ConsumeData.Type == ItemData.ItemType.HP)
            AddHP();
        if (m_ConsumeData.Type == ItemData.ItemType.EXP)
            AddExp();
        if (m_ConsumeData.Type == ItemData.ItemType.Life)
            AddExp();
        if (m_ConsumeData.Type == ItemData.ItemType.Money)
            AddMoney();
        Release();
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        SetCollider(m_ConsumeData.TriggerOffest, m_ConsumeData.TriggerSize);
        m_Collider.isTrigger = true;
        m_Collider.enabled = true;
        m_Rigidbody.gravityScale = 1.0f;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }
    private void AddHP()
    {
        if (m_Owner.Health >= m_Owner.MaxHealth)
            AddExp();
        else
            m_Owner.AddHealth(m_ConsumeData.Value);
    }

    private void AddExp()
    {
        PlayerMgr.Ins.AddExp(m_ConsumeData.Value);
    }

    private void AddLife()
    {
        PlayerMgr.Ins.AddLife(m_ConsumeData.Value);
    }

    private void AddMoney()
    {
        PlayerMgr.Ins.AddContinue(m_ConsumeData.Value);
    }

    public override void Release()
    {
        base.Release();
        m_ConsumeData = null;
    }

    private ItemData m_ConsumeData = null;
}
