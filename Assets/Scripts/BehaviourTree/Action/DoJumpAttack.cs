using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoJumpAttack : DoAttack
{
    private readonly bool m_IsMoveJump;
    private bool m_StartJump;
    private bool m_IsGround;
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
        
        if (owner.isInGround)
        {
            owner.onDropEvent += OnDrop;
            owner.onGroundEvent += OnGround;
            owner.OppositePlayer();
            owner.Jump(GetJumpDir(), false, true);
        }
    }

    private void OnGround()
    {
        currAttackCount++;
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
        if (owner.isInGround)
        {
            if (currAttackCount >= attackCount)
            {
                m_State = BehaviourTreeState.Success;
                return;
            }

            owner.onDropEvent += OnDrop;
            owner.onGroundEvent += OnGround;
            owner.Jump(GetJumpDir(), false, true);
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
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