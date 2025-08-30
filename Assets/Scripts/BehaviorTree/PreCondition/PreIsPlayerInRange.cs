using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsPlayerInRange : PreCondition
{
    public PreIsPlayerInRange(int id, object owner, int priority, bool isAndCondiont, string args) : base(id, owner, priority, isAndCondiont, args)
    {
        m_Regex = new(@"(Range:)(-?[0-9]+(\.[0-9])?)");

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
        BaseRole role = owner as BaseRole;
        Vector2 playerPos = PlayerMgr.instance.player.pos;
        Vector2 ownerPos = role.pos;

        float xDistance = Mathf.Abs(playerPos.x - ownerPos.x);
        float yDistance = Mathf.Abs(playerPos.y - ownerPos.y);

        if(m_Range > 0)
        {
            return yDistance <= 0.01f && xDistance <= m_Range;
        }

        Rect ownerBound = role.bound;
        Rect playerBound = PlayerMgr.instance.player.bound;

        return yDistance <= 0.01f && xDistance <= playerBound.width / 2 + ownerBound.width / 2 + 0.01f;
    }

    private float m_Range = -1;
    private Regex m_Regex = null;
}