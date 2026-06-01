using WuWuFramework.Utils;
using UnityEngine;

public class Consume : BaseSceneItem
{
    private SceneItemData m_ConsumeInfo;
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
        boxCollider2D.size = bc2.size;
        boxCollider2D.offset = bc2.offset;
    }

    private void AddHP()
    {
        if (owner.entityAttribute.IsFullHealth())
        {
            AddExp();
        }
        else
        {
            owner.entityAttribute.AddHealth(m_ConsumeInfo.value);
            GameEntry.uiMgr.Get<MainView>().presenter.SetPlayerHP(owner.entityAttribute.health, owner.entityAttribute.maxHealth);
        }

        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Eat));
    }

    private void AddExp()
    {
        PlayerMgr.instance.AddExp(m_ConsumeInfo.value);
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Eat));
    }

    private void AddLife()
    {
        PlayerMgr.instance.AddLife(m_ConsumeInfo.value);
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Eat));
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
}
