using GameFrameWork.BehaviourTree;
using UnityEngine;

public class DoRunToBorder : Action
{
    public DoRunToBorder(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        m_Owner = owner as BaseEnemy;
    }

    public override bool CanExecute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    protected override void OnEnter()
    {
        m_State = BehaviourTreeState.Running;
        Rect vision = CameraMgr.instance.GetVision();
        Vector2 pos = m_Owner.pos;

        float leftDistance = Mathf.Abs(pos.x - vision.xMin);
        float rightDistance = Mathf.Abs(pos.x - vision.xMax);

        m_BorderPosX = leftDistance < rightDistance ? vision.xMin : vision.xMax;
        m_MoveDir = leftDistance < rightDistance ? -1 : 1;
        m_Owner.ChangeDefaultState();
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        Rect ownerBound = m_Owner.bound;
        float distance = m_MoveDir > 0 ? Mathf.Abs(ownerBound.xMax - m_BorderPosX) : Mathf.Abs(ownerBound.xMin - m_BorderPosX);
        bool isArrive = distance <= ownerBound.width;

        if (isArrive)
        {
            m_State = BehaviourTreeState.Success;
        }
        else
        {
            m_Owner.Move(Vector2.right * m_MoveDir);
            m_Owner.OppositePlayer();
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_State = BehaviourTreeState.None;
        m_MoveDir = 1;
        m_BorderPosX = 0;
    }

    private int m_MoveDir = 1;
    private float m_BorderPosX = 0;
    private BaseEnemy m_Owner = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}