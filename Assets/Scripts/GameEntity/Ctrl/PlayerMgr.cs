using FrameWork;
using FrameWork.Camera;
using FrameWork.Input;
using FrameWork.Pool;
using FrameWork.UI;
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
        m_CurrCtrl = m_Player.AddCtrl<BaseHeroCtrl>();
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetRes(string.Format("{0}/{1}.prefab", ResDefine.MODEL_PATH, m_HeroData.AssetName));

        m_Player.InitData(new BaseRoleData()
        {
            Health = 10,
            MaxHealth = 10,
            AttackSpeed = m_HeroData.AttackSpeed,
            AttackValue = 1,
            Defense = 1,
            JumpForce = m_HeroData.JumpForce,
            MoveSpeed = m_HeroData.MoveSpeed
        });
   
        m_CurrCtrl.InitData(new BaseHeroSkillData()
        {
            AttackIDs = m_HeroData.AttackIDs,
            JumpAttackIDs = m_HeroData.JumpAttackIDs,
            Skills = m_HeroData.Skills,
            AttackWait = m_HeroData.AttackWait,
            AttackNextTime = m_HeroData.AttackNextTime,
            CatchAttackID = m_HeroData.CatchAttackID,
            ThrowAttackID = m_HeroData.ThrowAttackID,
        });

        InputMgr.Ins.GetDirFunc = delegate () { return m_Player.Dir; };
        InputMgr.Ins.AfterTriggeFunc = Control;
        InputMgr.Ins.GetPreconditonFunc = GetComboCondition;

        for (int i = 6; i < m_HeroData.Skills.Length; i++)
        {
            SkillData skillData = StaticConfig.SkillConfig.GetData(m_HeroData.Skills[i]);
            if (skillData.Key.Keys.Length > 0 && skillData.Key.AddTrigger)
            {
                InputMgr.Ins.AddKeyEvent(skillData.Key.Keys, skillData.ID, OnComboKeyEvent);
            }
        }

        Life = 5;
        
        CameraMgr.Ins.SetTarget(m_Player.transform);
    }

    public void Rebirth(Vector2 rebirthPos)
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
        m_Player.OnRebirthMsg(rebirthPos);
        UIMgr.Ins.GetPanel<MainPanelCtrl>().SetPlayerLife(Life);
    }

    private void Control()
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

    private bool GetComboCondition(int id)
    {
        SkillData skillData = StaticConfig.SkillConfig.GetData(id);
        return SkillFactory.CheckStatus(skillData.SkillPrevConditions, m_Player);
    }

    private void OnComboKeyEvent(int id, bool isTrigger)
    {
        if (m_Player.IsCatch) return;
        m_CurrCtrl.Skill(id);
    }

    private BaseRoleCtrl m_CurrCtrl = null;
    private HeroData m_HeroData = null;
    private BaseHero m_Player = null;
}
