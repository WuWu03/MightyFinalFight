using WuWuFramework.Fsm;

public class BarrelDrop : FsmState
{
    private Barrel m_Owner;
    
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.AddForce(0, 50);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.isInGround)
        {
            if (m_Owner.barrelData.moveSpeed > 0)
            {
                ChangeState<BarrelMove>(fsm);
            }
            else
            {
                ChangeState<BarrelIdle>(fsm);
            }
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {

    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }
}
