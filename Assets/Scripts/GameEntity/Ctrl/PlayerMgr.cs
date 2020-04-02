using FrameWork;
using FrameWork.Input;
using FrameWork.Pool;
using UnityEngine;

namespace Runtime
{
    public class PlayerMgr : MonoSingleton<PlayerMgr>
    {
        public string OwnerName
        {
            get
            {
                return "Player";
            }
        }

        public BaseHero Player
        {
            get
            {
                return m_Player;
            }
        }

        public PlayerInfo PlayerInfo
        {
            get;
            set;
        }

        protected override void Awake()
        {
            base.Awake();
            GameObject.DontDestroyOnLoad(gameObject);
        }

        public void InitPlayer(int roleID)
        {
            PlayerInfo = StaticConfig.PlayerInfo[roleID];
            m_Player = ObjectPool.Ins.Get<BaseHero>(OwnerName);
            m_Player.SetObjectType(ObjectType.Player);
            m_Player.SetRes(string.Format("{0}/{1}.prefab", ResDefine.MODEL_PATH, PlayerInfo.ResName));
            m_Player.Health = PlayerInfo.Health;
            m_Player.JumpForce = PlayerInfo.JumpForce;
            m_CurrCtrl = m_Player.gameObject.GetOrAddComponent<AvatarCtrl>();
            m_CurrCtrl.Init(PlayerInfo.AttackWait, PlayerInfo.Skills);
        }

        protected override void Update()
        {
            if (m_Player == null || m_CurrCtrl == null) return;

            if (m_Player.ResGO == null) return;
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

            if(Input.GetButtonDown("LB"))
            {
                m_CurrCtrl.Skill(1008);
            }

            base.Update();
        }

        private AvatarCtrl m_CurrCtrl = null;
        private BaseHero m_Player = null;
    }
}
