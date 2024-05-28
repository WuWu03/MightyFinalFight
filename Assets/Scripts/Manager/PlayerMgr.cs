using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Audio;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
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

    public BaseHeroCtrl playerCtrl
    {
        get
        {
            return m_PlayerCtrl;
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
        InputMgr.instance.AddComboKey(KeyType.A, "A");
        InputMgr.instance.AddComboKey(KeyType.B, "B");
        InputMgr.instance.AddTurboComboKey(KeyType.X, "X", KeyType.A);
        InputMgr.instance.AddTurboComboKey(KeyType.Y, "Y", KeyType.B);
        InputMgr.instance.AddKey(KeyType.Start, "Start");
        InputMgr.instance.AddKey(KeyType.Select, "Select");
        InputMgr.instance.AddKey(KeyType.LB, "LB");
        InputMgr.instance.AddKey(KeyType.RB, "RB");
        InputMgr.instance.AddKey(KeyType.LT, "LT");
        InputMgr.instance.AddKey(KeyType.RT, "RT");

        InputMgr.instance.AddAfterTriggerEvent(KeyType.A, AfterTriggerAttack);
        InputMgr.instance.AddAfterTriggerEvent(KeyType.X, AfterTriggerAttack);
        InputMgr.instance.AddAfterTriggerEvent(KeyType.B, AfterTriggerJump);
        InputMgr.instance.AddAfterTriggerEvent(KeyType.Y, AfterTriggerJump);
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

        m_RoleConfigData = ConfigDataHelper.roleConfigDatas.GetConfigDataById(m_SelectRoleId);
        m_LevelConfigData = ConfigDataHelper.levelConfigDatas.GetSingConfigDataByAttr("roleId=" + m_SelectRoleId + ",level=" + m_Level);
        m_Player = EntityMgr.instance.GetEntity<BaseHero>("Player");
        m_Player.SetObjectType(ObjectType.Player);
        m_Player.SetAsset(PathUtil.FormatPath(ResDefine.PrefabPath, m_RoleConfigData.assetName));
        m_Player.SetLayer(LayerName.Unit);
        m_PlayerCtrl = m_Player.AddCtrl<BaseHeroCtrl>();

        BaseRoleData roleData = BaseRoleData.Create();
        BaseHeroSkillData heroSkillData = BaseHeroSkillData.Create();
        EntityAttribute roleAttribute = EntityAttribute.Create();

        roleAttribute.health = m_LevelConfigData.hpValue;
        roleAttribute.maxHealth = m_LevelConfigData.hpValue;
        roleAttribute.attackSpeed = m_RoleConfigData.attackSpeed;
        roleAttribute.attackValue = m_LevelConfigData.attackValue;
        roleAttribute.defenseValue = m_LevelConfigData.defenseValue;
        roleAttribute.criticalValue = m_LevelConfigData.criticalValue;
        roleAttribute.jumpForce = m_LevelConfigData.jumpForce;
        roleAttribute.moveSpeed = m_LevelConfigData.moveSpeed;

        roleData.isCatchControl = m_RoleConfigData.isCatchControl;

        heroSkillData.id = m_RoleConfigData.id;
        heroSkillData.attackIds = m_RoleConfigData.attactIds;
        heroSkillData.jumpAttackIds = m_RoleConfigData.jumpAttackIds;
        heroSkillData.skillIds = m_RoleConfigData.skillIds;
        heroSkillData.attackWait = new float[3] { 0.2f, 0.4f,1f };//m_RoleConfigData.attackWait;
        heroSkillData.attackNextTime = m_RoleConfigData.attackNextTime;
        heroSkillData.catchAttackID = m_RoleConfigData.catchAttackId;
        heroSkillData.throwAttackID = m_RoleConfigData.throwAttackId;
        heroSkillData.weaponAttackID = m_RoleConfigData.weaponAttackId;
        heroSkillData.throwWeaponID = m_RoleConfigData.throwWeaponId;

        m_Player.SetAttribute(roleAttribute);
        m_Player.SetData(roleData);
        m_Player.SetObjectType(ObjectType.Player);
        m_PlayerCtrl.SetData(heroSkillData);

        for (int i = 6; i < m_RoleConfigData.skillIds.Length; i++)
        {
            SkillConfigData skillData = StaticConfig.SkillConfig.GetData(m_RoleConfigData.skillIds[i]);

            if (skillData.Key.Keys.Length > 0 && skillData.Key.AddTrigger)
            {
                InputMgr.instance.AddComboKeyEvent(skillData.Key.Keys, skillData.id, OnComboKeyEvent);
            }
        }

        m_CanCtrl = true;

        CameraMgr.instance.SetFollowTarget(m_Player.transform);
        InputMgr.instance.getDirectionEvent = GetDirction;
        InputMgr.instance.getPreconditonEvent = GetPreCondition;
        InputMgr.instance.isRunning = true;
    }

    public void Rebirth(Vector2 rebirthPos)
    {
        m_Life -= 1;
        UIMgr.instance.Get<MainPanel>().SetPlayerLife(life);

        if (life < 1)
        {
            CameraMgr.instance.EndFollow();
            InputMgr.instance.RemoveAllComboKeyEvent();

            m_Player.Release();
            m_Player = null;
            m_PlayerCtrl = null;
            return;
        }

        m_Player.entityAttribute.ResetHealth();
        m_Player.OnRebirthMsg(rebirthPos);
    }

    public void Jump(Vector2 dir,bool canChangeDir,bool isForceJump)
    {
        m_PlayerCtrl.Jump(dir, canChangeDir, isForceJump);
    }

    public void AddExp(int value)
    {
        m_EXP += value;
        MainPanel mainPanel = UIMgr.instance.Get<MainPanel>();

        if (m_EXP >= m_LevelConfigData.exp)
        {
            m_Level++;
            m_EXP -= m_LevelConfigData.exp;
            m_LevelConfigData = ConfigDataHelper.levelConfigDatas.GetSingConfigDataByAttr("roleId=" + m_RoleConfigData.id + ",level=" + m_Level);
            m_Player.entityAttribute.health = m_LevelConfigData.hpValue;
            m_Player.entityAttribute.maxHealth = m_LevelConfigData.hpValue;
            mainPanel.SetPlayerHP(m_LevelConfigData.hpValue, m_LevelConfigData.hpValue, m_LevelConfigData.hpBarWidth);
            mainPanel.SetPlayerLevel();
            AudioMgr.instance.PlaySE(ResDefine.AudioClipPath, SoundName.LevelUp);
        }

        mainPanel.SetPlayerExp(m_EXP, m_LevelConfigData.exp);
    }

    public void AddLife(int value)
    {
        m_Life += value;
        UIMgr.instance.Get<MainPanel>().SetPlayerLife(m_Life);
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

    private void AfterTriggerAttack()
    {
        if (m_Player == null || m_PlayerCtrl == null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return;
        }

        Vector2 asix = InputMgr.instance.GetAxis(AxisType.LeftAxis);
        m_PlayerCtrl.Attack(asix);
    }

    private void AfterTriggerJump()
    {
        if (m_Player == null || m_PlayerCtrl == null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return;
        }

        Vector2 asix = InputMgr.instance.GetAxis(AxisType.LeftAxis);
        m_PlayerCtrl.Jump(asix, m_RoleConfigData.id != 1002);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_Player == null || m_PlayerCtrl == null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return;
        }

        if(m_Player.canMove)
        {
            Vector2 asix = InputMgr.instance.GetAxis(AxisType.LeftAxis);
            asix.y *= 0.8f;
            m_PlayerCtrl.Move(asix);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            float dir = -1;
            int groundY = -40;
            int itemId = -1;
            bool isFloat = false;
            float moveSpeed = 0;
            SceneEntityMgr.instance.CreateBarrel(1, dir, groundY, itemId, isFloat, moveSpeed, new Vector2Int(m_Player.mapPos.x + 40, m_Player.mapPos.y));
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneEntityMgr.instance.CreateSceneItem(1001, m_Player.mapPos);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
           m_Player.OnHurtMsg(new HurtStateData() { attackerDir = 1, attackerId = 10011, attackValue = 1, isSwoon = true });
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            UIMgr.instance.Open<RoleSelectPanel>();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            UIMgr.instance.Close<RoleSelectPanel>();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            StageMgr.instance.StageEnterNext();
        }
    }

    private bool AfterTrigger()
    {
        if (m_Player == null || m_PlayerCtrl == null || !m_Player.isAssetLoadComplete || m_Player.entityAttribute.health <= 0 || !m_CanCtrl)
        {
            return false;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {

            return false;
        }

        Vector2 asix = InputMgr.instance.GetAxis(AxisType.LeftAxis);
        bool result = asix.x != 0 || asix.y != 0;

        asix.y *= 0.8f;
        m_PlayerCtrl.Move(asix);

        if (InputMgr.instance.GetKeyDown(KeyType.A, true) || InputMgr.instance.GetKeyDown(KeyType.X))
        {
            m_PlayerCtrl.Attack(asix);
            result = true;
        }

        if (InputMgr.instance.GetKeyDown(KeyType.B, true) || InputMgr.instance.GetKeyDown(KeyType.Y))
        {
            m_PlayerCtrl.Jump(asix, m_RoleConfigData.id != 1002);
            result = true;
        }

        return result;
    }

    private bool GetPreCondition(int id)
    {    
        SkillConfigData skillData = StaticConfig.SkillConfig.GetData(id);
        bool a = SkillUtil.CheckStatus(skillData.SkillPrevConditions, m_Player);
        return a;
    }

    private void OnComboKeyEvent(int id, bool isTrigger)
    {
        m_PlayerCtrl.DeploySkill(id);
    }

    private BaseHeroCtrl m_PlayerCtrl = null;
    private RoleConfigData m_RoleConfigData = null;
    private BaseHero m_Player = null;
    private LevelConfigData m_LevelConfigData = null;

    private int m_Life = 0;
    private int m_EXP = 0;
    private int m_Level = 0;
    private int m_ContinueCount = 0;
    private int m_SelectRoleId = 0;
    private float m_CurrSpeed = 0f;
    private bool m_CanCtrl = false;
}