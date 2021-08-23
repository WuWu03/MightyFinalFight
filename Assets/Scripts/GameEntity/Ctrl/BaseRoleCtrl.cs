using DragonBones;
using GameFrameWork;
using GameFrameWork.Sound;
using System.Data.Common;
using UnityEngine;

public class BaseRoleCtrl : BaseCtrl
{
    public bool AttackSuccess
    {
        get
        {
            return m_AttackSuccess;
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
    }

    public virtual void SetData(BaseRoleSkillData data)
    {
        m_Data = data;
        m_SkillManager = new SkillManager(m_Owner, data.SkillIds);
    }

    protected override void OnRelease()
    {
        ReferencePool.Release(m_Data);
        m_SkillManager.Release();
        m_SkillManager = null;
    }

    public void Move(Vector2 dir,bool canChangeDir = true)
    {
        if (!m_Owner.CanMove)
        {
            return;
        }

        MoveData moveData = MoveData.Create();
        moveData.Dir = dir;
        moveData.CanChangeDir = canChangeDir;
        m_Owner.OnMoveMsg(moveData);

        ReferencePool.Release(moveData);
    }

    public void Attack(Vector2 dir)
    {
        if (!m_Owner.CanAttack)
        {
            return;
        }

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

    public void OnAttackSuccess(bool success)
    {
        m_AttackSuccess = success;
    }

    public void Skill(int skillID)
    {
        if (!m_Owner.CanSkill)
        {
            return;
        }

        if(m_SkillManager.IsCurrSkill(skillID) && !m_SkillManager.IsSkillComplete(skillID))
        {
            return;
        }

        ExitSkill();
        m_CurrSkillID = skillID;
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    public virtual void ExitSkill()
    {
        if (m_SkillManager == null)
        {
            return;
        }

        for (int i = 0; i < m_Data.AttackIds.Length; i++)
        {
            if(m_CurrSkillID == m_Data.AttackIds[i])
            {
                m_AttackIndex = 0;
                m_AttackTimer = 0;
                m_AttackSuccess = false;
                break;
            }
        }

        m_SkillManager.ExitSkill();
    }

    public void Jump(Vector2 jumpDir,bool canChangeDir)
    {
        if (!m_Owner.CanJump)
        {
            return;
        }

        JumpData jumpData = JumpData.Create();
        jumpData.Dir = jumpDir;
        jumpData.CanChangeDir = canChangeDir;
        m_Owner.OnJumpMsg(jumpData);

        ReferencePool.Release(jumpData);
    }

    protected override void OnUpdate()
    {
        if (m_AttackTimer > 0)
        {
            float currWait = m_Data.AttackWait[m_AttackIndex];

            if (currWait < 0)
            {
                if (m_Owner.IsPlayComplete())
                {
                    m_AttackIndex = 0;
                    m_AttackTimer = 0;
                    m_AttackSuccess = false;
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
                    m_AttackSuccess = false;
                    m_Owner.FsmMachine.ChangeDefaultState();
                }
            }
        }
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();

        if (m_SkillManager != null)
        {
            m_SkillManager.Update();
        }
    }

    protected virtual void NormalAttack(Vector2 dir)
    {
        if (m_Data.AttackWait == null || m_Data.AttackWait.Length < 1) return;
        if (m_AttackIndex >= m_Data.AttackWait.Length) return;

        if (m_AttackTimer > 0 && m_Data.AttackNextTime != null && m_Data.AttackNextTime.Length > 0)
        {
            if (Time.time - m_AttackTimer < m_Data.AttackNextTime[m_AttackIndex]) return;
        }

        if (AttackSuccess) m_AttackIndex++;
        else m_AttackIndex = 0;

        if(m_AttackIndex >= m_Data.AttackWait.Length)
        {
            m_AttackIndex = m_Data.AttackWait.Length - 1;
        }

        m_AttackTimer = Time.time;
        m_CurrSkillID = m_Data.AttackIds[m_AttackIndex];
        m_SkillManager.DeploySkill(m_CurrSkillID);

        if (m_CurrSkillID == 3004)
        {
            m_Owner.FsmMachine.SetDefaultState<HeroAttackEnd>();
        }
    }

    protected virtual void JumpAttack(Vector2 dir)
    {
        m_AttackSuccess = false;
        m_CurrSkillID = (dir.y < 0 && m_Data.JumpAttackIds.Length > 1) ? m_Data.JumpAttackIds[1] : m_Data.JumpAttackIds[0];
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    private int m_CurrSkillID = 0;
    private int m_AttackIndex = 0;
    private bool m_AttackSuccess = false;
    private float m_AttackTimer = 0;
    private BaseRoleSkillData m_Data = null;

    protected SkillManager m_SkillManager = null;
}