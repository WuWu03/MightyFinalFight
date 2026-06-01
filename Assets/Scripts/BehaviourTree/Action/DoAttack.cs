using WuWuFramework.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoAttack : Action
{
    private readonly BaseEnemy m_AttackOwner;
    private readonly bool m_IsRandomAttackCount;
    private int m_AttackCount;
    private int m_CurrAttackCount;
    private float m_AttackTimer = -1f;
    private bool m_IsAttacking;
    private BehaviourTreeState m_State = BehaviourTreeState.None;

    protected BaseEnemy attackOwner
    {
        get
        {
            return m_AttackOwner;
        }
    }

    protected int attackCount
    {
        get
        {
            return m_AttackCount;
        }
    }

    protected int currAttackCount
    {
        get
        {
            return m_CurrAttackCount;
        }
        set
        {
            m_CurrAttackCount = value;
        }
    }

    public bool isRandomAttackCount
    {
        get
        {
            return m_IsRandomAttackCount;
        }
    }

    public DoAttack(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        m_AttackOwner = owner as BaseEnemy;
        Regex mRegex = new("(AttackTime:)(-?[0-9]+)");

        if (!string.IsNullOrEmpty(args))
        {
            Match m = mRegex.Match(args);
            if (m.Success)
            {
                m_AttackCount = int.Parse(m.Groups[2].Value);
                m_IsRandomAttackCount = m_AttackCount == 0;
            }
        }
    }

    public override bool CanExecute()
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

        if (m_IsRandomAttackCount)
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
            m_AttackOwner.OppositePlayer();
            m_AttackOwner.Attack(Vector2.zero);
            m_IsAttacking = true;
        }

        if (m_AttackOwner.IsCurrAnimationComplete())
        {
            if (m_AttackTimer < 0)
            {
                m_AttackTimer = Time.time;
            }
        }

        if (m_AttackTimer > 0 && Time.time - m_AttackTimer >= 0.05f / m_AttackOwner.entityAttribute.attackSpeed)
        {
            m_CurrAttackCount++;
            m_AttackTimer = -1;
            m_IsAttacking = false;

            if (m_CurrAttackCount >= m_AttackCount)
            {
                m_State = BehaviourTreeState.Success;
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
}