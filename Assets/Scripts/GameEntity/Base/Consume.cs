using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consume : BaseSceneItem
{
    public override void InitInfo(BaseSceneObjectInfo data)
    {
        base.InitInfo(data);
        m_ConsumeInfo = data as SceneItemInfo;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        if (m_ConsumeInfo.Type == SceneItemInfo.ItemType.HP)
            AddHP();
        if (m_ConsumeInfo.Type == SceneItemInfo.ItemType.EXP)
            AddExp();
        if (m_ConsumeInfo.Type == SceneItemInfo.ItemType.Life)
            AddExp();
        if (m_ConsumeInfo.Type == SceneItemInfo.ItemType.Money)
            AddMoney();
        Release();
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        SetCollider(m_ConsumeInfo.TriggerOffest, m_ConsumeInfo.TriggerSize);
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
            m_Owner.AddHealth(m_ConsumeInfo.Value);
    }

    private void AddExp()
    {
        PlayerMgr.Ins.AddExp(m_ConsumeInfo.Value);
    }

    private void AddLife()
    {
        PlayerMgr.Ins.AddLife(m_ConsumeInfo.Value);
    }

    private void AddMoney()
    {
        PlayerMgr.Ins.AddContinue(m_ConsumeInfo.Value);
    }

    public override void Release()
    {
        base.Release();
        m_ConsumeInfo = null;
    }

    private SceneItemInfo m_ConsumeInfo = null;
}
