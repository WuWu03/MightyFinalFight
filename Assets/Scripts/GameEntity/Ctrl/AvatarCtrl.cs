using DragonBones;
using FrameWork.Sound;
using UnityEngine;

public class AvatarCtrl : BaseCtrl
{
    public bool AttackSuccess
    {
        get;
        set;
    }

    protected override void Awake()
    {
        base.Awake();
        m_Owner = base.m_Owner as BaseRole;
    }

    public override void Release()
    {
        base.Release();

        m_SkillManager.Release();
        m_AttackWaitTime = null;
        m_SkillManager = null;

    }
    public void Init(float[] attackWaitTime, int[] skillIDs, float attackNextTime)
    {
        m_AttackWaitTime = attackWaitTime == null ? new float[1] { 0.2f } : attackWaitTime;
        m_SkillManager = new SkillManager(m_Owner, skillIDs);
        m_AttackNextTime = attackNextTime;
        m_Owner = GetComponent<BaseRole>();
    }

    public void Move(Vector2 dir)
    {
        if (!m_Owner.CanMove) return;

        MoveData moveMsgData = new MoveData()
        {
            Dir = dir,
        };

        m_Owner.OnMoveMsg(moveMsgData);
    }

    public void Attack(Vector2 dir)
    {
        if (!m_Owner.CanAttack) return;

        bool isJump = m_Owner.IsAnyState(typeof(RoleJump), typeof(RoleDropTrag));

        if (isJump)
        {
            JumpAttack(dir);
        }
        else
        {
            NormalAttack();
        }
    }

    public void Skill(int skillID)
    {
        if (!m_Owner.CanSkill) return;

        m_CurrSkillID = skillID;
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    public void Jump(Vector2 jumpDir)
    {
        if (!m_Owner.CanJump) return;

        JumpData jumpMsgData = new JumpData()
        {
            Dir = jumpDir,
        };

        m_Owner.OnJumpMsg(jumpMsgData);
    }

    protected override void Update()
    {
        if (m_Owner == null || m_Owner.ResGO == null) return;
        m_SkillManager.Update();

        if (m_AttackTimer > 0)
        {
            float currWait = m_AttackWaitTime[0];
            if (m_AttackWaitTime.Length > 1)
                currWait = m_AttackWaitTime[m_AttackIndex - 1 <= 0 ? 1 : m_AttackIndex - 1];

            if (currWait < 0)
            {
                if (m_Owner.IsPlayComplete())
                {
                    m_AttackIndex = 0;
                    m_AttackTimer = 0;
                    AttackSuccess = false;
                    m_Owner.FsmMachine.ChangeDefaultState();
                }
            }
            else
            {
                float attckStamp = Time.time - m_AttackTimer;
                if (attckStamp > currWait)
                {
                    m_AttackIndex = 0;
                    m_AttackTimer = 0;
                    AttackSuccess = false;
                    m_Owner.FsmMachine.ChangeDefaultState();
                }
            }
        }
    }

    private void NormalAttack()
    {
        if (m_AttackWaitTime == null || m_AttackWaitTime.Length < 1) return;
        if (m_AttackIndex >= m_AttackWaitTime.Length) return;
        if (m_AttackTimer > 0 && Time.time - m_AttackTimer < m_AttackNextTime) return;

        if (m_AttackIndex == 0) AttackSuccess = true;
        if (AttackSuccess) m_AttackIndex++;
        else m_AttackIndex = 1;

        m_AttackTimer = Time.time;
        m_CurrSkillID = 1000 + m_AttackIndex;
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    private void JumpAttack(Vector2 dir)
    {
        AttackSuccess = false;
        m_CurrSkillID = 1004 + (dir.y < 0 ? 2 : 1);
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    private SkillManager m_SkillManager = null;
    private float[] m_AttackWaitTime = null;
    protected new BaseRole m_Owner = null;
    private float m_AttackTimer = 0;
    private int m_AttackIndex = 0;
    private int m_CurrSkillID = 0;
    private float m_AttackNextTime = 0;
}