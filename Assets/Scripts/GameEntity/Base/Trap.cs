using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;

public class Trap : BaseSceneItem
{
    public override void Init(int id, string name)
    {
        base.Init(id, name);
    }

    public override void InitInfo(BaseSceneObjectInfo info)
    {
        base.InitInfo(info);
        m_TrapData = info as TrapInfo;
        SetPos(m_TrapData.Pos);
    }

    public override void SetPos(Vector2 pos)
    {
        m_Pos = pos;
        transform.localPosition = new Vector3(pos.x, pos.y, 0);
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        SetCollider(m_TrapData.TriggerOffest, m_TrapData.TriggerSize);
        m_Collider.enabled = true;
        m_Collider.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        BaseRole target = collision.gameObject.GetComponent<BaseRole>();
        if (target == null || target.IsDropTrag) return;

        float width = m_Collider.size.x;
        float boundsLeft = target.Pos.x - 0.1f;
        float boundsRight = target.Pos.x + 0.1f;
        float selfLeft = m_Pos.x - width / 2;
        float selfRight = m_Pos.x + width / 2;

        bool isEnter = boundsLeft >= selfLeft && boundsRight <= selfRight;

        if (!isEnter) return;
        Vector2 rebirthPos = Vector2.zero;
        if (target.Pos.x < m_Pos.x)
            rebirthPos = new Vector2(m_Pos.x - width - 0.1f, target.Pos.y);
        else
            rebirthPos = new Vector2(m_Pos.x + width + 0.1f, target.Pos.y);

        target.OnDropTragMsg(new TrapInfo()
        {
            Pos = rebirthPos,
            AttackValue = m_TrapData.AttackValue,
        });
    }

    private TrapInfo m_TrapData = null;
}