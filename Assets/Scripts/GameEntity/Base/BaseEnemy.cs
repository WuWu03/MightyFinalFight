using GameFrameWork;
using GameFrameWork.UI;
using UnityEngine;

public class BaseEnemy : BaseRole
{
    public bool IsThrowing 
    {
        get
        {
            return m_IsBeThrow;
        }
    }

    public override bool CanJump
    {
        get
        {
            return false;
        }
    }

    public event GameFrameWorkAction<int> OnDead
    {
        add 
        {
            m_OnDeadEventHandler += value;
        }
        remove
        {
            m_OnDeadEventHandler -= value;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_HurtAnim = (data as BaseEnemyData).HurtAnim;
    }

    public override void SetPos(Vector2 pos)
    {
        if (IsAnyState(typeof(RoleMove)))
        {
            if (!CanMove) return;
            Rect bound = GetBound(pos);
            bool isMapXCanMove = StageMgr.Ins.CanMovePosX(m_MoveDir.x > 0 ? bound.xMax : bound.xMin);
            bool isMapYCanMove = StageMgr.Ins.CanMovePosY(pos.y);

            if (!isMapXCanMove) pos.x = m_Pos.x;
            if (!isMapYCanMove) pos.y = m_Pos.y;
        }

        base.SetPos(pos);
    }

    public override void SubHealth(int value)
    {
        base.SubHealth(value);
        UIMgr.Ins.GetPanel<MainPanel>().SetEnemyHP(m_Health, m_MaxHealth, 400f);
    }

    public override void OnHurtMsg(HurtData data)
    {
        if(m_IsBeCatch)
        {
            data.HurtAnim = m_HurtAnim[m_HurtAnim.Length - 1];
        }
        else
        {
            data.HurtAnim = m_HurtAnim[Random.Range(0, m_HurtAnim.Length)];
        }

        if(m_Health - data.AttackValue <= 0)
        {
            m_SkillExp = data.SkillExp;
        }

        base.OnHurtMsg(data);
    }

    public override void SetCatch(bool value)
    {
        base.SetCatch(value);
        if(value)
        {
            ChangeState<RoleIdle>();
        }
    }

    public override void Release()
    {
        base.Release();
        PlayerMgr.Ins.AddExp(m_SkillExp);
        m_OnDeadEventHandler?.Invoke(m_EntityID);
        m_SkillExp = 0;
        m_HurtAnim = null;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_Rigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            if (!StageMgr.Ins.CanMovePosX(transform.localPosition.x) && Mathf.Abs(m_Rigidbody.velocity.x) > 0)
            {
                m_Rigidbody.velocity = new Vector2(0, m_Rigidbody.velocity.y);
            }
        }
    }

    protected override void OnGroundHurtMsg(HurtData data)
    {
        if (!data.IsGroundHurt)
        {
            int dir = data.AttackerPos.x > m_Pos.x ? -1 : 1;
            Vector3 pos = new Vector3(dir > 0 ? 0 : 0, Bound.size.y/2, 0.1f * -m_Dir);
            EffectMgr.Ins.PlayEffect(PlayerMgr.Ins.HeroData.HitEffect, transform, pos, Vector3.zero, true, true, 0.1f);
        }

        base.OnGroundHurtMsg(data);     
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.Equals(gameObject)) return;
        BaseRole throwTarget = collision.gameObject.GetComponent<BaseRole>();
        ICanBeHit hit = collision.gameObject.GetComponent<ICanBeHit>();

        if (throwTarget == null || hit == null || throwTarget.ObjectType != ObjectType.Monster || !throwTarget.IsBeThrow) return;

        bool isInRange = Mathf.Abs(m_Pos.y - throwTarget.Pos.y) <= 0.1f;
        if (!isInRange) return;

        HurtData hurtData = HurtData.Create();
        hurtData.Id = 0;
        hurtData.SkillExp = 2;
        hurtData.AttackerDir = -m_Dir;
        hurtData.AttackForce = new Vector2(40 * -m_Dir, 150);
        hurtData.AttackerPos = m_Pos;
        hurtData.CanBeDefense = false;
        hurtData.IsSwoon = true;
        hurtData.AttackerID = m_ID;
        hurtData.AttackValue = 1;
        hurtData.HurtSound = string.Empty;
        hurtData.HurtAnim = string.Empty;
        hurtData.IsGroundHurt = true;

        OnHurtMsg(hurtData);
        ReferencePool.Release(hurtData);
    }

    private GameFrameWorkAction<int> m_OnDeadEventHandler = null;
    private int m_SkillExp = 0;
    private string[] m_HurtAnim = null;
}