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
        m_FsmMachine = FsmMachine.Create(this, this.GetType().Name);
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

    public override void Release()
    {
        m_FsmMachine.ShutDown();
        m_Animator.animation.Reset();

        m_FsmMachine = null;
        m_CurrAnimName = string.Empty;

        base.Release();
    }

    public void PlayAnimation(string animName, int playTimes = -1, float speed = 1f)
    {
        if (m_Animator == null)
        {
            Log.LogError("Animator is invalid!");
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
            Log.LogError("Animator is invalid!");
            return false;
        }

        bool result = m_CurrAnimName.Equals(animName);

        if (m_Animator.animation.isCompleted)
        {
            m_CurrAnimName = string.Empty;
        }

        return result;
    }

    public void StopAnimation(string animName = null)
    {
        if (m_Animator == null)
        {
            Log.LogError("Animator is invalid!");
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

    public bool IsPlayComplete()
    {
        return m_Animator.animation.isCompleted;
    }

    public void AddAnimationEvent(string eventName, ListenerDelegate<EventObject> listener)
    {
        m_Animator.AddEventListener(eventName, listener);
    }

    public void RemoveAnimationEvent(string eventName, ListenerDelegate<EventObject> listener)
    {
        m_Animator.RemoveEventListener(eventName, listener);
    }

    public bool IsAnyState(params Type[] stateTypes)
    {
        if (m_FsmMachine == null || !m_FsmMachine.isRunning || stateTypes == null || stateTypes.Length < 1)
        {
            return false;
        }

        for (int i = 0; i < stateTypes.Length; i++)
        {
            if (m_FsmMachine.currStateType.Equals(stateTypes[i]))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCurrState<T>() where T : BaseFsmState, new()
    {
        return m_FsmMachine.currStateType == typeof(T);
    }

    public void AddState<T>() where T : BaseFsmState, new()
    {
        m_FsmMachine.AddState<T>();
    }


    public T GetState<T>() where T : BaseFsmState
    {
        return m_FsmMachine.GetState<T>();
    }

    public void ChangeState<T>(bool isForce = false) where T : BaseFsmState
    {
        m_FsmMachine.ChangeState<T>(isForce);
    }

    public void ChangeDefaultState()
    {
        m_FsmMachine.ChangeDefaultState();
    }

    public void RemoveState<T>() where T : BaseFsmState
    {
        m_FsmMachine.RemoveState<T>();
    }

    public void SetDefaultState<T>() where T : BaseFsmState
    {
        m_FsmMachine.SetDefaultState<T>();
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
        m_FsmMachine.Update(Time.deltaTime, Time.unscaledDeltaTime);

        if (m_Animator.animation.isPlaying)
        {
            int frameCount = (int)m_Animator.animation.animations[m_CurrAnimName].frameCount;
            float duration = m_Animator.animation.animations[m_CurrAnimName].duration;
            int frameIndex = (int)(m_Animator.animation.GetState(m_CurrAnimName).currentTime * frameCount / duration);

            SetTrigger(m_CurrAnimName, frameIndex);
        }
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        m_Animator = m_ResGO.GetComponent<UnityArmatureComponent>();
        m_HitTrigger = m_ResGO.GetComponent<HitTrigger>();
    }

    protected string m_CurrAnimName = string.Empty;
    protected HitTrigger m_HitTrigger = null;
    protected FsmMachine m_FsmMachine = null;
    protected UnityArmatureComponent m_Animator;
    private string m_LastTriggerAnimName = string.Empty;
    private int m_LastTriggerFrameIndex = -1;
}