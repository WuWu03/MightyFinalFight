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
        m_AvatarCtrl = gameObject.GetOrAddComponent<AvatarCtrl>();
        m_AvatarCtrl.Init(null, new int[1] { 1001 }, 0.5f);
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

        if (ResGO == null) return;
        m_AvatarCtrl.Move(InputMgr.TestAxis());

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            m_AvatarCtrl.Attack(InputMgr.TestAxis());
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            m_AvatarCtrl.Jump(InputMgr.TestAxis());
        }
    }

    protected AvatarCtrl m_AvatarCtrl = null;
}