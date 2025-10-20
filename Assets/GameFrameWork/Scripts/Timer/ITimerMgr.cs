using GameFrameWork.Event;

namespace GameFrameWork.Timer
{
    public interface ITimerMgr
    {
        public Timer Register(float duration, GameFrameWorkAction onComplete, GameFrameWorkAction<float> onUpdate = null, bool isLooped = false, bool useRealTime = false);
        public void RegisterTimer(Timer timer);
        public void CancelAllTimers();
        public void PauseAllTimers();
        public void ResumeAllTimers();
    }
}