using GameFrameWork;
using GameFrameWork.Utils;
using UnityEngine;

public class BaseEnemy : BaseRole
{
    private int m_SkillExp;
    private int m_HpBarWidth;
    private bool m_IsBoss;
    private string[] m_HurtAnim;
    private BaseEnemySkillData m_BaseEnemySkillData;
    
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
        GameEntry.behaviourTreeMgr.StartTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (rigidbody2D is not null && rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
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
        GameEntry.behaviourTreeMgr.RemoveBehaviourTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
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

        if (enemyData == null)
        {
            Log.LogError("敌人数据为空");
            return;
        }
        
        m_HurtAnim = enemyData.hurtAnims;
        m_HpBarWidth = enemyData.hpBarWdith;
        m_IsBoss = enemyData.isBoss;
    }

    public override void SetSkillData(BaseRoleSkillData skilldata)
    {
        base.SetSkillData(skilldata);
        m_BaseEnemySkillData = skilldata as BaseEnemySkillData;

        if (m_BaseEnemySkillData == null)
        {
            Log.LogError("敌人技能数据为空");
            return;
        }
        
        GameEntry.behaviourTreeMgr.AddBehaviourTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
    }

    public void OppositePlayer()
    {
        SetDir(PlayerMgr.instance.player.pos.x - pos.x > 0 ? 1f : -1f);
    }

    public override void Pause()
    {
        base.Pause();
        GameEntry.behaviourTreeMgr.PauseTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
    }

    public override void Resume()
    {
        base.Resume();
        GameEntry.behaviourTreeMgr.ResumeTree(this, m_BaseEnemySkillData.behaviourTreeIds[0]);
    }

    public override void SetPos(Vector2 pos, float posZ, bool calculateZ = false)
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

        base.SetPos(pos, posZ, calculateZ);
    }

    public override void SetIsBeCatch(bool value)
    {
        base.SetIsBeCatch(value);

        if (value)
        {
            ChangeState<RoleIdle>();
        }
    }

    public override void OnHurtMsg(HurtStateArg arg)
    {
        if (arg.isBoss)
        {
            if (IsAnyState(typeof(RoleSkill)))
            {
                return;
            }
        }

        if (isBeCatch)
        {
            if (m_HurtAnim is { Length: > 0 })
            {
                arg.hurtAnim = m_HurtAnim[0];
            }
        }
        else
        {
            if (m_HurtAnim is { Length: > 0 })
            {
                arg.hurtAnim = m_HurtAnim[Random.Range(0, m_HurtAnim.Length)];
            }
        }

        if (IsHurtWillDie(arg.attackValue))
        {
            m_SkillExp = arg.skillExp;
        }

        base.OnHurtMsg(arg);
    }

    protected override void OnGroundHurtMsg(HurtStateArg arg)
    {
        if (!arg.isGroundHurt)
        {
            int dir = arg.attackerPos.x > pos.x ? -1 : 1;
            Vector3 tempPos = new(0, bound.size.y / 2, 0.1f * -dir);
            EffectMgr.instance.PlayDBEffect(PlayerMgr.instance.roleConfigData.hitEffect, transform, tempPos, Vector3.zero, true, true, 0.1f);
        }

        Vector3 damagePos = transform.position + Vector3.up * boxCollider2D.size.y / 2f + Vector3.right * boxCollider2D.size.x / 2 * arg.attackerDir;

        if (arg.attackValue > 0)
        {
            HudMgr.instance.ShowPlayerDamage(arg.attackValue, damagePos);
            base.OnGroundHurtMsg(arg);
            GameEntry.uiMgr.Get<MainView>().SetEnemyHP(entityAttribute.health, entityAttribute.maxHealth, m_HpBarWidth);
        }
        else
        {
            base.OnGroundHurtMsg(arg);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFloat || isBeCatch || collision.gameObject.Equals(gameObject))
        {
            return;
        }

        BaseRole throwTarget = collision.gameObject.GetComponent<BaseRole>();

        if (throwTarget is null || throwTarget.objectType != ObjectType.Enemy || !throwTarget.isBeThrow)
        {
            return;
        }

        if (Mathf.Abs(pos.y - throwTarget.pos.y) > 0.1f)
        {
            return;
        }

        HurtStateArg hurtArg = HurtStateArg.Create();
        hurtArg.skillExp = 2;
        hurtArg.isChangeVelocity = true;
        hurtArg.changeVelocity = Vector2.zero;
        hurtArg.attackerDir = throwTarget.pos.x < pos.x ? 1 : -1;
        hurtArg.attackForce = SkillUtil.GetSmoonForce(hurtArg.attackerDir);
        hurtArg.attackerPos = throwTarget.pos;
        hurtArg.canBeDefense = false;
        hurtArg.isSwoon = true;
        hurtArg.attackerId = entityID;
        hurtArg.attackValue = Mathf.FloorToInt(entityAttribute.maxHealth * 0.1f);
        hurtArg.hurtSound = string.Empty;
        hurtArg.hurtAnim = string.Empty;
        hurtArg.isGroundHurt = true;
        OnHurtMsg(hurtArg);

        HurtStateArg targetHurt = HurtStateArg.Create();
        targetHurt.skillExp = 2;
        targetHurt.isChangeVelocity = true;
        targetHurt.changeVelocity = Vector2.zero;
        targetHurt.attackerDir = throwTarget.pos.x < pos.x ? 1 : -1;
        targetHurt.attackForce = SkillUtil.GetSmoonForce(-hurtArg.attackerDir);
        targetHurt.attackerPos = pos;
        targetHurt.canBeDefense = false;
        targetHurt.isSwoon = true;
        targetHurt.attackerId = entityID;
        targetHurt.attackValue = Mathf.FloorToInt(entityAttribute.maxHealth * 0.1f);
        targetHurt.hurtSound = string.Empty;
        targetHurt.hurtAnim = string.Empty;
        targetHurt.isGroundHurt = true;
        throwTarget.OnHurtMsg(targetHurt);
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.OnHit02));
    }
}