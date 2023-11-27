using GameFrameWork.Sound;
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
        if (m_ConsumeInfo.itemType == 2)
            AddHP();
        if (m_ConsumeInfo.itemType == 3)
            AddExp();
        if (m_ConsumeInfo.itemType == 4)
            AddExp();
        if (m_ConsumeInfo.itemType == 5)
            AddMoney();
        Release();
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        ResetRigidbody();
    }

    private void AddHP()
    {
        if (m_Owner.entityAttribute.IsFullHealth())
            AddExp();
        else
        {
            m_Owner.entityAttribute.AddHealth(m_ConsumeInfo.value);
            //UIMgr.Ins.GetPanel<MainPanel>().SetPlayerHP(m_Health, m_MaxHealth);
        }
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnEat");
    }

    private void AddExp()
    {
        PlayerMgr.instance.AddExp(m_ConsumeInfo.value);
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnEat");
    }

    private void AddLife()
    {
        PlayerMgr.instance.AddLife(m_ConsumeInfo.value);
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/GetRobot");
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
