using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoAttack : Action
{
    public DoAttack(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        m_Owner = owner as BaseEnemy;
        m_Regex = new(@"(AttackTime:)(-?[0-9]+)");

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

    public override bool CanExcute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    protected override void OnEnter()
    {
        m_State = BehaviourTreeState.Running;

        if (m_IsRandomAttckCount)
        {
            m_AttackCount = Random.Range(1, 9);
        }

        m_CurrAttackCount = 0;
        m_AttackTimer = -1f;
        m_IsAttacking = false;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!m_IsAttacking)
        {
            m_Owner.OppositePlayer();
            m_Owner.Attack(Vector2.zero);
            m_IsAttacking = true;
        }

        if (m_Owner.IsCurrAnimationComplete())
        {
            if (m_AttackTimer < 0)
            {
                m_AttackTimer = Time.time;
            }
        }

        if (m_AttackTimer > 0 && Time.time - m_AttackTimer >= 0.05f / m_Owner.entityAttribute.attackSpeed)
        {
            m_CurrAttackCount++;
            m_AttackTimer = -1;
            m_IsAttacking = false;

            if (m_CurrAttackCount >= m_AttackCount)
            {
                m_State = BehaviourTreeState.Success;
                return;
            }
        }
    }

    protected override void OnReset()
    {
        base.OnReset();

        m_CurrAttackCount = 0;
        m_AttackTimer = -1f;
        m_IsAttacking = false;
    }

    protected int m_CurrAttackCount = 0;
    protected int m_AttackCount = 0;
    protected BaseEnemy m_Owner = null;
    protected bool m_IsRandomAttckCount = false;
    private float m_AttackTimer = -1f;
    private bool m_IsAttacking = false;
    private Regex m_Regex = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}