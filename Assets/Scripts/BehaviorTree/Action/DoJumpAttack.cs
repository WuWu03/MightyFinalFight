using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoJumpAttack : DoAttack
{
    public DoJumpAttack(string name, string args, object owner) : base(name, args, owner)
    {
        if (!string.IsNullOrEmpty(args))
        {
            m_IsMoveJump = m_Regex.Match(args).Success;
        }
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        m_IsGround = false;
        m_Owner.Owner.OnGroundEvent.AddListener(OnGround);

        if (m_Owner.Owner.IsInGround)
        {
            m_StartJump = false;
            m_Owner.Owner.OnDropEvent.AddListener(OnDrop);
            m_Owner.OppositePlayer();
            m_Owner.Jump(GetJumpDir(), false, true);
        }
        else
        {
            m_StartJump = true;
        }
    }

    private void OnGround()
    {
        if (!m_StartJump)
        {
            m_CurrAttackCount++;
        }

        m_IsGround = true;
    }

    private void OnDrop()
    {
        if (!m_IsMoveJump)
        {
            m_Owner.OppositePlayer();
        }

        m_Owner.Attack(Vector2.zero);
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!m_IsGround || m_CurrAttackCount >= m_AttackCount)
        {
            return;
        }

        m_IsGround = false;

        if (m_StartJump)
        {
            m_StartJump = false;
            m_Owner.Owner.OnDropEvent.AddListener(OnDrop);
            m_Owner.Owner.OnGroundEvent.AddListener(OnGround);
            m_Owner.OppositePlayer();
            m_Owner.Jump(GetJumpDir(), false, true);
        }
        else
        {
            m_Owner.Owner.OnDropEvent.AddListener(OnDrop);
            m_Owner.Owner.OnGroundEvent.AddListener(OnGround);

            if (!m_IsMoveJump)
            {
                m_Owner.OppositePlayer();
            }
            m_Owner.Jump(GetJumpDir(), false, true);
        }
    }

    public override BehaviorTreeState Excute()
    {
        if (m_CurrAttackCount >= m_AttackCount && m_Owner.Owner.IsInGround)
        {
            return BehaviorTreeState.Success;
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
    }

    private Vector2 GetJumpDir()
    {
        if (!m_IsMoveJump)
        {
            return Vector2.zero;
        }

        return Vector2.right * m_Owner.Owner.Dir;
    }

    private bool m_StartJump = false;
    private bool m_IsMoveJump = false;
    private Regex m_Regex = new Regex(@"(Move)");
    private bool m_IsGround = false;
}
