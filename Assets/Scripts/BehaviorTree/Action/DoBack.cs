using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoBack : Action
{
    public DoBack(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);
            if (m.Success)
            {
                m_BackDistance = float.Parse(m.Groups[2].Value);
            }
        }
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    protected override void OnEnter()
    {
        m_TargetPos = Vector2.zero;
        m_TargetPos = m_Owner.owner.pos;
        m_TargetPos.x += m_BackDistance * -m_Owner.owner.dir;
        Rect visionRect = CameraMgr.instance.GetVision();
        m_TargetPos.x = Mathf.Clamp(m_TargetPos.x, visionRect.xMin + m_Owner.owner.bound.width, visionRect.xMax - m_Owner.owner.bound.width);

        m_State = BehaviourTreeState.Running;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        Vector2 enemyPos = m_Owner.owner.pos;
        bool isArrived = Mathf.Abs(m_TargetPos.x - enemyPos.x) <= 0.01f && Mathf.Abs(m_TargetPos.y - enemyPos.y) <= 0.01f;

        if(isArrived)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.OppositePlayer();
            m_State = BehaviourTreeState.Success;
        }
        else
        {
            m_Owner.Move((m_TargetPos - enemyPos).normalized, false);
            m_Owner.OppositePlayer();
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_TargetPos = Vector2.zero;
    }

    private Vector2 m_TargetPos = Vector2.zero;
    private float m_BackDistance = 0;
    private Regex m_Regex = new Regex(@"(BackDistance:)(-?[0-9]+\.?[0-9]+)");
    private new BaseEnemyCtrl m_Owner = null;

    private BehaviourTreeState m_State = BehaviourTreeState.None;
}
