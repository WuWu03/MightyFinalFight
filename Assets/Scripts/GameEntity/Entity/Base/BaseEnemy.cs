using GameFrameWork;
using GameFrameWork.UI;
using UnityEngine;

public class BaseEnemy : BaseRole
{
    public bool isBoss
    {
        get
        {
            return m_IsBoss;
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
        m_HurtAnim = enemyData.hurtAnims;
        m_HpBarWidth = enemyData.hpBarWdith;
        m_IsBoss = enemyData.isBoss;
    }

    public override void SetPos(Vector2 pos, float posZ, bool caculateZ = false)
    {
        if (IsAnyState(typeof(RoleMove)))
        {
            if (!canMove)
            {
                return;
            }

            bool isMapXCanMove = StageMgr.instance.CanMovePosX(m_MoveDir.x > 0 ? bound.xMax : bound.xMin);
            bool isMapYCanMove = StageMgr.instance.CanMovePosY(pos.y);

            if (!isMapXCanMove) pos.x = m_Pos.x;
            if (!isMapYCanMove) pos.y = m_Pos.y;
        }

        base.SetPos(pos, posZ, caculateZ);
    }

    public override void OnHurtMsg(HurtStateData data)
    {
        if (data.isBoss)
        {
            if (IsAnyState(typeof(RoleAttack)))
            {
                return;
            }
        }

        if(m_IsBeCatch)
        {
            if (m_HurtAnim != null && m_HurtAnim.Length > 0)
            {
                data.hurtAnim = m_HurtAnim[0];
            }
        }
        else
        {
            if(isDrop)
            {
                data.isSwoon = true;
                data.attackForce = SkillUtil.GetSmoonForce(data.attackerDir);
            }

            if(m_HurtAnim != null && m_HurtAnim.Length > 0)
            {
                data.hurtAnim = m_HurtAnim[Random.Range(0, m_HurtAnim.Length)];
            }
        }

        if(m_EntityAttribute.health - data.attackValue <= 0)
        {
            m_SkillExp = data.skillExp;
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
        PlayerMgr.instance.AddExp(m_SkillExp);

        m_SkillExp = 0;
        m_HpBarWidth = 0;
        m_IsBoss = false;
        m_HurtAnim = null;

        base.Release();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_Rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
        {
            float x = m_Rigidbody2D.linearVelocity.x > 0 ? bound.xMax : bound.xMin;

            if (!StageMgr.instance.CanMovePosX(x))
            {
                SetVelocityX(0);
            }

            if (m_IsBoss && IsOutVersionX(x))
            {
                SetVelocityX(0);
            }
        }
    }

    protected override void OnGroundHurtMsg(HurtStateData data)
    {
        if (!data.isGroundHurt)
        {
            int dir = data.attackerPos.x > m_Pos.x ? -1 : 1;
            Vector3 pos = new Vector3(dir > 0 ? 0 : 0, bound.size.y / 2, 0.1f * -m_Dir);
            EffectMgr.instance.PlayDBEffect(PlayerMgr.instance.roleConfigData.hitEffect, transform, pos, Vector3.zero, true, true, 0.1f);
        }

        Vector3 damagePos = transform.position + Vector3.up * m_BoxCollider2D.size.y / 2f + Vector3.right * m_BoxCollider2D.size.x / 2 * data.attackerDir;

        if (data.attackValue > 0)
        {
            HudMgr.instance.ShowPlayerDamage(data.attackValue, damagePos);
            base.OnGroundHurtMsg(data);
            UIMgr.instance.Get<MainPanel>().SetEnemyHP(m_EntityAttribute.health, m_EntityAttribute.maxHealth, m_HpBarWidth);
        }
        else
        {
            base.OnGroundHurtMsg(data);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (isBeCatch || collision.gameObject.Equals(gameObject))
        {
            return;
        }

        BaseRole throwTarget = collision.gameObject.GetComponent<BaseRole>();
        ICanBeHit hit = collision.gameObject.GetComponent<ICanBeHit>();

        if (throwTarget == null || hit == null || throwTarget.objectType != ObjectType.Enemy || !throwTarget.isBeThrow)
        {
            return;
        }

        if (Mathf.Abs(m_Pos.y - throwTarget.pos.y) > 0.1f)
        {
            return;
        }

        HurtStateData hurtData = HurtStateData.Create();
        hurtData.id = 0;
        hurtData.skillExp = 2;
        hurtData.attackerDir = -m_Dir;
        hurtData.attackForce = SkillUtil.GetSmoonForce(-m_Dir);
        hurtData.attackerPos = m_Pos;
        hurtData.canBeDefense = false;
        hurtData.isSwoon = true;
        hurtData.attackerId = id;
        hurtData.attackValue = Mathf.FloorToInt(m_EntityAttribute.maxHealth * 0.1f);
        hurtData.hurtSound = string.Empty;
        hurtData.hurtAnim = string.Empty;
        hurtData.isGroundHurt = true;

        OnHurtMsg(hurtData);
    }

   
    private int m_SkillExp = 0;
    private int m_HpBarWidth = 0;
    private bool m_IsBoss = false;
    private string[] m_HurtAnim = null;
}