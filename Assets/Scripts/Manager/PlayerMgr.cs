using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Sound;
using GameFrameWork.UI;
using GameFrameWork.Utility;
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

    public HeroConfigData HeroData
    {
        get
        {
            return m_HeroData;
        }
    }

    public LevelConfigData.LevelInfo LevelData
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

    public int SelectId
    {
        set
        {
            m_SelectId = value;
        }
    }

    public void InitPlayer()
    {
        if(m_Player != null)
        {
            return;
        }

        m_Life = 5;
        m_Continue = 3;
        m_Level = 1;
        m_EXP = 0;

        m_HeroData = StaticConfig.HeroConfig.GetData(m_SelectId);
        m_LevelData = StaticConfig.LevelConfig.GetData(m_SelectId).Levels[m_Level - 1];
        m_Player = EntityMgr.Ins.GetEntity<BaseHero>("Player");
        m_CurrCtrl = m_Player.AddCtrl<BaseHeroCtrl>();
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, m_HeroData.AssetName));

        BaseRoleData roleData = ReferencePool.Acquire<BaseRoleData>();
        BaseHeroSkillData heroSkillData = ReferencePool.Acquire<BaseHeroSkillData>();

        roleData.Health = m_LevelData.Health;
        roleData.MaxHealth = m_LevelData.Health;
        roleData.AttackSpeed = m_HeroData.AttackSpeed;
        roleData.AttackValue = m_LevelData.AttackValue;
        roleData.DefenseValue = m_LevelData.DefenseValue;
        roleData.CriticalValue = m_LevelData.CriticalValue;
        roleData.JumpForce = m_LevelData.JumpForce;
        roleData.MoveSpeed = m_LevelData.MoveSpeed;
        roleData.CatchControl = m_HeroData.CatchControl;

        heroSkillData.Id = m_HeroData.ID;
        heroSkillData.AttackIds = m_HeroData.AttackIDs;
        heroSkillData.JumpAttackIds = m_HeroData.JumpAttackIDs;
        heroSkillData.SkillIds = m_HeroData.Skills;
        heroSkillData.AttackWait = m_HeroData.AttackWait;
        heroSkillData.AttackNextTime = m_HeroData.AttackNextTime;
        heroSkillData.CatchAttackID = m_HeroData.CatchAttackID;
        heroSkillData.ThrowAttackID = m_HeroData.ThrowAttackID;
        heroSkillData.WeaponAttackID = m_HeroData.WeaponAttackID;
        heroSkillData.ThrowWeaponID = m_HeroData.ThrowWeaponID;

        m_Player.SetData(roleData);
        m_CurrCtrl.SetData(heroSkillData);

        InputMgr.Ins.GetDirection = delegate () { return m_Player.Dir; };
        InputMgr.Ins.AfterTrigger = AfterTrigger;
        InputMgr.Ins.GetPreconditon = GetPreCondition;

        for (int i = 6; i < m_HeroData.Skills.Length; i++)
        {
            SkillConfigData skillData = StaticConfig.SkillConfig.GetData(m_HeroData.Skills[i]);
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

        m_Player.SetHealth(10);
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
            m_Player.SetMaxHealth(m_LevelData.Health);
            m_Player.SetHealth(m_LevelData.Health);
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
        if (m_CurrSpeed == 0)
        {
            m_CurrSpeed = m_Player.MoveSpeed;
        }

        m_Player.MoveSpeed = 0f;
    }

    public void RevertSpeed()
    {
        m_Player.MoveSpeed = m_CurrSpeed;
        m_CurrSpeed = 0f;
    }

    private bool AfterTrigger()
    {
        if (m_Player == null || m_CurrCtrl == null || !m_Player.IsResComplete || m_Player.Health <= 0) return false;
        if (!CanContrl) return false;
        bool resutl = false;
        Vector2 asix = InputMgr.GetAxis();
        resutl = asix.x != 0 || asix.y != 0;

        m_CurrCtrl.Move(asix);

        if (Input.GetButtonDown("A") || Input.GetButton("X"))
        {
            m_CurrCtrl.Attack(InputMgr.GetAxis());
            resutl = true;
        }

        if (Input.GetButtonDown("B") || Input.GetButton("Y"))
        {
            m_CurrCtrl.Jump(InputMgr.GetAxis(), m_HeroData.ID != 2001);
            resutl = true;
        }
        return resutl;
    }

    private bool GetPreCondition(int id)
    {
        SkillConfigData skillData = StaticConfig.SkillConfig.GetData(id);
        bool a = SkillFactory.CheckStatus(skillData.SkillPrevConditions, m_Player);
        return a;
    }

    private void OnComboKeyEvent(int id, bool isTrigger)
    {
        m_CurrCtrl.Skill(id);
    }

    private BaseRoleCtrl m_CurrCtrl = null;
    private HeroConfigData m_HeroData = null;
    private BaseHero m_Player = null;
    private LevelConfigData.LevelInfo m_LevelData = null;

    private int m_Life = 0;
    private int m_EXP = 0;
    private int m_Level = 0;
    private int m_Continue = 0;
    private int m_SelectId = 0;
    private float m_CurrSpeed = 0f;
}
