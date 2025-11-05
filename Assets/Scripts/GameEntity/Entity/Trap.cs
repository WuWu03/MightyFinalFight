using UnityEngine;

public class Trap : BaseBoundObject
{
    private TrapData m_TrapData;
    
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        BaseRole target = collision.gameObject.GetComponent<BaseRole>();

        if (target is null || target.IsDropTrap)
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

        Vector2 rebirthPos = target.pos.x < pos.x ? 
            new Vector2(pos.x - width - 0.1f, target.pos.y) : 
            new Vector2(pos.x + width + 0.1f, target.pos.y);
        DropTrapStateArg dropTrapArg = DropTrapStateArg.Create();
        dropTrapArg.rebirthPos = rebirthPos;
        dropTrapArg.attackValue = 1;
        target.DropTrapState(dropTrapArg);
    }
}