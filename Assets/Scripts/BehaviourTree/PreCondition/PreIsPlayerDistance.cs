using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsPlayerDistance : PreCondition
{
    public PreIsPlayerDistance(int id, object owner, int priority, bool isAndCondiont, string args) : base(id, owner, priority, isAndCondiont, args)
    {
        m_Regex = new(@"(Distance:)(-?[0-9]+(\.[0-9])?)");

        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);
            if (m.Success)
            {
                m_Distance = Mathf.Abs(float.Parse(m.Groups[2].Value));
            }
        }
    }

    protected override bool OnCheckPreCondition()
    {
        Vector2 playerPos = PlayerMgr.instance.player.pos;
        Vector2 ownerPos = (owner as BaseRole).pos;

        float distance = Vector2.Distance(playerPos, ownerPos);
        return distance <= m_Distance;
    }

    private float m_Distance = 0.5f;
    private Regex m_Regex = null;
}