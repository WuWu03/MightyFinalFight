public class BaseSceneItem : BaseGravityObject
{
    public BaseRole owner
    {
        get
        {
            return m_Owner;
        }
    }

    public virtual bool canReleaseInSceneChange
    {
        get
        {
            return true;
        }
    }

    public virtual bool canPickUp
    {
        get
        {
            return true;
        }
    }

    public virtual void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_Owner = null;
        SceneEntityMgr.instance.ReleaseSceneItem(this);
    }

    protected BaseRole m_Owner = null;
}
