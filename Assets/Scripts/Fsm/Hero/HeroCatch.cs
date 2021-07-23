using GameFrameWork.Fsm;

public class HeroCatch : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseHero;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.ActorAnimator.animation.Play(AnimName.Catch, 1);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {

    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {

    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner.ActorAnimator.animation.Stop(AnimName.Catch);
    }

    private BaseHero m_Owner = null;
}