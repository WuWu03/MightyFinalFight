using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoRunToBorder : Action
{
    public DoRunToBorder(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
        m_ActionOwner = base.m_Owner as BaseEnemyCtrl;
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
        m_State = BehaviourTreeState.Running;
        Rect vision = CameraMgr.instance.GetVision();
        Vector2 pos = m_ActionOwner.owner.pos;

        float leftDistance = Mathf.Abs(pos.x - vision.xMin);
        float rightDistance = Mathf.Abs(pos.x - vision.xMax);

        m_BorderPosX = leftDistance < rightDistance ? vision.xMin : vision.xMax;
        m_MoveDir = leftDistance < rightDistance ? -1 : 1;
        m_ActionOwner.owner.ChangeDefaultState();
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        Rect ownerBound = m_ActionOwner.owner.bound;
        Vector2 size = m_ActionOwner.owner.GetCurrTriggerSize();
        float distance = m_MoveDir > 0 ? Mathf.Abs(ownerBound.xMax - m_BorderPosX) : Mathf.Abs(ownerBound.xMin - m_BorderPosX);
        bool isArrive = distance <= size.x;

        if (isArrive)
        {
            m_State = BehaviourTreeState.Success;
        }
        else
        {
            m_ActionOwner.Move(Vector2.right * m_MoveDir);
            m_ActionOwner.OppositePlayer();
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
    private BaseEnemyCtrl m_ActionOwner = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}