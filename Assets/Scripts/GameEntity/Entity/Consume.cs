using GameFrameWork.Audio;
using GameFrameWork.UI;
using GameFrameWork.Utils;
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

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
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
            MainPanel mainPanel = UIMgr.instance.Get(UINames.MainPanel) as MainPanel;
            mainPanel.SetPlayerHP(m_Owner.entityAttribute.health, m_Owner.entityAttribute.maxHealth);
        }

        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Eat));
    }

    private void AddExp()
    {
        PlayerMgr.instance.AddExp(m_ConsumeInfo.value);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Eat));
    }

    private void AddLife()
    {
        PlayerMgr.instance.AddLife(m_ConsumeInfo.value);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Eat));
    }

    private void AddMoney()
    {
        PlayerMgr.instance.AddContinue(m_ConsumeInfo.value);
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_ConsumeInfo = null;
    }

    private SceneItemData m_ConsumeInfo = null;
}
