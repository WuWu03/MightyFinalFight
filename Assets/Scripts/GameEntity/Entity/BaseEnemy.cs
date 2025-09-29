using GameFrameWork.Audio;
using GameFrameWork.BehaviourTree;
using GameFrameWork.UI;
using GameFrameWork.Utils;
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

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
        BehaviourTreeMgr.instance.StartTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (rigidbody2D != null && rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
        {
            float x = rigidbody2D.linearVelocity.x > 0 ? bound.xMax : bound.xMin;

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

    protected override void OnRelease()
    {
        PlayerMgr.instance.AddExp(m_SkillExp);
        BehaviourTreeMgr.instance.RemoveBehaviourTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
        m_SkillExp = 0;
        m_HpBarWidth = 0;
        m_IsBoss = false;
        m_HurtAnim = null;
        m_BaseEnemySkillData = null;
        base.OnRelease();
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        BaseEnemyData enemyData = data as BaseEnemyData;
        m_HurtAnim = enemyData.hurtAnims;
        m_HpBarWidth = enemyData.hpBarWdith;
        m_IsBoss = enemyData.isBoss;
    }

    public override void SetSkillData(BaseRoleSkillData skilldata)
    {
        base.SetSkillData(skilldata);
        m_BaseEnemySkillData = skilldata as BaseEnemySkillData;
        BehaviourTreeMgr.instance.AddBehaviourTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
    }

    public void OppositePlayer()
    {
        SetDir(PlayerMgr.instance.player.pos.x - pos.x > 0 ? 1f : -1f);
    }

    public override void Pause()
    {
        base.Pause();
        BehaviourTreeMgr.instance.PauseTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
    }

    public override void Resume()
    {
        base.Resume();
        BehaviourTreeMgr.instance.ResumeTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
    }

    public override void SetPos(Vector2 pos, float posZ, bool caculateZ = false)
    {
        if (IsAnyState(typeof(RoleMove)))
        {
            if (!canMove)
            {
                return;
            }

            bool isMapXCanMove = StageMgr.instance.CanMovePosX(moveDir.x > 0 ? bound.xMax : bound.xMin);
            bool isMapYCanMove = StageMgr.instance.CanMovePosY(pos.y);
            pos.x = isMapXCanMove ? pos.x : this.pos.x;
            pos.y = isMapYCanMove ? pos.y : this.pos.y;
        }

        base.SetPos(pos, posZ, caculateZ);
    }

    public override void SetIsBeCatch(bool value)
    {
        base.SetIsBeCatch(value);

        if (value)
        {
            ChangeState<RoleIdle>();
        }
    }

    public override void OnHurtMsg(HurtStateData data)
    {
        if (data.isBoss)
        {
            if (IsAnyState(typeof(RoleSkill)))
            {
                return;
            }
        }

        if (isBeCatch)
        {
            if (m_HurtAnim != null && m_HurtAnim.Length > 0)
            {
                data.hurtAnim = m_HurtAnim[0];
            }
        }
        else
        {
            if (m_HurtAnim != null && m_HurtAnim.Length > 0)
            {
                data.hurtAnim = m_HurtAnim[Random.Range(0, m_HurtAnim.Length)];
            }
        }

        if (IsHurtWillDie(data.attackValue))
        {
            m_SkillExp = data.skillExp;
        }

        base.OnHurtMsg(data);
    }

    protected override void OnGroundHurtMsg(HurtStateData data)
    {
        if (!data.isGroundHurt)
        {
            int dir = data.attackerPos.x > pos.x ? -1 : 1;
            Vector3 tempPos = new(dir > 0 ? 0 : 0, bound.size.y / 2, 0.1f * -dir);
            EffectMgr.instance.PlayDBEffect(PlayerMgr.instance.roleConfigData.hitEffect, transform, tempPos, Vector3.zero, true, true, 0.1f);
        }

        Vector3 damagePos = transform.position + Vector3.up * boxCollider2D.size.y / 2f + Vector3.right * boxCollider2D.size.x / 2 * data.attackerDir;

        if (data.attackValue > 0)
        {
            HudMgr.instance.ShowPlayerDamage(data.attackValue, damagePos);
            base.OnGroundHurtMsg(data);
            MainPanel mainPanel = UIMgr.instance.Get(UINames.MainPanel) as MainPanel;
            mainPanel.SetEnemyHP(entityAttribute.health, entityAttribute.maxHealth, m_HpBarWidth);
        }
        else
        {
            base.OnGroundHurtMsg(data);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFloat || isBeCatch || collision.gameObject.Equals(gameObject))
        {
            return;
        }

        BaseRole throwTarget = collision.gameObject.GetComponent<BaseRole>();

        if (throwTarget == null || throwTarget.objectType != ObjectType.Enemy || !throwTarget.isBeThrow)
        {
            return;
        }

        if (Mathf.Abs(pos.y - throwTarget.pos.y) > 0.1f)
        {
            return;
        }

        HurtStateData hurtData = HurtStateData.Create();
        hurtData.id = 0;
        hurtData.skillExp = 2;
        hurtData.isChangeVelocity = true;
        hurtData.changeVelocity = Vector2.zero;
        hurtData.attackerDir = throwTarget.pos.x < pos.x ? 1 : -1;
        hurtData.attackForce = SkillUtil.GetSmoonForce(hurtData.attackerDir);
        hurtData.attackerPos = throwTarget.pos;
        hurtData.canBeDefense = false;
        hurtData.isSwoon = true;
        hurtData.attackerId = id;
        hurtData.attackValue = Mathf.FloorToInt(entityAttribute.maxHealth * 0.1f);
        hurtData.hurtSound = string.Empty;
        hurtData.hurtAnim = string.Empty;
        hurtData.isGroundHurt = true;
        OnHurtMsg(hurtData);

        HurtStateData targetHurt = HurtStateData.Create();
        targetHurt.id = 0;
        targetHurt.skillExp = 2;
        targetHurt.isChangeVelocity = true;
        targetHurt.changeVelocity = Vector2.zero;
        targetHurt.attackerDir = throwTarget.pos.x < pos.x ? 1 : -1;
        targetHurt.attackForce = SkillUtil.GetSmoonForce(-hurtData.attackerDir);
        targetHurt.attackerPos = pos;
        targetHurt.canBeDefense = false;
        targetHurt.isSwoon = true;
        targetHurt.attackerId = id;
        targetHurt.attackValue = Mathf.FloorToInt(entityAttribute.maxHealth * 0.1f);
        targetHurt.hurtSound = string.Empty;
        targetHurt.hurtAnim = string.Empty;
        targetHurt.isGroundHurt = true;
        throwTarget.OnHurtMsg(targetHurt);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.OnHit02));
 
    }

    private int m_SkillExp = 0;
    private int m_HpBarWidth = 0;
    private bool m_IsBoss = false;
    private string[] m_HurtAnim = null;
    private BaseEnemySkillData m_BaseEnemySkillData = null;
}