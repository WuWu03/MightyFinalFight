using WuWuFramework.BehaviourTree;

public class ParallelSelector : Composite
{
    public ParallelSelector(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {

    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    public override bool CanExecute()
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

    protected override void OnChildExecuteResult(int childIndex, BehaviourTreeState state)
    {
        m_ChildrenState[childIndex] = state;
        bool isAllFailure = true;

        for (int i = 0; i < m_ChildrenState.Length; i++)
        {
            if (m_ChildrenState[i] == BehaviourTreeState.Success)
            {
                m_State = BehaviourTreeState.Success;
                return;
            }
            else if (m_ChildrenState[i] != BehaviourTreeState.Failure)
            {
                isAllFailure = true;
                break;
            }
        }

        if (isAllFailure)
        {
            m_State = BehaviourTreeState.Failure;
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
