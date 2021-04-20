using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsPlayerDistance : PreCondition
{
    public PreIsPlayerDistance(string name, string args, object owner) : base(name, args, owner) 
    {
        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);
            if (m.Success) m_Distance = float.Parse(m.Groups[2].Value);
        }
    }

    protected override bool OnCheckPreCondition()
    {
        float distance = Vector2.Distance(PlayerMgr.Ins.Player.Pos, (m_Owner as BaseRoleCtrl).Owner.Pos);
        if(m_Distance < 0)
        {
            return distance >= Mathf.Abs(m_Distance);
        }

        return distance <= m_Distance;
    }

    private float m_Distance = 0.5f;
    private Regex m_Regex = new Regex(@"(Distance:)(-?[0-9]+\.?[0-9]+)");
}

   
