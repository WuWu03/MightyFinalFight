using GameFrameWork;
using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillConfigData;

public class BaseSceneItem : BaseGravityObject
{
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

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
    }

    public override void Release()
    {
        m_Owner = null;
        SceneEntityMgr.instance.ReleaseSceneItem(this);
        base.Release();
    }

    protected BaseRole m_Owner = null;
}
