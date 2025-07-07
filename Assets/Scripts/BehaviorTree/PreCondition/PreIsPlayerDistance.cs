using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsPlayerDistance : PreCondition
{
    public PreIsPlayerDistance(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
    {
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
        Vector2 ownerPos = (m_Owner as BaseRoleCtrl).owner.pos;

        float distance = Vector2.Distance(playerPos, ownerPos);
        return distance <= m_Distance;
    }

    private float m_Distance = 0.5f;
    private Regex m_Regex = new Regex(@"(Distance:)(-?[0-9]+(\.[0-9])?)");
}