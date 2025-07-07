using GameFrameWork;
using GameFrameWork.FSM;
using UnityEngine;

public class RoleSkill : BaseFsmState
{
    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {

    }

    protected override void OnUpdate(FiniteStateMachine fsm, float deltaTime, float unscaledDeltaTime)
    {
        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir.x);
        }
    }

    protected override void OnFixedUpdate(FiniteStateMachine fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
    {
        if (m_CanMove)
        {
            Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Dir.x, m_Dir.y, 0) * m_Owner.entityAttribute.moveSpeed * fixedDeltaTime;
            m_Owner.SetPos2(ownerPos);
        }
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);

        if (stateData is SkillStateData)
        {
            SkillStateData skillData = stateData as SkillStateData;
            m_CanChangeDir = skillData.canChangeDir;
            m_CanMove = skillData.canMove;
        }
        else if (stateData is MoveStateData)
        {
            MoveStateData moveData = stateData as MoveStateData;
            m_Dir = moveData.dir;
        }
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {
        m_CanChangeDir = false;
        m_CanMove = false;
        m_Dir = Vector2.zero;
    }

    protected override void OnRelease(FiniteStateMachine fsm)
    {
        m_Owner = null;
    }

    private bool m_CanMove = false;
    private bool m_CanChangeDir = false;
    private Vector2 m_Dir = Vector2.zero;
    private BaseRole m_Owner = null;
}