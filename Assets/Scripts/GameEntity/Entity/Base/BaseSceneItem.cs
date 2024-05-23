using UnityEngine;

public class BaseSceneItem : BaseGravityObject
{
    public BaseRole owner
    {
        get
        {
            return m_Owner;
        }
    }

    public virtual bool canPickUp
    {
        get
        {
            return true;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
    }

    public virtual void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
    }

    protected override void OnLoadAssetComplete(GameObject go, object[] param)
    {
        base.OnLoadAssetComplete(go, param);
    }

    public override void Release()
    {
        m_Owner = null;
        SceneEntityMgr.instance.ReleaseSceneItem(this);
        base.Release();
    }

    protected BaseRole m_Owner = null;
}
