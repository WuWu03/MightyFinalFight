using DragonBones;
using GameFrameWork;
using GameFrameWork.Fsm;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using GameFrameWork.Utility;
using UnityEngine;

public class Barrel : BaseSceneItem, ICanBeHit
{
    public override bool CanPickUp
    {
        get
        {
            return false;
        }
    }

    public bool CanBeHit
    {
        get
        {
            return !IsDead && m_IsResComplete;
        }
    }

    public bool IsBeCatch
    {
        get
        {
            return false;
        }
    }


    public bool IsDead
    {
        get
        {
            return m_Health <= 0;
        }
    }

    public UnityArmatureComponent BarrelAnimator
    {
        get
        {
            return m_Animator;
        }
    }

    public BarrelData BarrelData
    {
        get
        {
            return m_BarrelData;
        }
    }

    public FsmMachine BarrelFsm
    {
        get
        {
            return m_FsmMachine;
        }
    }

    public bool IsBeThrow
    {
        get
        {
            return false;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_FsmMachine = new FsmMachine(this, this.GetType().Name);
        m_FsmMachine.AddState<BarrelIdle>();
        m_FsmMachine.AddState<BarrelMove>();
        m_FsmMachine.AddState<BarrelDrop>();
        m_FsmMachine.AddState<BarrelDead>();
    }

    public override void SetData(BaseSceneObjectData info)
    {
        base.SetData(info);
        m_BarrelData = info as BarrelData;
    }

    public override void Release()
    {
        base.Release();
        m_FsmMachine.ShutDown();
        m_FsmMachine = null;
        m_BarrelData = null;
    }

    public void OnHurtMsg(HurtData data)
    {
        SubHealth(data.AttackValue);
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/OnHit");

        if (IsDead)
        {
            SetTrigger(AnimName.Dead);
            GetState<BarrelDead>().AttackerDir = data.AttackerDir;
            ChangeState<BarrelDead>();
            SceneEntityMgr.Ins.CreateSceneItem(m_BarrelData.ItemId, m_MapPos);
        }
    }

    public void SetCatch(bool value) { }

    public void SetThrow(bool value) { }

    protected override void OnUpdate()
    {
        bool isOut = m_BarrelData.Dir > 0 ? IsOutVersionXRight(m_Pos.x) : IsOutVersionXLeft(m_Pos.x);
        if (isOut)
        {
            Release();
            return;
        }
        base.OnUpdate();
       
        m_FsmMachine.Update(Time.deltaTime, Time.unscaledDeltaTime);
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
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_Animator.animation.Play(AnimName.Idle, 0);
        if (!m_BarrelData.IsFloat)
        {
            if (m_BarrelData.MoveSpeed > 0)
            {
                SetTrigger(AnimName.Move);
                m_FsmMachine.Start<BarrelMove>();
                SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/Barrel");
            }
            else
            {
                SetTrigger(AnimName.Idle);
                m_FsmMachine.Start<BarrelIdle>();
            }
        }
        else
        {
            UpdatePos2(m_Pos.x, m_BarrelData.GroundY / 100f);
            SetBodyType(RigidbodyType2D.Dynamic);
        }
    }

    protected override void OnGround()
    {
        SetTrigger(AnimName.Drop);
        m_FsmMachine.Start<BarrelDrop>();
    }

    private void CheckStrike(GameObject go)
    {
        if (!m_IsResComplete || m_BarrelData.MoveSpeed <= 0 || IsDead) return;

        BaseRole role = go.GetComponent<BaseRole>();
        ICanBeHit hit = go.GetComponent<ICanBeHit>();

        if (role == null || hit == null || role.ObjectType != ObjectType.Player || role.IsAnyState(typeof(RoleAttack)))
        {
            return;
        }

        HurtData hurtData = HurtData.Create();
        hurtData.AttackerDir = -role.Dir;
        hit.OnHurtMsg(hurtData);
    }

    private void CheckThrow(GameObject go)
    {
        if (!m_IsResComplete || IsDead) return;

        BaseRole role = go.GetComponent<BaseRole>();
        ICanBeHit hit = go.GetComponent<ICanBeHit>();

        if (role == null || hit == null || role.ObjectType != ObjectType.Monster || !role.IsBeThrow)
        {
            return;
        }

        if (Mathf.Abs(role.Pos.y - m_Pos.y) > 0.1f) return;

        HurtData hurtData = HurtData.Create();
        hurtData.AttackerDir = -role.Dir;
        hurtData.AttackForce = SkillFactory.GetSmoonForce();
        hurtData.AttackerPos = m_Pos;
        hurtData.IsSwoon = true;
        hurtData.AttackerId = Id;
        hurtData.AttackValue = 1;
        hurtData.HurtAnim = string.Empty;
        hurtData.IsGroundHurt = false;

        OnHurtMsg(hurtData);
    }

    private void ChangeState<T>() where T : BaseFsmState
    {
        m_FsmMachine.ChangeState<T>();
    }

    private T GetState<T>() where T : BaseFsmState
    {
        return m_FsmMachine.GetState<T>();
    }

    private FsmMachine m_FsmMachine = null;
    private UnityArmatureComponent m_Animator = null;
    private BarrelData m_BarrelData = null;
}
