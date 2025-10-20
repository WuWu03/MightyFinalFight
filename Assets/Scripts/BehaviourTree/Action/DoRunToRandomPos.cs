using GameFrameWork.BehaviourTree;
using UnityEngine;

public class DoRunToRandomPos : Action
{
    public DoRunToRandomPos(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        m_Owner = owner as BaseEnemy;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }
    public override bool CanExcute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    protected override void OnEnter()
    {
        m_State = BehaviourTreeState.Running;

        Rect ownerBound = m_Owner.bound;
        Rect playerBound = PlayerMgr.instance.player.bound;
        Rect visionRect = CameraMgr.instance.GetVision();
        Vector2 playerPos = PlayerMgr.instance.player.pos;

        visionRect.xMin = Mathf.Max(visionRect.xMin, playerPos.x - visionRect.width / 3);
        visionRect.xMax = Mathf.Min(visionRect.xMax, playerPos.x + visionRect.width / 3);

        m_RandomPos = StageMgr.instance.GetRandomPos(visionRect);
       // m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, visionRect.xMin + size, visionRect.xMax - size);

        if(m_RandomPos.x <= playerPos.x)
        {
            m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, visionRect.xMin + ownerBound.width / 2, playerPos.x - (playerBound.width + ownerBound.width) / 2);
        }
        else
        {
            m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, playerPos.x + (playerBound.width + ownerBound.width) / 2, visionRect.xMax - ownerBound.width / 2);
        }
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        bool isArrive = Mathf.Abs(m_RandomPos.x - m_Owner.pos.x) <= 0.03f && Mathf.Abs(m_RandomPos.y - m_Owner.pos.y) <= 0.03f;

        if (isArrive)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.OppositePlayer();
            m_State = BehaviourTreeState.Success;
        }
        else
        {
            m_Owner.Move((m_RandomPos - m_Owner.pos).normalized, false);
            m_Owner.OppositePlayer();
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_State = BehaviourTreeState.None;
    }

    private Vector2 m_RandomPos = Vector2.zero;
    private BaseEnemy m_Owner = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}