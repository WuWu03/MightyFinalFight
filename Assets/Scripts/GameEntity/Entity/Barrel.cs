using DragonBones;
using GameFrameWork.Fsm;
using GameFrameWork.Sound;
using UnityEngine;

public class Barrel : BaseSceneItem, ICanBeHit
{
    public override bool canPickUp
    {
        get
        {
            return false;
        }
    }

    public bool canBeHit
    {
        get
        {
            return !isDead && m_IsResComplete;
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

    public FsmMachine barrelFsm
    {
        get
        {
            return m_FsmMachine;
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

    public override void SetData(BaseSceneObjectData info)
    {
        base.SetData(info);
        m_BarrelData = info as BarrelData;
    }

    public override void Release()
    {
        m_FsmMachine.ShutDown();
        m_FsmMachine = null;
        m_BarrelData = null;
        base.Release();
    }

    public bool IsHurtWillDie(int attackValue)
    {
        return m_EntityAttribute.health - attackValue <= 0;
    }

    public void OnHurtMsg(HurtData data)
    {
        m_EntityAttribute.SubHealth(data.attackValue);
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnHit");

        if (isDead)
        {
            SetTrigger(AnimName.Dead);
            GetState<BarrelDead>().attackerDir = data.attackerDir;
            ChangeState<BarrelDead>();
            SceneEntityMgr.instance.CreateSceneItem(m_BarrelData.itemId, m_MapPos);
        }
    }

    public void SetCatch(bool value) { }

    public void SetThrow(bool value) { }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        bool isOut = m_BarrelData.dir > 0 ? IsOutVersionXRight(m_Pos.x) : IsOutVersionXLeft(m_Pos.x);

        if (isOut)
        {
            Release();
            return;
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

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        CheckStrike(collision.gameObject);
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        PlayAnimation(AnimName.Idle, 0);

        if (!m_BarrelData.isFloat)
        {
            if (m_BarrelData.moveSpeed > 0)
            {
                SetTrigger(AnimName.Move);
                ChangeState<BarrelMove>();
                SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/Barrel");
            }
            else
            {
                SetTrigger(AnimName.Idle);
                ChangeState<BarrelIdle>();
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
        if (!m_IsResComplete || m_BarrelData.moveSpeed <= 0 || isDead)
        {
            return;
        }

        BaseRole role = go.GetComponent<BaseRole>();
        ICanBeHit hit = go.GetComponent<ICanBeHit>();

        if (role == null || hit == null || role.objectType != ObjectType.Player || role.IsAnyState(typeof(RoleAttack)))
        {
            return;
        }

        HurtData hurtData = HurtData.Create();
        hurtData.attackerDir = -role.dir;
        hit.OnHurtMsg(hurtData);
    }

    private void CheckThrow(GameObject go)
    {
        if (!m_IsResComplete || isDead)
        {
            return;
        }

        BaseRole role = go.GetComponent<BaseRole>();
        ICanBeHit hit = go.GetComponent<ICanBeHit>();

        if (role == null || hit == null || role.objectType != ObjectType.Monster || !role.isBeThrow)
        {
            return;
        }

        if (Mathf.Abs(role.pos.y - m_Pos.y) > 0.1f)
        {
            return;
        }

        HurtData hurtData = HurtData.Create();
        hurtData.attackerDir = -role.dir;
        hurtData.attackForce = SkillFactory.GetSmoonForce();
        hurtData.attackerPos = m_Pos;
        hurtData.isSwoon = true;
        hurtData.attackerId = id;
        hurtData.attackValue = 1;
        hurtData.hurtAnim = string.Empty;
        hurtData.isGroundHurt = false;

        OnHurtMsg(hurtData);
    }

    private UnityArmatureComponent m_ArmatureAnimator = null;
    private BarrelData m_BarrelData = null;
}
