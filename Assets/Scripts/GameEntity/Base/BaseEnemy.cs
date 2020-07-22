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

    public override void SetPos(Vector2 pos)
    {
        if (IsAnyState(typeof(RoleMove)))
        {
            if (!CanMove) return;
            bool isMapXCanMove = StageMgr.Ins.CanMovePosX(pos.x + Bound.width / 2 * m_Dir);
            bool isMapYCanMove = StageMgr.Ins.CanMovePosY(pos.y - Bound.height / 2);

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

    protected override void Update()
    {
        base.Update();

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
            data.HurtAnim = AnimName.Hurt2;
        }
        else
        {
            data.HurtAnim = Random.Range(0, 100) >= 50 ? AnimName.Hurt1 : AnimName.Hurt2;
        }

        int dir = data.AttackerPos.x > m_Pos.x ? -1 : 1;
        Vector3 pos = new Vector3(0.05f * dir * -m_Dir, 0, 0.1f * -m_Dir);
        EffectMgr.Ins.PlayEffect(PlayerMgr.Ins.HeroData.HitEffect, transform, pos, Vector3.zero, true, true, 0.1f);
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
        OnDead(m_EntityID);
        OnDead -= OnDead;
        OnDead = null;
    }
    protected BaseRoleCtrl m_AvatarCtrl = null;
}