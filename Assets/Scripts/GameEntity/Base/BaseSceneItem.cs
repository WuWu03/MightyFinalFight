using FrameWork;
using FrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneItem : BaseGravityObject
{
    public virtual bool CanPickUp
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

    public override void SetPos(Vector2 pos)
    {
        base.SetPos(pos);
        transform.localPosition = new Vector3(pos.x, pos.y, Bound.yMin);
    }

    public virtual void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
    }

    public override void Release()
    {
        base.Release();
        m_Owner = null;
    }

    protected BaseRole m_Owner = null;
}
