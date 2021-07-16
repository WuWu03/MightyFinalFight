using GameFrameWork.Camera;
using GameFrameWork.Fsm;
using UnityEngine;

public class RoleMove : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        if (m_Owner.ObjectType == ObjectType.Player && (m_Owner as BaseHero).Weapon != null)
        {
            m_Owner.PlayAnimation(AnimName.Move_Weapon, 0, m_Owner.MoveSpeed * 0.2f);
        }
        else
        {
            m_Owner.PlayAnimation(AnimName.Move, 0, m_Owner.MoveSpeed * 0.2f);
        }
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Owner.MoveDir.x);
        }

        Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Owner.MoveDir.x, m_Owner.MoveDir.y, 0) * m_Owner.MoveSpeed * Time.deltaTime;
        m_Owner.SetPos(ownerPos);
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move);
        m_Owner.StopAnimation(AnimName.Move_Weapon);
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    public override void SetParam(object[] args)
    {
        m_CanChangeDir = (bool)args[0];
    }

    private bool m_CanChangeDir = false;
    private BaseRole m_Owner = null;
}