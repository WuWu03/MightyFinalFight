using FrameWork;
using FrameWork.Camera;
using FrameWork.Input;
using FrameWork.Pool;
using UnityEngine;

namespace Runtime
{
    public class PlayerMgr : MonoSingleton<PlayerMgr>
    {
        public BaseHero Player
        {
            get
            {
                return m_Player;
            }
        }

        public int Life
        {
            get;
            private set;
        }

        public int CurrExp
        {
            get;
            private set;
        }

        public int CurrLevel 
        {
            get;
            private set;
        }

        public void InitPlayer(int roleID)
        {
            m_HeroData = StaticConfig.HeroConfig.GetData(roleID);
            m_Player = ObjectPool.Ins.Get<BaseHero>();
            m_Player.SetObjectType(ObjectType.Player);
            m_Player.SetRes(string.Format("{0}/{1}.prefab", ResDefine.MODEL_PATH, m_HeroData.AssetName));
            m_Player.InitValue(3, m_HeroData.AttackSpeed, 1, 1, m_HeroData.JumpForce, m_HeroData.MoveSpeed);
            m_CurrCtrl = m_Player.gameObject.GetOrAddComponent<AvatarCtrl>();
            m_CurrCtrl.Init(m_HeroData.AttackWait, m_HeroData.Skills,0.11f);
            Life = 5;
        }

        public void Rebirth()
        {
            Life -= 1;

            if(Life < 1)
            {
                Debug.Log("你死光了");
                CameraMgr.Ins.EndFollow();
                m_Player.Release();
                m_Player = null;
                m_CurrCtrl = null;
                return;
            }

            m_Player.Health = 3;
            m_Player.OnRebirthMsg();
        }

        private void Update()
        {
            if (m_Player == null || m_CurrCtrl == null) return;

            if (m_Player.ResGO == null) return;
            if (m_Player.Health <= 0) return;

            m_CurrCtrl.Move(InputMgr.GetAxis());

            if (Input.GetButtonDown("A") || Input.GetButton("X"))
            {
                m_CurrCtrl.Attack(InputMgr.GetAxis());
            }

            if (Input.GetButtonDown("B"))
            {
                m_CurrCtrl.Jump(InputMgr.GetAxis());
            }

            if(Input.GetButtonDown("Y"))
            {
                m_CurrCtrl.Skill(1007);
            }

            if (Input.GetButtonDown("LB"))
            {
                m_CurrCtrl.Skill(1008);
            }
        }

        private AvatarCtrl m_CurrCtrl = null;
        private Config.HeroData m_HeroData = null;
        private BaseHero m_Player = null;
    }
}
