using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsLockPlayer : PreCondition
{
    public PreIsLockPlayer(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)    
    {
        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);

            if (m.Success)
            {
                m_Distance = float.Parse(m.Groups[2].Value);
            }
        }

        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        if (m_IsLockPlayer)
        {
            return true;
        }

        float distance = Vector2.Distance(PlayerMgr.instance.player.pos, m_Owner.owner.pos);
        if (distance <= m_Distance)
        {
            m_IsLockPlayer = true;
            m_Owner.owner.ChangeState<RoleIdle>();
            m_Owner.OppositePlayer();
            return true;
        }

        return false;
    }

    private float m_Distance = 0f;
    private Regex m_Regex = new Regex(@"(Distance:)(-?[0-9]+(\.[0-9])?)");
    private bool m_IsLockPlayer = false;
    private new BaseEnemyCtrl m_Owner = null;
}