using System.Collections.Generic;
using UnityEngine;
using WuWuFramework;
using WuWuFramework.ConfigData;
using WuWuFramework.Input;
using WuWuFramework.Localization;
using WuWuFramework.Utils;

public class PlayerMgr : Singleton<PlayerMgr>
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
            ComboMgr.instance.isRunning = value;
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

    public PlayerMgr()
    {
        MonoBehaviourMgr.instance.updateEvent += Update;
    }

    public void InitPlayer()
    {
        if (m_Player is not null)
        {
            return;
        }

        ComboMgr.instance.AddAfterTriggerEvent(ComboKey.A, AfterTriggerAttack);
        ComboMgr.instance.AddAfterTriggerEvent(ComboKey.X, AfterTriggerAttack);
        ComboMgr.instance.AddAfterTriggerEvent(ComboKey.B, AfterTriggerJump);
        ComboMgr.instance.AddAfterTriggerEvent(ComboKey.Y, AfterTriggerJump);

        m_Life = 99;
        m_ContinueCount = 3;
        m_Level = 1;
        m_Exp = 0;
        m_CanCtrl = true;
        m_RoleConfigData = GameEntry.configDataMgr.Get<RoleConfigData>().Get(m_SelectRoleId);
        m_LevelConfigData = GameEntry.configDataMgr.Get<LevelConfigData>().Get(OnLevelDataForEach);
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
                ComboMgr.instance.AddComboKeyEvent(skillData.Key.Keys, skillData.id, OnComboKeyEvent);
            }
        }

        CameraFollowMgr.instance.cameraFollow.SetTarget(m_Player.transform);
        ComboMgr.instance.getPreConditionEvent += GetPreCondition;
        ComboMgr.instance.isRunning = true;
    }

    public void Rebirth(Vector2 rebirthPos)
    {
        m_Life -= 1;
        GameEntry.uiMgr.Get<MainView>().presenter.SetPlayerLife(life);

        if (life < 1)
        {
            CameraFollowMgr.instance.cameraFollow.EndFollow();
            ComboMgr.instance.RemoveAllComboKeyEvent();

            m_Player.Release();
            m_Player = null;
            return;
        }

        m_Player.entityAttribute.ResetHealth();
        m_Player.RebirthState(rebirthPos);
    }

    public void AddExp(int value)
    {
        m_Exp += value;
        MainViewPresenter mainView = GameEntry.uiMgr.Get<MainView>().presenter;

        if (m_Exp >= m_LevelConfigData.exp)
        {
            m_Level++;
            m_Exp -= m_LevelConfigData.exp;
            m_LevelConfigData = GameEntry.configDataMgr.Get<LevelConfigData>().Get(OnLevelDataForEach);
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
        GameEntry.uiMgr.Get<MainView>().presenter.SetPlayerLife(m_Life);
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

    public override void Shutdown()
    {
        MonoBehaviourMgr.instance.updateEvent -= Update;
    }

    private bool OnLevelDataForEach(LevelConfigData levelConfigData)
    {
        return levelConfigData.roleId == m_SelectRoleId && levelConfigData.level == m_Level;
    }

    private void AfterTriggerAttack()
    {
        if (m_Player is null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return;
        }

        Vector2 axis = ComboMgr.instance.currLeftAxis;
        m_Player.Attack(axis);
    }

    private void AfterTriggerJump()
    {
        if (m_Player is null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return;
        }

        Vector2 axis = ComboMgr.instance.currLeftAxis;
        m_Player.Jump(axis, m_RoleConfigData.id != 1002);
    }

    private void CheckPlayerMove()
    {
        if (m_Player is null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0)
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
            Vector2 axis = ComboMgr.instance.currLeftAxis;
            axis.y *= 0.8f;
            m_Player.Move(axis);
        }
    }

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

    private int m_LanguageIndex;
    private bool m_IsStopBehaviour;

    private void Update(float deltaTime, float unscaledDeltaTime, float time, float AunscaledTime)
    {
        CheckPlayerMove();

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

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SceneEntityMgr.instance.CreateSceneItem(1001, m_Player.mapPos);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            m_Player.HurtState(new HurtStateArg() { attackerDir = 1, attackerId = 10011, attackValue = 1, isSwoon = true, attackForce = SkillUtil.GetSmoonForce() });
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
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
            float moveSpeed = 2;
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
        else if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            m_Player.HurtState(new HurtStateArg() { attackerDir = 1, attackerId = 10011, attackValue = 9999, isSwoon = false });
        }
    }
}