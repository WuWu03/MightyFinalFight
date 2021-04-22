using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Input;
using GameFrameWork.Pool;
using GameFrameWork.Sound;
using GameFrameWork.UI;
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

    public LevelData.LevelInfo LevelData
    {
        get
        {
            return m_LevelData;
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
        m_LevelData = StaticConfig.LevelConfig.GetData(roleID).Levels[m_Level - 1];
        m_Player = SceneObjectPool.Ins.Get<BaseHero>("Player");
        m_CurrCtrl = m_Player.AddCtrl<BaseHeroCtrl>();
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetRes(string.Format("{0}/{1}", ResDefine.PREFAB_PATH, m_HeroData.AssetName));

        m_Player.InitInfo(new BaseRoleInfo()
        {
            Health = m_LevelData.Health,
            MaxHealth = m_LevelData.Health,
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
            WeaponAttackID = m_HeroData.WeaponAttackID,
            ThrowWeaponID = m_HeroData.ThrowWeaponID,
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
        CanContrl = true;
    }

    public void Rebirth(Vector2 rebirthPos)
    {
        m_Life -= 1;
        UIMgr.Ins.GetPanel<MainPanel>().SetPlayerLife(Life);

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
        MainPanel mainPanel = UIMgr.Ins.GetPanel<MainPanel>();
        if (m_EXP >= m_LevelData.EXP)
        {
            m_Level++;
            m_EXP -= m_LevelData.EXP;
            m_LevelData = StaticConfig.LevelConfig.GetData(m_HeroData.ID).Levels[m_Level - 1];
            m_Player.Health = m_LevelData.Health;
            m_Player.MaxHealth = m_LevelData.Health;
            mainPanel.SetPlayerHP(m_LevelData.Health, m_LevelData.Health, m_LevelData.HPBarWidth);
            mainPanel.SetPlayerLevel();
            SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/LevelUp");
        }
        mainPanel.SetPlayerExp(m_EXP, m_LevelData.EXP);
    }

    public void AddLife(int value)
    {
        m_Life += value;
        UIMgr.Ins.GetPanel<MainPanel>().SetPlayerLife(m_Life);
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
           m_CurrCtrl.Jump(InputMgr.GetAxis(), m_HeroData.ID != 2001);
        }
    }

    private bool GetComboCondition(int id)
    {
        SkillData skillData = StaticConfig.SkillConfig.GetData(id);
        bool a = SkillFactory.CheckStatus(skillData.SkillPrevConditions, m_Player);
        return a;
    }

    private void OnComboKeyEvent(int id, bool isTrigger)
    {
        m_CurrCtrl.Skill(id);
    }

    private BaseRoleCtrl m_CurrCtrl = null;
    private HeroData m_HeroData = null;
    private BaseHero m_Player = null;
    private LevelData.LevelInfo m_LevelData = null;

    private int m_Life = 0;
    private int m_EXP = 0;
    private int m_Level = 0;
    private int m_Continue = 0;
    private float m_CurrSpeed = 0f;
}
