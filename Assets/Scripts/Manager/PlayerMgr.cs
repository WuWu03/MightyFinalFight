using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Sound;
using GameFrameWork.UI;
using GameFrameWork.Utility;
using System;
using UnityEngine;

public class PlayerMgr : BaseMgr<PlayerMgr>
{
    public BaseHero Player
    {
        get
        {
            return m_Player;
        }
    }

    public CharacterConfigData CharacterData
    {
        get
        {
            return m_CharacterData;
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
        get
        {
            return m_CanCtrl;
        }
        set
        {
            m_CanCtrl = value;
            InputMgr.Ins.IsRunning = value;
        }
    }

    public int SelectCharacterId
    {
        get
        {
            return m_SelectCharacterId;
        }
        set
        {
            m_SelectCharacterId = value;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        InputMgr.Ins.AddAxis(AxisType.LeftAxis, "Horizontal", "Vertical");
        InputMgr.Ins.AddKey(KeyType.A, "A");
        InputMgr.Ins.AddKey(KeyType.B, "B");
        InputMgr.Ins.AddKey(KeyType.X, "X", KeyType.A, true);
        InputMgr.Ins.AddKey(KeyType.Y, "Y", KeyType.B, true);
        InputMgr.Ins.AddKey(KeyType.Start, "Start");
        InputMgr.Ins.AddKey(KeyType.Select, "Select");
        InputMgr.Ins.AddKey(KeyType.LB, "LB");
        InputMgr.Ins.AddKey(KeyType.RB, "RB");
        InputMgr.Ins.AddKey(KeyType.LT, "LT");
        InputMgr.Ins.AddKey(KeyType.RT, "RT");
    }

    public void InitPlayer()
    {
        if(m_Player != null)
        {
            return;
        }

        m_Life = 99;
        m_Continue = 3;
        m_Level = 1;
        m_EXP = 0;

        m_CharacterData = StaticConfig.CharacterConfig.GetData(m_SelectCharacterId);
        m_LevelData = StaticConfig.LevelConfig.GetData(m_SelectCharacterId).Levels[m_Level - 1];
        m_Player = EntityMgr.Ins.GetEntity<BaseHero>("Player");
        m_CurrCtrl = m_Player.AddCtrl<BaseHeroCtrl>();
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, m_CharacterData.AssetName));
        m_Player.SetLayer(LayerName.Unit);

        BaseRoleData roleData = ReferencePool.Acquire<BaseRoleData>();
        BaseHeroSkillData heroSkillData = ReferencePool.Acquire<BaseHeroSkillData>();

        roleData.Health = m_LevelData.Health;
        roleData.MaxHealth = m_LevelData.Health;
        roleData.AttackSpeed = m_CharacterData.AttackSpeed;
        roleData.AttackValue = m_LevelData.AttackValue;
        roleData.DefenseValue = m_LevelData.DefenseValue;
        roleData.CriticalValue = m_LevelData.CriticalValue;
        roleData.JumpForce = m_LevelData.JumpForce;
        roleData.MoveSpeed = m_LevelData.MoveSpeed;
        roleData.CatchControl = m_CharacterData.CatchControl;

        heroSkillData.Id = m_CharacterData.Id;
        heroSkillData.AttackIds = m_CharacterData.AttackIDs;
        heroSkillData.JumpAttackIds = m_CharacterData.JumpAttackIDs;
        heroSkillData.SkillIds = m_CharacterData.Skills;
        heroSkillData.AttackWait = m_CharacterData.AttackWait;
        heroSkillData.AttackNextTime = m_CharacterData.AttackNextTime;
        heroSkillData.CatchAttackID = m_CharacterData.CatchAttackID;
        heroSkillData.ThrowAttackID = m_CharacterData.ThrowAttackID;
        heroSkillData.WeaponAttackID = m_CharacterData.WeaponAttackID;
        heroSkillData.ThrowWeaponID = m_CharacterData.ThrowWeaponID;

        m_Player.SetData(roleData);
        m_CurrCtrl.SetData(heroSkillData);

        for (int i = 6; i < m_CharacterData.Skills.Length; i++)
        {
            SkillConfigData skillData = StaticConfig.SkillConfig.GetData(m_CharacterData.Skills[i]);
            if (skillData.Key.Keys.Length > 0 && skillData.Key.AddTrigger)
            {
                InputMgr.Ins.AddComboKeyEvent(skillData.Key.Keys, skillData.Id, OnComboKeyEvent);
            }
        }

        m_CanCtrl = true;

        CameraMgr.Ins.SetTarget(m_Player.transform);

        InputMgr.Ins.GetDirection = GetDirction;
        InputMgr.Ins.AfterTrigger = AfterTrigger;
        InputMgr.Ins.GetPreconditon = GetPreCondition;
        InputMgr.Ins.IsRunning = true;
    }

 
    public void Rebirth(Vector2 rebirthPos)
    {
        m_Life -= 1;
        UIMgr.Ins.GetPanel<MainPanel>().SetPlayerLife(Life);

        if (Life < 1)
        {
            CameraMgr.Ins.EndFollow();
            InputMgr.Ins.RemoveAllComboKeyEvent();

            m_Player.Release();
            m_Player = null;
            m_CurrCtrl = null;
            return;
        }

        m_Player.SetHealth(m_Player.MaxHealth);
        m_Player.OnRebirthMsg(rebirthPos);
    }

