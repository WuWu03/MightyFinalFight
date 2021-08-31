using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoRunToBorder : Action
{
    public DoRunToBorder(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        Rect vision = CameraMgr.Ins.GetVision();
        Vector2 pos = m_Owner.Owner.Pos;

        float leftDistance = Mathf.Abs(pos.x - vision.xMin);
        float rightDistance = Mathf.Abs(pos.x - vision.xMax);

        m_BorderPosX = leftDistance < rightDistance ? vision.xMin : vision.xMax;
        m_MoveDir = leftDistance < rightDistance ? -1 : 1;
        m_IsArrived = false;
    }

    public override BehaviorTreeState Excute()
    {
        if(!m_IsArrived)
        {
            Rect ownerBound = m_Owner.Owner.Bound;
            Vector2 size = m_Owner.Owner.GetCurrTriggerSize();
            float distance = m_MoveDir > 0 ? Mathf.Abs(ownerBound.xMax - m_BorderPosX) : Mathf.Abs(ownerBound.xMin - m_BorderPosX);
            m_IsArrived = distance <= size.x;
            m_Owner.Move(Vector2.right * m_MoveDir);
            m_Owner.OppositePlayer();
            return BehaviorTreeState.Running;
        }

        return BehaviorTreeState.Success;
    }


    public override void Reset()
    {
        base.Reset();
    }


    private bool m_IsArrived = false;
    private int m_MoveDir = 1;
    private float m_BorderPosX = 0;
    protected new BaseEnemyCtrl m_Owner = null;
}