using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoJumpAttack : DoAttack
{
    private readonly bool m_IsMoveJump;
    private bool m_CanJump;
    private BehaviourTreeState m_State = BehaviourTreeState.None;

    public DoJumpAttack(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        Regex mRegex = new(@"(Move)");

        if (!string.IsNullOrEmpty(args))
        {
            m_IsMoveJump = mRegex.Match(args).Success;
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
        base.OnEnter();
        m_State = BehaviourTreeState.Running;
        m_CanJump = owner.isInGround;
        currAttackCount = m_CanJump ? 0 : -1;
        owner.onGroundEvent += OnGround;

        if (m_CanJump)
        {
            m_CanJump = false;
            owner.onDropEvent += OnDrop;
            owner.OppositePlayer();
            owner.Jump(GetJumpDir(), false, true);
        }
    }
    
    private void OnGround()
    {
        currAttackCount++;
        m_CanJump = true;
        
        if (!m_IsMoveJump)
        {
            owner.OppositePlayer();
        }
    }

    private void OnDrop()
    {
        owner.onDropEvent -= OnDrop;
        owner.Attack(Vector2.zero);
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (currAttackCount >= attackCount)
        {
            m_State = BehaviourTreeState.Success;
            return;
        }

        if (m_CanJump)
        {
            m_CanJump = false;
            owner.onDropEvent += OnDrop;
            owner.onGroundEvent += OnGround;
            owner.Jump(GetJumpDir(), false, true);
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_CanJump = false;
        owner.onGroundEvent -= OnGround;
        owner.onDropEvent -= OnDrop;
        m_State = BehaviourTreeState.None;
    }

    private Vector2 GetJumpDir()
    {
        if (!m_IsMoveJump)
        {
            return Vector2.zero;
        }

        return Vector2.right * owner.dir;
    }
}