    public void Jump(Vector2 dir,bool canChangeDir,bool isForceJump)
    {
        m_CurrCtrl.Jump(dir, canChangeDir, isForceJump);
    }

    public void AddExp(int value)
    {
        m_EXP += value;
        MainPanel mainPanel = UIMgr.Ins.GetPanel<MainPanel>();
        if (m_EXP >= m_LevelData.EXP)
        {
            m_Level++;
            m_EXP -= m_LevelData.EXP;
            m_LevelData = StaticConfig.LevelConfig.GetData(m_CharacterData.Id).Levels[m_Level - 1];
            m_Player.SetMaxHealth(m_LevelData.Health);
            m_Player.SetHealth(m_LevelData.Health);
            mainPanel.SetPlayerHP(m_LevelData.Health, m_LevelData.Health, m_LevelData.HPBarWidth);
            mainPanel.SetPlayerLevel();
            SoundMgr.Ins.PlaySound(ResDefine.AudioClipPath, "Sound/LevelUp");
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

    private float GetDirction()
    {
        if (m_Player == null)
        {
            return 1;
        }

        return m_Player.Dir;
    }

    private bool AfterTrigger()
    {
        if (m_Player == null || m_CurrCtrl == null || !m_Player.IsResComplete || m_Player.Health <= 0 || !m_CanCtrl)
        {
            return false;
        }

        Vector2 asix = InputMgr.Ins.GetAxis(AxisType.LeftAxis);
        bool result = asix.x != 0 || asix.y != 0;

        m_CurrCtrl.Move(asix);

        if (InputMgr.Ins.GetKeyDown(KeyType.A, true) || InputMgr.Ins.GetKeyDown(KeyType.X))
        {
            m_CurrCtrl.Attack(asix);
            result = true;
        }

        if (InputMgr.Ins.GetKeyDown(KeyType.B, true) || InputMgr.Ins.GetKeyDown(KeyType.Y))
        {
            m_CurrCtrl.Jump(asix, m_CharacterData.Id != 2001);
            result = true;
        }

        return result;
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

    private BaseHeroCtrl m_CurrCtrl = null;
    private CharacterConfigData m_CharacterData = null;
    private BaseHero m_Player = null;
    private LevelConfigData.LevelInfo m_LevelData = null;

    private int m_Life = 0;
    private int m_EXP = 0;
    private int m_Level = 0;
    private int m_Continue = 0;
    private int m_SelectCharacterId = 0;
    private float m_CurrSpeed = 0f;
    private bool m_CanCtrl = false;
}