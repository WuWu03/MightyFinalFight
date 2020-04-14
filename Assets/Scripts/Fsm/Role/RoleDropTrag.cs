using FrameWork.Camera;
using FrameWork.Fsm;
using UnityEngine;

public class RoleDropTrag : BaseFsmState, IStateParam<DropTragData>
{
    public DropTragData StateParam { get; set; }
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        CameraMgr.Ins.EndFollow();
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;

        //if (StateParam.CanPlayAnim)
        //    m_Owner.PlayAnimation(AnimName.JumpDown);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        //if (CameraMgr.Ins.IsOutVision(m_Owner.transform.localPosition + Vector3.up * 0.6f))
        //{
        //    if (StateParam.IsJustDead)
        //    {
        //        m_Owner.Release();
        //    }
        //    else
        //    {
        //        m_Owner.SetPos(StateParam.InitPos);
        //        ChangeState<RoleIdle>(fsm);
        //    }
        //}
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        StateParam = null;
        CameraMgr.Ins.StartFollow();
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private BaseRole m_Owner = null;
}