using FrameWork.Fsm;
using UnityEngine;

namespace Runtime
{
    public class RoleIdle : BaseFsmState
    {
        private BaseRole m_Owner = null;

        public override void OnInit(BaseFsm fsm)
        {
            m_Owner = fsm.Owner as BaseRole;
        }

        public override void OnEnter(BaseFsm fsm)
        {
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            m_Owner.Rigidbody.gravityScale = 1;
            m_Owner.Rigidbody.velocity = Vector2.zero;
            m_Owner.SetPos(m_Owner.Pos);
            m_Owner.PlayAnimation(AnimName.Idle, -1, 1);
            m_Owner.SetTrigger(AnimName.Idle);
        }

        public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {

        }

        public override void OnExit(BaseFsm fsm, bool isShutdown)
        {
            m_Owner.StopAnimation(AnimName.Idle);
        }
   
        public override void OnDestroy(BaseFsm fsm)
        {
            m_Owner = null;
        }
    }
}

