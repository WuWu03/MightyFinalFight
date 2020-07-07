using FrameWork.Fsm;
using System;
using UnityEngine;
using FrameWork.GameEntity;
using FrameWork;

public abstract class BaseAvatar : BaseSceneObject
{
    public float MoveSpeed
    {
        get { return m_MoveSpeed; }
        set { m_MoveSpeed = value; }
    }
    public bool IsFloat
    {
        get
        {
            return m_Rigidbody.velocity.y >= 0 && m_Rigidbody.bodyType == RigidbodyType2D.Dynamic;
        }
    }
    public Vector2 MoveToPoint
    {
        get
        {
            return m_MoveToPoint;
        }
    }

    public Vector2 MoveDir
    {
        get
        {
            return m_MoveDir;
        }
    }

    public DragonBones.UnityArmatureComponent ActorAnimator
    {
        get
        {
            return m_Animator;
        }
    }

    public FsmMachine FsmMachine
    {
        get
        {
            return m_FsmMachine;
        }
    }

    public Rigidbody2D Rigidbody
    {
        get
        {
            return m_Rigidbody;
        }
    }

    public BoxCollider2D Collider
    {
        get
        {
            return m_Collider;
        }
    }

    public DBTrigger DBTrigger
    {
        get
        {
            return m_DBTrigger;
        }
    }

    public TriggerTargets TriggerTargets
    {
        get
        {
            return m_TriggerTargets;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_FsmMachine = FsmMachine.Create(this, string.Format("{0}Fsm", this.GetType().Name));
        m_TriggerTargets = gameObject.GetOrAddComponent<TriggerTargets>();

        m_Collider = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_Collider.isTrigger = true;

        m_Rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        m_Rigidbody.freezeRotation = true;
    }

    public override void Release()
    {
        base.Release();
        m_FsmMachine.ShutDown();
        m_TriggerTargets.Release();
        m_TriggerTargets = null;
        m_FsmMachine = null;
    }

    public void SetTrigger(string animName)
    {
        if (m_DBTrigger == null) return;

        TriggerData triggerData = m_DBTrigger.GetTriggerData(animName);

        if (triggerData != null)
        {
            m_Collider.size = triggerData.Size;
            m_Collider.offset = triggerData.Offest;
        }
    }

    protected override void Update()
    {
        m_FsmMachine.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        m_Animator = m_ResGO.GetComponent<DragonBones.UnityArmatureComponent>();
        m_DBTrigger = m_ResGO.GetComponent<DBTrigger>();
    }

    public void PlayAnimation(string animName, int playTimes = -1, float speed = 1f)
    {
        if (m_Animator == null)
        {
            Debug.LogError("Animator is invalid!");
            return;
        }

        if(IsAnim(animName))
        {
            return;
        }

        m_CurrAnimName = animName;
        m_Animator.animation.timeScale = speed;
        m_Animator.animation.Play(animName, playTimes);
    }

    public bool IsAnim(string animName)
    {
        if (m_Animator.animation.isCompleted)
        {
            m_CurrAnimName = string.Empty;
        }

        return m_CurrAnimName.Equals(animName);
    }

    public void StopAnimation(string animName)
    {
        m_Animator.animation.Stop(animName);
    }

    public bool IsPlayComplete()
    {
        return m_Animator.animation.isCompleted;
    }

    public bool IsAnyState(params Type[] stateTypes)
    {
        if (m_FsmMachine == null || !m_FsmMachine.IsRunning || stateTypes == null || stateTypes.Length < 1)
        {
            return false;
        }

        for (int i = 0; i < stateTypes.Length; i++)
        {
            if (m_FsmMachine.CurrStateType.Equals(stateTypes[i]))
            {
                return true;
            }
        }

        return false;
    }

    protected void AddState<T>() where T : BaseFsmState, new()
    {
        m_FsmMachine.AddState<T>();
    }

    protected T GetState<T>() where T : BaseFsmState
    {
        return m_FsmMachine.GetState<T>();
    }
    protected void ChangeState<T>() where T : BaseFsmState
    {
        m_FsmMachine.ChangeState<T>();
    }

    protected void RemoveState<T>() where T : BaseFsmState
    {
        m_FsmMachine.RemoveState<T>();
    }

    protected void SetDefaultState<T>() where T : BaseFsmState
    {
        m_FsmMachine.SetDefaultState<T>();
    }

    protected string m_CurrAnimName = string.Empty;
    protected float m_MoveSpeed = 0.8f;
    protected Vector2 m_MoveToPoint = Vector2.zero;
    protected Vector2 m_MoveDir = Vector2.zero;
    protected Rigidbody2D m_Rigidbody = null;
    protected TriggerTargets m_TriggerTargets;
    protected DBTrigger m_DBTrigger = null;
    protected BoxCollider2D m_Collider = null;
    protected FsmMachine m_FsmMachine = null;
    protected DragonBones.UnityArmatureComponent m_Animator;
}