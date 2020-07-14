using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameWork;
using FrameWork.Input;
using FrameWork.UI;
using UnityEngine;

public class BaseEnemy : BaseRole
{
    public override bool CanJump
    {
        get
        {
            return false;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
    }

    public virtual void AddAI()
    {

    }

    public override void SetPos(Vector2 pos)
    {
        if (IsAnyState(typeof(RoleMove)))
        {
            if (!CanMove || !StageMgr.Ins.CanMovePos2(pos)) return;
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
        //m_CurrCtrl.Move(InputMgr.TestAxis());

        //if (Input.GetKeyDown(KeyCode.Keypad1))
        //{
        //    m_CurrCtrl.Attack(InputMgr.TestAxis());
        //}

        //if (Input.GetKeyDown(KeyCode.Keypad2))
        //{
        //    m_CurrCtrl.Jump(InputMgr.TestAxis());
        //}
    }

    public override void OnHurtMsg(HurtData data)
    {
        if(m_IsBeCatch)
        {
            data.HurtAnim = AnimName.Hurt2;
        }
        else
        {
            data.HurtAnim = UnityEngine.Random.Range(0, 100) >= 50 ? AnimName.Hurt1 : AnimName.Hurt2;
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

    protected BaseRoleCtrl m_AvatarCtrl = null;
}