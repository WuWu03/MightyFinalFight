using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoJumpAttack : DoAttack
{
    public DoJumpAttack(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        m_Regex = new(@"(Move)");

        if (!string.IsNullOrEmpty(args))
        {
            m_IsMoveJump = m_Regex.Match(args).Success;
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
        base.OnEnter();
        m_State = BehaviourTreeState.Running;

        m_IsGround = false;
        m_Owner.onGroundEvent.AddListener(OnGround);

        if (m_Owner.isInGround)
        {
            m_StartJump = false;
            m_Owner.onDropEvent.AddListener(OnDrop);
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

        if (!m_IsMoveJump)
        {
            m_Owner.OppositePlayer();
        }

        m_IsGround = true;
    }

    private void OnDrop()
    {
        m_Owner.onDropEvent.RemoveListener(OnDrop);
        m_Owner.Attack(Vector2.zero);
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (m_CurrAttackCount >= m_AttackCount && m_Owner.isInGround)
        {
            m_State = BehaviourTreeState.Success;
            return;
        }

        if (!m_IsGround || m_CurrAttackCount >= m_AttackCount)
        {
            return;
        }

        m_IsGround = false;

        if (m_StartJump)
        {
            m_StartJump = false;
            m_Owner.onDropEvent.AddListener(OnDrop);
            m_Owner.onGroundEvent.AddListener(OnGround);
            m_Owner.Jump(GetJumpDir(), false, true);
        }
        else
        {
            m_Owner.onDropEvent.AddListener(OnDrop);
            m_Owner.onGroundEvent.AddListener(OnGround);
            m_Owner.Jump(GetJumpDir(), false, true);
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_Owner.onDropEvent.RemoveListener(OnDrop);
        m_State = BehaviourTreeState.None;
    }
    private Vector2 GetJumpDir()
    {
        if (!m_IsMoveJump)
        {
            return Vector2.zero;
        }

        return Vector2.right * m_Owner.dir;
    }

    private bool m_StartJump = false;
    private bool m_IsMoveJump = false;
    private Regex m_Regex = null;
    private bool m_IsGround = false;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}