using GameFrameWork;
using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        m_DBTrigger = go.GetComponent<DBTrigger>();
    }

    protected void SetTrigger(string animName)
    {
        if (m_DBTrigger == null)
        {
            return;
        }

        TriggerData triggerData = m_DBTrigger.GetTriggerData(animName);

        if (triggerData != null)
        {
            SetCollider(triggerData.Offest, triggerData.Size);
        }
    }

    public override void Release()
    {
        m_Owner = null;
        base.Release();
    }

    protected DBTrigger m_DBTrigger = null;
    protected BaseRole m_Owner = null;
}
