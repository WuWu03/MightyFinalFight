using FrameWork.Camera;
using FrameWork.Fsm;
using UnityEngine;

namespace Runtime
{
    public class RoleMove : BaseFsmState
    {
        private BaseRole m_Owner = null;

        public override void OnInit(BaseFsm fsm)
        {
            m_Owner = fsm.Owner as BaseRole;
        }

        public override void OnEnter(BaseFsm fsm)
        {
            m_Owner.PlayAnimation(AnimName.Move, -1, m_Owner.MoveSpeed * 0.2f);
        }

        public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {
            float angleY = m_Owner.transform.localRotation.eulerAngles.y;

            if (m_Owner.MoveDir.x > 0) angleY = 0;
            else if (m_Owner.MoveDir.x < 0) angleY = 180;

            Vector3 ownerPos = m_Owner.transform.localPosition;
            m_Owner.transform.localRotation = Quaternion.Euler(0, angleY, 0);
            ownerPos += new Vector3(m_Owner.MoveDir.x, m_Owner.MoveDir.y, 0) * m_Owner.MoveSpeed * Time.deltaTime;

            if (m_Owner.ObjectType == ObjectType.Player)
            {
                if (StageMgr.Ins.IsOutArea(ownerPos))
                {
                    CameraMgr.Ins.EndFollow();
                }
                else
                {
                    CameraMgr.Ins.StartFollow();
                }

                Vector2[] vision = CameraMgr.Ins.GetVision();
                bool isOutVision = ownerPos.x - 0.1f <= vision[0].x || ownerPos.x + 0.1f >= vision[1].x;
                if (!m_Owner.CanMove || !StageMgr.Ins.CanMove(ownerPos) || isOutVision) return;
            }
            m_Owner.SetPos(ownerPos);
        }

        public override void OnExit(BaseFsm fsm, bool isShutdown)
        {
            m_Owner.StopAnimation(AnimName.Move);
        }

        public override void OnDestroy(BaseFsm fsm)
        {
            
        }
    }
}
