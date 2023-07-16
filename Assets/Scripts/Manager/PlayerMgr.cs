using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Sound;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using System;
using UnityEngine;

public class PlayerMgr : BaseMgr<PlayerMgr>
{
    public BaseHero player
    {
        get
        {
            return m_Player;
        }
    }

    public RoleData roleData
    {
        get
        {
            return m_RoleData;
        }
    }

    public LevelData levelData
    {
        get
        {
            return m_LevelData;
        }
    }

    public int life
    {
        get
        {
            return m_Life;
        }
    }

    public int exp
    {
        get
        {
            return m_EXP;
        }
    }

    public int level
    {
        get
        {
            return m_Level;
        }
    }

    public int continueCount
    {
        get
        {
            return m_ContinueCount;
        }
    }

    public bool canContrl
    {
        get
        {
            return m_CanCtrl;
        }
        set
        {
            m_CanCtrl = value;
            InputMgr.instance.isRunning = value;
        }
    }

    public int selectRoleId
    {
        get
        {
            return m_SelectRoleId;
        }
        set
        {
            m_SelectRoleId = value;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        InputMgr.instance.AddAxis(AxisType.LeftAxis, "Horizontal", "Vertical");
        InputMgr.instance.AddKey(KeyType.A, "A");
        InputMgr.instance.AddKey(KeyType.B, "B");
        InputMgr.instance.AddKey(KeyType.X, "X", KeyType.A, true);
        InputMgr.instance.AddKey(KeyType.Y, "Y", KeyType.B, true);
        InputMgr.instance.AddKey(KeyType.Start, "Start");
        InputMgr.instance.AddKey(KeyType.Select, "Select");
        InputMgr.instance.AddKey(KeyType.LB, "LB");
        InputMgr.instance.AddKey(KeyType.RB, "RB");
        InputMgr.instance.AddKey(KeyType.LT, "LT");
        InputMgr.instance.AddKey(KeyType.RT, "RT");
    }

    public void InitPlayer()
    {
        if(m_Player != null)
        {
            return;
        }

        m_Life = 99;
        m_ContinueCount = 3;
        m_Level = 1;
        m_EXP = 0;

        m_RoleData = DataHelper.roleDatas.GetDataById(m_SelectRoleId);
        m_LevelData = DataHelper.levelDatas.GetSingDataByAttr("roleId=" + m_SelectRoleId + ",level=" + m_Level);
        m_Player = EntityMgr.instance.GetEntity<BaseHero>("Player");
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, m_RoleData.assetName));
        m_Player.SetLayer(LayerName.Unit);
        m_CurrCtrl = m_Player.AddCtrl<BaseHeroCtrl>();

        BaseRoleData roleData = ReferencePool.Acquire<BaseRoleData>();
        BaseHeroSkillData heroSkillData = ReferencePool.Acquire<BaseHeroSkillData>();
        EntityAttribute roleAttribute = ReferencePool.Acquire<EntityAttribute>();

        roleAttribute.health = m_LevelData.hpValue;
        roleAttribute.maxHealth = m_LevelData.hpValue;
        roleAttribute.attackSpeed = m_RoleData.attackSpeed;
        roleAttribute.attackValue = m_LevelData.attackValue;
        roleAttribute.defenseValue = m_LevelData.defenseValue;
        roleAttribute.criticalValue = m_LevelData.criticalValue;
        roleAttribute.jumpForce = m_LevelData.jumpForce;
        roleAttribute.moveSpeed = m_LevelData.moveSpeed;

        roleData.isCatchControl = m_RoleData.isCatchControl;

        heroSkillData.id = m_RoleData.id;
        heroSkillData.attackIds = m_RoleData.attactIds;
        heroSkillData.jumpAttackIds = m_RoleData.jumpAttackIds;
        heroSkillData.skillIds = m_RoleData.skillIds;
        heroSkillData.attackWait = m_RoleData.attackWait;
        heroSkillData.attackNextTime = m_RoleData.attackNextTime;
        heroSkillData.catchAttackID = m_RoleData.catchAttackId;
        heroSkillData.throwAttackID = m_RoleData.throwAttackId;
        heroSkillData.weaponAttackID = m_RoleData.weaponAttackId;
        heroSkillData.throwWeaponID = m_RoleData.throwWeaponId;

        m_Player.SetAttribute(roleAttribute);
        m_Player.SetData(roleData);
        m_CurrCtrl.SetData(heroSkillData);

