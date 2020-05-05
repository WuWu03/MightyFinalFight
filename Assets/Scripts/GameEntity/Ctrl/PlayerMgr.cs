using FrameWork;
using FrameWork.Camera;
using FrameWork.Input;
using FrameWork.Pool;
using UnityEngine;

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
        m_Player = SceneObjectPool.Ins.Get<BaseHero>("Player");
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetRes(string.Format("{0}/{1}.prefab", ResDefine.MODEL_PATH, m_HeroData.AssetName));
        m_Player.InitValue(3, m_HeroData.AttackSpeed, 1, 1, m_HeroData.JumpForce, m_HeroData.MoveSpeed);
        m_CurrCtrl = m_Player.AddCtrl<HeroCtrl>();
        m_CurrCtrl.Init(m_HeroData.AttackWait, m_HeroData.Skills, 0.11f);

        InputMgr.Ins.GetDirFunc = delegate () { return m_Player.Dir; };

        for (int i = 6; i < m_HeroData.Skills.Length; i++)
        {
            SkillData skillData = StaticConfig.SkillConfig.GetData(m_HeroData.Skills[i]);
            if (skillData.Type == SkillData.SkillType.SkillAttack)
            {
                InputMgr.Ins.AddKeyEvent(skillData.SkillKeys, skillData.ID, OnComboKeyEvent);
            }
        }
        Life = 5;

        CameraMgr.Ins.SetTarget(m_Player.transform);
    }

    public void Rebirth()
    {
        Life -= 1;

        if (Life < 1)
        {
            CameraMgr.Ins.EndFollow();
            InputMgr.Ins.RemoveAllKeyEvent();

            m_Player.Release();
            m_Player = null;
            m_CurrCtrl = null;
            return;
        }

        m_Player.Health = 3;
        m_Player.OnRebirthMsg();
    }

    private void LateUpdate()
    {
        if (m_Player == null || m_CurrCtrl == null) return;

        if (m_Player.ResGO == null) return;
        if (m_Player.Health <= 0) return;

        m_CurrCtrl.Move(InputMgr.GetAxis());

        if (Input.GetButtonDown("A") || Input.GetButton("X"))
        {
            m_CurrCtrl.Attack(InputMgr.GetAxis());
        }

        if (Input.GetButtonDown("B") || Input.GetButton("Y"))
        {
            m_CurrCtrl.Jump(InputMgr.GetAxis());
        }
    }

    private void OnComboKeyEvent(int id, bool isTrigger)
    {
        m_CurrCtrl.Skill(id);
    }

    private AvatarCtrl m_CurrCtrl = null;
    private HeroData m_HeroData = null;
    private BaseHero m_Player = null;
}
