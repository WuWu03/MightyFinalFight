using GameFrameWork.Audio;
using GameFrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consume : BaseSceneItem
{
    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_ConsumeInfo = data as SceneItemData;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);

        if (m_ConsumeInfo.itemType == 2) AddHP();
        else if (m_ConsumeInfo.itemType == 3) AddExp();
        else if (m_ConsumeInfo.itemType == 4) AddLife();
        else if (m_ConsumeInfo.itemType == 5) AddMoney();

        Release();
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        ResetRigidbody();
        
        BoxCollider2D bc2 = go.GetComponent<BoxCollider2D>();
        m_BoxCollider2D.size = bc2.size;
        m_BoxCollider2D.offset = bc2.offset;
    }

    private void AddHP()
    {
        if (m_Owner.entityAttribute.IsFullHealth())
        {
            AddExp();
        }
        else
        {
            m_Owner.entityAttribute.AddHealth(m_ConsumeInfo.value);
            UIMgr.instance.Get<MainPanel>().SetPlayerHP(m_Owner.entityAttribute.health, m_Owner.entityAttribute.maxHealth);
        }
        AudioMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnEat");
    }

    private void AddExp()
    {
        PlayerMgr.instance.AddExp(m_ConsumeInfo.value);
        AudioMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnEat");
    }

    private void AddLife()
    {
        PlayerMgr.instance.AddLife(m_ConsumeInfo.value);
        AudioMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/GetRobot");
    }

    private void AddMoney()
    {
        PlayerMgr.instance.AddContinue(m_ConsumeInfo.value);
    }

    public override void Release()
    {
        base.Release();
        m_ConsumeInfo = null;
    }

    private SceneItemData m_ConsumeInfo = null;
}
