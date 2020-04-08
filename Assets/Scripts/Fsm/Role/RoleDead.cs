using FrameWork.Fsm;
using UnityEngine;

namespace Runtime
{
    public class RoleDead : BaseFsmState
    {
        public override void OnInit(BaseFsm fsm)
        {
            m_Owner = fsm.Owner as BaseRole;
        }

        public override void OnEnter(BaseFsm fsm)
        {
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            m_Owner.Rigidbody.velocity = Vector2.zero;
            m_Owner.SetPos(m_Owner.Pos);
            m_Owner.PlayAnimation(AnimName.Dead, 1, 1);
        }

        public override void OnExit(BaseFsm fsm, bool isShutdown)
        {

        }

        public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {
            if (m_Owner.IsPlayComplete())
            {
                if(m_Owner.ObjectType == ObjectType.Player)
                {
                    PlayerMgr.Ins.Rebirth();
                    return;
                }
                m_Owner.Release();
            }
        }

        public override void OnDestroy(BaseFsm fsm)
        {

        }

        private BaseRole m_Owner = null;
    }
}