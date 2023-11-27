using System;
using UnityEngine;
using DragonBones;
using GameFrameWork.Debug;
using System.Collections.Generic;
using GameFrameWork;
using GameFrameWork.Fsm;
using System.Linq;

public abstract class BaseAvatar : BaseGravityObject
{
    public BoxCollider2D boxCollider2D
    {
        get
        {
            return m_BoxCollider2D;
        }
    }

    public UnityArmatureComponent armatureAnimator
    {
        get
        {
            return m_Animator;
        }
    }

    public HitTrigger dragonBonesTrigger
    {
        get
        {
            return m_DragonBonesTrigger;
        }
    }

    public List<BaseSceneObject> targets
    {
        get
        {
            return m_ListTargets;
        }
    }

    public Rect bound
    {
        get
        {
            return m_Bound;
        }
    }
    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_BoxCollider2D = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_BoxCollider2D.isTrigger = true;
        m_BoxCollider2D.enabled = false;
        m_ListTargets = new List<BaseSceneObject>();
        m_FsmMachine = FsmMachine.Create(this, this.GetType().Name);
    }

    public override void UpdatePos(Vector2 pos, float posZ)
    {
        base.UpdatePos(pos, posZ);
        UpdateBound();
    }

    public Vector2 GetCurrTriggerSize()
    {
        return m_BoxCollider2D.size;
    }

    public Vector2 GetAnimTriggerSize(string animName)
    {
        if (m_DragonBonesTrigger == null)
        {
            return Vector2.zero;
        }

        TriggerData triggerData = m_DragonBonesTrigger.GetTriggerData(animName);

        if (triggerData != null)
        {
            return triggerData.Size;
        }

        return Vector2.zero;
    }

    public override void Release()
    {
        m_FsmMachine.ShutDown();
        m_ListTargets.Clear();
        m_Animator.animation.Reset();

        m_FsmMachine = null;
        m_ListTargets = null;
        m_CurrAnimName = string.Empty;

        base.Release();
    }

    protected void SetTrigger(string animName)
    {
        if (m_DragonBonesTrigger == null)
        {
            return;
        }

        TriggerData triggerData = m_DragonBonesTrigger.GetTriggerData(animName);

        if (triggerData != null)
        {
            SetCollider(triggerData.Offest, triggerData.Size);
        }
    }

    protected void UpdateBound()
    {
        m_Bound.width = m_BoxCollider2D.size.x;
        m_Bound.height = m_BoxCollider2D.size.y;
        m_Bound.xMin = m_Pos.x + m_BoxCollider2D.offset.x * m_Dir - m_BoxCollider2D.size.x / 2;
        m_Bound.xMax = m_Pos.x + m_BoxCollider2D.offset.x * m_Dir + m_BoxCollider2D.size.x / 2;
        m_Bound.yMin = m_Pos.y + m_BoxCollider2D.offset.y - m_BoxCollider2D.size.y / 2;
        m_Bound.yMax = m_Pos.y + m_BoxCollider2D.offset.y + m_BoxCollider2D.size.y / 2;
        m_Bound.center = new Vector2(m_Bound.xMin + m_Bound.width / 2, m_Bound.yMin + m_Bound.height / 2);
    }

    protected void SetCollider(Vector2 offest, Vector2 size)
    {
        m_BoxCollider2D.size = size;
        m_BoxCollider2D.offset = offest;
        UpdateBound();
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
        m_DragonBonesTrigger = m_ResGO.GetComponent<HitTrigger>();
        m_BoxCollider2D.enabled = true;
        UpdateBound();
    }

    public void PlayAnimation(string animName, int playTimes = -1, float speed = 1f)
    {
        if (m_Animator == null)
        {
            GameFrameworkLog.DebugError("Animator is invalid!");
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

    protected virtual void OnTriggerEnter2D(Collider2D collision) { }
    protected virtual void OnTriggerStay2D(Collider2D collision) { }
    protected virtual void OnTriggerExit2D(Collider2D collision) { }

    protected string m_CurrAnimName = string.Empty;
    protected Rect m_Bound = Rect.zero;
    protected BoxCollider2D m_BoxCollider2D = null;
    protected HitTrigger m_DragonBonesTrigger = null;
    protected FsmMachine m_FsmMachine = null;
    protected UnityArmatureComponent m_Animator;
    protected List<BaseSceneObject> m_ListTargets = null;
}