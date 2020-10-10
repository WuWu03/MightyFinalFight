using FrameWork.UI;
using UnityEngine;
using UnityEngine.Events;

public class BaseEnemy : BaseRole
{
    public override bool CanJump
    {
        get
        {
            return false;
        }
    }

    public event VoidParamT<int> OnDead;

    public override void Init(int id, string name)
    {
        base.Init(id, name);
    }

    public override void InitInfo(BaseSceneObjectInfo data)
    {
        base.InitInfo(data);
        BaseEnemyInfo baseEnemyInfo = data as BaseEnemyInfo;
        m_HurtAnim = baseEnemyInfo.HurtAnim;
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
        UIMgr.Ins.GetPanel<MainPanelCtrl>().SetEnemyHP(m_Health, m_MaxHealth, 400f);
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

        if (ResGO == null || m_Health <= 0) return;
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

    public override void Release()
    {
        base.Release();
        PlayerMgr.Ins.AddExp(m_SkillExp);
        OnDead(m_EntityID);
        OnDead -= OnDead;
        OnDead = null;
        m_SkillExp = 0;
    }

    private int m_SkillExp = 0;
    private string[] m_HurtAnim = null;
    protected BaseRoleCtrl m_AvatarCtrl = null;
}