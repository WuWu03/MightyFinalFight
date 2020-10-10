using DragonBones;
using FrameWork;
using FrameWork.Fsm;
using FrameWork.Sound;
using FrameWork.Timer;
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
            return !IsDead;
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

    public BarrelInfo BarrelInfo
    {
        get
        {
            return m_BarrelInfo;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_FsmMachine = new FsmMachine(this, string.Format("{0}Fsm", this.GetType().Name));
        m_FsmMachine.AddState<BarrelIdle>();
        m_FsmMachine.AddState<BarrelMove>();
        m_FsmMachine.AddState<BarrelDrop>();
        m_FsmMachine.AddState<BarrelDead>();
    }

    public override void InitInfo(BaseSceneObjectInfo info)
    {
        base.InitInfo(info);
        m_BarrelInfo = info as BarrelInfo;
        SetCollider(m_BarrelInfo.TriggerOffest, m_BarrelInfo.TriggerSize);
    }

    public override void Release()
    {
        base.Release();
        m_FsmMachine.ShutDown();
        m_FsmMachine = null;
        m_BarrelInfo = null;
    }

    public void OnHurtMsg(HurtData data)
    {
        SubHealth(data.AttackValue);
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnHit");
        if (IsDead)
        {
            m_FsmMachine.GetState<BarrelDead>().AttackerDir = data.AttackerDir;
            m_FsmMachine.ChangeState<BarrelDead>();
            SceneEntityMgr.Ins.CreateSceneItem(m_BarrelInfo.Item, m_MapPos);
        }
    }

    public void SetCatch(bool value) { }

    protected override void OnUpdate()
    {
        bool isOut = m_BarrelInfo.Dir > 0 ? IsOutVersionXRight(m_Pos.x) : IsOutVersionXLeft(m_Pos.x);
        if (isOut)
        {
            Release();
            return;
        }
        base.OnUpdate();
       
        m_FsmMachine.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckStrike(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckStrike(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckStrike(collision.gameObject);
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_Animator.animation.Play(AnimName.Idle, 0);
        if (!m_BarrelInfo.IsFloat)
        {
            if (m_BarrelInfo.MoveSpeed > 0)
            {
                m_FsmMachine.Start<BarrelMove>();
                SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "Barrel");
            }
            else
            {
                m_FsmMachine.Start<BarrelIdle>();
            }
        }
        else
        {
            UpdatePos2(m_Pos.x, m_BarrelInfo.GroundY / 100f);
            m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            OnGroundEvent.AddListener(OnGround);
        }
    }

    private void OnGround()
    {
        m_FsmMachine.Start<BarrelDrop>();
    }

    private void CheckStrike(GameObject go)
    {
        if (!ResComplete || m_BarrelInfo.MoveSpeed <= 0 || IsDead) return;
        BaseRole role = go.GetComponent<BaseRole>();
        if (role == null || !(role is ICanBeHit)) return;
        if (role.ObjectType != ObjectType.Player) return;
        if (role.IsAnyState(typeof(RoleAttack))) return;

        ICanBeHit hit = role as ICanBeHit;
        hit.OnHurtMsg(new HurtData()
        {
            AttackerDir = m_BarrelInfo.Dir,
            AttackForce = new Vector2(40, 150),
            AttackerPos = m_Pos,
            IsSwoon = true,
            AttackerID = ID,
            AttackValue = 1,
            HurtSound = "OnBlow",
            HurtAnim = string.Empty,
            IsGroundHurt = false,
        });
    }

    private FsmMachine m_FsmMachine = null;
    private UnityArmatureComponent m_Animator = null;
    private BarrelInfo m_BarrelInfo = null;
}
