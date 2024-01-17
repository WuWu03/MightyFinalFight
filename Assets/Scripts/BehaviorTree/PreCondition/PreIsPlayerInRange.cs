using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsPlayerInRange : PreCondition
{
    public PreIsPlayerInRange(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);
            if (m.Success)
            {
                m_Range = float.Parse(m.Groups[2].Value);
            }
        }
    }

    protected override bool OnCheckPreCondition()
    {
        Vector2 playerPos = PlayerMgr.instance.player.pos;
        Vector2 ownerPos = (m_Owner as BaseRoleCtrl).owner.pos;

        float xDistance = Mathf.Abs(playerPos.x - ownerPos.x);
        float yDistance = Mathf.Abs(playerPos.y - ownerPos.y);

        if(m_Range > 0)
        {
            return yDistance <= 0.01f && xDistance <= m_Range;
        }

        Rect ownerBound = (m_Owner as BaseRoleCtrl).owner.bound;
        Rect playerBound = PlayerMgr.instance.player.bound;

        return yDistance <= 0.01f && xDistance <= playerBound.width / 2 + ownerBound.width / 2 + 0.01f;
    }

    private float m_Range = -1;
    private Regex m_Regex = new Regex(@"(Range:)(-?[0-9]+(\.[0-9])?)");
}


