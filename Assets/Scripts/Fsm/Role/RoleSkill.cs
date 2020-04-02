using FrameWork.Fsm;

namespace Runtime
{
    public class RoleSkill : BaseFsmState
    {
        public override void OnInit(BaseFsm fsm)
        {
            m_Owner = fsm.Owner as BaseRole;
        }

        public override void OnEnter(BaseFsm fsm)
        {

        }

        public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {
            //if (m_Owner.ActorAnimator.animation.isCompleted)
            //{
            //   ChangeState<RoleIdle>(fsm);
            //}
        }

        public override void OnExit(BaseFsm fsm, bool isShutdown)
        {

        }

        public override void OnDestroy(BaseFsm fsm)
        {

        }

        private BaseRole m_Owner = null;
    }
}
