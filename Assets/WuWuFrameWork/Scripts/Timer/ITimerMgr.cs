using WuWuFramework.Event;

namespace WuWuFramework.Timer
{
    public interface ITimerMgr
    {
        public Timer Register(float duration, WuWuFrameworkAction onComplete, WuWuFrameworkAction<float> onUpdate = null, bool isLooped = false, bool useRealTime = false);
        public void RegisterTimer(Timer timer);
        public void CancelAllTimers();
        public void PauseAllTimers();
        public void ResumeAllTimers();
    }
}