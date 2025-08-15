using GameFrameWork.BehaviourTree;
using UnityEngine;

public class DoRunAwayPlayer : Action
{
    public DoRunAwayPlayer(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
    {
        m_Owner = base.m_Owner as BaseEnemy;
    }

    public override bool CanExcute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        m_State = BehaviourTreeState.Running;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        float playerDir = PlayerMgr.instance.player.dir;
        float ownerDir = m_Owner.dir;

        if (playerDir == ownerDir)
        {
            m_State = BehaviourTreeState.Success;
            return;
        }

        Vector2 playerPos = PlayerMgr.instance.player.pos;
        Vector2 ownerPos = m_Owner.pos;

        if (playerPos.x > ownerPos.x)
        {
            if (playerDir != -1)
            {
                m_State = BehaviourTreeState.Success;
                return;
            }
        }
        else
        {
            if (playerDir != 1)
            {
                m_State = BehaviourTreeState.Success;
                return;
            }
        }

        if (Vector2.Distance(playerPos, ownerPos) < m_Owner.bound.width * 3)
        {
            m_Owner.Move((playerPos - ownerPos).normalized, false);
            m_Owner.OppositePlayer();
            return;
        }

        m_State = BehaviourTreeState.Success;
    }

    protected override void OnReset()
    {
        base.OnReset();
    }

    private BehaviourTreeState m_State = BehaviourTreeState.None;
    private new BaseEnemy m_Owner = null;
}