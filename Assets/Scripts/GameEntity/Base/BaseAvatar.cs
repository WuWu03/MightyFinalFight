using GameFrameWork.Fsm;
using System;
using UnityEngine;
using DragonBones;
using GameFrameWork.Log;

public abstract class BaseAvatar : BaseGravityObject
{
    public float MoveSpeed
    {
        get {return m_MoveSpeed; }
        set { m_MoveSpeed = value; }
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

    public UnityArmatureComponent ActorAnimator
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

    public DBTrigger DBTrigger
    {
        get
        {
            return m_DBTrigger;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_FsmMachine = FsmMachine.Create(this, this.GetType().Name);
    }

    public override void Release()
    {
        base.Release();
        m_FsmMachine.ShutDown();
        m_FsmMachine = null;
    }

    public Vector2 GetAnimTriggerSize(string animName)
    {
        if (m_DBTrigger == null) return Vector2.zero;
        TriggerData triggerData = m_DBTrigger.GetTriggerData(animName);

        if (triggerData != null)
        {
            return triggerData.Size;
        }

        return Vector2.zero;
    }

    protected void SetTrigger(string animName)
    {
        if (m_DBTrigger == null) return;

        TriggerData triggerData = m_DBTrigger.GetTriggerData(animName);

        if (triggerData != null)
            SetCollider(triggerData.Offest, triggerData.Size);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_FsmMachine.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        m_Animator = m_ResGO.GetComponent<UnityArmatureComponent>();
        m_DBTrigger = m_ResGO.GetComponent<DBTrigger>();
    }

    public void PlayAnimation(string animName, int playTimes = -1, float speed = 1f)
    {
        if (m_Animator == null)
        {
            GameFrameworkLog.LogError("Animator is invalid!");
            return;
        }

        if (IsAnim(animName))
        {
            return;
        }

        SetTrigger(animName);
        m_CurrAnimName = animName;
        m_Animator.animation.timeScale = speed;
        m_Animator.animation.Play(animName, playTimes);
    }

    public bool IsAnim(string animName)
    {
        bool result = m_CurrAnimName.Equals(animName);

        if (m_Animator.animation.isCompleted)
        {
            m_CurrAnimName = string.Empty;
        }

        return result;
    }

    public void StopAnimation(string animName = null)
    {
        if (string.IsNullOrEmpty(animName))
        {
            if (string.IsNullOrEmpty(m_CurrAnimName)) return;
            animName = m_CurrAnimName;
        }
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

    public bool IsCurrState<T>() where T : BaseFsmState, new()
    {
        return m_FsmMachine.CurrStateType == typeof(T);
    }

    protected void AddState<T>() where T : BaseFsmState, new()
    {
        m_FsmMachine.AddState<T>();
    }

    protected T GetState<T>() where T : BaseFsmState
    {
        return m_FsmMachine.GetState<T>();
    }

    protected void ChangeState<T>(bool isForce = false) where T : BaseFsmState
    {
        m_FsmMachine.ChangeState<T>(isForce);
    }

    protected void ChangeDefaultState()
    {
        m_FsmMachine.ChangeDefaultState();
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
    protected DBTrigger m_DBTrigger = null;
    protected FsmMachine m_FsmMachine = null;
    protected UnityArmatureComponent m_Animator;
}