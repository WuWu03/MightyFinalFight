using DragonBones;
using GameFrameWork;
using GameFrameWork.Fsm;
using System;
using UnityEngine;

public abstract class BaseAvatar : BaseGravityObject
{
    public UnityArmatureComponent armatureAnimator
    {
        get
        {
            return m_Animator;
        }
    }

    public HitTrigger hitTrigger
    {
        get
        {
            return m_HitTrigger;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
    }

    public Vector2 GetAnimTriggerSize(string animName,int frame = 0)
    {
        if (m_HitTrigger == null)
        {
            return Vector2.zero;
        }

        TriggerData triggerData = m_HitTrigger.GetTriggerData(animName);

        if (triggerData != null)
        {
            return triggerData.sizeList[frame];
        }

        return Vector2.zero;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_Animator.animation?.Reset();
        FsmMgr.instance.ReleaseFsm(m_Fsm);
        m_HitTrigger = null;
        m_Animator = null;
        m_CurrAnimName = string.Empty;
        m_LastTriggerAnimName = string.Empty;
        m_Fsm = null;
    }

    public void PlayAnimation(string animName, int playTimes = -1, float speed = 1f)
    {
        if (!HasAnimation(animName))
        {
            return;
        }

        if (IsAnimation(animName))
        {
            if (!m_Animator.animation.isCompleted)
            {
                return; 
            }

            m_CurrAnimName = string.Empty;
        }

        SetTrigger(animName);

        m_CurrAnimName = animName;
        m_Animator.animation.timeScale = speed;
        m_Animator.animation.Play(animName, playTimes);
    }

    public bool IsAnimation(string animName)
    {
        if (m_Animator == null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return false;
        }

        bool result = m_CurrAnimName.Equals(animName);

        if (m_Animator.animation != null && m_Animator.animation.isCompleted)
        {
            m_CurrAnimName = string.Empty;
        }

        return result;
    }

    public bool HasAnimation(string animName)
    {
        if (m_Animator == null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return false;
        }

        if (m_Animator.animation == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(animName))
        {
            return m_Animator.animation.HasAnimation(animName);
        }

        return false;
    }

    public void StopAnimation(string animName = null)
    {
        if (m_Animator == null)
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
            if (string.IsNullOrEmpty(m_CurrAnimName))
            {
                return;
            }

            animName = m_CurrAnimName;
        }

        m_Animator.animation.Stop(animName);
    }

    public void PauseAnimation()
    {
        if (m_Animator == null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return;
        }

        if(m_Animator.animation == null)
        {
            return;
        }

        m_LastAnimTimeScale = m_Animator.animation.timeScale;
        m_Animator.animation.timeScale = 0f;
    }

    public void ResumeAnimation()
    {
        if (m_Animator == null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return;
        }

        if (m_Animator.animation != null)
        {
            m_Animator.animation.timeScale = m_LastAnimTimeScale;
        }
    }

    public bool IsPlayComplete()
    {
        if (m_Animator == null)
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
        if (m_Fsm == null || !m_Fsm.isRunning)
        {
            return false;
        }

        bool condition1 = stateType1 != null && m_Fsm.currStateType.Equals(stateType1);
        bool condition2 = stateType2 != null && m_Fsm.currStateType.Equals(stateType2);
        bool condition3 = stateType3 != null && m_Fsm.currStateType.Equals(stateType3);
        bool condition4 = stateType4 != null && m_Fsm.currStateType.Equals(stateType4);
        bool condition5 = stateType5 != null && m_Fsm.currStateType.Equals(stateType5);
        bool condition6 = stateType6 != null && m_Fsm.currStateType.Equals(stateType6);
        return condition1 || condition2 || condition3 || condition4 || condition5 || condition6;
    }

    public bool IsAnyState(params Type[] stateTypes)
    {
        if (m_Fsm == null || !m_Fsm.isRunning || stateTypes == null || stateTypes.Length < 1)
        {
            return false;
        }

        for (int i = 0; i < stateTypes.Length; i++)
        {
            if (m_Fsm.currStateType.Equals(stateTypes[i]))
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

    public void AddState<T>() where T : BaseFsmState, new()
    {
        m_Fsm ??= FsmMgr.instance.CreateFsm(this, name);
        m_Fsm.AddState<T>();
    }

    public T GetState<T>() where T : BaseFsmState
    {
        return m_Fsm.GetState<T>();
    }

    public void SetStateData<T>(BaseEventArgs stateData) where T : BaseFsmState
    {
        m_Fsm.SetStateData<T>(stateData);
    }

    public void ChangeState<T>(BaseEventArgs stateData = null) where T : BaseFsmState
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
        if (m_HitTrigger == null || frameIndex < 0)
        {
            return;
        }

        if(m_LastTriggerAnimName == animName && m_LastTriggerFrameIndex == frameIndex)
        {
            return;
        }

        TriggerData triggerData = m_HitTrigger.GetTriggerData(animName);

        if (triggerData != null)
        {
            SetCollider(triggerData.offestList[frameIndex], triggerData.sizeList[frameIndex]);
        }

        m_LastTriggerAnimName = animName;
        m_LastTriggerFrameIndex = frameIndex;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_Fsm != null)
        {
            m_Fsm.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        if (m_Animator != null && m_Animator.animation != null && m_Animator.animation.isPlaying)
        {
            int frameCount = (int)m_Animator.animation.animations[m_CurrAnimName].frameCount;
            float duration = m_Animator.animation.animations[m_CurrAnimName].duration;
            int frameIndex = (int)(m_Animator.animation.GetState(m_CurrAnimName).currentTime * frameCount / duration);

            SetTrigger(m_CurrAnimName, frameIndex);
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

    protected string m_CurrAnimName = string.Empty;
    protected HitTrigger m_HitTrigger = null;
    protected Fsm m_Fsm = null;
    protected UnityArmatureComponent m_Animator;

    private float m_LastAnimTimeScale = 1f;
    private string m_LastTriggerAnimName = string.Empty;
    private int m_LastTriggerFrameIndex = -1;
}