using FrameWork.Fsm;
using System;
using UnityEngine;

namespace Runtime
{
    public class RoleSwoon : BaseFsmState
    {
        public Vector2 Force 
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
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            m_Owner.Rigidbody.AddForce(Force);
            m_Owner.OnGroundEvent.AddListener(OnGround);
            m_Owner.PlayAnimation(AnimName.SmoonUp);
        }

        public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {
            m_Owner.UpdatePos2(m_Owner.transform.localPosition.x, m_Owner.Pos.y);

            //if (m_Owner.IsInGround)
            //    m_Owner.SetPos(m_Owner.Pos);
        }

        private void OnGround()
        {
            m_Owner.Rigidbody.velocity = new Vector2(m_Owner.Rigidbody.velocity.x, 0.001f);
            m_Owner.StopAnimation(AnimName.SmoonUp);
            m_Owner.PlayAnimation(AnimName.SmoonDown);
            m_Owner.SetPos(m_Owner.Pos);
            Debug.Log(m_Owner.Rigidbody.bodyType);
        }

        public override void OnExit(BaseFsm fsm, bool isShutdown)
        {
            m_Owner.StopAnimation(AnimName.SmoonDown);
        }

        public override void OnDestroy(BaseFsm fsm)
        {

        }

        
        private BaseRole m_Owner = null;
    }
}
