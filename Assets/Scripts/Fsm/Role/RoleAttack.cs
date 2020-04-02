using FrameWork.Fsm;
using UnityEngine;


namespace Runtime
{
    public class RoleAttack : BaseFsmState, IStateParam<AttackData>
    {
        public AttackData StateParam
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
        }

        public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {
            float angleY = m_Owner.transform.localRotation.eulerAngles.y;

            if (StateParam.Dir > 0) angleY = 0;
            else if (StateParam.Dir < 0) angleY = 180f;
            m_Owner.transform.localRotation = Quaternion.Euler(0, angleY, 0);
        }

        public override void OnExit(BaseFsm fsm, bool isShutdown)
        {
            StateParam = null;
        }

        public override void OnDestroy(BaseFsm fsm)
        {

        }

        private BaseRole m_Owner = null;
    }
}