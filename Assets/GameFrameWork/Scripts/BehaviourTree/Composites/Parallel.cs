using GameFrameWork.BehaviourTree;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : Composite
{
    public Parallel(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {

    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    public override bool CanExcute()
    {
        return m_State != BehaviourTreeState.Running;
    }

    protected override bool CanRunParallelChildren()
    {
        return true;
    }

    protected override void OnStart()
    {
        base.OnStart();
        m_ChildrenState = new BehaviourTreeState[GetChildCount()];
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        m_State = BehaviourTreeState.Running;
    }

    protected override void OnChildExcuteResult(int childIndex, BehaviourTreeState state)
    {
        m_ChildrenState[childIndex] = state;
        bool isAllSuccess = true;

        for (int i = 0; i < m_ChildrenState.Length; i++)
        {
            if (m_ChildrenState[i] == BehaviourTreeState.Failure)
            {
                m_State = BehaviourTreeState.Failure;
                return;
            }
            else if (m_ChildrenState[i] != BehaviourTreeState.Success)
            {
                isAllSuccess = false;
                break;
            }
        }

        if(isAllSuccess)
        {
            m_State = BehaviourTreeState.Success;
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_State = BehaviourTreeState.None;
    }


    private BehaviourTreeState m_State = BehaviourTreeState.None;
    private BehaviourTreeState[] m_ChildrenState = null;
}
