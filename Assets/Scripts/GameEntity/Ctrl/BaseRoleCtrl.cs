using DragonBones;
using FrameWork.Sound;
using System.Data.Common;
using UnityEngine;

public class BaseRoleCtrl : BaseCtrl
{
    public bool AttackSuccess
    {
        get;
        set;
    }

    protected override void OnInit()
    {
        base.OnInit();
        m_MoveData = new MoveData();
        m_JumpData = new JumpData();
    }

    public virtual void InitData(BaseRoleSkillData data)
    {
        m_AttackIDs = data.AttackIDs;
        m_JumpAttackIDs = data.JumpAttackIDs;
        m_AttackWait = data.AttackWait;
        m_AttackNextTime = data.AttackNextTime;

        m_SkillManager = new SkillManager(m_Owner, data.Skills);
    }

    protected override void OnRelease()
    {
        m_SkillManager.Release();
        m_AttackIDs = null;
        m_JumpAttackIDs = null;
        m_AttackWait = null;
        m_SkillManager = null;
        m_MoveData = null;
        m_JumpData = null;
    }

    public void Move(Vector2 dir)
    {
        if (!m_Owner.CanMove) return;

        m_MoveData.Clear();
        m_MoveData.Dir = dir;
        m_Owner.OnMoveMsg(m_MoveData);
    }

    public void Attack(Vector2 dir)
    {
        if (!m_Owner.CanAttack) return;

        bool isJump = m_Owner.IsAnyState(typeof(RoleJump));

        if (isJump)
        {
            JumpAttack(dir);
        }
        else
        {
            NormalAttack(dir);
        }
    }

    public void Skill(int skillID)
    {
        if (!m_Owner.CanSkill) return;

        m_CurrSkillID = skillID;
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    public virtual void ExitSkill()
    {
        if (m_SkillManager == null) return;

        for(int i = 0; i < m_AttackIDs.Length; i++)
        {
            if(m_CurrSkillID == m_AttackIDs[i])
            {
                m_AttackIndex = 0;
                m_AttackTimer = 0;
                AttackSuccess = false;
                break;
            }
        }

        m_SkillManager.ExitSkill();
    }

    public void Jump(Vector2 jumpDir)
    {
        if (!m_Owner.CanJump) return;

        m_JumpData.Clear();
        m_JumpData.Dir = jumpDir;
        m_Owner.OnJumpMsg(m_JumpData);
    }

    protected override void OnUpdate()
    {
        if (m_SkillManager != null)
        {
            m_SkillManager.Update();
        }

        if (m_AttackTimer > 0)
        {
            float currWait = m_AttackWait[0];
            if (m_AttackWait.Length > 1)
                currWait = m_AttackWait[m_AttackIndex - 1 <= 0 ? 1 : m_AttackIndex - 1];

            if (currWait < 0)
            {
                if (m_Owner.IsPlayComplete())
                {
                    m_AttackIndex = 0;
                    m_AttackTimer = 0;
                    AttackSuccess = false;
                    if (m_Owner.IsInGround)
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

    protected virtual void NormalAttack(Vector2 dir)
    {
        if (m_AttackWait == null || m_AttackWait.Length < 1) return;
        if (m_AttackIndex >= m_AttackWait.Length) return;
        if (m_AttackTimer > 0 && Time.time - m_AttackTimer < m_AttackNextTime) return;

        if (m_AttackIndex == 0) AttackSuccess = true;
        if (AttackSuccess) m_AttackIndex++;
        else m_AttackIndex = 1;

        m_AttackTimer = Time.time;
        m_CurrSkillID = m_AttackIDs[m_AttackIndex - 1];
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    protected virtual void JumpAttack(Vector2 dir)
    {
        AttackSuccess = false;
        m_CurrSkillID = dir.y < 0 ? m_JumpAttackIDs[1] : m_JumpAttackIDs[0];
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }
 
    private int[] m_AttackIDs = null;
    private int[] m_JumpAttackIDs = null;
    private float[] m_AttackWait = null;
    private float m_AttackTimer = 0;
    private int m_AttackIndex = 0;
    private int m_CurrSkillID = 0;
    private float m_AttackNextTime = 0;

    private MoveData m_MoveData = null;
    private JumpData m_JumpData = null;
    protected SkillManager m_SkillManager = null;
}