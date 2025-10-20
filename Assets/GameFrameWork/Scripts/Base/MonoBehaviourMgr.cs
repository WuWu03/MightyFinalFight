using GameFrameWork.Event;
using UnityEngine;

namespace GameFrameWork
{
    public class MonoBehaviourMgr : BaseMgr<MonoBehaviourMgr>
    {
        public GameFrameWorkEvent<float, float, float, float> updateEvent = new();
        public GameFrameWorkEvent<float, float, float, float> lateUpdateEvent = new();
        public GameFrameWorkEvent<float, float, float, float> fixedUpdateEvent = new();

        private void Update()
        {
            updateEvent.Invoke(Time.deltaTime, Time.unscaledDeltaTime, Time.time, Time.unscaledTime);
        }

        private void LateUpdate()
        {
            lateUpdateEvent.Invoke(Time.deltaTime, Time.unscaledDeltaTime, Time.time, Time.unscaledTime);
        }

        private void FixedUpdate()
        {
            fixedUpdateEvent.Invoke(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime, Time.fixedTime, Time.fixedUnscaledTime);
        }
    }
}