using GameFrameWork;
using UnityEngine;

public class BaseRoleCtrl : BaseCtrl
{
    public bool isHitSuccess
    {
        get
        {
            return m_IsHitSuccess;
        }
    }

    public virtual void SetData(BaseRoleSkillData data)
    {
        m_Data = data;
        m_SkillManager = new SkillManager(m_Owner, data.skillIds);

        if (m_Owner.isResComplete && !IsRunning())
        {
            Start();
        }
    }

    public void Move(Vector2 dir, bool canChangeDir = true)
    {
        if (!m_Owner.canMove)
        {
            return;
        }

        MoveData moveData = MoveData.Create();
        moveData.dir = dir;
        moveData.canChangeDir = canChangeDir;
        m_Owner.OnMoveMsg(moveData);

        ReferencePool.Release(moveData);
    }

    public void Attack(Vector2 dir)
    {
        if (!m_Owner.canAttack)
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

    public void SetHitState(bool success)
    {
        m_IsHitSuccess = success;
    }

    public void DeploySkill(int skillID)
    {
        if (!m_Owner.canSkill)
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

    public bool IsInSkill()
    {
        return m_SkillManager.IsInSkill();
    }

    public bool IsCurrSkill(int skillId)
    {
        return m_SkillManager.IsCurrSkill(skillId);
    }

    public bool IsSkillComplete(int skillId)
    {
        return m_SkillManager.IsSkillComplete(skillId);
    }

    public virtual void ExitSkill()
    {
        if (m_SkillManager == null)
        {
            return;
        }

        for (int i = 0; i < m_Data.attackIds.Length; i++)
        {
            if (m_CurrSkillID == m_Data.attackIds[i])
            {
                m_IsHitSuccess = false;
                m_IsAttack = false;
                m_AttackTimer = -1;
                m_CanAttack = true;
                m_AttackIndex = 0;
                break;
            }
        }

        m_IsHitSuccess = false;
        m_SkillManager.ExitSkill();
    }

    public void Jump(Vector2 jumpDir, bool canChangeDir, bool isForceJump = false)
    {
        if (!isForceJump && !m_Owner.canJump)
        {
            return;
        }

        JumpData jumpData = JumpData.Create();
        jumpData.dir = jumpDir;
        jumpData.canChangeDir = canChangeDir;
        m_Owner.OnJumpMsg(jumpData);

        ReferencePool.Release(jumpData);
    }

    protected override void OnUpdate()
    {
        if (m_IsAttack)
        {
            if (m_Owner.IsPlayComplete() )
            {
                m_AttackTimer = Time.time;
                m_IsAttack = false;
            }
        }

        if (m_AttackTimer > 0)
        {
            if (Time.time - m_AttackTimer > 0.0555f)
            {
                if (m_AttackIndex < m_Data.attackIds.Length - 1)
                {
                    m_CanAttack = true;
                }
            }

            if(Time.time - m_AttackTimer > 0.2f && m_Owner.isInGround)
            {
                m_IsHitSuccess = false;
                m_IsAttack = false;
                m_AttackTimer = -1;
                m_CanAttack = true;
                m_AttackIndex = 0;
                m_Owner.ChangeDefaultState();
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

    protected override void OnRelease()
    {
        ReferencePool.Release(m_Data);
        m_SkillManager.Release();
        m_SkillManager = null;
        base.OnRelease();
    }

    protected virtual void NormalAttack(Vector2 dir)
    {
        if(!m_CanAttack)
        {
            return;
        }

        if(m_IsHitSuccess)
        {
            if (m_AttackIndex < m_Data.attackIds.Length - 1)
            {
                m_AttackIndex++;
            }
        }
        else
        {
            m_AttackIndex = 0;
        }

        m_IsAttack = true;
        m_CanAttack = false;
        m_AttackTimer = -1;
        m_CurrSkillID = m_Data.attackIds[m_AttackIndex];
        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    protected virtual void JumpAttack(Vector2 dir)
    {
        m_IsHitSuccess = false;

        if(dir.y < 0 && m_Data.jumpAttackIds.Length > 1)
        {
            m_CurrSkillID = m_Data.jumpAttackIds[1];
        }
        else
        {
            m_CurrSkillID = m_Data.jumpAttackIds[0];
        }

        m_SkillManager.DeploySkill(m_CurrSkillID);
    }

    protected virtual bool IsCanNormalAttack()
    {
        if (m_Data.attackWait == null || m_Data.attackWait.Length < 1)
        {
            return false;
        }

        if (m_AttackTimer > 0 && m_Data.attackNextTime != null && m_Data.attackNextTime.Length > 0)
        {
            if (m_AttackIndex >= m_Data.attackNextTime.Length)
            {
                return false;
            }

            if (Time.time - m_AttackTimer < m_Data.attackNextTime[m_AttackIndex])
            {
                return false;
            }
        }

        return true;
    }

    private int m_CurrSkillID = 0;
    private int m_AttackIndex = 0;
    private bool m_IsHitSuccess = false;
    private bool m_IsAttack = false;
    private bool m_CanAttack = true;
    private float m_AttackTimer = -1;
    private BaseRoleSkillData m_Data = null;
    private SkillManager m_SkillManager = null;
}