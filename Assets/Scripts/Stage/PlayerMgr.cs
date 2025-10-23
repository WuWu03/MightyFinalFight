using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Input;
using GameFrameWork.Localization;
using GameFrameWork.Utils;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMgr : BaseMgr<PlayerMgr>
{
    private RoleConfigData m_RoleConfigData;
    private BaseHero m_Player;
    private LevelConfigData m_LevelConfigData;
    private int m_Life;
    private int m_Exp;
    private int m_Level;
    private int m_ContinueCount;
    private int m_SelectRoleId;
    private float m_CurrSpeed;
    private bool m_CanCtrl;
    
    public BaseHero player
    {
        get
        {
            return m_Player;
        }
    }

    public RoleConfigData roleConfigData
    {
        get
        {
            return m_RoleConfigData;
        }
    }

    public LevelConfigData levelConfigData
    {
        get
        {
            return m_LevelConfigData;
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
            return m_Exp;
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

    public bool canControl
    {
        get
        {
            return m_CanCtrl;
        }
        set
        {
            m_CanCtrl = value;
            GameEntry.inputMgr.isRunning = value;
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
        GameEntry.inputMgr.SetAxis(AxisType.LeftAxis, KeyCode.D, KeyCode.A, KeyCode.W, KeyCode.S);
        GameEntry.inputMgr.SetKey(KeyType.A, KeyCode.J, false, true);
        GameEntry.inputMgr.SetKey(KeyType.B, KeyCode.K, false, true);
        GameEntry.inputMgr.SetKey(KeyType.X, KeyType.A, KeyCode.U, true, true);
        GameEntry.inputMgr.SetKey(KeyType.Y, KeyType.B, KeyCode.I, true, true);
        GameEntry.inputMgr.SetKey(KeyType.Start, KeyCode.G, false, false);
        GameEntry.inputMgr.SetKey(KeyType.Select, KeyCode.H, false, false);
        GameEntry.inputMgr.AddAfterTriggerEvent(KeyType.A, AfterTriggerAttack);
        GameEntry.inputMgr.AddAfterTriggerEvent(KeyType.X, AfterTriggerAttack);
        GameEntry.inputMgr.AddAfterTriggerEvent(KeyType.B, AfterTriggerJump);
        GameEntry.inputMgr.AddAfterTriggerEvent(KeyType.Y, AfterTriggerJump);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        m_RoleConfigData = null;
        m_Player = null;
        m_LevelConfigData = null;
    }

    public void InitPlayer()
    {
        if (m_Player is not null)
        {
            return;
        }

        m_Life = 99;
        m_ContinueCount = 3;
        m_Level = 1;
        m_Exp = 0;
        m_CanCtrl = true;
        m_RoleConfigData = ConfigDataSheet.roleConfigDatas.GetConfigDataById(m_SelectRoleId);
        m_LevelConfigData = ConfigDataSheet.levelConfigDatas.GetSingConfigDataByAttr(StringUtil.Append("{roleId=", m_SelectRoleId.ToString(), ",level=", m_Level.ToString(),"}"));
        m_Player = GameEntry.entityMgr.GetEntity<BaseHero>("Player");
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, m_RoleConfigData.assetName));
        m_Player.SetLayer(LayerName.Unit);

        BaseRoleData roleData = BaseRoleData.Create();
        roleData.isCatchControl = m_RoleConfigData.isCatchControl;
        m_Player.SetData(roleData);

        EntityAttribute roleAttribute = EntityAttribute.Create();
        roleAttribute.health = m_LevelConfigData.hpValue;
        roleAttribute.maxHealth = m_LevelConfigData.hpValue;
        roleAttribute.attackSpeed = m_RoleConfigData.attackSpeed;
        roleAttribute.attackValue = m_LevelConfigData.attackValue;
        roleAttribute.defenseValue = m_LevelConfigData.defenseValue;
        roleAttribute.criticalValue = m_LevelConfigData.criticalValue;
        roleAttribute.jumpForce = m_LevelConfigData.jumpForce;
        roleAttribute.moveSpeed = m_LevelConfigData.moveSpeed;
        m_Player.SetAttribute(roleAttribute);

        BaseHeroSkillData heroSkillData = BaseHeroSkillData.Create();
        heroSkillData.roleId = m_RoleConfigData.id;
        heroSkillData.attackIds = m_RoleConfigData.attactIds;
        heroSkillData.jumpAttackIds = m_RoleConfigData.jumpAttackIds;
        heroSkillData.skillIds = m_RoleConfigData.skillIds;
        heroSkillData.attackWait = new[] { 0.2f, 0.4f, 1f };//m_RoleConfigData.attackWait;
        heroSkillData.catchAttackID = m_RoleConfigData.catchAttackId;
        heroSkillData.throwAttackID = m_RoleConfigData.throwAttackId;
        heroSkillData.weaponAttackID = m_RoleConfigData.weaponAttackId;
        heroSkillData.throwWeaponID = m_RoleConfigData.throwWeaponId;
        m_Player.SetSkillData(heroSkillData);

        for (int i = 6; i < m_RoleConfigData.skillIds.Length; i++)
        {
            SkillConfigData skillData = StaticConfig.SkillConfig.GetData(m_RoleConfigData.skillIds[i]);

            if (skillData.Key.Keys.Length > 0 && skillData.Key.AddTrigger)
            {
                GameEntry.inputMgr.AddComboKeyEvent(skillData.Key.Keys, skillData.id, OnComboKeyEvent);
            }
        }

        CameraMgr.instance.SetFollowTarget(m_Player.transform);
        GameEntry.inputMgr.getDirectionEvent += GetDirection;
        GameEntry.inputMgr.getPreConditonEvent += GetPreCondition;
        GameEntry.inputMgr.isRunning = true;
    }

    public void Rebirth(Vector2 rebirthPos)
    {
        m_Life -= 1;
        GameEntry.uiMgr.Get<MainView>().SetPlayerLife(life);
        
        if (life < 1)
        {
            CameraMgr.instance.EndFollow();
            GameEntry.inputMgr.RemoveAllComboKeyEvent();

            m_Player.Release();
            m_Player = null;
            return;
        }

        m_Player.entityAttribute.ResetHealth();
        m_Player.OnRebirthMsg(rebirthPos);
    }

    public void Jump(Vector2 dir, bool canChangeDir, bool isForceJump)
    {
        m_Player.Jump(dir, canChangeDir, isForceJump);
    }

    public void AddExp(int value)
    {
        m_Exp += value;
        MainView mainView = GameEntry.uiMgr.Get<MainView>();

        if (m_Exp >= m_LevelConfigData.exp)
        {
            m_Level++;
            m_Exp -= m_LevelConfigData.exp;
            m_LevelConfigData = ConfigDataSheet.levelConfigDatas.GetSingConfigDataByAttr(StringUtil.Append("{roleId=", m_RoleConfigData.id.ToString(), ",level=", m_Level.ToString(), "}"));
            m_Player.entityAttribute.health = m_LevelConfigData.hpValue;
            m_Player.entityAttribute.maxHealth = m_LevelConfigData.hpValue;
            mainView.SetPlayerHP(m_LevelConfigData.hpValue, m_LevelConfigData.hpValue, m_LevelConfigData.hpBarWidth);
            mainView.SetPlayerLevel();
            GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.LevelUp));
        }

        mainView.SetPlayerExp(m_Exp, m_LevelConfigData.exp);
    }

    public void AddLife(int value)
    {
        m_Life += value;
        GameEntry.uiMgr.Get<MainView>().SetPlayerLife(m_Life);
    }

    public void AddContinue(int value)
    {
        m_ContinueCount += value;
    }

    public void SetSpeedZero()
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

    private float GetDirection()
    {
        if (m_Player is null)
        {
            return 1;
        }

        return m_Player.dir;
    }

    private void AfterTriggerAttack()
    {
        if (m_Player is null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return;
        }

        Vector2 axis = GameEntry.inputMgr.GetAxis(AxisType.LeftAxis, true);
        m_Player.Attack(axis);
    }

    private void AfterTriggerJump()
    {
        if (m_Player is null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return;
        }

        Vector2 axis = GameEntry.inputMgr.GetAxis(AxisType.LeftAxis, true);
        m_Player.Jump(axis, m_RoleConfigData.id != 1002);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            m_LanguageIndex++;

            if (m_LanguageIndex > 2)
            {
                m_LanguageIndex = 0;
            }

            if (m_LanguageIndex == 0)
            {
                GameEntry.localizationMgr.ChangeLanguage(LanguageType.SimplifiedChinese);
            }
            else if (m_LanguageIndex == 1)
            {
                GameEntry.localizationMgr.ChangeLanguage(LanguageType.English);
            }
            else if (m_LanguageIndex == 2)
            {
                GameEntry.localizationMgr.ChangeLanguage(LanguageType.Japanese);
            }
        }

        if (m_Player == null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0)
        {
            return;
        }

        if (!m_CanCtrl)
        {
            m_Player.Move(Vector2.zero);
            return;
        }

        if (m_Player.canMove)
        {
            Vector2 leftAxis = GameEntry.inputMgr.GetAxis(AxisType.LeftAxis, true);
            Vector2 crossAxis = GameEntry.inputMgr.GetAxis(AxisType.CrossAxis, true);
            Vector2 axis = leftAxis != Vector2.zero ? leftAxis : crossAxis;
            axis.y *= 0.8f;
            m_Player.Move(axis);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SceneEntityMgr.instance.CreateSceneItem(1001, m_Player.mapPos);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            m_Player.OnHurtMsg(new HurtStateArg() { attackerDir = 1, attackerId = 10011, attackValue = 1, isSwoon = true,attackForce = SkillUtil.GetSmoonForce()});
            m_Player.OnHurtMsg(new HurtStateArg() { attackerDir = 1, attackerId = 10011, attackValue = 1, isSwoon = true, attackForce = SkillUtil.GetSmoonForce() });
            m_Player.OnHurtMsg(new HurtStateArg() { attackerDir = 1, attackerId = 10011, attackValue = 1, isSwoon = true, attackForce = SkillUtil.GetSmoonForce() });
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            //m_Player.OnHurtMsg(new HurtStateData() { attackerDir = 1, attackerId = 10011, attackValue = 9999, isSwoon = false});
            StageMgr.instance.StageEnterNext();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            List<BaseEnemy> enemies = SceneEntityMgr.instance.GetEnemies();

            foreach (var enemy in enemies)
            {
                if (m_IsStopBehaviour)
                {
                    enemy.Resume();
                }
                else
                {
                    enemy.Pause();
                }
            }

            m_IsStopBehaviour = !m_IsStopBehaviour;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            float dir = -1;
            int groundY = -40;
            int itemId = -1;
            bool isFloat = false;
            float moveSpeed = 0;
            SceneEntityMgr.instance.CreateBarrel(1, dir, groundY, itemId, isFloat, moveSpeed, new Vector2Int(m_Player.mapPos.x + 40, m_Player.mapPos.y));
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            m_Player.entityAttribute.attackValue = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            m_Player.entityAttribute.attackValue = 9999;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            if (m_Player.isPause)
            {
                m_Player.Resume();
            }
            else
            {
                m_Player.Pause();
            }
        }
    }

    private int m_LanguageIndex;
    private bool m_IsStopBehaviour;

    private bool GetPreCondition(int id)
    {
        SkillConfigData skillData = StaticConfig.SkillConfig.GetData(id);
        bool a = SkillUtil.CheckStatus(skillData.SkillPrevConditions, m_Player);
        return a;
    }

    private void OnComboKeyEvent(int id, bool isTrigger)
    {
        m_Player.DeploySkill(id);
    }
}