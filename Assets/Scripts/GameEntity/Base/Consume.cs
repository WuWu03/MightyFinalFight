using GameFrameWork.Sound;
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
        if (m_ConsumeInfo.Type == SceneItemData.ItemType.HP)
            AddHP();
        if (m_ConsumeInfo.Type == SceneItemData.ItemType.EXP)
            AddExp();
        if (m_ConsumeInfo.Type == SceneItemData.ItemType.Life)
            AddExp();
        if (m_ConsumeInfo.Type == SceneItemData.ItemType.Money)
            AddMoney();
        Release();
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        BoxCollider2D boxCollider2D = go.GetComponent<BoxCollider2D>();

        if (boxCollider2D != null)
        {
            SetCollider(boxCollider2D.offset, boxCollider2D.size);
            boxCollider2D.enabled = false;
        }

        ResetRigidbody();
    }

    private void AddHP()
    {
        if (m_Owner.Health >= m_Owner.MaxHealth)
            AddExp();
        else
            m_Owner.AddHealth(m_ConsumeInfo.Value);
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/OnEat");
    }

    private void AddExp()
    {
        PlayerMgr.Ins.AddExp(m_ConsumeInfo.Value);
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/OnEat");
    }

    private void AddLife()
    {
        PlayerMgr.Ins.AddLife(m_ConsumeInfo.Value);
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/GetRobot");
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

    private SceneItemData m_ConsumeInfo = null;
}
