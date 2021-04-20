using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoBack : Action
{
    public DoBack(string name, string args, object owner) : base(name, args, owner)
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

    protected override void OnEnter()
    {
        m_TargetPos = Vector2.zero;
        m_TargetPos = m_Owner.Owner.Pos;
        m_TargetPos.x += m_BackDistance * -m_Owner.Owner.Dir;
        Rect visionRect = CameraMgr.Ins.GetVision();
        m_TargetPos.x = Mathf.Clamp(m_TargetPos.x, visionRect.xMin + m_Owner.Owner.Bound.width, visionRect.xMax - m_Owner.Owner.Bound.width);
        m_IsArravied = false;
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.OppositePlayer();
            return BehaviorTreeState.Success;
        }

        Vector2 enemyPos = m_Owner.Owner.Pos;

        m_IsArravied = Mathf.Abs(m_TargetPos.x - enemyPos.x) <= 0.01f && Mathf.Abs(m_TargetPos.y - enemyPos.y) <= 0.01f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_TargetPos - enemyPos).normalized, false);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_TargetPos = Vector2.zero;
        m_IsArravied = false;
    }

    private bool m_IsArravied = false;
    private Vector2 m_TargetPos = Vector2.zero;
    private float m_BackDistance = 0;
    private Regex m_Regex = new Regex(@"(BackDistance:)(-?[0-9]+\.?[0-9]+)");
    private new BaseEnemyCtrl m_Owner = null;
}
