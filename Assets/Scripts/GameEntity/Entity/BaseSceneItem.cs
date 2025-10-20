public class BaseSceneItem : BaseGravityObject
{
    private BaseRole m_Owner;
    public BaseRole owner
    {
        get
        {
            return m_Owner;
        }
        protected set
        {
            m_Owner = value;
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
}
