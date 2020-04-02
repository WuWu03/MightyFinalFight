using FrameWork.Fsm;
using UnityEngine;

namespace Runtime
{
    public class RoleJumpDown : BaseFsmState
    {
        public override void OnInit(BaseFsm fsm)
        {
            m_Owner = fsm.Owner as BaseAvatar;
        }


        public override void OnEnter(BaseFsm fsm)
        {
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;

        }

        public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {
            m_Owner.UpdatePos2(m_Owner.transform.localPosition.x, m_Owner.Pos.y);

            if (m_Owner.IsInGround)
            {
                ChangeState<RoleIdle>(fsm);
            }
        }

        public override void OnExit(BaseFsm fsm, bool isShutdown)
        {

        }

        public override void OnDestroy(BaseFsm fsm)
        {

        }

        private BaseAvatar m_Owner = null;
    }
}
