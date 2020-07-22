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

    public HeroData HeroData
    {
        get
        {
            return m_HeroData;
        }
    }

    public int Life
    {
        get
        {
            return m_Life;
        }
    }

    public int EXP
    {
        get
        {
            return m_EXP;
        }
    }

    public int Level
    {
        get
        {
            return m_Level;
        }
    }

    public int Continue
    {
        get
        {
            return m_Continue;
        }
    }

    public bool CanContrl
    {
        get;
        set;
    }

    public void InitPlayer(int roleID)
    {
        m_Life = 5;
        m_Continue = 3;
        m_Level = 1;
        m_EXP = 0;

        m_HeroData = StaticConfig.HeroConfig.GetData(roleID);
        m_Player = SceneObjectPool.Ins.Get<BaseHero>("Player");
        m_CurrCtrl = m_Player.AddCtrl<BaseHeroCtrl>();
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetRes(string.Format("{0}/{1}", ResDefine.PREFAB_PATH, m_HeroData.AssetName));

        m_Player.InitInfo(new BaseRoleInfo()
        {
            Health = 10,
            MaxHealth = 10,
            AttackSpeed = m_HeroData.AttackSpeed,
            AttackValue = 1,
            Defense = 1,
            JumpForce = m_HeroData.JumpForce,
            MoveSpeed = m_HeroData.MoveSpeed
        });
   
        m_CurrCtrl.InitData(new BaseHeroSkillInfo()
        {
            ID = m_HeroData.ID,
            AttackIDs = m_HeroData.AttackIDs,
            JumpAttackIDs = m_HeroData.JumpAttackIDs,
            Skills = m_HeroData.Skills,
            AttackWait = m_HeroData.AttackWait,
            AttackNextTime = m_HeroData.AttackNextTime,
            CatchAttackID = m_HeroData.CatchAttackID,
            ThrowAttackID = m_HeroData.ThrowAttackID,
            WeaponAttackID = 1012,
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
 
        CameraMgr.Ins.SetTarget(m_Player.transform);
        TaskMgr.Ins.AcceptTask(1001);
        CanContrl = true;
    }

    public void Rebirth(Vector2 rebirthPos)
    {
        m_Life -= 1;
        UIMgr.Ins.GetPanel<MainPanelCtrl>().SetPlayerLife(Life);

        if (Life < 1)
        {
            CameraMgr.Ins.EndFollow();
            InputMgr.Ins.RemoveAllKeyEvent();

            m_Player.Release();
            m_Player = null;
            m_CurrCtrl = null;
            return;
        }

        m_Player.Health = 10;
        m_Player.OnRebirthMsg(rebirthPos);
    }

    public void AddExp(int value)
    {
        m_EXP += value;
    }

    public void AddLife(int value)
    {
        m_Life += value;
    }

    public void AddContinue(int value)
    {
        m_Continue += value;
    }

    public void SetSpeedZero(bool isZero)
    {
        if (isZero)
        {
            if(m_CurrSpeed == 0)
                m_CurrSpeed = m_Player.MoveSpeed;
            m_Player.MoveSpeed = 0f;
        }
        else
        {
            m_Player.MoveSpeed = m_CurrSpeed;
            m_CurrSpeed = 0f;        
        }
    }

    private void Control()
    {
        if (m_Player == null || m_CurrCtrl == null || !m_Player.ResComplete || m_Player.Health <= 0) return;
        if (!CanContrl) return;

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

    private int m_Life = 0;
    private int m_EXP = 0;
    private int m_Level = 0;
    private int m_Continue = 0;
    private float m_CurrSpeed = 0f;
}
