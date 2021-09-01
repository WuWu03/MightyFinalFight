using GameFrameWork;
using GameFrameWork.UI;
using UnityEngine;

public class BaseEnemy : BaseRole
{
    public bool IsBoss
    {
        get
        {
            return m_IsBoss;
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
        BaseEnemyData enemyData = data as BaseEnemyData;
        m_HurtAnim = enemyData.HurtAnim;
        m_HpBarWidth = enemyData.HpBarWdith;
        m_IsBoss = enemyData.IsBoss;
    }

    public override void SetPos(Vector2 pos)
    {
        if (IsAnyState(typeof(RoleMove)))
        {
            if (!CanMove)
            {
                return;
            }

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
        UIMgr.Ins.GetPanel<MainPanel>().SetEnemyHP(m_Health, m_MaxHealth, m_HpBarWidth);
    }

    public override void OnHurtMsg(HurtData data)
    {
        if(m_IsBeCatch)
        {
            if (m_HurtAnim != null && m_HurtAnim.Length > 0)
            {
                data.HurtAnim = m_HurtAnim[0];
            }
        }
        else
        {
            if(IsDrop)
            {
                data.IsSwoon = true;
                data.AttackForce = SkillFactory.GetSmoonForce(data.AttackerDir);
            }

            if(m_HurtAnim != null && m_HurtAnim.Length > 0)
            {
                data.HurtAnim = m_HurtAnim[Random.Range(0, m_HurtAnim.Length)];
            }
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
            Rect bound = GetBound(transform.localPosition);
            float x = m_Rigidbody.velocity.x > 0 ? bound.xMax : bound.xMin;

            if (!StageMgr.Ins.CanMovePosX(x))
            {
                SetVelocityX(0);
            }

            if (m_IsBoss && IsOutVersionX(x))
            {
                SetVelocityX(0);
            }
        }
    }

    protected override void OnGroundHurtMsg(HurtData data)
    {
        if (!data.IsGroundHurt)
        {
            int dir = data.AttackerPos.x > m_Pos.x ? -1 : 1;
            Vector3 pos = new Vector3(dir > 0 ? 0 : 0, Bound.size.y/2, 0.1f * -m_Dir);
            EffectMgr.Ins.PlayEffect(PlayerMgr.Ins.CharacterData.HitEffect, transform, pos, Vector3.zero, true, true, 0.1f);
        }

        Vector3 damagePos = transform.position + Vector3.up * m_Collider.size.y / 2f + Vector3.right * m_Collider.size.x / 2 * data.AttackerDir;
        UIMgr.Ins.GetPanel<MainPanel>().ShowEnemyDamage(data.AttackValue, damagePos);
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
        hurtData.AttackForce = SkillFactory.GetSmoonForce(-m_Dir);
        hurtData.AttackerPos = m_Pos;
        hurtData.CanBeDefense = false;
        hurtData.IsSwoon = true;
        hurtData.AttackerId = Id;
        hurtData.AttackValue = Mathf.FloorToInt(m_MaxHealth * 0.1f);
        hurtData.HurtSound = string.Empty;
        hurtData.HurtAnim = string.Empty;
        hurtData.IsGroundHurt = true;

        OnHurtMsg(hurtData);
    }

    private GameFrameWorkAction<int> m_OnDeadEventHandler = null;
    private int m_SkillExp = 0;
    private int m_HpBarWidth = 0;
    private bool m_IsBoss = false;
    private string[] m_HurtAnim = null;
}