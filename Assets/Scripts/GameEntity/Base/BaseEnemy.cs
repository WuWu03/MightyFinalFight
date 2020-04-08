using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameWork;
using FrameWork.Input;
using UnityEngine;

namespace Runtime
{
    public class BaseEnemy : BaseRole
    {
        public override void Init(int id, string name)
        {
            base.Init(id, name);
            m_AvatarCtrl = gameObject.GetOrAddComponent<AvatarCtrl>();
            m_AvatarCtrl.Init(null, new int[1] { 1001 },0.5f);
        }
        protected override void Update()
        {
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

            base.Update();
        }

        protected AvatarCtrl m_AvatarCtrl = null;
    }
}
