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

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {

    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if(m_CanMove)
        {
            Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Dir.x, m_Dir.y, 0) * m_Owner.entityAttribute.moveSpeed * Time.deltaTime;
            m_Owner.SetPos2(ownerPos);
        }

        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir.x);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_CanChangeDir = false;
        m_CanMove = false;
    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private bool m_CanMove = false;
    private bool m_CanChangeDir = false;
    private Vector2 m_Dir = Vector2.zero;
    private BaseRole m_Owner = null;
}