        for (int i = 6; i < m_RoleData.skillIds.Length; i++)
        {
            SkillConfigData skillData = StaticConfig.SkillConfig.GetData(m_RoleData.skillIds[i]);

            if (skillData.Key.Keys.Length > 0 && skillData.Key.AddTrigger)
            {
                InputMgr.instance.AddComboKeyEvent(skillData.Key.Keys, skillData.Id, OnComboKeyEvent);
            }
        }

        m_CanCtrl = true;

        CameraMgr.instance.SetTarget(m_Player.transform);
        InputMgr.instance.getDirectionEvent = GetDirction;
        InputMgr.instance.afterTriggerEvent = AfterTrigger;
        InputMgr.instance.getPreconditonEvent = GetPreCondition;
        InputMgr.instance.isRunning = true;
    }

 
    public void Rebirth(Vector2 rebirthPos)
    {
        m_Life -= 1;
        UIMgr.instance.GetPanel<MainPanel>().SetPlayerLife(life);

        if (life < 1)
        {
            CameraMgr.instance.EndFollow();
            InputMgr.instance.RemoveAllComboKeyEvent();

            m_Player.Release();
            m_Player = null;
            m_CurrCtrl = null;
            return;
        }

        m_Player.entityAttribute.ResetHealth();
        m_Player.OnRebirthMsg(rebirthPos);
    }

    public void Jump(Vector2 dir,bool canChangeDir,bool isForceJump)
    {
        m_CurrCtrl.Jump(dir, canChangeDir, isForceJump);
    }

    public void AddExp(int value)
    {
        m_EXP += value;
        MainPanel mainPanel = UIMgr.instance.GetPanel<MainPanel>();

        if (m_EXP >= m_LevelData.exp)
        {
            m_Level++;
            m_EXP -= m_LevelData.exp;
            m_LevelData = DataHelper.levelDatas.GetSingDataByAttr("roleId=" + m_RoleData.id + ",level=" + m_Level);
            m_Player.entityAttribute.health = m_LevelData.hpValue;
            m_Player.entityAttribute.maxHealth = m_LevelData.hpValue;
            mainPanel.SetPlayerHP(m_LevelData.hpValue, m_LevelData.hpValue, m_LevelData.hpBarWidth);
            mainPanel.SetPlayerLevel();
            SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/LevelUp");
        }

        mainPanel.SetPlayerExp(m_EXP, m_LevelData.exp);
    }

    public void AddLife(int value)
    {
        m_Life += value;
        UIMgr.instance.GetPanel<MainPanel>().SetPlayerLife(m_Life);
    }

    public void AddContinue(int value)
    {
        m_ContinueCount += value;
    }

    public void SetSpeedZero(bool isZero)
    {
        if (m_CurrSpeed == 0)
        {
            m_CurrSpeed = m_Player.entityAttribute.moveSpeed;
        }

        m_Player.entityAttribute.moveSpeed = 0f;
    }

    public void RevertSpeed()
    {
        m_Player.entityAttribute.moveSpeed = m_CurrSpeed;
        m_CurrSpeed = 0f;
    }

    private float GetDirction()
    {
        if (m_Player == null)
        {
            return 1;
        }

        return m_Player.dir;
    }

    private bool AfterTrigger()
    {
        if (m_Player == null || m_CurrCtrl == null || !m_Player.isResComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return false;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_CurrCtrl.DeploySkill(1001004);
            return false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_CurrCtrl.DeploySkill(1001008);
            return false;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            StageMgr.instance.StageEnterNext();
            return false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_Player.OnHurtMsg(new HurtData() { attackerDir = 1, attackerId = 10011, attackValue = 1 });
            return false;
        }

        Vector2 asix = InputMgr.instance.GetAxis(AxisType.LeftAxis);
        bool result = asix.x != 0 || asix.y != 0;

        m_CurrCtrl.Move(asix);

        if (InputMgr.instance.GetKeyDown(KeyType.A, true) || InputMgr.instance.GetKeyDown(KeyType.X))
        {
            m_CurrCtrl.Attack(asix);
            result = true;
        }

        if (InputMgr.instance.GetKeyDown(KeyType.B, true) || InputMgr.instance.GetKeyDown(KeyType.Y))
        {
            m_CurrCtrl.Jump(asix, m_RoleData.id != 1002);
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
        m_CurrCtrl.DeploySkill(id);
    }

    private BaseHeroCtrl m_CurrCtrl = null;
    private RoleData m_RoleData = null;
    private BaseHero m_Player = null;
    private LevelData m_LevelData = null;

    private int m_Life = 0;
    private int m_EXP = 0;
    private int m_Level = 0;
    private int m_ContinueCount = 0;
    private int m_SelectRoleId = 0;
    private float m_CurrSpeed = 0f;
    private bool m_CanCtrl = false;
}