using DragonBones;
using GameFrameWork;
using GameFrameWork.Fsm;
using System;
using GameFrameWork.Event;
using UnityEngine;

public abstract class BaseAvatar : BaseGravityObject
{
    private Fsm m_Fsm = null;
    public Fsm fsm
    {
        get
        {
            return m_Fsm;
        }
    }

    private HitTrigger m_HitTrigger = null;
    public HitTrigger hitTrigger
    {
        get
        {
            return m_HitTrigger;
        }
    }

    private DragonBones.AnimationState m_CurrAnimationState = null;
    private TriggerDatum m_CurrTriggerDatum = null;
    private float m_LastAnimTimeScale = 1f;
    private int m_LastTriggerFrameIndex = -1;
    private UnityArmatureComponent m_Animator;
    
    public Vector2 GetAnimTriggerSize(string animName, int frame = 0)
    {
        if (m_HitTrigger is null)
        {
            return Vector2.zero;
        }

        TriggerDatum triggerDatum = m_HitTrigger.GetTriggerData(animName);
        return triggerDatum != null ? triggerDatum.sizeList[frame] : Vector2.zero;
    }

    protected override void OnRelease()
    {
        m_Animator.animation?.Reset();
        FsmMgr.instance.ReleaseFsm(m_Fsm);
        m_HitTrigger = null;
        m_Animator = null;
        m_CurrAnimationState = null;
        m_CurrTriggerDatum = null;
        m_Fsm = null;
        base.OnRelease();
    }

    public void PlayAnimation(string animName, int playTimes = -1, float speed = 1f)
    {
        if (!HasAnimation(animName))
        {
            return;
        }

        if (IsAnimation(animName) && !IsCurrAnimationComplete())
        {
            return;
        }

        SetTrigger(animName);
        m_Animator.animation.timeScale = speed;
        m_CurrAnimationState = m_Animator.animation.Play(animName, playTimes);
    }

    public bool IsAnimation(string animName)
    {
        return m_CurrAnimationState != null && m_CurrAnimationState.name.Equals(animName);//m_Animator.animation.animationConfig.animation.Equals(animName);
    }

    public bool HasAnimation(string animName)
    {
        if (m_Animator is null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return false;
        }

        if (m_Animator.animation == null)
        {
            return false;
        }

        return !string.IsNullOrEmpty(animName) && m_Animator.animation.HasAnimation(animName);
    }

    public void StopAnimation(string animName)
    {
        if (m_Animator is null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return;
        }

        if (m_Animator.animation == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(animName))
        {
            return;
        }

        m_Animator.animation.Stop(animName);
    }

    public void PauseAnimation()
    {
        if (m_Animator is null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return;
        }

        if (m_Animator.animation == null)
        {
            return;
        }

        m_LastAnimTimeScale = m_Animator.animation.timeScale;
        m_Animator.animation.timeScale = 0f;
    }

    public void ResumeAnimation()
    {
        if (m_Animator is null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return;
        }

        if (m_Animator.animation != null && m_Animator.animation.timeScale <= 0)
        {
            m_Animator.animation.timeScale = m_LastAnimTimeScale;
        }
    }

    public bool IsCurrAnimationComplete()
    {
        return m_CurrAnimationState is { isCompleted: true };
    }

    public bool IsAllAnimationComplete()
    {
        if (m_Animator is null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return true;
        }

        if (m_Animator.animation != null)
        {
            return m_Animator.animation.isCompleted;
        }

        return true;
    }

    public void AddAnimationEvent(string eventName, ListenerDelegate<EventObject> listener)
    {
        m_Animator.AddEventListener(eventName, listener);
    }

    public void RemoveAnimationEvent(string eventName, ListenerDelegate<EventObject> listener)
    {
        m_Animator.RemoveEventListener(eventName, listener);
    }

    public bool IsAnyState(Type stateType1, Type stateType2 = null, Type stateType3 = null, Type stateType4 = null, Type stateType5 = null, Type stateType6 = null)
    {
        if (m_Fsm is not { isRunning: true })
        {
            return false;
        }

        bool condition1 = stateType1 != null && m_Fsm.currStateType == stateType1;
        bool condition2 = stateType2 != null && m_Fsm.currStateType == stateType2;
        bool condition3 = stateType3 != null && m_Fsm.currStateType == stateType3;
        bool condition4 = stateType4 != null && m_Fsm.currStateType == stateType4;
        bool condition5 = stateType5 != null && m_Fsm.currStateType == stateType5;
        bool condition6 = stateType6 != null && m_Fsm.currStateType == stateType6;
        return condition1 || condition2 || condition3 || condition4 || condition5 || condition6;
    }

    public bool IsAnyState(params Type[] stateTypes)
    {
        if (m_Fsm == null || !m_Fsm.isRunning || stateTypes == null || stateTypes.Length < 1)
        {
            return false;
        }

        foreach (var stateType in stateTypes)
        {
            if (m_Fsm.currStateType == stateType)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCurrState<T>() where T : BaseFsmState, new()
    {
        return m_Fsm.currStateType == typeof(T);
    }

    protected void AddState<T>() where T : BaseFsmState, new()
    {
        m_Fsm ??= FsmMgr.instance.CreateFsm(this, name);
        m_Fsm.AddState<T>();
    }

    public T GetState<T>() where T : BaseFsmState
    {
        return m_Fsm.GetState<T>();
    }

    protected void SetStateData<T>(GameFrameWorkEventArg stateData) where T : BaseFsmState
    {
        m_Fsm.SetStateData<T>(stateData);
    }

    public void ChangeState<T>(GameFrameWorkEventArg stateData = null) where T : BaseFsmState
    {
        m_Fsm.ChangeState<T>(stateData);
    }

    public void ChangeDefaultState()
    {
        m_Fsm.ChangeDefaultState();
    }

    public void RemoveState<T>() where T : BaseFsmState
    {
        m_Fsm.RemoveState<T>();
    }

    public void SetDefaultState<T>() where T : BaseFsmState
    {
        m_Fsm.SetDefaultState<T>();
    }

    protected void SetTrigger(string animName, int frameIndex = 0)
    {
        if (m_HitTrigger is null || frameIndex < 0)
        {
            return;
        }

        if (m_CurrTriggerDatum != null && m_CurrTriggerDatum.animName == animName && m_LastTriggerFrameIndex == frameIndex)
        {
            return;
        }

        if (m_CurrTriggerDatum == null || m_CurrTriggerDatum.animName != animName) 
        {
            m_CurrTriggerDatum = m_HitTrigger.GetTriggerData(animName);
        }

        if (m_CurrTriggerDatum != null)
        {
            SetCollider(m_CurrTriggerDatum.offestList[frameIndex], m_CurrTriggerDatum.sizeList[frameIndex]);
        }

        m_LastTriggerFrameIndex = frameIndex;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        m_Fsm?.Update(Time.deltaTime, Time.unscaledDeltaTime);

        if (m_CurrAnimationState != null && m_CurrAnimationState.isPlaying)
        {
            string animName = m_CurrAnimationState.name;
            uint frameCount = m_CurrAnimationState._animationData.frameCount;
            float duration = m_CurrAnimationState._animationData.duration;
            int frameIndex = (int)(m_CurrAnimationState.currentTime / duration * frameCount);
            SetTrigger(animName, frameIndex);
        }
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();
        m_Fsm?.LateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        m_Fsm?.FixedUpdate(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
    }

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_HitTrigger = go.GetComponent<HitTrigger>();

        if (m_Fsm != null && m_Fsm.HasDefaultState())
        {
            m_Fsm.Start();
        }
    }
}