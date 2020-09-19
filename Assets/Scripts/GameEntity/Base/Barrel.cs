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
        m_TriggerTargets = gameObject.GetOrAddComponent<TriggerTargets>();
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
        m_TriggerTargets.Release(); 
        m_FsmMachine = null;
        m_TriggerTargets = null;
        m_BarrelInfo = null;
        m_Timer = 0;
    }

    public void OnHurtMsg(HurtData data)
    {
        SubHealth(data.AttackValue);
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnHit");
        if (IsDead)
        {
            m_FsmMachine.GetState<BarrelDead>().AttackerDir = data.AttackerDir;
            m_FsmMachine.ChangeState<BarrelDead>();
            StageMgr.Ins.CreateSceneItem(m_BarrelInfo.Item, m_MapPos);
        }
    }

    public void SetCatch(bool value) { }

    protected override void Update()
    {
        bool isOut = m_BarrelInfo.Dir > 0 ? IsOutVersionXRight(m_Pos.x) : IsOutVersionXLeft(m_Pos.x);
        if (isOut)
        {
            Release();
            return;
        }
        base.Update();
        CheckStrike();
        m_FsmMachine.Update(Time.deltaTime, Time.unscaledDeltaTime);

    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_DBTrigger = m_ResGO.GetComponent<DBTrigger>();
        m_Animator.animation.Play(AnimName.Idle, 0);
        if (!m_BarrelInfo.IsFloat)
        {
            m_FsmMachine.Start<BarrelMove>();
            SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "Barrel");
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

    private void CheckStrike()
    {
        if (!ResComplete || m_TriggerTargets.Targets.Count < 1 || IsDead) return;
        if (m_Timer == 0) m_Timer = Time.time;
        if (Time.time - m_Timer < 0.3f) return;

        for (int i = 0; i < m_TriggerTargets.Targets.Count; i++)
        {
            BaseRole role = m_TriggerTargets.Targets[i].GetComponent<BaseRole>();
            if (role == null) continue;
            if (role.ObjectType != ObjectType.Player) continue;
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

        m_Timer = Time.time;
    }

    private float m_Timer = 0;
    private DBTrigger m_DBTrigger = null;
    private TriggerTargets m_TriggerTargets;
    private FsmMachine m_FsmMachine = null;
    private UnityArmatureComponent m_Animator = null;
    private BarrelInfo m_BarrelInfo = null;
}
