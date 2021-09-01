using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsPlayerInRange : PreCondition
{
    public PreIsPlayerInRange(string name, string args, object owner) : base(name, args, owner)
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
        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;
        Vector2 ownerPos = (m_Owner as BaseRoleCtrl).Owner.Pos;

        float xDistance = Mathf.Abs(playerPos.x - ownerPos.x);
        float yDistance = Mathf.Abs(playerPos.y - ownerPos.y);

        return yDistance <= 0.05f && xDistance <= m_Range;
    }

    private float m_Range = 0.5f;
    private Regex m_Regex = new Regex(@"(Range:)([0-9]+\.?[0-9]+)");
}


