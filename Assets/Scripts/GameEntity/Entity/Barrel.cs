using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.FSM;
using GameFrameWork.Utils;
using UnityEngine;

public class Barrel : BaseAvatar, ICanBeHit
{
    public bool canBeHit
    {
        get
        {
            return !isDead && m_IsAssetLoadComplete;
        }
    }

    public bool isBeCatch
    {
        get
        {
            return false;
        }
    }


    public bool isDead
    {
        get
        {
            return m_EntityAttribute.health <= 0;
        }
    }


    public BarrelData barrelData
    {
        get
        {
            return m_BarrelData;
        }
    }

    public FiniteStateMachine barrelFSM
    {
        get
        {
            return m_FSM;
        }
    }

    public bool isBeThrow
    {
        get
        {
            return false;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        AddState<BarrelIdle>();
        AddState<BarrelMove>();
        AddState<BarrelDrop>();
        AddState<BarrelDead>();
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_BarrelData = data as BarrelData;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_BarrelData = null;
    }

    public bool IsHurtWillDie(int attackValue)
    {
        return m_EntityAttribute.health - attackValue <= 0;
    }

    public void OnHurtMsg(HurtStateData data)
    {
        m_EntityAttribute.SubHealth(data.attackValue);
        AudioMgr.instance.PlaySE(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Hurt));

        if (isDead)
        {
            SetTrigger(AnimName.Dead);
            ChangeState<BarrelDead>(data);
            SceneEntityMgr.instance.CreateSceneItem(m_BarrelData.itemId, m_MapPos);
        }

        ReferencePool.ReleaseReference(data);
    }

    public void SetCatch(bool value) { }

    public void SetThrow(bool value) { }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (m_BarrelData != null)
        {
            bool isOut = m_BarrelData.dir > 0 ? IsOutVersionXRight(m_Pos.x) : IsOutVersionXLeft(m_Pos.x);

            if (isOut)
            {
                Release();
                return;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        CheckThrow(collision.gameObject);
        CheckStrike(collision.gameObject);
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        CheckStrike(collision.gameObject);
    }

    //protected override void OnTriggerExit2D(Collider2D collision)
    //{
    //    CheckStrike(collision.gameObject);
    //}

    protected override void OnLoadAssetComplete(GameObject go, object[] param)
    {
        base.OnLoadAssetComplete(go, param);
        PlayAnimation(AnimName.Idle, 0);

        if (!m_BarrelData.isFloat)
        {
            if (m_BarrelData.moveSpeed >= 0)
            {
                SetTrigger(AnimName.Move);
                m_FSM.Start<BarrelMove>();
                AudioMgr.instance.PlaySE(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Barrel));
            }
            else
            {
                SetTrigger(AnimName.Idle);
                m_FSM.Start<BarrelIdle>();
            }
        }
        else
        {
            UpdatePosY(m_BarrelData.groundY / 100f);
            SetBodyType(RigidbodyType2D.Dynamic);
        }
    }

    protected override void OnGround()
    {
        SetTrigger(AnimName.Drop);
        ChangeState<BarrelDrop>();
    }

    private void CheckStrike(GameObject go)
    {
        if (!m_IsAssetLoadComplete || m_BarrelData.moveSpeed <= 0 || isDead)
        {
            return;
        }

        BaseHero player = PlayerMgr.instance.player;

        if (player == null || player.gameObject != go)
        {
            return;
        }
          
        bool isInRange = false;

        if (SkillUtil.IsRectangleCollide(player.bound, bound) && player.pos.y >= m_Pos.y)
        {
            Vector2 bsoLeftTop = new Vector2(bound.xMin, bound.yMax) - bound.center;
            float selectorAngle = Vector2.Angle(Vector2.left, bsoLeftTop.normalized);

            Vector2 target = (player.pos - m_Pos).normalized;
            Vector2 normal = m_Dir >= 0 ? Vector2.right : Vector2.left - Vector2.zero;
            float angle = Vector2.Angle(target, normal);

            if (angle <= selectorAngle / 2)
            {
                isInRange = true;
            }
        }

        if (isInRange)
        {
            HurtStateData hurtData = HurtStateData.Create();
            hurtData.attackerDir = m_Dir;
            hurtData.attackForce = SkillUtil.GetSmoonForce(m_Dir);
            hurtData.isSwoon = true;
            player.OnHurtMsg(hurtData);
        }
    }

    private void CheckThrow(GameObject go)
    {
        if (!m_IsAssetLoadComplete || isDead)
        {
            return;
        }

        BaseRole role = go.GetComponent<BaseRole>();

        if (role == null || role.objectType != ObjectType.Enemy || !role.isBeThrow || !role.canBeHit)
        {
            return;
        }

        if (Mathf.Abs(role.pos.y - m_Pos.y) > 0.1f)
        {
            return;
        }

        HurtStateData hurtData = HurtStateData.Create();
        hurtData.attackerDir = -role.dir;
        hurtData.attackForce = SkillUtil.GetSmoonForce();
        hurtData.attackerPos = m_Pos;
        hurtData.isSwoon = true;
        hurtData.attackerId = id;
        hurtData.attackValue = 1;
        hurtData.hurtAnim = string.Empty;
        hurtData.isGroundHurt = false;

        OnHurtMsg(hurtData);
    }

    private BarrelData m_BarrelData = null;
}
