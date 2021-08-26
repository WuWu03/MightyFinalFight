using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoSkill : Action
{
    public DoSkill(string name, string args, object owner) : base(name, args, owner)
    {
        m_ActionOwner = base.m_Owner as BaseEnemyCtrl;

        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);
            if (m.Success)
            {
                m_SkllId = int.Parse(m.Groups[2].Value);
            }
        }
    }

    protected override void OnEnter()
    {
        m_ActionOwner.Skill(m_SkllId);
    }

    public override BehaviorTreeState Excute()
    {
        if (m_ActionOwner.IsSkillComplete(m_SkllId))
        {
            return BehaviorTreeState.Success;
        }

        return BehaviorTreeState.Running;
    }


    public override void Reset()
    {
        base.Reset();
    }

    private int m_SkllId = 0;
    private Regex m_Regex = new Regex(@"(SkillId:)(-?[0-9]+)");
    private BaseEnemyCtrl m_ActionOwner = null;
}