using GameFrameWork;
using GameFrameWork.Fsm;

public class RoleAttack : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {

    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir);
        }
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);

        if(stateData is AttackStateData)
        {
            AttackStateData attackData = stateData as AttackStateData;
            m_CanChangeDir = attackData.canChangeDir;
            m_Dir = attackData.dir;
        }
        else if(stateData is MoveStateData)
        {
            MoveStateData moveData = stateData as MoveStateData;
            m_Dir = moveData.dir.x;
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_CanChangeDir = false;
        m_Dir = 0;
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private bool m_CanChangeDir = false;
    private float m_Dir = 0;
    private BaseRole m_Owner = null;
}