using FrameWork.Fsm;

namespace Runtime
{
    public class RoleHurt : BaseFsmState,IStateParam<HurtData>
    {
        public HurtData StateParam
        {
            get;
            set;
        }

        public override void OnInit(BaseFsm fsm)
        {
            m_Owner = fsm.Owner as BaseRole;
        }

        public override void OnEnter(BaseFsm fsm)
        {
            m_Owner.PlayAnimation(AnimName.Hurt, 1,2.0f);
        }

        public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {
            if(m_Owner.IsPlayComplete())
            {
                if (m_Owner.Health <= 0)
                {
                    if (m_Owner.IsInGround)
                        ChangeState<RoleDead>(fsm);
                }
                else ChangeState<RoleIdle>(fsm);
            }
        }

        public override void OnExit(BaseFsm fsm, bool isShutdown)
        {
            m_Owner.StopAnimation(AnimName.Hurt);
        }

        public override void OnDestroy(BaseFsm fsm)
        {

        }

        private BaseRole m_Owner = null;
    }
}
