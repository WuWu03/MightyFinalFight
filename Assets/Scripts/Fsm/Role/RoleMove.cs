using FrameWork.Camera;
using FrameWork.Fsm;
using UnityEngine;

public class RoleMove : BaseFsmState
{
    private BaseRole m_Owner = null;
    public bool CanChangeDir;
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.PlayAnimation(AnimName.Move, -1, m_Owner.MoveSpeed * 0.2f);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (CanChangeDir)
            m_Owner.SetDir(m_Owner.MoveDir.x);
        Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Owner.MoveDir.x, m_Owner.MoveDir.y, 0) * m_Owner.MoveSpeed * Time.deltaTime;
        m_Owner.SetPos(ownerPos);
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move);
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }
}