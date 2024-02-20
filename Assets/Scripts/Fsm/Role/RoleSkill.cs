using GameFrameWork.Fsm;
using UnityEngine;

public class RoleSkill : BaseFsmState
{
    public Vector2 dir
    {
        set
        {
            m_Dir = value;
        }
    }

    public bool canChangeDir
    {
        set
        {
            m_CanChangeDir = value;
        }
    }

    public bool canMove
    {
        set
        {
            m_CanMove = value;
        }
    }

    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(BaseFsm fsm)
    {

    }

    protected override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir.x);
        }
    }

    protected override void OnFixedUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_CanMove)
        {
            Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Dir.x, m_Dir.y, 0) * m_Owner.entityAttribute.moveSpeed * Time.deltaTime;
            m_Owner.SetPos2(ownerPos);
        }
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_CanChangeDir = false;
        m_CanMove = false;
    }

    protected override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private bool m_CanMove = false;
    private bool m_CanChangeDir = false;
    private Vector2 m_Dir = Vector2.zero;
    private BaseRole m_Owner = null;
}