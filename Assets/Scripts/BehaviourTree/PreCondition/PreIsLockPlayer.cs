using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsLockPlayer : PreCondition
{
    public PreIsLockPlayer(int id, object owner, int priority, bool isAndCondiont, string args) : base(id, owner, priority, isAndCondiont, args)    
    {
        m_Regex = new(@"(Distance:)(-?[0-9]+(\.[0-9])?)");
        m_Owner = owner as BaseEnemy;

        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);

            if (m.Success)
            {
                m_Distance = float.Parse(m.Groups[2].Value);
            }
        }
    }

    protected override bool OnCheckPreCondition()
    {
        if (m_IsLockPlayer)
        {
            return true;
        }

        float distance = Vector2.Distance(PlayerMgr.instance.player.pos, m_Owner.pos);
        if (distance <= m_Distance)
        {
            m_IsLockPlayer = true;
            m_Owner.ChangeState<RoleIdle>();
            m_Owner.OppositePlayer();
            return true;
        }

        return false;
    }

    private float m_Distance = 0f;
    private Regex m_Regex = null;
    private bool m_IsLockPlayer = false;
    private BaseEnemy m_Owner = null;
}