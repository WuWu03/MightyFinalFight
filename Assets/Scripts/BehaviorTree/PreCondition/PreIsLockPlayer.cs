using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsLockPlayer : PreCondition
{
    public PreIsLockPlayer(string name, string args, object owner) : base(name, args, owner) 
    {
        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);
            if (m.Success) m_Distance = float.Parse(m.Groups[2].Value);
        }

        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        if (m_IsLockPlayer)
        {
            return true;
        }

        float distance = Vector2.Distance(PlayerMgr.Ins.Player.Pos, m_Owner.Owner.Pos);

        if (distance <= m_Distance)
        {
            m_IsLockPlayer = true;
            m_Owner.OppositePlayer();
            m_Owner.Owner.FsmMachine.ChangeState<RoleIdle>();
        }

        return false;
    }

    private float m_Distance = 0f;
    private Regex m_Regex = new Regex(@"(Distance:)([0-9]+\.?[0-9]*)");
    private bool m_IsLockPlayer = false;
    private new BaseEnemyCtrl m_Owner = null;
}
