using UnityEngine;

public class Trap : BaseBoundObject
{
    public override void Init(int id, string name)
    {
        base.Init(id, name);
    }

    public override void SetData(BaseSceneObjectData info)
    {
        base.SetData(info);
        m_TrapData = info as TrapData;
    }

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
        SetCollider(m_TrapData.triggerOffest, m_TrapData.triggerSize);
        boxCollider2D.enabled = true;
        boxCollider2D.isTrigger = true;
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        BaseRole target = collision.gameObject.GetComponent<BaseRole>();

        if (target == null || target.isDropTrag)
        {
            return;
        }

        float width = boxCollider2D.size.x;
        float boundsLeft = target.pos.x - 0.1f;
        float boundsRight = target.pos.x + 0.1f;
        float selfLeft = pos.x - width / 2;
        float selfRight = pos.x + width / 2;

        bool isEnter = boundsLeft >= selfLeft && boundsRight <= selfRight;

        if (!isEnter)
        {
            return;
        }

        Vector2 rebirthPos = Vector2.zero;

        if (target.pos.x < pos.x)
        {
            rebirthPos = new Vector2(pos.x - width - 0.1f, target.pos.y);
        }
        else
        {
            rebirthPos = new Vector2(pos.x + width + 0.1f, target.pos.y);
        }

        DropTrapStateData dropTrapData = DropTrapStateData.Create();
        dropTrapData.rebirthPos = rebirthPos;
        dropTrapData.attackValue = 1;

        target.OnDropTragMsg(dropTrapData);
    }

    private TrapData m_TrapData = null;
}