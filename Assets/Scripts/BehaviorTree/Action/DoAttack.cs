using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoAttack : Action
{
    public DoAttack(string name, string args, object owner) : base(name, args, owner) 
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;

        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);
            if (m.Success)
            {
                m_AttackCount = int.Parse(m.Groups[2].Value);
                m_IsRandomAttckCount = m_AttackCount == 0;
            }
        }
    }

    protected override void OnEnter()
    {
        if (m_IsRandomAttckCount)
        {
            m_AttackCount = Random.Range(1, 9);
        }

        m_CurrAttackCount = 0;
        m_AttackTimer = -1;
    }

    public override BehaviorTreeState Excute()
    {
        if (m_AttackTimer > 0 && Time.time - m_AttackTimer < 0.5f)
        {
            return BehaviorTreeState.Running;
        }

        m_Owner.Attack(Vector2.zero);
        m_Owner.OppositePlayer();
        m_AttackTimer = -1;

        if (m_Owner.Owner.IsPlayComplete())
        {
            m_AttackTimer = Time.time;
            m_CurrAttackCount++;
            if (m_CurrAttackCount >= m_AttackCount)
            {
                return BehaviorTreeState.Success;
            }
        }

        return BehaviorTreeState.Running;
    }


    public override void Reset()
    {
        base.Reset();
        m_CurrAttackCount = 0;
        m_AttackTimer = -1;
    }

    protected int m_CurrAttackCount = 0;
    protected int m_AttackCount = 0;
    private float m_AttackTimer = -1f;
    private Regex m_Regex = new Regex(@"(AttackTime:)(-?[0-9]+)");

    protected new BaseEnemyCtrl m_Owner = null;
    protected bool m_IsRandomAttckCount = false;

}
