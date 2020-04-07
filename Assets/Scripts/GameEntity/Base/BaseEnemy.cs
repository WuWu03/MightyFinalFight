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
            m_Ctrl = gameObject.GetOrAddComponent<AvatarCtrl>();
            m_Ctrl.Init(StaticConfig.PlayerInfo[0].AttackWait, StaticConfig.PlayerInfo[0].Skills);
        }
        protected override void Update()
        {
            if (ResGO == null) return;
            m_Ctrl.Move(InputMgr.GetAxis());

            if (Input.GetKeyDown(KeyCode.T))
            {
                m_Ctrl.Attack(InputMgr.GetAxis());
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                m_Ctrl.Jump(InputMgr.GetAxis());
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                m_Ctrl.Skill(1007);
            }

            //if (Input.GetButtonDown"))
            //{
            //    m_Ctrl.Skill(1008);
            //}
            base.Update();
        }
    }
}
